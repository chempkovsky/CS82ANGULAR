using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewFunSerializable
    {
        public string FunName { get; set; }
        public bool IsSub { get; set; }
        public bool IsConstructor { get; set; }
        public string RetTypeFullName { get; set; }
        public string RetUnderlyingTypeName { get; set; }
        public List<ModelViewFunParamSerializable> FunParams { get; set; }
    }
}
