using Muziekspeler.UWP.Connectivity;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Muziekspeler.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoomView : Page
    {
        private Client client;

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
