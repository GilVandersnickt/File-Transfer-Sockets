using System.Windows;

namespace Ait.FTSock.Client
{
    /// <summary>
    /// Interaction logic for CreateFolderWindow.xaml
    /// </summary>
    public partial class CreateFolderWindow : Window
    {
        public string FolderName { get; private set; }
        public CreateFolderWindow()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            FolderName = txtFolderName.Text;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            FolderName = string.Empty;
            Close();
        }
    }
}
