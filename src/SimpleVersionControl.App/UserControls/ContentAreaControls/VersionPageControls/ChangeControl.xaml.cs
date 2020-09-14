// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace SimpleVersionControl.App.UserControls.ContentAreaControls.VersionPageControls
{
    /// <summary>
    /// Interaction logic for ChangeControl.xaml
    /// </summary>
    public partial class ChangeControl : UserControl
    {
        public ChangeControl(ChangeRef changeRef)
        {
            InitializeComponent();

            ChangeTitle.Text = changeRef.GetChange(false).Result.Title;
        }

        public void EditPressed(object sender, RoutedEventArgs args)
        {
            VersionPage.GetInstance().Edit(this);
        }

        public void MoveUpPressed(object sender, RoutedEventArgs args)
        {
            VersionPage.GetInstance().MoveUp(this);
        }

        public void MoveDownPressed(object sender, RoutedEventArgs args)
        {
            VersionPage.GetInstance().MoveDown(this);
        }

        public void DuplicatePressed(object sender, RoutedEventArgs args)
        {
            VersionPage.GetInstance().Duplicate(this);
        }

        public void DeletePressed(object sender, RoutedEventArgs args)
        {
            VersionPage.GetInstance().Delete(this);
        }
    }
}
