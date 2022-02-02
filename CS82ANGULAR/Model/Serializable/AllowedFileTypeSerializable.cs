using System;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class AllowedFileTypeSerializable
    {
        public string FileType { get; set; }
        public string ModuleFileType { get; set; }
        public bool IsRouted { get; set; }
        public int DefaultMaxHeight { get; set; }
        public int DefaultFilterMaxHeight { get; set; }
        public int ExpandMaxHeight { get; set; }
        public int ExpandFilterMaxHeight { get; set; }
        public int DefaultCols { get; set; }
        public int DefaultRows { get; set; }
        public int ExpandCols { get; set; }
        public int ExpandRows { get; set; }
        public int ColLargeBreakpointMult { get; set; }
        public int ColSmallBreakpointMult { get; set; }
        public int RowLargeBreakpointMult { get; set; }
        public int RowSmallBreakpointMult { get; set; }
    }
}
