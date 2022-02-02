using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewPropertyOfVwSerializable : ModelViewPropertySerializable
    {
        public bool IsUsedByfilter { get; set; }
        public bool IsUsedBySorting { get; set; }
        public List<ModelViewAttributeSerializable> Attributes { get; set; }
        public List<ModelViewFAPIAttributeSerializable> FAPIAttributes { get; set; }
    }
}
