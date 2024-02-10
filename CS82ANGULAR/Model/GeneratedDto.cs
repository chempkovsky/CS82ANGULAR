using CS82ANGULAR.Helpers.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public class GeneratedDto : NotifyPropertyChangedViewModel
    {
        #region Fiedls
        protected string _ViewType;
        protected string _ViewProject;
        protected string _ViewDefaultProjectNameSpace;
        protected string _ViewClassName;
        public string _PageViewClassName;
        #endregion

        public string ViewClassName
        {
            get
            {
                return _ViewClassName;
            }
            set
            {
                if (_ViewClassName != value)
                {
                    _ViewClassName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PageViewClassName
        {
            get
            {
                return _PageViewClassName;
            }
            set
            {
                if (_PageViewClassName != value)
                {
                    _PageViewClassName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ViewType
        {
            get
            {
                return _ViewType;
            }
            set
            {
                if (_ViewType != value)
                {
                    _ViewType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ViewProject
        {
            get
            {
                return _ViewProject;
            }
            set
            {
                if (_ViewProject != value)
                {
                    _ViewProject = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ViewDefaultProjectNameSpace
        {
            get
            {
                return _ViewDefaultProjectNameSpace;
            }
            set
            {
                if (_ViewDefaultProjectNameSpace != value)
                {
                    _ViewDefaultProjectNameSpace = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
