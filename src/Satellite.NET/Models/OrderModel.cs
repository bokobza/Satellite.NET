using System;
using Newtonsoft.Json;

namespace Satellite.NET.Models
{
    public class OrderModel
    {
        [JsonProperty("bid")]
        public long Bid { get; set; }

        [JsonProperty("message_size")]
        public long MessageSize { get; set; }

        [JsonProperty("bid_per_byte")]
        public double BidPerByte { get; set; }

        [JsonProperty("message_digest")]
        public string MessageDigest { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("started_transmission_at")]
        public DateTimeOffset? StartedTransmissionAt { get; set; }

        [JsonProperty("ended_transmission_at")]
        public DateTimeOffset? EndedTransmissionAt { get; set; }

        [JsonProperty("tx_seq_num")]
        public long? TxSeqNum { get; set; }

        [JsonProperty("unpaid_bid")]
        public long UnpaidBid { get; set; }
    }
}
