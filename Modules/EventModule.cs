using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using GreeneryBOT.Models;
using GreeneryBOT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreeneryBOT.Modules {
    public class EventModule : BaseCommandModule {
        static private Dictionary<int, RandomEvent> _eventsRecord = new Dictionary<int, RandomEvent>();

        static public void ProcessInteraction(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            //NOTE: Member will always be null for this interaction
            if (args[1].Equals("fund"))
                _ = ReceiveFund(d, e, args);
            else if (args[1].Equals("freebies"))
                _ = ReceiveFreebies(d, e, args);
            else {
                Console.WriteLine($"Unknown event command {e.Id}");
                _ = e.Message.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Error** - Cannot complete the action").Result;
            }
        }

        static public void SendEvent(DiscordGuild guild, DiscordChannel channel) {
            RandomEvent ev = RandomEvent.GetEvent();
            DiscordMessageBuilder msgb;
            DiscordMessage msg;
            switch (ev.Type) {
                case RandomEventType.FUND:
                    msgb = _buildEventFundMessage(ev);
                    msg = msgb.SendAsync(channel).Result;
                    Task.Delay(TimeSpan.FromMilliseconds(60 * 1000))
                    .ContinueWith(task => {
                        try {
                            _eventsRecord.Remove(ev.Id);
                            msgb = _buildEventFundMessage(ev, true);
                            msgb.ModifyAsync(msg);
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message + e.StackTrace);
                        }
                    });
                    _eventsRecord.Add(ev.Id, ev);
                    break;
                case RandomEventType.FREEBIES:
                    msgb = _buildEventFreebiesMessage(ev);
                    msg = msgb.SendAsync(channel).Result;
                    Task.Delay(TimeSpan.FromMilliseconds(60 * 1000))
                    .ContinueWith(task => {
                        try {
                            _eventsRecord.Remove(ev.Id);
                            msgb = _buildEventFreebiesMessage(ev, true);
                            msgb.ModifyAsync(msg);
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message + e.StackTrace);
                        }
                    });
                    _eventsRecord.Add(ev.Id, ev);
                    break;
                case RandomEventType.RAIN:
                    channel.SendMessageAsync($"{DiscordUtils.Emoji(":performing_arts:")} **Event**\n" +
                        $"{DiscordUtils.Emoji(":cloud_rain:")} The rain has blessed the fields. All the gardens are super watered, fertilized and cleaned.");
                    foreach (Garden g in SoftStorage.Gardens.Where(x => x.GuildId == guild.Id)) {
                        g.RainBless();
                    }
                    break;
            }
        }

        static private DiscordMessageBuilder _buildEventFundMessage(RandomEvent re, bool ended = false) {
            string content = $"{DiscordUtils.Emoji(":performing_arts:")} **Event**\n" +
                $"{DiscordUtils.Emoji(":rocket:")} There is a fund for farmers available. You just need to apply to obtain " +
                $"{re.Money} {DiscordUtils.Emoji(":moneybag:")} and {re.Experience} {DiscordUtils.Emoji(":arrow_up:")}\n";

            if (ended) {
                content += $"**The event already ended**";
            }
            else { 
                content += $"Hurry up! It is just for a limited time";
            }

            if (re.ReceivedBy.Count > 0) {
                content += $"\n**Received by:**\n";
                content += String.Join(", ", re.ReceivedBy.Values);
            }

            var msgBuilder = new DiscordMessageBuilder().WithContent(content);

            if (!ended) {
                List<DiscordComponent> components = new List<DiscordComponent>();
                components.Add(new DiscordButtonComponent(ButtonStyle.Success, $"event_fund_{re.Id}", $"Apply",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":+1:"))));
                msgBuilder.AddComponents(components.ToArray());
            }

            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildEventFreebiesMessage(RandomEvent re, bool ended = false) {
            Item i = Item.GetById(re.ItemId);
            string content = $"{DiscordUtils.Emoji(":performing_arts:")} **Event**\n" +
                $"{DiscordUtils.Emoji(":mailbox:")} Someone sent x{re.ItemQuantity} {DiscordUtils.Emoji(i.Image)} {i.Name} to your mailbox. Be sure to claim them\n";

            if (ended) {
                content += $"**The event already ended**";
            }
            else {
                content += $" Hurry up! It is just for a limited time";
            }

            if (re.ReceivedBy.Count > 0) {
                content += $"\n**Received by:**\n";
                content += String.Join(", ", re.ReceivedBy.Values);
            }

            var msgBuilder = new DiscordMessageBuilder().WithContent(content);

            if (!ended) {
                List<DiscordComponent> components = new List<DiscordComponent>();
                components.Add(new DiscordButtonComponent(ButtonStyle.Success, $"event_freebies_{re.Id}", $"Receive",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":+1:"))));
                msgBuilder.AddComponents(components.ToArray());
            }

            return msgBuilder;
        }

        public static async Task ReceiveFund(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args) {
            int eventId = int.Parse(args[2]);
            RandomEvent re = _eventsRecord.GetValueOrDefault(eventId);
            if (re == null || re.ReceivedBy.GetValueOrDefault(e.User.Id) != null)
                return;
            Garden g = Garden.Get(e.Guild.Id, e.User.Id, e.User.Username);
            g.AddMoney(re.Money);
            g.AddExperience(re.Experience);
            re.ReceivedBy.Add(g.OwnerId, g.Name);
            _ = await _buildEventFundMessage(re).ModifyAsync(e.Message);
        }

        public static async Task ReceiveFreebies(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args) {
            int eventId = int.Parse(args[2]);
            RandomEvent re = _eventsRecord.GetValueOrDefault(eventId);
            if (re == null || re.ReceivedBy.GetValueOrDefault(e.User.Id) != null)
                return;
            Garden g = Garden.Get(e.Guild.Id, e.User.Id, e.User.Username);
            g.Inventory.Add(re.ItemId, re.ItemQuantity);
            re.ReceivedBy.Add(g.OwnerId, g.Name);
            _ = await _buildEventFreebiesMessage(re).ModifyAsync(e.Message);
        }
    }
}
