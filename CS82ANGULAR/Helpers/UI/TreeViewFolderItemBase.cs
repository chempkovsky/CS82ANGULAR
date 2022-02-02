using System.Collections;

namespace CS82ANGULAR.Helpers.UI
{
    public class TreeViewFolderItemBase : NotifyPropertyChangedViewModel
    {
        public string FolderName { get; set; }
        public IEnumerable FolderItems { get; set; }
    }
}
