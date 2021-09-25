using System;
using System.Collections.Generic;
using System.Text;

namespace GreeneryBOT.Models {
    public class GardenSlot {
        static public string DefaultImage = ":hole:";
        static public string DefaultInfestedImage = ":cricket:";

        static protected double InfestChance = 0.01;
        static protected Random Random = new Random();

        public int WaterLevel;
        public bool IsInfested;
        public float SpeedFertilizerLevel;

        public GardenSlot() {
            WaterLevel = 0;
            IsInfested = false;
            SpeedFertilizerLevel = 0;
        }

        public virtual void DoTick() {
            WaterLevel -= 1;
            if (WaterLevel < 0)
                WaterLevel = 0;

            if (!IsInfested && InfestChance > Random.NextDouble())
                IsInfested = true;
        }

        public virtual string GetImage() {
            if (IsInfested)
                return DefaultInfestedImage;
            return DefaultImage;
        }

        public void SetValuesFrom(GardenSlot gs) {
            WaterLevel = gs.WaterLevel;
            IsInfested = gs.IsInfested;
            SpeedFertilizerLevel = gs.SpeedFertilizerLevel;
        }
    }
}
