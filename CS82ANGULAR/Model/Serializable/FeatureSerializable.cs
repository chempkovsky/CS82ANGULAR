using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class FeatureSerializable
    {
        public string FeatureName { get; set; }
        public List<FeatureItemSerializable> FeatureItems { get; set; }
        public List<CommonStaffSerializable> CommonStaffs { get; set; }
    }
}
