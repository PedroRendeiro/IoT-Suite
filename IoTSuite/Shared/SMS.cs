using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IoTSuite.Shared
{
    public class SMSDeliveryReceipt
    {
        public ulong timestamp_send { get; set; }
        public DateTime TimeStampSend {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp_send); ;
            }
        }

        public ulong timestamp { get; set; }
        public DateTime TimeStamp
        {
            get
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp); ;
            }
        }

        public Guid message_id { get; set; }

        public ClickSendMessageStatus status { get; set; }

        public HttpStatusCode status_code { get; set; }

        public string status_text { get; set; }

        public string error_code { get; set; }

        public string error_text { get; set; }

        public string custom_string { get; set; }

        public string user_id { get; set; }

        public string subaccount_id { get; set; }

        public ClickSendMessageType message_type { get; set; }
    }

    public enum ClickSendMessageStatus
    {
        Undelivered,
        Delivered
    }

    public enum ClickSendMessageType
    {
        SMS
    }
}
