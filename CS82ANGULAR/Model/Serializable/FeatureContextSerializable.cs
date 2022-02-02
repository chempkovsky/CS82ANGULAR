using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class FeatureContextSerializable
    {
        public List<FeatureSerializable> Features { get; set; }
    }
}
