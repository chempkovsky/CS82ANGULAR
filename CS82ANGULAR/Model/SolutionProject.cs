using EnvDTE;

namespace CS82ANGULAR.Model
{
    public class SolutionProject
    {
        public string ProjectName { get; set; }
        public string ProjectUniqueName { get; set; }
        public Project ProjectRef { get; set; }
    }
}
