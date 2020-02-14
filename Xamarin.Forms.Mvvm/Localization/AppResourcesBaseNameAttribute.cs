using System;

namespace Xamarin.Forms.Mvvm
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AppResourcesBaseNameAttribute : Attribute
    {
        public AppResourcesBaseNameAttribute(string name)
        {
            BaseName = name;
        }

        public string BaseName { get; }
    }
}
