using CS82ANGULAR.ViewModel;
using System.Windows;

namespace CS82ANGULAR.View
{
    /// <summary>
    /// Interaction logic for WindowModifyFeature.xaml
    /// </summary>
    public partial class WindowModifyFeature : Window
    {
        public WindowModifyFeature(ModifyFeatureViewModel context)
        {
            InitializeComponent();
            SetDataContext(context);
        }
        public void SetDataContext(ModifyFeatureViewModel dataContext)
        {
            if (dataContext != null)
            {
                dataContext.wnd = this;
            }
            this.DataContext = dataContext;
        }
    }
}
