using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct JoinRoomData
    {
        [JsonProperty]
        public string RoomName;

        [JsonProperty]
        public string Password;
    }
}
