using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
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
    }
}