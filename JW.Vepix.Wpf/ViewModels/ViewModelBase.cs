using JW.Vepix.Core;

namespace JW.Vepix.Wpf.ViewModels
{
    public class ViewModelBase : ObjectBase
    {
        public virtual string ViewTitle
        {
            get { return string.Empty; }
            protected set { }
        }
    }
}
