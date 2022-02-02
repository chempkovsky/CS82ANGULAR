using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewFAPIAttributeSerializable
    {
        public string AttrName { get; set; }
        public List<ModelViewFAPIAttributePropertySerializable> VaueProperties { get; set; }
    }
}
