using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Satellite.NET.Models;

namespace Satellite.NET
{
    public class SatelliteApi
    {
        /// <summary>
        /// The URL of the API.
        /// </summary>
        public string ApiUrl { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="SatelliteApi"/>.
        /// </summary>
        /// <param name="testApi">Whether the API used shoud be Mainnet (default) or Testnet.</param>
        public SatelliteApi(bool testApi = false)
        {
            this.ApiUrl = testApi ? Constants.ApiUrlTest : Constants.ApiUrlMain;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SatelliteApi"/>.
        /// </summary>
        /// <param name="customHost">A custom API URL.</param>
        public SatelliteApi(string customHost)
        {
            // Check the URL is in a valid format.
            if (!(Uri.TryCreate(customHost, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)))
            {
                throw new FormatException($"'{customHost}' is not a valid URL.");
            }
            
            this.ApiUrl = customHost;
        }

        /// <summary>
        /// Places an order for a message transmission.
        /// </summary>
        /// <param name="bid">The bid in millisatoshis.</param>
        /// <param name="filePath">The file to send.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A lightning payment invoice.</returns>
        public async Task<InvoiceModel> CreateOrderAsync(int bid, string filePath, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) && string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException("A message or a file must be set.");
                }

                if (!string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(message))
                {
                    throw new ArgumentException("You can only send a message or a file but not both.");
                }

                InvoiceModel invoice = null;

                if (!string.IsNullOrEmpty(filePath))
                {
                    if (Uri.IsWellFormedUriString(filePath, UriKind.Absolute))
                    {
                        throw new FormatException($"The path '{filePath}' is not a valid path.");
                    }

                    FileStream fs = File.OpenRead(filePath);

                    invoice = await this.ApiUrl
                    .AppendPathSegment(Constants.CreateOrderEndpoint)
                    .PostMultipartAsync(mp => mp
                    .AddString("bid", bid.ToString())
                    .AddFile("file", fs, "helo.txt"))
                    .ReceiveJson<InvoiceModel>();
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    invoice = await this.ApiUrl
                    .AppendPathSegment(Constants.CreateOrderEndpoint)
                    .PostMultipartAsync(mp => mp
                    .AddString("bid", bid.ToString())
                    .AddString("message", message))
                    .ReceiveJson<InvoiceModel>();
                }

                return invoice;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Increases the bid for an order sitting in the transmission queue.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="authToken">The authentiation token that was provided when the order was created.</param>
        /// <param name="bidIncrease">The amount in millisatoshis by which to increase the bid.</param>
        /// <returns>A new in</returns>
        public async Task<InvoiceModel> BumpBidAsync(string orderId, string authToken, int bidIncrease)
        {
            try
            {
                InvoiceModel invoice = await this.ApiUrl
                .AppendPathSegment(string.Format(Constants.BumpOrderEndpoint, orderId))
                .WithHeader("X-Auth-Token", authToken)
                .PostMultipartAsync(mp => mp
                .AddString("bid_increase", bidIncrease.ToString()))
                .ReceiveJson<InvoiceModel>();

                return invoice;
            }
            catch (FlurlHttpException ex) // TODO fix issue where this throws a 500.
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Retrieves an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="authToken">The authentiation token that was provided when the order was created.</param>
        /// <returns>The order.</returns>
        public async Task<OrderModel> GetOrderAsync(string orderId, string authToken)
        {
            try
            {
                OrderModel order = await this.ApiUrl
                    .AppendPathSegment(string.Format(Constants.GetOrderEndpoint, orderId))
                    .WithHeader("X-Auth-Token", authToken)
                    .GetJsonAsync<OrderModel>();

                return order;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="authToken">The authentiation token that was provided when the order was created.</param>
        /// <returns>A value indicating whether the cancellation was successful or not.</returns>
        public async Task<bool> CancelOrderAsync(string orderId, string authToken)
        {
            try
            {
                MessageModel result = await this.ApiUrl
                    .AppendPathSegment(string.Format(Constants.CancelOrderEndpoint, orderId))
                    .WithHeader("X-Auth-Token", authToken)
                    .DeleteAsync().ReceiveJson<MessageModel>();

                return result.Message == "order cancelled";
            }
            catch (FlurlHttpException ex) // TODO fix issue where this throws a 500.
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Retrieves a list of paid, but unsent orders in descending order of bid-per-byte.
        /// </summary>
        /// <param name="limit">Optionally parameter. Specifies the limit of queued orders to return.</param>
        /// <returns>A collection of queued orders sorted by bid-per-byte descending.</returns>
        public async Task<IEnumerable<OrderModel>> GetQueuedOrdersAsync(int? limit = null)
        {
            try
            {
                IEnumerable<OrderModel> orders = null;

                if (limit != null)
                {
                    orders = await this.ApiUrl
                    .AppendPathSegment(Constants.GetQueuedOrdersEndpoint)
                    .SetQueryParams(new { limit = limit.Value })
                    .GetJsonAsync<IEnumerable<OrderModel>>();
                }
                else
                {
                    orders = await this.ApiUrl
                    .AppendPathSegment(Constants.GetQueuedOrdersEndpoint)
                    .GetJsonAsync<IEnumerable<OrderModel>>();
                }

                return orders;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Retrieves a list of 20 paid orders sorted in reverse chronological order.
        /// </summary>
        /// <param name="before">Optional parameter. The 20 orders immediately prior to the given date will be returned.</param>
        /// <returns>A collection of pending orders sorted in reverse chronological order.</returns>
        public async Task<IEnumerable<OrderModel>> GetSentOrdersAsync(DateTime? before = null)
        {
            try
            {
                IEnumerable<OrderModel> orders = null;

                if (before != null)
                {
                    orders = await this.ApiUrl
                    .AppendPathSegment(Constants.GetSentOrdersEndpoint)
                    .SetQueryParams(new { before = before.Value.ToString("o") })
                    .GetJsonAsync<IEnumerable<OrderModel>>();
                }
                else
                {
                    orders = await this.ApiUrl
                    .AppendPathSegment(Constants.GetSentOrdersEndpoint)
                    .GetJsonAsync<IEnumerable<OrderModel>>();
                }

                return orders;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Retrieves a list of 20 orders awaiting payment sorted in reverse chronological order.
        /// </summary>
        /// <param name="before">Optional parameter. The 20 orders immediately prior to the given date will be returned.</param>
        /// <returns>A collection of pending orders sorted in reverse chronological order.</returns>
        public async Task<IEnumerable<OrderModel>> GetPendingOrdersAsync(DateTime? before = null)
        {
            try
            {
                IEnumerable<OrderModel> orders = null;

                if (before != null)
                {
                    orders = await this.ApiUrl
                    .AppendPathSegment(Constants.GetPendingOrdersEndpoint)
                    .SetQueryParams(new { before = before.Value.ToString("o") })
                    .GetJsonAsync<IEnumerable<OrderModel>>();
                }
                else
                {
                    orders = await this.ApiUrl
                    .AppendPathSegment(Constants.GetPendingOrdersEndpoint)
                    .GetJsonAsync<IEnumerable<OrderModel>>();
                }

                return orders;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Returns information about the c-lightning node where satellite API payments are terminated.
        /// </summary>
        /// <returns>Information about the c-lightning node where satellite API payments are terminated.</returns>
        public async Task<InfoModel> GetInfoAsync()
        {
            try
            {
                InfoModel info = await this.ApiUrl
                    .AppendPathSegment(Constants.InfoEndpoint)
                    .GetJsonAsync<InfoModel>();

                return info;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Retrieves the message sent in an order.
        /// </summary>
        /// <param name="messageNum">The message number.</param>
        /// <returns>A stream containing the message.</returns>
        public async Task<Stream> RetrieveMessageAsync(int messageNum)
        {
            try
            {
                Stream result = await this.ApiUrl
                    .AppendPathSegment(string.Format(Constants.RetrieveMessageEndpoint, messageNum))
                    .GetStreamAsync();

                return result;
            }
            catch (FlurlHttpException ex)
            {
                throw await this.CreateApiExceptionAsync(ex);
            }
        }

        /// <summary>
        /// Creates an exception based off the error response data returned from the API.
        /// </summary>
        /// <param name="ex">The error response data.</param>
        /// <returns>The created exception.</returns>
        private async Task<ApiException> CreateApiExceptionAsync(FlurlHttpException ex)
        {
            ErrorResponseModel errorResponse = await ex.GetResponseJsonAsync<ErrorResponseModel>();

            return new ApiException(errorResponse != null ? errorResponse.Message : "An error occurred.", ex)
            {
                RequestUrl = ex.Call.Request.RequestUri,
                StatusCode = ex.Call.HttpStatus,
                Errors = errorResponse?.Errors?.Select(err => new Error { Title = err.Title, Detail = err.Detail })
            };
        }
    }
}
