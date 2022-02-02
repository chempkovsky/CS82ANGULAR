using CS82ANGULAR.Model.Serializable;
using System.Collections.Generic;

namespace CS82ANGULAR.Helpers
{
    public static class FeatureViewModelHelper
    {
        public static FeatureSerializable FeatureContextSerializableGetShallowCopy(this FeatureSerializable srcFeatureSerializable)
        {
            if (srcFeatureSerializable == null) return null;
            FeatureSerializable result = new FeatureSerializable()
            {
                FeatureName = srcFeatureSerializable.FeatureName,
                FeatureItems = srcFeatureSerializable.FeatureItems
            };
            result.CommonStaffs = new List<CommonStaffSerializable>();
            if (srcFeatureSerializable.CommonStaffs != null)
            {
                srcFeatureSerializable.CommonStaffs.ForEach(c => result.CommonStaffs.Add(new CommonStaffSerializable()
                {
                    Extension = c.Extension,
                    FileType = c.FileType,
                    FileName = c.FileName,
                    FileProject = c.FileProject,
                    FileDefaultProjectNameSpace = c.FileDefaultProjectNameSpace,
                    FileFolder = c.FileFolder,
                    T4Template = c.T4Template
                }));
            }

            return result;
        }
    }

}
