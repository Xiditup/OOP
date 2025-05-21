using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PhoneService.DAL.Entities;
using PhoneService.Models;
using PhoneService.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PhoneService.ViewModels
{
    public class ViewsVM : ViewModelBase
    {
        private readonly Mediator _mediator;
        private readonly UserControl[] _views;

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                SetProperty(ref _currentView, value);
                OnPropertyChanged(nameof(IsNavVisible));
            }
        }

        private User? _user;
        public User? User
        {
            get => _user;
            set
            {
                SetProperty(ref _user, value);
                StaticUser = value;
                _mediator.UpdateUser();
                OnPropertyChanged(nameof(IsClient));
                OnPropertyChanged(nameof(IsMaster));
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public static User? StaticUser { get; set; }

        public ObservableCollection<string> Errors { get; set; } = [];

        public bool IsNavVisible { get; set; } = false;

        public bool IsErrorsVisible { get; set; } = false;

        private RelayCommand _openActiveRequests = null!;
        public RelayCommand OpenActiveRequests => _openActiveRequests ??= new(() => CurrentView = _views[(byte)View.Requests]);

        private RelayCommand _openStorage = null!;
        public RelayCommand OpenStorage => _openStorage ??= new(() => CurrentView = _views[(byte)View.Storage]);

        private RelayCommand _openEmployees = null!;
        public RelayCommand OpenEmployees => _openEmployees ??= new(() => CurrentView = _views[(byte)View.Employee]);

        private RelayCommand _openServices = null!;
        public RelayCommand OpenServices => _openServices ??= new(() => CurrentView = _views[(byte)View.Services]);

        private RelayCommand _openStock = null!;
        public RelayCommand OpenStock => _openStock ??= new(() => CurrentView = _views[(byte)View.Stock]);

        private RelayCommand _createRequest = null!;
        public RelayCommand CreateRequest => _createRequest ??= new(() => CurrentView = _views[(byte)View.CreateRequest]);

        private RelayCommand _openReviews = null!;
        public RelayCommand OpenReviews => _openReviews ??= new(() => CurrentView = _views[(byte)View.Reviews]);
        private RelayCommand _changeTheme = null!;
        public RelayCommand ChangeTheme
        {
            get => _changeTheme ??= new(() =>
            {
                foreach (var view in _views)
                {
                    view.Resources.MergedDictionaries.Clear();
                    if (IsLight)
                    {
                        view.Resources.MergedDictionaries.Add(new ResourceDictionary
                        {
                            Source = new Uri("pack://application:,,,/Resources/DarkTheme.xaml", UriKind.Absolute)
                        });
                    } else
                    {
                        view.Resources.MergedDictionaries.Add(new ResourceDictionary
                        {
                            Source = new Uri("pack://application:,,,/Resources/LightTheme.xaml", UriKind.Absolute)
                        });
                    }
                    view.Resources.MergedDictionaries.Add(new ResourceDictionary
                    {
                        Source = new Uri("pack://application:,,,/Resources/Styles.xaml", UriKind.Absolute)
                    });
                }
                IsLight = !IsLight;
            });
        }
        public bool IsLight { get; set; } = true;

        public bool IsClient => User?.Role == "Client";
        public bool IsMaster => User?.Role == "Master";
        public bool IsAdmin => User?.Role == "Admin";


        private RelayCommand _logout = null!;
        public RelayCommand Logout => _logout ??= new(() =>
        {
            CurrentView = _views[(byte)View.Authorization];
            IsNavVisible = false;
        });

        private RelayCommand _closeErrors = null!;
        public RelayCommand CloseErrors
        {
            get => _closeErrors ??= new(() => { IsErrorsVisible = false; });
        }
        public ViewsVM(
            AuthorizationPage authorization,
            RegistrationPage registration,
            CreateRequestPage createRequest,
            StoragePage storage,
            UserPage employees,
            ServicesPage services,
            StockPage stock,
            RequestsPage requests,
            SingleRequestPage singleRequest,
            Mediator mediator,
            ReviewPage review
        )
        {
            _views = [
                authorization,
                registration,
                createRequest,
                storage,
                employees,
                services,
                stock,
                requests,
                singleRequest,
                review
            ];
            _mediator = mediator;

            _currentView = _views[(byte)View.Authorization];

            Messenger.Default.Register<View>(
                this,
                (view) =>
                {
                    CurrentView = _views[(byte)view];
                    if (view != View.Authorization && view != View.Registration)
                    {
                        IsNavVisible = true;
                    }
                    else
                    {
                        IsNavVisible = false;
                    }
                }
            );

            Messenger.Default.Register<string>(this, (str) =>
            {
                Errors = [.. str.Split('\n')];
                IsErrorsVisible = true;
            });

            Messenger.Default.Register<bool>(this, (canChangeView) =>
            {
                IsNavVisible = canChangeView;
            });

            Messenger.Default.Register<User>(this, (user) =>
            {
                User = user;
            });
        }
    }
}
