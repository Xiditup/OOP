using PhoneService.DAL.Entities;
using PhoneService.DAL.Repository;

namespace PhoneService.BLL.Services
{
    public class UserManager
    {
        private readonly UserRepository _userRepository;
        private readonly PasswordService _passwordService;
        private readonly ImageService _imageService;
        public UserManager(
            UserRepository userRepository,
            PasswordService passwordService,
            ImageService imageService
        )
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _imageService = imageService;

            if (_userRepository.GetAll().Count == 0)
            {
                var admin = new User
                {
                    Login = "admin",
                    PasswordHash = _passwordService.HashPassword("admin"),
                    Name = "Администратор",
                    Phone = "+375000000000",
                    Email = "admin@gmail.com",
                    Role = "Admin",
                    AvatarPath = "images/admin.jpg"
                };

                _ = _userRepository.AddAsync(admin);
            }
        }
        public async Task<User> CreateUser(
            string name,
            string login,
            string password,
            string phone,
            string email,
            FileInfo? avatar,
            bool isEmployee
        )
        {
            var user = new User
            {
                Login = login,
                PasswordHash = password,
                Name = name,
                Phone = phone,
                Email = email,
                AvatarPath = "",
                Role = isEmployee ? "Employee" : "Client"
            };

            ModelValidator.ValidateUser(user);

            user.PasswordHash = _passwordService.HashPassword(password);

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

            if (avatar != null)
            {
                user.AvatarPath = _imageService.Upload(avatar);
            }
            else
            {
                throw new ArgumentException("Выберите аватар");
            }

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task DeleteUserAsync(User user)
        {
            await _userRepository.RemoveAsync(user);
        }

        public IEnumerable<User> GetAllExceptAdmin()
        {
            return _userRepository.GetAllExceptAdmin();
        }

        public IEnumerable<User> GetUsersWithName(string name, string login, bool employees, bool clients)
        {
            return _userRepository.GetUsersFiltered(name, login, employees, clients);
        }

        public async Task SaveChangesAsync(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                ModelValidator.ValidateUser(user);
                if (user.NewPassword != "")
                {
                    user.PasswordHash = _passwordService.HashPassword(user.NewPassword);
                }
            }
            await _userRepository.UpdateRangeAsync(users);
        }
    }
}
