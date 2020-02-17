using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms.Mvvm.Sample.ViewModels
{
    public class DetailViewModel : ViewModelBase
    {
        private string _suppliedText;
        public string SuppliedText
        {
            get => _suppliedText;
            set => SetProperty(ref _suppliedText, value);
        }

        public async override Task Initialize(Dictionary<string, object> navigationsParams = null)
        {
            // do not do too much work here, or it'll take to long to push the page.
            // will need to refactor this to eliminate the Task
            // it is valid to invoke a Task.Run(...) or Device.BeginInvokeOnMainThread(...) here.

            SuppliedText = (string)navigationsParams?["enteredtext"] ?? "n/a";

            await Task.CompletedTask;
        }
    }
}
