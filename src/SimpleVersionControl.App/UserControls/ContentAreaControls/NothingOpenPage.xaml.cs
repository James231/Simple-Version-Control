// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using SimpleVersionControl.App.UserControls.TitleBarControls;

namespace SimpleVersionControl.App.UserControls.ContentAreaControls
{
    /// <summary>
    /// Interaction logic for NothingOpenPage.xaml
    /// </summary>
    public partial class NothingOpenPage : UserControl
    {
        public NothingOpenPage()
        {
            InitializeComponent();
        }

        private void OpenButtonPressed(object sender, RoutedEventArgs args)
        {
            FileMenu.GetInstance().OpenButtonPressed(sender, args);
        }

        private void NewButtonPressed(object sender, RoutedEventArgs args)
        {
            FileMenu.GetInstance().NewButtonPressed(sender, args);
        }
    }
}
