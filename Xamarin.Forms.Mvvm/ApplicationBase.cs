using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;
using Xamarin.Forms;

namespace Xamarin.Forms.Mvvm
{
    public abstract class ApplicationBase : Application
    {
        private Lazy<UnityContainer> _container = new Lazy<UnityContainer>(() => new UnityContainer());
        public UnityContainer Container => _container.Value;

        public void Init(Action<UnityContainer> platformInitializeContainer = null)
        {
            platformInitializeContainer?.Invoke(Container);
            InitializeContainer();

            Page mainPage = null;
            Task.Run(async () => mainPage = await CreateMainPage()).Wait();

            MainPage = mainPage;
        }

        protected abstract void InitializeContainer();
        protected abstract Task<Page> CreateMainPage();

        public Task<Page> CreatePage<TPage, TViewModel>(Dictionary<string, object> navigationParams = null)
            where TPage : ContentPageBase
            where TViewModel : ViewModelBase
            => CreatePage<TPage, TViewModel>(false, navigationParams);

        public async Task<Page> CreatePage<TPage, TViewModel>(bool wrapInNavigationPage, Dictionary<string, object> navigationParams = null)
            where TPage : ContentPageBase
            where TViewModel : ViewModelBase
        {
            var vm = Container.Resolve<TViewModel>();
            var page = Container.Resolve<TPage>();

            vm.SetWeakPage(page);
            await vm.Initialize(navigationParams);

            page.BindingContext = vm;
            page.Initialize();

            if (wrapInNavigationPage)
            {
                return new NavigationPage(page)
                {
                    Title = nameof(TPage) // HACK: Bug in Forms where even a Nav page needs a title in the master pane
                };
            }

            return page;
        }

        public async Task<Page> CreateMasterDetailPage<TPage>(Dictionary<string, object> navigationParams = null)
            where TPage : MasterDetailPageBase
        {
            var masterDetailPage = Container.Resolve<TPage>();
            await masterDetailPage.Initialize(navigationParams);

            Page masterPage = await masterDetailPage.CreateMasterPage();
            masterDetailPage.Master = masterPage;

            var rootMasterPage = masterPage is NavigationPage ? ((NavigationPage)masterPage).RootPage : masterPage;
            (rootMasterPage.BindingContext as ViewModelBase)?.SetWeakMasterDetailpage(masterDetailPage);

            Page detailPage = await masterDetailPage.CreateDetailPage();
            masterDetailPage.Detail = detailPage;

            var rootDetailPage = detailPage is NavigationPage ? ((NavigationPage)detailPage).RootPage : detailPage;
            (rootDetailPage.BindingContext as ViewModelBase)?.SetWeakMasterDetailpage(masterDetailPage);

            return masterDetailPage;
        }


        public async Task<Page> CreateTab<TView, TViewModel>(
            Dictionary<string, object> navigationParams = null,
            bool wrapInNavigationPage = false,
            Style navigationStyle = null)
            where TView : ContentPageBase
            where TViewModel : ViewModelBase
        {
            var page = Container.Resolve<TView>();
            var vm = Container.Resolve<TViewModel>();
            await vm.Initialize(navigationParams);

            vm.SetWeakPage((ContentPageBase)page);
            page.BindingContext = vm;
            ((ContentPageBase)page).Initialize();

            if (wrapInNavigationPage)
            {
                if (navigationStyle == null)
                {
                    return new NavigationPage(page)
                    {
                        Title = page.Title,
                        IconImageSource = page.IconImageSource,
                    };
                }
                else 
                {
                    return new NavigationPage(page)
                    {
                        Title = page.Title,
                        IconImageSource = page.IconImageSource,
                        Style = navigationStyle 
                    };
                }
               
            }
            return page;
        }


        public async Task<Page> CreateTabForMasterDetail<MView, MViewModel, DView, DViewModel>(
            Dictionary<string, object> masterNavigationParams = null,
            Dictionary<string, object> detailNavigationParams = null,
            bool wrapDetailInNavigationPage = false,
            Style navigationStyle = null)
            where MView : ContentPageBase
            where DView : ContentPageBase
            where MViewModel : ViewModelBase
            where DViewModel : ViewModelBase
        {
            MasterDetailPage mdPage;
            NavigationPage navDetailPage;

            var masterPage = await CreatePage<MView, MViewModel>(masterNavigationParams);
            var detailPage = await CreatePage<DView, DViewModel>(detailNavigationParams);

            if (wrapDetailInNavigationPage)
            {
                navDetailPage = (navigationStyle == null) 
                    ? new NavigationPage(detailPage)
                    {
                        Title = detailPage.Title,
                    } 
                    : new NavigationPage(detailPage)
                    {
                        Title = detailPage.Title,
                        Style = navigationStyle
                    };

                mdPage = new MasterDetailPage
                {
                    Master = masterPage,
                    Detail = navDetailPage,
                    Title = masterPage.Title,
                    IconImageSource = masterPage.IconImageSource
                };
            }
            else 
            {
                mdPage = new MasterDetailPage
                {
                    Master = masterPage,
                    Detail = detailPage,
                    Title = masterPage.Title,
                    IconImageSource = masterPage.IconImageSource
                };
            }
            return mdPage;
        }
    }
}
