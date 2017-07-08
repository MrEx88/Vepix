using J.Vepix.Core.Interfaces;
using J.Vepix.Core.Services;
using J.Vepix.Infrastructure.Data;
using J.Vepix.Wpf.Services;
using J.Vepix.Wpf.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace J.Vepix.Wpf.Utilities
{
    public class ViewModelLocator
    {
        //public PictureGridViewModel PictureGridViewModel { get; }
        public MainViewModel MainViewModel { get; }
        public PictureFolderTreeViewModel TreePictureFolderViewModel { get; }

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
            container.RegisterType<IPictureFolderTreeViewModel, PictureFolderTreeViewModel>();

            container.RegisterType<ICollectionViewModel, EditNamesViewModel>();
            container.RegisterType<ICollectionViewModel, PicturesViewerViewModel>();
            
            //PictureGridViewModel = container.Resolve<PictureGridViewModel>();
            MainViewModel = container.Resolve<MainViewModel>();
            TreePictureFolderViewModel = container.Resolve<PictureFolderTreeViewModel>();

            Container = container;
        }

        public static UnityContainer Container { get; private set; }
    }
}
