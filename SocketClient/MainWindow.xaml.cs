using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace SocketClient
{
    /// <summary>
    /// MainWindow.xaml Client
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socketClient = null; 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = txtInput.Text.Trim();
                if (socketClient != null && !string.IsNullOrEmpty(msg))
                {
                    socketClient.Send(Encoding.UTF8.GetBytes(msg));
                    ShowMsg("Message envoyé：" + msg);
                }
            }
            catch (Exception er)
            {
                ShowMsg("Erreur：" + er.ToString());
            }

        }

        private async void Recive()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] bytes = new byte[1024 * 1024 * 2];
                        int length = socketClient.Receive(bytes);
                        string msg = Encoding.UTF8.GetString(bytes, 0, length);
                        ShowMsg("Message reçue：" + msg);
                    }
                    catch (Exception er)
                    {
                        ShowMsg("Erreur：" + er.ToString());
                        break;
                    }
                }
            });
        }

        private void btnLinkServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (socketClient == null)
                {
                    socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socketClient.Connect(IPAddress.Parse(txtIP.Text.Trim()), int.Parse(txtPort.Text.Trim()));
                    ShowMsg(string.Format("Connexion au Serveur（{0}:{1}）Réussie！", txtIP.Text.Trim(), txtPort.Text.Trim()));
                    Recive();
                }
            }
            catch (Exception er)
            {
                ShowMsg("Erreur：" + er.ToString());
                socketClient = null;
            }
        }

        private void ShowMsg(string msg)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbInfo.Items.Add(msg);
            }));
        }
    }
}
