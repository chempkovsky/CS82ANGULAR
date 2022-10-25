using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable.Angular
{
    [Serializable]
    public class AngularJson
    {
        public string NewProjectRoot { get; set; }
        public IList<AngularProject> Projects { get; set; }

        public string AngularJsonPath { get; set; }
    }
}
