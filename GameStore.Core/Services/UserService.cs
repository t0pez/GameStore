using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using GameStore.Core.Exceptions;
using GameStore.Core.Interfaces;
using GameStore.Core.Models.Constants;
using GameStore.Core.Models.Server.Users;
using GameStore.Core.Models.Server.Users.Specifications;
using GameStore.Core.Models.ServiceModels.Users;
using GameStore.SharedKernel.Interfaces.DataAccess;

namespace GameStore.Core.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<User> UsersRepository => _unitOfWork.GetEfRepository<User>();

    public async Task<List<User>> GetAllAsync()
    {
        var result = await UsersRepository.GetBySpecAsync(new UsersSpec());

        return result;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        var spec = new UsersSpec().ById(id);

        var user = await UsersRepository.GetSingleOrDefaultBySpecAsync(spec)
                   ?? throw new ItemNotFoundException(typeof(User), id);

        return user;
    }

    public async Task<User> GetByLoginModelOrDefaultAsync(LoginModel loginModel)
    {
        var spec = new UsersSpec().ByEmail(loginModel.Email);
        var user = await UsersRepository.GetSingleOrDefaultBySpecAsync(spec);

        if (user is null)
        {
            return null;
        }

        if (VerifyHashedPassword(user.PasswordHash, loginModel.Password) == false)
        {
            return null;
        }

        return user;
    }

    public async Task<User> CreateAsync(UserCreateModel createModel)
    {
        var user = _mapper.Map<User>(createModel);
        user.Role = Roles.User;

        var passwordHash = HashPassword(createModel.Password);
        user.PasswordHash = passwordHash;

        await UsersRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task UpdateAsync(UserUpdateModel updateModel)
    {
        var user = await GetByIdAsync(updateModel.Id);

        UpdateValues(user, updateModel);

        await UsersRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);

        user.IsDeleted = true;

        await UsersRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Guid> GenerateUniqueUserIdAsync()
    {
        while (true)
        {
            var id = Guid.NewGuid();

            if (await IsUserIdExistsAsync(id) == false)
            {
                return id;
            }
        }
    }

    public Task<bool> IsUserIdExistsAsync(Guid id)
    {
        return UsersRepository.AnyAsync(new UsersSpec().ById(id));
    }

    public Task<bool> IsUserNameExistsAsync(string userName)
    {
        return UsersRepository.AnyAsync(new UsersSpec().ByUserName(userName));
    }

    public Task<bool> IsUserEmailExistsAsync(string email)
    {
        return UsersRepository.AnyAsync(new UsersSpec().ByEmail(email));
    }

    private void UpdateValues(User user, UserUpdateModel updateModel)
    {
        user.Email = updateModel.Email;
        user.UserName = updateModel.UserName;

        if (updateModel.Role is not null)
        {
            user.Role = updateModel.Role;
        }

        if (updateModel.PublisherName is not null)
        {
            user.PublisherName = updateModel.PublisherName;
        }
    }

    private string HashPassword(string password)
    {
        byte[] salt;
        byte[] buffer2;

        using (var bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
        {
            salt = bytes.Salt;
            buffer2 = bytes.GetBytes(0x20);
        }

        var dst = new byte[0x31];
        Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
        Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);

        return Convert.ToBase64String(dst);
    }

    private bool VerifyHashedPassword(string hashedPassword, string password)
    {
        byte[] buffer4;

        var src = Convert.FromBase64String(hashedPassword);

        if (src.Length != 0x31 || src[0] != 0)
        {
            return false;
        }

        var dst = new byte[0x10];
        Buffer.BlockCopy(src, 1, dst, 0, 0x10);
        var buffer3 = new byte[0x20];
        Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);

        using (var bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
        {
            buffer4 = bytes.GetBytes(0x20);
        }

        return ByteArraysEqual(buffer3, buffer4);
    }

    private bool ByteArraysEqual(byte[] first, byte[] second)
    {
        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i])
            {
                return false;
            }
        }

        return true;
    }
}