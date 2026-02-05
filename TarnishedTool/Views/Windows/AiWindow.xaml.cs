// 

using System;
using System.Windows;
using TarnishedTool.Utilities;

namespace TarnishedTool.Views.Windows;

public partial class AiWindow : TopmostWindow
{
    public AiWindow()
    {
        InitializeComponent();
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.AiWindowWindowLeft > 0)
                Left = SettingsManager.Default.AiWindowWindowLeft;
        
            if (SettingsManager.Default.AiWindowWindowTop > 0)
                Top = SettingsManager.Default.AiWindowWindowTop;
            
            AlwaysOnTopCheckBox.IsChecked = SettingsManager.Default.AiWindowAlwaysOnTop;
            
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
            }
                
            
        };
    }
    
    
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        SettingsManager.Default.AiWindowWindowLeft = Left;
        SettingsManager.Default.AiWindowWindowTop = Top;
        SettingsManager.Default.AiWindowAlwaysOnTop = AlwaysOnTopCheckBox.IsChecked ?? false;
        SettingsManager.Default.Save();
        
        (DataContext as IDisposable)?.Dispose();
    }
}