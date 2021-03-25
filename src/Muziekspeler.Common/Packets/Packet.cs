using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct Packet
    {
        public PacketType Type;
        public JObject Data;
    }

    public enum PacketType
    {
        RoomList,
        JoinRoom,
        LeaveRoom,
        ChatMessage,
        RoomUpdate,
        KeepAlive,
        PlayMusic,
        PauseMusic,
        ClearQueue,
        SkipSong,
        CreateRoom,
        SetUserData
    }
}
