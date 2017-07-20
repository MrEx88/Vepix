using JW.Vepix.Core;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
