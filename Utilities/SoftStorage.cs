using DSharpPlus.Entities;
using GreeneryBOT.Enums;
using GreeneryBOT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GreeneryBOT.Utilities {
    public class SoftStorage {
        static private string _workingDirectory = Environment.CurrentDirectory + "/storage";
        static private JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        static public Dictionary<ulong, DiscordMessage> MessageTrack = new Dictionary<ulong, DiscordMessage>();

        static public List<Server> JoinedServers;
        static public List<Garden> Gardens;
        static public Seasons Season;

        static public void Load() {
            Console.WriteLine("[Storage] Loading data from files");
            Directory.CreateDirectory(_workingDirectory);

            //TODO: Do generic method for generic types <T>
            try {
                string json = File.ReadAllText(_workingDirectory + "/JoinedServers.txt");
                JoinedServers = JsonConvert.DeserializeObject<List<Server>>(json, settings);
                Console.WriteLine($"[Storage] {JoinedServers.Count} joined servers loaded");
            }
            catch (Exception e) {
                JoinedServers = new List<Server>();
                Console.WriteLine($"[Storage] Can't load JoinedServers - {e.Message}\n" +
                    $"[Storage] Initialized as empty");
            }

            try {
                string json = File.ReadAllText(_workingDirectory + "/Gardens.txt");
                Gardens = JsonConvert.DeserializeObject<List<Garden>>(json, settings);
                Console.WriteLine($"[Storage] {Gardens.Count} gardens loaded");
            }
            catch (Exception e) {
                Gardens = new List<Garden>();
                Console.WriteLine($"[Storage] Can't load Gardens - {e.Message}\n" +
                    $"[Storage] Initialized as empty");
            }

            //TODO: Load season from file
            Season = Seasons.SUMMER;
        }

        static public void Save() {
            Console.WriteLine("[Storage] Saving data to files");

            string json = JsonConvert.SerializeObject(JoinedServers, settings);
            File.WriteAllText(_workingDirectory + "/JoinedServers.txt", json);

            json = JsonConvert.SerializeObject(Gardens, settings);
            File.WriteAllText(_workingDirectory + "/Gardens.txt", json);
        }
    }
}
