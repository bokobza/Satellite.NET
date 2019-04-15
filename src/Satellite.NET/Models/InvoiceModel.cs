using System;
using Newtonsoft.Json;

namespace Satellite.NET.Models
{
    public class InvoiceModel
    {
        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }

        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("lightning_invoice")]
        public LightningInvoiceModel LightningInvoice { get; set; }
    }

    public class LightningInvoiceModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("msatoshi")]
        public long Msatoshi { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rhash")]
        public string Rhash { get; set; }

        [JsonProperty("payreq")]
        public string Payreq { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("metadata")]
        public MetadataModel Metadata { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class MetadataModel
    {
        [JsonProperty("msatoshis_per_byte")]
        public long MsatoshisPerByte { get; set; }

        [JsonProperty("sha256_message_digest")]
        public string Sha256MessageDigest { get; set; }

        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }
    }
}
