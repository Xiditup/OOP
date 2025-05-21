using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.Models;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PhoneService.ViewModels
{
    public class SingleRequestVM : ViewModelBase
    {
        private readonly RequestManager _requestManager;
        private readonly Mediator _mediator;

        private Request? _request;

        public string Name { get; set; } = "";

        public string Device { get; set; } = "";

        public string Service { get; set; } = "";

        public string Response { get; set; } = "";

        public int Price { get; set; } = 0;

        private static IEnumerable<RequestStatus> _statuses = [];

        public IEnumerable<RequestStatus> Statuses { get; set; } = [];

        public RequestStatus Status { get; set; }
        public RequestStatus NewStatus { get; set; } = RequestStatus.Approved;

        public string Description { get; set; } = "";

        public BitmapImage? MainPhoto { get; set; }

        public IEnumerable<BitmapImage> Images { get; set; } = [];

        public IEnumerable<Detail> Details { get; set; } = [];

        public Detail? SelectedDetail { get; set; }

        private string _cost = "0";
        public string Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                if (Regex.IsMatch(value, "^[0-9]*$"))
                {
                    try
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _cost, "0");
                            return;
                        }
                        if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '0' && char.IsDigit(value[1]))
                        {
                            value = value[1..];
                        }
                        Convert.ToInt32(value);
                        SetProperty(ref _cost, value);
                    }
                    catch
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _cost, "");
                        }
                        else
                        {
                            SetProperty(ref _cost, Int32.MaxValue.ToString());
                        }
                    }
                }
                OnPropertyChanged(nameof(Cost));
            }
        }

        private string _detailQuantity = "0";
        public string DetailQuantity
        {
            get => _detailQuantity;
            set
            {
                _detailQuantity = value;
                if (Regex.IsMatch(value, "^[0-9]*$"))
                {
                    try
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _detailQuantity, "0");
                            return;
                        }
                        if (!string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '0' && char.IsDigit(value[1]))
                        {
                            value = value[1..];
                        }
                        Convert.ToInt32(value);
                        SetProperty(ref _detailQuantity, value);
                    }
                    catch
                    {
                        if (value?.Length == 0)
                        {
                            SetProperty(ref _detailQuantity, "");
                        }
                        else
                        {
                            SetProperty(ref _detailQuantity, Int32.MaxValue.ToString());
                        }
                    }
                }
                OnPropertyChanged(nameof(DetailQuantity));
                GetDetail.RaiseCanExecuteChanged();
            }
        }
        public IEnumerable<UsedDetail> UsedDetails { get; set; } = [];

        public BitmapImage? UserAvatar { get; set; }
        public string UserName { get; set; } = "";
        public string UserPhone { get; set; } = "";

        public BitmapImage? MasterAvatar { get; set; }
        public string MasterName { get; set; } = "";
        public string MasterPhone { get; set; } = "";

        private RelayCommand _getDetail = null!;
        public RelayCommand GetDetail
        {
            get => _getDetail ??= new(
                async () =>
                {
                    if (_request == null || SelectedDetail == null) return;
                    var sd = SelectedDetail;
                    await _requestManager.AddDetailsToRequestAsync(_request, SelectedDetail, Convert.ToInt32(DetailQuantity));
                    Update();
                    SelectedDetail = sd;
                    GetDetail.RaiseCanExecuteChanged();
                    _mediator.UpdateDetails();
                }, () => Convert.ToInt32(DetailQuantity) <= SelectedDetail?.Quantity && Convert.ToInt32(DetailQuantity) != 0);
        }

        public bool IsControlVisible => _request?.MasterId == ViewsVM.StaticUser?.Id && _request?.MasterId != null && _request.Status != RequestStatus.Closed && _request.Status != RequestStatus.Canceled;
        public bool CanBecameMaster => _request?.MasterId == null && ViewsVM.StaticUser?.Role != "Client" && _request?.Status == RequestStatus.Created;
        public bool CanCancelRequest => _request?.MasterId == ViewsVM.StaticUser?.Id && (_request?.CancelRequired ?? false);

        private RelayCommand _cancelRequest = null!;
        public RelayCommand CancelRequest
        {
            get => _cancelRequest ??= new(
                async () =>
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
                        if (_request == null) return;
                        await _requestManager.CancelRequestAsync(_request, true);
                        _mediator.UpdateRequests();
                        OnPropertyChanged(nameof(IsControlVisible));
                        Status = RequestStatus.Canceled;
                    }
                });
        }

        private RelayCommand _setRequestMaster = null!;
        public RelayCommand SetRequestMaster
        {
            get => _setRequestMaster ??= new(
                async () =>
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
                        if (_request == null) return;
                        if (ViewsVM.StaticUser == null) return;
                        await _requestManager.SetRequestMasterAsync(_request, ViewsVM.StaticUser);
                        NewStatus = RequestStatus.Approved;
                        Status = RequestStatus.Approved;
                        MasterAvatar = new BitmapImage(PhotoBrowser.GetFullPathUri(_request.Master?.AvatarPath ?? ""));
                        MasterName = _request.Master?.Name ?? "";
                        MasterPhone = _request.Master?.Phone ?? "";
                        OnPropertyChanged(nameof(IsControlVisible));
                        OnPropertyChanged(nameof(CanBecameMaster));
                        Statuses = _statuses.Where(s => s >= RequestStatus.Approved);
                        _mediator.UpdateRequests();
                    }
                });
        }

        private RelayCommand _save = null!;
        public RelayCommand Save
        {
            get => _save ??= new(
                async () =>
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
                        if (_request == null) return;
                        Status = NewStatus;
                        await _requestManager.SaveRequestAsync(_request, Status, Response, Convert.ToInt32(Cost));
                        Statuses = _statuses.Where(s => s >= Status);
                        Price = Convert.ToInt32(Cost);
                        _mediator.UpdateRequests();
                        OnPropertyChanged(nameof(IsControlVisible));
                    }
                });
        }

        public SingleRequestVM(RequestManager requestManager, Mediator mediator)
        {
            _requestManager = requestManager;
            _mediator = mediator;

            _statuses = [
                RequestStatus.Created,
                RequestStatus.Canceled,
                RequestStatus.Approved,
                RequestStatus.InProgress,
                RequestStatus.Completed,
                RequestStatus.Closed
            ];

            Messenger.Default.Register<Request>(
                this,
                (request) =>
                {
                    _request = request;
                    Update();
                }
            );

            _mediator.UpdateDetailsEvent += Update;
        }

        public void Update()
        {
            if (_request == null) return;
            _request = _requestManager.GetFullRequest(_request);
            Name = _request.Name;
            Device = _request.Device;
            Status = _request.Status;
            NewStatus = Status;
            Price = _request.Cost;
            Cost = Price.ToString();
            Service = _request.Service.Name;
            Description = _request.Description;
            Response = _request.Response;
            Images = _request.ImagePathes.Select(p => new BitmapImage(PhotoBrowser.GetFullPathUri(p)));
            MainPhoto = Images.ElementAt(0);
            UsedDetails = [.. _request.UsedDetails];
            UserAvatar = new BitmapImage(PhotoBrowser.GetFullPathUri(_request.Client.AvatarPath));
            UserName = _request.Client.Name;
            UserPhone = _request.Client.Phone;
            Details = _requestManager.GetAllDetails().Where(d => d.Quantity > 0);
            SelectedDetail = null;
            if (_request.Master != null)
            {
                MasterAvatar = new BitmapImage(PhotoBrowser.GetFullPathUri(_request.Master.AvatarPath));
                MasterName = _request.Master?.Name ?? "";
                MasterPhone = _request.Master?.Phone ?? "";
            }
            else
            {
                MasterAvatar = null;
                MasterName = "";
                MasterPhone = "";
            }
            Statuses = _statuses.Where(s => s >= _request.Status);
            OnPropertyChanged(nameof(IsControlVisible));
            OnPropertyChanged(nameof(CanBecameMaster));
            OnPropertyChanged(nameof(CanCancelRequest));

            RequestStatus buffer = NewStatus;
            NewStatus = RequestStatus.Created;
            NewStatus = RequestStatus.Canceled;
            NewStatus = buffer;
        }
    }
}
