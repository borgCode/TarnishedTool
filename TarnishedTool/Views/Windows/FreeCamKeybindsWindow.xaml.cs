using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TarnishedTool.Utilities;

namespace TarnishedTool.Views.Windows
{
    public partial class FreeCamKeybindsWindow : Window
    {
        private readonly List<(string Label, Func<int> Get, Action<int> Set, int DefaultVk)> _entries;
        private readonly Dictionary<TextBox, (Func<int> Get, Action<int> Set)> _boxMap = new();

        public FreeCamKeybindsWindow()
        {
            InitializeComponent();
            var s = SettingsManager.Default;
            _entries =
            [
                ("Forward", () => s.FreeCamForwardKey, v => s.FreeCamForwardKey = v, 0x57),
                ("Backward", () => s.FreeCamBackwardKey, v => s.FreeCamBackwardKey = v, 0x53),
                ("Left", () => s.FreeCamLeftKey, v => s.FreeCamLeftKey = v, 0x41),
                ("Right", () => s.FreeCamRightKey, v => s.FreeCamRightKey = v, 0x44),
                ("Up", () => s.FreeCamUpKey, v => s.FreeCamUpKey = v, 0x20),
                ("Down", () => s.FreeCamDownKey, v => s.FreeCamDownKey = v, 0x11),
                ("Speed Boost", () => s.FreeCamFastKey, v => s.FreeCamFastKey = v, 0x10),
                ("Tilt Left", () => s.FreeCamTiltLeftKey, v => s.FreeCamTiltLeftKey = v, 0x51),
                ("Tilt Right", () => s.FreeCamTiltRightKey, v => s.FreeCamTiltRightKey = v, 0x45),
                ("Reset Tilt", () => s.FreeCamResetTiltKey, v => s.FreeCamResetTiltKey = v, 0x52)
            ];

            BuildRows();
        }

        private void BuildRows()
        {
            BindingsPanel.Children.Clear();
            _boxMap.Clear();

            foreach (var (label, get, set, _) in _entries)
            {
                var row = new Grid { Margin = new Thickness(0, 3, 0, 3) };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var tbLabel = new TextBlock
                {
                    Text = label,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(tbLabel, 0);

                var box = new TextBox
                {
                    Text = FormatVk(get()),
                    IsReadOnly = true,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Height = 24,
                    Cursor = Cursors.Hand
                };
                box.GotFocus += Box_GotFocus;
                box.LostFocus += Box_LostFocus;
                box.PreviewKeyDown += Box_PreviewKeyDown;

                _boxMap[box] = (get, set);
                Grid.SetColumn(box, 1);

                row.Children.Add(tbLabel);
                row.Children.Add(box);
                BindingsPanel.Children.Add(row);
            }
        }

        private void Box_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox box)
                box.Text = "Press a key...";
        }

        private void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox box && _boxMap.TryGetValue(box, out var entry))
                box.Text = FormatVk(entry.Get());
        }

        private void Box_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox box || !_boxMap.TryGetValue(box, out var entry)) return;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            e.Handled = true;

            if (key == Key.Escape)
            {
                box.Text = FormatVk(entry.Get());
                Keyboard.ClearFocus();
                return;
            }

            var vk = key switch
            {
                Key.LeftCtrl or Key.RightCtrl => 0x11,
                Key.LeftShift or Key.RightShift => 0x10,
                Key.LeftAlt or Key.RightAlt => 0x12,
                _ => KeyInterop.VirtualKeyFromKey(key)
            };

            if (vk <= 0) return;

            entry.Set(vk);
            SettingsManager.Default.Save();
            box.Text = FormatVk(vk);
            Keyboard.ClearFocus();
        }

        private static string FormatVk(int vk) => vk switch
        {
            0x10 => "Shift",
            0x11 => "Ctrl",
            0x12 => "Alt",
            0x20 => "Space",
            _ => KeyInterop.KeyFromVirtualKey(vk).ToString()
        };

        private void ResetDefaults_Click(object sender, RoutedEventArgs e)
        {
            foreach (var (_, _, set, defaultVk) in _entries)
                set(defaultVk);
            SettingsManager.Default.Save();
            BuildRows();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
