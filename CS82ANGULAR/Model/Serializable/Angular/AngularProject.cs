
using System;

namespace CS82ANGULAR.Model.Serializable.Angular
{
    [Serializable]
    public class AngularProject
    {
        public string ProjectPath { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string ProjectRoot { get; set; }
        public string SourceRoot { get; set; }
        public string ProjectPrefix { get; set; }
        public string AbsoluteProjectRoot { get; set; }
        public string AbsoluteSourceRoot { get; set; }
        public AngularPublicApiJson PublicApiJson { get; set; }
        public AngularWebpackConfigJson WebpackConfigJson { get; set; }
    }
}
