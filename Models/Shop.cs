using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreeneryBOT.Models {
    public class Shop {
        static private List<ShopItem> _shopItems;

        static public ShopItem GetById(int id) {
            return _shopItems.FirstOrDefault(x => x.Id == id);
        }

        static public ShopItem GetByName(string name) {
            return _shopItems.FirstOrDefault(x => x.Name == name);
        }

        static public List<ShopItem> AllItems() {
            if (_shopItems == null)
                _shopItems = _all().ToList();
            return _shopItems;
        }

        static public IEnumerable<ShopItem> _all() {
            yield return new ShopItem {
                Id = 1,
                Name = "Garden expansion",
                Image = ":golf:",
                Description = "Increases garden spaces by 1. Maximum of 25 spaces",
                Item = Item.GetById(1),
                ItemQuantity = 1,
            };
            yield return new ShopItem {
                Id = 2,
                Name = "Speed fertilizer pack (x10)",
                Image = ":oil:",
                Description = "A pack of 10 speed fertilizers for your garden",
                Item = Item.GetById(2),
                ItemQuantity = 10,
            };
            yield return new ShopItem {
                Id = 3,
                Name = "Speed+ fertilizer pack (x10)",
                Image = ":oil:",
                Description = "A pack of 10 faster speed fertilizers for your garden",
                Item = Item.GetById(3),
                ItemQuantity = 10,
            };
            yield return new ShopItem {
                Id = 4,
                Name = "Lettuce seed pack (x10)",
                Image = ":package:",
                Description = "A pack of 10 lettuce seeds to grow in your garden",
                Item = Item.GetById(4),
                ItemQuantity = 10,
            };
            yield return new ShopItem {
                Id = 5,
                Name = "Carrot seed pack (x10)",
                Image = ":package:",
                Description = "A pack of 10 carrot seeds to grow in your garden",
                Item = Item.GetById(5),
                ItemQuantity = 10,
            };
            yield return new ShopItem {
                Id = 6,
                Name = "Onion seed pack (x10)",
                Image = ":package:",
                Description = "A pack of 10 onion seeds to grow in your garden",
                Item = Item.GetById(6),
                ItemQuantity = 10,
            };
            yield return new ShopItem {
                Id = 7,
                Name = "Potato seed pack (x10)",
                Image = ":package:",
                Description = "A pack of 10 potato seeds to grow in your garden",
                Item = Item.GetById(7),
                ItemQuantity = 10,
            };
            yield return new ShopItem {
                Id = 8,
                Name = "Tomato seed pack (x10)",
                Image = ":package:",
                Description = "A pack of 10 tomato seeds to grow in your garden",
                Item = Item.GetById(8),
                ItemQuantity = 10,
            };
        }
    }

    public class ShopItem {
        public int Id;
        public string Name;
        public string Image;
        public string Description;
        public Item Item;
        public int ItemQuantity;

        public int Price() {
            return ItemQuantity * Item.PricePerUnit;
        }

        public bool AddSingleTo(Garden g) {
            if (Item.Type == ItemType.INSTANT) {
                if (Item.Id == 1) {
                    return g.AddSlot();
                }
            }
            else {
                g.Inventory.Add(Item, ItemQuantity);
                return true;
            }
            return false;
        }
    }
}
