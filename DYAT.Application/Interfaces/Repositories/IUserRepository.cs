using DYAT.Domain.Entities;

namespace DYAT.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByEmail(string email);
} 