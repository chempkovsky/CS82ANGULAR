using CS82ANGULAR.ViewModel;
using System;
using System.Windows;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for WindowCS2ANGLAR.xaml
    /// </summary>
    public partial class WindowCS2ANGLAR : Window
    {

        //BackgroundWorker Worker;
        public WindowCS2ANGLAR(MainWindowBase dataContext)
        {

            InitializeComponent();
            this.SetDataContext(dataContext);
        }
        public void SetDataContext(MainWindowBase dataContext)
        {
            if (dataContext != null)
            {
                dataContext.CancelClicked.ButtonClickedEvent += CS2ANGLARMainWindowViewModel_CancelClicked;
            }
            this.DataContext = dataContext;
        }

        private void CS2ANGLARMainWindowViewModel_CancelClicked(Object sender)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = MessageBox.Show("Do you want to exit ?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No;
        }
    }
}
