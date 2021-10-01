using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreeneryBOT.Models {
    public enum ItemType { INSTANT, CONSUMABLE, OTHER }

    public class Item {
        static private List<Item> _items;

        public ulong Id;
        public string Name;
        public string Description;
        public string Image;
        public ItemType Type;
        public int PricePerUnit;
        public Item() { }

        static public Item GetById(ulong id) {
            return All().FirstOrDefault(x => x.Id == id);
        }

        static public void SetAll(List<Item> items) {
            _items = items;        
        }

        static public List<Item> All() {
            if (_items == null)
                _items = _all().ToList();
            return _items;
        }

        static private IEnumerable<Item> _all() {
            yield return new Item {
                Id = 1,
                Name = "Garden expansion",
                Image = ":golf:",
                Description = "Gives one slot more to the garden. Maximum of 25 slots",
                Type = ItemType.INSTANT,
                PricePerUnit = 1000
            };
            yield return new ItemFertilizer {
                Id = 2,
                Name = "Speed fertilizer",
                Image = ":oil:",
                Description = "Improves growing time for the plants. Lasts until the plant is removed",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 25,
                FertilizerSpeed = 1.20f
            };
            yield return new ItemFertilizer {
                Id = 3,
                Name = "Speed+ fertilizer",
                Image = ":oil:",
                Description = "Highly improves growing time for the plants. Lasts until the plant is removed",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 75,
                FertilizerSpeed = 1.5f
            };
            yield return new ItemSeed {
                Id = 4,
                Name = "Lettuce Seed",
                Image = ":leafy_green:",
                Description = "A lettuce seed. Plant it to grow a lettuce",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 40,
                Variety = PlantVariety.GetByName("Lettuce")
            };
            yield return new ItemSeed {
                Id = 5,
                Name = "Carrot Seed",
                Image = ":carrot:",
                Description = "A carrot seed. Plant it to grow a carrot",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 15,
                Variety = PlantVariety.GetByName("Carrot")
            };
            yield return new ItemSeed {
                Id = 6,
                Name = "Onion Seed",
                Image = ":onion:",
                Description = "An onion seed. Plant it to grow an onion",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 50,
                Variety = PlantVariety.GetByName("Onion")
            };
            yield return new ItemSeed {
                Id = 7,
                Name = "Potato Seed",
                Image = ":potato:",
                Description = "A potato seed. Plant it to grow a potato",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 10,
                Variety = PlantVariety.GetByName("Potato")
            };
            yield return new ItemSeed {
                Id = 8,
                Name = "Tomato Seed",
                Image = ":tomato:",
                Description = "A tomato seed. Plant it to grow a tomato",
                Type = ItemType.CONSUMABLE,
                PricePerUnit = 115,
                Variety = PlantVariety.GetByName("Tomato")
            };
        }
    }

    public class ItemSeed : Item {
        public PlantVariety Variety;

        public ItemSeed() { }
    }

    public class ItemFertilizer : Item {
        public float FertilizerSpeed;

        public ItemFertilizer() { }
    }
}
