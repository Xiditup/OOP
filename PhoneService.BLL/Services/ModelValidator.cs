using PhoneService.DAL.Entities;
using System.Text.RegularExpressions;

namespace PhoneService.BLL.Services
{
    public static class ModelValidator
    {
        public static bool IsRu { get; set; } = true;

        private static string Translate(string ruText, string enText) => IsRu ? ruText : enText;

        public static void ValidateUser(User user)
        {
            List<string> errors = [];

            if (string.IsNullOrWhiteSpace(user.Login) || user.Login.Length < 5)
            {
                errors.Add(Translate("Логин должен содержать не менее 5 символов и не быть пустым.",
                                     "Login must be at least 5 characters long and not be empty."));
            }

            if (user.Login.Length > 30)
            {
                errors.Add(Translate("Максимальная длина логина - 30 символов",
                                     "Maximum login length is 30 characters."));
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash.Length < 8 || !user.PasswordHash.Any(c => char.IsDigit(c)))
            {
                errors.Add(Translate("Пароль должен содержать не менее 8 символов, содержать как минимум 1 цифру и не быть пустым.",
                                     "Password must be at least 8 characters long, contain at least one digit, and not be empty."));
            }

            if (!string.IsNullOrWhiteSpace(user.NewPassword))
            {
                if (user.NewPassword.Length < 8 || !user.NewPassword.Any(c => char.IsDigit(c)))
                {
                    errors.Add(Translate("Пароль должен содержать не менее 8 символов, содержать как минимум 1 цифру и не быть пустым.",
                                         "Password must be at least 8 characters long, contain at least one digit, and not be empty."));
                }

                if (user.NewPassword.Length > 30)
                {
                    errors.Add(Translate("Максимальная длина пароля - 30 символов",
                                         "Maximum password length is 30 characters."));
                }
            }

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                errors.Add(Translate("Имя не должно быть пустым.", "Name must not be empty."));
            }

            if (user.Name.Length > 50)
            {
                errors.Add(Translate("Максимальная длина имени - 50 символов", "Maximum name length is 50 characters."));
            }

            if (!Regex.IsMatch(user.Phone, @"^\+?\d{12}$"))
            {
                errors.Add(Translate("Неверный формат телефона", "Invalid phone format."));
            }

            if (!Regex.IsMatch(user.Email, @"^\S+@\S+\.\S+$"))
            {
                errors.Add(Translate("Неверный формат email", "Invalid email format."));
            }

            if (user.Email.Length > 100)
            {
                errors.Add(Translate("Максимальная длина почты - 100 символов", "Maximum email length is 100 characters."));
            }

            if (errors.Count != 0)
            {
                if (user.Id != 0)
                {
                    errors.Add(Translate($"Id пользователя = {user.Id}", $"User Id = {user.Id}"));
                }
                throw new ArgumentException(string.Join("\n", errors));
            }
        }

        public static void ValidateService(Service service)
        {
            List<string> errors = [];

            if (string.IsNullOrWhiteSpace(service.Name))
            {
                errors.Add(Translate("Имя не может быть пустым", "Name can not be empty"));
            }

            if (service.Name.Length > 100)
            {
                errors.Add(Translate("Максимальная длина имени услуги - 100 символов", "Maximal service's name length is 100 characters"));
            }

            if (service.Price < 0)
            {
                errors.Add(Translate("Цена услуги не может быть меньше 0", "Service's price can not be lower than 0"));
            }

            if (errors.Count != 0)
            {
                if (service.Id != 0)
                {
                    errors.Add(Translate($"Id услуги = {service.Id}", $"Service Id = {service.Id}"));
                }
                throw new ArgumentException(string.Join("\n", errors.ToArray()));
            }
        }

        public static void ValidateDetail(Detail detail)
        {
            List<string> errors = [];

            if (string.IsNullOrWhiteSpace(detail.Name))
            {
                errors.Add(Translate("Имя не может быть пустым", "Name can not be empty"));
            }

            if (detail.Name.Length > 100)
            {
                errors.Add(Translate("Максимальная длина имени детали - 100 символов", "Maximal detail's name length is 100 characters."));
            }

            if (detail.Quantity < 0)
            {
                errors.Add(Translate("Количество детали не может быть меньше 0", "Detail quantity can not be lower than 0"));
            }

            if (errors.Count != 0)
            {
                if (detail.Id != 0)
                {
                    errors.Add(Translate($"Id детали = {detail.Id}", $"Detail Id = {detail.Id}"));
                }
                throw new ArgumentException(string.Join("\n", errors.ToArray()));
            }
        }

        public static void ValidateStock(Stock stock)
        {
            List<string> errors = [];


            if (string.IsNullOrWhiteSpace(stock.Name))
            {
                errors.Add(Translate("Имя акции не может быть пустым", "Stock name cannot be empty"));
            }

            if (string.IsNullOrWhiteSpace(stock.Description))
            {
                errors.Add(Translate("Описание акции не может быть пустым", "Stock description cannot be empty"));
            }

            if (stock.Type == DAL.Models.StockType.Discount)
            {
                if (stock.Discount <= 0)
                {
                    errors.Add(Translate("Размер скидки должен быть больше 0%", "Discount value must be greater than 0%"));
                }

                if (stock.Discount > 100)
                {
                    errors.Add(Translate("Размер скидки не может быть больше 100%", "Discount value cannot exceed 100%"));
                }
            }

            if (errors.Count != 0)
            {
                if (stock.Id != 0)
                {
                    errors.Add(Translate($"Id акции = {stock.Id}", $"Stock Id = {stock.Id}"));
                }
                throw new ArgumentException(string.Join("\n", errors.ToArray()));
            }
        }

        public static void ValidateRequest(Request request)
        {
            List<string> errors = [];

            if (string.IsNullOrEmpty(request.Name))
            {
                errors.Add(Translate("Дайте название заявке", "Give the request a name"));
            }

            if (string.IsNullOrEmpty(request.Description))
            {
                errors.Add(Translate("Опишите вашу проблему", "Describe your issue"));
            }

            if (string.IsNullOrEmpty(request.Device))
            {
                errors.Add(Translate("Укажите модель устройства", "Specify the device model"));
            }

            if (errors.Count != 0)
            {
                throw new ArgumentException(string.Join("\n", errors));
            }
        }
    }
}
