using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Packets
{
    public struct Packet
    {
        [JsonProperty]
        public PacketType Type;

        [JsonProperty]
        public JObject Data;

        public Packet(PacketType type, object data)
        {
            Type = type;
            Data = JObject.FromObject(data);
        }
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
        SetUserData,
        UserId
    }
}
