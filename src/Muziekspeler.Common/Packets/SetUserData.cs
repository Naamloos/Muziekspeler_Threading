using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct SetUserData
    {
        [JsonProperty]
        public string DisplayName;

        [JsonProperty]
        public string Password;
    }
}
