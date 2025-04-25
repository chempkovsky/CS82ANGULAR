using CS82ANGULAR.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public class ModelViewFunParam : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected int _ParamOrder;
        protected string _OriginalParamName;
        protected string _TypeFullName;
        protected string _UnderlyingTypeName;
        protected bool _IsNullable;
        protected bool _IsInParam;
        protected bool _IsOutParam;
        #endregion

        public int ParamOrder
        {
            get
            {
                return _ParamOrder;
            }
            set
            {
                if (_ParamOrder != value)
                {
                    _ParamOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OriginalParamName
        {
            get
            {
                return _OriginalParamName;
            }
            set
            {
                if (_OriginalParamName != value)
                {
                    _OriginalParamName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypeFullName
        {
            get
            {
                return _TypeFullName;
            }
            set
            {
                if (_TypeFullName != value)
                {
                    _TypeFullName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UnderlyingTypeName
        {
            get
            {
                return _UnderlyingTypeName;
            }
            set
            {
                if (_UnderlyingTypeName != value)
                {
                    _UnderlyingTypeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNullable
        {
            get
            {
                return _IsNullable;
            }
            set
            {
                if (_IsNullable != value)
                {
                    _IsNullable = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsInParam
        {
            get
            {
                return _IsInParam;
            }
            set
            {
                if (_IsInParam != value)
                {
                    _IsInParam = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsOutParam
        {
            get
            {
                return _IsOutParam;
            }
            set
            {
                if (_IsOutParam != value)
                {
                    _IsOutParam = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
