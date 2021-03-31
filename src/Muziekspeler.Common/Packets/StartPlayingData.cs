using Muziekspeler.Common.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public class StartPlayingData
    {
        [JsonProperty]
        public QueueSong SongToPlay;
    }
}
