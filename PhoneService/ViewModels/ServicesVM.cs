using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.Commands;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace PhoneService.ViewModels
{
    public class ServicesVM : ViewModelBase
    {
        private readonly ServiceManager _serviceManager;
        private readonly Mediator _mediator;

        public ObservableCollection<Service> Services { get; set; } = [];
        public IEnumerable<ServiceCategory> CategoriesFilter { get; set; } = [
            ServiceCategory.All,
            ServiceCategory.Repair,
            ServiceCategory.Replacement,
            ServiceCategory.Maintenance,
            ServiceCategory.Diagnostics,
            ServiceCategory.Recovery,
            ServiceCategory.Update,
            ServiceCategory.Unlocking,
        ];
        public IEnumerable<ServiceCategory> Categories { get; set; } = [
            ServiceCategory.Repair,
            ServiceCategory.Replacement,
            ServiceCategory.Maintenance,
            ServiceCategory.Diagnostics,
            ServiceCategory.Recovery,
            ServiceCategory.Update,
            ServiceCategory.Unlocking,
        ];

        public string Name { get; set; } = "";

        private string _price = "0";
        public ServiceCategory SelectedCategory { get; set; } = ServiceCategory.Repair;
        public string Price
        {
            get => _price;
            set
            {
                if (Regex.IsMatch(value, "^[0-9]*$"))
                {
                    try
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _price, "0");
                            return;
                        }
                        if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '0' && char.IsDigit(value[1]))
                        {
                            value = value[1..];
                        }
                        Convert.ToInt32(value);
                        SetProperty(ref _price, value);
                    }
                    catch
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _price, "0");
                        }
                        else
                        {
                            SetProperty(ref _price, Int32.MaxValue.ToString());
                        }
                    }
                }
                OnPropertyChanged(nameof(Price));
            }
        }
        public string SearchName { get; set; } = "";
        public ServiceCategory SelectedFilterCategory { get; set; } = ServiceCategory.All;

        public bool IsEditing { get; set; } = false;

        private RelayCommand _create = null!;
        public RelayCommand Create
        {
            get => _create ??= new(
                async () =>
                {
                    var service = await _serviceManager.CreateNewService(Name, Convert.ToInt32(Price), SelectedCategory);
                    _mediator.UpdateServices();
                    Search.Execute(null);
                    Name = "";
                }, () => Name != "");
        }

        private RelayCommand _save = null!;
        public RelayCommand Save
        {
            get => _save ??= new(
                async () =>
                {
                    await _serviceManager.SaveChangesAsync(Services);
                    _mediator.UpdateServices();
                    Messenger.Default.Send(true);
                    IsEditing = false;
                });
        }

        private RelayCommand _search = null!;
        public RelayCommand Search
        {
            get => _search ??= new(
                () =>
                {
                    Services = [.. _serviceManager.GetServicesWithNameAndCategory(SearchName, SelectedFilterCategory)];
                    Services.CollectionChanged += DeleteService;
                });
        }

        public ServicesVM(ServiceManager servicesService, Mediator mediator)
        {
            _serviceManager = servicesService;
            _mediator = mediator;
            Services = [.. _serviceManager.GetAllServices()];
            Services.CollectionChanged += DeleteService;
            _mediator.UpdateUserEvent += Update;
        }

        public void Update()
        {
            ServiceCategory buffer = SelectedCategory;
            SelectedCategory = ServiceCategory.Repair;
            SelectedCategory = ServiceCategory.Replacement;
            SelectedCategory = buffer;

            ServiceCategory buffer2 = SelectedFilterCategory;
            SelectedFilterCategory = ServiceCategory.Repair;
            SelectedFilterCategory = ServiceCategory.Replacement;
            SelectedFilterCategory = buffer2;

            CategoriesFilter = [.. CategoriesFilter];
            Categories = [.. Categories];
        }

        private async void DeleteService(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        await _serviceManager.DeleteServiceAsync((Service)item);
                    }
                    _mediator.UpdateServices();
                }
            }
        }
    }
}
