using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public class AngularJson
    {
        public string NewProjectRoot { get; set; }
        public IList<AngularProject> Projects { get; set; }

        public string AngularJsonPath { get; set; }
    }
}
