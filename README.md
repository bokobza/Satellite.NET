# Satellite.NET

This is a C# SDK for the Blockstream Satellite API found [here](https://github.com/Blockstream/satellite-api).
#### Using the SDK
##### Initializing the API object  
To use Mainnet:
`SatelliteApi api = new SatelliteApi();`  
To use Testnet:
`SatelliteApi api = new SatelliteApi(true);`  
To use your own custom API:
`SatelliteApi api = new SatelliteApi("http://localhost:....");`  

##### Calling the API
Call the desired method, for example  
`IEnumerable<OrderModel> orders = await api.GetSentOrdersAsync();`


#### List of supported methods
```csharp
Task<InvoiceModel> BumpBidAsync(string orderId, string authToken, int bidIncrease);
Task<bool> CancelOrderAsync(string orderId, string authToken);
Task<InvoiceModel> CreateOrderAsync(int bid, string filePath, string message);
Task<OrderModel> GetOrderAsync(string orderId, string authToken);
Task<IEnumerable<OrderModel>> GetPendingOrdersAsync(DateTime? before = null);
Task<IEnumerable<OrderModel>> GetQueuedOrdersAsync(int? limit = null);
Task<IEnumerable<OrderModel>> GetSentOrdersAsync(DateTime? before = null);
Task<InfoModel> GetInfoAsync();
Task<Stream> RetrieveMessageAsync(int messageNum);
void ReceiveTransmissionsMessages(Action<EventSourceMessageEventArgs> onReceive, Action<DisconnectEventArgs> onDisconnection);
```

#### Command line interface
A Cli using all the functionality above is provided [here](https://github.com/bokobza/Satellite.NET/tree/master/src/Satellite.NET.Cli).

#### TODO
- Tests
- Web interface
