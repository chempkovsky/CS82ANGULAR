using System.Collections.ObjectModel;

namespace CS82ANGULAR.Model
{
    public class ModelViewEntityProperty : ModelViewKeyProperty
    {
        protected ObservableCollection<ModelViewAttribute> _Attributes;
        protected ObservableCollection<ModelViewFAPIAttribute> _FAPIAttributes;
        public ObservableCollection<ModelViewAttribute> Attributes
        {
            get
            {
                return _Attributes;
            }
            set
            {
                if (_Attributes != value)
                {
                    _Attributes = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ModelViewFAPIAttribute> FAPIAttributes
        {
            get
            {
                return _FAPIAttributes;
            }
            set
            {
                if (_FAPIAttributes != value)
                {
                    _FAPIAttributes = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
