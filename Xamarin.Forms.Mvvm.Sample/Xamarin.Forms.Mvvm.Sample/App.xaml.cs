using System.Threading.Tasks;
using Xamarin.Forms.Mvvm.Sample.Pages;
using Unity;

namespace Xamarin.Forms.Mvvm.Sample
{
    public partial class App : ApplicationBase
    {
        public App() => InitializeComponent();

        protected override void InitializeContainer()
        {
            //Container.RegisterType<...>
        }

        protected override Task<Page> CreateMainPage()
            => CreateMasterDetailPage<TabletHomePage>();
    }
}
