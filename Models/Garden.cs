using GreeneryBOT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreeneryBOT.Models {
    public class Garden {
        static public int BaseWateringValue = 15;

        public ulong GuildId;
        public ulong OwnerId;
        public string Name;
        public int Level;
        public int Experience;
        public int Capacity;
        public int Money;
        public Inventory Inventory;
        public GardenSlot[,] Slots;

        public Garden() { }

        public bool AddSlot() {
            if (Capacity >= 25)
                return false;
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    if (Slots[i, j] == null) {
                        Slots[i, j] = new GardenSlot();
                        Capacity++;
                        return true;
                    }  
                }
            }
            return false;
        }

        public void WaterSlot(int i, int j) {
            if (Slots[i, j] != null)
                Slots[i, j].WaterLevel = BaseWateringValue;
        }

        public void DoTick() {
            foreach (GardenSlot gs in Slots) {
                if (gs != null)
                    gs.DoTick();
            }
        }

        public bool Plant(ulong seedId, int row, int col) {
            ItemSeed i = (ItemSeed) Item.GetById(seedId);
            GardenSlot gs = Slots[row, col];
            if (gs != null && Inventory.RemoveOne(i)) {
                Plant p = new Plant(i.Variety);
                p.SetValuesFrom(gs);
                Slots[row, col] = p;
                return true;
            }
            else
                return false;
        }

        public bool Harvest(int row, int col, out int money, out int exp, out Plant Harvested) {
            GardenSlot gs = Slots[row, col];
            money = 0;
            exp = 0;
            Harvested = null;

            if (gs == null || !(gs is Plant))
                return false;

            Harvested = (Plant) Slots[row, col];
            Slots[row, col] = new GardenSlot();
            AddExperience(Harvested.Variety.ExperienceWorth);
            AddMoney(Harvested.Variety.MoneyWorth);
            exp = Harvested.Variety.ExperienceWorth;
            money = Harvested.Variety.MoneyWorth;
            return true;
        }

        public bool Clean(int row, int col) {
            GardenSlot gs = Slots[row, col];
            if (gs == null || !(gs is Plant) || ((Plant) gs).State != PlantState.WITHERED)
                return false;
            Slots[row, col] = new GardenSlot();
            return true;
        }

        public bool Unpest(int row, int col) {
            GardenSlot gs = Slots[row, col];
            if (gs == null || !gs.IsInfested)
                return false;
            gs.IsInfested = false;
            return true;
        }

        public bool Fertilize(ulong fertilizerItemId, int row, int col) {
            ItemFertilizer i = (ItemFertilizer) Item.GetById(fertilizerItemId);
            GardenSlot gs = Slots[row, col];
            if (gs != null && Inventory.RemoveOne(i)) {
                gs.SpeedFertilizerLevel = i.FertilizerSpeed;
                return true;
            }
            else
                return false;
        }

        public void AddExperience(int q) {
            this.Experience += q;
            if (Experience >= GetExperienceToLevelUp())
                Level++;
        }

        public int GetExperienceToLevelUp() {
            if (Level == 1)
                return 1000;
            else if (Level == 2)
                return 3000;
            else if (Level == 3)
                return 10000;
            else if (Level == 4)
                return 50000;
            return int.MaxValue;
        }

        public void AddMoney(int money) {
            this.Money += money;
        }

        public string GetStatusString() {
            bool isUnderwated = false;
            bool isInfested = false;

            foreach (GardenSlot gs in Slots) {
                if (gs != null) {
                    if (gs.IsInfested) {
                        isInfested = true;
                        break;
                    }
                    else if (gs.WaterLevel < 1) {
                        isUnderwated = true;
                        break;
                    }
                }
            }

            if (isInfested)
                return "Infested";
            else if (isUnderwated)
                return "Unwatered";
            else
                return "Thriving";
        }

        public static Garden Get(ulong gid, ulong uid, string name) {
            Garden g = SoftStorage.Gardens.FirstOrDefault(x => x.GuildId == gid && x.OwnerId == uid);
            if (g == null) {
                g = Create(gid, uid, name);
            }
            return g;
        }

        public static Garden Create(ulong gid, ulong uid, string name) {
            Garden g = new Garden {
                GuildId = gid,
                OwnerId = uid,
                Name = name + "'s garden",
                Level = 1,
                Experience = 0,
                Capacity = 5,
                Money = 1000,
                Inventory = new Inventory(),
                Slots = new GardenSlot[5, 5]
            };

            for (int i = 0; i < 5; i++) {
                g.Slots[0, i] = new GardenSlot();
            }

            SoftStorage.Gardens.Add(g);
            return g;
        }
    }
}
