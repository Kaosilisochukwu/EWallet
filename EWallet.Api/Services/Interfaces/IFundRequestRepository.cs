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
        public Task<int> RequestFunding(Fund model);
        public Task<int> ApproveFundRequest(int requestId);
        public Task<int> DeclineFundRequest(int requestId);
    }
}
