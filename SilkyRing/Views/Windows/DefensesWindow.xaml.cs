// 

using System.Windows;
using System.Windows.Input;
using SilkyRing.Utilities;

namespace SilkyRing.Views.Windows;

public partial class DefensesWindow : Window
{
    public DefensesWindow()
    {
        InitializeComponent();
        
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.Closing += (sender, args) => { Close(); };
        }
        
        Loaded += (s, e) =>
        {
            if (SettingsManager.Default.DefenseWindowLeft > 0)
                Left = SettingsManager.Default.DefenseWindowLeft;
        
            if (SettingsManager.Default.DefenseWindowTop > 0)
                Top = SettingsManager.Default.DefenseWindowTop;
        };
    }
    
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        base.OnClosing(e);


        SettingsManager.Default.DefenseWindowLeft = Left;
        SettingsManager.Default.DefenseWindowTop = Top;
        SettingsManager.Default.Save();
    }
}