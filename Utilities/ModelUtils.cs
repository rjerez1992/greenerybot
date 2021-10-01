using DSharpPlus.Entities;
using GreeneryBOT.Enums;
using GreeneryBOT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GreeneryBOT.Utilities {
    public class ModelUtils {
        static private string _workingDirectory = Environment.CurrentDirectory + "/data";
        static private JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

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

        public static void Load() {
            Console.WriteLine("[Model] Loading stored instances");
            Directory.CreateDirectory(_workingDirectory);
            string json;

            try {
                json = File.ReadAllText(_workingDirectory + "/PlantVariety.txt");
                List<PlantVariety> plants = JsonConvert.DeserializeObject<List<PlantVariety>>(json, settings);
                Console.WriteLine($"[Model] Loaded {plants.Count} instances of PlantVariety");
            }
            catch (Exception e) {
                Console.WriteLine($"[Model] Unable to load PlantVariety. Using example values - {e.Message}");
            }

            try {
                json = File.ReadAllText(_workingDirectory + "/Item.txt");
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json, settings);
                Console.WriteLine($"[Model] Loaded {items.Count} instances of Item");
            }
            catch (Exception e) {
                Console.WriteLine($"[Model] Unable to load Item. Using example values - {e.Message}");
            }

            try {
                json = File.ReadAllText(_workingDirectory + "/Shop.txt");
                List<ShopItem> shopItems = JsonConvert.DeserializeObject<List<ShopItem>>(json, settings);
                Console.WriteLine($"[Model] Loaded {shopItems.Count} instances of ShopItem");
            }
            catch (Exception e) {
                Console.WriteLine($"[Model] Unable to load Shop. Using example values - {e.Message}");
            }
        } 
    }
}
