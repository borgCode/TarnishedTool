// 

using System.Windows;
using SilkyRing.Utilities;

namespace SilkyRing.Views.Windows;

public partial class AttackInfoWindow : Window
{
    public AttackInfoWindow()
    {
        InitializeComponent();
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.AttackInfoWindowLeft > 0)
                Left = SettingsManager.Default.AttackInfoWindowLeft;
        
            if (SettingsManager.Default.AttackInfoWindowTop > 0)
                Top = SettingsManager.Default.AttackInfoWindowTop;
        };
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);


        SettingsManager.Default.AttackInfoWindowLeft = Left;
        SettingsManager.Default.AttackInfoWindowTop = Top;
        SettingsManager.Default.Save();
    }
}