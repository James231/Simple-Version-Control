// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleVersionControl.App.UserControls
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        private Point startPos;
        private System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;

        public TitleBar()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        public void Window_LocationChanged(object sender, EventArgs e)
        {
            MainWindow window = MainWindow.GetInstance();

            int sum = 0;
            foreach (var item in screens)
            {
                sum += item.WorkingArea.Width;
                if (sum >= window.Left + (window.Width / 2))
                {
                    window.MaxHeight = item.WorkingArea.Height + 7;
                    break;
                }
            }
        }

        private void System_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = MainWindow.GetInstance();
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount >= 2)
                {
                    window.WindowState = (window.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
                }
                else
                {
                    startPos = e.GetPosition(null);
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                var pos = window.PointToScreen(e.GetPosition(window));
                IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
                IntPtr hMenu = GetSystemMenu(hWnd, false);
                int cmd = TrackPopupMenu(hMenu, 0x100, (int)pos.X, (int)pos.Y, 0, hWnd, IntPtr.Zero);
                if (cmd > 0)
                {
                    SendMessage(hWnd, 0x112, (IntPtr)cmd, IntPtr.Zero);
                }
            }
        }

        private void System_MouseMove(object sender, MouseEventArgs e)
        {
            MainWindow window = MainWindow.GetInstance();
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (window.WindowState == WindowState.Maximized && Math.Abs(startPos.Y - e.GetPosition(null).Y) > 2)
                {
                    var point = window.PointToScreen(e.GetPosition(null));

                    window.WindowState = WindowState.Normal;

                    window.Left = point.X - (window.ActualWidth / 2);
                    window.Top = point.Y - (border.ActualHeight / 2);
                }

                window.DragMove();
            }
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.GetInstance().WindowState = (MainWindow.GetInstance().WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.GetInstance().VersionController != null)
            {
                if (MessageBox.Show("Any unsaved changes will be lost.", "Are you sure you want to Exit?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            MainWindow.GetInstance().Close();
        }

        private void Mimimize_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.GetInstance().WindowState = WindowState.Minimized;
        }

        public void Window_StateChanged(object sender, EventArgs e)
        {
            MainWindow window = MainWindow.GetInstance();

            if (window.WindowState == WindowState.Maximized)
            {
                window.main.BorderThickness = new Thickness(0);
                window.main.Margin = new Thickness(7, 7, 7, 0);
                rectMax.Visibility = Visibility.Hidden;
                rectMin.Visibility = Visibility.Visible;
            }
            else
            {
                window.main.BorderThickness = new Thickness(1);
                window.main.Margin = new Thickness(0);
                rectMax.Visibility = Visibility.Visible;
                rectMin.Visibility = Visibility.Hidden;
            }
        }
    }
}
