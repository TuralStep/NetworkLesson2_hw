using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Client
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 45678;
            remoteEP = new IPEndPoint(ip, port);
        }

        private Socket client;
        private EndPoint remoteEP;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            var buffer = new byte[ushort.MaxValue - 29];
            await client.SendToAsync(buffer, SocketFlags.None, remoteEP);
            var list = new List<byte>();
            var len = 0;
            do
            {
                SocketReceiveFromResult result = default;
                try
                {
                    result = await client.ReceiveFromAsync(buffer, SocketFlags.None, remoteEP);
                    len = result.ReceivedBytes;

                    list.AddRange(buffer.Take(len));

                    try
                    {
                        BitmapImage image;
                        var imageData = list.ToArray();

                        if (imageData == null || imageData.Length == 0)
                            image = null;
                        else
                        {
                            image = new BitmapImage();
                            using (var mem = new MemoryStream(imageData))
                            {
                                mem.Position = 0;
                                image.BeginInit();
                                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.UriSource = null;
                                image.StreamSource = mem;
                                image.EndInit();
                            }
                            image.Freeze();
                            
                        }

                        if (image != null)
                            imageView.Source = image;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Server is not running");
                }

            } while (len == buffer.Length);

        }

    }

}
