using Ait.FTSock.Core.Constants;
using Ait.FTSock.Core.Services;
using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ait.FTSock.Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        ValidatorService validatorService;
        NetworkInterfaceService networkInterfaceService;
        ServerService serverService;
        bool serverOnline;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            validatorService = new ValidatorService();
            networkInterfaceService = new NetworkInterfaceService();
            serverService = new ServerService();

            PopulateUI();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
            if (serverOnline)
                StopServer();
        }
        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (Validate(cmbIpAddresses.SelectedItem, cmbPorts.SelectedItem))
                OperateServer();
            else
                MessageBox.Show(ErrorMessages.ValidationErrorMessage, ErrorMessages.ValidationTitleErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void btnBasePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty && result.Equals(System.Windows.Forms.DialogResult.OK))
            {
                txtBasePath.Text = openFileDlg.SelectedPath;
            }
        }
        private void OperateServer()
        {
            if (btnStartStop.Content.Equals(ServerOperation.Start))
                StartServer();
            else
                StopServer();
        }
        private void StartServer()
        {
            UpdateUI(true);
            StartListening();
        }
        private void StopServer()
        {
            UpdateUI(false);
            StopListening();
        }
        private void PopulateUI()
        {
            cmbIpAddresses.ItemsSource = networkInterfaceService.GetLocalIpAddresses();
            cmbPorts.ItemsSource = networkInterfaceService.GetPorts();
            GetLastSettings();
        }
        private void UpdateUI(bool start)
        {
            if (start)
            {
                btnStartStop.Content = ServerOperation.Stop;
                btnStartStop.Background = Brushes.Red;
                btnStartStop.Foreground = Brushes.White;
                gpbCommunications.Visibility = Visibility.Visible;
                cmbIpAddresses.IsEnabled = false;
                cmbPorts.IsEnabled = false;
                txtBasePath.IsEnabled = false;
                btnBasePath.IsEnabled = false;
                lsbCommunications.ItemsSource = null;
            }
            else
            {
                btnStartStop.Content = ServerOperation.Start;
                btnStartStop.Background = Brushes.ForestGreen;
                btnStartStop.Foreground = Brushes.White;
                gpbCommunications.Visibility = Visibility.Hidden;
                cmbIpAddresses.IsEnabled = true;
                cmbPorts.IsEnabled = true;
                txtBasePath.IsEnabled = true;
                btnBasePath.IsEnabled = true;
            }
        }
        private void UpdateListBox()
        {
            lsbCommunications.ItemsSource = null;
            lsbCommunications.ItemsSource = serverService.CommunicationStrings;
        }
        private void StartListening()
        {
            string ipAddress = cmbIpAddresses.SelectedItem.ToString();
            int port = (int)cmbPorts.SelectedItem;
            string basePath = txtBasePath.Text;

            serverService.Start(ipAddress, port, basePath);
            serverOnline = true;
            try
            {
                serverService.ServerSocket.Bind(serverService.ServerEndpoint);
                serverService.ServerSocket.Listen(int.MaxValue);
                while (serverService.ServerOnline)
                {
                    DoEvents();
                    if (serverService.ServerSocket != null)
                    {
                        if (serverService.ServerSocket.Poll(200000, SelectMode.SelectRead))
                        {
                            serverService.HandleClientCall(serverService.ServerSocket.Accept());
                            UpdateListBox();
                        }
                    }
                    if (Application.Current == null)
                        break;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void StopListening()
        {
            serverService.Stop();
            serverOnline = false;
        }
        private static void DoEvents()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
            catch (Exception error)
            {
                Application.Current.Dispatcher.DisableProcessing();
            }
        }
        private bool Validate(object ipAddress, object port)
        {
            return validatorService.Validate(ipAddress, port);
        }
        private void GetLastSettings()
        {
            cmbIpAddresses.SelectedIndex = Properties.Settings.Default.LastIpAddress;
            cmbPorts.SelectedIndex = Properties.Settings.Default.LastPort;
            txtBasePath.Text = Properties.Settings.Default.LastBasePath;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.LastIpAddress = cmbIpAddresses.SelectedIndex;
            Properties.Settings.Default.LastPort = cmbPorts.SelectedIndex;
            Properties.Settings.Default.LastBasePath = txtBasePath.Text;

            Properties.Settings.Default.Save();
        }
    }
}
