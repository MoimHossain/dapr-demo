using System;
using System.Collections.Generic;
using System.Text;

namespace Dapr.Common
{
    public class DaprConstants
    {
        public const string StoreName = "statestore";
        public const string PubsubName = "pubsub";

        public const string BackendAppID = "backend";

        public class MethodNames
        {
            public const string Deposit = "deposit";
            public const string Withdraw = "withdraw";
            public const string Balance = "balance";
        }
    }
}
