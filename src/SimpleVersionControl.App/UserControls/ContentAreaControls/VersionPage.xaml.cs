// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HL.Interfaces;
using HL.Manager;
using Newtonsoft.Json.Linq;
using SimpleVersionControl.App.UserControls.ContentAreaControls.VersionPageControls;

namespace SimpleVersionControl.App.UserControls.ContentAreaControls
{
    /// <summary>
    /// Interaction logic for VersionPage.xaml
    /// </summary>
    public partial class VersionPage : UserControl
    {
        private VersionRef openVersionRef;
        private bool eventBlock = false;
        private SolidColorBrush redColor;
        private SolidColorBrush whiteColor;

        public VersionPage()
        {
            InitializeComponent();

            redColor = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            whiteColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            IThemedHighlightingManager hm = ThemedHighlightingManager.Instance;
            hm.SetCurrentTheme("VS2019_Dark");
            AdditionalDataField.SyntaxHighlighting = hm.GetDefinitionByExtension(".js");
        }

        public static VersionPage GetInstance()
        {
            return ContentArea.GetInstance().VersionPage;
        }

        public void Refresh(VersionRef versionRef)
        {
            AdditionalDataExpander.IsExpanded = false;

            openVersionRef = versionRef;
            Version version = versionRef.GetVersion().Result;

            VersionField.Foreground = whiteColor;
            VersionTitle.Foreground = whiteColor;

            AdditionalDataExpander.IsExpanded = false;

            eventBlock = true;
            VersionField.Text = versionRef.VersionName;

            eventBlock = true;
            DescriptionField.Text = version.Description;

            eventBlock = true;
            LinkField.Text = version.DownloadLink;

            eventBlock = true;
            ReleaseDateField.SelectedDate = version.ReleaseDate;

            if (FunctioningField.IsChecked == !version.IsFunctioning)
            {
                eventBlock = true;
                FunctioningField.IsChecked = version.IsFunctioning;
            }

            if (version.AdditionalData == null)
            {
                version.AdditionalData = JObject.Parse("{}");
            }

            eventBlock = true;
            AdditionalDataField.Text = version.AdditionalData.ToString(Newtonsoft.Json.Formatting.Indented);

            ChangeParent.Children.Clear();

            foreach (ChangeRef changeRef in version.Changes)
            {
                ChangeControl changeControl = new ChangeControl(changeRef);
                ChangeParent.Children.Add(changeControl);
            }
        }

        public void BackButtonPressed(object sender, RoutedEventArgs args)
        {
            openVersionRef = null;
            ContentArea.GetInstance().OpenPage(ContentArea.PageType.ChangeLog);
        }

        private void NewChangeButtonPressed(object sender, RoutedEventArgs args)
        {
            Version version = openVersionRef.GetVersion().Result;
            VersionController versionController = MainWindow.GetInstance().VersionController;

            string guid = Guid.NewGuid().ToString();
            string relativePath = Path.Combine(version.VersionName, "changes", $"{guid}.json");
            ChangeRef changeRef = new ChangeRef(versionController, guid, relativePath);
            version.Changes.Insert(0, changeRef);

            Change change = new Change(versionController, guid, relativePath, "New Change", string.Empty, openVersionRef);
            change.ReleaseVersion = new VersionRef(versionController, openVersionRef.RelativeFilePath, openVersionRef.VersionName);
            versionController.CacheChange(change);

            ChangeControl newControl = new ChangeControl(changeRef);
            ChangeParent.Children.Insert(0, newControl);

            ContentArea.GetInstance().OpenPage(ContentArea.PageType.Change, openVersionRef, changeRef);
        }

        public async void Edit(ChangeControl control)
        {
            int index = ChangeParent.Children.IndexOf(control);

            if (index == -1)
            {
                return;
            }

            Version version = await openVersionRef.GetVersion();
            ContentArea.GetInstance().OpenPage(ContentArea.PageType.Change, openVersionRef, version.Changes[index]);
        }

        public void Duplicate(ChangeControl control)
        {
            int index = ChangeParent.Children.IndexOf(control);

            if (index == -1)
            {
                return;
            }

            VersionController versionController = MainWindow.GetInstance().VersionController;
            Version version = openVersionRef.GetVersion().Result;
            ChangeRef oldChangeRef = version.Changes[index];
            string guid = Guid.NewGuid().ToString();
            string relativePath = Path.Combine(version.VersionName, "changes", $"{guid}.json");
            ChangeRef newChangeRef = new ChangeRef(versionController, guid, relativePath);
            version.Changes.Insert(index + 1, newChangeRef);

            Change oldChange = oldChangeRef.GetChange(false).Result;
            VersionRef releaseVersion = new VersionRef(versionController, oldChange.ReleaseVersion.RelativeFilePath, oldChange.ReleaseVersion.VersionName);
            Change newChange = new Change(versionController, guid, relativePath, oldChange.Title, oldChange.Description, releaseVersion);

            if (oldChange.AdditionalData != null)
            {
                newChange.AdditionalData = (JObject)oldChange.AdditionalData.DeepClone();
            }

            versionController.CacheChange(newChange);

            ChangeControl newControl = new ChangeControl(newChangeRef);
            ChangeParent.Children.Insert(index + 1, newControl);
        }

        public void MoveUp(ChangeControl control)
        {
            int index = ChangeParent.Children.IndexOf(control);

            if (index <= 0)
            {
                return;
            }

            Version version = openVersionRef.GetVersion().Result;
            ChangeRef changeRef = version.Changes[index];
            version.Changes.Remove(changeRef);
            ChangeParent.Children.Remove(control);

            version.Changes.Insert(index - 1, changeRef);
            ChangeParent.Children.Insert(index - 1, control);
        }

        public void MoveDown(ChangeControl control)
        {
            int index = ChangeParent.Children.IndexOf(control);

            if (index == -1 || index == ChangeParent.Children.Count - 1)
            {
                return;
            }

            Version version = openVersionRef.GetVersion().Result;
            ChangeRef changeRef = version.Changes[index];
            version.Changes.Remove(changeRef);
            ChangeParent.Children.Remove(control);

            version.Changes.Insert(index + 1, changeRef);
            ChangeParent.Children.Insert(index + 1, control);
        }

        public void Delete(ChangeControl control)
        {
            int index = ChangeParent.Children.IndexOf(control);

            if (index == -1)
            {
                return;
            }

            ChangeParent.Children.Remove(control);
            Version version = openVersionRef.GetVersion().Result;
            version.Changes.RemoveAt(index);
        }

        private void VersionField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;
            bool versionAlreadyInUse = changeLog.Versions.FirstOrDefault(v => v != openVersionRef && v.VersionName == VersionField.Text) != null;

            if (versionAlreadyInUse)
            {
                VersionField.Foreground = redColor;
                VersionTitle.Foreground = redColor;
                return;
            }

            VersionField.Foreground = whiteColor;
            VersionTitle.Foreground = whiteColor;

            Version version = openVersionRef.GetVersion().Result;
            version.VersionName = VersionField.Text;

            openVersionRef.VersionName = VersionField.Text;
            openVersionRef.RelativeFilePath = Path.Combine(VersionField.Text, "version.json");

            foreach (ChangeRef changeRef in version.Changes)
            {
                Change change = changeRef.GetChange().Result;
                change.ReleaseVersion = openVersionRef;
                changeRef.RelativeFilePath = Path.Combine(version.VersionName, "changes", $"{changeRef.Guid}.json");
            }
        }

        private async void AdditionalDataField_TextChanged(object sender, System.EventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Version version = await openVersionRef.GetVersion();

            try
            {
                JObject newObject = JObject.Parse(AdditionalDataField.Text);
                version.AdditionalData = newObject;
                JsonErrorText.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception)
            {
                JsonErrorText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private async void DescriptionField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Version version = await openVersionRef.GetVersion();
            version.Description = DescriptionField.Text;
        }

        private async void LinkField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Version version = await openVersionRef.GetVersion();
            version.DownloadLink = LinkField.Text;
        }

        private async void ReleaseDateField_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Version version = await openVersionRef.GetVersion();
            version.ReleaseDate = ReleaseDateField.SelectedDate.Value;
        }

        private async void FunctioningField_Checked(object sender, RoutedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Version version = await openVersionRef.GetVersion();
            version.IsFunctioning = true;
        }

        private async void FunctioningField_Unchecked(object sender, RoutedEventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            Version version = await openVersionRef.GetVersion();
            version.IsFunctioning = false;
        }

        internal VersionRef GetRef()
        {
            return openVersionRef;
        }
    }
}