using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Jw.Vepix.Wpf.Utilities
{
    public class ViewModelLocator
    {
        public EditNameDialogViewModel EditNameDialogViewModel { get; }
        public PictureGridViewModel PictureGridViewModel { get; }
        public VepixWindowViewModel VepixWindowViewModel { get; }

        public ViewModelLocator()
        {
            var container = new UnityContainer();

            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IMessageDialogService, MessageDialogService>();
            container.RegisterType<IPictureRepository, PictureRepository>();

            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());

            EditNameDialogViewModel = container.Resolve<EditNameDialogViewModel>();
            PictureGridViewModel = container.Resolve<PictureGridViewModel>();
            VepixWindowViewModel = container.Resolve<VepixWindowViewModel>();
        }
    }
}
