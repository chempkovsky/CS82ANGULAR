using EnvDTE;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.AnalyzeOnModelCreating
{
    public class FluentAPIForeignKey
    {
        public string EntityName { get; set; }
        public string EntityFullName { get; set; }
        public string NavigationName { get; set; }
        public string NavigationEntityName { get; set; }
        public string NavigationEntityFullName { get; set; }
        public string InverseNavigationName { get; set; }
        public string GenericForeignKeyClassName { get; set; }
        public List<FluentAPIProperty> PrincipalKeyProps { get; set; }
        public List<FluentAPIProperty> ForeignKeyProps { get; set; }
        public InfoSourceEnum ForeignKeySource { get; set; } = InfoSourceEnum.ByConvention;
        public InfoSourceEnum PrincipalKeySource { get; set; } = InfoSourceEnum.ByConvention;
        public InfoSourceEnum InverseNavigationSource { get; set; } = InfoSourceEnum.ByConvention;
        public int PrincipalKeySourceCount { get; set; } = 0;
        public int ForeignKeySourceCount { get; set; } = 0;
        public int SourceCount { get; set; } = 0;
        public CodeElement CodeElementEntityRef { get; set; }
        public CodeElement CodeElementNavigationRef { get; set; }
        public NavigationTypeEnum NavigationType { get; set; }
        public bool HasErrors { get; set; }
        public string ErrorsText { get; set; }
        public bool IsCascadeDelete { get; set; }
        public string DeleteBehavior { get; set; } = "DeleteBehavior.NoAction";
    }
}
