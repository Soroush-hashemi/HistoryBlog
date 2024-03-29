﻿using TestBlog.DataLayer.Context;
using TestBlog.Services.Utilities;
using TestBlog.DataLayer.Entities;
using TestBlog.Services.DTOs.Users;

namespace TestBlog.Services.Services.Users
{
    public class UserService : IUserService
    {
        private readonly BlogContext _context;

        public UserService(BlogContext context)
        {
            _context = context;
        }

        public OperationResult RegisterUser(UserRegisterDto registerDto)
        {
            var isUserNameExist = _context.Users.Any(u => u.UserName == registerDto.UserName);
            var isUserEmailExist = _context.Users.Any(e => e.UserEmail == registerDto.UserEmail);
            var user = _context.Users;

            if (isUserNameExist)
                return OperationResult.Error("نام‌کاربری تکراری است");
            if (isUserEmailExist)
                return OperationResult.Error("ایمیل تکراری است");
            if (user == null)
                return null;

            var passwordHash = registerDto.Password.EncodeToMd5();
            _context.Users.Add(new User()
            {
                FullName = registerDto.Fullname,
                UserEmail = registerDto.UserEmail,
                IsDelete = false,
                UserName = registerDto.UserName,
                Role = UserRole.User,
                CreationDate = DateTime.Now,
                Password = passwordHash
            });
            _context.SaveChanges();
            return OperationResult.Success();
        }

        public UserDto LoginUser(LoginUserDto loginDto)
        {
            var passwordHashed = loginDto.Password.EncodeToMd5();
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == loginDto.UserName && u.Password == passwordHashed);

            if (user == null)
                return null;

            var userDto = new UserDto()
            {
                FullName = user.FullName,
                UserEmail= user.UserEmail,
                Password = user.Password,
                Role = user.Role,
                UserName = user.UserName,
                RegisterDate = user.CreationDate,
                UserId = user.Id
            };
            return userDto;
        }

        public UserDto UserPanel(string UserId)
        {
            int Id = Convert.ToInt32(UserId);
            var user = _context.Users.FirstOrDefault(u => u.Id == Id);

            var userDto = new UserDto()
            {
                FullName = user.FullName,
                Role = user.Role,
                UserEmail= user.UserEmail,
                UserName= user.UserName,
                RegisterDate = user.CreationDate,
            };

            return userDto;
        }
    }
}
