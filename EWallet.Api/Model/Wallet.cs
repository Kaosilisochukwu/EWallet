using System;

namespace EWallet.Api.Model
{
    public class Wallet
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public bool IsMain { get; set; }
        public DateTime DateCreated { get; set; }
    }
}