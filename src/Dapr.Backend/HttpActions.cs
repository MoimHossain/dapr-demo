

using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dapr.Backend
{
    public class HttpActions
    {
        public static async Task Balance(HttpContext context)
        {
            Console.WriteLine("Enter Balance");
            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();

            var id = (string)context.Request.RouteValues["id"];
            Console.WriteLine("id is {0}", id);
            var account = await client.GetStateAsync<Account>(DaprConstants.StoreName, id);
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

        public static async Task Deposit(HttpContext context)
        {
            Console.WriteLine("Enter Deposit");

            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();

            var transaction = await JsonSerializer.DeserializeAsync<Transaction>(context.Request.Body, serializerOptions);
            Console.WriteLine("Id is {0}, Amount is {1}", transaction.Id, transaction.Amount);
            var account = await client.GetStateAsync<Account>(DaprConstants.StoreName, transaction.Id);
            if (account == null)
            {
                account = new Account() { Id = transaction.Id, };
            }

            if (transaction.Amount < 0m)
            {
                Console.WriteLine("Invalid amount");
                context.Response.StatusCode = 400;
                return;
            }

            account.Balance += transaction.Amount;
            await client.SaveStateAsync(DaprConstants.StoreName, transaction.Id, account);
            Console.WriteLine("Balance is {0}", account.Balance);

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, account, serializerOptions);
        }

        public static async Task Withdraw(HttpContext context)
        {
            Console.WriteLine("Enter Withdraw");
            var client = context.RequestServices.GetRequiredService<DaprClient>();
            var serializerOptions = context.RequestServices.GetRequiredService<JsonSerializerOptions>();
            var transaction = await JsonSerializer.DeserializeAsync<Transaction>(context.Request.Body, serializerOptions);
            Console.WriteLine("Id is {0}", transaction.Id);
            var account = await client.GetStateAsync<Account>(DaprConstants.StoreName, transaction.Id);
            if (account == null)
            {
                Console.WriteLine("Account not found");
                context.Response.StatusCode = 404;
                return;
            }

            if (transaction.Amount < 0m)
            {
                Console.WriteLine("Invalid amount");
                context.Response.StatusCode = 400;
                return;
            }

            account.Balance -= transaction.Amount;
            await client.SaveStateAsync(DaprConstants.StoreName, transaction.Id, account);
            Console.WriteLine("Balance is {0}", account.Balance);

            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, account, serializerOptions);
        }
    }
}
