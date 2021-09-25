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
using System.Threading.Tasks;

namespace GreeneryBOT.Modules {
    public class GenericModule : BaseCommandModule {
        [Command("menu"), Description("Shows the menu of the mini game. Allows to interact with all options")]
        public async Task MenuCommand(CommandContext ctx) {
            try {
                Garden g = Garden.Get(ctx.Guild.Id, ctx.Member.Id, ctx.Member.DisplayName);
                DiscordMessageBuilder msgBuilder = BuildMenuMessage(g, ctx.Member.Id);
                _ = ctx.Message.DeleteAsync();
                await DiscordUtils.ReplaceOldMessage(msgBuilder, ctx.Member, ctx.Channel);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, ctx.Message);
            }
        }

        [Command("name"), Description("Allows to change the name of your garden")]
        public async Task NameCommand(CommandContext ctx, string name) {
            try {
                Garden g = Garden.Get(ctx.Guild.Id, ctx.Member.Id, ctx.Member.DisplayName);
                g.Name = name;
                await ctx.RespondAsync($"{DiscordUtils.Emoji(":seedling:")} {ctx.Member.Mention} your garden has been renamed to **{name}**");
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, ctx.Message);
            }
        }

        static public void ProcessInteraction(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            if (args[1].Equals("guide"))
                _ = ShowGuide(d, e, args, member);
            else {
                Console.WriteLine($"Unknown generic command {e.Id}");
                _ = e.Message.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Error** - Cannot complete the action").Result;
            }
        }

        static public async Task ShowGuide(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                //Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                //DiscordMessage message = await _buildWaterMessage(g, member.Id).ModifyAsync(e.Message);
                //await ModuleUtils.SetCloseBackMenuOption(g, message, member);
                await new Task(() => { Console.WriteLine("Method not implemented yet"); });
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static public DiscordMessageBuilder BuildMenuMessage(Garden g, ulong memberId) {
            string content = ModuleUtils.GetGardenHeader(g);
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_show_{memberId}", $"Garden",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":seedling:"))));
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"generic_guide_{memberId}", $"Guide",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":blue_book:"))));

            msgBuilder.AddComponents(components.ToArray());
            components.Clear();

            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_sow_{memberId}", $"Sow",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":inbox_tray:"))));
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_water_{memberId}", $"Water",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":droplet:"))));
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_fertilize_{memberId}", $"Fertilize",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":sparkles:"))));
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_clean_{memberId}", $"Clean",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":broom:"))));
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_harvest_{memberId}", $"Harvest",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":outbox_tray:"))));

            msgBuilder.AddComponents(components.ToArray());
            components.Clear();

            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"shop_show_{memberId}", $"Shop",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":shopping_cart:"))));
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"shop_bag_{memberId}", $"Bag",
                        false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, ":handbag:"))));

            msgBuilder.AddComponents(components.ToArray());
            return msgBuilder;
        }
    }
}
