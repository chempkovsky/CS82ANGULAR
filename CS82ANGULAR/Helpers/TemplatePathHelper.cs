using System.IO;

namespace CS82ANGULAR.Helpers
{
    public static class TemplatePathHelper
    {
        public static string GetTemplatePath()
        {
            string str = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return Path.Combine(Path.GetDirectoryName(str), "Templates");
        }
    }
}
