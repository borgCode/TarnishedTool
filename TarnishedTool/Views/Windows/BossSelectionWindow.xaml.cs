using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using TarnishedTool.Models;

namespace TarnishedTool.Views.Windows
{
    public partial class BossSelectionWindow : Window
    {
        public BossRevive SelectedBoss { get; private set; }

        public BossSelectionWindow(List<BossRevive> bosses)
        {
            InitializeComponent();

            foreach (var boss in bosses)
            {
                BossListBox.Items.Add(boss.BossName);
            }
            BossListBox.SelectedIndex = 0;
            
            BossListBox.Tag = bosses;
        }

        private void WarpButton_Click(object sender, RoutedEventArgs e)
        {
            var bosses = (List<BossRevive>)BossListBox.Tag;
            SelectedBoss = bosses[BossListBox.SelectedIndex];
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}