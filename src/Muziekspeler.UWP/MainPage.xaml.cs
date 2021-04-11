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
using Windows.Storage;
using Windows.Media.Core;
using Windows.Media.Audio;
using Windows.Media.MediaProperties;
using Windows.UI.Popups;

namespace Muziekspeler.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<string> Rooms = new ObservableCollection<string>();
        public List<User> users;
        public List<QueueSong> queueSongs;
        public Client Client;

        public MainPage()
        {
            this.InitializeComponent();

            Client = Client.Get();

            HookClientEvents();
            requestRooms();
            roomList.ItemsSource = Rooms;
        }

        private void HookClientEvents()
        {
            Client.RoomListUpdate += Client_RoomListUpdate;
            Client.JoinRoom += Client_JoinRoom;
            Client.ServerError += Client_ServerError;
        }

        private void Client_ServerError(ReasonData data)
        {
            RunOnUi(async () =>
            {
                _ = new MessageDialog($"Server error!\n{data.Reason}").ShowAsync();
            });
        }

        private async void RunOnUi(Action action)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                action();
            });
        }

        private void Client_JoinRoom(JoinRoomData data)
        {
            goToRoomView();
        }

        private void UnhookClientEvents()
        {
            Client.RoomListUpdate -= Client_RoomListUpdate;
            Client.JoinRoom -= Client_JoinRoom;
        }

        private async void requestRooms()
        {
            await Client.ServerConnection.SendPacketAsync(new Packet(PacketType.RoomList, null));
        }

        private async void Client_RoomListUpdate(RoomListData data)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                listRooms(data.RoomNames);
            });
        }

        public void listRooms(List<string> rooms)
        {
            //ListView roomList = new ListView();
            Rooms.Clear();
            foreach(string room in rooms)
                Rooms.Add(room);
            return;
        }

        private async void goToRoomView()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                UnhookClientEvents();
                this.Frame.Navigate(typeof(RoomView));
            });
        }

        private async void hostRoom(object sender, RoutedEventArgs e)
        {
            // do stuff to host a new room
            await Client.ServerConnection.SendPacketAsync(new Packet(PacketType.CreateRoom, new CreateRoomData() { Name = new Random().Next().ToString() }));
            // if creation is successful, server tells client to join
        }

        private async void joinRoom(object sender, RoutedEventArgs e)
        {
            // do stuff to join existing room
            string selected = (string)roomList.SelectedItem;
            if (!string.IsNullOrEmpty(selected))
                await Client.ServerConnection.SendPacketAsync(new Packet(PacketType.JoinRoom, new JoinRoomData() { RoomName = selected }));
            else
            {
                RunOnUi(async () =>
                {
                    _ = new MessageDialog($"Please select a room to join.").ShowAsync();
                });
            }

            // Server should tell the client to join if all is OK
        }

        private void refreshRoomList(object sender, RoutedEventArgs e)
        {
            requestRooms();
        }

        private void quit_btn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}

