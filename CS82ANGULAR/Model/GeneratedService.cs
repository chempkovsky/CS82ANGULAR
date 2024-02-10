using CS82ANGULAR.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public class GeneratedService : NotifyPropertyChangedViewModel
    {
        #region Fiedls
        protected string _SrvClassName;
        protected string _SrvType;
        protected string _SrvFolder;
        protected string _SrvDefaultProjectNameSpace;
        #endregion

        public string SrvClassName
        {
            get
            {
                return _SrvClassName;
            }
            set
            {
                if (_SrvClassName != value)
                {
                    _SrvClassName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SrvType
        {
            get
            {
                return _SrvType;
            }
            set
            {
                if (_SrvType != value)
                {
                    _SrvType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SrvFolder
        {
            get
            {
                return _SrvFolder;
            }
            set
            {
                if (_SrvFolder != value)
                {
                    _SrvFolder = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SrvDefaultProjectNameSpace
        {
            get
            {
                return _SrvDefaultProjectNameSpace;
            }
            set
            {
                if (_SrvDefaultProjectNameSpace != value)
                {
                    _SrvDefaultProjectNameSpace = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
