using GalaSoft.MvvmLight.Messaging;
using PhoneService.BLL.Services;
using PhoneService.Commands;
using PhoneService.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace PhoneService.ViewModels
{
    public class AuthVM : ViewModelBase
    {
        private readonly AuthService _authService;

        public string Login { get; set; } = "";

        public string Password { get; set; } = "";

        public string Name { get; set; } = "";

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

        private FileInfo? _avatar = null;
        public FileInfo? Avatar
        {
            get => _avatar;
            set
            {
                SetProperty(ref _avatar, value);
                OnPropertyChanged(nameof(AvatarBitmap));
            }
        }

        public BitmapImage? AvatarBitmap => Avatar != null ? new(new Uri(Avatar.FullName)) : null;

        private RelayCommand _loginCommand = null!;
        public RelayCommand LoginCommand
        {
            get => _loginCommand ??= new(
                async () =>
                {
                    var user = await _authService.LoginAsync(Login, Password);
                    Messenger.Default.Send(user);
                    Messenger.Default.Send(View.Requests);
                }
            );
        }

        private RelayCommand _registerCommand = null!;
        public RelayCommand RegisterCommand
        {
            get => _registerCommand ??= new(
                async () =>
                {
                    await _authService.RegisterAsync(
                        Login,
                        Password,
                        Name,
                        Phone,
                        Email,
                        Avatar
                    );
                    var user = await _authService.LoginAsync(Login, Password);
                    Messenger.Default.Send(user);
                    Messenger.Default.Send(View.Requests);
                }
            );
        }

        private RelayCommand _choosePhoto = null!;
        public RelayCommand ChoosePhoto => _choosePhoto ??= new(() => { Avatar = PhotoBrowser.GetPhoto(); });

        private RelayCommand _toLogin = null!;
        public RelayCommand ToLogin => _toLogin ??= new(() => Messenger.Default.Send(View.Authorization));

        private RelayCommand _toRegister = null!;
        public RelayCommand ToRegister => _toRegister ??= new(() => Messenger.Default.Send(View.Registration));

        public AuthVM(AuthService authService)
        {
            _authService = authService;
        }
    }
}
