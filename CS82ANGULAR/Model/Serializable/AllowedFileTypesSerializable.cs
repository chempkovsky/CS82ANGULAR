using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class AllowedFileTypesSerializable
    {
        public List<AllowedFileTypeSerializable> Items { get; set; }
    }
}
