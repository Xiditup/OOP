using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.Commands;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace PhoneService.ViewModels
{
    public class StockVM : ViewModelBase
    {
        private readonly StockManager _stockManager;
        private readonly Mediator _mediator;

        public ObservableCollection<Stock> Stocks { get; set; } = [];

        public IEnumerable<Service> Services { get; set; } = [];
        public IEnumerable<StockType> StockTypes { get; set; } = [
            StockType.Discount,
            StockType.Gift,
            StockType.NewClient,
            StockType.Giveaway,
        ];

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public FileInfo? Image { get; set; }
        public StockType SelectedStockType { get; set; }
        public Service? SelectedService { get; set; } = null;

        public bool WithoutService { get; set; } = false;

        private string _percentage = "0";
        public string Percentage
        {
            get => _percentage;
            set
            {
                if (Regex.IsMatch(value, "^[0-9]*$"))
                {
                    try
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _percentage, "0");
                            return;
                        }
                        if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '0' && char.IsDigit(value[1]))
                        {
                            value = value[1..];
                        }
                        int res = Convert.ToInt32(value);
                        if (res > 100)
                        {
                            res = 100;
                        }
                        SetProperty(ref _percentage, res.ToString());
                    }
                    catch
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _percentage, "");
                        }
                        else
                        {
                            SetProperty(ref _percentage, Int32.MaxValue.ToString());
                        }
                    }
                }
                OnPropertyChanged(nameof(Percentage));
            }
        }

        public bool IsAdmin { get; set; } = false;
        public bool IsEditing { get; set; } = false;

        private RelayCommand _choosePhoto = null!;
        public RelayCommand ChoosePhoto
        {
            get => _choosePhoto ??= new(
                () => Image = PhotoBrowser.GetPhoto());
        }

        private RelayCommand _create = null!;
        public RelayCommand Create
        {
            get => _create ??= new(async () =>
            {
                if (Image == null) return;
                var discount = await _stockManager.CreateStock(
                    Name,
                    Description,
                    Image,
                    SelectedStockType,
                    Convert.ToInt32(Percentage),
                    SelectedService,
                    WithoutService
                );
                Stocks = [discount, .. Stocks];
                Name = "";
                Description = "";
                SelectedService = null;
                Image = null;
            },
            () => (SelectedService != null || WithoutService) && Name != "" && Description != "" && Image != null);
        }

        private RelayCommand<Stock> _remove = null!;
        public RelayCommand<Stock> Remove
        {
            get => _remove ??= new(
                async (stock) =>
                {
                    if (stock == null) return;
                    await _stockManager.DeleteStockAsync(stock);
                    Stocks.Remove(stock);
                    _mediator.UpdateServices();
                });
        }

        private RelayCommand _save = null!;
        public RelayCommand Save => _save ??= new(
            async () =>
            {
                await _stockManager.SaveChangesAsync(Stocks);
                Messenger.Default.Send(true);
                IsEditing = false;
            });

        public StockVM(StockManager discountManager, Mediator mediator)
        {
            _stockManager = discountManager;
            _mediator = mediator;
            Update();
            _mediator.UpdateServicesEvent += Update;
            _mediator.UpdateUserEvent += UpdateUser;
        }

        public void Update()
        {
            Stocks = [.. _stockManager.GetAllStocks()];
            Services = _stockManager.GetAllServices();
            StockType buffer = SelectedStockType;
            StockTypes = [.. StockTypes];
            SelectedStockType = StockType.Giveaway;
            SelectedStockType = StockType.NewClient;
            SelectedStockType = buffer;
        }

        public void UpdateUser()
        {
            if (ViewsVM.StaticUser != null)
            {
                IsAdmin = ViewsVM.StaticUser.Role == "Admin";
            }
        }
    }
}
