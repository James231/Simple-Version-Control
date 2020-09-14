// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using HL.Interfaces;
using HL.Manager;
using Newtonsoft.Json.Linq;

namespace SimpleVersionControl.App.UserControls.ContentAreaControls
{
    /// <summary>
    /// Interaction logic for ChangePage.xaml
    /// </summary>
    public partial class ChangePage : UserControl
    {
        private bool eventBlock = false;
        private ChangeRef openChangeRef;
        private VersionRef versionRef;

        public ChangePage()
        {
            InitializeComponent();

            IThemedHighlightingManager hm = ThemedHighlightingManager.Instance;
            hm.SetCurrentTheme("VS2019_Dark");
            AdditionalDataField.SyntaxHighlighting = hm.GetDefinitionByExtension(".js");
        }

        public static ChangePage GetInstance()
        {
            return ContentArea.GetInstance().ChangePage;
        }

        public void Refresh(ChangeRef changeRef)
        {
            AdditionalDataExpander.IsExpanded = false;

            openChangeRef = changeRef;

            Change change = changeRef.GetChange().Result;
            versionRef = change.ReleaseVersion;
            VersionText.Text = $"Version: {change.ReleaseVersion.VersionName}";

            eventBlock = true;
            ChangeTitleField.Text = change.Title;

            eventBlock = true;
            DescriptionField.Text = change.Description;

            if (change.AdditionalData == null)
            {
                change.AdditionalData = JObject.Parse("{}");
            }

            eventBlock = true;
            AdditionalDataField.Text = change.AdditionalData.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        public async void BackButtonPressed(object sender, RoutedEventArgs args)
        {
            Change change = await openChangeRef.GetChange();
            VersionRef versionRef = change.ReleaseVersion;
            openChangeRef = null;
            ContentArea.GetInstance().OpenPage(ContentArea.PageType.Version, versionRef);
        }

        private async void ChangeTitleField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Change change = await openChangeRef.GetChange();
            change.Title = ChangeTitleField.Text;
        }

        private async void DescriptionField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Change change = await openChangeRef.GetChange();
            change.Description = DescriptionField.Text;
        }

        private async void AdditionalDataField_TextChanged(object sender, System.EventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Change change = await openChangeRef.GetChange();

            try
            {
                JObject newObject = JObject.Parse(AdditionalDataField.Text);
                change.AdditionalData = newObject;
                JsonErrorText.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception)
            {
                JsonErrorText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        internal ChangeRef GetChangeRef()
        {
            return openChangeRef;
        }

        internal VersionRef GetVersionRef()
        {
            return versionRef;
        }
    }
}
