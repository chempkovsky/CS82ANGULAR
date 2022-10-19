using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model
{
    [Serializable]
    public class AngularWebpackConfigJson
    {
        public List<AngularWebpackConfigExposeItem> exposeItems { get; set; }
    }
}
