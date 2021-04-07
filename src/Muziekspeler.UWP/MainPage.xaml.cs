using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Muziekspeler.Common;
using Muziekspeler.Common.Types;
using Muziekspeler.Common.Packets;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Muziekspeler.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Dictionary<string, int> Rooms = new Dictionary<string, int>();
        public List<User> users;
        public List<QueueSong> queueSongs;

        public MainPage()
        {
            this.InitializeComponent();
            this.Rooms.Add("kamer", 1);
            roomList.ItemsSource = Rooms;
        }

        public void listRooms(Dictionary<string, int> Rooms)
        {
            //ListView roomList = new ListView();

            foreach (KeyValuePair<string, int> entry in Rooms)
            {
                // do something with entry.Value or entry.Key
                ListViewItem roomList = new ListViewItem();
                Console.Write(entry.Key + entry.Value);
            }
            return;
        }
    }
}

