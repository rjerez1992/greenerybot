using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreeneryBOT.Models {
    public class Inventory {
        public List<InventoryItem> Items;

        public Inventory() {
            Items = new List<InventoryItem>();
        }

        public void Add(Item i, int quantity) {
            InventoryItem ii = Items.FirstOrDefault(x => x.Item.Id == i.Id);
            if (ii == null) {
                Items.Add(new InventoryItem(i, quantity));
            }
            else {
                ii.Quantity += quantity;
            }
        }

        public bool RemoveOne(Item i) {
            InventoryItem ii = Items.FirstOrDefault(x => x.Item.Id == i.Id);
            if (ii == null)
                return false;
            ii.Quantity--;
            if (ii.Quantity < 1)
                Items.Remove(ii);
            return true;
        }

        public InventoryItem GetByItem(Item i) {
            return Items.FirstOrDefault(x => x.Item.Id == i.Id);
        }

        public List<InventoryItem> GetSeeds() {
            return Items.Where(x => x.Item is ItemSeed).ToList();
        }

        public List<InventoryItem> GetFertilizers() {
            return Items.Where(x => x.Item is ItemFertilizer).ToList();
        }
    }

    public class InventoryItem {
        public Item Item;
        public int Quantity;

        public InventoryItem(Item i, int q) {
            Item = i;
            Quantity = q;
        }
    }
}
