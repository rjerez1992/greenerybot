using GreeneryBOT.Utilities;
using System;
using System.Threading.Tasks;

namespace GreeneryBOT {
    class Program {
        static public Timers timers;

        static void Main(string[] args) {
            timers = new Timers();
            ModelUtils.Load();
            SoftStorage.Load();
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync() {
            Console.WriteLine("[Client] Starting discord API connection");
            await Client.Start();
            await Task.Delay(-1);
        }
    }
}
