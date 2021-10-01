using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeneryBOT.Models {
    public enum RandomEventType { FUND, FREEBIES, RAIN }

    public class RandomEvent {
        static private int EventIdSequence = 1000;

        public int Id;
        public RandomEventType Type;
        public int Money;
        public int Experience;
        public ulong ItemId;
        public int ItemQuantity;
        public Dictionary<ulong, string> ReceivedBy;
 

        public RandomEvent() {
            ReceivedBy = new Dictionary<ulong, string>();
        }

        public static RandomEvent GetEvent() {
            Random r = new Random();
            double rate = r.NextDouble();

            if (rate > 0.60) {
                return new RandomEvent {
                    Id = EventIdSequence++,
                    Type = RandomEventType.FUND,
                    Money = r.Next(1, 5) * 100,
                    Experience = r.Next(1, 10) * 100
                };
            
            }
            else if (rate > 0.20) {
                ShopItem si;
                do {
                    si = Shop.GetRandom();
                } while (si.ItemId == 1);
                return new RandomEvent {
                    Id = EventIdSequence++,
                    Type = RandomEventType.FREEBIES,
                    ItemId = si.ItemId,
                    ItemQuantity = si.ItemQuantity
                };
            }
            return new RandomEvent {
                Id = EventIdSequence++,
                Type = RandomEventType.RAIN
            };
        }
    }
}
