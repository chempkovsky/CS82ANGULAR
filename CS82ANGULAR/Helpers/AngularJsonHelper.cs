using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using CS82ANGULAR.Model.Serializable.Angular;
using Jering.Javascript.NodeJS;
using System.Threading.Tasks;

namespace CS82ANGULAR.Helpers
{
    public static class AngularJsonHelper
    {
        private static AngularJson angularJson = new AngularJson();
        private static string BabelFolderPath = null;
        private static string PublicApiTsFileName = "public-api.ts";
        private static string WebpackConfigJs = "webpack.config.js";
        private static string PublicApiTsReader = "reader_of_public-api.ts.js";
        private static string WebpackConfigJsReader = "reader_of_webpack.config.js.js";
        private static string NodejsSriptsFolder = "NodejsSripts";
        private static string T4RootFolder = null;

        public static AngularJson GetAngularJson()
        {
            return angularJson;
        }
        public static string GetBabelFolderPath()
        {
            return BabelFolderPath;
        }
        public static void SetBabelFolderPath(string aPath)
        {
            BabelFolderPath = aPath;
        }

        public static string GetNodejsSriptsFolder()
        {
            return NodejsSriptsFolder; 
        }
        public static void SetNodejsSriptsFolder(string aPath)
        {
            NodejsSriptsFolder = aPath;
        }
        public static string GetT4RootFolder()
        {
            return T4RootFolder;
        }
        public static void SetT4RootFolder(string aPath)
        {
            T4RootFolder = aPath;
        }
        public static void ClearAngularJson(this AngularJson angularJson)
        {
            if (angularJson.Projects is null) angularJson.Projects = new List<AngularProject>();
            angularJson.Projects.Clear();
            angularJson.NewProjectRoot = null;
            angularJson.AngularJsonPath = null;
        }
        public static void ParseAngularJson(this AngularJson angularJson, string jsonString)
        {
            angularJson.ClearAngularJson();
            if (string.IsNullOrEmpty(jsonString)) return;
            JObject jObject = JsonConvert.DeserializeObject<JObject>(jsonString);
            if (jObject is null) return;
            angularJson.NewProjectRoot = (string)jObject["newProjectRoot"];
            if (!string.IsNullOrEmpty(angularJson.NewProjectRoot))
            {
                if (!(jObject[angularJson.NewProjectRoot] is null))
                {
                    foreach (var aProject in jObject[angularJson.NewProjectRoot].Values())
                    {
                        AngularProject angularProject = new AngularProject();
                        angularProject.ProjectPath = aProject.Path;
                        if (!string.IsNullOrEmpty(angularProject.ProjectPath))
                        {
                            angularProject.ProjectName = angularProject.ProjectPath.Replace(angularJson.NewProjectRoot + ".", "");
                        }
                        angularProject.ProjectType = (string)aProject["projectType"];
                        angularProject.ProjectRoot = (string)aProject["root"];
                        angularProject.SourceRoot = (string)aProject["sourceRoot"];
                        angularProject.ProjectPrefix = (string)aProject["prefix"];
                        angularJson.Projects.Add(angularProject);
                    }
                }
            }
        }
        public static void ReadAngularJson(this AngularJson angularJson, string currPath)
        {
            string tmpPath = currPath;
            while (!string.IsNullOrEmpty(tmpPath))
            {
                string angularJsonPath = Path.Combine(tmpPath, "Angular.json");
                if (File.Exists(angularJsonPath))
                {
                    string jsonString = File.ReadAllText(angularJsonPath);
                    try
                    {
                        angularJson.ParseAngularJson(jsonString);
                        angularJson.AngularJsonPath = Path.GetDirectoryName(angularJsonPath);
                        foreach(var p in angularJson.Projects)
                        {
                            if (string.IsNullOrEmpty(p.ProjectRoot)) p.ProjectRoot = "";
                            if (string.IsNullOrEmpty(p.SourceRoot)) p.SourceRoot = "";
                            p.AbsoluteProjectRoot = Path.Combine(angularJson.AngularJsonPath, p.ProjectRoot.Replace("/","\\"));
                            p.AbsoluteSourceRoot = Path.Combine(angularJson.AngularJsonPath, p.SourceRoot.Replace("/", "\\"));
                        }
                    } 
                    catch
                    {
                        angularJson.ClearAngularJson();
                    }
                    return;
                }
                DirectoryInfo dirInfo = Directory.GetParent(tmpPath);
                if (dirInfo is null)
                {
                    tmpPath = null;
                }
                else
                {
                    tmpPath = dirInfo.FullName;
                }
            }
            angularJson.ClearAngularJson();
        }
        public static async Task ReadPublicApiTsAndWebpackConfigJsAsync(this AngularJson angularJson)
        {
            if (angularJson is null) return;
            if (angularJson.Projects is null) return;
            if(angularJson.Projects.Count < 1) return;
            foreach (AngularProject prj in angularJson.Projects)
            {
                prj.WebpackConfigJson = null;
                prj.PublicApiJson = null;
            }
            string WebpackConfigJsReaderCode = File.ReadAllText(Path.Combine(T4RootFolder, NodejsSriptsFolder, WebpackConfigJsReader));
            string PublicApiTsReaderCode = File.ReadAllText(Path.Combine(T4RootFolder, NodejsSriptsFolder, PublicApiTsReader));
            StaticNodeJSService.Configure<NodeJSProcessOptions>(options => options.ProjectPath = BabelFolderPath);
            
            foreach (AngularProject prj in angularJson.Projects)
            {
                if (prj.ProjectType == "application")
                {
                    string wpcjs = Path.Combine(prj.AbsoluteProjectRoot, WebpackConfigJs);
                    if (File.Exists(wpcjs))
                    {
                        string wpcjsText = File.ReadAllText(wpcjs);
                        string wpcjsResult = await StaticNodeJSService.InvokeFromStringAsync<string>(WebpackConfigJsReaderCode, args: new object[] { wpcjsText });
                        prj.WebpackConfigJson = JsonConvert.DeserializeObject<AngularWebpackConfigJson>(wpcjsResult);
                    } else
                    {
                        prj.WebpackConfigJson = null;
                    }
                }
                else if (prj.ProjectType == "library")
                {
                    string pats = Path.Combine(prj.AbsoluteSourceRoot, PublicApiTsFileName);
                    if (File.Exists(pats))
                    {
                        string patsText = File.ReadAllText(pats);
                        string patsResult = await StaticNodeJSService.InvokeFromStringAsync<string>(PublicApiTsReaderCode, args: new object[] { patsText });
                        prj.PublicApiJson = JsonConvert.DeserializeObject<AngularPublicApiJson>(patsResult);
                    } else
                    {
                        prj.PublicApiJson = null;
                    }
                }
            }
        }
    }
}
