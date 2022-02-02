using CS82ANGULAR.ViewModel;
using System.Windows;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for WindowBatch.xaml
    /// </summary>
    public partial class WindowBatch : Window
    {

        //BackgroundWorker Worker;
        public WindowBatch(BatchProcessingViewModel dataContext)
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            this.SetDataContext(dataContext);
        }
        public void SetDataContext(BatchProcessingViewModel dataContext)
        {
            this.DataContext = dataContext;
        }


        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
