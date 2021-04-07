using Muziekspeler.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.UWP.Connectivity
{
    // This is a stream for use in a MediaElement.
    public class AudioFilterStream : MemoryStream
    {
        public AudioFilterStream(Connection connection)
        {
            connection.MediaReceived = handleMediaAsync;
        }

        private async Task handleMediaAsync(byte[] data)
        {
            // write newly received data to self
            long pos = Position;
            Position = Length;
            Write(data, 0, data.Length);
            Position = pos;
        }
    }
}
