using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EvtSource;
using Satellite.NET.Models;

namespace Satellite.NET
{
    public interface ISatelliteApi
    {
        /// <summary>
        /// The URL of the API.
        /// </summary>
        string ApiUrl { get; }

        /// <summary>
        /// Places an order for a message transmission.
        /// </summary>
        /// <param name="bid">The bid in millisatoshis.</param>
        /// <param name="filePath">The file to send.</param>
        /// <param name="message">The message to send.</param>
        /// <returns>A lightning payment invoice.</returns>
        Task<InvoiceModel> CreateOrderAsync(int bid, string filePath, string message);

        /// <summary>
        /// Increases the bid for an order sitting in the transmission queue.
        /// </summary>
        /// <param name="orderId">The order id.</param>
        /// <param name="authToken">The authentiation token that was provided when the order was created.</param>
        /// <param name="bidIncrease">The amount in millisatoshis by which to increase the bid.</param>
        /// <returns>A new in</returns>
        Task<InvoiceModel> BumpBidAsync(string orderId, string authToken, int bidIncrease);

        /// <summary>
        /// Retrieves an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="authToken">The authentiation token that was provided when the order was created.</param>
        /// <returns>The order.</returns>
        Task<OrderModel> GetOrderAsync(string orderId, string authToken);

        /// <summary>
        /// Cancels an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="authToken">The authentiation token that was provided when the order was created.</param>
        /// <returns>A value indicating whether the cancellation was successful or not.</returns>
        Task<bool> CancelOrderAsync(string orderId, string authToken);

        /// <summary>
        /// Retrieves a list of paid, but unsent orders in descending order of bid-per-byte.
        /// </summary>
        /// <param name="limit">Optionally parameter. Specifies the limit of queued orders to return.</param>
        /// <returns>A collection of queued orders sorted by bid-per-byte descending.</returns>
        Task<IEnumerable<OrderModel>> GetQueuedOrdersAsync(int? limit = null);

        /// <summary>
        /// Retrieves a list of 20 paid orders sorted in reverse chronological order.
        /// </summary>
        /// <param name="before">Optional parameter. The 20 orders immediately prior to the given date will be returned.</param>
        /// <returns>A collection of pending orders sorted in reverse chronological order.</returns>
        Task<IEnumerable<OrderModel>> GetSentOrdersAsync(DateTime? before = null);

        /// <summary>
        /// Retrieves a list of 20 orders awaiting payment sorted in reverse chronological order.
        /// </summary>
        /// <param name="before">Optional parameter. The 20 orders immediately prior to the given date will be returned.</param>
        /// <returns>A collection of pending orders sorted in reverse chronological order.</returns>
        Task<IEnumerable<OrderModel>> GetPendingOrdersAsync(DateTime? before = null);

        /// <summary>
        /// Returns information about the c-lightning node where satellite API payments are terminated.
        /// </summary>
        /// <returns>Information about the c-lightning node where satellite API payments are terminated.</returns>
        Task<InfoModel> GetInfoAsync();

        /// <summary>
        /// Retrieves the message sent in an order.
        /// </summary>
        /// <param name="messageNum">The message number.</param>
        /// <returns>A stream containing the message.</returns>
        Task<Stream> RetrieveMessageAsync(int messageNum);

        /// <summary>
        /// Subscribe to server-sent events for transmission messages.
        /// </summary>
        /// <param name="onReceive">Code to execute on receiving a message.</param>
        /// <param name="onDisconnection">Code to execute on disconnection. This is followed by automatic reconnection.</param>
        void ReceiveTransmissionsMessages(Action<EventSourceMessageEventArgs> onReceive, Action<DisconnectEventArgs> onDisconnection);
    }
}