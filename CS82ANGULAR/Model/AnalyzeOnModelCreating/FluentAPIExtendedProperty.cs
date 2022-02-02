
namespace CS82ANGULAR.Model.AnalyzeOnModelCreating
{
    public class FluentAPIExtendedProperty : FluentAPIProperty
    {
        public bool IsNullable { get; set; }
        public bool IsRequired { get; set; }
        public string UnderlyingTypeName { get; set; }
        public string TypeFullName { get; set; }
    }
}
