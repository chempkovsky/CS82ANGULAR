using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlInvitation.xaml
    /// </summary>
    public partial class UserControlInvitation : UserControl
    {
        public UserControlInvitation(Object dataContext)
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
