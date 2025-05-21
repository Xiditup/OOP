using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.DAL.Entities;
using PhoneService.DAL.Models;
using PhoneService.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhoneService.ViewModels
{
    public class RequestsVM : ViewModelBase
    {
        private readonly RequestManager _requestManager;
        private readonly Mediator _mediator;

        public string RequestName { get; set; } = "";

        public string DeviceName { get; set; } = "";

        public ObservableCollection<Service> Services { get; set; } = [];

        public Service? SelectedService { get; set; }

        public IEnumerable<RequestStatus> Statuses { get; set; }

        public RequestStatus SelectedStatus { get; set; } = RequestStatus.All;

        public bool OnlyActive { get; set; } = false;

        public bool OnlyWithoutMaster { get; set; } = false;
        public bool OnlyCancelRequired { get; set; } = false;

        public bool ShowCanceled { get; set; } = false;

        public bool CanCancel => ViewsVM.StaticUser?.Role == "Client";

        public ObservableCollection<Request> Requests { get; set; } = [];

        public BitmapImage? UserAvatar
        {
            get
            {
                if (ViewsVM.StaticUser == null) return null;
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                path = Path.Combine(path, "PhoneService");
                var uri = new Uri(Path.Combine(path, ViewsVM.StaticUser.AvatarPath));
                return new BitmapImage(uri);
            }
        }
        public string UserName => ViewsVM.StaticUser != null ? ViewsVM.StaticUser.Name : "";
        public string UserRole
        {
            get {
                var role = ViewsVM.StaticUser?.Role;
                if (MainWindow.IsRu)
                {
                    return role switch
                    {
                        "Client" => "Клиент",
                        "Employee" => "Сотрудник",
                        _ => "Администратор",
                    };
                }
                else
                {
                    return role switch
                    {
                        "Client" => "Cleint",
                        "Employee" => "Employeee",
                        _ => "Administrator",
                    };
                }
            }
        }

        private RelayCommand<Request> _cancel = null!;
        public RelayCommand<Request> Cancel
        {
            get => _cancel ??= new(
                async (request) =>
                {
                    DoFilter.Execute(null);
                    await _requestManager.CancelRequestAsync(request);
                });
        }

        private RelayCommand _doFilter = null!;
        public RelayCommand DoFilter
        {
            get => _doFilter ??= new(
                async () =>
                {
                    var newRequests = await _requestManager.GetRequestsByFiltersAsync(
                                            ViewsVM.StaticUser!,
                                            RequestName,
                                            DeviceName,
                                            SelectedService,
                                            SelectedStatus,
                                            OnlyActive,
                                            OnlyWithoutMaster,
                                            OnlyCancelRequired
                                        );
                    Requests = [.. newRequests];
                });
        }

        private RelayCommand _doReset = null!;
        public RelayCommand DoReset
        {
            get => _doReset ??= new(
                () =>
                {
                    RequestName = "";
                    DeviceName = "";
                    SelectedService = null;
                    SelectedStatus = RequestStatus.All;
                    OnlyActive = false;
                    OnlyWithoutMaster = false;
                    Update();
                });
        }

        private RelayCommand<Request> _openRequest = null!;
        public RelayCommand<Request> OpenRequest
        {
            get => _openRequest ??= new(
                (request) =>
                {
                    Messenger.Default.Send(View.SingleRequest);
                    Messenger.Default.Send(request);
                });
        }

        public RequestsVM(RequestManager requestManager, Mediator mediator)
        {
            _requestManager = requestManager;
            _mediator = mediator;
            Services = [.. _requestManager.GetAllServices()];
            Statuses = [
                RequestStatus.All,
                RequestStatus.Created,
                RequestStatus.Canceled,
                RequestStatus.Approved,
                RequestStatus.InProgress,
                RequestStatus.Completed,
                RequestStatus.Closed
            ];
            _mediator.UpdateUserEvent += Update;
            _mediator.UpdateRequestsEvent += Update;
            _mediator.UpdateServicesEvent += Update;
        }

        public async void Update()
        {
            if (ViewsVM.StaticUser != null)
            {
                var newRequests = await _requestManager.GetRequestsByFiltersAsync(
                        ViewsVM.StaticUser,
                        RequestName,
                        DeviceName,
                        SelectedService,
                        SelectedStatus,
                        OnlyActive,
                        OnlyWithoutMaster,
                        OnlyCancelRequired
                    );
                Requests = [.. newRequests];
            }
            Services = [.. _requestManager.GetAllServices()];
            RequestStatus buffer = SelectedStatus;
            SelectedStatus = RequestStatus.Created;
            SelectedStatus = RequestStatus.Canceled;
            SelectedStatus = buffer;
            OnPropertyChanged(nameof(UserAvatar));
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(UserRole));
            OnPropertyChanged(nameof(CanCancel));
        }
    }
}
