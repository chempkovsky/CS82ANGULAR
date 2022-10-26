using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model.Serializable.Angular
{
    [Serializable]
    public class FileModifierSerializable
    {
        public List<FileModifierItemSerializable> FileModifierItems { get; set; }
    }
}
