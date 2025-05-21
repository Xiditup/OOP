using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.DAL.Entities;
using PhoneService.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace PhoneService.ViewModels
{
    public class StorageVM : ViewModelBase
    {
        private readonly StorageManager _storageManager;
        private readonly Mediator _mediator;

        public ObservableCollection<Detail> Details { get; set; } = [];

        public string Name { get; set; } = "";
        public bool IsEditing { get; set; } = false;

        private string _quantity = "0";
        public string Quantity
        {
            get => _quantity;
            set
            {
                if (Regex.IsMatch(value, @"^[0-9]*$"))
                {
                    try
                    {
                        if (value == "")
                        {
                            SetProperty(ref _quantity, "0");
                            return;
                        }
                        if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '0' && char.IsDigit(value[1]))
                        {
                            value = value.Substring(1);
                        }
                        Convert.ToInt32(value);
                        SetProperty(ref _quantity, value);
                    }
                    catch
                    {
                        if (value == "")
                        {
                            SetProperty(ref _quantity, "");
                        }
                        else
                        {
                            SetProperty(ref _quantity, Int32.MaxValue.ToString());
                        }
                    }
                }
                OnPropertyChanged(nameof(Quantity));
            }
        }

        private RelayCommand<string> _search = null!;
        public RelayCommand<string> Search
        {
            get => _search ??= new(
                (name) =>
                {
                    Details = [.. _storageManager.GetDetailsByName(name)];
                    Details.CollectionChanged += DeleteDetail;
                });
        }

        private RelayCommand _create = null!;
        public RelayCommand Create
        {
            get => _create ??= new(async () =>
            {
                var detail = await _storageManager.CreateDetail(Name, Convert.ToInt32(Quantity));
                _mediator.UpdateDetails();
            });
        }

        private RelayCommand _save = null!;
        public RelayCommand Save
        {
            get => _save ??= new(async () =>
            {
                await _storageManager.SaveChangesAsync(Details);
                _mediator.UpdateDetails();
                Messenger.Default.Send(true);
                IsEditing = false;
            });
        }

        public StorageVM(StorageManager storageManager, Mediator mediator)
        {
            _storageManager = storageManager;
            _mediator = mediator;
            _mediator.UpdateDetailsEvent += () => Details = [.. _storageManager.GetAllDetails()];
            Details = [.. _storageManager.GetAllDetails()];
            Details.CollectionChanged += DeleteDetail;
        }

        private async void DeleteDetail(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        await _storageManager.DeleteDetailAsync((Detail)item);
                    }
                    _mediator.UpdateDetails();
                }
            }
        }
    }
}
