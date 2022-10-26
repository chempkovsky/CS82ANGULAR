using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.Model.Serializable.Angular;
using Jering.Javascript.NodeJS;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Helpers
{
    public static class ExportFileModifier
    {
        private static string BabelFolderPath = null;
        private static string T4RootFolder = null;
        private static string NodejsSriptsFolder = "NodejsSripts";
        private static string PublicApiTsUpdater = "updater_of_public-api.ts.js";
        private static string WebpackConfigJsUpdater = "updater_of_webpack.config.js.js";
        private static string WebpackConfigJsUpdaterCode = null;
        private static string PublicApiTsUpdaterCode = null;

        public static string GetBabelFolderPathEFM()
        {
            return BabelFolderPath;
        }
        public static void SetBabelFolderPathEFM(string aPath)
        {
            BabelFolderPath = aPath;
        }
        public static string GetT4RootFolderEFM()
        {
            return T4RootFolder;
        }
        public static void SetT4RootFolderEFM(string aPath)
        {
            T4RootFolder = aPath;
        }
        public static AngularProject GetAngularProjectByRefItemEFM(AngularJson anglJson, CommonStaffSerializable refItem)
        {
            if ((refItem is null) || (anglJson is null)) return null;
            if (anglJson.Projects is null) return null;
            string aPath = "";
            if (!string.IsNullOrEmpty(refItem.FileProject)) aPath = refItem.FileProject;
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                if (!string.IsNullOrEmpty(aPath)) aPath += "\\";
                aPath += refItem.FileFolder;
            }
            foreach (AngularProject prj in anglJson.Projects)
            {
                if (aPath.StartsWith(prj.AbsoluteSourceRoot + "\\" + prj.ProjectPrefix , StringComparison.OrdinalIgnoreCase))
                {
                    return prj;
                }
            }
            return null;
        }
        public static StringBuilder FormatOutputEFM(StringBuilder sb, FileModifierItemSerializable stepItm)
        {
            StringBuilder result = null;
            if (sb == null) result = new StringBuilder(); else result = sb;
            result.AppendLine("");
            if (stepItm != null)
            {
                if (stepItm.Description != null)
                {
                    foreach (string s in stepItm.Description) result.AppendLine(s);
                }
                if (stepItm.StepDescription != null) result.AppendLine(stepItm.StepDescription);
                result.AppendLine("Result:");
                if (string.IsNullOrEmpty(stepItm.Result)) result.AppendLine("Result is not defined"); else result.AppendLine(stepItm.Result);
            }
            return result;
        }
        public static async Task<string> UpdateExportFileEFMAsync(AngularJson angularJson, ModelViewSerializable model, FileModifierItemSerializable stepItm)
        {
            if (model is null) return "Error: ModelViewSerializable is not set";
            if (model.CommonStaffs is null) return "Error: ModelViewSerializable.CommonStaffs is not set";
            if (stepItm is null) return "Error: FileModifierItem is not set";
            if (stepItm.InvocationParams is null) return "Error: FileModifierItem.InvocationParams is not set";
            if (stepItm.InvocationParams.Count() < 1) return "Error: FileModifierItem.InvocationParams is not set";
            if (angularJson is null) return "Error: AngularJson is not set";
            if (angularJson.Projects is null) return "Error: AngularJson is not set";
            if (angularJson.Projects.Count < 1) return "Error: AngularJson is not set";
            string result = "";
            foreach (string fileType in stepItm.InvocationParams)
            {
                if (string.IsNullOrEmpty(fileType))
                {
                    result += "FileType is not defined.\n";
                    continue;
                }
                CommonStaffSerializable refItem =
                    model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
                if (refItem is null)
                {
                    result += "CommonStaffs does not contain Item with fileType = '"+ fileType + "'.\n";
                    continue;
                }
                AngularProject angPrj = GetAngularProjectByRefItemEFM(angularJson, refItem);
                if (angPrj is null)
                {
                    result += "AngularProject does not contain Project Item with fileType = '" + fileType + "'.\n";
                    continue;
                }
                if (angPrj.ProjectType == "library")
                {
                    string patsNm = angPrj.AbsoluteSourceRoot + "\\" + "public-api.ts";
                    if (File.Exists(patsNm))
                    {
                        string patsText = File.ReadAllText(patsNm);
                        
                        string patsPath = Path.Combine(refItem.FileProject, refItem.FileFolder, refItem.FileName).Replace(angPrj.AbsoluteSourceRoot, "");
                        if (!patsPath.StartsWith("\\")) patsPath = "\\" + patsPath;
                        patsPath = ("." + patsPath).Replace("\\", "/");

                        patsText = await StaticNodeJSService.InvokeFromStringAsync<string>(PublicApiTsUpdaterCode, args: new object[] { patsText, patsPath });
                        File.WriteAllText(patsNm, patsText);
                        result += "The file '" + patsNm + "' has been updated.\n";
                    }
                    else
                    {
                        result += "The file '" + patsNm + "' does not exist.\n";
                    }
                }
                else if (angPrj.ProjectType == "application")
                {
                    string wpcjsNm = angPrj.AbsoluteProjectRoot + "\\" + "webpack.config.js";
                    if (File.Exists(wpcjsNm))
                    {
                        string wpcjsText = File.ReadAllText(wpcjsNm);
                        string wpcjsPath = Path.Combine(refItem.FileProject, refItem.FileFolder, refItem.FileName).Replace(angularJson.AngularJsonPath, "");
                        if (!wpcjsPath.StartsWith("\\")) wpcjsPath = "\\" + wpcjsPath;
                        wpcjsPath = ("." + wpcjsPath).Replace("\\", "/");

                        wpcjsText = await StaticNodeJSService.InvokeFromStringAsync<string>(WebpackConfigJsUpdaterCode, args: new object[] { wpcjsText, wpcjsPath });
                        File.WriteAllText(wpcjsNm, wpcjsText);
                        result += "The file '" + wpcjsNm + "' has been updated.\n";
                    } 
                    else
                    {
                        result += "The file '" + wpcjsNm + "' does not exist.\n";
                    }
                }
            }
            return result;
        }
        public static async Task<string> ExecuteJsonScriptEFMAsync(AngularJson angularJson, ModelViewSerializable model, string jsonScript)
        {
            StringBuilder sb;
            if (string.IsNullOrEmpty(jsonScript)) sb = new StringBuilder(); else sb = new StringBuilder(jsonScript);
            sb.AppendLine("==============================================================================");
            sb.AppendLine("Deserialize json Script");
            FileModifierSerializable steps = null;
            try
            {
                steps = JsonConvert.DeserializeObject<FileModifierSerializable>(jsonScript);
                sb.AppendLine("Deserialize json Script: Done");
                if (steps == null)
                {
                    sb.AppendLine("Result:");
                    sb.AppendLine(" The list of FileModifierSerializable steps is epmty.");
                    return sb.ToString();
                }
                if (steps.FileModifierItems == null)
                {
                    sb.AppendLine("Result:");
                    sb.AppendLine(" The list of FileModifierSerializable steps is epmty.");
                    return sb.ToString();
                }
                if (steps.FileModifierItems.Count < 1)
                {
                    sb.AppendLine("Result:");
                    sb.AppendLine(" The list of FileModifierSerializable steps is epmty.");
                    return sb.ToString();
                }
            }
            catch (Exception e)
            {
                sb.AppendLine("Error:");
                sb.AppendLine(e.Message);
                return sb.ToString();
            }
            
            try
            {
                sb.AppendLine("Reading updaters code");
                WebpackConfigJsUpdaterCode = File.ReadAllText(Path.Combine(T4RootFolder, NodejsSriptsFolder, WebpackConfigJsUpdater));
                PublicApiTsUpdaterCode = File.ReadAllText(Path.Combine(T4RootFolder, NodejsSriptsFolder, PublicApiTsUpdater));
                sb.AppendLine("Reading updaters code: Done");
                sb.AppendLine("Setting Babel Folder Path");
                StaticNodeJSService.Configure<NodeJSProcessOptions>(options => options.ProjectPath = BabelFolderPath);
                sb.AppendLine("Setting Babel Folder Path: Done");
            }
            catch (Exception e)
            {
                sb.AppendLine("Error:");
                sb.AppendLine(e.Message);
                return sb.ToString();
            }

            foreach (FileModifierItemSerializable stepItm in steps.FileModifierItems)
            {
                switch (stepItm.MethodName)
                {
                    case "UpdateExport":
                        try
                        {
                            stepItm.Result = await UpdateExportFileEFMAsync(angularJson, model, stepItm);
                        }
                        catch (Exception e)
                        {
                            stepItm.Result = "Exception thrown: " + e.Message;
                        }
                        break;


                    default:
                        stepItm.Result = "Error: Unknown MethodName";
                        break;
                }
                sb = FormatOutputEFM(sb, stepItm);
            }
            return sb.ToString();
        }
    }

}
