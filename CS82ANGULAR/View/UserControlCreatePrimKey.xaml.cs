using CS82ANGULAR.ViewModel;
using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlCreatePrimKey.xaml
    /// </summary>
    public partial class UserControlCreatePrimKey : UserControl
    {
        public UserControlCreatePrimKey(CreatePrimaryKeyViewModel dataContext)
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
