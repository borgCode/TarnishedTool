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
        [AtkParam_Npc] = (7, 0),
        [AtkParam_PC] = (8, 0),
        [BehaviorParam] = (12, 0),
        [BehaviorParam_PC] = (13, 0),
        [BuddyParam] = (128, 0),
        [Bullet] = (10, 0),
        [CalcCorrectGraph] = (30, 0),
        [CharaInitParam] = (23, 0),
        [EquipParamAccessory] = (2, 0),
        [EquipParamGem] = (154, 0),
        [EquipParamGoods] = (3, 0),
        [EquipParamProtector] = (1, 0),
        [EquipParamWeapon] = (0, 0),
        [GameAreaParam] = (29, 0),
        [ItemLotParam_enemy] = (20, 0),
        [ItemLotParam_map] = (21, 0),
        [Magic] = (14,0),
        [NpcParam] = (6,0),
        [NpcThinkParam] = (9,0),
        [ResistCorrectParam] = (174, 0),
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
        var baseName = paramName.Split('_')[0];
        var fields = LoadFieldDefs(baseName);

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
    
    private int CalculateRowSize(IReadOnlyList<ParamFieldDef> fields)
    {
        var lastField = fields[fields.Count - 1];
        return lastField.Offset + GetTypeSize(lastField);  // Pass the whole field
    }
    

    private void CalculateOffsets(List<ParamFieldDef> fields)
    {
        int offset = 0;
        int bitPos = 0;
        int bitFieldSize = 0;  // Size of current bitfield type in bytes

        foreach (var field in fields)
        {
            if (field.BitWidth.HasValue)
            {
                int bitLimit = GetBitLimit(field.DataType);
                int typeSize = GetBaseTypeSize(field.DataType);
            
                // Start new bitfield if: first bitfield, different size type, or would overflow
                if (bitPos == 0 || typeSize != bitFieldSize || bitPos + field.BitWidth.Value > bitLimit)
                {
                    if (bitPos > 0)
                    {
                        offset += bitFieldSize;  // Advance by previous bitfield's type size
                    }
                    bitPos = 0;
                    bitFieldSize = typeSize;
                }

                field.Offset = offset;
                field.BitPos = bitPos;
                bitPos += field.BitWidth.Value;
            }
            else
            {
                if (bitPos > 0)
                {
                    offset += bitFieldSize;  // Finish the bitfield
                    bitPos = 0;
                    bitFieldSize = 0;
                }

                field.Offset = offset;
                field.BitPos = null;
                offset += GetTypeSize(field);
            }
        }
    }

    private int GetBitLimit(string type) => type switch
    {
        "s8" or "u8" or "dummy8" => 8,
        "s16" or "u16" => 16,
        "s32" or "u32" => 32,
        _ => 8
    };

    private int GetBaseTypeSize(string type) => type switch
    {
        "s8" or "u8" or "dummy8" => 1,
        "s16" or "u16" => 2,
        "s32" or "u32" or "f32" => 4,
        "s64" or "u64" or "f64" => 8,
        _ => 1
    };

    private int GetTypeSize(ParamFieldDef field)
    {
        int baseSize = GetBaseTypeSize(field.DataType);
        return baseSize * (field.ArrayLength ?? 1);
    }
}