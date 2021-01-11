using EWallet.Api.DTOs;
using EWallet.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EWallet.Api.Services.Interfaces
{
    public interface IFundRequestRepository
    {
        Task<int> RequestFunding(Fund model);
        Task<int> ApproveFundRequest(int requestId);
        Task<int> DeclineFundRequest(int requestId);
    }
}
