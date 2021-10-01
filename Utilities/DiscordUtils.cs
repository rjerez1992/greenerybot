using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeneryBOT.Utilities {
    public class DiscordUtils {
        private static DiscordEmoji CloseEmoji;

        static public string Emoji(string emoji) {
            DiscordEmoji e;
            if (DiscordEmoji.TryFromName(Client.Instance, emoji, out e))
                return e;
            else
                return DiscordEmoji.FromName(Client.Instance, ":question:");
        }

        static public async Task ReplaceOldMessage(DiscordMessageBuilder dmb, DiscordMember m, DiscordChannel ch) {
            if (SoftStorage.MessageTrack.ContainsKey(m.Id)) {
                await SoftStorage.MessageTrack[m.Id].DeleteAsync();
                SoftStorage.MessageTrack[m.Id] = dmb.SendAsync(ch).Result;
            }
            else {
                SoftStorage.MessageTrack.Add(m.Id, dmb.SendAsync(ch).Result);
            }
        }

        static public DiscordEmoji GetCloseEmoji() { 
            if(CloseEmoji == null)
                CloseEmoji = DiscordEmoji.FromName(Client.Instance, ":x:");
            return CloseEmoji;
        }
    }
}
