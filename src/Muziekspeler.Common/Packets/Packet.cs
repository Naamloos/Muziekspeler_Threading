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
            if (data != null)
                Data = JObject.FromObject(data);
            else
                Data = new JObject();
        }
    }

    public enum PacketType
    {
        /// <summary>
        /// Gives the client info about the current room list. Coupled with <seealso cref="RoomListData"/>.
        /// </summary>
        RoomList,

        /// <summary>
        /// Tells the server that a client is trying to join a specific room. Couples with <seealso cref="JoinRoomData"/>
        /// </summary>
        JoinRoom,

        /// <summary>
        /// Either tells the server a client is leaving, or kicks a client. Coupled with <seealso cref="ReasonData"/>
        /// </summary>
        LeaveRoom,

        /// <summary>
        /// A chat message. Clients can send these, and servers can broadcast these. Coupled with <seealso cref="ChatMessageData"/>
        /// </summary>
        ChatMessage,

        /// <summary>
        /// Sent by the server to update room data. Sent on queue change or user list change. Coupled with <seealso cref="RoomUpdateData"/>
        /// </summary>
        RoomUpdate,

        /// <summary>
        /// Sent by server to request a ping back. Sent by client to notify connection is still alive. No data.
        /// </summary>
        KeepAlive,

        /// <summary>
        /// Tells the room music is starting.
        /// </summary>
        PlayMusic,

        /// <summary>
        /// Pauses music. Client-side just causes a UI update.
        /// </summary>
        PauseMusic,

        /// <summary>
        /// Clears queue. Host can send this to server, server sends this to update UI.
        /// </summary>
        ClearQueue,

        /// <summary>
        /// Host can use this to skip a song.
        /// </summary>
        SkipSong,

        /// <summary>
        /// Tells the server that a client wants to create a new room. Uses <seealso cref="CreateRoomData"/>
        /// </summary>
        CreateRoom,

        /// <summary>
        /// Updates a user's Display name and Status. Uses <seealso cref="Packets.SetUserData"/>
        /// </summary>
        SetUserData,

        /// <summary>
        /// Used by server to assign a User ID to a client. <seealso cref="UserIdData"/>
        /// </summary>
        UserId,

        /// <summary>
        /// Tells a client to start playing a song they enqueues. <seealso cref="StartPlayingData"/>
        /// </summary>
        StartPlaying,

        /// <summary>
        /// Indicates to a client that something failed. Coupled with <seealso cref="ReasonData"/>
        /// </summary>
        Fail,

        /// <summary>
        /// Indicates playback is done.
        /// </summary>
        Done,

        EncodingData,

        QueueSong
    }
}
