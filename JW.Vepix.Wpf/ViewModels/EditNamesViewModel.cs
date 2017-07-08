using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Payloads;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JW.Vepix.Wpf.ViewModels
{
    public class EditNamesViewModel : ViewModelBase, ICollectionViewModel
    {
        public EditNamesViewModel(IPictureRepository pictureRepository,
                                  IMessageDialogService messageDialogService,
                                  IEventAggregator eventAggregator)
        {
            _pictureRepository = pictureRepository;
            _messageDialogService = messageDialogService;
            _eventAggregator = eventAggregator;

            RemovePictureCommand = new RelayCommand<int>(OnRemovePictureCommand);
            IsAllPrefixesCheckedCommand = new RelayCommand<object>(OnIsAllPrefixesCheckedCommand);
            IsAllSuffixesCheckedCommand = new RelayCommand<object>(OnIsAllSuffixesCheckedCommand);
            OverwriteNamesCommand = new RelayCommand<object>(OnOverwriteNamesCommand, OnCanOverwriteNamesCommand);
        }
        
        private void OnRemovePictureCommand(int index)
        {
            Pictures.RemoveAt(index);
            EditPictureNames.RemoveAt(index);
        }

        private void OnIsAllPrefixesCheckedCommand()
        {
            var val = _editPictureNames.All(name => name.IsPrefixChecked);
            if (val != _isAllPrefixChecked)
            {
                _isAllPrefixChecked = val;
                NotifyPropertyChanged("IsAllPrefixChecked");
            }
        }

        private void OnIsAllSuffixesCheckedCommand()
        {
            var val = _editPictureNames.All(name => name.IsSuffixChecked);
            if (val != _isAllSuffixChecked)
            {
                _isAllSuffixChecked = val;
                NotifyPropertyChanged("IsAllSuffixChecked");
            }
        }

        private void OnOverwriteNamesCommand()
        {
            for (int i = 0; i < Pictures.Count; i ++)
            {
                if (_pictureRepository.TryChangePictureName(Pictures[i], EditPictureNames[i].ToString()))
                {
                    _eventAggregator.GetEvent<PictureNameChangedEvent>().Publish(new PictureNameChangePayload()
                    {
                        Guid = Pictures[i].Guid,
                        NewPictureName = EditPictureNames[i].ToString()
                    });
                }
                else
                {
                    //todo: maybe display a list of failed filenames
                    //      and remove the successfully overwritten names from the list
                } 
            }
        }
        private bool OnCanOverwriteNamesCommand()
        {
            //todo also check for valid file names
            return EditPictureNames.Count > 0;
        }


        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (value != _prefix)
                {
                    _prefix = value;
                    for (int i = 0; i < _editPictureNames.Count; i++)
                    {
                        _editPictureNames[i].Prefix = _prefix;
                    }
                    NotifyPropertyChanged(() => EditPictureNames);
                }
            }
        }

        public string Suffix
        {
            get { return _suffix; }
            set
            {
                if (value != _suffix)
                {
                    _suffix = value;
                    for (int i = 0; i < _editPictureNames.Count; i++)
                    {
                        _editPictureNames[i].Suffix = _suffix;
                    }
                    NotifyPropertyChanged(() => EditPictureNames);
                }
            }
        }

        private bool _isAllSuffixChecked;

        public bool IsAllSuffixChecked
        {
            get { return _isAllSuffixChecked; }
            set
            {
                if (value != _isAllSuffixChecked)
                {
                    _isAllSuffixChecked = value;
                    NotifyPropertyChanged();
                    for (int i = 0; i < _editPictureNames.Count; i++)
                    {
                        _editPictureNames[i].IsSuffixChecked = _isAllSuffixChecked;
                    }
                }
            }
        }

        private bool _isAllPrefixChecked;

        public bool IsAllPrefixChecked
        {
            get { return _isAllPrefixChecked; }
            set
            {
                if (value != _isAllPrefixChecked)
                {
                    _isAllPrefixChecked = value;
                    NotifyPropertyChanged();
                    for (int i = 0; i < _editPictureNames.Count; i++)
                    {
                        _editPictureNames[i].IsPrefixChecked = _isAllPrefixChecked;
                    }
                }
            }
        }

        public ObservableCollection<Picture> Pictures
        {
            get { return _pictures; }
            set
            {
                if (value != _pictures)
                {
                    _pictures = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<AffixedName> EditPictureNames
        {
            get { return _editPictureNames; }
            set
            {
                if (value != _editPictureNames)
                {
                    _editPictureNames = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ViewTitle => "Edit Picture Names";

        public RelayCommand<int> RemovePictureCommand { get; private set; }
        public RelayCommand<object> IsAllPrefixesCheckedCommand { get; private set; }
        public RelayCommand<object> IsAllSuffixesCheckedCommand { get; private set; }
        public RelayCommand<object> OverwriteNamesCommand { get; private set; }

        public void Load(List<Picture> pictures)
        {
            Pictures = new ObservableCollection<Picture>(pictures);

            EditPictureNames = new ObservableCollection<AffixedName>();
            Pictures.ToList().ForEach(pic =>
                EditPictureNames.Add(new AffixedName(pic.ImageName)));
        }

        private IPictureRepository _pictureRepository;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private ObservableCollection<Picture> _pictures;
        private ObservableCollection<AffixedName> _editPictureNames;
        private string _prefix;
        private string _suffix;
    }
}
