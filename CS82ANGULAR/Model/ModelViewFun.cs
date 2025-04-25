using CS82ANGULAR.Helpers.UI;
using CS82ANGULAR.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public class ModelViewFun : NotifyPropertyChangedViewModel
    {
        #region Fiedls
        protected string _FunName;
        protected bool _IsSub;
        protected bool _IsConstructor;
        protected string _RetTypeFullName;
        protected string _RetUnderlyingTypeName;
        protected List<ModelViewFunParam> _FunParams;
        #endregion

        public string FunName
        {
            get
            {
                return _FunName;
            }
            set
            {
                if (_FunName != value)
                {
                    _FunName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSub
        {
            get
            {
                return _IsSub;
            }
            set
            {
                if (_IsSub != value)
                {
                    _IsSub = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsConstructor
        {
            get
            {
                return _IsConstructor;
            }
            set
            {
                if (_IsConstructor != value)
                {
                    _IsConstructor = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RetTypeFullName
        {
            get
            {
                return _RetTypeFullName;
            }
            set
            {
                if (_RetTypeFullName != value)
                {
                    _RetTypeFullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RetUnderlyingTypeName
        {
            get
            {
                return _RetUnderlyingTypeName;
            }
            set
            {
                if (_RetUnderlyingTypeName != value)
                {
                    _RetUnderlyingTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<ModelViewFunParam> FunParams
        {
            get
            {
                return _FunParams;
            }
            set
            {
                if (_FunParams != value)
                {
                    _FunParams = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
