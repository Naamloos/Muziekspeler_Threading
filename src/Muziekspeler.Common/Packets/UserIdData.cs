using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public class UserIdData
    {
        [JsonProperty]
        public int Id;
    }
}
