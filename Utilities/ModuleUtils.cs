using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using GreeneryBOT.Models;
using GreeneryBOT.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeneryBOT.Utilities {
    public class ModuleUtils {

        public static async Task SetCloseBackMenuOption(Garden g, DiscordMessage msg, DiscordMember member) {
            _ = msg.CreateReactionAsync(DiscordUtils.GetCloseEmoji());
            //TODO: Set timeout as app.config variaable
            var result = await msg.WaitForReactionAsync(member, DiscordUtils.GetCloseEmoji(), TimeSpan.FromSeconds(60));
            if (!result.TimedOut) {
                _ = msg.DeleteAllReactionsAsync();
                await SendBackToMenu(g, member.Id, msg);
            }
        }

        public static async Task SendBackToMenu(Garden g, ulong memberId, DiscordMessage msg) {
            DiscordMessageBuilder msgBuilder = GenericModule.BuildMenuMessage(g, memberId);
            await msgBuilder.ModifyAsync(msg);
        }

        public static async Task SendAngLogException(Exception e, DiscordMessage msg) {
            Console.WriteLine(e.Message + e.StackTrace);
            await msg.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Error** - Cannot complete the action");
        }

        public static string GetGardenHeader(Garden g) {
            return $"{DiscordUtils.Emoji(":seedling:")} **{g.Name}** - " +
                    $"{DiscordUtils.Emoji(":star:")} **Level {g.Level}** - " +
                    $"{DiscordUtils.Emoji(":arrow_up:")} **Exp. {g.Experience}/{g.GetExperienceToLevelUp()}**\n" +
                    $"{DiscordUtils.Emoji(":moneybag:")} **Money**: {g.Money} - " +
                    $"{DiscordUtils.Emoji(":mag:")} **Status**: {g.GetStatusString()}";
        }
    }
}
