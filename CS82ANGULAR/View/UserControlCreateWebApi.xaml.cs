using CS82ANGULAR.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for UserControlCreateWebApi.xaml
    /// </summary>
    public partial class UserControlCreateWebApi : UserControl
    {
        public UserControlCreateWebApi(CreateWebApiViewModel dataContext)
        {
            InitializeComponent();
            SetDataContext(dataContext);
        }
        public void SetDataContext(Object dataContext)
        {
            if (dataContext is CreateWebApiViewModel)
            {
                (dataContext as CreateWebApiViewModel).MainTreeViewRootItem = this.MainTreeViewRootItem;
            }
            this.DataContext = dataContext;
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is CreateWebApiViewModel)
            {
                (this.DataContext as CreateWebApiViewModel).SelectedTreeViewItem = e.NewValue;
            }
        }

    }
}
