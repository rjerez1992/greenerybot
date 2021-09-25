using GreeneryBOT.Models;
using GreeneryBOT.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreeneryBOT {
    public class Timers {
        private Timer _saveTimer;
        private int _saveInterval;

        private Timer _tickTimer;
        private int _tickInterval;

        public Timers() {
            SetTimers();
        }

        public void SetTimers() {
            Console.WriteLine("[Timers] Timers initilized");

            _saveInterval = int.Parse(ConfigurationManager.AppSettings.Get("SaveTimerInterval"));
            _saveTimer = new Timer(SaveData, null, _saveInterval * 1000, Timeout.Infinite);

            _tickInterval = int.Parse(ConfigurationManager.AppSettings.Get("TimeTickInterval"));
            _tickTimer = new Timer(TickServer, null, _tickInterval * 1000, Timeout.Infinite);

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

        private void SaveData(object state) {
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

        private void TickServer(object state) {
            try {
                //TODO: Tick based on user activity
                Console.WriteLine("[Timer] Server ticked");
                foreach (Garden g in SoftStorage.Gardens) {
                    g.DoTick();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            finally {
                _tickTimer?.Change(_tickInterval * 1000, Timeout.Infinite);
            }
        }
    }
}
