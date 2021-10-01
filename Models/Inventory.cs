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

        public void Add(ulong itemId, int quantity) {
            InventoryItem ii = Items.FirstOrDefault(x => x.ItemId == itemId);
            if (ii == null) {
                Items.Add(new InventoryItem(itemId, quantity));
            }
            else {
                ii.Quantity += quantity;
            }
        }

        public bool RemoveOne(Item i) {
            InventoryItem ii = Items.FirstOrDefault(x => x.ItemId == i.Id);
            if (ii == null)
                return false;
            ii.Quantity--;
            if (ii.Quantity < 1)
                Items.Remove(ii);
            return true;
        }

        public InventoryItem GetByItem(Item i) {
            return Items.FirstOrDefault(x => x.ItemId == i.Id);
        }

        public List<InventoryItem> GetSeeds() {
            return Items.Where(x => Item.GetById(x.ItemId) is ItemSeed).ToList();
        }

        public List<InventoryItem> GetFertilizers() {
            return Items.Where(x => Item.GetById(x.ItemId) is ItemFertilizer).ToList();
        }
    }

    public class InventoryItem {
        public ulong ItemId;
        public int Quantity;

        public InventoryItem(ulong itemId, int q) {
            ItemId = itemId;
            Quantity = q;
        }
    }
}
