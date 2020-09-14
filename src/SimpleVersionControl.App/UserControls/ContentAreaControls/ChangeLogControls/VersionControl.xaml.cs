// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace SimpleVersionControl.App.UserControls.ContentAreaControls.ChangeLogControls
{
    /// <summary>
    /// Interaction logic for VersionControl.xaml
    /// </summary>
    public partial class VersionControl : UserControl
    {
        public VersionControl(VersionRef versionRef)
        {
            InitializeComponent();

            VersionText.Text = versionRef.VersionName;
        }

        public void EditPressed(object sender, RoutedEventArgs args)
        {
            ChangeLogPage.GetInstance().Edit(this);
        }

        public void MoveUpPressed(object sender, RoutedEventArgs args)
        {
            ChangeLogPage.GetInstance().MoveUp(this);
        }

        public void MoveDownPressed(object sender, RoutedEventArgs args)
        {
            ChangeLogPage.GetInstance().MoveDown(this);
        }

        public void DuplicatePressed(object sender, RoutedEventArgs args)
        {
            ChangeLogPage.GetInstance().Duplicate(this);
        }

        public void DeletePressed(object sender, RoutedEventArgs args)
        {
            ChangeLogPage.GetInstance().Delete(this);
        }
    }
}
