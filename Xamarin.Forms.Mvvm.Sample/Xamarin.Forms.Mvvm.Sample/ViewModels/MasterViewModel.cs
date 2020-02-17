using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Mvvm.Sample.Pages;

namespace Xamarin.Forms.Mvvm.Sample.ViewModels
{
    public class MasterViewModel : ViewModelBase
    {
        private string _enteredText;
        public string EnteredText
        {
            get => _enteredText;
            set => SetProperty(ref _enteredText, value);
        }

        public Command PushDetailCommand { get; }

        public MasterViewModel() // DI is supported 
        {
            PushDetailCommand = new Command(async () => await PushDetail());
        }

        private async Task PushDetail()
        {
            var p = new Dictionary<string, object>()
            {
                ["enteredtext"] = EnteredText
            };

            await ReplaceDetailPageAsync<DetailPage, DetailViewModel>(p);
        }
    }
}
