﻿using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.Model.Serializable.Angular;
using CS82ANGULAR.TemplateProcessingHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace CS82ANGULAR.Model
{

    public class ContextLevelTemplateCode
    {
        string GetExtForLkUpClassName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUp";
            return result;
        }
        string GetAbpEtoClassName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.RootEntityClassName;
            result = result + "Eto";
            return result;
        }
        string GetAbpEtoFullClassName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.RootEntityFullClassName;
            result = result + "Eto";
            return result;
        }
        string GetExtForLkUpConfName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUpConf";
            return result;
        }

        AngularProject GetAngularProjectByRefItem(AngularJson anglJson, CommonStaffSerializable refItem)
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
                if (aPath.StartsWith(prj.AbsoluteSourceRoot + "\\" + prj.ProjectPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return prj;
                }
            }
            return null;
        }
        string GetNameByAngularJson(string srcName, AngularJson anglJson, CommonStaffSerializable refItem, CommonStaffSerializable curItem)
        {
            if ((anglJson != null) && (refItem != null) && (curItem != null) && (!string.IsNullOrEmpty(srcName)))
            {
                AngularProject refAngularProject = GetAngularProjectByRefItem(anglJson, refItem);
                AngularProject curAngularProject = GetAngularProjectByRefItem(anglJson, curItem);
                if ((refAngularProject != null) && (curAngularProject != null))
                {
                    if (refAngularProject != curAngularProject)
                    {
                        if (refAngularProject.ProjectType == "library")
                        {
                            // string libFl = (".\\" + refItem.FileFolder + "\\" + refItem.FileName).Replace("\\", "/");
                            string libFl = Path.Combine(refItem.FileProject, refItem.FileFolder, refItem.FileName).Replace(refAngularProject.AbsoluteSourceRoot, "");
                            if (!libFl.StartsWith("\\")) libFl = "\\" + libFl;
                            libFl = ("." + libFl).Replace("\\", "/");
                            if (refAngularProject.PublicApiJson != null)
                            {
                                if (refAngularProject.PublicApiJson.exportItems != null)
                                {
                                    AngularPublicApiExportItem exportItem =
                                        refAngularProject.PublicApiJson.exportItems
                                        .Where(expItm => expItm.exportType == "ExportAllDeclaration" && libFl.Equals(expItm.exportSource, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    if (exportItem != null)
                                    {
                                        return srcName;
                                    }
                                    exportItem = refAngularProject.PublicApiJson.exportItems
                                                    .Where(expItm => expItm.exportType == "ExportDefaultDeclaration" && libFl.Equals(expItm.exportSource, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    if (exportItem != null)
                                    {
                                        return srcName;
                                    }
                                    exportItem =
                                        refAngularProject.PublicApiJson.exportItems
                                        .Where(expItm => expItm.exportType == "ExportNamedDeclaration" && expItm.exportSubtype == "ExportSpecifier" && libFl.Equals(expItm.exportSource, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    if (exportItem != null)
                                    {
                                        if (exportItem.exportSpecifiers != null)
                                        {
                                            AngularPublicApiExportSpecifier exportSpecifier =
                                                exportItem.exportSpecifiers.Where(expSpec => expSpec.localNm == srcName).FirstOrDefault();
                                            if (exportSpecifier != null)
                                            {
                                                return exportSpecifier.exportedNm;
                                            }
                                        }
                                    }
                                    // 
                                    // We ignore ExportNamespaceSpecifier:
                                    // https://www.typescriptlang.org/docs/handbook/namespaces-and-modules.html#needless-namespacing
                                    //
                                    //exportItem =
                                    //    refAngularProject.PublicApiJson.exportItems
                                    //    .Where(expItm => expItm.exportType == "ExportNamedDeclaration" && expItm.exportSubtype == "ExportNamespaceSpecifier" && libFl.Equals(expItm.exportSource, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                                }
                            }

                        }
                        else if (refAngularProject.ProjectType == "application")
                        {
                            // string appFl = (".\\" + refAngularProject.SourceRoot + "\\" + refItem.FileFolder + "\\" + refItem.FileName).Replace("\\", "/");
                            string appFl = Path.Combine(refItem.FileProject, refItem.FileFolder, refItem.FileName).Replace(anglJson.AngularJsonPath, "");
                            if (!appFl.StartsWith("\\")) appFl = "\\" + appFl;
                            appFl = ("." + appFl).Replace("\\", "/");

                            if (refAngularProject.WebpackConfigJson != null)
                            {
                                if (refAngularProject.WebpackConfigJson.exposeItems != null)
                                {
                                    AngularWebpackConfigExposeItem exposeItm =
                                        refAngularProject.WebpackConfigJson.exposeItems
                                        .Where(itm => appFl.Equals(itm.exposeValue, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                    if (exposeItm != null)
                                    {
                                        return exposeItm.exposeKey;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return srcName;
        }
        string GetServiceClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".service", "Service");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetServiceClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string serviceClassName = GetServiceClassName(model, fileType);
            return GetNameByAngularJson(serviceClassName, anglJson, refItem, curItem);
        }
        string GetModuleClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".module", "Module").Replace(".routing", "Routing");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

            }
            return sb.ToString();
        }
        string GetModuleClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string moduleClassName = GetModuleClassName(model, fileType);
            return GetNameByAngularJson(moduleClassName, anglJson, refItem, curItem);
        }
        string GetModelClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".interface", "");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

            }
            return "I" + sb.ToString();
        }
        string GetModelClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string modelClassName = GetModelClassName(model, fileType);
            return GetNameByAngularJson(modelClassName, anglJson, refItem, curItem);
        }
        string GetFolderName(ModelViewSerializable model, string refFolder, string currFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }
        string GetFolderNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string refFolder, string currFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            if (anglJson != null)
            {
                AngularProject refAngularProject = GetAngularProjectByRefItem(anglJson, refItem);
                AngularProject curAngularProject = GetAngularProjectByRefItem(anglJson, curItem);
                if ((refAngularProject != null) && (curAngularProject != null))
                {
                    if (refAngularProject != curAngularProject)
                    {
                        return refAngularProject.ProjectName;
                    }
                }
            }


            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }
        string GetComponentClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".component", "Component");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetComponentClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string componentClassName = GetComponentClassName(model, fileType);
            return GetNameByAngularJson(componentClassName, anglJson, refItem, curItem);
        }
        string GetComponentSelectorCommonPart(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            return refItem.FileName.Replace(".component", "");
        }
        string GetInterfaceDlgName(ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Dlg";
        }
        string GetInterfaceName(ModelViewSerializable model)
        {
            return "I" + model.ViewName;
        }
        string GetEnumClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".enum", "");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

            }
            return sb.ToString();
        }
        string GetEnumClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string enumClassName = GetEnumClassName(model, fileType);
            return GetNameByAngularJson(enumClassName, anglJson, refItem, curItem);
        }
        string GetPipeClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".pipe", "Pipe");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetPipeClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string pipeClassName = GetPipeClassName(model, fileType);
            return GetNameByAngularJson(pipeClassName, anglJson, refItem, curItem);
        }
        string GetPipeSelectorName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".pipe", ""); // remove '.pipe'
            StringBuilder sb = new StringBuilder();
            bool toUpper = false; // the name starts from lower case letter
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetDirectiveClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".directive", "Directive");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetDirectiveClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string directiveClassName = GetDirectiveClassName(model, fileType);
            return GetNameByAngularJson(directiveClassName, anglJson, refItem, curItem);
        }
        string GetDirectiveSelectorName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".directive", "");
            StringBuilder sb = new StringBuilder();
            bool toUpper = false; // the name starts from lower case letter
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetDirectiveEventName(ModelViewSerializable model, string fileType)
        {
            string result = GetDirectiveSelectorName(model, fileType);
            return result + "Event";
        }
        string GetDirectiveEventNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            result = GetDirectiveEventName(model, fileType);
            return GetNameByAngularJson(result, anglJson, refItem, curItem);

        }
        string GetInterceptorClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".Interceptor", "Interceptor").Replace(".interceptor", "Interceptor");
            StringBuilder sb = new StringBuilder();
            bool toUpper = true;
            foreach (char c in fn)
            {
                if (c == '-')
                {
                    toUpper = true;
                }
                else
                {
                    if (toUpper)
                    {
                        sb.Append(Char.ToUpper(c));
                        toUpper = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString();
        }
        string GetInterceptorClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string interceptorClassName = GetInterceptorClassName(model, fileType);
            return GetNameByAngularJson(interceptorClassName, anglJson, refItem, curItem);
        }
        string GetModuleFileName(ModelViewSerializable model, string fileType)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            CommonStaffSerializable curItem = model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (curItem == null)
            {
                return result;
            }
            return result + curItem.FileFolder.Replace("\\", "/").Replace("app/", "").Replace("src/", "") + "/" + GetModuleComponentFolderName(model, fileType, fileType).Replace("./", "");
        }
        string GetModuleComponentFolderName(ModelViewSerializable model, string currFolder, string refFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(refFolder))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }
        string GetModuleComponentFolderNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string currFolder, string refFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(refFolder))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            if (anglJson != null)
            {
                AngularProject refAngularProject = GetAngularProjectByRefItem(anglJson, refItem);
                AngularProject curAngularProject = GetAngularProjectByRefItem(anglJson, curItem);
                if ((refAngularProject != null) && (curAngularProject != null))
                {
                    if (refAngularProject != curAngularProject)
                    {
                        return refAngularProject.ProjectName;
                    }
                }
            }
            return GetModuleComponentFolderName(model, currFolder, refFolder);
        }
        string GetEntityClassName(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            return refItem.FileName;
        }
        string GetNameSpaceName(ModelViewSerializable model, string currFolder, string DefaultProjectNameSpace)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if (curItem == null)
            {
                return result;
            }
            result = curItem.FileFolder.Replace("\\", ".");
            if (string.IsNullOrEmpty(DefaultProjectNameSpace))
            {
                return result;
            }
            return DefaultProjectNameSpace + "." + result;
        }
        string GetCrossComponentFolderNameEx(ModelViewSerializable model, string currFolder, ModelViewSerializable refModel, string refFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(currFolder) || (refModel == null) || string.IsNullOrEmpty(refFolder))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (refModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                refModel.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = refItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = new string[] { };
            if (!string.IsNullOrEmpty(curItem.FileFolder))
            {
                currFolders = curItem.FileFolder.Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            int refLen = refFolders.Length;
            int currLen = currFolders.Length;
            int minLen = refLen < currLen ? refLen : currLen;
            int cnt = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (!refFolders[i].Equals(currFolders[i], StringComparison.OrdinalIgnoreCase)) break;
                cnt++;
            }
            if (currLen > cnt)
            {
                result += string.Join("", Enumerable.Repeat("../", currLen - cnt));
            }
            if (refLen > cnt)
            {
                result += string.Join("/", refFolders, cnt, refLen - cnt) + "/";
            }
            result += refItem.FileName;
            return result;
        }
        string GenerateLoadChildrenImportWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = "loadChildren: () => import('').then(m => m.)";
            if ((anglJson == null) || (model == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (model.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            AngularProject refAngularProject = GetAngularProjectByRefItem(anglJson, refItem);
            AngularProject curAngularProject = GetAngularProjectByRefItem(anglJson, curItem);
            if ((refAngularProject != null) && (curAngularProject != null))
            {
                if (refAngularProject != curAngularProject)
                {
                    if (refAngularProject.ProjectType == "library")
                    {
                        return "loadChildren: () => import('" + refAngularProject.ProjectName + "').then(m => m." + GetModuleClassName(model, fileType) + ")";
                    } else if (refAngularProject.ProjectType == "application")
                    {
                        string aliasNm = null;
                        string appFl = Path.Combine(refItem.FileProject, refItem.FileFolder, refItem.FileName).Replace(anglJson.AngularJsonPath, "");
                        if (!appFl.StartsWith("\\")) appFl = "\\" + appFl;
                        appFl = ("." + appFl).Replace("\\", "/");

                        if (refAngularProject.WebpackConfigJson != null)
                        {
                            if (refAngularProject.WebpackConfigJson.exposeItems != null)
                            {
                                AngularWebpackConfigExposeItem exposeItm =
                                    refAngularProject.WebpackConfigJson.exposeItems
                                    .Where(itm => appFl.Equals(itm.exposeValue, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                                if (exposeItm != null)
                                {
                                    aliasNm = exposeItm.exposeKey;
                                }
                            }
                        }
                        if(string.IsNullOrEmpty(aliasNm)) aliasNm = appFl;
                        return "loadChildren: () => loadRemoteModule({type: 'manifest', remoteName: '" + refAngularProject.ProjectName + "', exposedModule: '"+ aliasNm + "'}).then(m => m." + GetModuleClassName(model, fileType) + ")";
                    }
                }
            }
            return "loadChildren: () => import('" + GetCrossComponentFolderNameEx(model, currFolder, model, fileType) + "').then(m => m." + GetModuleClassName(model, fileType) + ")";
        }
    }
}
