using System;
using System.Globalization;
using System.Reflection;

namespace Xamarin.Forms.Mvvm
{
    public interface ICulture
    {
        event EventHandler<CultureInfoChangedEventArgs> CultureInfoChanged;

        Assembly ResourceAssembly { get; }
        string BaseResourceName { get; }
        CultureInfo CultureInfo { get; }
    }
}
