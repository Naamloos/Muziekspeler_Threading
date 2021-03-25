using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct CreateRoomData
    {
        [JsonProperty]
        public string Name;

        [JsonProperty]
        public string Password;
    }
}
