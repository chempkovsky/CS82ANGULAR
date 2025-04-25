using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewFunParamSerializable
    {
        public int ParamOrder { get; set; }
        public string OriginalParamName { get; set; }
        public string TypeFullName { get; set; }
        public string UnderlyingTypeName { get; set; }
        public bool IsNullable { get; set; }
        public bool IsInParam { get; set; }
        public bool IsOutParam { get; set; }
    }
}
