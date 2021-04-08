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
using Muziekspeler.UWP.Connectivity;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.ApplicationModel;

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
        public Client Client;

        public MainPage()
        {
            this.InitializeComponent();

            Client = new Client();
            HookClientEvents();
            _ = Task.Run(async () => await Client.ConnectAsync());
            roomList.ItemsSource = Rooms;
        }

        private void HookClientEvents()
        {
            Client.RoomListUpdate += Client_RoomListUpdate;
        }

        private void UnhookClientEvents()
        {
            Client.RoomListUpdate -= Client_RoomListUpdate;
        }

        private void Client_RoomListUpdate(RoomListData data)
        {
            _ = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.High,
           () =>
           {
               listRooms(data.RoomNames);
           });
        }

        public void listRooms(List<string> Rooms)
        {
            //ListView roomList = new ListView();

            foreach (string entry in Rooms)
            {
                // do something with entry.Value or entry.Key
                ListViewItem roomList = new ListViewItem();
                Console.Write(entry);
            }
            return;
        }

        private void quit_btn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}

