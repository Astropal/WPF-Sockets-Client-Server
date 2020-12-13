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

namespace SocketServer
{
    /// <summary>
    /// MainWindow.xaml Server
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socketWatch = null;
        Socket socketCommunication = null;
        Dictionary<string, Socket> dicSocket;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (socketWatch == null)
            {
                dicSocket = new Dictionary<string, Socket>();
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
                    IPEndPoint endPoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
                    socketWatch.Bind(endPoint);

                    socketWatch.Listen(20);   //Limite de clients (surcharge réseau)
                    Listen();
                    ShowMsg("Démarrage du serveur terminé！");
                }
                catch (Exception error)
                {
                    ShowMsg("Erreur démarrage du serveur：" + error.ToString());
                }
            }
        }

        private void ShowMsg(string msg)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                lbInfo.Items.Add(msg);
            }));

        }

        private async void Listen()
        {
            await Task.Run(new Action(() =>
            {
                while (true)
                {
                    socketCommunication = socketWatch.Accept();
                    Receive(socketCommunication);
                    dicSocket.Add(socketCommunication.RemoteEndPoint.ToString(), socketCommunication);

                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lbClientList.Items.Add(socketCommunication.RemoteEndPoint.ToString());
                    }));

                    ShowMsg("Un nouvel utilisateur ! Son adresse ip est：" + socketCommunication.RemoteEndPoint.ToString());
                }
            }));
        }

        private async void Receive(Socket socket)
        {
            await Task.Run(new Action(() =>
            {
                while (true)
                {
                    byte[] bytes = new byte[1024 * 1024 * 2];
                    try
                    {
                        int length = socket.Receive(bytes);
                        string msg = Encoding.UTF8.GetString(bytes, 0, length);
                        ShowMsg("Reçu de" + socket.RemoteEndPoint.ToString() + "：" + msg);
                    }
                    catch (SocketException error)
                    {
                        ShowMsg("Erreur：" + error.ToString());
                    }
                }
            }));
        }

        private void BtnSendAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = txtInput.Text.Trim();
                foreach (var socket in dicSocket.Values)    //Message de groupe
                {
                    socket.Send(Encoding.UTF8.GetBytes(msg));
                }
                ShowMsg("Message serveur：" + msg);
            }
            catch (Exception error)
            {
                ShowMsg("Erreur：" + error.ToString());
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (lbClientList.SelectedIndex > -1)
            {
                try
                {
                    string msg = txtInput.Text.Trim();
                    dicSocket[lbClientList.SelectedItem.ToString()].Send(Encoding.UTF8.GetBytes(msg));   //Message à un client
                    ShowMsg("Message envoyé：" + msg);
                }
                catch (Exception er)
                {
                    ShowMsg("Erreur：" + er.ToString());
                }
            }
            else
            {
                ShowMsg("Veuillez sélectionner un client！");
            }
        }
    }
}
