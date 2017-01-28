using Jw.Vepix.Wpf.Services;
using Jw.Vepix.Wpf.ViewModels;

namespace Jw.Vepix.Wpf.Utilities
{
    public class ViewModelLocator
    {
        public static VepixWindowViewModel VepixWindowViewModel =>_vepixWindowViewModel;
        public static ThumbnailGridViewModel ThumbnailGridViewModel => _thumbnailGridViewModel;
        public static ThumbnailViewModel ThumbnailViewModel => _thumbnailViewModel;
        public static EditNameDialogViewModel EditNameDialogViewModel => _editNameDialogViewModel;
        public static IDialogService modalDialog = new ModalDialogService();

        private static VepixWindowViewModel _vepixWindowViewModel = new VepixWindowViewModel();
        private static ThumbnailGridViewModel _thumbnailGridViewModel = new ThumbnailGridViewModel(modalDialog);
        private static ThumbnailViewModel _thumbnailViewModel = new ThumbnailViewModel();
        private static EditNameDialogViewModel _editNameDialogViewModel = new EditNameDialogViewModel();
    }
}
