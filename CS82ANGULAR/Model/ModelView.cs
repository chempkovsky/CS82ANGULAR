using CS82ANGULAR.Helpers.UI;
using CS82ANGULAR.Model.Serializable;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CS82ANGULAR.Model
{
    public class ModelView : NotifyPropertyChangedViewModel
    {
        #region Fields
        protected string _ViewName = "";
        protected string _DomainViewName = "";
        protected string _RootEntityClassName = "";
        protected string _RootEntityFullClassName = "";
        protected string _RootEntityUniqueProjectName = "";
        protected string _DestinationProject = "";
        protected string _DefaultProjectNameSpace = "";
        protected string _DestinationFolder = "";
        protected bool _GenerateJSonAttribute;
        protected bool _UseOnlyRootPropsForSelect;
        protected string _RootEntityDbContextPropertyName;
        protected string _PageViewName;
        protected string _DomainPageViewName;
        protected string _PluralTitle = "";
        protected string _Title = "";
        protected string _BaseClass = "";

        #endregion

        public string BaseClass
        {
            get
            {
                return _BaseClass;
            }
            set
            {
                if (_BaseClass != value)
                {
                    _BaseClass = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PluralTitle
        {
            get
            {
                return _PluralTitle;
            }
            set
            {
                if (_PluralTitle != value)
                {
                    _PluralTitle = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ViewName
        {
            get
            {
                return _ViewName;
            }
            set
            {
                if (_ViewName != value)
                {
                    _ViewName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PageViewName
        {
            get
            {
                return _PageViewName;
            }
            set
            {
                if (_PageViewName != value)
                {
                    _PageViewName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DomainViewName
        {
            get
            {
                return _DomainViewName;
            }
            set
            {
                if (_DomainViewName != value)
                {
                    _DomainViewName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DomainPageViewName
        {
            get
            {
                return _DomainPageViewName;
            }
            set
            {
                if (_DomainPageViewName != value)
                {
                    _DomainPageViewName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityDbContextPropertyName
        {
            get
            {
                return _RootEntityDbContextPropertyName;
            }
            set
            {
                if (_RootEntityDbContextPropertyName != value)
                {
                    _RootEntityDbContextPropertyName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityClassName
        {
            get
            {
                return _RootEntityClassName;
            }
            set
            {
                if (_RootEntityClassName != value)
                {
                    _RootEntityClassName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityFullClassName
        {
            get
            {
                return _RootEntityFullClassName;
            }
            set
            {
                if (_RootEntityFullClassName != value)
                {
                    _RootEntityFullClassName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RootEntityUniqueProjectName
        {
            get
            {
                return _RootEntityUniqueProjectName;
            }
            set
            {
                if (_RootEntityUniqueProjectName != value)
                {
                    _RootEntityUniqueProjectName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViewProject
        {
            get
            {
                return _DestinationProject;
            }
            set
            {
                if (_DestinationProject != value)
                {
                    _DestinationProject = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViewDefaultProjectNameSpace
        {
            get
            {
                return _DefaultProjectNameSpace;
            }
            set
            {
                if (_DefaultProjectNameSpace != value)
                {
                    _DefaultProjectNameSpace = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViewFolder
        {
            get
            {
                return _DestinationFolder;
            }
            set
            {
                if (_DestinationFolder != value)
                {
                    _DestinationFolder = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool GenerateJSonAttribute
        {
            get
            {
                return _GenerateJSonAttribute;
            }
            set
            {
                if (_GenerateJSonAttribute != value)
                {
                    _GenerateJSonAttribute = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool UseOnlyRootPropsForSelect
        {
            get
            {
                return _UseOnlyRootPropsForSelect;
            }
            set
            {
                if (_UseOnlyRootPropsForSelect != value)
                {
                    _UseOnlyRootPropsForSelect = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ModelViewProperty> ScalarProperties { get; set; }
        public ObservableCollection<ModelViewForeignKey> ForeignKeys { get; set; }
        public ObservableCollection<ModelViewKeyProperty> PrimaryKeyProperties { get; set; }
        public ObservableCollection<ModelViewEntityProperty> AllProperties { get; set; }
        public ObservableCollection<ModelViewUIFormProperty> UIFormProperties { get; set; }
        public ObservableCollection<ModelViewUIListProperty> UIListProperties { get; set; }
        public ObservableCollection<ModelViewUniqueKey> UniqueKeys { get; set; }
        public ObservableCollection<GeneratedDto> GeneratedDtos { get; set; }
        public ObservableCollection<GeneratedService> GeneratedServices { get; set; }
        public ObservableCollection<ModelViewFun> RootEntityFunctions { get; set; }
    }

}
