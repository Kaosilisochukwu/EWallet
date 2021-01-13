using AutoMapper;
using EWallet.Api.Data;
using EWallet.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Services.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
         public async Task<IEnumerable<UserToReturn>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            var usersToReturn = _mapper.Map<IEnumerable<UserToReturn>>(users);
            return usersToReturn;
        }

        public async Task<UserToReturn> GetUserById(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);
            var userToReturn = _mapper.Map<UserToReturn>(user);
            return userToReturn;
        }
    }
}
