using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.ViewModels;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Jw.Vepix.Wpf.Utilities
{
    public class ViewModelLocator
    {
        public VepixWindowViewModel VepixWindowViewModel { get; }
        public PictureGridViewModel PictureGridViewModel {get;}
        public EditNameDialogViewModel EditNameDialogViewModel { get; }

        public ViewModelLocator()
        {
            var container = new UnityContainer();

            container.RegisterType<IFileService, FileService>();
            container.RegisterType<IMessageDialogService, MessageDialogService>();
            container.RegisterType<IPictureRepository, PictureRepository>();
            container.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());

            VepixWindowViewModel = container.Resolve<VepixWindowViewModel>();
            PictureGridViewModel = container.Resolve<PictureGridViewModel>();
            EditNameDialogViewModel = container.Resolve<EditNameDialogViewModel>();
        }
    }
}
