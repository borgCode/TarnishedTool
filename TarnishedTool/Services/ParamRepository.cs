// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TarnishedTool.Enums;
using TarnishedTool.Interfaces;
using TarnishedTool.Models;
using TarnishedTool.Properties;
using TarnishedTool.Utilities;
using static TarnishedTool.Enums.Param;

namespace TarnishedTool.Services;

public class ParamRepository : IParamRepository
{
    private readonly Dictionary<Param, LoadedParam> _cache = new();

    private readonly Dictionary<Param, (int TableIndex, int SlotIndex)> _paramLocations = new()
    {
        // ["AtkParam_Npc"] = (7, 0),
        // ["AtkParam_PC"] = (8, 0),
        // ["BehaviorParam"] = (12, 0),
        // ["BehaviorParam_PC"] = (13, 0),
        // ["BuddyParam"] = (134, 0),
        //bullet
        [CalcCorrectGraph] = (30, 0),
        [CharaInitParam] = (23, 0),
        [EquipParamAccessory] = (2, 0),
        [EquipParamGem] = (154, 0),
        [EquipParamGoods] = (3, 0),
        [EquipParamProtector] = (1, 0),
        [EquipParamWeapon] = (0, 0),
        [GameAreaParam] = (29, 0),
        [SpEffectParam] = (15, 0),
        [SwordArtsParam] = (82, 0),
    };

    public IReadOnlyList<Param> AvailableParams => _paramLocations.Keys.ToList();

    public (int TableIndex, int SlotIndex) GetLocation(Param param)
    {
        if (_paramLocations.TryGetValue(param, out var location))
            return location;
        throw new ArgumentException($"Unknown param: {param}");
    }

    public Dictionary<Param, List<ParamEntry>> GetAllEntriesByParam()
    {
        var result = new Dictionary<Param, List<ParamEntry>>();
    
        foreach (Param p in EnumUtil.GetValues<Param>())
        {
            var loaded = GetParam(p);
            result[p] = loaded.Entries.ToList();
        }
    
        return result;
    }

    public LoadedParam GetParam(Param param)
    {
        if (_cache.TryGetValue(param, out var cached))
            return cached;

        var (tableIndex, slotIndex) = GetLocation(param);
        var paramName = param.ToString();
        var fields = LoadFieldDefs(paramName);

        var loaded = new LoadedParam
        {
            Name = paramName,
            TableIndex = tableIndex,
            SlotIndex = slotIndex,
            Fields = fields,
            Entries = LoadEntries(paramName),
            RowSize = CalculateRowSize(fields)
        };

        _cache[param] = loaded;
        return loaded;
    }

    private int CalculateRowSize(IReadOnlyList<ParamFieldDef> fields)
    {
        var lastField = fields[fields.Count - 1];
        return lastField.Offset + GetTypeSize(lastField.DataType);
    }
    
    private List<ParamFieldDef> LoadFieldDefs(string paramName)
    {
        string json = Resources.ResourceManager.GetString($"Param_{paramName}");
        if (json == null) return new List<ParamFieldDef>();

        var fields = JsonSerializer.Deserialize<List<ParamFieldDef>>(json);
        CalculateOffsets(fields);
        return fields;
    }

    private List<ParamEntry> LoadEntries(string paramName)
    {
        string csv = Resources.ResourceManager.GetString($"ParamEntries_{paramName}");
        if (csv == null) return new List<ParamEntry>();

        var entries = new List<ParamEntry>();
        using var reader = new StringReader(csv);
        
        reader.ReadLine();

        while (reader.ReadLine() is { } line)
        {
            int comma = line.IndexOf(',');
            if (comma < 0) continue;
            if (!uint.TryParse(line.Substring(0, comma), out uint id)) continue;

            string name = comma + 1 < line.Length ? line.Substring(comma + 1) : string.Empty;
            entries.Add(new ParamEntry(id, name));
        }

        return entries;
    }

    private void CalculateOffsets(List<ParamFieldDef> fields)
    {
        int offset = 0;
        int bitPos = 0;

        foreach (var field in fields)
        {
            if (field.BitWidth.HasValue)
            {
                field.Offset = offset;
                field.BitPos = bitPos;
                bitPos += field.BitWidth.Value;

                if (bitPos >= 8)
                {
                    offset++;
                    bitPos = 0;
                }
            }
            else
            {
                if (bitPos > 0)
                {
                    offset++;
                    bitPos = 0;
                }

                field.Offset = offset;
                field.BitPos = null;
                offset += GetTypeSize(field.DataType);
            }
        }
    }

    private int GetTypeSize(string type) => type switch
    {
        "s8" or "u8" or "dummy8" => 1,
        "s16" or "u16" => 2,
        "s32" or "u32" or "f32" => 4,
        "s64" or "u64" or "f64" => 8,
        _ => 1
    };
}