using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.Commands;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PhoneService.ViewModels
{
    public class CreateRequestVM : ViewModelBase
    {
        private readonly RequestManager _requestManager;
        private readonly Mediator _mediator;

        public string Name { get; set; } = "";

        public string Device { get; set; } = "";

        public string Description { get; set; } = "";
        public bool HasDiscount { get; set; } = false;

        public IEnumerable<ServiceCategory> ServiceCategories { get; set; } = [
            ServiceCategory.All,
            ServiceCategory.Repair,
            ServiceCategory.Replacement,
            ServiceCategory.Maintenance,
            ServiceCategory.Diagnostics,
            ServiceCategory.Recovery,
            ServiceCategory.Update,
            ServiceCategory.Unlocking,
        ];

        private ServiceCategory _selectedServiceCategory = ServiceCategory.All;
        public ServiceCategory SelectedServiceCategory
        {
            get => _selectedServiceCategory;
            set
            {
                SetProperty(ref _selectedServiceCategory, value);
                Services = [.. _requestManager.GetServicesByCategory(value)];
                SelectedService = null;
                HasDiscount = false;
            }
        }

        private Service? _selectedService;
        public Service? SelectedService
        {
            get => _selectedService;
            set
            {
                SetProperty(ref _selectedService, value);
                if (_selectedService?.Stocks.Any(s => s.Type == StockType.Discount) == true)
                {
                    HasDiscount = true;
                }
            }
        }

        public ObservableCollection<FileInfo> Photos { get; set; } = [];

        public ObservableCollection<Service> Services { get; set; } = [];

        public BitmapImage? MainPhoto => Photos.Count > 0 ? new(PhotoBrowser.GetFullPathUri(Photos[0].FullName)) : null;

        private RelayCommand _addPhoto = null!;
        public RelayCommand AddPhoto
        {
            get => _addPhoto ??= new(
                    () =>
                    {
                        var photo = PhotoBrowser.GetPhoto();
                        if (photo != null)
                        {
                            Photos.Add(photo);
                        }
                    }, () => Photos.Count < 4
                );
        }

        private RelayCommand _addMainPhoto = null!;
        public RelayCommand AddMainPhoto
        {
            get => _addMainPhoto ??= new(
                    () =>
                    {
                        var photo = PhotoBrowser.GetPhoto();
                        if (photo != null)
                        {
                            if (Photos.Count > 0)
                            {
                                Photos.RemoveAt(0);
                            }
                            Photos.Insert(0, photo);
                        }
                    }
                );
        }

        private RelayCommand<int> _removePhoto = null!;
        public RelayCommand<int> RemovePhoto => _removePhoto ??= new(Photos.RemoveAt);

        private RelayCommand _create = null!;
        public RelayCommand Create
        {
            get => _create ??= new(async () =>
            {
                if (SelectedService != null && ViewsVM.StaticUser != null)
                {
                    MessageBoxResult result;
                    if (MainWindow.IsRu)
                    {
                        result = MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }
                    else
                    {
                        result = MessageBox.Show("Are you sure?", "Application", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    }

                    if (result == MessageBoxResult.Yes)
                    {
                        await _requestManager.CreateRequest(Name, Description, Device, Photos, SelectedService, ViewsVM.StaticUser);
                        _mediator.UpdateRequests();
                        Messenger.Default.Send(View.Requests);
                        Reset();
                    }
                }
            }, () =>
            {
                return SelectedService != null &&
                       Name != "" &&
                       Device != "" &&
                       Photos.Count > 0 &&
                       Description != "";
            });
        }

        public CreateRequestVM(RequestManager requestManager, Mediator mediator)
        {
            _requestManager = requestManager;
            Update();
            Photos.CollectionChanged += (s, e) => OnPropertyChanged(nameof(MainPhoto));
            _mediator = mediator;
            _mediator.UpdateServicesEvent += Update;
            _mediator.UpdateUserEvent += Reset;
        }

        public void Update()
        {
            Services = [.. _requestManager.GetAllServices()];
            SelectedService = Services.FirstOrDefault(s => s.Id == SelectedService?.Id);
        }

        public void Reset()
        {
            Name = "";
            Device = "";
            SelectedService = null;
            Description = "";
            Photos = [];
            HasDiscount = false;
            Photos.CollectionChanged += (s, e) => OnPropertyChanged(nameof(MainPhoto));
            OnPropertyChanged(nameof(MainPhoto));
            ServiceCategory buffer = SelectedServiceCategory;
            SelectedServiceCategory = ServiceCategory.Repair;
            SelectedServiceCategory = ServiceCategory.Replacement;
            SelectedServiceCategory = buffer;
        }
    }
}
