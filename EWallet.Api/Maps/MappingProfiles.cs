using AutoMapper;
using EWallet.Api.DTOs;
using EWallet.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.WebAPI.Maps
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserToRegisterDTO, User>();
            CreateMap<WalletToAdd, Wallet>();
            CreateMap<User, UserToReturn>();
        }
    }
}
