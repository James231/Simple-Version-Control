// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleVersionControl.App.UserControls
{
    /// <summary>
    /// Interaction logic for ContentArea.xaml
    /// </summary>
    public partial class ContentArea : UserControl
    {
        public ContentArea()
        {
            InitializeComponent();
        }

        public enum PageType
        {
            Empty,
            ChangeLog,
            Version,
            Change,
        }

        public static ContentArea GetInstance()
        {
            return MainWindow.GetInstance().ContentArea;
        }

        public void OpenPage(PageType pageType, VersionRef versionRef = null, ChangeRef changeRef = null)
        {
            EmptyPage.Visibility = Visibility.Collapsed;
            ChangeLogPage.Visibility = Visibility.Collapsed;
            VersionPage.Visibility = Visibility.Collapsed;
            ChangePage.Visibility = Visibility.Collapsed;

            switch (pageType)
            {
                case PageType.Empty:
                    EmptyPage.Visibility = Visibility.Visible;
                    break;
                case PageType.ChangeLog:
                    ChangeLogPage.Visibility = Visibility.Visible;
                    ChangeLogPage.Refresh();
                    break;
                case PageType.Version:
                    if (versionRef == null)
                    {
                        return;
                    }

                    VersionPage.Visibility = Visibility.Visible;
                    VersionPage.Refresh(versionRef);
                    break;
                case PageType.Change:
                    if (changeRef == null)
                    {
                        return;
                    }

                    ChangePage.Visibility = Visibility.Visible;
                    ChangePage.Refresh(changeRef);
                    break;
            }
        }

        public async Task PostSerializeReload(VersionController versionController)
        {
            ChangeLog changeLog = await versionController.GetChangeLog();

            if (EmptyPage.Visibility == Visibility.Visible)
            {
                return;
            }

            if (ChangeLogPage.Visibility == Visibility.Visible)
            {
                OpenPage(PageType.ChangeLog);
                return;
            }

            if (VersionPage.Visibility == Visibility.Visible)
            {
                VersionRef oldRef = VersionPage.GetRef();
                foreach (VersionRef versionRef in changeLog.Versions)
                {
                    if (versionRef.VersionName == oldRef.VersionName)
                    {
                        OpenPage(PageType.Version, versionRef);
                        return;
                    }
                }

                OpenPage(PageType.ChangeLog);
                return;
            }

            if (ChangePage.Visibility == Visibility.Visible)
            {
                VersionRef oldVersionRef = VersionPage.GetRef();
                ChangeRef oldChangeRef = ChangePage.GetChangeRef();
                foreach (VersionRef versionRef in changeLog.Versions)
                {
                    if (versionRef.VersionName == oldVersionRef.VersionName)
                    {
                        Version version = await versionRef.GetVersion();
                        foreach (ChangeRef changeRef in version.Changes)
                        {
                            if (changeRef.Guid == oldChangeRef.Guid)
                            {
                                OpenPage(PageType.Change, versionRef, changeRef);
                                return;
                            }
                        }

                        OpenPage(PageType.Version, versionRef);
                        return;
                    }
                }

                OpenPage(PageType.ChangeLog);
                return;
            }
        }
    }
}
