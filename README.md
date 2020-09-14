# Simple Version Control

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MLD56V6HQWCKU&source=url)

<img align="left" width="64" height="64" src="https://cdn.jam-es.com/img/simple-version-control/icon.png">

This is a .NET Standard Library for setting up very basic version control in your .NET applications. It comes with a WPF app to quickly create your new version information.  

:star: :star: :star: And if you like it ... please star it! :star: :star: :star:  

## Gui Screenshots

Click for full size.

[<img width="122" height="86" src="https://cdn.jam-es.com/img/simple-version-control/screen1.png">](https://cdn.jam-es.com/img/simple-version-control/screen1.png)

[<img width="123" height="91" src="https://cdn.jam-es.com/img/simple-version-control/screen2.png">](https://cdn.jam-es.com/img/simple-version-control/screen2.png)

[<img width="124" height="70" src="https://cdn.jam-es.com/img/simple-version-control/screen3.png">](https://cdn.jam-es.com/img/simple-version-control/screen3.png)

## How to Use

Install the WPF app from the [GitHub Releases Page](https://github.com/James231/Simple-Version-Control/releases). Use the app to produce a set of version control JSON files. You should use the app to update these files every time a new version is released.  
  
**Note:** It is possible to write the version control files by hand, but I strongly recommend you use the app.  
  
Next you need somewhere to host your version control files. I recommend a free static web host like [GitHub Pages](https://pages.github.com/), [GitLab Pages](https://docs.gitlab.com/ee/user/project/pages/), or [Netlify](https://www.netlify.com/). My preferred choice is [GitLab Pages](https://docs.gitlab.com/ee/user/project/pages/).  
  
Once you have created and hosted the files to a url like ...  
```
https://example.com/app/vc/changelog.json
```
... then you can add the version control to your app. For example, you could add a 'Check For Updates' button.  
  
## Add Version Control to Your App

Within a console app, WinForms, WPF app, library or any other .NET project you can add version control information or checks.  
  
Start by installing the following SimpleVersionControl NuGet package. This can be done using the NuGet package manager with the following command:  
```
Install-Package SimpleVersionControl
```

Then within a C# file add:
```cs
using SimpleVersionControl;
```

Then the most important functionality is demonstrated in the code below:
```cs
// Create instance of VersionController by passing in this version of the application, and the URL of the directory containing changelog file:
VersionController versionController = new VersionController("1.0.1", "https://example.com/app/vc/");

// Check this is the latest version
bool isLatestVersion = await versionController.IsLatestVersion();

// If not latest, you can check this version is still meant to function correctly
bool isFunctioningVersion = await versionController.IsFunctioningVersion();

// Receive a list of all changes between two versions
// These are actually references to changes and you need to use ChangeRef.GetChange to get the Change object
IEnumerable<ChangeRef> changes = await versionController.GetChangesBetween("1.0.0", "1.0.1");
foreach(ChangeRef changeRef in changes) {
    Change change = await changeRef.GetChange();
    Console.WriteLine($"Change Title: {change.Title}");
    Console.WriteLine($"Change Description: {change.Description}");
    Console.WriteLine($"Change Release Version: {change.ReleaseVersion.VersionName}");
}

// The ChangeLog object contains a list of all versions that have been released
// We can list version properties like this:
ChangeLog changeLog = await versionController.GetChangeLog();
foreach (VersionRef versionRef in changeLog.Versions) {
    Version version = await versionRef.GetVersion();
    Console.WriteLine($"Version Name: {version.VersionName}");
    Console.WriteLine($"Version Description: {version.Description}");
    Console.WriteLine($"Version Download Link: {version.DownloadLink}");
    Console.WriteLine($"Version Release Date: {version.ReleaseDate}");
    // You can get all changes in a version with version.Changes
}

// Get individual versions with:
VersionRef firstAppVersionRef = await versionController.GetFirstVersionRef();
Version latestAppVersionRef = await versionController.GetLatestVersion();
VersionRef specificVersionRef = await versionController.GetVersionRef("1.0.1b4");
// Convert references to Version objects to get all the properties
Version specificVersion = await specificVersionRef.GetVersion();

// Get the current Version object representing this application version
Version curVersion = await versionController.GetVersion();

// ChangeLog, Version and Change objects can all have additional data stored within them (set in the WPF app)
// You can access this data by deserializing the AdditionalData property (it is a JObject)
// For example, to retrieve additional data from a version
class ExtraVersionData {
    public int Importance { get; set; }
}
ExtraVersionData extraData = curVersion.AdditionalData.ToObject<ExtraVersionData>();

// You could use this to order lists of versions or changes using Linq:
List<Task<Version>> getVersionTasks = changeLog.Versions.Select(async v => await v.GetVersion()).ToList();
IEnumerable<Version> versions = await Task.WhenAll(getVersionTasks);
versions.OrderBy(v => v.AdditionalData.ToObject<ExtraVersionData>().Importance);
// It is gerenally a good idea to surround this in try{}catch{} in case ToObject throws an error
```

## License

This code is released under MIT license. This means you can use this for whatever you want. Modify, distribute, sell, fork, and use this as much as you like. Both for personal and commercial use. I hold no responsibility if anything goes wrong.  
  
If you use this, you don't need to refer to this repo, or give me any kind of credit but it would be appreciated. At least a :star: would be nice.  

It took a lot of work to make this available for free. If you are feeling more generous, perhaps you could consider donating?

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MLD56V6HQWCKU&source=url)

## Contributing

Pull Requests are welcome. But, note that by creating a pull request you are giving me permission to merge your code and release it under the MIT license mentioned above. At no point will you be able to withdraw merged code from the repository, or change the license under which it has been made available.

## References

This wouldn't have been possible without ...

[Material Design In Xaml](http://materialdesigninxaml.net/) - The WPF styles used in this app.  
  
[AvalonEdit](http://avalonedit.net/) - The code editor WPF control used for the JSON editing in the app.  
  
[AvalonEditHighlightingThemes](https://github.com/Dirkster99/AvalonEditHighlightingThemes) - Implementation of Themes in AvalonEdit. Used for light/dark JSON editing themes.  
  
[Json.NET](https://www.newtonsoft.com/json) - JSON serializer.  

[Wix Toolset](https://wixtoolset.org/) - Used to create the `.msi` installer.

... and obvious credit to Microsoft for C#, WPF, .NET, and the best OS in existence :)