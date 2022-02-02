using CS82ANGULAR.Helpers.UI;
using System.Collections.ObjectModel;

namespace CS82ANGULAR.Model
{
    public class ModelViewFAPIAttribute : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _AttrName = "";
        protected ObservableCollection<ModelViewFAPIAttributeProperty> _VaueProperties;
        #endregion
        public string AttrName
        {
            get
            {
                return _AttrName;
            }
            set
            {
                if (_AttrName != value)
                {
                    _AttrName = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ModelViewFAPIAttributeProperty> VaueProperties
        {
            get
            {
                return _VaueProperties;
            }
            set
            {
                if (_VaueProperties != value)
                {
                    _VaueProperties = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
