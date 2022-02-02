using CS82ANGULAR.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;


namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlCreateView.xaml
    /// </summary>
    public partial class UserControlCreateView : UserControl
    {
        public UserControlCreateView(CreateViewViewModel dataContext)
        {
            InitializeComponent();
            SetDataContext(dataContext);
        }
        public void SetDataContext(Object dataContext)
        {
            if (dataContext is CreateViewViewModel)
            {
                (dataContext as CreateViewViewModel).MainTreeViewRootItem = this.MainTreeViewRootItem;
            }
            this.DataContext = dataContext;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is CreateViewViewModel)
            {
                (this.DataContext as CreateViewViewModel).SelectedTreeViewItem = e.NewValue;
            }
        }

    }
}
