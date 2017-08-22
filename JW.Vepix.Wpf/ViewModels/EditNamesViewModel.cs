using JW.Vepix.Core.Extensions;
using JW.Vepix.Core.Interfaces;
using JW.Vepix.Core.Models;
using JW.Vepix.Wpf.Events;
using JW.Vepix.Wpf.Payloads;
using JW.Vepix.Wpf.Services;
using JW.Vepix.Wpf.Utilities;
using Prism.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace JW.Vepix.Wpf.ViewModels
{
    public class EditNamesViewModel : ViewModelBase, IFlyoutViewModel, IEditNamesViewModel
    {
        private IPictureRepository _pictureRepository;
        private IMessageDialogService _messageDialogService;
        private IEventAggregator _eventAggregator;
        private ObservableCollection<AffixedPictureName> _editPictureNames;
        private string _prefix;
        private string _suffix;
        private bool _isAllPrefixChecked;
        private bool _isAllSuffixChecked;

        public EditNamesViewModel(IPictureRepository pictureRepository,
                                  IMessageDialogService messageDialogService,
                                  IEventAggregator eventAggregator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            _pictureRepository = pictureRepository;
            _messageDialogService = messageDialogService;
            _eventAggregator = eventAggregator;

            _editPictureNames = new ObservableCollection<AffixedPictureName>();

            RemovePictureCommand = new RelayCommand<int>(OnRemovePictureCommand);
            IsAllPrefixesCheckedCommand = new RelayCommand<object>(OnIsAllPrefixesCheckedCommand);
            IsAllSuffixesCheckedCommand = new RelayCommand<object>(OnIsAllSuffixesCheckedCommand);
            OverwriteNamesCommand = new RelayCommand<object>(OnOverwriteNamesCommand,
                                                             OnCanOverwriteNamesCommand);
        }

        public override string ViewTitle => "Edit Picture Names";

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
                        _editPictureNames[i].IsSuffixOn = _isAllSuffixChecked;
                    }
                }
            }
        }

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
                        _editPictureNames[i].IsPrefixOn = _isAllPrefixChecked;
                    }
                }
            }
        }

        public ObservableCollection<AffixedPictureName> EditPictureNames
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

        public void Load(List<Picture> pictures)
        {
            pictures.ToList().ForEach(pic =>
                EditPictureNames.Add(new AffixedPictureName(pic)));
        }

        public RelayCommand<int> RemovePictureCommand { get; private set; }
        public RelayCommand<object> IsAllPrefixesCheckedCommand { get; private set; }
        public RelayCommand<object> IsAllSuffixesCheckedCommand { get; private set; }
        public RelayCommand<object> OverwriteNamesCommand { get; private set; }

        private void OnRemovePictureCommand(int index)
        {
            EditPictureNames.RemoveAt(index);
        }

        private void OnIsAllPrefixesCheckedCommand()
        {
            var val = _editPictureNames.All(name => name.IsPrefixOn);
            if (val != _isAllPrefixChecked)
            {
                _isAllPrefixChecked = val;
                NotifyPropertyChanged(() => IsAllPrefixChecked);
            }
        }

        private void OnIsAllSuffixesCheckedCommand()
        {
            var val = _editPictureNames.All(name => name.IsSuffixOn);
            if (val != _isAllSuffixChecked)
            {
                _isAllSuffixChecked = val;
                NotifyPropertyChanged(() => IsAllSuffixChecked);
            }
        }

        private void OnOverwriteNamesCommand()
        {
            var dirtyNames = GetDirtyObjects().Where(obj => obj is AffixedPictureName)
                                              .Select(obj => (AffixedPictureName)obj)
                                              .ToList();
            
            var allInvalidNames = dirtyNames.Where(name => !name.ToString()
                                                                .IsValidFileNameAndDoesntExist())
                                                                .ToList();

            if (allInvalidNames.Count > 0)
            {
                _messageDialogService.ShowMessage("Invalid file names:",
                                                  string.Join("\r\n", allInvalidNames.Select(name =>
                                                    name.ToString())));
                return;
            }

            dirtyNames.RemoveAll(editName =>
            {
                if (_pictureRepository.TryChangePictureName(
                    editName.Picture, editName.ToString()).Success.Value)
                {
                    _eventAggregator.GetEvent<PictureNameChangedEvent>()
                                    .Publish(new PictureNameChangePayload()
                    {
                        Guid = editName.Picture.Guid,
                        NewPictureName = editName.ToString()
                    });

                    EditPictureNames.Remove(editName);

                    return true;
                }
                else
                {
                    return false;
                }
            });
            
            if (dirtyNames.Count > 0)
            {
                _messageDialogService.ShowMessage("Unable to change names for these:",
                                                  string.Join("\r\n", dirtyNames.Select(name =>
                                                    name.ToString())));
            }
        }

        private bool OnCanOverwriteNamesCommand() => EditPictureNames.Count > 0;
    }
}
