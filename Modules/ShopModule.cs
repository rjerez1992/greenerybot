using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using GreeneryBOT;
using GreeneryBOT.Models;
using GreeneryBOT.Utilities;

public class ShopModule : BaseCommandModule {
    static private Dictionary<ulong, Queue<string>> _purschaseHistory = new Dictionary<ulong, Queue<string>>();

    static public void ProcessInteraction(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
        if (args[1].Equals("show"))
            _ = ShowShop(d, e, args, member);
        else if (args[1].Equals("buy"))
            _ = BuyItem(d, e, args, member);
        else if (args[1].Equals("bag"))
            _ = ShowBag(d, e, args, member);
        else {
            Console.WriteLine($"Unknown generic command {e.Id}");
            _ = e.Message.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Error** - Cannot complete the action").Result;
        }
    }

    static public async Task ShowShop(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
        try {
            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            Queue<string> memberHistory = _purschaseHistory.GetValueOrDefault(member.Id);
            if (memberHistory == null) {
                memberHistory = new Queue<string>();
                _purschaseHistory.Add(member.Id, memberHistory);
            }
            memberHistory.Clear();           
            DiscordMessage message = await _buildShopMessage(g, member.Id).ModifyAsync(e.Message);
            await ModuleUtils.SetCloseBackMenuOption(g, message, member);
        }
        catch (Exception ex) {
            _ = ModuleUtils.SendAngLogException(ex, e.Message);
        }
    }

    static public async Task ShowBag(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
        try {
            Garden g = Garden.Get(e.Guild.Id, e.User.Id, member.DisplayName);
            DiscordMessage message = await _buildBagMessage(g, member.Id).ModifyAsync(e.Message);
            await ModuleUtils.SetCloseBackMenuOption(g, message, member);
        }
        catch (Exception ex) {
            _ = ModuleUtils.SendAngLogException(ex, e.Message);
        }
    }

    static private DiscordMessageBuilder _buildShopMessage(Garden g, ulong memberId) {
        string content = $"{DiscordUtils.Emoji(":seedling:")} **{g.Name}** - {DiscordUtils.Emoji(":shopping_cart:")} **Shop** - *Click on an item to buy* - " +
            $"{DiscordUtils.Emoji(":moneybag:")} {g.Money} money\n";

        if (_purschaseHistory.Count > 0)
            content += $"{DiscordUtils.Emoji(":shopping_bags:")} **Purschase record**\n";
        foreach (string s in _purschaseHistory.GetValueOrDefault(memberId))
            content += s;

        var msgBuilder = new DiscordMessageBuilder()
            .WithContent(content);

        List<DiscordComponent> components = new List<DiscordComponent>();
        int maxItems = Shop.AllItems().Count;

        for (int i = 0; i < 50 && i < maxItems; i++) {
            ShopItem si = Shop.AllItems()[i];
            bool disabled = si.Price() > g.Money ? true : false;
            components.Add(new DiscordButtonComponent(ButtonStyle.Secondary, $"shop_buy_{memberId}_{si.Id}", $"{si.Name} - ${si.Price()}",
                    disabled, new DiscordComponentEmoji(DiscordEmoji.FromName(Client.Instance, si.Image))));
            if ((i + 1) % 5 == 0) {
                msgBuilder.AddComponents(components.ToArray());
                components.Clear();
            }
        }

        if (components.Count > 0) {
            msgBuilder.AddComponents(components.ToArray());
        }
        return msgBuilder;
    }

    static private DiscordMessageBuilder _buildBagMessage(Garden g, ulong memberId) {
        string content = $"{g.Name} - {DiscordUtils.Emoji(":handbag:")} **Bag** - *Items you have available*\n";
   
        for (int i = 0; i < g.Inventory.Items.Count; i++) {
            InventoryItem ii = g.Inventory.Items[i];
            Item item = Item.GetById(ii.ItemId);
            content += $"\n> {DiscordUtils.Emoji(item.Image)} **{item.Name}** - x{ii.Quantity}\n" +
                $"> {item.Description}";
        }

        if (g.Inventory.Items.Count <= 0) {
            content += "> *Your bag is empty*";
        }

        var msgBuilder = new DiscordMessageBuilder()
            .WithContent(content);
        return msgBuilder;
    }

    static public async Task BuyItem(DiscordClient d, ComponentInteractionCreateEventArgs e, string[] args, DiscordMember member) {
        Garden g = Garden.Get(e.Guild.Id, member.Id, member.DisplayName);
        int shopItemId = int.Parse(args[3]);
        ShopItem si = Shop.GetById(shopItemId);
        if (g.Money >= si.Price()) {
            if (si.AddSingleTo(g)) {
                g.Money -= si.Price();
                Queue<string> memberHistory = _purschaseHistory.GetValueOrDefault(member.Id);
                memberHistory.Enqueue($"{DiscordUtils.Emoji(":small_blue_diamond:")} You bought {DiscordUtils.Emoji(si.Image)} {si.Name} for {DiscordUtils.Emoji(":moneybag:")} {si.Price()}\n");
                if (memberHistory.Count > 3)
                    memberHistory.Dequeue();
                await _buildShopMessage(g, member.Id).ModifyAsync(e.Message);                
                return;
            }
            await e.Message.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Warning** - Can't process purschase");
        }
        else {
            await e.Message.RespondAsync($"{DiscordUtils.Emoji(":warning:")} **Warning** - Not enough money");
        }
    }
}