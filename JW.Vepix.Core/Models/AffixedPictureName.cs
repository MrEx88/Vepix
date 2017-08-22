
namespace JW.Vepix.Core.Models
{
    public class AffixedPictureName : ObjectBase
    {
        private string _prefix;
        private string _name;
        private string _suffix;
        private bool _isPrefixOn;
        private bool _isSuffixOn;

        public AffixedPictureName(Picture picture)
        {
            Picture = picture;
            _name = picture.ImageName;
        }

        public Picture Picture { get; }

        public string Prefix
        {
            get { return IsPrefixOn ? _prefix : string.Empty; }
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
            get { return IsSuffixOn ? _suffix : string.Empty; }
            set
            {
                if (value != _suffix)
                {
                    _suffix = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsPrefixOn
        {
            get { return _isPrefixOn; }
            set
            {
                if (value != _isPrefixOn)
                {
                    _isPrefixOn = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => Prefix);
                }
            }
        }

        public bool IsSuffixOn
        {
            get { return _isSuffixOn; }
            set
            {
                if (value != _isSuffixOn)
                {
                    _isSuffixOn = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(() => Suffix);
                }
            }
        }

        public override string ToString()
        {
            var prefix = IsPrefixOn ? Prefix : string.Empty;
            var suffix = IsSuffixOn ? Suffix : string.Empty;
            return prefix + _name + suffix;
        }
    }
}
