using EnvDTE;

namespace CS82ANGULAR.Model
{
    public class SolutionCodeElement
    {
        public int Order { get; set; }
        public string CodeElementName { get; set; }
        public string CodeElementFullName { get; set; }
        public CodeElement CodeElementRef { get; set; }
    }
}
