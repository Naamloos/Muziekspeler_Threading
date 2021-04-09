using Muziekspeler.Common.Types;
using Muziekspeler.UWP.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Muziekspeler.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoomView : Page
    {
        private Client client;
        private ObservableCollection<string> chat;
        private ObservableCollection<QueueSong> songqueue;
        private ObservableCollection<User> userlist;

        public RoomView()
        {
            this.InitializeComponent();
            client = Client.Get();
            hookEvents();
        }

        private void leaveButton(object sender, RoutedEventArgs e)
        {
            leaveRoom();
        }

        private void enqueueButton(object sender, RoutedEventArgs e)
        {
        }

        private void hookEvents()
        {
            client.ChatMessageReceived += Client_ChatMessageReceived;
            client.PausePlaying += Client_PausePlaying;
            client.RoomUpdated += Client_RoomUpdated;
            client.ServerError += Client_ServerError;
            client.StartPlaying += Client_StartPlaying;
        }

        private void Client_StartPlaying(Common.Packets.StartPlayingData data)
        {
            throw new NotImplementedException();
        }

        private void Client_ServerError(Common.Packets.ReasonData data)
        {
            throw new NotImplementedException();
        }

        private void Client_RoomUpdated(Common.Types.Room data)
        {
            this.songqueue.Clear();
            foreach(var song in data.SongQueue)
            {
                this.songqueue.Add(song);
            }

            this.userlist.Clear();
        }

        private void Client_PausePlaying()
        {
            throw new NotImplementedException();
        }

        private void Client_ChatMessageReceived(Common.Packets.ChatMessageData data)
        {
            throw new NotImplementedException();
        }

        private void unhookEvents()
        {

        }

        private async void leaveRoom()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                unhookEvents();
                this.Frame.Navigate(typeof(MainPage));
            });
        }
    }
}
