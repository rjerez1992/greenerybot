using DSharpPlus.Entities;
using GreeneryBOT.Models;
using GreeneryBOT.Modules;
using GreeneryBOT.Utilities;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GreeneryBOT {
    public class Timers {
        private Timer _saveTimer;
        private int _saveInterval;

        private Timer _tickTimer;
        private int _tickInterval;

        private Timer _eventTimer;
        private int _eventInterval;

        public Timers() {
            SetTimers();
        }

        public void SetTimers() {
            Console.WriteLine("[Timers] Timers initilized");

            _saveInterval = int.Parse(ConfigurationManager.AppSettings.Get("SaveTimerInterval"));
            _saveTimer = new Timer(_saveData, null, _saveInterval * 1000, Timeout.Infinite);

            _tickInterval = int.Parse(ConfigurationManager.AppSettings.Get("TimeTickInterval"));
            _tickTimer = new Timer(_tickServer, null, _tickInterval * 1000, Timeout.Infinite);

            _eventInterval = int.Parse(ConfigurationManager.AppSettings.Get("EventTickInterval"));
            _eventTimer = new Timer(_triggerEvent, null, _eventInterval * 1000, Timeout.Infinite);

            _setStatusTrigger();
        }

        private void _setStatusTrigger() {
            Task.Delay(TimeSpan.FromMilliseconds(15 * 1000))
                .ContinueWith(task => {
                    try {
                        Client.SetStatus();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message + e.StackTrace);
                    }
                });
        }

        private void _saveData(object state) {
            try {
                SoftStorage.Save();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            finally {
                _saveTimer?.Change(_saveInterval * 1000, Timeout.Infinite);
            }
        }

        private void _tickServer(object state) {
            try {
                Console.WriteLine("[Timer] Server ticked");
                foreach (ulong gid in Client.Instance.Guilds.Keys) {
                    DiscordGuild dgd = Client.Instance.Guilds[gid];

                    if (SoftStorage.JoinedServers.FirstOrDefault(x => x.GuildId == gid) == null) {
                        Client.RecordGuild(Client.Instance.Guilds[gid]);
                    }

                    foreach (DiscordChannel vch in dgd.Channels.Values.Where(x => x.Type == DSharpPlus.ChannelType.Voice)) {
                        foreach (DiscordUser vchu in vch.Users) {
                            Garden garden = SoftStorage.Gardens.FirstOrDefault(g => g.GuildId == dgd.Id && g.OwnerId == vchu.Id);
                            if (garden != null)
                                garden.DoTick();
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            finally {
                _tickTimer?.Change(_tickInterval * 1000, Timeout.Infinite);
            }
        }

        private void _triggerEvent(object state) {
            try {
                Console.WriteLine("[Timer] Server event");
                foreach (ulong gid in Client.Instance.Guilds.Keys) {
                    DiscordGuild dgd = Client.Instance.Guilds[gid];
                    //NOTE: Checks if there are active users before sending event
                    foreach (DiscordChannel vch in dgd.Channels.Values.Where(x => x.Type == DSharpPlus.ChannelType.Voice)) {
                        if (vch.Users.Count() > 0) {
                            Server s = SoftStorage.JoinedServers.FirstOrDefault(x => x.GuildId == dgd.Id);
                            if (s != null) {
                                DiscordChannel chn = dgd.GetChannel(s.ChannelId);
                                if(chn != null)
                                    EventModule.SendEvent(dgd, chn);
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            finally {
                _eventTimer?.Change(_eventInterval * 1000, Timeout.Infinite);
            }
        }
    }
}
