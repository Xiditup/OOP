﻿namespace PhoneService.BLL.Services
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public bool VerifyPassword(string password, string PasswordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }
    }
}
