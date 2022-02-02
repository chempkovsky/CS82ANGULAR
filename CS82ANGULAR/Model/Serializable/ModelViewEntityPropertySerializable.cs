using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewEntityPropertySerializable : ModelViewKeyPropertySerializable
    {
        public List<ModelViewAttributeSerializable> Attributes { get; set; }
        public List<ModelViewFAPIAttributeSerializable> FAPIAttributes { get; set; }
    }
}
