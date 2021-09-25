using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using GreeneryBOT.Models;
using GreeneryBOT.Modules;
using GreeneryBOT.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeneryBOT {
    public class Client {
        static public DiscordClient Instance;

        public static Task Start() {
            Instance = new DiscordClient(ClientConfig());

            _setupListeners();
            _setupInteractivity();
            _setupCommands();

            return Instance.ConnectAsync();
        }

        public static void SetStatus() {
            Console.WriteLine($"[Client] Setting status for bot");
            DiscordActivity activity = new DiscordActivity("some plants grow", ActivityType.Watching);
            _ = Instance.UpdateStatusAsync(activity);
        }

        static private DiscordConfiguration ClientConfig() {
            return new DiscordConfiguration() {
                Token = ConfigurationManager.AppSettings.Get("DiscordKey"),
                TokenType = TokenType.Bot
            };
        }

        static private void _setupCommands() {
            var commands = Instance.UseCommandsNext(new CommandsNextConfiguration() {
                StringPrefixes = new[] { ConfigurationManager.AppSettings.Get("CommandPrefix") }
            });

            commands.RegisterCommands<GenericModule>();
            //commands.RegisterCommands<GardenModule>();
            //commands.RegisterCommands<ShopModule>();
        }

        static private void _setupInteractivity() {
            Instance.UseInteractivity(new InteractivityConfiguration() {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30),
            });

            Instance.ComponentInteractionCreated += async (s, e) => {
                Console.WriteLine($"Button {e.Id} interacted by user {e.User.Id}");

                string[] args = e.Id.Split("_");
                DiscordMember member = e.Guild.GetMemberAsync(e.User.Id).Result;
                if (ulong.Parse(args[2]) != member.Id)
                    return;

                if (args[0].Equals("generic"))
                    GenericModule.ProcessInteraction(s, e, args, member);
                else if (args[0].Equals("garden"))
                    GardenModule.ProcessInteraction(s, e, args, member);
                else if (args[0].Equals("shop"))
                    ShopModule.ProcessInteraction(s, e, args, member);
                else {
                    Console.WriteLine($"[Interaction] Unknown interaction for {e.Id}");
                }

                await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            };
        }

        static private void _setupListeners() {
            Instance.GuildCreated += (dc, args) => {
                Console.WriteLine($"[Event] Joined to new server - GID:{args.Guild}");
                Server s = SoftStorage.JoinedServers.FirstOrDefault(x => x.GuildId == args.Guild.Id);
                DiscordChannel chn;
                if (s == null) {
                    chn = args.Guild.CreateTextChannelAsync("greenery").Result;
                }
                else {
                    chn = args.Guild.GetChannel(s.ChannelId);
                }                
                chn.SendMessageAsync($"{DiscordUtils.Emoji(":seedling:")} **Greenery is here!** Use *!g-help* to get information of the available commands");
                return Task.CompletedTask;
            };
        }
    }
}
