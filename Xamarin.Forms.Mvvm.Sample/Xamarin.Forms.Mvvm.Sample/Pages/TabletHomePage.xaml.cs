using System.Threading.Tasks;
using Xamarin.Forms.Mvvm.Sample.ViewModels;

namespace Xamarin.Forms.Mvvm.Sample.Pages
{
    public partial class TabletHomePage : MasterDetailPageBase
    {
        public TabletHomePage() => InitializeComponent();

        public override Task<Page> CreateMasterPage()
            => CurrentApplication.CreatePage<MasterPage, MasterViewModel>(true);

        public override Task<Page> CreateDetailPage()
            => CurrentApplication.CreatePage<DetailPage, DetailViewModel>(true);
    }
}
