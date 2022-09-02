using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStore.Core.Models.Server.Users;
using GameStore.Core.Models.ServiceModels.Users;

namespace GameStore.Core.Interfaces;

public interface IUserService
{
    public Task<List<User>> GetAllAsync();
    public Task<User> GetByIdAsync(Guid id);
    public Task<User> GetByLoginModelOrDefaultAsync(LoginModel loginModel);
    public Task<User> CreateAsync(UserCreateModel createModel);
    public Task UpdateAsync(UserUpdateModel updateModel);
    public Task DeleteAsync(Guid id);
    public Task<Guid> GenerateUniqueUserIdAsync();
    public Task<bool> IsUserIdExistsAsync(Guid id);
    public Task<bool> IsUserNameExistsAsync(string userName);
    public Task<bool> IsUserEmailExistsAsync(string email);
}