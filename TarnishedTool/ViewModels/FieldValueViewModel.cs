// 

using TarnishedTool.Models;

namespace TarnishedTool.ViewModels;

public class FieldValueViewModel(ParamFieldDef field, ParamEditorViewModel parent) : BaseViewModel
{
    #region Properties

    public string DisplayName => field.DisplayName;
    public string InternalName => field.InternalName;
    public string DataType => field.DataType;
    public float? Minimum => field.Minimum;
    public float? Maximum => field.Maximum;
    public bool IsBitfield => field.BitWidth.HasValue;
    public int Offset => field.Offset;
    
    public string FullName => $"0x{Offset:X}  {field.DisplayName} ({field.InternalName})";

    private object _value;

    public object Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                parent.WriteFieldValue(field, value);
            }
        }
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
            _ => val.ToString()
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
        OnPropertyChanged(nameof(Value));
        OnPropertyChanged(nameof(ValueText));
    }

    #endregion
}