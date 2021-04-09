using Muziekspeler.Common.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.Common.Types
{
    public abstract class Room
    {
        public string Name;

        public int HostUserId;

        public List<User> Users;

        public Queue<QueueSong> SongQueue;

        public QueueSong CurrentSong;

        public abstract Task NextSongAsync();

        public abstract Task ClearQueueAsync();
    }
}
