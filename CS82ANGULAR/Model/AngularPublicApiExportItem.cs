using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model
{
    [Serializable]
    public class AngularPublicApiExportItem
    {
        public string exportType { get; set; }
        public string exportSubtype { get; set; }
        public string exportNamespace { get; set; }
        public string exportSource { get; set; }

        public List<AngularPublicApiExportSpecifier> exportSpecifiers { get; set; }
    }
}
