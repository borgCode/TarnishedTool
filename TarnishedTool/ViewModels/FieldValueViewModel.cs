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

    #endregion

    #region Public Methods

    public void RefreshValue()
    {
        _value = parent.ReadFieldValue(field);
        OnPropertyChanged(nameof(Value));
    }

    #endregion
}