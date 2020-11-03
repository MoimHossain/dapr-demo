using System;
using System.Collections.Generic;
using System.Text;

namespace Dapr.Common
{
    public class Transaction
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
    }
    public class Account
    {
        public string Id { get; set; }
        public decimal Balance { get; set; }
    }
}
