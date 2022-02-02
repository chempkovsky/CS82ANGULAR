using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.BatchProcess
{
    [Serializable]
    public class BatchSettings
    {
        public List<string> Description { get; set; }
        public List<BatchItem> BatchItems { get; set; }
    }
}
