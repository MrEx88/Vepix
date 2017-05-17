using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Jw.Vepix.Wpf.Utilities
{
    public class ViewModelLocator
    {
        public EditNameDialogViewModel EditNameDialogViewModel { get; }
        //public PictureGridViewModel PictureGridViewModel { get; }
        public PictureFolderTreeViewModel TreePictureFolderViewModel { get; }
        public VepixWindowViewModel VepixWindowViewModel { get; }

        public ViewModelLocator()
        {
            var container = new UnityContainer();

            container.RegisterType<IFileExplorerDialogService, FileExplorerDialogService>();
            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IMessageDialogService, MessageDialogService>();
            container.RegisterType<IPictureRepository, PictureRepository>();

            container.RegisterType<EditNameDialogViewModel>();
            container.RegisterType<IPictureGridViewModel, PictureGridViewModel>();
            container.RegisterType<IPictureFolderTreeViewModel, PictureFolderTreeViewModel>();

            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());

            //EditNameDialogViewModel = container.Resolve<EditNameDialogViewModel>();
            //PictureGridViewModel = container.Resolve<PictureGridViewModel>();
            TreePictureFolderViewModel = container.Resolve<PictureFolderTreeViewModel>();
            VepixWindowViewModel = container.Resolve<VepixWindowViewModel>();

            Container = container;
        }

        public static UnityContainer Container { get; private set; }
    }
}
