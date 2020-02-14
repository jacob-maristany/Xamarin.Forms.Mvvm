using System;
using System.Globalization;
using System.Resources;
using Xamarin.Forms;

namespace Xamarin.Forms.Mvvm
{
    public class TranslationService
    {
        public event EventHandler<CultureInfoChangedEventArgs> CultureInfoChanged;

        public ICulture PlatformCulture { get; }
        public CultureInfo CurrentCultureInfo { get; private set; }
        public ResourceManager ResourceManager { get; }

        #region Singleton
        private static Lazy<TranslationService> _instance = new Lazy<TranslationService>(() => new TranslationService());
        public static TranslationService Instance => _instance.Value;
        private TranslationService()
        {
            PlatformCulture = DependencyService.Get<ICulture>();
            ResourceManager = new ResourceManager(PlatformCulture.BaseResourceName, PlatformCulture.ResourceAssembly);
            CurrentCultureInfo = PlatformCulture.CultureInfo;

            PlatformCulture.CultureInfoChanged += PlatformCultureDidChange;
        }
        #endregion

        private void PlatformCultureDidChange(object sender, CultureInfoChangedEventArgs e)
        {
            CurrentCultureInfo = e.NewCultureInfo;
            OnCultureInfoChanged();
        }

        private void OnCultureInfoChanged() => CultureInfoChanged?.Invoke(this, new CultureInfoChangedEventArgs(CurrentCultureInfo));

        public string Translate(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return ResourceManager.GetString(key, CurrentCultureInfo) ?? key;
        }
    }
}
