using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public class AngularProject
    {
        public string ProjectPath { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string ProjectRoot { get; set; }
        public string SourceRoot { get; set; }
        public string ProjectPrefix { get; set; }
        public string AbsoluteSourceRoot { get; set; }
    }
}
