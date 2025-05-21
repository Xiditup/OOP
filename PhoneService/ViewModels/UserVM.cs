using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.Commands;
using PhoneService.DAL.Entities;
using PhoneService.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;

namespace PhoneService.ViewModels
{
    public class UserVM : ViewModelBase
    {
        private readonly UserManager _userManager;
        private readonly Mediator _mediator;

        public ObservableCollection<User> Users { get; set; } = [];

        public FileInfo? Avatar { get; set; } = null;

        public string Name { get; set; } = "";

        public string Login { get; set; } = "";

        public string Password { get; set; } = "";

        public bool IsEmployee { get; set; } = false;
        public string SearchName { get; set; } = "";
        public string SearchLogin { get; set; } = "";
        public bool ShowEmployees { get; set; } = true;
        public bool ShowClients { get; set; } = true;

        private string _phone = "+375";
        public string Phone
        {
            get => _phone;
            set
            {
                if (Regex.IsMatch(value, @"^\+375\d{0,9}$"))
                {
                    SetProperty(ref _phone, value);
                }
                else
                {
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        private string _email = "";
        public string Email
        {
            get => _email;
            set
            {
                if (Regex.IsMatch(value, @"^[^@\s]*@?[^@\s]*$"))
                {
                    SetProperty(ref _email, value);
                }
                else
                {
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public bool IsEditing { get; set; } = false;

        private RelayCommand _search = null!;
        public RelayCommand Search
        {
            get => _search ??= new(
                () =>
                {
                    Users = [.. _userManager.GetUsersWithName(SearchName, SearchLogin, ShowEmployees, ShowClients)];
                    Users.CollectionChanged += DeleteEmployee;
                });
        }

        private RelayCommand _create = null!;
        public RelayCommand Create
        {
            get => _create ??= new(
                async () =>
                {
                    var employee = await _userManager.CreateUser(
                        Name,
                        Login,
                        Password,
                        Phone,
                        Email,
                        Avatar,
                        IsEmployee
                    );
                    Search.Execute(null);
                    Name = "";
                    Login = "";
                    Password = "";
                    Phone = "+375";
                    Email = "";
                    Avatar = null;
                    IsEmployee = false;
                }, () =>
                {
                    return Avatar != null;
                });
        }

        private RelayCommand _save = null!;
        public RelayCommand Save => _save ??= new(
            async () =>
            {
                await _userManager.SaveChangesAsync(Users);
                Users = [.. Users.Select(u => { u.NewPassword = ""; return u; })];
                Users.CollectionChanged += DeleteEmployee;
                Messenger.Default.Send(true);
                IsEditing = false;
            });

        private RelayCommand _choosePhoto = null!;
        public RelayCommand ChoosePhoto => _choosePhoto ??= new(() => { Avatar = PhotoBrowser.GetPhoto(); });

        public UserVM(UserManager userManager, Mediator mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
            Users = [.. _userManager.GetAllExceptAdmin()];
            Users.CollectionChanged += DeleteEmployee;
            _mediator.UpdateUserEvent += Update;
        }

        public void Update()
        {
            Users = [.. _userManager.GetAllExceptAdmin()];
        }

        private async void DeleteEmployee(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        await _userManager.DeleteUserAsync((User)item);
                    }
                    _mediator.UpdateRequests();
                }
            }
        }
    }
}
