using System;

namespace CS82ANGULAR.Model.Serializable.Angular
{
    [Serializable]
    public class FileModifierItemSerializable
    {
        public string[] Description { get; set; }
        public string MethodName { get; set; }
        public string[] InvocationParams { get; set; }
        public string Result { get; set; }
        public string StepDescription { get; set; }

    }
}
