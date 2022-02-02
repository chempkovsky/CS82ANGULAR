using CS82ANGULAR.ViewModel;
using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlT4SelectTemplate.xaml
    /// </summary>
    public partial class UserControlT4SelectTemplate : UserControl
    {
        public UserControlT4SelectTemplate(Selectt4TemplateViewModel dataContext)
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
