using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Mvvm
{
    [ContentProperty("Key")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Key { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider) => TranslationService.Instance.Translate(Key);
    }
}
