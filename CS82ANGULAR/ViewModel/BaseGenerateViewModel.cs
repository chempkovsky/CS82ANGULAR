using CS82ANGULAR.Helpers.UI;
using CS82ANGULAR.Model.Serializable;

namespace CS82ANGULAR.ViewModel
{
    #pragma warning disable VSTHRD010
    public class BaseGenerateViewModel : IsReadyViewModel
    {
        #region Fields
        protected string _GenText;
        protected string _GenerateText;
        protected string _GenerateError;
        protected string _FileExtension;
        #endregion

        public string GenText
        {
            get
            {
                return _GenText;
            }
            set
            {
                if (_GenText == value) return;
                _GenText = value;
                OnPropertyChanged();
            }
        }
        public string GenerateText
        {
            get
            {
                return _GenerateText;
            }
            set
            {
                if (_GenerateText == value) return;
                _GenerateText = value;
                OnPropertyChanged();
            }
        }
        public string GenerateError
        {
            get
            {
                return _GenerateError;
            }
            set
            {
                if (_GenerateError == value) return;
                _GenerateError = value;
                OnPropertyChanged();
            }
        }
        public string FileExtension
        {
            get
            {
                return _FileExtension;
            }
            set
            {
                if (_FileExtension == value) return;
                _FileExtension = value;
                OnPropertyChanged();
            }
        }

        public ModelViewSerializable GeneratedModelView { get; set; } = null;
        public FeatureSerializable GeneratedFeature { get; set; } = null;

        public BaseGenerateViewModel() : base()
        {

        }

    }
}
