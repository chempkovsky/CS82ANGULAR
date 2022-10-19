using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    [Serializable]
    public class AngularPublicApiExportSpecifier
    {
        public string localNm { get; set; }
        public string exportedNm { get; set; }
    }
}
