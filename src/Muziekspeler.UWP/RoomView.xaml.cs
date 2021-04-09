using Muziekspeler.Common.Packets;
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
using Windows.UI.Popups;
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
        private ObservableCollection<ChatMessageData> chat = new ObservableCollection<ChatMessageData>();
        private ObservableCollection<QueueSong> songqueue = new ObservableCollection<QueueSong>();
        private ObservableCollection<User> userlist = new ObservableCollection<User>();

        public RoomView()
        {
            this.InitializeComponent();
            client = Client.Get();
            hookEvents();
            player.MediaEnded += Player_MediaEnded;

            this.listViewChat.ItemsSource = chat;
            this.listViewQueue.ItemsSource = songqueue;
            this.listViewUsers.ItemsSource = userlist;
        }

        private async void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            await client.ServerConnection.SendPacketAsync(new Common.Packets.Packet(Common.Packets.PacketType.Done, null));
        }

        private void leaveButton(object sender, RoutedEventArgs e)
        {
            leaveRoom();
        }

        private async void enqueueButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var newuri = new Uri(this.urlinput.Text);
                await client.ServerConnection.SendPacketAsync(new Packet(PacketType.QueueSong, new QueueSongData() { Song = newuri.ToString() }));
            }
            catch (Exception)
            {
                RunOnUi(async () =>
                {
                    _ = new MessageDialog($"That URI seems invalid! :(").ShowAsync();
                });
            }
        }

        private void hookEvents()
        {
            client.ChatMessageReceived += Client_ChatMessageReceived;
            client.PausePlaying += Client_PausePlaying;
            client.RoomUpdated += Client_RoomUpdated;
            client.ServerError += Client_ServerError;
            client.StartPlaying += Client_StartPlaying;
        }

        private async void RunOnUi(Action action)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () =>
            {
                action();
            });
        }

        private void Client_StartPlaying(Common.Packets.StartPlayingData data)
        {
            RunOnUi(() =>
            {
                this.player.Stop();
                this.player.Source = new Uri(data.SongToPlay.Path);
                this.player.Play();
            });
        }

        private void Client_ServerError(Common.Packets.ReasonData data)
        {
            RunOnUi(async () => 
            {
                _ = new MessageDialog($"Server did an oopsie! {data.Reason}").ShowAsync();
            });
        }

        private void Client_RoomUpdated(Common.Types.Room data)
        {
            RunOnUi(() =>
            {
                if (data.SongQueue.Count() > 0)
                    songname.Text = data.SongQueue.Peek().ToString();
                else
                    songname.Text = "No songs in queue..";

                this.songqueue.Clear();
                foreach (var song in data.SongQueue)
                {
                    this.songqueue.Add(song);
                }

                this.userlist.Clear();
                foreach (var user in data.Users)
                {
                    this.userlist.Add(user);
                }
            });
        }

        private void Client_PausePlaying()
        {
            // /shrug
        }

        private void Client_ChatMessageReceived(Common.Packets.ChatMessageData data)
        {
            RunOnUi(() =>
            {
                this.chat.Add(data);
                listViewChat.ScrollIntoView(data);
            });
        }

        private void unhookEvents()
        {
            client.ChatMessageReceived -= Client_ChatMessageReceived;
            client.PausePlaying -= Client_PausePlaying;
            client.RoomUpdated -= Client_RoomUpdated;
            client.ServerError -= Client_ServerError;
            client.StartPlaying -= Client_StartPlaying;
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

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(chatBox.Text))
                await client.ServerConnection.SendPacketAsync(new Packet(PacketType.ChatMessage, new ChatMessageData() { Message = chatBox.Text }));
        }

        private async void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            await client.ServerConnection.SendPacketAsync(new Packet(PacketType.SkipSong, null));
        }
    }
}
