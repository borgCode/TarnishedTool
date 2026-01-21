using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using TarnishedTool.Enums;
using TarnishedTool.GameIds;
using TarnishedTool.Models;
using TarnishedTool.Properties;

namespace TarnishedTool.Utilities
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
                    Id = int.Parse(parts[0], CultureInfo.InvariantCulture),
                    Name = parts[1],
                    WeaponType = ushort.Parse(parts[2], CultureInfo.InvariantCulture),
                    GemMountType = byte.Parse(parts[3], CultureInfo.InvariantCulture),
                    UpgradeType = byte.Parse(parts[4], CultureInfo.InvariantCulture),
                    CategoryName = "Weapons",
                    StackSize = 1,
                    MaxStorage = 1
                });
            }

            return weapons;
        }

        public static List<Item> GetItems(string resourceName, string category)
        {
            List<Item> items = new List<Item>();

            string csvData = Resources.ResourceManager.GetString(resourceName);

            if (string.IsNullOrEmpty(csvData)) return new List<Item>();

            using (StringReader reader = new StringReader(csvData))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = ParseCsvLine(line);


                    items.Add(new Item
                    {
                        IsDlc = byte.Parse(parts[0], CultureInfo.InvariantCulture) == 1,
                        Id = Convert.ToInt32(parts[1], 16),
                        Name = parts[2],
                        StackSize = int.Parse(parts[3], CultureInfo.InvariantCulture),
                        MaxStorage = int.Parse(parts[4], CultureInfo.InvariantCulture),
                        CategoryName = category
                    });
                }
            }

            return items;
        }

        public static List<EventItem> GetEventItems(string resourceName, string category)
        {
            List<EventItem> items = new List<EventItem>();

            string csvData = Resources.ResourceManager.GetString(resourceName);

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
                        IsDlc = byte.Parse(parts[0], CultureInfo.InvariantCulture) == 1,
                        Id = Convert.ToInt32(parts[1], 16),
                        Name = parts[2],
                        StackSize = int.Parse(parts[3], CultureInfo.InvariantCulture),
                        MaxStorage = int.Parse(parts[4], CultureInfo.InvariantCulture),
                        NeedsEvent = byte.Parse(parts[5], CultureInfo.InvariantCulture) == 1,
                        EventId = int.Parse(parts[6], CultureInfo.InvariantCulture),
                        CategoryName = category
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
                    Id = int.Parse(parts[0], CultureInfo.InvariantCulture),
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

        private static string[] ParseCsvLine(string line)
        {
            var parts = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            parts.Add(current.ToString());

            return parts.ToArray();
        }

        public static Dictionary<string, List<BossRevive>> GetBossRevives()
        {
            Dictionary<string, List<BossRevive>> bossRevives = new Dictionary<string, List<BossRevive>>();
            string csvData = Resources.BossRevives;
            if (string.IsNullOrWhiteSpace(csvData)) return bossRevives;

            using StringReader reader = new StringReader(csvData);
            reader.ReadLine();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                string area = parts[1];
                var blockId = uint.Parse(parts[5], CultureInfo.InvariantCulture);

                BossRevive boss = new BossRevive
                {
                    IsDlc = bool.Parse(parts[0]),
                    Area = area,
                    BossName = parts[2],
                    IsInitializeDeadSet = bool.Parse(parts[3]),
                    NpcParamIds = ParseNpcParamIds(parts[4]),
                    BlockId = blockId,
                    FirstEncounterFlags = ParseFlags(parts[6]),
                    BossFlags = ParseFlags(parts[7]),
                    Position = ParsePosition(blockId, parts[8], parts[9]),
                    PositionFirstEncounter = ParsePosition(blockId, parts[10], parts[11]),
                    ShouldSetNight = bool.Parse(parts[12]),
                    BossBlockId = uint.Parse(parts[13], CultureInfo.InvariantCulture),
                };

                if (!bossRevives.ContainsKey(area))
                {
                    bossRevives[area] = new List<BossRevive>();
                }

                bossRevives[area].Add(boss);
            }

            return bossRevives;
        }

        private static List<uint> ParseNpcParamIds(string npcParamIds)
        {
            if (string.IsNullOrWhiteSpace(npcParamIds))
            {
                return new List<uint>();
            }

            string[] parts = npcParamIds.Split('|');

            List<uint> idsList = new List<uint>();

            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    idsList.Add(uint.Parse(part, CultureInfo.InvariantCulture));
                }
            }

            return idsList;
        }

        private static List<BossFlag> ParseFlags(string flagData)
        {
            var flags = new List<BossFlag>();
            if (string.IsNullOrWhiteSpace(flagData)) return flags;

            string[] flagEntries = flagData.Split('|');

            foreach (string entry in flagEntries)
            {
                string[] pair = entry.Split(':');
                if (pair.Length == 2 && int.TryParse(pair[0], NumberStyles.Integer, CultureInfo.InvariantCulture,
                        out int eventId))
                {
                    flags.Add(new BossFlag
                    {
                        EventId = eventId,
                        SetValue = bool.Parse(pair[1])
                    });
                }
            }

            return flags;
        }

        private static Position ParsePosition(uint blockId, string coordData, string rotationData)
        {
            return new Position(
                blockId,
                ParseCoords(coordData),
                string.IsNullOrWhiteSpace(rotationData) ? 0f : float.Parse(rotationData, CultureInfo.InvariantCulture)
            );
        }

        private static Vector3 ParseCoords(string coordData)
        {
            if (string.IsNullOrWhiteSpace(coordData))
            {
                return new Vector3();
            }

            string[] parts = coordData.Split('|');
            float[] coords = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                coords[i] = float.Parse(parts[i], CultureInfo.InvariantCulture);
            }

            return new Vector3(coords[0], coords[1], coords[2]);
        }

        public static List<ShopCommand> GetShops()
        {
            List<ShopCommand> shops = new List<ShopCommand>();
            string csvData = Resources.Shops;
            if (string.IsNullOrWhiteSpace(csvData)) return shops;
            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                bool isDlc = byte.Parse(parts[0], CultureInfo.InvariantCulture) == 1;
                var name = parts[1];
                var commandId = int.Parse(parts[2], CultureInfo.InvariantCulture);
                int[] @params = parts.Skip(3).Select(p => int.Parse(p, CultureInfo.InvariantCulture)).ToArray();

                var command = new EzState.TalkCommand(commandId, @params);
                var shopCommand = new ShopCommand(isDlc, name, command);
                shops.Add(shopCommand);
            }

            return shops;
        }

        public static List<T> GetSimpleList<T>(string resourceName, Func<string, T> parser)
        {
            List<T> items = new List<T>();
            string csvData = Resources.ResourceManager.GetString(resourceName);

            if (string.IsNullOrWhiteSpace(csvData)) return items;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                items.Add(parser(line.Trim()));
            }

            return items;
        }

        private static readonly string LoadoutsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TarnishedTool",
            "CustomLoadouts.json");

        public static Dictionary<string, LoadoutTemplate> LoadCustomLoadouts()
        {
            try
            {
                if (!File.Exists(LoadoutsPath))
                    return new Dictionary<string, LoadoutTemplate>();

                string json = File.ReadAllText(LoadoutsPath);
                var loadouts = JsonSerializer.Deserialize<List<LoadoutTemplate>>(json);

                return loadouts?.ToDictionary(l => l.Name) ?? new Dictionary<string, LoadoutTemplate>();
            }
            catch
            {
                return new Dictionary<string, LoadoutTemplate>();
            }
        }

        public static void SaveCustomLoadouts(Dictionary<string, LoadoutTemplate> loadouts)
        {
            try
            {
                string directory = Path.GetDirectoryName(LoadoutsPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(loadouts.Values.ToList(), options);
                File.WriteAllText(LoadoutsPath, json);
            }
            catch
            {
                // Silent fail or log
            }
        }

        private static readonly string GracePresetsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TarnishedTool",
            "GracePresets.json");

        public static Dictionary<string, GracePresetTemplate> LoadGracePresets()
        {
            try
            {
                if (!File.Exists(GracePresetsPath))
                    return new Dictionary<string, GracePresetTemplate>();

                string json = File.ReadAllText(GracePresetsPath);
                var presets = JsonSerializer.Deserialize<List<GracePresetTemplate>>(json);

                return presets?.ToDictionary(p => p.Name) ?? new Dictionary<string, GracePresetTemplate>();
            }
            catch
            {
                return new Dictionary<string, GracePresetTemplate>();
            }
        }

        public static void SaveGracePresets(Dictionary<string, GracePresetTemplate> presets)
        {
            try
            {
                string directory = Path.GetDirectoryName(GracePresetsPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(presets.Values.ToList(), options);
                File.WriteAllText(GracePresetsPath, json);
            }
            catch
            {
                // Silent fail or log
            }
        }

        public static List<Weather> GetWeatherTypes()
        {
            List<Weather> weathers = new List<Weather>();
            string csvData = Resources.WeatherTypes;
            if (string.IsNullOrWhiteSpace(csvData)) return weathers;

            using StringReader reader = new StringReader(csvData);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                sbyte weatherType = sbyte.Parse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture);
                string name = parts[1];
                weathers.Add(new Weather(weatherType, name));
            }

            return weathers;
        }
    }
}