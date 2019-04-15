namespace Satellite.NET
{
    public class Constants
    {
        /// <summary>
        /// GET /info
        /// https://github.com/Blockstream/satellite/tree/master/api#get-info
        /// </summary>
        public const string InfoEndpoint = "/info";

        /// <summary>
        /// POST /order
        /// https://github.com/Blockstream/satellite/tree/master/api#post-order
        /// </summary>
        public const string CreateOrderEndpoint = "/order";

        /// <summary>
        /// POST /order/:uuid/bump
        /// https://github.com/Blockstream/satellite/tree/master/api#post-orderuuidbump
        /// </summary>
        public const string BumpOrderEndpoint = "/order/{0}/bump";

        /// <summary>
        /// GET /order/:uuid
        /// https://github.com/Blockstream/satellite/tree/master/api#get-orderuuid
        /// </summary>
        public const string GetOrderEndpoint = "/order/{0}";

        /// <summary>
        /// DELETE /order/:uuid
        /// https://github.com/Blockstream/satellite/tree/master/api#delete-orderuuid
        /// </summary>
        public const string CancelOrderEndpoint = "/order/{0}";

        /// <summary>
        /// GET /orders/queued
        /// https://github.com/Blockstream/satellite/tree/master/api#get-ordersqueued
        /// </summary>
        public const string GetQueuedOrdersEndpoint = "/orders/queued";

        /// <summary>
        /// GET /orders/pending
        /// </summary>
        public const string GetPendingOrdersEndpoint = "/orders/pending";

        /// <summary>
        /// GET /orders/sent
        /// https://github.com/Blockstream/satellite/tree/master/api#get-orderssent
        /// </summary>
        public const string GetSentOrdersEndpoint = "/orders/sent";

        /// <summary>
        /// GET /message/:tx_seq_num
        /// </summary>
        public const string RetrieveMessageEndpoint = "/message/{0}";

        /// <summary>
        /// The URL of the API on Mainnet.
        /// </summary>
        public const string ApiUrlMain = "https://api.blockstream.space";

        /// <summary>
        /// /// The URL of the API on Testnet.
        /// </summary>
        public const string ApiUrlTest = "https://api.blockstream.space/testnet";
    }
}
