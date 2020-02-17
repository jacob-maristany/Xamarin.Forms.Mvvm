# Xamarin.Forms.Mvvm

This project includes a simple and light MVVM framework designed to be as close as possible to the raw surface of the Xamarin.Forms APIs.  For the introductory philosophy behind this framework, refer to the blog series starting at https://jacobmaristany.blog/2019/10/08/a-responsive-viewmodel-part-i/. 

### Unity
This MVVM framework supports dependency injection (DI) for your Pages and view models using Unity as an IoC container.  Any reference to an API denoting a "Container" implies that the MVVM framework is interacting with UnityContainer in order to register entries to be injected in any Page or view model you Create via the MVVM framework.

### NotifyPropertyChanged
This is a basic and common implemetation of `INotifyPropertyChanged`.  `ViewModelBase` implements this class.  It's exposed publicly for your convienence.

### ApplicationBase
This class derives from `Xamarin.Forms.Application` and changes the way you bootstrap the application, both in this class and in the `AppDelegate.cs` for iOS and the `MainActivity.cs` for Android.  Unlike the default implementation of `Xamarin.Forms.Application`, this pattern allow your `ApplicationBase`'s constructor to fully construct and return before setting the `MainPage` of the application.

For each Bootstrapping implementation for each platform, you have the ability to pass an `Action<UnityContainer>` to be used to register platform dependent implementations.

This class also contains methods to easily create a Page, using a `ContentPageBase` derived class and a `ViewModelBase` derived class`, to construct and intialize a Xamarin.Forms Page to supply for navigation.

Example implementation of ApplicationBase:
```C#
using System.Threading.Tasks;
using EY.Mobile.XamarinForms;
using Unity;
using Xamarin.Forms;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
[assembly: AppResourcesBaseName("EY.Mobile.AwesomeApp.Resources.AppResources")]

namespace EY.Mobile.AwesomeApp
{
    public partial class App : ApplicationBase
    {
        public App() => InitializeComponent();

        protected override Task<Page> CreateMainPage() 
            => CreatePage<StartPage, StartViewModel>(true);

        protected override void InitializeContainer()
        {
#if DEBUG
            Container.EnableDebugDiagnostic();
#endif

            Container.RegisterType<AwesomeWebService>();
            Container.RegisterSingleton<ILoggingService, LoggingService>();
        }
    }
}
```

Bootstrapping in AppDelegate.cs for iOS:
```C#
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        global::Xamarin.Forms.Forms.Init();

        var app = new App();
        app.Init(PlatformInitializeContainer); 

        LoadApplication(app);

        return base.FinishedLaunching(app, options);
    }

    public void PlatformInitializeContainer(UnityContainer container)
    {
        container.RegisterType<IPlatformService, iOSService>();
    }
```

Bootstrapping in MainActivity.cs for Android:
```C#
    protected override void OnCreate(Bundle savedInstanceState)
    {
        TabLayoutResource = Resource.Layout.Tabbar;
        ToolbarResource = Resource.Layout.Toolbar;

        base.OnCreate(savedInstanceState);

        global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

        var app = new App();
        app.Init(PlatformInitializeContainer);

        LoadApplication(checklistApp);
    }

    public void PlatformInitializeContainer(UnityContainer container)
    {
        container.RegisterType<IPlatformService, AndroidService>();
    }
```

### ContentPageBase
Every Page used by this framework is required to derive from `ContentPageBase`.  This is to support `OnAppearing()` and `OnDisappearing()` method invocation in `ViewModelBase` as well as providing an `Initialize()` method to allow for additional logic to be run in the Page without having it run in the consructor.

If you override `OnAppearing()` or `OnDisappearing()` in `ContentPageBase`, you must call the `base` method in order for the proper method to be invoked on the `ViewModelBase`.

Example use of ContentPageBase:
```XML
<s:ContentPageBase  
	xmlns="http://xamarin.com/schemas/2014/forms" 
	xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
	x:Class="EY.Mobile.AwesomeApp.Pages.AwesomePage"
	
    xmlns:s="clr-namespace:EY.Mobile.XamarinForms;assembly=EY.Mobile.XamarinForms"
	>

</s:ContentPageBase>
```

```C#
public partial class AwesomePage : ContentPageBase
{
    public AwesomePage() => InitializeComponent();

    public override void Initialize()
    {
        // Any heavy logic here that doesn't need to be in the constructor.
	// Do not do too much here as it will slow down page creation and navigation.
	// Task.Run(...) and Device.BeingInvokeOnMainThread(...) are valid strategies here.
    }
}
```

### ViewModelBase
Like `ContentPageBase`, this framework intends for all of our view models to inherit from `ViewModelBase`.  This class is deisgned to provide access to methods that only exist on the Xamarin.Forms.Page class.  When you navigate via a navigation method on `ViewModelBase` or when you create a Page using `ApplicationBase.CreatePage<TPage, TViewModel>(...)`, the framework will create a `Weak<T>` reference to the Page within the `ViewModelBase`.  In effect, a usable subset of APIs from the Page class are available to use from your view model. 

From your `ViewModelBase` derived class, you can access such methods and properties as `DisplayAlert(...)`, `DisplayActionSheet(...)`, `ModalStack`, `NavigationStack`, `OnAppearing()/OnDisappearing()`, `PopToRootAsync(...)`, navigation methods such as `PopAsync(...)` and `PushAsync(...)`, and more.

There are also convienence methods available such as `ReplaceMainPageAsync<TPage, TViewModel>(...)`, `ReplaceDetailPageAsync<TPage, TViewModel>(...)`, `PushMasterDetailAsync<TPage>(...)`, `PopToPageAsync<TPage>(...)` and `PoppingTo(...)`.

The navigation method definitions have been modified to add support to specificy the types of your Page and view model, which is uses to construct your page via the `ApplicationBase.CreatePage<TPage, TViewModel>(...)` methods.

### MasterDetailPageBase
When using a `MasterDetailPage` in your Xamarin.Forms applicaiton, you must also use the `MasterDetailPageBase` to benefit from this framework.  Doing so allows you to pass paramters to the `Initialize(...)` method of `MasterDetailPageBase`.  You can create one via `ViewModelBase.PushMasterDetailAsync<TPage>(...)`, `ViewModelBase.PushMasterDetailModalAsync<TPage>(...)` or `ApplicationBase.CraeteMasterDetailPage<TPage>(...)`.

Ths base class also has convienence methods for creating both the Master and Detail methods in code, using the framework to construct them.

Example TabletPage.xaml
```XML
<s:MasterDetailPageBase
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    x:Class="EY.Mobile.AwesomeApp.Pages.TabletPage" 
    
    xmlns:s="clr-namespace:EY.Mobile.XamarinForms;assembly=EY.Mobile.XamarinForms"

    Title="Must have a title"
    />
```

Example TabletPage.xaml.cs
```C#
using System;
using System.Threading.Tasks;
using EY.Mobile.XamarinForms;
using Xamarin.Forms;

namespace AT.Mobile.Checklist.Core.Pages
{
    public partial class TabletPage : MasterDetailPageBase
    {
        public TabletPage() => InitializeComponent();

        public override Task<Page> CreateMasterPage()
            => CurrentApplication.CreatePage<MyMasterPage, MyMasterViewModel>(true);

        public override Task<Page> CreateDetailPage()
            => CurrentApplication.CreatePage<MyDetailPage, MyDetailViewModel>(true);
    }
}
```

### Messaging
While you can still use the `Xamarin.Forms.MessagingCenter`, this framework contains an optional `Messaging` class that is more lighter weight, but is necessarily contention-based.  This means that you must follow a specific pattern in order for it to function properly.  Specically, you must have a reference to your own subscriber `Action` in your class that is maintaining the subscription.  

You can easily use this `Messaging` class via `Messaging.Instance` or through a property on `ViewModelBase` simply called `Messaging`.

The important concepts of this `Messaging` class are channels and subscription keys.  Channels are string-based identifiers to tie subscribers and publishers.  For any publication on a specific channel, each subscriber on that channel will recieve the publication.  A subscriber key is a unique identifier to define a subscriber.  They should be unique as this is how the `Messaging` class identifies any specific subscurber if you ever voluntarily `Unsubscribe(...)`.

Note on channels: There are version of the `Publish` and `Subscribe` methods that take no parameters and ones that take parameters.  These are considererd seperated channels.  You are either publishing or subscribing on the parameterless channel or the one that takes a parameter.  

Example publisher:
```C#

Messaging.Publish("something-happened-channel"); // Notifies the scubscribers of this channel just that this thing occured.

Messaging.Publish<int>("something-happened-channel", 2); // This is a seperate channel as it expects parameters. 

```

Example subscriber:
```C#

public AwesomeViewModel: ViewModelBase
{
    // You must hold your own reference to the Actions of the subscription. 
    // If yo do not, the garabage collector will near-instantly dispose of them once you subscribe.
    // This is the convention-based approach and the feature that makes this class super lightweight.
    private Action SomethingHappenedSubscription;
    private Action<int> SomethingHappenedWithParamSubscription;

    public AwesomeViewModel()
    {
        SomethingHappenedSubscription = SomethingHappened;
        SomethingHappenedWithParamSubscription = SomethingHappenedWithParam;

        Messaging.Subscribe("something-happened-channel", "awesome-subscriber-one", SomethingHappenedSubscription);
        Messaging.Subscribe<int>("something-happened-channel", "awesome-subscriber-one", SomethingHappenedWithParamSubscription);
    }

    private void SomethingHappened() { }
    private void SomethingHappenedWithParam(int p) { }
}

```

### Navigation and Passing Paramters
The navigation and Page creation methods in this framework have been expanded to allow passing paramters when creating/pushing Pages as well as when popping pages. 

The parametrs you pass are represented by a simple `Dictionary<string, object>` where the key a string to identify which paramter is being passed and the value is the actual object reference you wish to pass.  The reason for using a `Dictionary` is to support cases where you have multiple paramters of the same type to pass between pages.

Note: When suppling navigation parameters during a call to `PopAsync(...)`, the parameters are passed to the previous view model's `PoppingTo(...)` method.

Here is an example of populating and suppling parameters during navigation. 

```C#
    var p = new Dictionary<string, object> 
    {
        ["important-item"] = new ImportantItem()
    };

    await PushModalAsync<NextPage, NextViewModel>(p);
```

Paramerters are recieved by the resulting `ViewModelBase.Iniitalize(...)` method.

```C#
public class NextViewModel : ViewModelBase
{
    public async override Task Initialize(Dictionary<string, object> navigationsParams = null)
    {
    	// Do not do too much here as it will slow down page creation and navigation.
	// Task.Run(...) and Device.BeingInvokeOnMainThread(...) are valid strategies here.
    
        if (navigationsParams.Containskey("important-item")}
        {
            // We got something important here!
        }
    }
}
```

## Feature:  Translation Services
### Using the translation service
This framework allows support for localization of a Xamarin.Forms application through resx files contained in the .NETStandard library.  You will have a resx file for each language you support.  In order to organize your resx files, it is recommended to create a folder in your .NETStandard project (e.g. "Resources").  Once you add a resx file (e.g. "AppResources.resx"), it will also contain a designer file.  Ignore it.  Each translation file for each language needs a resx file by the same name, in the same namespace and with the two-digit locale code as a suffix. For example, a resx file for Spanish (locale code "es") would be named "AppResources.es.resx".

In order for translations to work, the framework must be informed of where your resx files are located.  This is done through an assembly-level attribute.  The recommendation approach is to put this in your App.xaml.cs file.  This is an assembly-level attribute, so the definition needs to exist outside of your namespace and class definition.

```C#
using EY.Mobile.XamarinForms;
using Xamarin.Forms;

[assembly: AppResourcesBaseName("EY.Mobile.AwesomeApp.Resources.AppResources")]

namespace EY.Mobile.AwesomeApp
{
    public partial class App : ApplicationBase
    {
        ...
    }
}
```

Each entry in a resx file is represented by a key/value pair.  For any entry missing for any locale other than the default, the value used in the base resx will be used.  If there is no value for the given key, then the translation service will return the key string you provided.

The framework provides a TranslateExtension to be used in XAML to allow access to these values in your declarations.  You can also use your resouces class directly, where each property matches the key available in the default resx to return the appropriate value.  This class will share the same name as your resx file.

Using a translation key in XAML:
```XML
<s:ContentPageBase
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    x:Class="AT.Mobile.Checklist.Core.Pages.AzureLoginPage"
    
    xmlns:s="clr-namespace:EY.Mobile.XamarinForms;assembly=EY.Mobile.XamarinForms"

    Title="{s:Translate TitleForThisPage}"
    >
</s:ContentPageBase>
```

Using a translation key in C#:
```C#
Title = AppResources.TitleForThisPage;
```

### Updating the RESX Culture
There is an issue with the resx files where they will not respond to culture changes from the device.  The end result is that any reference to the resx file key/values in code (e.g. `AppResources.Mykey`), will result in the old translation resx file being used and not the new one.  In order to resolve this issue, the `TranslationService` provides an event hook to respond to culture changes from the device, in which you can update the culture of our resx file.  The recommended approach is to do this in your `App:ApplicationBase` constructor.

i.e.
```C#
public partial class App : ApplicationBase
{
    public App()
    {
        InitializeComponent();

        AppResources.Culture = TranslationService.Instance.CurrentCultureInfo;
        TranslationService.Instance.CultureInfoChanged += (sender, e) =>
        {
            AppResources.Culture = e.NewCultureInfo;
        };
    }
}
```
