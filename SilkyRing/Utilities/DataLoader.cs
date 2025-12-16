using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using SilkyRing.Enums;
using SilkyRing.Models;
using SilkyRing.Properties;

namespace SilkyRing.Utilities
{
    public static class DataLoader
    {
        public static Dictionary<string, List<Grace>> GetGraces()
        {
            Dictionary<string, List<Grace>> graceDict = new Dictionary<string, List<Grace>>();
            string csvData = Resources.Graces;

            if (string.IsNullOrWhiteSpace(csvData)) return graceDict;

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    string[] parts = line.Split(',');
                    if (parts.Length < 5) continue;

                    bool isDlc = parts[0] == "1";
                    string mainArea = parts[1];
                    string name = parts[2];
                    long graceEntityId = long.Parse(parts[3], CultureInfo.InvariantCulture);
                    int flagId = int.Parse(parts[4], CultureInfo.InvariantCulture);

                    Grace grace = new Grace
                    {
                        IsDlc = isDlc,
                        MainArea = mainArea,
                        Name = name,
                        GraceEntityId = graceEntityId,
                        FlagId = flagId
                    };

                    if (!graceDict.ContainsKey(mainArea))
                    {
                        graceDict[mainArea] = new List<Grace>();
                    }

                    graceDict[mainArea].Add(grace);
                }
            }

            return graceDict;
        }

        public static Dictionary<string, List<BossWarp>> GetBossWarps()
        {
            Dictionary<string, List<BossWarp>> warpDict = new Dictionary<string, List<BossWarp>>();
            string csvData = Resources.BossWarps;

            if (string.IsNullOrWhiteSpace(csvData)) return warpDict;

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    string[] parts = line.Split(',');

                    bool isDlc = parts[0] == "1";
                    string mainArea = parts[1];
                    string name = parts[2];
                    uint blockId = uint.Parse(parts[3], CultureInfo.InvariantCulture);

                    var coordParts = parts[4].Split('|');
                    Vector3 coords = new Vector3(
                        float.Parse(coordParts[0], CultureInfo.InvariantCulture),
                        float.Parse(coordParts[1], CultureInfo.InvariantCulture),
                        float.Parse(coordParts[2], CultureInfo.InvariantCulture)
                    );

                    float angle = float.Parse(parts[5], CultureInfo.InvariantCulture);

                    BossWarp bossWarp = new BossWarp
                    {
                        IsDlc = isDlc,
                        MainArea = mainArea,
                        Name = name,
                        Position = new Position(blockId, coords, angle)
                    };

                    if (!warpDict.ContainsKey(mainArea))
                    {
                        warpDict[mainArea] = new List<BossWarp>();
                    }

                    warpDict[mainArea].Add(bossWarp);
                }
            }

            return warpDict;
        }

        public static List<Act> GetEbActs()
        {
            List<Act> ebActs = new List<Act>();
            string csvData = Resources.EbActs;
            if (string.IsNullOrWhiteSpace(csvData)) return ebActs;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                int actIdx = int.Parse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture);
                string name = parts[1];
                ebActs.Add(new Act(actIdx, name));
            }

            return ebActs;
        }

        public static List<Weapon> GetWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();
            string csvData = Resources.Weapons;
            if (string.IsNullOrWhiteSpace(csvData)) return weapons;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                weapons.Add(new Weapon
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    WeaponType = ushort.Parse(parts[2]),
                    GemMountType = byte.Parse(parts[3]),
                    UpgradeType = byte.Parse(parts[4]),
                    CategoryName = "Weapons",
                    StackSize = 1,
                    MaxStorage = 1
                });
            }

            return weapons;
        }

        public static List<Item> GetItems(string name)
        {
            List<Item> items = new List<Item>();

            string csvData = Resources.ResourceManager.GetString(name);

            if (string.IsNullOrEmpty(csvData)) return new List<Item>();

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');


                    items.Add(new Item
                    {
                        IsDlc = byte.Parse(parts[0]) == 1,
                        Id = Convert.ToInt32(parts[1], 16),
                        Name = parts[2],
                        StackSize = int.Parse(parts[3]),
                        MaxStorage = int.Parse(parts[4]),
                        CategoryName = name
                    });
                }
            }

            return items;
        }

        public static List<EventItem> GetEventItems(string name)
        {
            List<EventItem> items = new List<EventItem>();

            string csvData = Resources.ResourceManager.GetString(name);

            if (string.IsNullOrEmpty(csvData)) return new List<EventItem>();

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');


                    items.Add(new EventItem
                    {
                        IsDlc = byte.Parse(parts[0]) == 1,
                        Id = Convert.ToInt32(parts[1], 16),
                        Name = parts[2],
                        StackSize = int.Parse(parts[3]),
                        MaxStorage = int.Parse(parts[4]),
                        NeedsEvent = byte.Parse(parts[5]) == 1,
                        EventId = int.Parse(parts[6]),
                        CategoryName = name
                    });
                }
            }

            return items;
        }

        public static List<AshOfWar> GetAshOfWars()
        {
            List<AshOfWar> aowList = new List<AshOfWar>();
            string csvData = Resources.AoW;
            if (string.IsNullOrWhiteSpace(csvData)) return aowList;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                var affinityBytes = HexStringToByteArray(parts[2]);
                var affinity = (Affinity)(affinityBytes[0] | (affinityBytes[1] << 8));

                aowList.Add(new AshOfWar
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    AvailableAffinities = affinity,
                    WeaponTypeFlags = HexStringToByteArray(parts[3])
                });
            }

            return aowList;
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        public static List<BossRevive> GetBossRevives()
        {
            List<BossRevive> bossRevives = new List<BossRevive>();
            string csvData = Resources.BossRevives;
            if (string.IsNullOrWhiteSpace(csvData)) return bossRevives;
    
            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length < 3) continue;
        
                BossRevive boss = new BossRevive
                {
                    IsDlc = parts[0] == "1",
                    BossName = parts[1],
                    IsInitializeDeadSet = parts[2] == "1"
                };
                
                List<BossFlag> flags = new List<BossFlag>();
                for (int i = 3; i < parts.Length - 1; i += 2)
                {
                    if (int.TryParse(parts[i], out int eventId))
                    {
                        bool setValue = parts[i + 1] == "1";
                        flags.Add(new BossFlag { EventId = eventId, SetValue = setValue });
                    }
                }

                boss.BossFlags = flags;
                bossRevives.Add(boss);
            }
    
            return bossRevives;
        }
    }
}