using GreeneryBOT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreeneryBOT.Models {
    public class PlantVariety {
        static private List<PlantVariety> _varieties;

        public string Name;
        public int WaterConsumption;
        public int TimeToSprout;
        public int TimeToMature;
        public int TimeToWither;
        public Seasons PreferedSeason;
        public int MoneyWorth;
        public int ExperienceWorth;
        public string Image;

        public PlantVariety() { }

        static public PlantVariety GetByName(string name) {
            return All().FirstOrDefault(x => x.Name == name);
        }

        static public IEnumerable<PlantVariety> All() {
            if (_varieties == null)
                _varieties = _all().ToList();
            return _varieties;
        }

        static private IEnumerable<PlantVariety> _all(){
            yield return new PlantVariety {
                Name = "Lettuce",
                WaterConsumption = 1,
                TimeToSprout = 30,
                TimeToMature = 90,
                TimeToWither = 180,
                PreferedSeason = Seasons.WINTER,
                MoneyWorth = 150,
                ExperienceWorth = 100,
                Image = ":leafy_green:"
            };

            yield return new PlantVariety {
                Name = "Carrot",
                WaterConsumption = 2,
                TimeToSprout = 50,
                TimeToMature = 120,
                TimeToWither = 360,
                PreferedSeason = Seasons.SPRING,
                MoneyWorth = 100,
                ExperienceWorth = 150,
                Image = ":carrot:"
            };

            yield return new PlantVariety {
                Name = "Onion",
                WaterConsumption = 3,
                TimeToSprout = 60,
                TimeToMature = 160,
                TimeToWither = 360,
                PreferedSeason = Seasons.SPRING,
                MoneyWorth = 125,
                ExperienceWorth = 200,
                Image = ":onion:"
            };

            yield return new PlantVariety {
                Name = "Potato",
                WaterConsumption = 1,
                TimeToSprout = 30,
                TimeToMature = 90,
                TimeToWither = 90,
                PreferedSeason = Seasons.SUMMER,
                MoneyWorth = 100,
                ExperienceWorth = 175,
                Image = ":potato:"
            };

            yield return new PlantVariety {
                Name = "Tomato",
                WaterConsumption = 2,
                TimeToSprout = 40,
                TimeToMature = 90,
                TimeToWither = 240,
                PreferedSeason = Seasons.SPRING,
                MoneyWorth = 325,
                ExperienceWorth = 300,
                Image = ":tomato:"
            };
        }
    }
}
