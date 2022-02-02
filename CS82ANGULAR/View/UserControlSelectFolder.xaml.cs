using CS82ANGULAR.ViewModel;
using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlSelectFolder.xaml
    /// </summary>
    public partial class UserControlSelectFolder : UserControl
    {
        public UserControlSelectFolder(SelectFolderViewModel dataContext)
        {
            InitializeComponent();
            SetDataContext(dataContext);
        }
        public void SetDataContext(Object dataContext)
        {
            this.DataContext = dataContext;
        }
    }
}
