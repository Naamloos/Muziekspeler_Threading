using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct RoomListData
    {
        [JsonProperty]
        public List<string> RoomNames;
    }
}
