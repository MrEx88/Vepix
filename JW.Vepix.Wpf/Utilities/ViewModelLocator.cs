using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Services;
using JW.Vepix.Infrastructure.Data;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace JW.Vepix.Wpf.Utilities
{
    public class ViewModelLocator
    {
        //public PictureGridViewModel PictureGridViewModel { get; }
        public MainViewModel MainViewModel { get; }
        public FolderTreeViewModel TreePictureFolderViewModel { get; }

        public ViewModelLocator()
        {
            var container = new UnityContainer();

            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileExplorerDialogService, FileExplorerDialogService>();
            container.RegisterType<IBitmapService, BitmapService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IMessageDialogService, MessageDialogService>();
            container.RegisterType<IPictureRepository, PictureRepository>();

            container.RegisterType<IPictureGridViewModel, PictureGridViewModel>();
            container.RegisterType<IFolderTreeViewModel, FolderTreeViewModel>();

            container.RegisterType<IFlyoutViewModel, EditNamesViewModel>();
            container.RegisterType<IFlyoutViewModel, PicturesViewerViewModel>();
            
            //PictureGridViewModel = container.Resolve<PictureGridViewModel>();
            MainViewModel = container.Resolve<MainViewModel>();
            TreePictureFolderViewModel = container.Resolve<FolderTreeViewModel>();

            Container = container;
        }

        public static UnityContainer Container { get; private set; }
    }
}
