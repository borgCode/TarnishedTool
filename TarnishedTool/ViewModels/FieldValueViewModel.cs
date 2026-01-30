// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using TarnishedTool.Core;
using TarnishedTool.Enums;
using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class FieldValueViewModel(ParamFieldDef field, ParamEditorViewModel parent) : BaseViewModel
{
    #region Properties

    public string DisplayName => field.DisplayName;
    public string InternalName => field.InternalName;
    public string DataType => field.DataType;
    public double? Minimum => field.Minimum;
    public double? Maximum => field.Maximum;
    public bool IsBitfield => field.BitWidth.HasValue;
    public int Offset => field.Offset;
    public string VanillaValueText => FormatValue(_vanillaValue);
    public bool IsModified => !Equals(_value, _vanillaValue);

    public string FullName
    {
        get
        {
            return parent.ParamFieldDisplayMode switch
            {
                ParamFieldDisplayMode.OffsetNameInternal => $"0x{Offset:X}  {field.DisplayName} ({field.InternalName})",
                ParamFieldDisplayMode.NameInternal => $"{field.DisplayName} ({field.InternalName})",
                ParamFieldDisplayMode.NameOnly => field.DisplayName,
                ParamFieldDisplayMode.OffsetInternal => $"0x{Offset:X}  ({field.InternalName})",
                _ => $"0x{Offset:X}  {field.DisplayName} ({field.InternalName})"
            };
        }
    }
    
    public bool HasEnum => field.EnumType != null && EnumValues != null;
    
    private IReadOnlyList<EnumValueItem> _enumValues;

    public IReadOnlyList<EnumValueItem> EnumValues
    {
        get => _enumValues;
        set => SetProperty(ref _enumValues, value);
    }
    
    private object _value;

    public object Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                _value = ClampValue(_value);
                parent.WriteFieldValue(field, _value);
                OnPropertyChanged(nameof(IsModified));
                OnPropertyChanged(nameof(ValueText));
            }
        }
    }
    
    private object _vanillaValue;

    public object VanillaValue => _vanillaValue;

    private object ClampValue(object val)
    {
        if (val == null)
            return val;

        return field.DataType switch
        {
            "f32" => ((float)val).Clamp((float?)field.Minimum ?? float.MinValue, (float?)field.Maximum ?? float.MaxValue),
            "s32" => ((int)val).Clamp((int?)field.Minimum ?? int.MinValue, (int?)field.Maximum ?? int.MaxValue),
            "u32" => ((uint)val).Clamp((uint?)field.Minimum ?? uint.MinValue, (uint?)field.Maximum ?? uint.MaxValue),
            "s16" => ((short)val).Clamp((short?)field.Minimum ?? short.MinValue, (short?)field.Maximum ?? short.MaxValue),
            "u16" => ((ushort)val).Clamp((ushort?)field.Minimum ?? ushort.MinValue, (ushort?)field.Maximum ?? ushort.MaxValue),
            "s8"  => ((sbyte)val).Clamp((sbyte?)field.Minimum ?? sbyte.MinValue, (sbyte?)field.Maximum ?? sbyte.MaxValue),
            "u8" or "dummy8" => ((byte)val).Clamp((byte?)field.Minimum ?? byte.MinValue, (byte?)field.Maximum ?? byte.MaxValue),
            _ => val
        };
    }
    
    public string ValueText
    {
        get => FormatValue(_value);
        set
        {
            if (TryParseValue(value, out var parsed))
            {
                Value = parsed;
            }
        }
    }

    private string FormatValue(object val)
    {
        if (val == null) return "";
        return field.DataType switch
        {
            "f32" => $"{val:F2}",
            _ => $"{val:0}"
        };
    }
    

    private bool TryParseValue(string text, out object result)
    {
        result = null;
        if (string.IsNullOrEmpty(text)) return false;
    
        try
        {
            result = field.DataType switch
            {
                "f32" => float.Parse(text),
                "s32" => int.Parse(text),
                "u32" => uint.Parse(text),
                "s16" => short.Parse(text),
                "u16" => ushort.Parse(text),
                "s8" => sbyte.Parse(text),
                "u8" or "dummy8" => byte.Parse(text),
                _ => text
            };
            return true;
        }
        catch { return false; }
    }
    
    #endregion
    
    #region Public Methods

    public void RefreshValue()
    {
        
        _value = parent.ReadFieldValue(field);
        _vanillaValue = parent.ReadVanillaFieldValue(field);
        
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(ValueText));
        OnPropertyChanged(nameof(VanillaValue));
        OnPropertyChanged(nameof(VanillaValueText));
        OnPropertyChanged(nameof(IsModified));
    }
    
    
    public void SetEnumValues(IReadOnlyList<EnumValueItem> values)
    {
        EnumValues = values;
        OnPropertyChanged(nameof(EnumValues)); 
    }

    #endregion
}
