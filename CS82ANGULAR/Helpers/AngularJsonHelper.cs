using CS82ANGULAR.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace CS82ANGULAR.Helpers
{
    public static class AngularJsonHelper
    {
        private static AngularJson angularJson = new AngularJson();
        public static AngularJson GetAngularJson()
        {
            return angularJson;
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
                            p.AbsoluteSourceRoot = Path.Combine(angularJson.AngularJsonPath, p.SourceRoot.Replace("/","\\"));
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

    }
}
