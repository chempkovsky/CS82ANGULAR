using CS82ANGULAR.ViewModel;
using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlSelectFeatureFolder.xaml
    /// </summary>
    public partial class UserControlSelectFeatureFolder : UserControl
    {
        public UserControlSelectFeatureFolder(SelectFeatureFolderViewModel dataContext)
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
