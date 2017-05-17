using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jw.Vepix.Data
{
    public class EditNameAffix : INotifyPropertyChanged
    {
        public EditNameAffix(string name)
        {
            _name = name;
        }

        public string Prefix
        {
            get { return IsPrefixChecked ? _prefix : string.Empty; }
            set
            {
                if(value != _prefix)
                {
                    _prefix = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Suffix
        {
            get { return IsSuffixChecked ? _suffix : string.Empty; }
            set
            {
                if (value != _suffix)
                {
                    _suffix = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsPrefixChecked
        {
            get { return _isPrefixChecked; }
            set
            {
                if (value != _isPrefixChecked)
                {
                    _isPrefixChecked = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Prefix");
                }
            }
        }
        public bool IsSuffixChecked
        {
            get { return _isSuffixChecked; }
            set
            {
                if (value != _isSuffixChecked)
                {
                    _isSuffixChecked = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("Suffix");
                }
            }
        }

        private string _prefix;
        private string _name;
        private string _suffix;
        private bool _isPrefixChecked;
        private bool _isSuffixChecked;

        public override string ToString()
        {
            var prefix = IsPrefixChecked ? Prefix : string.Empty;
            var suffix = IsSuffixChecked ? Suffix : string.Empty;
            return prefix + _name + suffix;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
