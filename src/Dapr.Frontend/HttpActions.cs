

using Dapr.Client;
using Dapr.Client.Http;
using Dapr.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dapr.Frontend
{
    public class HttpActions
    {
        private static async Task BalanceCore(HttpContext context, string id)
        {
            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();
            Console.WriteLine("id is {0}", id);
            var account = await client.InvokeMethodAsync<Account>(
                DaprConstants.BackendAppID,
                DaprConstants.MethodNames.Balance, new HTTPExtension
                {
                    Verb = HTTPVerb.Get,
                    QueryString = new Dictionary<string, string>
                    {
                        ["id"] = id
                    }
                });

            if (account == null)
            {
                Console.WriteLine("Account not found");
                context.Response.StatusCode = 404;
                return;
            }
            Console.WriteLine("Account balance is {0}", account.Balance);
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, account, serializerOptions);
        }

        public static async Task Balance(HttpContext context)
        {
            Console.WriteLine("Get Balance");
            var id = context.Request.Query["id"];


            Console.WriteLine($"Get Balance ID = {id}");
            await BalanceCore(context, id);
        }

        public static async Task Deposit(HttpContext context)
        {
            Console.WriteLine("Enter Deposit");
            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();

            var transaction = await JsonSerializer.DeserializeAsync<Transaction>(context.Request.Body, serializerOptions);
            Console.WriteLine("Id is {0}, Amount is {1}", transaction.Id, transaction.Amount);
            
            await client.InvokeMethodAsync(DaprConstants.BackendAppID, DaprConstants.MethodNames.Deposit, transaction);
            await BalanceCore(context, transaction.Id);
        }

        public static async Task Withdraw(HttpContext context)
        {
            Console.WriteLine("Enter Deposit");
            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();

            var transaction = await JsonSerializer.DeserializeAsync<Transaction>(context.Request.Body, serializerOptions);
            Console.WriteLine("Id is {0}, Amount is {1}", transaction.Id, transaction.Amount);

            await client.InvokeMethodAsync(DaprConstants.BackendAppID, DaprConstants.MethodNames.Withdraw, transaction);
            await BalanceCore(context, transaction.Id);
        }
    }
}
