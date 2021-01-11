    using EWallet.Api.Data;
    using EWallet.Api.DTOs;
using EWallet.Api.Model;
using EWallet.Api.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

namespace EWallet.Api.Services.Repositories
{
    public class FundRequestRepository : IFundRequestRepository
    {
        private readonly AppDbContext _context;
        private readonly WalletRepository _walletRepository;

        public FundRequestRepository(AppDbContext context, WalletRepository walletRepository)
        {
            _context = context;
            _walletRepository = walletRepository;
        }
        public async Task<int> ApproveFundRequest(int requestId)
        {
            var request = await _context.FundRequests.FirstOrDefaultAsync(fundRequest => fundRequest.Id == requestId);
            if(request != null)
            {
                var fundResult = await _walletRepository.FundWallet(request.WalletId, request.Amount);
                if(fundResult > 0)
                {
                    request.Status = RequestStatus.Approved;
                    _context.FundRequests.Update(request);
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeclineFundRequest(int requestId)
        {
            var request = await _context.FundRequests.FirstOrDefaultAsync(fundRequest => fundRequest.Id == requestId);
            if(request != null)
            {
                request.Status = RequestStatus.Disapproved;
                _context.FundRequests.Update(request);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RequestFunding(Fund model)
        {
            await _context.FundRequests.AddAsync(model);
            return await _context.SaveChangesAsync();
        }
    }
}
