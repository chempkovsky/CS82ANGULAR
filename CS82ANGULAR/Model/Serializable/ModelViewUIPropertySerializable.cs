using System;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewUIPropertySerializable
    {
        public string OriginalPropertyName { get; set; }
        public string ForeignKeyName { get; set; }
        public string ForeignKeyNameChain { get; set; }
        public string ViewPropertyName { get; set; }
        public string JsonPropertyName { get; set; }
        public bool IsShownInView { get; set; }
        public bool IsNewLineAfter { get; set; }
    }
}
