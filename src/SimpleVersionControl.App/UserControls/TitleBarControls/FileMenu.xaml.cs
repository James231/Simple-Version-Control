// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using SimpleVersionControl.App.UserControls.ContentAreaControls;

namespace SimpleVersionControl.App.UserControls.TitleBarControls
{
    /// <summary>
    /// Interaction logic for FileMenu.xaml
    /// </summary>
    public partial class FileMenu : UserControl
    {
        private Random random;

        public FileMenu()
        {
            InitializeComponent();
        }

        public static FileMenu GetInstance()
        {
            return MainWindow.GetInstance().TitleBar.FileMenu;
        }

        public void OpenButtonPressed(object sender, RoutedEventArgs args)
        {
            if (MainWindow.GetInstance().VersionController != null)
            {
                if (MessageBox.Show("Any unsaved changes will be lost.", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ChangeLog Json File (changelog.json)|changelog.json";
            openFileDialog.InitialDirectory = Environment.SpecialFolder.Personal.ToString();
            if (openFileDialog.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    if (Path.GetFileName(openFileDialog.FileName) == "changelog.json" && File.Exists(openFileDialog.FileName))
                    {
                        string dir = Path.GetDirectoryName(openFileDialog.FileName);
                        VersionController controller = new VersionController("0", new FileSystemProvider(dir));
                        ChangeLog changeLog = controller.GetChangeLog().Result;
                        if (changeLog != null)
                        {
                            MainWindow.GetInstance().VersionController = controller;
                            ContentArea.GetInstance().OpenPage(ContentArea.PageType.ChangeLog);
                            return;
                        }
                    }
                }

                MessageBox.Show("The selected file was not a valid 'changelog.json' file.", "Open ChangeLog Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SaveButtonPressed(object sender, RoutedEventArgs args)
        {
            if (MainWindow.GetInstance().VersionController == null)
            {
                MessageBox.Show("No ChangeLog was open!", "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // check json is correct in changelog
            if (!ChangeLogPage.GetInstance().IsJsonValid())
            {
                MessageBox.Show("The ChangeLog's additional data is not valid JSON.", "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                bool seriallizeWorked = await MainWindow.GetInstance().VersionController.Serialize();
                if (!seriallizeWorked)
                {
                    MessageBox.Show("Something went wrong.", "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                OpenFolder(((FileSystemProvider)MainWindow.GetInstance().VersionController.FileProvider).RootPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Save Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (random == null)
            {
                random = new Random();
            }

            if (random.Next(5) == 1)
            {
                if (MessageBox.Show("It took a lot of work to make this available for free. If you have any money going spare, please consider donating to keep the project alive. Would you like to donate now?", "Please Consider Donating", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(@"https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MLD56V6HQWCKU&source=url");
                }
            }

            await ContentArea.GetInstance().PostSerializeReload(MainWindow.GetInstance().VersionController);
        }

        public async void NewButtonPressed(object sender, RoutedEventArgs args)
        {
            if (MainWindow.GetInstance().VersionController != null)
            {
                if (MessageBox.Show("Any unsaved changes will be lost.", "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Pick Empty Directory to Store ChangeLog";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (!Directory.Exists(dialog.SelectedPath))
                    {
                        MessageBox.Show("The selected directory could not be found.", "New ChangeLog Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!IsDirectoryEmpty(dialog.SelectedPath))
                    {
                        MessageBox.Show("The selected directory was not empty!", "New ChangeLog Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    await CreateNewChangeLog(dialog.SelectedPath);
                }
            }
        }

        private async Task CreateNewChangeLog(string path)
        {
            VersionController newVC = new VersionController("0", new FileSystemProvider(path));
            ChangeLog newCL = new ChangeLog(newVC, new List<VersionRef>());
            MainWindow.GetInstance().VersionController = newVC;
            bool seriallizeWorked = await newVC.Serialize(newCL);
            if (!seriallizeWorked)
            {
                MessageBox.Show("The selected directory was not empty!", "New ChangeLog Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ContentArea.GetInstance().OpenPage(ContentArea.PageType.Empty);
                return;
            }

            ContentArea.GetInstance().OpenPage(ContentArea.PageType.ChangeLog);
        }

        private bool IsDirectoryEmpty(string path)
        {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
            using (IEnumerator<string> en = items.GetEnumerator())
            {
                return !en.MoveNext();
            }
        }

        private void OpenFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe",
                };

                Process.Start(startInfo);
            }
        }
    }
}
