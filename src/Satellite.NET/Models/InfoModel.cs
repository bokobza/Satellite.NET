using Newtonsoft.Json;

namespace Satellite.NET.Models
{
    public class InfoModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("num_peers")]
        public long PeersCount { get; set; }

        [JsonProperty("num_pending_channels")]
        public long PendingChannelsCount { get; set; }

        [JsonProperty("num_active_channels")]
        public long ActiveChannelsCount { get; set; }

        [JsonProperty("num_inactive_channels")]
        public long InactiveChannelsCount { get; set; }

        [JsonProperty("address")]
        public Address[] Address { get; set; }

        [JsonProperty("binding")]
        public Address[] Binding { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("blockheight")]
        public long Blockheight { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("msatoshi_fees_collected")]
        public long MsatoshiFeesCollected { get; set; }

        [JsonProperty("fees_collected_msat")]
        public string FeesCollectedMsat { get; set; }
    }

    public partial class Address
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("address")]
        public string AddressAddress { get; set; }

        [JsonProperty("port")]
        public long Port { get; set; }
    }
}
