using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EvtSource;
using Newtonsoft.Json;
using Satellite.NET.Models;

namespace Satellite.NET.Cli
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var command = new RootCommand("satellite", treatUnmatchedTokensAsErrors: true, isHidden: true);
                
                List<Option> commonOptions = new List<Option>();
                commonOptions.Add(new Option("--test", "Call the test API", argument: new Argument<bool> { Arity = ArgumentArity.ZeroOrOne }));
                commonOptions.Add(new Option("--url", "Call an API at this URL", argument: new Argument<string> { Arity = ArgumentArity.ExactlyOne }));
                commonOptions.Add(new Option("--pretty", "Prettify the JSON result", argument: new Argument<bool> { Arity = ArgumentArity.ZeroOrOne }));

                #region Create order

                var createOrderArgument = new Argument();
                createOrderArgument.AddValidator(symbol =>
                {
                    if (symbol.Children.Contains("filepath") && symbol.Children.Contains("message"))
                    {
                        return "Options '--filepath' and '--message' cannot be used together.";
                    }

                    if (!symbol.Children.Contains("filepath") && !symbol.Children.Contains("message"))
                    {
                        return "Options '--filepath' or '--message' must be set.";
                    }

                    if (!symbol.Children.Contains("bid"))
                    {
                        return "Option '--bid' must be set.";
                    }

                    return null;
                });

                var createOrderCommand = new Command("create-order", description: "Place an order for a message transmission", argument: createOrderArgument, symbols: commonOptions)
                {
                    new Option("--bid", argument: new Argument<int>()),
                    new Option("--filepath", argument: new Argument<string>()),
                    new Option("--message", argument: new Argument<string>())
                };

                createOrderCommand.Handler = CommandHandler.Create((int bid, string filePath, string message, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        InvoiceModel result = api.CreateOrderAsync(bid, filePath, message).GetAwaiter().GetResult();
                        ShowApiCallResult(result, pretty);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(createOrderCommand);

                #endregion

                #region Cancel order

                var cancelOrderArgument = new Argument();
                cancelOrderArgument.AddValidator(symbol =>
                {
                    if (!symbol.Children.Contains("orderid") || !symbol.Children.Contains("authtoken"))
                    {
                        return "Options '--orderid' and '--authtoken' must be set.";
                    }

                    return null;
                });

                var cancelOrderCommand = new Command("cancel-order", description: "Cancel an order", argument: cancelOrderArgument, symbols: commonOptions)
                {
                    new Option("--orderid", argument: new Argument<string>()),
                    new Option("--authtoken", argument: new Argument<string>())
                };

                cancelOrderCommand.Handler = CommandHandler.Create((string orderId, string authToken, bool test, string url) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        bool result = api.CancelOrderAsync(orderId, authToken).GetAwaiter().GetResult();
                        string message = result ? "Order cancelled" : "Order has not been cancelled";
                        Console.WriteLine(message);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(cancelOrderCommand);

                #endregion

                #region Get order

                var getOrderArgument = new Argument();
                getOrderArgument.AddValidator(symbol =>
                {
                    if (!symbol.Children.Contains("orderid") || !symbol.Children.Contains("authtoken"))
                    {
                        return "Options '--orderid' and '--authtoken' must be set.";
                    }

                    return null;
                });

                var getOrderCommand = new Command("get-order", description: "Retrieve an order", argument: getOrderArgument, symbols: commonOptions)
                {
                    new Option("--orderid", argument: new Argument<string>()),
                    new Option("--authtoken", argument: new Argument<string>())
                };

                getOrderCommand.Handler = CommandHandler.Create((string orderId, string authToken, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        OrderModel result = api.GetOrderAsync(orderId, authToken).GetAwaiter().GetResult();
                        ShowApiCallResult(result, pretty);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(getOrderCommand);

                #endregion

                #region Bump order

                var bumpOrderArgument = new Argument();
                bumpOrderArgument.AddValidator(symbol =>
                {
                    if (!symbol.Children.Contains("orderid") || !symbol.Children.Contains("authtoken") || !symbol.Children.Contains("amount"))
                    {
                        return "Options '--orderid', '--authtoken' and '--amount' must be set.";
                    }

                    return null;
                });

                var bumpOrderCommand = new Command("bump-order", description: "Increase the bid for an order sitting in the transmission queue", argument: bumpOrderArgument, symbols: commonOptions)
                {
                    new Option("--orderid", argument: new Argument<string>()),
                    new Option("--authtoken", argument: new Argument<string>()),
                    new Option("--amount", argument: new Argument<int>())
                };

                bumpOrderCommand.Handler = CommandHandler.Create((string orderId, string authToken, int amount, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        InvoiceModel result = api.BumpBidAsync(orderId, authToken, amount).GetAwaiter().GetResult();
                        ShowApiCallResult(result, pretty);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(bumpOrderCommand);

                #endregion

                #region Get queued orders.

                var queuedCommand = new Command("queued-orders", description: "Retrieve a list of paid, but unsent orders in descending order of bid-per-byte", symbols: commonOptions);
                queuedCommand.Add(new Option("--limit", "(optional) Specifies the limit of queued orders to return", new Argument<int>()));
                queuedCommand.Handler = CommandHandler.Create((int? limit, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        IEnumerable<OrderModel> result = api.GetQueuedOrdersAsync(limit).GetAwaiter().GetResult();
                        ShowApiCallResult(result, pretty);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(queuedCommand);

                #endregion

                #region Get pending orders

                var pendingCommand = new Command("pending-orders", description: "Retrieve a list of 20 orders awaiting payment sorted in reverse chronological order", symbols: commonOptions);
                pendingCommand.Add(new Option("--before", "(optional) The 20 orders immediately prior to the given date will be returned", new Argument<DateTime>()));
                pendingCommand.Handler = CommandHandler.Create((DateTime? before, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        IEnumerable<OrderModel> result = api.GetPendingOrdersAsync(before).GetAwaiter().GetResult();
                        ShowApiCallResult(result, pretty);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(pendingCommand);

                #endregion

                #region Get sent orders.

                var sentCommand = new Command("sent-orders", description: "Retrieve a list of 20 paid orders sorted in reverse chronological order", symbols: commonOptions);
                sentCommand.Add(new Option("--before", "(optional) The 20 orders immediately prior to the given date will be returned", new Argument<DateTime>()));
                sentCommand.Handler = CommandHandler.Create((DateTime? before, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        IEnumerable<OrderModel> result = api.GetSentOrdersAsync(before).GetAwaiter().GetResult();
                        ShowApiCallResult(result, pretty);
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(sentCommand);

                #endregion

                #region Get message

                var getMessageArgument = new Argument();
                getMessageArgument.AddValidator(symbol =>
                {
                    if (!symbol.Children.Contains("num"))
                    {
                        return "Options '--num' must be set.";
                    }

                    return null;
                });

                var msgCommand = new Command("msg", description: "Retrieve the message sent in an order", argument: getMessageArgument, symbols: commonOptions);
                msgCommand.Add(new Option("--num", description: "The message number", argument: new Argument<int>()));
                msgCommand.Handler = CommandHandler.Create((int num, bool test, string url, bool pretty) =>
                {
                    try
                    {
                        ISatelliteApi api = InitializeApi(test, url);
                        Stream result = api.RetrieveMessageAsync(num).GetAwaiter().GetResult();
                        StreamReader reader = new StreamReader(result);
                        Console.WriteLine(reader.ReadToEnd());
                    }
                    catch (ApiException ex)
                    {
                        ShowException(ex);
                    }
                });

                command.Add(msgCommand);

                #endregion

                #region Get Info

                var infoCommand = new Command("info", description: "Return information about the c-lightning node where satellite API payments are terminated", symbols: commonOptions)
                {
                    Handler = CommandHandler.Create((bool test, string url, bool pretty) =>
                    {
                        try
                        {
                            ISatelliteApi api = InitializeApi(test, url);
                            InfoModel result = api.GetInfoAsync().GetAwaiter().GetResult();
                            ShowApiCallResult(result, pretty);
                        }
                        catch (ApiException ex)
                        {
                            ShowException(ex);
                        }
                    })
                };

                command.Add(infoCommand);

                #endregion

                #region Stream transmissions

                var transmissionsCommand = new Command("stream-transmissions", description: "Subscribe to server-sent events for transmission messages.", symbols: commonOptions)
                {
                    Handler = CommandHandler.Create((bool test, string url, bool pretty) =>
                    {
                        try
                        {
                            Console.WriteLine("Listening to new messages. Press Enter to stop.");

                            ISatelliteApi api = InitializeApi(test, url);
                            api.ReceiveTransmissionsMessages(
                            (EventSourceMessageEventArgs e) =>
                            {
                                Console.WriteLine($"Event: {e.Event}. Message: {e.Message}. Id: {e.Id}.");
                            }, 
                            (DisconnectEventArgs e) =>
                            {
                                Console.WriteLine($"Retry in: {e.ReconnectDelay}. Error: {e.Exception.Message}");
                            });

                            Console.Read();
                        }
                        catch (ApiException ex)
                        {
                            ShowException(ex);
                        }
                    })
                };

                command.Add(transmissionsCommand);

                #endregion

                await command.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
        }

        private static SatelliteApi InitializeApi(bool test, string url)
        {
            return !string.IsNullOrEmpty(url) ? new SatelliteApi(url) : new SatelliteApi(test);
        }

        private static void ShowException(ApiException exception)
        {
            Console.WriteLine($"The API threw an exception:");
            Console.WriteLine($"Request URL: {exception.RequestUrl}.");
            Console.WriteLine($"HTTP status code: {exception.StatusCode}.");

            if (exception.Errors != null && exception.Errors.Any())
            {
                Console.WriteLine($"Error messages: {string.Join(Environment.NewLine, exception.Errors)}");
            }
        }

        private static void ShowApiCallResult(object result, bool pretty)
        {
            Console.WriteLine(JsonConvert.SerializeObject(result, pretty ? Formatting.Indented : Formatting.None));
        }
    }
}
