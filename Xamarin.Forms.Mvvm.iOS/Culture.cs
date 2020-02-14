using System;
using System.Globalization;
using System.Reflection;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(Xamarin.Forms.Mvvm.iOS.Culture))]

namespace Xamarin.Forms.Mvvm.iOS
{
    [Preserve(AllMembers = true)]
    public sealed class Culture : ICulture
    {
        public event EventHandler<CultureInfoChangedEventArgs> CultureInfoChanged;

        private CultureInfo _cultureInfo;

        public Assembly ResourceAssembly { get; }
        public string BaseResourceName { get; }
        public CultureInfo CultureInfo { get => _cultureInfo; }

        public Culture()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var attribute = assembly.GetCustomAttribute(typeof(AppResourcesBaseNameAttribute)) as AppResourcesBaseNameAttribute;
                if (attribute != null)
                {
                    ResourceAssembly = assembly;
                    BaseResourceName = attribute.BaseName;
                    break;
                }
            }

            UpdateCultureInfo();

            NSNotificationCenter.DefaultCenter.AddObserver(NSLocale.CurrentLocaleDidChangeNotification, CurrentLocaleDidChange);
        }

        private void CurrentLocaleDidChange(NSNotification notification)
        {
            UpdateCultureInfo();
            OnCultureInfoChanged();
        }

        private void OnCultureInfoChanged() => CultureInfoChanged?.Invoke(this, new CultureInfoChangedEventArgs(CultureInfo));

        private void UpdateCultureInfo()
        {
            var netLanguage = "en";

            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = iOSToDotnetLanguage(pref);
            }

            CultureInfo ci = null;

            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException)
            {
                // iOS locale not valid .NET culture (eg. "en-ES" : English in Spain). fallback to first characters, in this case "en"
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new CultureInfo(fallback);
                }
                catch (CultureNotFoundException)
                {
                    // iOS language not valid .NET culture, falling back to English
                    ci = new CultureInfo("en");
                }
            }

            _cultureInfo = ci;
        }

        private string iOSToDotnetLanguage(string iOSLanguage)
        {
            //Console.WriteLine("iOS Language:" + iOSLanguage);
            var netLanguage = iOSLanguage;

            //certain languages need to be converted to CultureInfo equivalent
            switch (iOSLanguage)
            {
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture
                    netLanguage = "ms"; // closest supported
                    break;
                case "gsw-CH":  // "Schwiizertüütsch (Swiss German)" not supported .NET culture
                    netLanguage = "de-CH"; // closest supported
                    break;
                    // add more application-specific cases here (if required)
                    // ONLY use cultures that have been tested and known to work
            }

            return netLanguage;
        }

        private string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode; // use the first part of the identifier (two chars, usually);

            switch (platCulture.LanguageCode)
            {
                case "pt":
                    netLanguage = "pt-PT"; // fallback to Portuguese (Portugal)
                    break;
                case "gsw":
                    netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app
                    break;
                    // add more application-specific cases here (if required)
                    // ONLY use cultures that have been tested and known to work
            }

            return netLanguage;
        }
    }
}
