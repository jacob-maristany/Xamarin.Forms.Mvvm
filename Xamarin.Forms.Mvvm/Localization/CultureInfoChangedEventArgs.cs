using System;
using System.Globalization;

namespace Xamarin.Forms.Mvvm
{
    public class CultureInfoChangedEventArgs : EventArgs
    {
        public CultureInfo NewCultureInfo { get; private set; }

        public CultureInfoChangedEventArgs(CultureInfo newCultureInfo)
        {
            NewCultureInfo = newCultureInfo;
        }
    }
}
