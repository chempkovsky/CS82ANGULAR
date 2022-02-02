using System;

namespace CS82ANGULAR.Model.BatchProcess
{
    [Serializable]
    public class BatchItem
    {
        public string DestinationFolder { get; set; }
        public string GeneratorType { get; set; }
        public string GeneratorSript { get; set; }
        public string ViewModel { get; set; }
    }
}
