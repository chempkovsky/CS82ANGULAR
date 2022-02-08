using CS82ANGULAR.ViewModel;
using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlCreateUniqueKey.xaml
    /// </summary>
    public partial class UserControlCreateUniqueKey : UserControl
    {
        public UserControlCreateUniqueKey(CreateUniqueKeyViewModel dataContext)
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
