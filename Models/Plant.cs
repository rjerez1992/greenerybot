using System;
using System.Collections.Generic;
using System.Text;

namespace GreeneryBOT.Models {
    public enum PlantState { SEED, SEEDLING, MATURE, WITHERED }

    public class Plant : GardenSlot {
        static public string SeedImage = ":chestnut:";
        static public string SeedlingImage = ":seedling:";
        static public string WitheredImage = ":wilted_rose:";

        static public int TimePerTick = 1;

        public ulong VarietyId;
        public int AliveTime;
        public PlantState State;

        public Plant(ulong varietyId) {
            VarietyId = varietyId;
            AliveTime = 0;
            State = PlantState.SEED;
        }

        public override void DoTick() {
            int calculatedTimePerTick = (int) (TimePerTick * SpeedFertilizerLevel);
            PlantVariety variety = PlantVariety.GetById(VarietyId);
            WaterLevel -= variety.WaterConsumption;
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
            PlantVariety variety = PlantVariety.GetById(VarietyId);
            if (State == PlantState.SEED && AliveTime > variety.TimeToSprout)
                State = PlantState.SEEDLING;
            else if (State == PlantState.SEEDLING && AliveTime > variety.TimeToMature)
                State = PlantState.MATURE;
            else if (State == PlantState.MATURE && AliveTime > variety.TimeToWither)
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
                    PlantVariety variety = PlantVariety.GetById(VarietyId);
                    return variety.Image;
                case PlantState.WITHERED:
                    return WitheredImage;
            }
            return DefaultInfestedImage;
        }
    }
}
