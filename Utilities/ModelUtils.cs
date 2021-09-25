using DSharpPlus.Entities;
using GreeneryBOT.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GreeneryBOT.Utilities {
    public class ModelUtils {

        static public string SeasonEmoji(Seasons s) {
            switch (s) {
                case Seasons.SUMMER:
                    return DiscordUtils.Emoji(":sunny:");
                case Seasons.AUTUM:
                    return DiscordUtils.Emoji(":maple_leaf:");
                case Seasons.WINTER:
                    return DiscordUtils.Emoji(":snowflake:");
                case Seasons.SPRING:
                    return DiscordUtils.Emoji(":sunflower:");
                default:
                    return DiscordUtils.Emoji(":question:");
            }
        }

        static public string SeasonName(Seasons s) {
            switch (s) {
                case Seasons.SUMMER:
                    return "Summer";
                case Seasons.AUTUM:
                    return "Autum";
                case Seasons.WINTER:
                    return "Winter";
                case Seasons.SPRING:
                    return "Spring";
                default:
                    return "Unknown";
            }
        }
    }
}
