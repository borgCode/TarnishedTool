// 

using System.Windows;
using TarnishedTool.Utilities;

namespace TarnishedTool.Views.Windows;

public partial class ParamEditorWindow : TopmostWindow
{
    public ParamEditorWindow()
    {
        InitializeComponent();
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.ParamEditorWindowLeft > 0)
                Left = SettingsManager.Default.ParamEditorWindowLeft;
        
            if (SettingsManager.Default.ParamEditorWindowTop > 0)
                Top = SettingsManager.Default.ParamEditorWindowTop;
            
            AlwaysOnTopCheckBox.IsChecked = SettingsManager.Default.ParamEditorWindowAlwaysOnTop;
        };
        
        
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);


        SettingsManager.Default.ParamEditorWindowLeft = Left;
        SettingsManager.Default.ParamEditorWindowTop = Top;
        SettingsManager.Default.ParamEditorWindowAlwaysOnTop = AlwaysOnTopCheckBox.IsChecked ?? false;
        SettingsManager.Default.Save();
    }
}