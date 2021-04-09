using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct ChatMessageData
    {
        [JsonProperty]
        public string Message;

        public override string ToString()
        {
            return Message;
        }
    }
}
