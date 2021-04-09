using Muziekspeler.Common.Packets;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Muziekspeler.Common
{
    public class Connection
    {
        private TcpClient packetStream;
        private CancellationTokenSource cancellation;
        private Semaphore semaphore;

        public delegate Task PacketHandlerAsync(Packet packet);
        public delegate Task MediaHandlerAsync(byte[] packet);

        public PacketHandlerAsync PacketReceived;
        public MediaHandlerAsync MediaReceived;

        public Connection(TcpClient packetStream, PacketHandlerAsync PacketReceived)
        {
            this.packetStream = packetStream;
            this.PacketReceived = PacketReceived;
            this.cancellation = new CancellationTokenSource();
            this.semaphore = new Semaphore(1, 1);
        }

        public void StartClientLoop()
        {
            _ = Task.Run(async () => await clientLoop());
        }

        private async Task clientLoop()
        {
            await Task.Yield(); // begone squiggly line
            var stream = packetStream.GetStream();
            var reader = new BinaryReader(stream);
            while (!cancellation.IsCancellationRequested)
            {
                if (stream.DataAvailable)
                {
                    var isPacket = reader.ReadBoolean();
                    if (isPacket)
                    {
                        // read packet and handle
                        var contentString = reader.ReadString();
                        var packet = JsonConvert.DeserializeObject<Packet>(contentString);
                        handlePacket(packet);
                    }
                    else
                    {
                        var bytestoread = reader.ReadInt32();
                        var data = reader.ReadBytes(bytestoread);
                        handleMedia(data);
                    }
                }
            }
        }

        public void StopClientLoop()
        {
            cancellation.Cancel();
            packetStream.Close();
        }

        public async Task SendPacketAsync(Packet packet)
        {
            try
            {
                await Task.Yield(); // begone squiggly line
                semaphore.WaitOne();
                // Send packet
                var stream = packetStream.GetStream();
                var writer = new BinaryWriter(stream);
                writer.Write(true); // tell other end that this is a packet and not media.
                writer.Write(JsonConvert.SerializeObject(packet));
            }
            catch (Exception ex) { }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task SendDataAsync(byte[] data)
        {
            await Task.Yield(); // begone squiggly line
            semaphore.WaitOne();
            // Send packet
            var stream = packetStream.GetStream();
            var writer = new BinaryWriter(stream);
            writer.Write(false); // tell other end that this is a media and not packet.
            writer.Write(data.Length);
            writer.Write(data);
            semaphore.Release();
        }

        private void handlePacket(Packet packet)
        {
            _ = Task.Run(async () => await this.PacketReceived(packet));
        }

        //Leftover methods. Keeping this just in case we decide to use this in any other way.
        private void handleMedia(byte[] data)
        {
            if(this.MediaReceived != null)
                _ = Task.Run(async () => await this.MediaReceived(data));
        }
    }
}
