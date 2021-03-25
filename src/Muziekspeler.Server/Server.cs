using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Server
{
    public class Server
    {
        public List<UserConnection> Clients;
        public Dictionary<string, Room> Rooms;
    }
}
