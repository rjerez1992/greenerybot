using GreeneryBOT.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreeneryBOT.Models {
    public class PlantVariety {
        static private List<PlantVariety> _varieties;

        public ulong Id;
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

        static public PlantVariety GetById(ulong id) {
            return All().FirstOrDefault(x => x.Id == id);
        }

        static public PlantVariety GetByName(string name) {
            return All().FirstOrDefault(x => x.Name == name);
        }

        static public void SetAll(List<PlantVariety> varieties) {
            _varieties = varieties;
        }

        static public IEnumerable<PlantVariety> All() {
            if (_varieties == null)
                _varieties = _all().ToList();
            return _varieties;
        }

        static private IEnumerable<PlantVariety> _all(){
            yield return new PlantVariety {
                Id = 1,
                Name = "Lettuce",
                WaterConsumption = 1,
                TimeToSprout = 3,
                TimeToMature = 9,
                TimeToWither = 18,
                PreferedSeason = Seasons.WINTER,
                MoneyWorth = 150,
                ExperienceWorth = 100,
                Image = ":leafy_green:"
            };

            yield return new PlantVariety {
                Id = 2,
                Name = "Carrot",
                WaterConsumption = 2,
                TimeToSprout = 5,
                TimeToMature = 12,
                TimeToWither = 36,
                PreferedSeason = Seasons.SPRING,
                MoneyWorth = 100,
                ExperienceWorth = 150,
                Image = ":carrot:"
            };

            yield return new PlantVariety {
                Id = 3,
                Name = "Onion",
                WaterConsumption = 3,
                TimeToSprout = 6,
                TimeToMature = 16,
                TimeToWither = 36,
                PreferedSeason = Seasons.SPRING,
                MoneyWorth = 125,
                ExperienceWorth = 200,
                Image = ":onion:"
            };

            yield return new PlantVariety {
                Id = 4,
                Name = "Potato",
                WaterConsumption = 1,
                TimeToSprout = 3,
                TimeToMature = 9,
                TimeToWither = 9,
                PreferedSeason = Seasons.SUMMER,
                MoneyWorth = 100,
                ExperienceWorth = 175,
                Image = ":potato:"
            };

            yield return new PlantVariety {
                Id = 5,
                Name = "Tomato",
                WaterConsumption = 2,
                TimeToSprout = 4,
                TimeToMature = 9,
                TimeToWither = 24,
                PreferedSeason = Seasons.SPRING,
                MoneyWorth = 325,
                ExperienceWorth = 300,
                Image = ":tomato:"
            };
        }
    }
}
