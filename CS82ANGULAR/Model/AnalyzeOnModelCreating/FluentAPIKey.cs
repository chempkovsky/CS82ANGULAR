using System.Collections.Generic;

namespace CS82ANGULAR.Model.AnalyzeOnModelCreating
{
    public class FluentAPIKey
    {
        public string KeyName { get; set; }
        public bool IsPrimary { get; set; }
        public int SourceCount { get; set; } = 0;
        public InfoSourceEnum KeySource { get; set; } = InfoSourceEnum.ByConvention;
        public string KeySourceDisplay { get { return KeySource.ToString("g"); } }
        public List<FluentAPIProperty> KeyProperties { get; set; }
    }
}
