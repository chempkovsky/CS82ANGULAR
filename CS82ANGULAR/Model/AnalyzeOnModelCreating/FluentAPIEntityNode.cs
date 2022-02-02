using System.Collections.Generic;

namespace CS82ANGULAR.Model.AnalyzeOnModelCreating
{
    public class FluentAPIEntityNode
    {
        public FluentAPIEntityNode()
        {

        }
        public string EntityName { get; set; }
        public string MethodBodyString { get; set; }
        public List<FluentAPIMethodNode> Methods { get; set; }
    }
}
