// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Windows;

namespace SimpleVersionControl.App
{
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LocationChanged += TitleBar.Window_LocationChanged;
            StateChanged += TitleBar.Window_StateChanged;
        }

        public VersionController VersionController { get; set; }

        public static MainWindow GetInstance()
        {
            return (MainWindow)Application.Current.MainWindow;
        }
    }
}
