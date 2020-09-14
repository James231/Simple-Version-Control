// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CustomWindow;
using HL.Interfaces;
using HL.Manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleVersionControl.App.UserControls.ContentAreaControls.ChangeLogControls;

namespace SimpleVersionControl.App.UserControls.ContentAreaControls
{
    /// <summary>
    /// Interaction logic for ChagneLogPage.xaml
    /// </summary>
    public partial class ChangeLogPage : UserControl
    {
        private bool eventBlock = false;

        public ChangeLogPage()
        {
            InitializeComponent();

            IThemedHighlightingManager hm = ThemedHighlightingManager.Instance;
            hm.SetCurrentTheme("VS2019_Dark");
            AdditionalDataField.SyntaxHighlighting = hm.GetDefinitionByExtension(".js");
        }

        public static ChangeLogPage GetInstance()
        {
            return ContentArea.GetInstance().ChangeLogPage;
        }

        public async void Refresh()
        {
            AdditionalDataExpander.IsExpanded = false;

            ChangeLog changeLog = await MainWindow.GetInstance().VersionController.GetChangeLog();
            if (changeLog == null)
            {
                ContentArea.GetInstance().OpenPage(ContentArea.PageType.Empty);
                MainWindow.GetInstance().VersionController = null;
                return;
            }

            if (changeLog.AdditionalData == null)
            {
                changeLog.AdditionalData = JObject.Parse("{}");
            }

            eventBlock = true;
            AdditionalDataField.Text = changeLog.AdditionalData.ToString(Formatting.Indented);

            VersionParent.Children.Clear();

            foreach (VersionRef versionRef in changeLog.Versions)
            {
                VersionControl versionControl = new VersionControl(versionRef);
                VersionParent.Children.Add(versionControl);
            }
        }

        private async void AdditionalDataField_TextChanged(object sender, System.EventArgs e)
        {
            if (eventBlock || MainWindow.GetInstance().VersionController == null)
            {
                eventBlock = false;
                return;
            }

            ChangeLog changeLog = await MainWindow.GetInstance().VersionController.GetChangeLog();

            try
            {
                JObject newObject = JObject.Parse(AdditionalDataField.Text);
                changeLog.AdditionalData = newObject;
                JsonErrorText.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception)
            {
                JsonErrorText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public bool IsJsonValid()
        {
            return JsonErrorText.Visibility != System.Windows.Visibility.Visible;
        }

        public void CreateVersionPressed(object sender, RoutedEventArgs args)
        {
            VersionController versionController = MainWindow.GetInstance().VersionController;
            string newVersionName = GetNewVersionName();
            VersionRef versionRef = new VersionRef(versionController, Path.Combine(newVersionName, "version.json"), newVersionName);
            VersionControl newControl = new VersionControl(versionRef);
            VersionParent.Children.Insert(0, newControl);
            ChangeLog changeLog = versionController.GetChangeLog().Result;
            changeLog.Versions.Insert(0, versionRef);

            Version version = new Version(versionController, newVersionName, string.Empty, string.Empty, DateTime.Now, new System.Collections.Generic.List<ChangeRef>());
            versionController.CacheVersion(version);

            ContentArea.GetInstance().OpenPage(ContentArea.PageType.Version, versionRef);
        }

        public void Edit(VersionControl control)
        {
            int index = VersionParent.Children.IndexOf(control);

            if (index == -1)
            {
                return;
            }

            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;
            ContentArea.GetInstance().OpenPage(ContentArea.PageType.Version, changeLog.Versions[index]);
        }

        public void MoveUp(VersionControl control)
        {
            int index = VersionParent.Children.IndexOf(control);

            if (index <= 0)
            {
                return;
            }

            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;

            VersionRef versionRef = changeLog.Versions[index];
            changeLog.Versions.Remove(versionRef);
            VersionParent.Children.Remove(control);

            changeLog.Versions.Insert(index - 1, versionRef);
            VersionParent.Children.Insert(index - 1, control);
        }

        public void MoveDown(VersionControl control)
        {
            int index = VersionParent.Children.IndexOf(control);

            if (index == -1 || index == VersionParent.Children.Count - 1)
            {
                return;
            }

            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;
            VersionRef versionRef = changeLog.Versions[index];
            changeLog.Versions.Remove(versionRef);
            VersionParent.Children.Remove(control);

            changeLog.Versions.Insert(index + 1, versionRef);
            VersionParent.Children.Insert(index + 1, control);
        }

        public void Duplicate(VersionControl control)
        {
            int index = VersionParent.Children.IndexOf(control);

            if (index == -1)
            {
                return;
            }

            VersionController versionController = MainWindow.GetInstance().VersionController;
            ChangeLog changeLog = versionController.GetChangeLog().Result;
            VersionRef oldVersionRef = changeLog.Versions[index];
            string duplicateName = GetDuplicateName(oldVersionRef.VersionName);
            VersionRef newVersionRef = new VersionRef(versionController, oldVersionRef.RelativeFilePath, duplicateName);
            changeLog.Versions.Insert(index + 1, newVersionRef);
            VersionControl newControl = new VersionControl(newVersionRef);
            VersionParent.Children.Insert(index + 1, newControl);

            Version oldVersion = oldVersionRef.GetVersion(false).Result;
            Version version = new Version(versionController, duplicateName, oldVersion.Description, oldVersion.DownloadLink, oldVersion.ReleaseDate, new System.Collections.Generic.List<ChangeRef>());
            if (oldVersion.AdditionalData != null)
            {
                version.AdditionalData = (JObject)oldVersion.AdditionalData.DeepClone();
            }

            foreach (ChangeRef oldChangeRef in oldVersion.Changes)
            {
                string guid = Guid.NewGuid().ToString();
                string newChangePath = Path.Combine(version.VersionName, "changes", $"{guid}.json");
                ChangeRef newChangeRef = new ChangeRef(versionController, guid, newChangePath);
                Change oldChange = oldChangeRef.GetChange(false).Result;
                Change newChange = new Change(versionController, guid, newChangePath, oldChange.Title, oldChange.Description, newVersionRef);
                if (oldChange.ReleaseVersion != null)
                {
                    newChange.ReleaseVersion = new VersionRef(versionController, newVersionRef.RelativeFilePath, newVersionRef.VersionName);
                }

                if (oldChange.AdditionalData != null)
                {
                    newChange.AdditionalData = (JObject)oldChange.AdditionalData.DeepClone();
                }

                version.Changes.Add(newChangeRef);
                versionController.CacheChange(newChange);
            }

            versionController.CacheVersion(version);
        }

        public void Delete(VersionControl control)
        {
            int index = VersionParent.Children.IndexOf(control);

            if (index == -1)
            {
                return;
            }

            VersionParent.Children.Remove(control);
            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;
            changeLog.Versions.RemoveAt(index);
        }

        private string GetDuplicateName(string baseName)
        {
            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;

            // Check adding " Copy"
            if (changeLog.Versions.FirstOrDefault(v => v.VersionName == $"{baseName} Copy") == null)
            {
                return $"{baseName} Copy";
            }

            // Check adding " Copy 2", " Copy 3", ... etc
            int index = 2;
            while (changeLog.Versions.FirstOrDefault(v => v.VersionName == $"{baseName} Copy {index}") != null)
            {
                index++;
            }

            return $"{baseName} Copy {index}";
        }

        private string GetNewVersionName()
        {
            ChangeLog changeLog = MainWindow.GetInstance().VersionController.GetChangeLog().Result;

            // Try "New Version"
            if (changeLog.Versions.FirstOrDefault(v => v.VersionName == "New Version") == null)
            {
                return $"New Version";
            }

            // Try "New Version 2", "New Version 3", ... etc
            int index = 2;
            while (changeLog.Versions.FirstOrDefault(v => v.VersionName == $"New Version {index}") != null)
            {
                index++;
            }

            return $"New Version {index}";
        }
    }
}
