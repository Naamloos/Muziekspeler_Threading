using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct RoomUpdateData
    {
        public int HostId;
        public Queue<QueueSong> Queue;
        public List<User> Users;
    }
}
