using System;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewUIFormPropertySerializable : ModelViewUIPropertySerializable
    {
        public InputTypeEnum InputTypeWhenAdd { get; set; }
        public InputTypeEnum InputTypeWhenUpdate { get; set; }
        public InputTypeEnum InputTypeWhenDelete { get; set; }
        public string ForeifKeyViewNameForAdd { get; set; }
        public string ForeifKeyViewNameForUpd { get; set; }
        public string ForeifKeyViewNameForDel { get; set; }
    }
}
