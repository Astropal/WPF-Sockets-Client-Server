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
                catch (Exception er)
                {
                    ShowMsg("Erreur démarrage du serveur：" + er.ToString());
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
                    // Nouvel utilisateur
                }
            }));
        }

        //TODO
        private async void Receive()
        {
            await Task.Run(new Action(() =>
            {
                while (true)
                {
                    // Message Utilisateurs
                    // Execption
                }
            }));
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
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
            catch (Exception er)
            {
                ShowMsg("Erreur：" + er.ToString());
            }
        }
    }
}
