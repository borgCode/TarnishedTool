// 

using System.Windows;
using TarnishedTool.Utilities;
using TarnishedTool.ViewModels;

namespace TarnishedTool.Views.Windows;

public partial class ChrInsWindow : TopmostWindow
{
    public ChrInsWindow()
    {
        InitializeComponent();
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.AiWindowWindowLeft > 0)
                Left = SettingsManager.Default.AiWindowWindowLeft;
            
            if (SettingsManager.Default.AiWindowWindowTop > 0)
                Top = SettingsManager.Default.AiWindowWindowTop;
            
            AlwaysOnTopCheckBox.IsChecked = SettingsManager.Default.AiWindowAlwaysOnTop;
        };
        
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);
        var vm = (ChrInsWindowViewModel)DataContext;
        vm.ClearSelected();

        SettingsManager.Default.AiWindowWindowLeft = Left;
        SettingsManager.Default.AiWindowWindowTop = Top;
        SettingsManager.Default.AiWindowAlwaysOnTop = AlwaysOnTopCheckBox.IsChecked ?? false;
        SettingsManager.Default.Save();
    }
}