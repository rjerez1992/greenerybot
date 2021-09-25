using System;
using System.Collections.Generic;
using System.Text;

namespace GreeneryBOT.Models {
    public enum PlantState { SEED, SEEDLING, MATURE, WITHERED }

    public class Plant : GardenSlot {
        static public string SeedImage = ":chestnut:";
        static public string SeedlingImage = ":seedling:";
        static public string WitheredImage = ":wilted_rose:";

        static public int TimePerTick = 10;

        public PlantVariety Variety;
        public int AliveTime;
        public PlantState State;

        public Plant(PlantVariety variety) {
            Variety = variety;
            AliveTime = 0;
            State = PlantState.SEED;
        }

        public override void DoTick() {
            int calculatedTimePerTick = (int) (TimePerTick * SpeedFertilizerLevel);
            WaterLevel -= Variety.WaterConsumption;
            if (WaterLevel < 0)
                WaterLevel = 0;

            if (!IsInfested) {
                if (State == PlantState.SEED || State == PlantState.SEEDLING) {
                    if (WaterLevel > 0)
                        AliveTime += calculatedTimePerTick;
                }
                else if (State == PlantState.MATURE) {
                    if (WaterLevel > 0)
                        AliveTime += TimePerTick;
                    else
                        AliveTime += TimePerTick * 3;
                }
                else {
                    AliveTime += TimePerTick;
                }
            }
            if (!IsInfested && InfestChance > Random.NextDouble())
                IsInfested = true;
            CheckState();
        }

        private void CheckState() {
            if (State == PlantState.SEED && AliveTime > Variety.TimeToSprout)
                State = PlantState.SEEDLING;
            else if (State == PlantState.SEEDLING && AliveTime > Variety.TimeToMature)
                State = PlantState.MATURE;
            else if (State == PlantState.MATURE && AliveTime > Variety.TimeToWither)
                State = PlantState.WITHERED;
        }

        public override string GetImage() {
            if (IsInfested)
                return DefaultInfestedImage;
            switch (State) {
                case PlantState.SEED:
                    return SeedImage;
                case PlantState.SEEDLING:
                    return SeedlingImage;
                case PlantState.MATURE:
                    return Variety.Image;
                case PlantState.WITHERED:
                    return WitheredImage;
            }
            return Variety.Image;
        }
    }
}
