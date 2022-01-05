using Ait.FTSock.Core.Constants;
using Ait.FTSock.Core.Entities;
using Ait.FTSock.Core.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ait.FTSock.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ValidatorService validatorService;
        NetworkService networkService;
        FTService ftService;
        ServerService serverService;
        User currentUser;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            validatorService = new ValidatorService();
            networkService = new NetworkService();
            ftService = new FTService();
            serverService = new ServerService();

            PopulateUI();
        }

        private void btnConnectDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (Validate(txtIpAddress.Text, cmbPorts.SelectedItem))
                OperateConnection();
            else
                MessageBox.Show(ErrorMessages.ValidationErrorMessage, ErrorMessages.ValidationTitleErrorMessage, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void OperateConnection()
        {
            if (btnConnectDisconnect.Content.Equals(ButtonConnectionValues.Connect))
                Connect();
            else
                Disconnect();
        }
        private void Connect()
        {
            SaveSettings();
            ConnectWithServer();

        }
        private void ConnectWithServer()
        {
            string request = ftService.GetConnectionRequest(txtUsername.Text);
            string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
            if (responseString != "")
            {
                Response response = serverService.ConvertToResponse(responseString);
                currentUser = new User { Id = response.Id, ActivePath = response.ActivePath };
                UpdateUI(true);
                DisplayResponse(response);
            }
            else
            {
                MessageBox.Show(ErrorMessages.ServerUnreachableErrorMessage);
                UpdateUI(false);
            }
        }
        private void UpdateUI(bool isConnected)
        {
            if (isConnected)
            {
                btnConnectDisconnect.Content = ButtonConnectionValues.Disconnect;
                btnConnectDisconnect.Background = Brushes.Red;
                btnConnectDisconnect.Foreground = Brushes.White;
                gpbFts.Visibility = Visibility.Visible;
                cmbPorts.IsEnabled = false;
                txtIpAddress.IsEnabled = false;
                txtUsername.IsEnabled = false;
            }
            else
            {
                btnConnectDisconnect.Content = ButtonConnectionValues.Connect;
                btnConnectDisconnect.Background = Brushes.ForestGreen;
                btnConnectDisconnect.Foreground = Brushes.White;
                gpbFts.Visibility = Visibility.Hidden;
                lblActiveFolder.Content = "";
                lblUserId.Content = "";
                lblFolderName.Content = "";
                lblFullPathFolder.Content = "";
                lblParent.Content = "";
                lblDate.Content = "";
                lblFileName.Content = "";
                lblFileSize.Content = "";
                lblFullPathFile.Content = "";
                lsbFolders.Items.Clear();
                lsbFiles.Items.Clear();
                cmbPorts.IsEnabled = true;
                txtIpAddress.IsEnabled = true;
                txtUsername.IsEnabled = true;
            }
        }
        private void DisplayResponse(Response response)
        {
            lblUserId.Content = response.Id;
            lblFileName.Content = response.RequestName;
            lblActiveFolder.Content = response.ActivePath;
            lsbFolders.Items.Clear();
            lsbFiles.Items.Clear();
            lsbFolders.Items.Add("..");
            foreach (var folder in response.SubFolders)
            {
                lsbFolders.Items.Add(folder);
            }
            foreach (var file in response.Files)
            {
                lsbFiles.Items.Add(file);
            }
        }
        private void Disconnect()
        {
            string request = ftService.GetCloseRequest(currentUser.Id.ToString());
            string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
            serverService.Disconnect();

            UpdateUI(false);
        }
        private void PopulateUI()
        {
            cmbPorts.ItemsSource = networkService.GetPorts();
            GetLastSettings();
        }
        private bool Validate(string ipAddress, object port)
        {
            return validatorService.Validate(ipAddress, port);
        }
        private void GetLastSettings()
        {
            txtIpAddress.Text = Properties.Settings.Default.LastServerIpAddress;
            cmbPorts.SelectedIndex = Properties.Settings.Default.LastServerPort;
            txtUsername.Text = Properties.Settings.Default.LastUsername;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.LastServerIpAddress = txtIpAddress.Text;
            Properties.Settings.Default.LastServerPort = cmbPorts.SelectedIndex;
            Properties.Settings.Default.LastUsername = txtUsername.Text;
            Properties.Settings.Default.Save();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
            Disconnect();
        }
        private void lsbFolders_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItem is string)
            {
                string request = ftService.GetCDUPRequest(currentUser.Id.ToString());
                string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
                if (responseString != "")
                {
                    Response response = serverService.ConvertToResponse(responseString);
                    DisplayResponse(response);
                }
                else
                {
                    MessageBox.Show(ErrorMessages.ServerUnreachableErrorMessage);
                    UpdateUI(false);
                }
            }
            else
            {
                FTFolder selectedFolder = (FTFolder)listBox.SelectedItem;
                if (selectedFolder != null)
                {
                    lblFolderName.Content = selectedFolder.Name;
                    lblFullPathFolder.Content = selectedFolder.FullPath;
                    lblParent.Content = selectedFolder.Parent;
                }
            }
        }
        private void lsbFiles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            FTFile selectedFile = (FTFile)listBox.SelectedItem;
            if (selectedFile != null)
            {
                lblFileName.Content = selectedFile.Name;
                lblFileSize.Content = selectedFile.Size + " B";
                lblFullPathFile.Content = selectedFile.FullPath;
                lblDate.Content = selectedFile.CreationDate;
            }
        }
        private void lsbFolders_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItem is string)
            {

            }
            else
            {
                FTFolder selectedFolder = (FTFolder)listBox.SelectedItem;
                if (selectedFolder != null)
                {
                    string request = ftService.GetCDDIRRequest(currentUser.Id.ToString(), selectedFolder.FullPath);
                    string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
                    if (responseString != "")
                    {
                        Response response = serverService.ConvertToResponse(responseString);
                        DisplayResponse(response);
                    }
                    else
                    {
                        MessageBox.Show(ErrorMessages.ServerUnreachableErrorMessage);
                        UpdateUI(false);
                    }
                }
            }
        }
        private void lsbFiles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            FTFile selectedFile = (FTFile)listBox.SelectedItem;
            if (selectedFile != null)
            {
                string request = ftService.GetGETRequest(currentUser.Id.ToString(), selectedFile.Name);
                string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
                if (responseString != "")
                {
                    ResponseWithFile response = serverService.ConvertToResponseWithFile(responseString);

                    string filePath = "";
                    System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
                    var result = openFileDialog.ShowDialog();
                    if (result.ToString() != string.Empty && result.Equals(System.Windows.Forms.DialogResult.OK))
                    {
                        filePath = openFileDialog.SelectedPath;
                        byte[] bytes = response.FileBytes;

                        BinaryWriter binaryWriter = new BinaryWriter(File.Open(Path.Combine(filePath, response.FileName), FileMode.Append));
                        binaryWriter.Write(bytes, 0, response.FileBytes.Length);
                        binaryWriter.Close();
                    }
                    DisplayResponse(response);
                }
                else
                {
                    MessageBox.Show(ErrorMessages.ServerUnreachableErrorMessage);
                    UpdateUI(false);
                }
            }
        }
        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            CreateFolderWindow createFolderWindow = new CreateFolderWindow();
            createFolderWindow.ShowDialog();
            if (!string.IsNullOrEmpty(createFolderWindow.FolderName))
            {
                string request = ftService.GetMKDIRRequest(currentUser.Id.ToString(), createFolderWindow.FolderName);
                string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
                if (responseString != "")
                {
                    Response response = serverService.ConvertToResponse(responseString);
                    DisplayResponse(response);
                }
                else
                {
                    MessageBox.Show(ErrorMessages.ServerUnreachableErrorMessage);
                    UpdateUI(false);
                }
            }
        }
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "";
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result.ToString() != string.Empty && result.Equals(System.Windows.Forms.DialogResult.OK))
            {
                filePath = openFileDialog.FileName;
            }
            if (filePath != "")
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                DirectoryInfo directory = new DirectoryInfo(filePath);

                string request = ftService.GetPUTRequest(currentUser.Id.ToString(), directory.Name, fileData);
                string responseString = serverService.GetResponse(txtIpAddress.Text, cmbPorts.SelectedItem.ToString(), request);
                if (responseString != "")
                {
                    Response response = serverService.ConvertToResponse(responseString);
                    DisplayResponse(response);
                }
                else
                {
                    MessageBox.Show(ErrorMessages.ServerUnreachableErrorMessage);
                    UpdateUI(false);
                }
            }

        }
    }
}
