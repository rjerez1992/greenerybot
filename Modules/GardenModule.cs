using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using GreeneryBOT.Models;
using GreeneryBOT.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GreeneryBOT.Modules {
    public class GardenModule : BaseCommandModule {
        static private Dictionary<ulong, Queue<string>> _harvestHistory = new Dictionary<ulong, Queue<string>>();

        //TODO: All these methods seems to do the same. Move logic to previous function and only call _build message for each if
        static public void ProcessInteraction(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            if (args[1].Equals("show"))
                _ = ShowGarden(d, e, args, member);
            else if (args[1].Equals("sow"))
                _ = ShowSow(d, e, args, member);
            else if (args[1].Equals("water"))
                _ = ShowWater(d, e, args, member);
            else if (args[1].Equals("fertilize"))
                _ = ShowFertilize(d, e, args, member);
            else if (args[1].Equals("clean"))
                _ = ShowClean(d, e, args, member);
            else if (args[1].Equals("harvest"))
                _ = ShowHarvest(d, e, args, member);
            else if (args[1].Equals("start-sow"))
                _ = StartSow(d, e, args, member);
            else if (args[1].Equals("do-sow"))
                _ = SowTile(d, e, args, member);
            else if (args[1].Equals("do-water"))
                _ = WaterTile(d, e, args, member);
            else if (args[1].Equals("start-fertilize"))
                _ = StartFertilize(d, e, args, member);
            else if (args[1].Equals("do-fertilize"))
                _ = FertilizeTile(d, e, args, member);
            else if (args[1].Equals("do-clean"))
                _ = CleanTile(d, e, args, member);
            else if (args[1].Equals("do-harvest"))
                _ = HarvestTile(d, e, args, member);
            else {
                Console.WriteLine($"Unknown garden command {e.Id}");
                _ = e.Message.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Error** - Cannot complete the action").Result;
            }
        }

        static public async Task ShowGarden(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                DiscordMessage message = await _buildGardenMessage(g, member.Id).ModifyAsync(e.Message);
                await ModuleUtils.SetCloseBackMenuOption(g, message, member);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static public async Task ShowSow(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                DiscordMessage message = await _buildSowSeedMessage(g, member.Id).ModifyAsync(e.Message);
                await ModuleUtils.SetCloseBackMenuOption(g, message, member);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static public async Task ShowWater(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                DiscordMessage message = await _buildWaterMessage(g, member.Id).ModifyAsync(e.Message);
                await ModuleUtils.SetCloseBackMenuOption(g, message, member);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static public async Task ShowFertilize(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                DiscordMessage message = await _buildFertilizeFertilizerMessage(g, member.Id).ModifyAsync(e.Message);
                await ModuleUtils.SetCloseBackMenuOption(g, message, member);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static public async Task ShowClean(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                DiscordMessage message = await _buildCleanMessage(g, member.Id).ModifyAsync(e.Message);
                await ModuleUtils.SetCloseBackMenuOption(g, message, member);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static public async Task ShowHarvest(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            try {
                Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
                DiscordMessage message = await _buildHarvestMessage(g, member.Id).ModifyAsync(e.Message);
                await ModuleUtils.SetCloseBackMenuOption(g, message, member);
            }
            catch (Exception ex) {
                _ = ModuleUtils.SendAngLogException(ex, e.Message);
            }
        }

        static private DiscordMessageBuilder _buildGardenMessage(Garden g, ulong memberId) {
            string content = ModuleUtils.GetGardenHeader(g);
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    GardenSlot gl = g.Slots[i, j];
                    if (gl == null)
                        break;
                    //NOTE: Button identifier isn't required as all buttons are disabled
                    components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"garden_empty_{memberId}_{i}_{j}", "",
                            true, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, gl.GetImage()))));
                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components);
                    components.Clear();
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildSowSeedMessage(Garden g, ulong memberId) {
            string content = $"{g.Name} - {DiscordUtils.Emoji(":inbox_tray:")} **Sowing seeds** - *Select a seed to use*\n";
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);

            List<InventoryItem> seedsAvailable = g.Inventory.GetSeeds();
            if (seedsAvailable.Count <= 0) {
                content += "> *No seeds available. Go to the shop to get some seeds*";
                return new DiscordMessageBuilder().WithContent(content);
            }
            else {
                List<DiscordComponent> components = new List<DiscordComponent>();
                for (int i = 0; i < seedsAvailable.Count && i < 50; i++) {
                    InventoryItem ii = seedsAvailable[i];
                    Item item = ItemSeed.GetById(ii.ItemId);
                    components.Add(new DiscordButtonComponent(ButtonStyle.Success, $"garden_start-sow_{memberId}_{item.Id}", $"{item.Name} - x{ii.Quantity}",
                            false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, item.Image))));
                    if ((i + 1) % 5 == 0) {
                        msgBuilder.AddComponents(components.ToArray());
                        components.Clear();
                    }
                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components.ToArray());
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildSowMessage(Garden g, ulong memberId, ulong seedItemId) {
            Item item = Item.GetById(seedItemId);
            InventoryItem ii = g.Inventory.GetByItem(item);
            if (ii == null)
                ii = new InventoryItem(item.Id, 0);

            string content = $"{g.Name} - {DiscordUtils.Emoji(":inbox_tray:")} **Sowing seeds** - *Click on an empty tile to plant*\n" +
                $"**Selected seed**: {DiscordUtils.Emoji(item.Image)} {item.Name} - Remaining: {ii.Quantity}";
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    GardenSlot gl = g.Slots[i, j];
                    if (gl == null)
                        break;
                    ButtonStyle style = gl is Plant ? ButtonStyle.Success : ButtonStyle.Secondary;
                    bool disabled = gl is Plant ? true : false;
                    components.Add(new DiscordButtonComponent(style, $"garden_do-sow_{memberId}_{seedItemId}_{i}_{j}", "",
                            disabled, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, gl.GetImage()))));

                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components);
                    components.Clear();
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildWaterMessage(Garden g, ulong memberId) {
            string content = $"{g.Name} - {DiscordUtils.Emoji(":droplet:")} **Watering** - *Click on tiles to water them*\n";
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    GardenSlot gl = g.Slots[i, j];
                    if (gl == null)
                        break;
                    ButtonStyle style = gl.WaterLevel > 0 ? ButtonStyle.Primary : ButtonStyle.Secondary;
                    bool disabled = gl.WaterLevel > 0 ? true : false;
                    components.Add(new DiscordButtonComponent(style, $"garden_do-water_{memberId}_{i}_{j}", "",
                            disabled, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, gl.GetImage()))));
                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components);
                    components.Clear();
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildFertilizeFertilizerMessage(Garden g, ulong memberId) {
            string content = $"{g.Name} - {DiscordUtils.Emoji(":sparkle:")} **Fertilize** - *Select a fertilizer to use*\n";
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);

            List<InventoryItem> fertilizersAvailable = g.Inventory.GetFertilizers();
            if (fertilizersAvailable.Count < 1) {
                content += "> *You don't have any fertilizer. Get some on the shop*";
                return new DiscordMessageBuilder().WithContent(content);
            }
            else {
                List<DiscordComponent> components = new List<DiscordComponent>();
                for (int i = 0; i < fertilizersAvailable.Count && i < 50; i++) {
                    InventoryItem ii = fertilizersAvailable[i];
                    Item item = Item.GetById(ii.ItemId);
                    components.Add(new DiscordButtonComponent(ButtonStyle.Primary, $"garden_start-fertilize_{memberId}_{item.Id}", $"{item.Name} - x{ii.Quantity}",
                            false, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, item.Image))));
                    if ((i + 1) % 5 == 0) {
                        msgBuilder.AddComponents(components.ToArray());
                        components.Clear();
                    }
                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components.ToArray());
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildFertilizeMessage(Garden g, ulong memberId, ulong fertilizerItemId) {
            Item item = Item.GetById(fertilizerItemId);
            InventoryItem ii = g.Inventory.GetByItem(item);
            if (ii == null)
                ii = new InventoryItem(item.Id, 0);

            string content = $"{g.Name} - {DiscordUtils.Emoji(":sparkles:")} **Fertilize** - *Click on an empty tile to fertilize*\n" +
                $"**Selected fertilizer**: {DiscordUtils.Emoji(item.Image)} {item.Name} - Remaining: {ii.Quantity}";
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    GardenSlot gl = g.Slots[i, j];
                    if (gl == null)
                        break;
                    ButtonStyle style = gl.SpeedFertilizerLevel != 0 ? ButtonStyle.Primary : ButtonStyle.Secondary;
                    bool disabled = gl.SpeedFertilizerLevel != 0 ? true : false;
                    components.Add(new DiscordButtonComponent(style, $"garden_do-fertilize_{memberId}_{fertilizerItemId}_{i}_{j}", "",
                            disabled, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, gl.GetImage()))));
                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components);
                    components.Clear();
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildCleanMessage(Garden g, ulong memberId) {
            string content = $"{g.Name} - {DiscordUtils.Emoji(":broom:")} **Cleaning** - *Click on tiles to clean them*\n";
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    GardenSlot gl = g.Slots[i, j];
                    if (gl == null)
                        break;

                    bool available = ((gl is Plant) && ((Plant)gl).State == PlantState.WITHERED) || gl.IsInfested;
                    ButtonStyle style = available ? ButtonStyle.Danger : ButtonStyle.Secondary;
                    bool disabled = available ? false : true;
                    string image = gl.IsInfested ? ":cricket:" : gl.GetImage();
                    components.Add(new DiscordButtonComponent(style, $"garden_do-clean_{memberId}_{i}_{j}", "",
                            disabled, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, image))));
                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components);
                    components.Clear();
                }
            }
            return msgBuilder;
        }

        static private DiscordMessageBuilder _buildHarvestMessage(Garden g, ulong memberId) {
            string content = $"{DiscordUtils.Emoji(":seedling:")} **{g.Name}** - {DiscordUtils.Emoji(":outbox_tray:")} **Harvest** - *Click on a mature plant to harvest*\n";
            
            Queue<string> _userHarvest = _harvestHistory.GetValueOrDefault(memberId);
            if (_userHarvest == null) {
                _userHarvest = new Queue<string>();
                _harvestHistory.Add(memberId, _userHarvest);
            }

            if (_userHarvest.Count > 0)
                content += $"{DiscordUtils.Emoji(":basket:")} **Harvest record**\n";
            foreach (string s in _userHarvest)
                content += s;

            var msgBuilder = new DiscordMessageBuilder()
                .WithContent(content);
            List<DiscordComponent> components = new List<DiscordComponent>();

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    GardenSlot gl = g.Slots[i, j];
                    if (gl == null)
                        break;
                    bool isPlantAndMature = gl is Plant && ((Plant)gl).State == PlantState.MATURE;
                    ButtonStyle style = isPlantAndMature ? ButtonStyle.Success : ButtonStyle.Secondary;
                    bool disabled = isPlantAndMature ? false : true;
                    components.Add(new DiscordButtonComponent(style, $"garden_do-harvest_{memberId}_{i}_{j}", "",
                            disabled, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, gl.GetImage()))));

                }
                if (components.Count > 0) {
                    msgBuilder.AddComponents(components);
                    components.Clear();
                }
            }
            return msgBuilder;
        }

        public static async Task StartSow(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            ulong seedItemId = ulong.Parse(args[3]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            DiscordMessage message = await _buildSowMessage(g, member.Id, seedItemId).ModifyAsync(e.Message);
            await ModuleUtils.SetCloseBackMenuOption(g, message, member);
        }

        public static async Task SowTile(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            ulong seedItemId = ulong.Parse(args[3]);
            int row = int.Parse(args[4]);
            int column = int.Parse(args[5]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            if (g.Plant(seedItemId, row, column))
                _ = await _buildSowMessage(g, member.Id, seedItemId).ModifyAsync(e.Message);
        }

        public static async Task WaterTile(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            int row = int.Parse(args[3]);
            int column = int.Parse(args[4]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            g.WaterSlot(row, column);
            _ = await _buildWaterMessage(g, member.Id).ModifyAsync(e.Message);
        }

        public static async Task StartFertilize(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            ulong fertilizerItemId = ulong.Parse(args[3]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            DiscordMessage message = await _buildFertilizeMessage(g, member.Id, fertilizerItemId).ModifyAsync(e.Message);
            await ModuleUtils.SetCloseBackMenuOption(g, message, member);
        }

        public static async Task FertilizeTile(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            ulong fertilizerItemId = ulong.Parse(args[3]);
            int row = int.Parse(args[4]);
            int column = int.Parse(args[5]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            if (g.Fertilize(fertilizerItemId, row, column))
                _ = await _buildFertilizeMessage(g, member.Id, fertilizerItemId).ModifyAsync(e.Message);
        }

        public static async Task CleanTile(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            int row = int.Parse(args[3]);
            int column = int.Parse(args[4]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            if (g.Unpest(row, column) || g.Clean(row, column)) {
                _ = await _buildCleanMessage(g, member.Id).ModifyAsync(e.Message);
            }
        }

        public static async Task HarvestTile(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
            int row = int.Parse(args[3]);
            int column = int.Parse(args[4]);

            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            int level = g.Level;
            if (g.Harvest(row, column, out int moneyWon, out int expWon, out Plant harvested)) {
                PlantVariety variety = PlantVariety.GetById(harvested.VarietyId);

                Queue<string> _userHarvest = _harvestHistory.GetValueOrDefault(member.Id);
                if (_userHarvest == null) {
                    _userHarvest = new Queue<string>();
                    _harvestHistory.Add(member.Id, _userHarvest);
                }

                _userHarvest.Enqueue($"You harvested {DiscordUtils.Emoji(harvested.GetImage())} {variety.Name} " +
                    $"obtaining {moneyWon} {DiscordUtils.Emoji(":moneybag:")} and {expWon} EXP {DiscordUtils.Emoji(":arrow_up:")}\n");
                if (_harvestHistory.Count > 3)
                    _userHarvest.Dequeue();

                if (g.Level > level) {
                    _ = e.Message.RespondAsync($"{member.DisplayName} your farm just **leveled up to {g.Level}** {DiscordUtils.Emoji(":tada:")}");
                }
                _ = await _buildHarvestMessage(g, member.Id).ModifyAsync(e.Message);
            }
        }
    }
}
