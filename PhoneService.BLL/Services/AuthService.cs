using PhoneService.DAL.Entities;
using PhoneService.DAL.Repository;

namespace PhoneService.BLL.Services
{
    public class AuthService
    {
        private readonly PasswordService _passwordService;
        private readonly ImageService _imageService;
        private readonly UserRepository _userRepository;
        public AuthService(
            PasswordService passwordService,
            ImageService imageService,
            UserRepository userRepository
        )
        {
            _passwordService = passwordService;
            _imageService = imageService;
            _userRepository = userRepository;
        }

        public async Task RegisterAsync(
            string login,
            string password,
            string name,
            string phone,
            string email,
            FileInfo? avatar
        )
        {
            if ((await _userRepository.GetByLoginAsync(login)) != null)
            {
                throw new ArgumentException("Данный логин уже занят");
            }

            if ((await _userRepository.GetByPhoneAsync(phone)) != null)
            {
                throw new ArgumentException("Данный телефон уже занят");
            }

            if ((await _userRepository.GetByEmailAsync(email)) != null)
            {
                throw new ArgumentException("Данный email уже занят");
            }

            var user = new User()
            {
                Login = login,
                PasswordHash = password,
                Name = name,
                Phone = phone,
                Email = email,
                AvatarPath = "",
                Role = "Client"
            };

            ModelValidator.ValidateUser(user);

            user.PasswordHash = _passwordService.HashPassword(password);

            if (avatar != null)
            {
                user.AvatarPath = _imageService.Upload(avatar);
            }
            else
            {
                throw new ArgumentException("Выберите аватар");
            }

            await _userRepository.AddAsync(user);
        }

        public async Task<User> LoginAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login) ?? throw new NullReferenceException("Пользователь не найден");
            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
            {
                throw new Exception("Пароль неверный");
            }
            return user;
        }
    }
}
