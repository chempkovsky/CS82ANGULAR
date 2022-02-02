using CS82ANGULAR.ViewModel;
using System;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlT4Editor.xaml
    /// </summary>
    public partial class UserControlT4Editor : UserControl
    {
        public UserControlT4Editor(T4EditorViewModel dataContext)
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
