using DYAT.Application.Interfaces.Repositories;
using DYAT.Domain.Entities;
using DYAT.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace DYAT.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
} 