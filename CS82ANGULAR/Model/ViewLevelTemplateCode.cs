﻿using CS82ANGULAR.Model.Serializable;
using CS82ANGULAR.Model.Serializable.Angular;
using CS82ANGULAR.TemplateProcessingHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CS82ANGULAR.Model
{
    public class ViewLevelTemplateCode
    {
        // ModelViewSerializable GetViewByForeignNameChain(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        // string GetViewByForeignNameChain(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        // ModelViewSerializable GetViewByForeignNameChainEx(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
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
            return GetFolderName(model, refFolder, currFolder);
        }
        string GenerateLoadChildrenImportWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = "loadChildren: () => import('').then(m => m.)";
            if ((anglJson == null) || (model == null) || (currModel == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
                    }
                    else if (refAngularProject.ProjectType == "application")
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
                        if (string.IsNullOrEmpty(aliasNm)) aliasNm = appFl;
                        return "loadChildren: () => loadRemoteModule({type: 'manifest', remoteName: '" + refAngularProject.ProjectName + "', exposedModule: '" + aliasNm + "'}).then(m => m." + GetModuleClassName(model, fileType) + ")";
                    }
                }
            }
            return "loadChildren: () => import('" + GetCrossComponentFolderNameEx(currModel, currFolder, model, fileType) + "').then(m => m." + GetModuleClassName(model, fileType) + ")";
        }
        string GenerateLoadChildrenImportWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, AngularProject curAngularProject)
        {
            string result = "loadChildren: () => import('').then(m => m.)";
            if ((anglJson == null) || (model == null) || (curAngularProject == null) || string.IsNullOrEmpty(fileType))
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
            AngularProject refAngularProject = GetAngularProjectByRefItem(anglJson, refItem);
            if ((refAngularProject != null) && (curAngularProject != null))
            {
                if (refAngularProject != curAngularProject)
                {
                    if (refAngularProject.ProjectType == "library")
                    {
                        return "loadChildren: () => import('" + refAngularProject.ProjectName + "').then(m => m." + GetModuleClassName(model, fileType) + ")";
                    }
                    else if (refAngularProject.ProjectType == "application")
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
                        if (string.IsNullOrEmpty(aliasNm)) aliasNm = appFl;
                        return "loadChildren: () => loadRemoteModule({type: 'manifest', remoteName: '" + refAngularProject.ProjectName + "', exposedModule: '" + aliasNm + "'}).then(m => m." + GetModuleClassName(model, fileType) + ")";
                    }
                }
            }
            return "loadChildren: () => import('" + GetCrossComponentFolderNameExEx(Path.Combine(curAngularProject.AbsoluteSourceRoot, curAngularProject.ProjectPrefix), model, fileType) + "').then(m => m." + GetModuleClassName(model, fileType) + ")";
        }


        string GetInterfaceName(ModelViewSerializable model)
        {
            if (model == null)
            {
                return "I";
            }
            return "I" + model.ViewName;
        }
        string GetInterfaceNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfaceNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetInterfaceName(model);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfaceNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "I";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "I";
            }
            return GetInterfaceName(model);
        }
        string GetInterfaceNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetInterfaceNameEx(context, viewName);
            }
            if (context.ModelViews == null)
            {
                return GetInterfaceNameEx(context, viewName);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetInterfaceNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
        }

        string GetInterfacePageName(ModelViewSerializable model)
        {
            if (model == null)
            {
                return "IPage";
            }
            return "I" + model.PageViewName;
        }
        string GetInterfacePageNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfacePageName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfacePageNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetInterfacePageName(model);
            if (model == null)
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
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfacePageNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "IPage";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "IPage";
            }
            return GetInterfacePageName(model);
        }
        string GetInterfacePageNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetInterfacePageNameEx(context, viewName);
            }
            if (context.ModelViews == null)
            {
                return GetInterfacePageNameEx(context, viewName);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetInterfacePageNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
        }

        string GetInterfaceFilterName(ModelViewSerializable model)
        {
            if (model == null)
            {
                return "IFilter";
            }
            return "I" + model.ViewName + "Filter";
        }
        string GetInterfaceFilterNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceFilterName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfaceFilterNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetInterfaceFilterName(model);
            if (model == null)
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
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfaceFilterNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "IFilter";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "IFilter";
            }
            return GetInterfaceFilterName(model);
        }
        string GetInterfaceFilterNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetInterfaceFilterNameEx(context, viewName);
            }
            if (context.ModelViews == null)
            {
                return GetInterfaceFilterNameEx(context, viewName);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetInterfaceFilterNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
        }

        String GetJavaScriptServiceName(ModelViewSerializable model)
        {
            string result = model.ViewName + "Service";
            return result.First().ToString().ToUpper() + result.Substring(1);
        }
        String GetJavaScriptServiceNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetJavaScriptServiceName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetCommonFolderName(ModelViewSerializable model, DbContextSerializable context, string refFolder, string currFolder)
        {
            string result = "./";
            if ((model == null) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
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
        string GetCommonFolderNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string refFolder, string currFolder)
        {
            string result = "./";
            if ((model == null) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
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
            return GetCommonFolderName(model, context, refFolder, currFolder);
        }
        string GetCommonServiceClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetCommonServiceClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetCommonServiceClassName(context, fileType);
            if ((model == null) || (context == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetModelClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetModelClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetModelClassName(context, fileType);
            if (model == null)
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
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
            string result = GetServiceClassName(model, fileType);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetServiceClassNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetServiceClassName(model, fileType);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetServiceClassNameEx(DbContextSerializable context, string viewName, string fileType)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName) || string.IsNullOrEmpty(fileType))
            {
                return "";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "";
            }
            return GetServiceClassName(model, fileType);
        }
        string GetServiceClassNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetServiceClassNameEx(context, viewName, fileType);
            }
            if (context.ModelViews == null)
            {
                return GetServiceClassNameEx(context, viewName, fileType);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetServiceClassNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
        }

        string GetCrossComponentFolderName(ModelViewSerializable model, string currFolder, DbContextSerializable context, string refViewName, string refFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(currFolder) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(refViewName))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.ModelViews == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            ModelViewSerializable refModel = context.ModelViews.Where(v => v.ViewName == refViewName).FirstOrDefault();
            if (refModel == null)
            {
                return result;
            }
            if (refModel.CommonStaffs == null)
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
        string GetCrossComponentFolderNameExEx(string currAbsolutePath, ModelViewSerializable refModel, string refFolder)
        {
            string result = "./";
            if (string.IsNullOrEmpty(currAbsolutePath) || (refModel == null) || string.IsNullOrEmpty(refFolder))
            {
                return result;
            }
            if (refModel.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                refModel.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            string[] refFolders = new string[] { };
            if (!string.IsNullOrEmpty(refItem.FileFolder))
            {
                refFolders = Path.Combine(refItem.FileProject, refItem.FileFolder).Split(new string[] { "\\" }, StringSplitOptions.None);
            }
            string[] currFolders = currAbsolutePath.Split(new string[] { "\\" }, StringSplitOptions.None); ;

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

        string GetCrossComponentFolderNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string currFolder, DbContextSerializable context, string refViewName, string refFolder)
        {
            string result = "./";
            if ((model == null) || string.IsNullOrEmpty(currFolder) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(refViewName))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.ModelViews == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            ModelViewSerializable refModel = context.ModelViews.Where(v => v.ViewName == refViewName).FirstOrDefault();
            if (refModel == null)
            {
                return result;
            }
            if (refModel.CommonStaffs == null)
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
            return GetCrossComponentFolderName(model, currFolder, context, refViewName, refFolder);
        }
        string GetJavaScriptClassName(ModelViewSerializable model, string fileType)
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
            string fn = refItem.FileName.Replace(".class", "");
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
        string GetJavaScriptClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetJavaScriptClassName(model, fileType);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetJavaScriptClassNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetJavaScriptClassName(model, fileType);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetJavaScriptClassNameEx(DbContextSerializable context, string viewName, string fileType)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName) || string.IsNullOrEmpty(fileType))
            {
                return "";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            if (model == null)
            {
                return "";
            }
            return GetJavaScriptClassName(model, fileType);
        }
        string GetJavaScriptClassNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetJavaScriptClassNameEx(context, viewName, fileType);
            }
            if (context.ModelViews == null)
            {
                return GetJavaScriptClassNameEx(context, viewName, fileType);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetJavaScriptClassNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
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
            string result = GetComponentClassName(model, fileType);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetComponentClassNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetComponentClassName(model, fileType);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetComponentClassNameEx(DbContextSerializable context, string viewName, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType) || string.IsNullOrEmpty(viewName))
            {
                return result;
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetComponentClassName(model, fileType);
        }
        string GetComponentClassNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetComponentClassNameEx(context, viewName, fileType);
            }
            if (context.ModelViews == null)
            {
                return GetComponentClassNameEx(context, viewName, fileType);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetComponentClassNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
        }

        string GetContextComponentClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetContextComponentClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetContextComponentClassName(context, fileType);
            if ((model == null) || (context == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }

        string GetInterfaceDlgName(ModelViewSerializable model)
        {
            if (model == null)
            {
                return "IDlg";
            }
            return "I" + model.ViewName + "Dlg";
        }
        string GetInterfaceDlgNameEx(DbContextSerializable context, string viewName)
        {
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return "IDlg";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetInterfaceDlgName(model);
        }
        string GetInterfaceDlgNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceDlgName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfaceDlgNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetInterfaceDlgName(model);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetInterfaceDlgNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            if (context == null)
            {
                return GetInterfaceDlgNameEx(context, viewName);
            }
            if (context.ModelViews == null)
            {
                return GetInterfaceDlgNameEx(context, viewName);
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetInterfaceDlgNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
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
            string result = GetModuleClassName(model, fileType);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetModuleClassNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetModuleClassName(model, fileType);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }


        string GetModuleServiceClassName(ModelViewSerializable model, string fileType)
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
            string fn = refItem.FileName.Replace(".module", "Module").Replace(".routing", "Routing").Replace(".service", "Service");
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
        string GetModuleServiceClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetModuleServiceClassName(model, fileType);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetModuleServiceClassNameWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            string result = GetModuleServiceClassName(model, fileType);
            if ((model == null) || (currModel == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (currModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                currModel.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetModuleServiceClassNameExWithAnglrEx(AngularJson anglJson, DbContextSerializable context, string viewName, string fileType, ModelViewSerializable currModel, string currFolder)
        {
            ModelViewSerializable model = null;
            if (context == null)
            {
                return GetModuleServiceClassNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
            }
            if (context.ModelViews == null)
            {
                return GetModuleServiceClassNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
            }
            model = context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
            return GetModuleServiceClassNameWithAnglrEx(anglJson, model, fileType, currModel, currFolder);
        }


        string GetContextModuleClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetContextModuleClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetContextModuleClassName(context, fileType);
            if ((model == null) || (context == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }


        string GetInterfaceVDlgName(ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Vdlg";
        }
        string GetInterfaceVDlgNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceVDlgName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }

        string GetInterfaceADlgName(ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Adlg";
        }
        string GetInterfaceADlgNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceADlgName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }

        string GetInterfaceDDlgName(ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Ddlg";
        }
        string GetInterfaceDDlgNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceDDlgName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }

        string GetInterfaceUDlgName(ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Udlg";
        }
        string GetInterfaceUDlgNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, string currFolder)
        {
            string result = GetInterfaceUDlgName(model);
            if (model == null)
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
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }






        ModelViewSerializable GetViewByName(DbContextSerializable context, string ViewName)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
        }
        string GetViewByForeignNameChain(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return "";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (mv == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return ViewName;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (foreignKeys.Length < 1)
            {
                return "";
            }
            ModelViewForeignKeySerializable fk =
                mv.ForeignKeys.Where(f => f.NavigationName == foreignKeys[0]).FirstOrDefault();
            if (fk == null)
            {
                return "";
            }
            if (foreignKeys.Length == 1)
            {
                return GetViewByForeignNameChain(context, fk.ViewName, "");
            }
            return GetViewByForeignNameChain(context, fk.ViewName, string.Join(".", foreignKeys, 1, foreignKeys.Length - 1));
        }
        ModelViewSerializable GetViewByForeignNameChainEx(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return null;
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (mv == null)
            {
                return mv;
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return mv;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (foreignKeys.Length < 1)
            {
                return mv;
            }
            ModelViewForeignKeySerializable fk = mv.ForeignKeys.Where(f => f.NavigationName == foreignKeys[0]).FirstOrDefault();
            if (fk == null)
            {
                return mv;
            }
            if (foreignKeys.Length == 1)
            {
                return GetViewByForeignNameChainEx(context, fk.ViewName, "");
            }
            return GetViewByForeignNameChainEx(context, fk.ViewName, string.Join(".", foreignKeys, 1, foreignKeys.Length - 1));
        }
        List<string> GetSearchDialogViewsList(ModelViewSerializable model, DbContextSerializable context, List<string> sdViewsDict)
        {
            if ((model == null) || (context == null) || (sdViewsDict == null))
            {
                return sdViewsDict;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return sdViewsDict;
            }
            string viewNameForSel = null;
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!sdViewsDict.Contains(viewNameForSel))
                            {
                                sdViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!sdViewsDict.Contains(viewNameForSel))
                            {
                                sdViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!sdViewsDict.Contains(viewNameForSel))
                            {
                                sdViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
            }
            return sdViewsDict;
        }
        string GetDisplayAttributeValueString2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (sclrProp.Attributes == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributeSerializable attr =
                sclrProp.Attributes.Where(a => a.AttrName == "Display").FirstOrDefault();
            if (attr == null)
            {
                return prop.ViewPropertyName;
            }
            if (attr.VaueProperties == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributePropertySerializable attrProp =
                attr.VaueProperties.Where(v => v.PropName == propName).FirstOrDefault();
            if (attrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (string.IsNullOrEmpty(attrProp.PropValue))
            {
                return prop.ViewPropertyName;
            }
            else
            {
                char[] charsToTrim = { '"', ' ' };
                return attrProp.PropValue.Trim(charsToTrim);
            }
        }
        string GetAllDisplayedColumns(ModelViewSerializable model)
        {
            string result = "";
            if (model == null)
            {
                return result;
            }
            if ((model.UIListProperties == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewUIListPropertySerializable modelViewUIListPropertySerializable in model.UIListProperties)
            {
                if (modelViewUIListPropertySerializable.IsShownInView)
                {
                    if (result == "")
                    {
                        result = "'" + GetTypeScriptPropertyNameEx2(modelViewUIListPropertySerializable, model) + "'";
                    }
                    else
                    {
                        result += ", '" + GetTypeScriptPropertyNameEx2(modelViewUIListPropertySerializable, model) + "'";
                    }
                }
            }
            return result;
        }
        string GetDisplayedColumns(ModelViewSerializable model)
        {
            string result = "";
            if (model == null)
            {
                return result;
            }
            if ((model.UIListProperties == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewUIListPropertySerializable modelViewUIListPropertySerializable in model.UIListProperties)
            {
                if (modelViewUIListPropertySerializable.IsShownInView)
                {
                    if (result == "")
                    {
                        result = "'" + GetTypeScriptPropertyNameEx2(modelViewUIListPropertySerializable, model) + "'";
                    }
                    else
                    {
                        result += ", '" + GetTypeScriptPropertyNameEx2(modelViewUIListPropertySerializable, model) + "'";
                    }
                    if (modelViewUIListPropertySerializable.IsNewLineAfter)
                    {
                        break;
                    }
                }
            }
            return result;
        }
        bool hasMatSort(ModelViewSerializable model)
        {
            if (model == null)
            {
                return false;
            }
            if ((model.UIListProperties == null) || (model.ScalarProperties == null))
            {
                return false;
            }
            foreach (ModelViewUIListPropertySerializable modelViewUIListPropertySerializable in model.UIListProperties)
            {
                if (modelViewUIListPropertySerializable.IsShownInView)
                {
                    if (model.ScalarProperties.Any(s => s.ViewPropertyName == modelViewUIListPropertySerializable.ViewPropertyName && s.IsUsedBySorting))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        string matSortIfNeeded(ModelViewSerializable model)
        {
            if (hasMatSort(model))
            {
                return "matSort";
            }
            return "";
        }
        bool hasMatSortHeader(ModelViewUIListPropertySerializable modelViewUIListPropertySerializable, ModelViewSerializable model)
        {
            if ((model == null) || (modelViewUIListPropertySerializable == null))
            {
                return false;
            }
            if ((model.UIListProperties == null) || (model.ScalarProperties == null))
            {
                return false;
            }
            return model.ScalarProperties.Any(s => s.ViewPropertyName == modelViewUIListPropertySerializable.ViewPropertyName && s.IsUsedBySorting);
        }
        string matSortHeaderIfNeeded(ModelViewUIListPropertySerializable modelViewUIListPropertySerializable, ModelViewSerializable model)
        {
            if (hasMatSortHeader(modelViewUIListPropertySerializable, model))
            {
                return "mat-sort-header";
            }
            return "";
        }
        bool hasNgbSortHeader(ModelViewUIListPropertySerializable modelViewUIListPropertySerializable, ModelViewSerializable model)
        {
            if ((model == null) || (modelViewUIListPropertySerializable == null))
            {
                return false;
            }
            if ((model.UIListProperties == null) || (model.ScalarProperties == null))
            {
                return false;
            }
            return model.ScalarProperties.Any(s => s.ViewPropertyName == modelViewUIListPropertySerializable.ViewPropertyName && s.IsUsedBySorting);
        }
        string GetDirectiveSelectorName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetDirectiveSelectorNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetDirectiveSelectorName(context, fileType);
            if (model == null)
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string ngbSortHeaderIfNeeded(ModelViewUIListPropertySerializable modelViewUIListPropertySerializable, ModelViewSerializable model, DbContextSerializable context, string fileType)
        {
            if (hasNgbSortHeader(modelViewUIListPropertySerializable, model))
            {
                string selectorName = GetDirectiveSelectorName(context, fileType);
                return selectorName + "=\"" + GetTypeScriptPropertyNameEx2(modelViewUIListPropertySerializable, model) + "\"  (sort)=\"onSort($event)\"";
            }
            return "";
        }
        string GetDirectiveClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetDirectiveClassNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetDirectiveClassName(context, fileType);
            if (model == null)
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetDirectiveEventName(DbContextSerializable context, string fileType)
        {
            string result = GetDirectiveSelectorName(context, fileType);
            return result + "Event";
        }
        string GetDirectiveEventNameWithAnglr(AngularJson anglJson, ModelViewSerializable model, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetDirectiveEventName(context, fileType);
            if (model == null)
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                model.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }


        string GetInputTypeToEnumName(int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return "AddMode";
                case 2:
                    return "UpdateMode";
                default:
                    return "DeleteMode";
            }
        }
        InputTypeEnum GetInputTypeWhenXXX(ModelViewUIFormPropertySerializable prop, int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return prop.InputTypeWhenAdd;
                case 2:
                    return prop.InputTypeWhenUpdate;
                default:
                    return prop.InputTypeWhenDelete;
            }
        }
        string GetInterfaceEDlgName(ModelViewSerializable model)
        {
            return "I" + model.ViewName + "Edlg";
        }
        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }
        public string FirstLetterToLower(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);
            return str.ToUpper();
        }
        string GetTypeScriptPropertyNameEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetTypeScriptPropertyName(sclrProp, model);
        }
        string GetTypeScriptPropertyNameEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetTypeScriptPropertyName(sclrProp, model);
        }
        string GetUnNamedAtributeValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if (sclrProp != null)
            {
                if (sclrProp.Attributes != null)
                {
                    ModelViewAttributeSerializable modelViewAttributeSerializable =
                        sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                    if (modelViewAttributeSerializable != null)
                    {
                        if (modelViewAttributeSerializable.VaueProperties != null)
                        {

                            ModelViewAttributePropertySerializable modelViewAttributePropertySerializable =
                                modelViewAttributeSerializable.VaueProperties.Where(p => (string.IsNullOrEmpty(p.PropName) || (p.PropName == "..."))).FirstOrDefault();
                            if (modelViewAttributePropertySerializable != null)
                            {
                                return modelViewAttributePropertySerializable.PropValue;
                            }
                        }
                    }
                }
            }
            return null;
        }
        string GetMaxLen(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            if (prop.UnderlyingTypeName.ToLower() == "system.string")
            {
                string propValue = GetUnNamedAtributeValue(prop, "StringLength");
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
                propValue = GetUnNamedAtributeValue(prop, "MaxLength");
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
            }
            return "null";
        }
        string GetMaxLenEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMaxLen(sclrProp, model);
        }
        string GetMaxLenEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMaxLen(sclrProp, model);
        }
        string GetCCharpDatatype(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            return prop.UnderlyingTypeName.ToLower().Replace("system.", "");
        }
        string GetCCharpDatatypeEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetCCharpDatatype(sclrProp, model);
        }
        string GetCCharpDatatypeEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetCCharpDatatype(sclrProp, model);
        }
        string GetMinVal(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            string propValue = GetAtributeValueByNo(prop, "IntegerValidator", 0);
            if (!string.IsNullOrEmpty(propValue))
            {
                propValue = propValue.Replace("\"", "");
                if (!string.IsNullOrEmpty(propValue))
                {
                    return propValue;
                }
            }
            if (prop.UnderlyingTypeName.ToLower() == "system.datetime")
            {
                propValue = GetAtributeValueByNo(prop, "Range", 1);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return "new Date('" + propValue + "')";
                    }
                }
            }
            else
            {
                propValue = GetAtributeValueByNo(prop, "Range", 0);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
            }
            return "null";
        }
        string GetMinValEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMinVal(sclrProp, model);
        }
        string GetMinValEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMinVal(sclrProp, model);
        }
        string GetMaxVal(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            string propValue = GetAtributeValueByNo(prop, "IntegerValidator", 1);
            if (!string.IsNullOrEmpty(propValue))
            {
                propValue = propValue.Replace("\"", "");
                if (!string.IsNullOrEmpty(propValue))
                {
                    return propValue;
                }
            }
            if (prop.UnderlyingTypeName.ToLower() == "system.datetime")
            {
                propValue = GetAtributeValueByNo(prop, "Range", 2);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return "new Date('" + propValue + "')";
                    }
                }
            }
            else
            {
                propValue = GetAtributeValueByNo(prop, "Range", 1);
                if (!string.IsNullOrEmpty(propValue))
                {
                    propValue = propValue.Replace("\"", "");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        return propValue;
                    }
                }
            }
            return "null";
        }
        string GetMaxValEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMinVal(sclrProp, model);
        }
        string GetMaxValEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "null";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetMaxVal(sclrProp, model);
        }
        string GetFormControlHiddenCondition(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string eformModePropName)
        {
            if ((prop.InputTypeWhenAdd == InputTypeEnum.Hidden) &&
                (prop.InputTypeWhenUpdate == InputTypeEnum.Hidden) &&
                (prop.InputTypeWhenDelete == InputTypeEnum.Hidden))
            {
                return "";
            }
            string result = "*ngIf = \"";
            bool setOr = false;
            if (prop.InputTypeWhenAdd == InputTypeEnum.Hidden)
            {
                result = result + "(" + eformModePropName + "==1)";
                setOr = true;
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.Hidden)
            {
                if (setOr)
                {
                    result = result + "||";
                }
                result = result + "(" + eformModePropName + "==2)";
                setOr = true;
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.Hidden)
            {
                if (setOr)
                {
                    result = result + "||";
                }
                result = result + "(" + eformModePropName + "==3)";
            }
            return result + "\"";
        }
        string GetDisplayAttributeValueString(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model, string propName)
        {
            if (prop == null)
            {
                return "";
            }
            if (prop.Attributes == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributeSerializable attr =
                prop.Attributes.Where(a => a.AttrName == "Display").FirstOrDefault();
            if (attr == null)
            {
                return prop.ViewPropertyName;
            }
            if (attr.VaueProperties == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributePropertySerializable attrProp =
                attr.VaueProperties.Where(v => v.PropName == propName).FirstOrDefault();
            if (attrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (string.IsNullOrEmpty(attrProp.PropValue))
            {
                return prop.ViewPropertyName;
            }
            else
            {
                char[] charsToTrim = { '"', ' ' };
                return attrProp.PropValue.Trim(charsToTrim);
            }
        }
        string GetDisplayAttributeValueStringEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            if ((prop == null) || (model == null))
            {
                return "";
            }
            if (model.ScalarProperties == null)
            {
                return "";
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetDisplayAttributeValueString(sclrProp, model, propName);
        }
        string GetDisplayAttributeValueStringEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            if ((prop == null) || (model == null))
            {
                return "";
            }
            if (model.ScalarProperties == null)
            {
                return "";
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetDisplayAttributeValueString(sclrProp, model, propName);
        }
        string GetContextComponentSelectorCommonPart(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        List<string> GetFKViewsList(ModelViewSerializable model,
                                         DbContextSerializable context,
                                         List<string> fkViewsDict)
        {
            if ((model == null) || (context == null) || (fkViewsDict == null))
            {
                return fkViewsDict;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return fkViewsDict;
            }
            string viewNameForSel = null;
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if ((modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Typeahead))
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!fkViewsDict.Contains(viewNameForSel))
                            {
                                fkViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if ((modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Typeahead))
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!fkViewsDict.Contains(viewNameForSel))
                            {
                                fkViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
                if ((modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                    (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Typeahead))
                {
                    viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            if (!fkViewsDict.Contains(viewNameForSel))
                            {
                                fkViewsDict.Add(viewNameForSel);
                            }
                        }
                    }
                }
            }
            return fkViewsDict;
        }
        List<ModelViewPropertyOfVwSerializable> GetPropsByForeignKey(ModelViewSerializable model, ModelViewForeignKeySerializable foreignKey)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((model == null) || (foreignKey == null))
            {
                return result;
            }
            if (foreignKey.PrincipalKeyProps == null || foreignKey.ForeignKeyProps == null || model.ScalarProperties == null)
            {
                return result;
            }
            if ((foreignKey.PrincipalKeyProps.Count != foreignKey.ForeignKeyProps.Count) || (foreignKey.ForeignKeyProps.Count < 1))
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable fkProp in foreignKey.PrincipalKeyProps)
            {
                ModelViewPropertyOfVwSerializable prop =
                    model.ScalarProperties.Where(p => (p.OriginalPropertyName == fkProp.OriginalPropertyName) && (foreignKey.NavigationName == p.ForeignKeyNameChain)).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            foreach (ModelViewKeyPropertySerializable fkProp in foreignKey.ForeignKeyProps)
            {
                ModelViewPropertyOfVwSerializable prop =
                    model.ScalarProperties.Where(p => (p.OriginalPropertyName == fkProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain)).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelUniqueKeyProps(ModelViewSerializable model, ModelViewUniqueKeySerializable uk)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((model == null) || (uk == null))
            {
                return result;
            }
            if ((uk.UniqueKeyProperties == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable modelViewKeyPropertySerializable in uk.UniqueKeyProperties)
            {
                ModelViewPropertyOfVwSerializable prop = GetScalarPropByOriginaPropName(modelViewKeyPropertySerializable.OriginalPropertyName, model);
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            return result;
        }
        List<ModelViewUniqueKeyOfVwSerializable> GetModelUniqueKeys(ModelViewSerializable model, List<ModelViewUniqueKeyOfVwSerializable> rsltKeys)
        {
            if ((model == null) || (rsltKeys == null)) return rsltKeys;
            if ((model.UniqueKeys == null) || (model.ScalarProperties == null)) return rsltKeys;
            foreach (ModelViewUniqueKeySerializable uk in model.UniqueKeys)
            {
                if (uk.UniqueKeyProperties == null) continue;
                if (uk.UniqueKeyProperties.Count < 1) continue;
                List<ModelViewPropertyOfVwSerializable> ukprops = GetModelUniqueKeyProps(model, uk);
                if (ukprops.Count == uk.UniqueKeyProperties.Count)
                {
                    rsltKeys.Add(new ModelViewUniqueKeyOfVwSerializable()
                    {
                        UniqueKeyName = uk.UniqueKeyName,
                        IsPrimary = false,
                        UniqueKeyProperties = ukprops
                    });
                }
            }
            return rsltKeys;
        }
        ModelViewUniqueKeyOfVwSerializable GetModelPrimaryKey(ModelViewSerializable model)
        {
            if (model == null) return null;
            if (model.PrimaryKeyProperties == null) return null;
            if (model.PrimaryKeyProperties.Count < 1) return null;
            List<ModelViewPropertyOfVwSerializable> props = GetModelPrimaryKeyProps(model);
            if (props.Count != model.PrimaryKeyProperties.Count) return null;
            return new ModelViewUniqueKeyOfVwSerializable()
            {
                UniqueKeyName = null,
                IsPrimary = true,
                UniqueKeyProperties = props
            };
        }
        bool CheckModelIfIndexIsCorrect(ModelViewSerializable model, ModelViewUniqueKeyOfVwSerializable indx, out string error)
        {
            if ((model == null) || (indx == null))
            {
                error = "Input params is not defined";
                return false;
            }
            if (indx.UniqueKeyProperties == null)
            {
                error = "UniqueKeyProperties of the Index are not defined";
                return false;
            }
            if (indx.UniqueKeyProperties.Count < 1)
            {
                if (indx.IsPrimary)
                    error = "UniqueKeyProperties.Count of the Primary Index is less than 1";
                else
                    error = "UniqueKeyProperties.Count of the Unique Index (UniqueKeyName == " + indx.UniqueKeyName + ") is less than 1";
                return false;
            }

            if (indx.IsPrimary)
            {
                if (model.PrimaryKeyProperties == null)
                {
                    error = "PrimaryKeyProperties of the model are not defined";
                    return false;
                }
                if (model.PrimaryKeyProperties.Count != indx.UniqueKeyProperties.Count)
                {
                    error = "Not all Index fields are included in the Model";
                    return false;
                }
            }
            else
            {
                if (model.UniqueKeys == null)
                {
                    error = "UniqueKeys of the model are not defined (UniqueKeyName == " + indx.UniqueKeyName + ")";
                    return false;
                }
                if (string.IsNullOrEmpty(indx.UniqueKeyName))
                {
                    error = "The Name of the Index is not defined (UniqueKeyName)";
                    return false;
                }
                ModelViewUniqueKeySerializable mindx = model.UniqueKeys.Where(i => i.UniqueKeyName == indx.UniqueKeyName).FirstOrDefault();
                if (mindx == null)
                {
                    error = "Could not find index in model by name (Unique Index Name == " + indx.UniqueKeyName + ")";
                    return false;
                }
                if (mindx.UniqueKeyProperties == null)
                {
                    error = "UniqueKeyProperties of the Unique Index (Unique Index Name == " + indx.UniqueKeyName + ") are not defined";
                    return false;
                }
                if (mindx.UniqueKeyProperties.Count != indx.UniqueKeyProperties.Count)
                {
                    error = "Not all Unique Index fields are included in the Model (Unique Index Name == " + indx.UniqueKeyName + ")";
                    return false;
                }
            }
            error = "";
            return true;
        }
        bool IsTableMatchesIndex(ModelViewSerializable model)
        {
            if (model == null) return false;
            if ((model.ScalarProperties == null) || (model.PrimaryKeyProperties == null)) return false;
            if ((model.ScalarProperties.Count != model.PrimaryKeyProperties.Count) || (model.ScalarProperties.Count < 1)) return false;
            foreach (ModelViewKeyPropertySerializable pkp in model.PrimaryKeyProperties)
            {
                if (GetScalarPropByOriginaPropName(pkp.OriginalPropertyName, model) == null) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToTailOfPrimKey(ModelViewForeignKeySerializable fk, ModelViewSerializable model)
        {
            if ((model == null) || (fk == null)) return false;
            if ((model.PrimaryKeyProperties == null) || (model.ForeignKeys == null) || (fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) return false;
            if ((model.PrimaryKeyProperties.Count < 1) || (fk.PrincipalKeyProps.Count != fk.ForeignKeyProps.Count) || (fk.ForeignKeyProps.Count < 1) || (fk.ForeignKeyProps.Count >= model.PrimaryKeyProperties.Count)) return false;
            for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
            {
                if (fk.ForeignKeyProps[fk.ForeignKeyProps.Count - (1 + i)].OriginalPropertyName != model.PrimaryKeyProperties[model.PrimaryKeyProperties.Count - (1 + i)].OriginalPropertyName) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToScalars(ModelViewForeignKeySerializable fk, ModelViewSerializable model)
        {
            if ((model == null) || (fk == null)) return false;
            if ((model.ScalarProperties == null) || (fk.ForeignKeyProps == null)) return false;
            if (fk.ForeignKeyProps.Count < 1) return false;
            foreach (ModelViewKeyPropertySerializable fkp in fk.ForeignKeyProps)
            {
                if (GetScalarPropByOriginaPropName(fkp.OriginalPropertyName, model) == null) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToScalarsEx(ModelViewForeignKeySerializable detailFk, ModelViewSerializable detailModel, ModelViewSerializable model)
        {
            if ((detailModel == null) || (detailFk == null) || (model == null)) return false;
            if ((detailModel.ScalarProperties == null) || (detailFk.ForeignKeyProps == null) || (model.ScalarProperties == null) || (detailFk.PrincipalKeyProps == null) || (model.PrimaryKeyProperties == null)) return false;
            if ((detailFk.ForeignKeyProps.Count < 1) || (model.ScalarProperties.Count < 1) || (model.PrimaryKeyProperties.Count != detailFk.ForeignKeyProps.Count) || (detailFk.PrincipalKeyProps.Count != detailFk.ForeignKeyProps.Count) || (model.PrimaryKeyProperties.Count != detailFk.ForeignKeyProps.Count)) return false;
            for (int i = 0; i < detailFk.ForeignKeyProps.Count; i++)
            {
                ModelViewPropertyOfVwSerializable detailsprp = GetScalarPropByOriginaPropName(detailFk.ForeignKeyProps[i].OriginalPropertyName, detailModel);
                if (detailsprp == null) return false;
                ModelViewPropertyOfVwSerializable modelsprp = GetScalarPropByOriginaPropName(detailFk.PrincipalKeyProps[i].OriginalPropertyName, model);
                if (modelsprp == null) return false;
                if (modelsprp.ViewPropertyName != detailsprp.ViewPropertyName) return false;
            }
            return true;
        }
        bool IsForeigKeyMapedToScalarsExEx(ModelViewForeignKeySerializable detailFk, ModelViewSerializable detailModel, ModelViewSerializable model)
        {
            if ((detailModel == null) || (detailFk == null) || (model == null)) return false;
            if ((detailModel.ScalarProperties == null) || (detailFk.ForeignKeyProps == null) || (model.ScalarProperties == null) || (detailFk.PrincipalKeyProps == null) || (model.ForeignKeys == null)) return false;
            if ((detailFk.ForeignKeyProps.Count < 1) || (model.ScalarProperties.Count < 1) || (detailFk.PrincipalKeyProps.Count != detailFk.ForeignKeyProps.Count)) return false;
            List<ModelViewForeignKeySerializable> modelFks = model.ForeignKeys.Where(f => f.ViewName == detailFk.ViewName).ToList();
            if (modelFks.Count < 1) return false;
            for (int i = 0; i < detailFk.ForeignKeyProps.Count; i++)
            {
                ModelViewPropertyOfVwSerializable detailsprp = GetScalarPropByOriginaPropName(detailFk.ForeignKeyProps[i].OriginalPropertyName, detailModel);
                if (detailsprp == null) return false;
            }
            foreach (ModelViewForeignKeySerializable modelFk in modelFks)
            {
                if (modelFk.ForeignKeyProps == null) continue;
                if (modelFk.ForeignKeyProps.Count != detailFk.ForeignKeyProps.Count) continue;
                bool passed = false;
                for (int i = 0; i < detailFk.ForeignKeyProps.Count; i++)
                {
                    ModelViewPropertyOfVwSerializable detailsprp = GetScalarPropByOriginaPropName(detailFk.ForeignKeyProps[i].OriginalPropertyName, detailModel);
                    ModelViewPropertyOfVwSerializable modelsprp = GetScalarPropByOriginaPropName(modelFk.ForeignKeyProps[i].OriginalPropertyName, model);
                    passed = modelsprp != null;
                    if (!passed) break;
                    passed = modelsprp.ViewPropertyName == detailsprp.ViewPropertyName;
                    if (!passed) break;
                }
                if (passed) return true;
            }
            return false;
        }

        bool IsOnePropForeigKey(ModelViewForeignKeySerializable searchFk)
        {
            if (searchFk == null) return false;
            if ((searchFk.PrincipalKeyProps == null) || (searchFk.ForeignKeyProps == null)) return false;
            if ((searchFk.PrincipalKeyProps.Count == searchFk.ForeignKeyProps.Count) && (searchFk.ForeignKeyProps.Count == 1)) return true;
            return false;
        }
        /*
                bool IsLookUpTable(ModelViewSerializable searchMdl)
                {
                    if (searchMdl == null) return false;
                    if ((searchMdl.ScalarProperties == null) || (searchMdl.PrimaryKeyProperties == null) || (searchMdl.UniqueKeys == null)) return false;
                    if ((searchMdl.ScalarProperties.Count != 2) || (searchMdl.PrimaryKeyProperties.Count != 1) || (searchMdl.UniqueKeys.Count != 1)) return false;
                    if (searchMdl.UniqueKeys[0].UniqueKeyProperties == null) return false;
                    if (searchMdl.UniqueKeys[0].UniqueKeyProperties.Count != 1) return false;
                    if (searchMdl.UniqueKeys[0].UniqueKeyProperties[0].OriginalPropertyName == searchMdl.PrimaryKeyProperties[0].OriginalPropertyName) return false;
                    if ((GetScalarPropByOriginaPropName(searchMdl.UniqueKeys[0].UniqueKeyProperties[0].OriginalPropertyName, searchMdl) == null) ||
                        (GetScalarPropByOriginaPropName(searchMdl.PrimaryKeyProperties[0].OriginalPropertyName, searchMdl) == null)) return false;
                    return true;
                }

                bool IsUniqKeyMapedToScalarsEx(ModelViewUniqueKeySerializable searchUk, ModelViewSerializable searchModel, ModelViewSerializable model)
                {
                    if ((searchModel == null) || (searchUk == null) || (model == null)) return false;
                    if ((searchModel.ScalarProperties == null) || (searchUk.UniqueKeyProperties == null) || (model.ScalarProperties == null)) return false;
                    if ((searchUk.UniqueKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1)) return false;
                    foreach (ModelViewKeyPropertySerializable ukp in searchUk.UniqueKeyProperties)
                    {
                        ModelViewPropertyOfVwSerializable sprp = GetScalarPropByOriginaPropName(ukp.OriginalPropertyName, searchModel);
                        if (sprp == null) return false;
                        if (!model.ScalarProperties.Any(p => p.ViewPropertyName == sprp.ViewPropertyName)) return false;
                    }
                    return true;
                }
        */
        bool IsForeigKeyMapedToPrimKey(ModelViewForeignKeySerializable fk, ModelViewSerializable model)
        {
            if ((model == null) || (fk == null)) return false;
            if ((model.PrimaryKeyProperties == null) || (model.ForeignKeys == null) || (fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) return false;
            if ((model.PrimaryKeyProperties.Count < 1) || (fk.PrincipalKeyProps.Count != fk.ForeignKeyProps.Count) || (fk.ForeignKeyProps.Count < 1) || (fk.ForeignKeyProps.Count >= model.PrimaryKeyProperties.Count)) return false;
            foreach (ModelViewKeyPropertySerializable fkp in fk.ForeignKeyProps)
            {
                if (!model.PrimaryKeyProperties.Any(p => p.OriginalPropertyName == fkp.OriginalPropertyName)) return false;
            }
            return true;
        }
        bool IsForeigKeyWithCorrectPropsOrder(ModelViewForeignKeySerializable otherFk, ModelViewSerializable m2mMdl)
        {
            if ((m2mMdl == null) || (otherFk == null)) return false;
            if ((m2mMdl.PrimaryKeyProperties == null) || (otherFk.ForeignKeyProps == null)) return false;
            if ((m2mMdl.PrimaryKeyProperties.Count < 1) || (otherFk.ForeignKeyProps.Count < 1)) return false;
            List<int> positions = new List<int>();
            foreach (ModelViewKeyPropertySerializable fkp in otherFk.ForeignKeyProps)
            {
                int index = m2mMdl.PrimaryKeyProperties.FindIndex(delegate (ModelViewKeyPropertySerializable pkp) { return pkp.OriginalPropertyName == fkp.OriginalPropertyName; });
                if (index < 0) return false;
                positions.Add(index);
            }
            positions.Sort();
            for (int i = 0; i < positions.Count - 1; i++)
            {
                if (positions[i] + 1 != positions[i + 1]) return false;
            }
            return true;
        }
        int GetForeigKeyMaxPropsPosition(ModelViewForeignKeySerializable otherFk, ModelViewSerializable m2mMdl)
        {
            if ((m2mMdl == null) || (otherFk == null)) return -1;
            if ((m2mMdl.PrimaryKeyProperties == null) || (otherFk.ForeignKeyProps == null)) return -1;
            if ((m2mMdl.PrimaryKeyProperties.Count < 1) || (otherFk.ForeignKeyProps.Count < 1)) return -1;
            int rslt = 0;
            foreach (ModelViewKeyPropertySerializable fkp in otherFk.ForeignKeyProps)
            {
                int index = m2mMdl.PrimaryKeyProperties.FindIndex(delegate (ModelViewKeyPropertySerializable pkp) { return pkp.OriginalPropertyName == fkp.OriginalPropertyName; });
                if (index < 0) return -1;
                if (rslt < index) rslt = index;
            }
            return rslt;
        }
        List<Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>> GetSearchResources(ModelViewSerializable model, DbContextSerializable context)
        {
            if ((context == null) || (model == null)) return null;
            if ((context.ModelViews == null) || (model.PrimaryKeyProperties == null) || (model.ScalarProperties == null)) return null;
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1)) return null;
            List<ModelViewSerializable> m2mMdls = context.ModelViews.Where(p => (p.ForeignKeys.Any(f => f.ViewName == model.ViewName) && (p.ForeignKeys.Count > 1))).ToList();
            if (m2mMdls.Count < 1)
            {
                return null;
            }
            // m2mModel, m2mForeignKey, List<Tuple< model.ForeignKey, m2mModel.additionalForeignKey >>, searchModel, searchFk, searchUk, ukpropsMpt, ukpropsToFrgn
            List<Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>> result = null;
            foreach (ModelViewSerializable m2mMdl in m2mMdls)
            {
                if (!IsTableMatchesIndex(m2mMdl))
                {
                    continue;
                }
                List<ModelViewForeignKeySerializable> m2mFks = m2mMdl.ForeignKeys.Where(f => f.ViewName == model.ViewName).ToList();
                foreach (ModelViewForeignKeySerializable m2mFk in m2mFks)
                {
                    if (!IsForeigKeyMapedToTailOfPrimKey(m2mFk, m2mMdl)) continue;
                    if (!IsForeigKeyMapedToScalarsEx(m2mFk, m2mMdl, model)) continue;
                    // m2mMdl - m2mModel, m2mFk - m2mForeignKey,
                    List<KeyValuePair<ModelViewForeignKeySerializable, int>> searchFks = null;
                    //List<int> searchFkPosition = null;
                    foreach (ModelViewForeignKeySerializable searchFk in m2mMdl.ForeignKeys)
                    {
                        if (m2mFk == searchFk) continue;
                        if (!IsOnePropForeigKey(searchFk)) continue;
                        ModelViewSerializable searchMdl = context.ModelViews.Where(mv => (mv.ViewName == searchFk.ViewName)).FirstOrDefault();
                        if (searchMdl == null) continue;
                        if (!IsLookUpTable(searchMdl)) continue;
                        if (!IsUniqKeyMapedToScalarsEx(searchMdl.UniqueKeys[0], searchMdl, model)) continue;
                        if (searchFks == null) searchFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                        searchFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(searchFk, GetForeigKeyMaxPropsPosition(searchFk, m2mMdl)));
                    }
                    if (searchFks == null)
                    {
                        continue;
                    }
                    int lastValidPosition = m2mMdl.PrimaryKeyProperties.Count - model.PrimaryKeyProperties.Count - 1;
                    if (searchFks != null)
                    {
                        if (searchFks.Any(p => p.Value < 0)) continue;
                        searchFks = searchFks.OrderBy(p => p.Value).ToList();
                        bool IsCorrect = true;
                        for (int i = 0; i < searchFks.Count - 1; i++)
                        {
                            IsCorrect = searchFks[i].Value == searchFks[i + 1].Value - 1;
                            if (!IsCorrect) break;
                        }
                        if (!IsCorrect)
                        {
                            continue;
                        }
                        if (searchFks[searchFks.Count - 1].Value != lastValidPosition)
                        {
                            continue;
                        }
                        lastValidPosition = searchFks[0].Value - 1;
                    }


                    List<KeyValuePair<ModelViewForeignKeySerializable, int>> otherFks = null;
                    foreach (ModelViewForeignKeySerializable otherFk in m2mMdl.ForeignKeys)
                    {
                        if (m2mFk == otherFk) continue;
                        if (searchFks != null)
                        {
                            if (searchFks.Any(p => p.Key == otherFk)) continue;
                        }
                        if (!IsForeigKeyMapedToPrimKey(otherFk, m2mMdl)) continue;
                        if (!IsForeigKeyMapedToScalarsExEx(otherFk, m2mMdl, model)) continue;
                        if (!IsForeigKeyWithCorrectPropsOrder(otherFk, m2mMdl)) continue;
                        int mxPs = GetForeigKeyMaxPropsPosition(otherFk, m2mMdl);
                        if ((mxPs < 0) || (mxPs > lastValidPosition)) continue;
                        if (otherFks == null) otherFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                        otherFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(otherFk, mxPs));
                    }
                    if (otherFks != null)
                    {
                        otherFks = otherFks.OrderBy(p => p.Value).ToList();
                        bool IsCorrect = true;
                        for (int i = 0; i < otherFks.Count - 1; i++)
                        {
                            IsCorrect = otherFks[i].Value == otherFks[i + 1].Value - otherFks[i].Key.ForeignKeyProps.Count;
                            if (!IsCorrect) break;
                        }
                        if (!IsCorrect) continue;
                        if (otherFks[otherFks.Count - 1].Value != lastValidPosition)
                            continue;
                        lastValidPosition = otherFks[0].Value - otherFks[0].Key.ForeignKeyProps.Count;
                    }


                    List<KeyValuePair<ModelViewForeignKeySerializable, int>> externalFks = null;
                    foreach (ModelViewForeignKeySerializable externalFk in m2mMdl.ForeignKeys)
                    {
                        if (externalFk == m2mFk) continue;
                        if (searchFks != null)
                        {
                            if (searchFks.Any(p => p.Key == externalFk)) continue;
                        }
                        if (otherFks != null)
                        {
                            if (otherFks.Any(p => p.Key == externalFk)) continue;
                        }
                        if (!IsForeigKeyMapedToPrimKey(externalFk, m2mMdl)) continue;
                        if (!IsForeigKeyMapedToScalars(externalFk, m2mMdl)) continue;
                        if (!IsForeigKeyWithCorrectPropsOrder(externalFk, m2mMdl)) continue;
                        int mxPs = GetForeigKeyMaxPropsPosition(externalFk, m2mMdl);
                        if ((mxPs < 0) || (mxPs > lastValidPosition)) continue;
                        if (externalFks == null) externalFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                        externalFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(externalFk, mxPs));
                    }
                    if (externalFks != null)
                    {
                        externalFks = externalFks.OrderBy(p => p.Value).ToList();
                        bool IsCorrect = true;
                        for (int i = 0; i < externalFks.Count - 1; i++)
                        {
                            IsCorrect = externalFks[i].Value == externalFks[i + 1].Value - externalFks[i].Key.ForeignKeyProps.Count;
                            if (!IsCorrect) break;
                        }
                        if (!IsCorrect) continue;
                        if (externalFks[externalFks.Count - 1].Value != lastValidPosition) continue;
                        lastValidPosition = externalFks[0].Value - externalFks[0].Key.ForeignKeyProps.Count;
                    }
                    if (lastValidPosition != -1) continue;
                    int AllFkCount = (searchFks == null ? 0 : searchFks.Count) +
                                    (otherFks == null ? 0 : otherFks.Count) +
                                    (externalFks == null ? 0 : externalFks.Count);
                    if (AllFkCount != (m2mMdl.ForeignKeys.Count - 1)) continue;
                    if (result == null)
                    {
                        result = new List<Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>>();
                    }
                    result.Add(new Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>(
                        m2mMdl, m2mFk, searchFks, otherFks, externalFks
                    ));
                }
            }
            return result;
        }
        ModelViewPropertyOfVwSerializable GetModelScalarPropByKeyProp(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        {
            //ModelViewPropertyOfVwSerializable rslt = null;
            if ((model == null) || (pk == null)) return null;
            if (model.ScalarProperties == null) return null;
            if (model.AllProperties == null) return null;
            ModelViewEntityPropertySerializable aprop = model.AllProperties.Where(p => p.OriginalPropertyName == pk.OriginalPropertyName).FirstOrDefault();
            if (aprop == null) return null;
            ModelViewPropertyOfVwSerializable scProp =
                model.ScalarProperties.Where(p => ((p.OriginalPropertyName == pk.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))).FirstOrDefault();
            if (scProp != null) return scProp;
            if (model.ForeignKeys != null)
            {
                foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
                {
                    scProp = null;
                    if ((fk.ForeignKeyProps != null) && (fk.PrincipalKeyProps != null))
                    {
                        int cnt = fk.ForeignKeyProps.Count;
                        if (cnt < fk.PrincipalKeyProps.Count)
                        {
                            cnt = fk.PrincipalKeyProps.Count;
                        }
                        for (int i = 0; i < cnt; i++)
                        {
                            if (fk.ForeignKeyProps[i].OriginalPropertyName == pk.OriginalPropertyName)
                            {
                                scProp =
                                    model.ScalarProperties.Where(p =>
                                    ((p.OriginalPropertyName == fk.PrincipalKeyProps[i].OriginalPropertyName) && (p.ForeignKeyNameChain == fk.NavigationName))).FirstOrDefault();
                            }
                            if (scProp != null) return scProp;
                        }
                    }
                }
            }
            return null;
        }
        /*
        ModelViewPropertyOfVwSerializable GetFirstPropOfFirstUniqueKey(ModelViewSerializable model)
        {
            if (model == null) return null;
            if (model.UniqueKeys == null) return null;
            if (model.UniqueKeys.Count < 1) return null;
            if (model.UniqueKeys[0].UniqueKeyProperties == null) return null;
            if (model.UniqueKeys[0].UniqueKeyProperties.Count < 1) return null;
            return GetScalarPropByOriginaPropName(model.UniqueKeys[0].UniqueKeyProperties[0].OriginalPropertyName, model);
        }
        */
        bool IsUsebByForeignKey(ModelViewSerializable model, ModelViewPropertyOfVwSerializable sclProp)
        {
            if ((model == null) || (sclProp == null)) return false;
            if (model.ForeignKeys == null) return false;
            if (!string.IsNullOrEmpty(sclProp.ForeignKeyName)) return true;
            return model.ForeignKeys.Any(f => f.ForeignKeyProps.Any(p => p.OriginalPropertyName == sclProp.OriginalPropertyName));
        }
        string GetRequiredForeignKeyProps(ModelViewUniqueKeyOfVwSerializable ukofVm, ModelViewSerializable model)
        {
            if ((model == null) || (ukofVm == null)) return null;
            if (ukofVm.UniqueKeyProperties == null) return null;
            if (model.ForeignKeys == null) return "[]";
            if (ukofVm.IsPrimary)
            {
                if (model.PrimaryKeyProperties == null) return null;
            }
            else
            {
                if (model.UniqueKeys.Where(u => u.UniqueKeyName == ukofVm.UniqueKeyName).FirstOrDefault() == null) return null;
            }

            int k = -1;
            List<ModelViewForeignKeySerializable> tmpfklst = null;
            List<ModelViewForeignKeySerializable> fklst = new List<ModelViewForeignKeySerializable>();
            for (int i = 0; i < ukofVm.UniqueKeyProperties.Count; i++)
            {
                ModelViewPropertyOfVwSerializable scPrp = ukofVm.UniqueKeyProperties[i];
                if (string.IsNullOrEmpty(scPrp.ForeignKeyName))
                {
                    tmpfklst = model.ForeignKeys.Where(f => f.ForeignKeyProps.Any(p => (p.OriginalPropertyName == scPrp.OriginalPropertyName))).ToList();
                }
                else
                {
                    if (scPrp.ForeignKeyNameChain != scPrp.ForeignKeyName) return null; // this is incorrect data
                    tmpfklst = model.ForeignKeys.Where(f => f.NavigationName == scPrp.ForeignKeyName).ToList();
                }
                if (tmpfklst.Count < 1)
                {
                    if (fklst.Count < 1) k = i;
                }
                else
                {
                    foreach (ModelViewForeignKeySerializable fk in tmpfklst)
                    {
                        if (!fklst.Any(f => f == fk)) fklst.Add(fk);
                    }
                }
            }
            if (k == -1) return null;
            string result = "";
            foreach (ModelViewForeignKeySerializable fk in fklst)
            {
                foreach (ModelViewKeyPropertySerializable fkp in fk.ForeignKeyProps)
                {
                    ModelViewPropertyOfVwSerializable sp = GetScalarPropByOriginaPropName(fkp.OriginalPropertyName, model);
                    if (sp == null) return null;
                    int ui = ukofVm.UniqueKeyProperties.IndexOf(sp);
                    if (ui < 0) return null;
                    if (ui > k) return null;
                    if (result == "")
                    {
                        result = "\'" + sp.ViewPropertyName + "\'";
                    }
                    else
                    {
                        result = result + ", \'" + sp.ViewPropertyName + "\'";
                    }
                }
            }
            return "[" + result + "]";
        }
        string GeFileNameWithoutExt(ModelViewSerializable model, string fileType)
        {
            string result = "";
            if ((model == null) || string.IsNullOrEmpty(fileType)) return result;
            if (model.CommonStaffs == null) return result;
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
            if (string.IsNullOrEmpty(refItem.FileFolder))
            {
                result = "./";
            }
            else
            {
                result = refItem.FileFolder.Replace("\\", "/").Replace("src/", "").Replace("app/", "./");
            }
            if (!result.EndsWith("/"))
            {
                result += "/";

            }
            return result + refItem.FileName;
        }
        List<string> GetDetailViews(ModelViewSerializable model, DbContextSerializable context, List<string> result)
        {
            if (result == null)
            {
                result = new List<string>();
            }
            if ((model == null) || (context == null))
            {
                return result;
            }
            if ((model.ScalarProperties == null) || (model.PrimaryKeyProperties == null) || (context.ModelViews == null))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1))
            {
                return result;
            }
            List<ModelViewPropertyOfVwSerializable> primKeys = GetModelPrimaryKeyProps(model);
            if (primKeys == null)
            {
                return result;
            }
            if (primKeys.Count != model.PrimaryKeyProperties.Count)
            {
                return result;
            }
            string RootEntityFullClassName = model.RootEntityFullClassName;
            string RootEntityUniqueProjectName = model.RootEntityUniqueProjectName;
            List<ModelViewSerializable> details =
                context.ModelViews.Where(m => m.ForeignKeys.Any(f => (f.NavigationEntityFullName == RootEntityFullClassName) && (f.NavigationEntityUniqueProjectName == RootEntityUniqueProjectName))).ToList();
            if (details.Count < 1)
            {
                return result;
            }
            foreach (ModelViewSerializable detail in details)
            {
                if (detail.ScalarProperties == null) continue;
                if (detail.ForeignKeys == null) continue;
                if (detail.ForeignKeys.Count < 1) continue;
                List<ModelViewForeignKeySerializable> ForeignKeys =
                    detail.ForeignKeys.Where(f => (f.NavigationEntityFullName == RootEntityFullClassName) && (f.NavigationEntityUniqueProjectName == RootEntityUniqueProjectName)).ToList();
                if (ForeignKeys.Count < 1) continue;
                bool canBeUsed = false;
                foreach (ModelViewForeignKeySerializable ForeignKey in ForeignKeys)
                {
                    bool hasForeignKeyProps = true;
                    if (ForeignKey.ForeignKeyProps != null)
                    {
                        for (int i = 0; i < ForeignKey.ForeignKeyProps.Count; i++)
                        {
                            ModelViewKeyPropertySerializable ForeignKeyProp = ForeignKey.ForeignKeyProps[i];
                            if (!(detail.ScalarProperties.Any(s => (s.OriginalPropertyName == ForeignKeyProp.OriginalPropertyName) && (string.IsNullOrEmpty(s.ForeignKeyNameChain)))))
                            {
                                hasForeignKeyProps = false;
                            }
                            if (!hasForeignKeyProps)
                            {
                                ModelViewKeyPropertySerializable PrincipalKeyProp = ForeignKey.PrincipalKeyProps[i];
                                if (detail.ScalarProperties.Any(s => (s.OriginalPropertyName == PrincipalKeyProp.OriginalPropertyName) && (s.ForeignKeyNameChain == ForeignKey.NavigationName)))
                                {
                                    hasForeignKeyProps = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        hasForeignKeyProps = false;
                    }
                    if (hasForeignKeyProps)
                    {
                        canBeUsed = true;
                        break;
                    }
                }
                if (canBeUsed)
                {
                    if (!result.Contains(detail.ViewName))
                    {
                        result.Add(detail.ViewName);
                    }
                }
            }
            return result;
        }
        List<ModelViewForeignKeySerializable> GetDetailViewForeignKeys(ModelViewSerializable model, ModelViewSerializable detail, List<ModelViewForeignKeySerializable> result)
        {
            if (result == null) result = new List<ModelViewForeignKeySerializable>();
            if ((model == null) || (detail == null))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties == null) || (detail.ScalarProperties == null) || (detail.ForeignKeys == null))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1))
            {
                return result;
            }
            List<ModelViewPropertyOfVwSerializable> primKeys = GetModelPrimaryKeyProps(model);
            if (primKeys == null)
            {
                return result;
            }
            if (primKeys.Count != model.PrimaryKeyProperties.Count)
            {
                return result;
            }
            string RootEntityFullClassName = model.RootEntityFullClassName;
            string RootEntityUniqueProjectName = model.RootEntityUniqueProjectName;
            List<ModelViewForeignKeySerializable> ForeignKeys =
                detail.ForeignKeys.Where(f => (f.NavigationEntityFullName == RootEntityFullClassName) && (f.NavigationEntityUniqueProjectName == RootEntityUniqueProjectName)).ToList();
            if (ForeignKeys.Count < 1)
            {
                return result;
            }
            foreach (ModelViewForeignKeySerializable ForeignKey in ForeignKeys)
            {
                bool hasForeignKeyProps = true;
                if (ForeignKey.ForeignKeyProps != null)
                {
                    for (int i = 0; i < ForeignKey.ForeignKeyProps.Count; i++)
                    {
                        ModelViewKeyPropertySerializable ForeignKeyProp = ForeignKey.ForeignKeyProps[i];
                        hasForeignKeyProps =
                            detail.ScalarProperties.Any(s => (s.OriginalPropertyName == ForeignKeyProp.OriginalPropertyName) && (string.IsNullOrEmpty(s.ForeignKeyNameChain)));
                        if (!hasForeignKeyProps)
                        {
                            ModelViewKeyPropertySerializable PrincipalKeyProp = ForeignKey.PrincipalKeyProps[i];
                            hasForeignKeyProps = detail.ScalarProperties.Any(s => (s.OriginalPropertyName == PrincipalKeyProp.OriginalPropertyName) && (s.ForeignKeyNameChain == ForeignKey.NavigationName));
                            {
                                hasForeignKeyProps = true;
                            }
                            if (!hasForeignKeyProps)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    hasForeignKeyProps = false;
                }
                if (hasForeignKeyProps)
                {
                    result.Add(ForeignKey);
//                    break;
                }
            }
            return result;
        }
        bool AllPrimKeyPropsAreForeignKeysProps(ModelViewSerializable m2mMdl)
        {
            if (m2mMdl == null) return false;
            if ((m2mMdl.PrimaryKeyProperties == null) || (m2mMdl.ForeignKeys == null)) return false;
            int cnt = 0;
            foreach (ModelViewForeignKeySerializable fk in m2mMdl.ForeignKeys)
            {
                if (fk.ForeignKeyProps == null) return false;
                cnt += fk.ForeignKeyProps.Count;
            }
            if (cnt != m2mMdl.PrimaryKeyProperties.Count) return false;
            List<string> passed = new List<string>();
            foreach (ModelViewForeignKeySerializable fk in m2mMdl.ForeignKeys)
            {
                foreach (ModelViewKeyPropertySerializable fkp in fk.ForeignKeyProps)
                {
                    if (!m2mMdl.PrimaryKeyProperties.Any(p => p.OriginalPropertyName == fkp.OriginalPropertyName)) return false;
                    if (passed.Any(p => p == fkp.OriginalPropertyName)) return false;
                    passed.Add(fkp.OriginalPropertyName);
                }
            }
            return true;
        }
        bool ForeignKeysOrderedInsidePrimKey(ModelViewSerializable m2mMdl)
        {
            if (m2mMdl == null) return false;
            if ((m2mMdl.PrimaryKeyProperties == null) || (m2mMdl.ForeignKeys == null)) return false;
            ModelViewForeignKeySerializable currFk = null;
            int firstInx = 0;
            int lastInx = 0;
            for (int i = 0; i < m2mMdl.PrimaryKeyProperties.Count; i++)
            {
                ModelViewKeyPropertySerializable pkprp = m2mMdl.PrimaryKeyProperties[i];
                ModelViewForeignKeySerializable fk = m2mMdl.ForeignKeys.Where(f => f.ForeignKeyProps.Any(p => p.OriginalPropertyName == pkprp.OriginalPropertyName)).FirstOrDefault();
                if (fk == null) return false;
                if (i == 0)
                {
                    currFk = fk;
                    continue;
                }
                if (fk == currFk)
                {
                    lastInx = i;
                    continue;
                }
                if (currFk.ForeignKeyProps.Count != lastInx - firstInx + 1) return false;
                currFk = fk;
                firstInx = i;
                lastInx = i;
            }
            if (currFk.ForeignKeyProps.Count != lastInx - firstInx + 1) return false;
            return true;
        }
        bool IsForeignKeyFirstInsidePrimKey(ModelViewSerializable m2mMdl, ModelViewForeignKeySerializable m2mFk)
        {
            if ((m2mMdl == null) || (m2mFk == null)) return false;
            if ((m2mMdl.PrimaryKeyProperties == null) || (m2mFk.ForeignKeyProps == null)) return false;
            ModelViewForeignKeySerializable currFk = null;
            int firstInx = 0;
            int lastInx = 0;
            for (int i = 0; i < m2mMdl.PrimaryKeyProperties.Count; i++)
            {
                ModelViewKeyPropertySerializable pkprp = m2mMdl.PrimaryKeyProperties[i];
                if (m2mFk.ForeignKeyProps.Any(p => p.OriginalPropertyName == pkprp.OriginalPropertyName))
                {
                    if (i == 0)
                    {
                        currFk = m2mFk;
                        continue;
                    }
                    lastInx = i;
                }
                else break;
            }
            if (currFk == null) return false;
            if (currFk.ForeignKeyProps.Count != lastInx - firstInx + 1) return false;
            return true;
        }
        ModelViewForeignKeySerializable GetLastForeignKey(ModelViewSerializable m2mMdl)
        {
            if (m2mMdl == null) return null;
            if ((m2mMdl.PrimaryKeyProperties == null) || (m2mMdl.ForeignKeys == null)) return null;
            if (m2mMdl.PrimaryKeyProperties.Count < 1) return null;
            ModelViewKeyPropertySerializable pkprp = m2mMdl.PrimaryKeyProperties[m2mMdl.PrimaryKeyProperties.Count - 1];
            return m2mMdl.ForeignKeys.Where(f => f.ForeignKeyProps.Any(p => p.OriginalPropertyName == pkprp.OriginalPropertyName)).FirstOrDefault();
        }
        Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>> GetSearchResourcesForLastFk(ModelViewSerializable model, ModelViewSerializable m2mMdl, ModelViewForeignKeySerializable m2mFk, DbContextSerializable context)
        {
            if ((context == null) || (model == null) || (m2mMdl == null) || (m2mFk == null)) return null;
            if ((context.ModelViews == null) || (model.PrimaryKeyProperties == null) || (model.ScalarProperties == null) || (m2mMdl.ForeignKeys == null)) return null;
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1)) return null;
            if (!m2mMdl.ForeignKeys.Any(f => f == m2mFk)) return null;
            if (m2mFk.ViewName != model.ViewName) return null;
            if (!IsTableMatchesIndex(m2mMdl)) return null;
            if (!IsForeigKeyMapedToTailOfPrimKey(m2mFk, m2mMdl)) return null;
            if (!IsForeigKeyMapedToScalarsEx(m2mFk, m2mMdl, model)) return null;
            // m2mMdl - m2mModel, m2mFk - m2mForeignKey,
            List<KeyValuePair<ModelViewForeignKeySerializable, int>> searchFks = null;
            //List<int> searchFkPosition = null;
            foreach (ModelViewForeignKeySerializable searchFk in m2mMdl.ForeignKeys)
            {
                if (m2mFk == searchFk) continue;
                if (!IsOnePropForeigKey(searchFk)) continue;
                ModelViewSerializable searchMdl = context.ModelViews.Where(mv => (mv.ViewName == searchFk.ViewName)).FirstOrDefault();
                if (searchMdl == null) continue;
                if (!IsLookUpTable(searchMdl)) continue;
                if (!IsUniqKeyMapedToScalarsEx(searchMdl.UniqueKeys[0], searchMdl, model)) continue;
                if (searchFks == null) searchFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                searchFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(searchFk, GetForeigKeyMaxPropsPosition(searchFk, m2mMdl)));
            }
            if (searchFks == null) return null;
            int lastValidPosition = m2mMdl.PrimaryKeyProperties.Count - model.PrimaryKeyProperties.Count - 1;
            if (searchFks != null)
            {
                if (searchFks.Any(p => p.Value < 0)) return null;
                searchFks = searchFks.OrderBy(p => p.Value).ToList();
                bool IsCorrect = true;
                for (int i = 0; i < searchFks.Count - 1; i++)
                {
                    IsCorrect = searchFks[i].Value == searchFks[i + 1].Value - 1;
                    if (!IsCorrect) break;
                }
                if (!IsCorrect) return null;
                if (searchFks[searchFks.Count - 1].Value != lastValidPosition) return null;
                lastValidPosition = searchFks[0].Value - 1;
            }
            List<KeyValuePair<ModelViewForeignKeySerializable, int>> otherFks = null;
            foreach (ModelViewForeignKeySerializable otherFk in m2mMdl.ForeignKeys)
            {
                if (m2mFk == otherFk) continue;
                if (searchFks != null)
                {
                    if (searchFks.Any(p => p.Key == otherFk)) continue;
                }
                if (!IsForeigKeyMapedToPrimKey(otherFk, m2mMdl)) continue;
                if (!IsForeigKeyMapedToScalarsExEx(otherFk, m2mMdl, model)) continue;
                if (!IsForeigKeyWithCorrectPropsOrder(otherFk, m2mMdl)) continue;
                int mxPs = GetForeigKeyMaxPropsPosition(otherFk, m2mMdl);
                if ((mxPs < 0) || (mxPs > lastValidPosition)) continue;
                if (otherFks == null) otherFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                otherFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(otherFk, mxPs));
            }
            if (otherFks != null)
            {
                otherFks = otherFks.OrderBy(p => p.Value).ToList();
                bool IsCorrect = true;
                for (int i = 0; i < otherFks.Count - 1; i++)
                {
                    IsCorrect = otherFks[i].Value == otherFks[i + 1].Value - otherFks[i].Key.ForeignKeyProps.Count;
                    if (!IsCorrect) break;
                }
                if (!IsCorrect) return null;
                if (otherFks[otherFks.Count - 1].Value != lastValidPosition) return null;
                lastValidPosition = otherFks[0].Value - otherFks[0].Key.ForeignKeyProps.Count;
            }


            List<KeyValuePair<ModelViewForeignKeySerializable, int>> externalFks = null;
            foreach (ModelViewForeignKeySerializable externalFk in m2mMdl.ForeignKeys)
            {
                if (externalFk == m2mFk) continue;
                if (searchFks != null)
                {
                    if (searchFks.Any(p => p.Key == externalFk)) continue;
                }
                if (otherFks != null)
                {
                    if (otherFks.Any(p => p.Key == externalFk)) continue;
                }
                if (!IsForeigKeyMapedToPrimKey(externalFk, m2mMdl)) continue;
                if (!IsForeigKeyMapedToScalars(externalFk, m2mMdl)) continue;
                if (!IsForeigKeyWithCorrectPropsOrder(externalFk, m2mMdl)) continue;
                int mxPs = GetForeigKeyMaxPropsPosition(externalFk, m2mMdl);
                if ((mxPs < 0) || (mxPs > lastValidPosition)) continue;
                if (externalFks == null) externalFks = new List<KeyValuePair<ModelViewForeignKeySerializable, int>>();
                externalFks.Add(new KeyValuePair<ModelViewForeignKeySerializable, int>(externalFk, mxPs));
            }
            if (externalFks != null)
            {
                externalFks = externalFks.OrderBy(p => p.Value).ToList();
                bool IsCorrect = true;
                for (int i = 0; i < externalFks.Count - 1; i++)
                {
                    IsCorrect = externalFks[i].Value == externalFks[i + 1].Value - externalFks[i].Key.ForeignKeyProps.Count;
                    if (!IsCorrect) break;
                }
                if (!IsCorrect) return null;
                if (externalFks[externalFks.Count - 1].Value != lastValidPosition) return null;
                lastValidPosition = externalFks[0].Value - externalFks[0].Key.ForeignKeyProps.Count;
            }
            if (lastValidPosition != -1) return null;
            int AllFkCount = (searchFks == null ? 0 : searchFks.Count) +
                            (otherFks == null ? 0 : otherFks.Count) +
                            (externalFks == null ? 0 : externalFks.Count);
            if (AllFkCount != (m2mMdl.ForeignKeys.Count - 1)) return null;
            return new Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>>(
                m2mMdl, m2mFk, searchFks, otherFks, externalFks
            );
        }
        Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>> DetailViewAsM2mMdl(ModelViewSerializable m2mMdl, DbContextSerializable context)
        {
            if ((m2mMdl == null) || (context == null)) return null;
            if ((m2mMdl.ForeignKeys == null) || (context.ModelViews == null)) return null;
            if (!IsTableMatchesIndex(m2mMdl)) return null;
            if (!AllPrimKeyPropsAreForeignKeysProps(m2mMdl)) return null;
            if (!ForeignKeysOrderedInsidePrimKey(m2mMdl)) return null;
            ModelViewForeignKeySerializable lastFk = GetLastForeignKey(m2mMdl);
            if (lastFk == null) return null;
            ModelViewSerializable model = context.ModelViews.Where(m => m.ViewName == lastFk.ViewName).FirstOrDefault();
            if (model == null) return null;
            return GetSearchResourcesForLastFk(model, m2mMdl, lastFk, context);
        }
        ModelViewPropertyOfFkSerializable GetScalarPropByPrincipalKeyProp(ModelViewForeignKeySerializable foreignKey, ModelViewKeyPropertySerializable principalKeyProp)
        {
            if ((foreignKey == null) || (principalKeyProp == null))
            {
                return null;
            }
            if (foreignKey.ScalarProperties == null)
            {
                return null;
            }
            return foreignKey.ScalarProperties.Where(p => (p.OriginalPropertyName == principalKeyProp.OriginalPropertyName) && (p.ForeignKeyNameChain == foreignKey.NavigationName)).FirstOrDefault();
        }
        ModelViewPropertyOfVwSerializable GetScalarPropByOriginalPropertyNameAndForeignKeyNameChain(ModelViewSerializable model, string originalPropertyName, string foreignKeyNameChain)
        {
            if ((model == null) || (string.IsNullOrEmpty(originalPropertyName)))
            {
                return null;
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return model.ScalarProperties.Where(p => (p.OriginalPropertyName == originalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain))).FirstOrDefault();
            }
            else
            {
                return model.ScalarProperties.Where(p => (p.OriginalPropertyName == originalPropertyName) && (p.ForeignKeyNameChain == foreignKeyNameChain)).FirstOrDefault();
            }
        }
        ModelViewSerializable GetModelViewByName(DbContextSerializable context, string viewName)
        {
            if ((context == null) || (string.IsNullOrEmpty(viewName)))
            {
                return null;
            }
            if (context.ModelViews == null)
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault();
        }
        string GetExpressionForControlListFilter(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + "OnFilter";
        }
        string GetExpressionForControlListIsOpen(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + "IsOpen";
        }
        string GetExpressionForFormControl(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string mainFormGroupName)
        {
            return mainFormGroupName + ".controls['" + GetTypeScriptPropertyNameEx(prop, model) + "']";
        }
        string GetExpressionForInvalid(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string mainFormGroupName)
        {
            return GetExpressionForFormControl(prop, model, mainFormGroupName) + ".invalid";
        }
        string GetExpressionForInvalidBootstrap(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string mainFormGroupName)
        {
            return GetExpressionForFormControl(prop, model, mainFormGroupName) + ".invalid && " + GetExpressionForFormControl(prop, model, mainFormGroupName) + ".touched";
        }

        string GetExpressionForControlListOpen(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + "OnOpen";
        }
        string GetExpressionForControlInvalid(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + ".invalid";
        }
        string GetExpressionForControlInvalidBootstrap(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + ".invalid && " + GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + ".touched";
        }
        string GetComboControlListPropertyName(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return "NoName";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (mv == null)
            {
                return "NoName";
            }
            ModelViewPropertyOfVwSerializable propForSel =
                mv.ScalarProperties.Where(p => (string.IsNullOrEmpty(p.ForeignKeyNameChain) && p.OriginalPropertyName == prop.OriginalPropertyName)).FirstOrDefault();
            if (propForSel == null)
            {
                return "NoName";
            }
            return GetTypeScriptPropertyName(propForSel, mv);
        }
        string GetTypeaheadControlListPropertyName(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return "NoName";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (mv == null)
            {
                return "NoName";
            }
            ModelViewPropertyOfVwSerializable propForSel =
                mv.ScalarProperties.Where(p => (string.IsNullOrEmpty(p.ForeignKeyNameChain) && p.OriginalPropertyName == prop.OriginalPropertyName)).FirstOrDefault();
            if (propForSel == null)
            {
                return "NoName";
            }
            return GetTypeScriptPropertyName(propForSel, model);
        }
        int GetXXX1Wdth(int w, int inPercentEq100)
        {
            if ((w > 5) && (w < inPercentEq100))
            {
                w -= 1;
            }
            return w;
        }
        int GetGreaterThanPercent(int currCnt, int maxCnt, int[] wdths, int inPercentEq100)
        {
            int result = wdths[1];
            if (currCnt < maxCnt)
            {
                result = wdths[0];
            }
            if (result == 100)
            {
                result = inPercentEq100;
            }
            if ((result > 5) && (result < inPercentEq100)) result -= 1;
            return result;
        }
        bool HasButton(InputTypeEnum inputType)
        {
            if (inputType == InputTypeEnum.SearchDialog)
            {
                return true;
            }
            return false;
        }
        bool IsMemoInput(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            if (!("System.String".Equals(sclrProp.UnderlyingTypeName) || "String".Equals(sclrProp.UnderlyingTypeName)))
            {
                return false;
            }
            if (sclrProp.Attributes != null)
            {
                if (sclrProp.Attributes.Where(a => (a.AttrName == "MaxLength") || (a.AttrName == "StringLength")).Any())
                {
                    return false;
                }
            }
            if (sclrProp.FAPIAttributes != null)
            {
                if (sclrProp.FAPIAttributes.Where(a => a.AttrName == "HasMaxLength").Any())
                {
                    return false;
                }
            }
            return true;
        }
        string GetFormatters(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null) return "";
            string rsltStr = GetAtributeUnNamedValue(sclrProp, "DataType");
            if (string.IsNullOrEmpty(rsltStr)) return "";
            if (rsltStr.Replace("\"", "").ToLower() == "tobinaryformatter")
            {
                return "toBinaryFormatter";
            }
            return "";
        }
        string GetPropertyTypeScriptTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.boolean":
                    result = "boolean";
                    break;
                case "system.guid":
                case "system.string":
                    result = "string";
                    break;
                default:
                    result = "number";
                    break;
            }
            if (prop.IsNullable || (!prop.IsRequiredInView))
            {
                return result + " | null";
            }
            return result;
        }
        string GetPropertyTypeScriptTypeNameEx(ModelViewPropertyOfVwSerializable prop)
        {
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.boolean":
                    result = "boolean";
                    break;
                case "system.guid":
                case "system.string":
                    result = "string";
                    break;
                default:
                    result = "number";
                    break;
            }
            return result;
        }
        string GetJavaScriptToStringMethod(ModelViewPropertyOfVwSerializable prop)
        {
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.datetime":
                    result = ".toString()"; // .toDateString()
                    break;
                case "system.guid":
                case "system.string":
                    result = "";
                    break;
                default:
                    result = ".toString()";
                    break;
            }
            return result;
        }
        string GetPropertyTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return prop.UnderlyingTypeName;
            }
            if (prop.IsNullable || (!prop.IsRequiredInView))
            {
                return prop.UnderlyingTypeName + " ?";
            }
            return prop.UnderlyingTypeName;
        }
        String GetWebApiServicePrefix(ModelViewSerializable model)
        {
            string result = model.WebApiServiceName;
            if (!string.IsNullOrEmpty(result))
            {
                if (result.EndsWith("Controller"))
                {
                    result = result.Substring(0, result.LastIndexOf("Controller"));
                }
                result = result.ToLower();
            }
            return result;
        }
        String GetAbpWebApiServicePrefix(ModelViewSerializable model)
        {
            string result = model.WebApiRoutePrefix;
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Trim(new char[] { '/', ' ' }) + "/" + model.ViewName;
            } else
            {
                result = model.ViewName;
            }
            return result;
        }
        string GetTypeScriptPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return "Noname";
            }
            if (model.GenerateJSonAttribute)
            {
                return prop.JsonPropertyName;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName);
            }
        }
        string GetFilterPropertyOperatorName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model, string operatorSufix)
        {
            if (model.GenerateJSonAttribute)
            {
                return prop.JsonPropertyName + operatorSufix;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName) + operatorSufix;
            }
        }
        string GetAtributeNamedValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName, string attrProp)
        {
            if ((sclrProp != null) && (!(string.IsNullOrEmpty(attrProp))))
            {
                if (sclrProp.Attributes != null)
                {
                    ModelViewAttributeSerializable modelViewAttributeSerializable =
                        sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                    if (modelViewAttributeSerializable != null)
                    {
                        if (modelViewAttributeSerializable.VaueProperties != null)
                        {

                            ModelViewAttributePropertySerializable modelViewAttributePropertySerializable =
                                modelViewAttributeSerializable.VaueProperties.Where(p => (p.PropName == attrProp)).FirstOrDefault();
                            if (modelViewAttributePropertySerializable != null)
                            {
                                return modelViewAttributePropertySerializable.PropValue;
                            }
                        }
                    }
                }
            }
            return null;
        }
        string GetFluentAtributeValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if ((sclrProp != null) && (!(string.IsNullOrEmpty(attrName))))
            {
                if (sclrProp.FAPIAttributes != null)
                {
                    ModelViewFAPIAttributeSerializable modelViewFAPIAttributeSerializable =
                        sclrProp.FAPIAttributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                    if (modelViewFAPIAttributeSerializable != null)
                    {
                        if (modelViewFAPIAttributeSerializable.VaueProperties != null)
                        {

                            ModelViewFAPIAttributePropertySerializable modelViewFAPIAttributePropertySerializable =
                                modelViewFAPIAttributeSerializable.VaueProperties.FirstOrDefault();
                            if (modelViewFAPIAttributePropertySerializable != null)
                            {
                                return modelViewFAPIAttributePropertySerializable.PropValue;
                            }
                        }
                    }
                }
            }
            return null;
        }
        string GetAtributeUnNamedValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if (sclrProp != null)
            {
                if (sclrProp.Attributes != null)
                {
                    ModelViewAttributeSerializable modelViewAttributeSerializable =
                        sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                    if (modelViewAttributeSerializable != null)
                    {
                        if (modelViewAttributeSerializable.VaueProperties != null)
                        {

                            ModelViewAttributePropertySerializable modelViewAttributePropertySerializable =
                                modelViewAttributeSerializable.VaueProperties.Where(p => (string.IsNullOrEmpty(p.PropName) || (p.PropName == "..."))).FirstOrDefault();
                            if (modelViewAttributePropertySerializable != null)
                            {
                                return modelViewAttributePropertySerializable.PropValue;
                            }
                        }
                    }
                }
            }
            return null;
        }
        string GetAtributeValueByNo(ModelViewPropertyOfVwSerializable sclrProp, string attrName, int itemNo)
        {
            if (itemNo > -1)
            {
                if (sclrProp != null)
                {
                    if (sclrProp.Attributes != null)
                    {
                        ModelViewAttributeSerializable modelViewAttributeSerializable =
                            sclrProp.Attributes.Where(a => a.AttrName == attrName).FirstOrDefault();
                        if (modelViewAttributeSerializable != null)
                        {
                            if (modelViewAttributeSerializable.VaueProperties != null)
                            {
                                if (modelViewAttributeSerializable.VaueProperties.Count > itemNo)
                                {
                                    return modelViewAttributeSerializable.VaueProperties[itemNo].PropValue;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        bool HasAtributeWithValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName, string attrVal)
        {
            if ((sclrProp != null) && (!string.IsNullOrEmpty(attrName)) && (!string.IsNullOrEmpty(attrVal)))
            {
                if (sclrProp.Attributes != null)
                {
                    foreach (ModelViewAttributeSerializable a in sclrProp.Attributes)
                    {
                        if (attrName.Equals(a.AttrName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (a.VaueProperties != null)
                            {
                                foreach (ModelViewAttributePropertySerializable v in a.VaueProperties)
                                {
                                    if (!string.IsNullOrEmpty(v.PropValue))
                                    {
                                        if (v.PropValue.ToLower().Contains(attrVal))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        bool HasAtribute(ModelViewPropertyOfVwSerializable sclrProp, string attrName)
        {
            if ((sclrProp != null) && (!string.IsNullOrEmpty(attrName)))
            {
                if (sclrProp.Attributes != null)
                {
                    foreach (ModelViewAttributeSerializable a in sclrProp.Attributes)
                    {
                        if (attrName.Equals(a.AttrName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        bool HasFluentAtribute(ModelViewPropertyOfVwSerializable sclrProp, string[] attrName)
        {
            if ((sclrProp != null) && (attrName != null))
            {
                if ((sclrProp.FAPIAttributes != null) && (attrName.Length > 0))
                {
                    return sclrProp.FAPIAttributes.Any(a => attrName.Contains(a.AttrName));
                }
            }
            return false;
        }
        bool HasFluentAtributeWithValue(ModelViewPropertyOfVwSerializable sclrProp, string attrName, string attrVal)
        {
            if ((sclrProp != null) && (!string.IsNullOrEmpty(attrName)) && (!string.IsNullOrEmpty(attrVal)))
            {
                if (sclrProp.FAPIAttributes != null)
                {
                    foreach (ModelViewFAPIAttributeSerializable a in sclrProp.FAPIAttributes)
                    {
                        if (attrName.Equals(a.AttrName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (a.VaueProperties != null)
                            {
                                foreach (ModelViewFAPIAttributePropertySerializable v in a.VaueProperties)
                                {
                                    if (!string.IsNullOrEmpty(v.PropValue))
                                    {
                                        if (v.PropValue.ToLower().Contains(attrVal))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        List<string> GetValidators(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, Dictionary<string, string> regExps)
        {
            List<string> result = new List<string>();
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return result;
            }
            if (sclrProp.IsRequiredInView)
            {
                result.Add("Validators.required");
            }
            bool hasCurrencyAttr = false;
            if (sclrProp.Attributes != null)
            {
                hasCurrencyAttr = sclrProp.Attributes.Any(a => a.AttrName == "DataType" && a.VaueProperties.Any(p => p.PropValue == "DataType.Currency"));
            }
            string propValue = null;
            switch (sclrProp.UnderlyingTypeName.ToLower())
            {
                case "system.int16":
                case "system.int32":
                case "system.int64":
                case "system.uint16":
                case "system.uint32":
                case "system.uint64":
                    bool hasNoMin = true;
                    bool hasNoMax = true;
                    if (hasCurrencyAttr)
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpCurrency"] + ")");
                    }
                    else
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpInteger"] + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "IntegerValidator", 0);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        hasNoMin = false;
                        result.Add("Validators.min(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "IntegerValidator", 1);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        hasNoMax = false;
                        result.Add("Validators.max(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 0);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        hasNoMin = false;
                        result.Add("Validators.min(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 1);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        hasNoMax = false;
                        result.Add("Validators.max(" + propValue.Replace("\"", "") + ")");
                    }
                    if ((hasNoMin) || (hasNoMax))
                    {
                        switch (sclrProp.UnderlyingTypeName.ToLower())
                        {
                            case "system.int16":
                                if (hasNoMin)
                                {
                                    result.Add("Validators.min(32766)");
                                }
                                if (hasNoMax)
                                {
                                    result.Add("Validators.min(-32766)");
                                }
                                break;
                            case "system.int32":
                                if (hasNoMin)
                                {
                                    result.Add("Validators.max(2147483640)");
                                }
                                if (hasNoMax)
                                {
                                    result.Add("Validators.min(-2147483640)");
                                }
                                break;
                            case "system.uint16":
                                if (hasNoMin)
                                {
                                    result.Add("Validators.max(65534)");
                                }
                                if (hasNoMax)
                                {
                                    result.Add("Validators.min(0)");
                                }
                                break;
                            case "system.uint32":
                                if (hasNoMin)
                                {
                                    result.Add("Validators.max(4294967290)");
                                }
                                if (hasNoMax)
                                {
                                    result.Add("Validators.min(0)");
                                }
                                break;
                        }
                    }
                    break;
                case "system.guid":
                    result.Add("Validators.pattern(" + regExps["RegExpGuid"] + ")");
                    break;
                case "system.double":
                case "system.decimal":
                case "system.single":
                    if (hasCurrencyAttr)
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpCurrency"] + ")");
                    }
                    else
                    {
                        result.Add("Validators.pattern(" + regExps["RegExpFloat"] + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 0);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.min(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeValueByNo(sclrProp, "Range", 1);
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.max(" + propValue.Replace("\"", "") + ")");
                    }
                    break;
                case "system.string":
                    propValue = GetAtributeUnNamedValue(sclrProp, "StringLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.maxLength(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeUnNamedValue(sclrProp, "MaxLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.maxLength(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeUnNamedValue(sclrProp, "MinLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.minLength(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetAtributeNamedValue(sclrProp, "StringLength", "MinimumLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.minLength(" + propValue.Replace("\"", "") + ")");
                    }
                    propValue = GetFluentAtributeValue(sclrProp, "HasMaxLength");
                    if (!string.IsNullOrEmpty(propValue))
                    {
                        result.Add("Validators.maxLength(" + propValue.Replace("\"", "") + ")");
                    }
                    break;
            }
            return result;
        }
        bool HasCombo(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                    (prop.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                    (prop.InputTypeWhenDelete == InputTypeEnum.Combo);
        }
        bool HasButton(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog);
        }
        bool HasTypeahead(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead);
        }
        bool HasInitMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return HasCombo(prop, model) || HasButton(prop, model) || HasTypeahead(prop, model);
        }
        bool HasInitMethodForInputMode(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, int inputType)
        {
            switch (inputType)
            {
                case 1:
                    return
                        (prop.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead);
                case 2:
                    return
                        (prop.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead);
                case 3:
                    return
                        (prop.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                        (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                        (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead);
            }
            return false;
        }
        bool HasModelInitMethodForInputMode(ModelViewSerializable model, int inputType)
        {
            if (model == null)
            {
                return false;
            }
            if (model.UIFormProperties == null)
            {
                return false;
            }
            foreach (ModelViewUIFormPropertySerializable prop in model.UIFormProperties)
            {
                if (HasInitMethodForInputMode(prop, model, inputType))
                {
                    return true;
                }
            }
            return false;
        }
        string GetExpressionForControlList(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameWithSufix(prop, model, sufix) + "Vals";
        }
        string GetExpressionForOnFilterTypeaheadControlListMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string typeaheadSufix)
        {
            return "OnFilter" + GetExpressionForControlList(prop, model, typeaheadSufix);
        }
        string GetExpressionForOnUpdateComboControlListMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string comboSufix)
        {
            return "OnUpdate" + GetExpressionForControlList(prop, model, comboSufix);
        }
        string GetExpressionForOnValChangedMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return "OnValChanged" + GetTypeScriptPropertyNameEx(prop, model);
        }
        string GetTypeScriptPropertyNameWithSufixBase(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyName(prop, model) + sufix;
        }
        string GetTypeScriptPropertyNameWithSufix(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string sufix)
        {
            return GetTypeScriptPropertyNameEx(prop, model) + sufix;
        }
        string GetExpressionForOnInitMethod(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            return "OnInit" + GetTypeScriptPropertyNameEx(prop, model);
        }
        List<string> CollectComboListInterfacesEx(DbContextSerializable context,
                                                ModelViewUIFormPropertySerializable prop,
                                                ModelViewSerializable model,
                                                int currentInputTypeId)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;
            if (currentInputTypeId == 1)
            {
                if (prop.InputTypeWhenAdd == InputTypeEnum.Combo)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            result.Add("Array<" + GetInterfaceName(mv) + ">");
                        }
                    }
                }
            }
            else if (currentInputTypeId == 2)
            {
                if (prop.InputTypeWhenUpdate == InputTypeEnum.Combo)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            intrfsNm = "Array<" + GetInterfaceName(mv) + ">";
                            if (!result.Contains(intrfsNm))
                            {
                                result.Add(intrfsNm);
                            }
                        }
                    }
                }
            }
            else
            {
                if (prop.InputTypeWhenDelete == InputTypeEnum.Combo)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            intrfsNm = "Array<" + GetInterfaceName(mv) + ">";
                            if (!result.Contains(intrfsNm))
                            {
                                result.Add(intrfsNm);
                            }
                        }
                    }
                }
            }
            return result;
        }
        List<string> CollectComboListInterfaces(DbContextSerializable context,
                                                ModelViewUIFormPropertySerializable prop,
                                                ModelViewSerializable model)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;

            if (prop.InputTypeWhenAdd == InputTypeEnum.Combo)
            {
                viewNameForSel = prop.ForeifKeyViewNameForAdd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        result.Add("Array<" + GetInterfaceName(mv) + ">");
                    }
                }
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.Combo)
            {
                viewNameForSel = prop.ForeifKeyViewNameForUpd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = "Array<" + GetInterfaceName(mv) + ">";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.Combo)
            {
                viewNameForSel = prop.ForeifKeyViewNameForDel;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = "Array<" + GetInterfaceName(mv) + ">";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            return result;
        }

        List<string> CollectButtonItemInterfacesEx(DbContextSerializable context,
                                                ModelViewUIFormPropertySerializable prop,
                                                ModelViewSerializable model, int currentInputTypeId)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;
            if (currentInputTypeId == 1)
            {
                if (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            result.Add(GetInterfaceName(mv));
                        }
                    }
                }
            }
            else if (currentInputTypeId == 2)
            {
                if (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            intrfsNm = GetInterfaceName(mv);
                            if (!result.Contains(intrfsNm))
                            {
                                result.Add(intrfsNm);
                            }
                        }
                    }
                }
            }
            else
            {
                if (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            intrfsNm = GetInterfaceName(mv);
                            if (!result.Contains(intrfsNm))
                            {
                                result.Add(intrfsNm);
                            }
                        }
                    }
                }
            }
            return result;
        }
        List<string> CollectButtonItemInterfaces(DbContextSerializable context,
                                                ModelViewUIFormPropertySerializable prop,
                                                ModelViewSerializable model)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;

            if (prop.InputTypeWhenAdd == InputTypeEnum.SearchDialog)
            {
                viewNameForSel = prop.ForeifKeyViewNameForAdd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        result.Add(GetInterfaceName(mv));
                    }
                }
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)
            {
                viewNameForSel = prop.ForeifKeyViewNameForUpd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = GetInterfaceName(mv);
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.SearchDialog)
            {
                viewNameForSel = prop.ForeifKeyViewNameForDel;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = GetInterfaceName(mv);
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            return result;
        }
        List<string> CollectTypeaheadListInterfacesEx(DbContextSerializable context,
                                                ModelViewUIFormPropertySerializable prop,
                                                ModelViewSerializable model,
                                                int currentInputTypeId)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;
            if (currentInputTypeId == 1)
            {
                if (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            result.Add("Observable<Array<" + GetInterfaceName(mv) + ">>");
                        }
                    }
                }
            }
            else if (currentInputTypeId == 2)
            {
                if (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            intrfsNm = "Observable<Array<" + GetInterfaceName(mv) + ">>";
                            if (!result.Contains(intrfsNm))
                            {
                                result.Add(intrfsNm);
                            }
                        }
                    }
                }
            }
            else
            {
                if (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead)
                {
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    if (string.IsNullOrEmpty(viewNameForSel))
                    {
                        viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                    }
                    if (!string.IsNullOrEmpty(viewNameForSel))
                    {
                        mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                        if (mv != null)
                        {
                            intrfsNm = "Observable<Array<" + GetInterfaceName(mv) + ">>";
                            if (!result.Contains(intrfsNm))
                            {
                                result.Add(intrfsNm);
                            }
                        }
                    }
                }
            }
            return result;
        }
        List<string> CollectTypeaheadListInterfaces(DbContextSerializable context,
                                                ModelViewUIFormPropertySerializable prop,
                                                ModelViewSerializable model)
        {
            List<string> result = new List<string>();
            ModelViewSerializable mv = null;
            string intrfsNm = null;
            string viewNameForSel = null;

            if (prop.InputTypeWhenAdd == InputTypeEnum.Typeahead)
            {
                viewNameForSel = prop.ForeifKeyViewNameForAdd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        result.Add("Observable<Array<" + GetInterfaceName(mv) + ">>");
                    }
                }
            }
            if (prop.InputTypeWhenUpdate == InputTypeEnum.Typeahead)
            {
                viewNameForSel = prop.ForeifKeyViewNameForUpd;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = "Observable<Array<" + GetInterfaceName(mv) + ">>";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            if (prop.InputTypeWhenDelete == InputTypeEnum.Typeahead)
            {
                viewNameForSel = prop.ForeifKeyViewNameForDel;
                if (string.IsNullOrEmpty(viewNameForSel))
                {
                    viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
                }
                if (!string.IsNullOrEmpty(viewNameForSel))
                {
                    mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                    if (mv != null)
                    {
                        intrfsNm = "Observable<Array<" + GetInterfaceName(mv) + ">>";
                        if (!result.Contains(intrfsNm))
                        {
                            result.Add(intrfsNm);
                        }
                    }
                }
            }
            return result;
        }
        int GetGreaterThanPercent(int currCnt, int maxCnt, int[] wdths)
        {
            if (currCnt < maxCnt)
            {
                return wdths[0];
            }
            return wdths[1];
        }
        string GetDisplayAttributeValueString(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, string propName)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (sclrProp.Attributes == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributeSerializable attr =
                sclrProp.Attributes.Where(a => a.AttrName == "Display").FirstOrDefault();
            if (attr == null)
            {
                return prop.ViewPropertyName;
            }
            if (attr.VaueProperties == null)
            {
                return prop.ViewPropertyName;
            }
            ModelViewAttributePropertySerializable attrProp =
                attr.VaueProperties.Where(v => v.PropName == propName).FirstOrDefault();
            if (attrProp == null)
            {
                return prop.ViewPropertyName;
            }
            if (string.IsNullOrEmpty(attrProp.PropValue))
            {
                return prop.ViewPropertyName;
            }
            else
            {
                char[] charsToTrim = { '"', ' ' };
                return attrProp.PropValue.Trim(charsToTrim);
            }
        }
        bool IsDateInput(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return "System.DateTime".Equals(sclrProp.UnderlyingTypeName) || "DateTime".Equals(sclrProp.UnderlyingTypeName);
        }
        string GetCommonEnumClassName(DbContextSerializable context, string fileType)
        {
            string result = "";
            if ((context == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (context.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
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
        string GetViewNameByForeignNameChain(DbContextSerializable context, string ViewName, string foreignKeyNameChain)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return "";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (mv == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return ViewName;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (foreignKeys.Length < 1)
            {
                return "";
            }
            ModelViewForeignKeySerializable fk =
                mv.ForeignKeys.Where(f => f.NavigationName == foreignKeys[0]).FirstOrDefault();
            if (fk == null)
            {
                return "";
            }
            if (foreignKeys.Length == 1)
            {
                return GetViewNameByForeignNameChain(context, fk.ViewName, "");
            }
            return GetViewNameByForeignNameChain(context, fk.ViewName, string.Join(".", foreignKeys, 1, foreignKeys.Length - 1));
        }
        ModelViewPropertyOfVwSerializable GetScalarPropByOriginaPropName(string origPropName, ModelViewSerializable model)
        {
            if (string.IsNullOrEmpty(origPropName) || (model == null)) return null;
            if ((model.AllProperties == null) || (model.ScalarProperties == null)) return null;
            ModelViewPropertyOfVwSerializable sprop = model.ScalarProperties.Where(p =>
                    (p.OriginalPropertyName == origPropName) &&
                    string.IsNullOrEmpty(p.ForeignKeyName)).FirstOrDefault();
            if (sprop != null) return sprop;
            if (model.ForeignKeys == null) return null;
            if (model.ForeignKeys.Count < 1) return null;
            foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
            {
                if ((fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) continue;
                if ((fk.PrincipalKeyProps.Count != fk.ForeignKeyProps.Count) || (fk.ForeignKeyProps.Count < 1)) continue;
                for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
                {
                    if (fk.ForeignKeyProps[i].OriginalPropertyName == origPropName)
                    {
                        sprop = model.ScalarProperties.Where(p =>
                            (p.OriginalPropertyName == fk.PrincipalKeyProps[i].OriginalPropertyName) &&
                            (p.ForeignKeyName == fk.NavigationName) &&
                            (p.ForeignKeyName == p.ForeignKeyNameChain)
                        ).FirstOrDefault();
                        if (sprop != null) return sprop;
                    }
                }
            }
            return null;
        }
        string GetPrimKeyFilterForFindIndexMethod(DbContextSerializable context, string ViewName, string srcPrefix, string destPrefix)
        {
            if ((context == null) || (string.IsNullOrEmpty(ViewName)))
            {
                return "false";
            }
            ModelViewSerializable model = context.ModelViews.Where(v => v.ViewName == ViewName).FirstOrDefault();
            if (model == null)
            {
                return "false";
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null))
            {
                return "false";
            }
            string result = "";
            foreach (ModelViewKeyPropertySerializable keyProp in model.PrimaryKeyProperties)
            {
                ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializable = GetScalarPropByOriginaPropName(keyProp.OriginalPropertyName, model);
                if (modelViewPropertyOfVwSerializable != null)
                {
                    string proName = GetTypeScriptPropertyName(modelViewPropertyOfVwSerializable, model);
                    if (result != "")
                    {
                        result += " && ";
                    }
                    result += "(" + srcPrefix + "." + proName + " === " + destPrefix + "." + proName + ")";
                }
            }
            if (result == "")
            {
                return "false";
            }
            return result;
        }
        string GetControlListPropertyName(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return "NoName";
            }
            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (mv == null)
            {
                return "NoName";
            }
            ModelViewPropertyOfVwSerializable propForSel =
                mv.ScalarProperties.Where(p => (string.IsNullOrEmpty(p.ForeignKeyNameChain) && p.OriginalPropertyName == prop.OriginalPropertyName)).FirstOrDefault();
            if (propForSel == null)
            {
                return "NoName";
            }
            return GetTypeScriptPropertyName(propForSel, mv);
        }
        ModelViewSerializable GetViewForControlList(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            if ((prop == null) || (model == null) || (context == null))
            {
                return null;
            }
            string viewNameForSel = "";
            switch (inputType)
            {
                case 1: // add
                    viewNameForSel = prop.ForeifKeyViewNameForAdd;
                    break;
                case 2: // Upd
                    viewNameForSel = prop.ForeifKeyViewNameForUpd;
                    break;
                default: // Del == 3 
                    viewNameForSel = prop.ForeifKeyViewNameForDel;
                    break;
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, prop.ForeignKeyNameChain);
            }
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return null;
            }
            return context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
        }
        string GetViewNameForControlList(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType)
        {
            ModelViewSerializable mv =
                GetViewForControlList(prop, model, context, inputType);
            if (mv == null)
            {
                return "NoName";
            }
            return mv.ViewName;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelPrimaryKeyProps(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null)
            {
                return result;
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null))
            {
                return result;
            }
            foreach (ModelViewKeyPropertySerializable modelViewKeyPropertySerializable in model.PrimaryKeyProperties)
            {
                ModelViewPropertyOfVwSerializable prop = GetScalarPropByOriginaPropName(modelViewKeyPropertySerializable.OriginalPropertyName, model);
                if (prop != null)
                {
                    result.Add(prop);
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetModelForeignKeyProps(ModelViewSerializable model, string detailFkChain, string masterFkChain)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((model == null) || string.IsNullOrEmpty(masterFkChain))
            {
                return result;
            }
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null) || (model.ForeignKeys == null))
            {
                return result;
            }
            if (string.IsNullOrEmpty(detailFkChain))
            {
                detailFkChain = "";
            }
            else
            {
                detailFkChain += ".";
            }
            string[] chain = masterFkChain.Replace(detailFkChain, "").Split(new string[] { "." }, StringSplitOptions.None);
            if (chain.Length < 1)
            {
                return result;
            }
            ModelViewForeignKeySerializable foreignKeySerializable =
                model.ForeignKeys.Where(f => f.NavigationName == chain[0]).FirstOrDefault();
            if (foreignKeySerializable == null)
            {
                return result;
            }
            if ((foreignKeySerializable.ForeignKeyProps == null) || (foreignKeySerializable.PrincipalKeyProps == null))
            {
                return result;
            }
            if (foreignKeySerializable.ForeignKeyProps.Count != foreignKeySerializable.PrincipalKeyProps.Count)
            {
                return result;
            }
            for (int i = 0; i < foreignKeySerializable.ForeignKeyProps.Count; i++)
            {
                ModelViewKeyPropertySerializable modelViewKeyPropertySerializable = foreignKeySerializable.ForeignKeyProps[i];
                ModelViewPropertyOfVwSerializable prop =
                        model.ScalarProperties.Where(p => ((p.OriginalPropertyName == modelViewKeyPropertySerializable.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))).FirstOrDefault();
                if (prop != null)
                {
                    result.Add(prop);
                }
                else
                {
                    modelViewKeyPropertySerializable = foreignKeySerializable.PrincipalKeyProps[i];
                    prop =
                        model.ScalarProperties.Where(p => ((p.OriginalPropertyName == modelViewKeyPropertySerializable.OriginalPropertyName) && (p.ForeignKeyName == foreignKeySerializable.NavigationName))).FirstOrDefault();
                    if (prop != null)
                    {
                        result.Add(prop);
                    }
                }
            }
            return result;
        }
        List<ModelViewPropertyOfVwSerializable> GetPrimaryKeyProps(DbContextSerializable context, string viewName)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if ((context == null) || string.IsNullOrEmpty(viewName))
            {
                return result;
            }
            return GetModelPrimaryKeyProps(context.ModelViews.Where(v => v.ViewName == viewName).FirstOrDefault());
        }
        List<ModelViewUIFormPropertySerializable> GetDirectMasters(ModelViewUIFormPropertySerializable prop,
                             ModelViewSerializable model,
                             DbContextSerializable context, int inputType)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            string viewNameForSel = GetViewNameForControlList(prop, model, context, inputType);
            if (string.IsNullOrEmpty(viewNameForSel))
            {
                return result;
            }
            ModelViewSerializable modelViewSerializable = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
            if (modelViewSerializable == null)
            {
                return result;
            }
            if (modelViewSerializable.ForeignKeys == null)
            {
                return result;
            }
            string foreignKeyNameChain = prop.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                foreignKeyNameChain = "";
            }
            else
            {
                foreignKeyNameChain += ".";
            }
            foreach (ModelViewForeignKeySerializable modelViewForeignKeySerializable in modelViewSerializable.ForeignKeys)
            {
                if (!string.IsNullOrEmpty(modelViewForeignKeySerializable.ViewName))
                {
                    string fltFKNameChain = foreignKeyNameChain + modelViewForeignKeySerializable.NavigationName;
                    List<ModelViewUIFormPropertySerializable> propLst = null;
                    switch (inputType)
                    {
                        case 1:
                            propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                       ((p.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                                        (p.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                                        (p.InputTypeWhenAdd == InputTypeEnum.SearchDialog))).ToList();
                            break;
                        case 2:
                            propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                       ((p.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                                        (p.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                                        (p.InputTypeWhenUpdate == InputTypeEnum.SearchDialog))).ToList();
                            break;
                        case 3:
                            propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                       ((p.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                                        (p.InputTypeWhenDelete == InputTypeEnum.Typeahead) ||
                                        (p.InputTypeWhenDelete == InputTypeEnum.SearchDialog))).ToList();
                            break;
                        default:
                            break;
                    }
                    if (propLst != null)
                    {
                        result.AddRange(propLst);
                    }
                }
            }
            return result;
        }
        List<ModelViewUIFormPropertySerializable> GetDependentScalarProps(ModelViewUIFormPropertySerializable prop,
                                    ModelViewSerializable model,
                                    DbContextSerializable context, int inputType)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            if (!HasInitMethodForInputMode(prop, model, inputType))
            {
                return result;
            }
            string currentPropChain = string.IsNullOrEmpty(prop.ForeignKeyNameChain) ? "" : prop.ForeignKeyNameChain;
            List<ModelViewUIFormPropertySerializable> masters = GetDirectMasters(prop, model, context, inputType);
            foreach (ModelViewUIFormPropertySerializable dependentProp in model.UIFormProperties)
            {
                if (prop.ViewPropertyName == dependentProp.ViewPropertyName)
                {
                    result.Add(dependentProp);
                    continue;
                }
                if (HasInitMethodForInputMode(dependentProp, model, inputType))
                {
                    continue;
                }
                string dependentPropChain = string.IsNullOrEmpty(dependentProp.ForeignKeyNameChain) ? "" : dependentProp.ForeignKeyNameChain;
                if (dependentPropChain == currentPropChain)
                {
                    result.Add(dependentProp);
                    continue;
                }
                string locCurrentPropChain = currentPropChain;
                if (!string.IsNullOrEmpty(locCurrentPropChain)) locCurrentPropChain += ".";
                if (!dependentPropChain.StartsWith(locCurrentPropChain))
                {
                    continue;
                }
                if (!masters.Where(p => dependentPropChain.StartsWith(p.ForeignKeyNameChain)).Any())
                {
                    result.Add(dependentProp);
                }
            }
            return result;
        }
        List<ModelViewUIFormPropertySerializable> GetDirectDetails(ModelViewUIFormPropertySerializable prop,
                             ModelViewSerializable model,
                             DbContextSerializable context, int inputType)
        {
            List<ModelViewUIFormPropertySerializable> result = new List<ModelViewUIFormPropertySerializable>();
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            string foreignKeyNameChain = prop.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return result;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (foreignKeys.Length < 2)
            {
                return result;
            }
            string fltFKNameChain = string.Join(".", foreignKeys, 0, foreignKeys.Length - 1);
            List<ModelViewUIFormPropertySerializable> propLst = null;
            switch (inputType)
            {
                case 1:
                    propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                ((p.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                                (p.InputTypeWhenAdd == InputTypeEnum.Typeahead) ||
                                (p.InputTypeWhenAdd == InputTypeEnum.SearchDialog))).ToList();
                    break;
                case 2:
                    propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                ((p.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                                (p.InputTypeWhenUpdate == InputTypeEnum.Typeahead) ||
                                (p.InputTypeWhenUpdate == InputTypeEnum.SearchDialog))).ToList();
                    break;
                case 3:
                    propLst = model.UIFormProperties.Where(p => (p.ForeignKeyNameChain == fltFKNameChain) &&
                                ((p.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                                (p.InputTypeWhenDelete == InputTypeEnum.Typeahead) ||
                                (p.InputTypeWhenDelete == InputTypeEnum.SearchDialog))).ToList();
                    break;
                default:
                    break;
            }
            if (propLst != null)
            {
                return propLst;
            }
            return result;
        }
        bool MustHaveDirectDetails(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context)
        {
            bool result = false;
            if ((prop == null) || (model == null) || (context == null))
            {
                return result;
            }
            if (model.UIFormProperties == null)
            {
                return result;
            }
            string foreignKeyNameChain = prop.ForeignKeyNameChain;
            if (string.IsNullOrEmpty(foreignKeyNameChain))
            {
                return result;
            }
            string[] foreignKeys = foreignKeyNameChain.Split(new string[] { "." }, StringSplitOptions.None);
            if (foreignKeys.Length < 2)
            {
                return result;
            }
            return true;
        }
        string GetPrimKeyVarName(ModelViewPropertyOfVwSerializable pkpModelViewUIFormPropertySerializable)
        {
            return "pkp" + pkpModelViewUIFormPropertySerializable.ViewPropertyName;
        }
        ModelViewPropertyOfVwSerializable GetOnValChangeViewPropName(DbContextSerializable context, ModelViewSerializable model,
                                    ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable, ModelViewUIFormPropertySerializable dependentScalarProp, int inputType)
        {
            if ((dependentScalarProp == null) || (modelViewUIFormPropertySerializable == null))
            {
                return null;
            }
            ModelViewSerializable view = GetViewForControlList(modelViewUIFormPropertySerializable, model, context, inputType);
            if (view == null)
            {
                return null;
            }
            string foreignKeyNameChain =
                string.IsNullOrEmpty(modelViewUIFormPropertySerializable.ForeignKeyNameChain) ? "" : modelViewUIFormPropertySerializable.ForeignKeyNameChain;


            string dependentForeignKeyNameChain =
                (string.IsNullOrEmpty(dependentScalarProp.ForeignKeyNameChain) ? "" : dependentScalarProp.ForeignKeyNameChain);
            if (foreignKeyNameChain == dependentForeignKeyNameChain)
            {
                dependentForeignKeyNameChain = "";
            }
            else
            {
                if (foreignKeyNameChain != "")
                {
                    foreignKeyNameChain += ".";
                    dependentForeignKeyNameChain = dependentForeignKeyNameChain.Replace(foreignKeyNameChain, "");
                }
            }
            if (string.IsNullOrEmpty(dependentForeignKeyNameChain))
            {
                return
                    view.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain)).FirstOrDefault();
            }
            return
                view.ScalarProperties.Where(p => (p.OriginalPropertyName == dependentScalarProp.OriginalPropertyName) && (p.ForeignKeyNameChain == dependentForeignKeyNameChain)).FirstOrDefault();
        }
        bool HasOnValChangedMethod(DbContextSerializable context, ModelViewSerializable model, ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable)
        {
            bool result = false;
            for (int inputType = 1; inputType < 4; inputType++)
            {
                result =
                    (GetDirectDetails(modelViewUIFormPropertySerializable, model, context, inputType).Count > 0) ||
                    (GetDependentScalarProps(modelViewUIFormPropertySerializable, model, context, inputType).Count > 0);
                if (result)
                {
                    return result;
                }
            }
            return result;
        }
        bool IsBooleanInput(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return "System.Boolean".Equals(sclrProp.UnderlyingTypeName) || "Boolean".Equals(sclrProp.UnderlyingTypeName) || "bool".Equals(sclrProp.UnderlyingTypeName);
        }
        string GetOrderBy(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model, DbContextSerializable context, int inputType, string prefix)
        {
            string propName = GetControlListPropertyName(prop, model, context, inputType);
            if ("Noname".Equals(propName, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(propName))
            {
                return "";
            }
            return prefix + "['orderby']=['" + propName + "'];";
        }
        List<string> GetForeignKeyViewsList(ModelViewSerializable model,
                                    DbContextSerializable context,
                                    int currentInputTypeId,
                                    List<string> fkViewsDict)
        {
            if ((model == null) || (context == null) || (fkViewsDict == null))
            {
                return fkViewsDict;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return fkViewsDict;
            }
            string viewNameForSel = null;
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if (currentInputTypeId == 1)
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Typeahead))
                    {
                        viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd;
                        if (string.IsNullOrEmpty(viewNameForSel))
                        {
                            viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        }
                        if (!string.IsNullOrEmpty(viewNameForSel))
                        {
                            mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                            if (mv != null)
                            {
                                if (!fkViewsDict.Contains(viewNameForSel))
                                {
                                    fkViewsDict.Add(viewNameForSel);
                                }
                            }
                        }
                    }
                }
                else if (currentInputTypeId == 2)
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Typeahead))
                    {
                        viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd;
                        if (string.IsNullOrEmpty(viewNameForSel))
                        {
                            viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        }
                        if (!string.IsNullOrEmpty(viewNameForSel))
                        {
                            mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                            if (mv != null)
                            {
                                if (!fkViewsDict.Contains(viewNameForSel))
                                {
                                    fkViewsDict.Add(viewNameForSel);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Typeahead))
                    {
                        viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel;
                        if (string.IsNullOrEmpty(viewNameForSel))
                        {
                            viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        }
                        if (!string.IsNullOrEmpty(viewNameForSel))
                        {
                            mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                            if (mv != null)
                            {
                                if (!fkViewsDict.Contains(viewNameForSel))
                                {
                                    fkViewsDict.Add(viewNameForSel);
                                }
                            }
                        }
                    }
                }
            }
            return fkViewsDict;
        }
        List<string> GetSearchDialogViewsList(ModelViewSerializable model, DbContextSerializable context, int currentInputTypeId, List<string> sdViewsDict)
        {
            if ((model == null) || (context == null) || (sdViewsDict == null))
            {
                return sdViewsDict;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return sdViewsDict;
            }
            string viewNameForSel = null;
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if (currentInputTypeId == 1)
                {
                    if (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog)
                    {
                        viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForAdd;
                        if (string.IsNullOrEmpty(viewNameForSel))
                        {
                            viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        }
                        if (!string.IsNullOrEmpty(viewNameForSel))
                        {
                            mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                            if (mv != null)
                            {
                                if (!sdViewsDict.Contains(viewNameForSel))
                                {
                                    sdViewsDict.Add(viewNameForSel);
                                }
                            }
                        }
                    }
                }
                else if (currentInputTypeId == 2)
                {
                    if (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog)
                    {
                        viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForUpd;
                        if (string.IsNullOrEmpty(viewNameForSel))
                        {
                            viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        }
                        if (!string.IsNullOrEmpty(viewNameForSel))
                        {
                            mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                            if (mv != null)
                            {
                                if (!sdViewsDict.Contains(viewNameForSel))
                                {
                                    sdViewsDict.Add(viewNameForSel);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog)
                    {
                        viewNameForSel = modelViewUIFormPropertySerializable.ForeifKeyViewNameForDel;
                        if (string.IsNullOrEmpty(viewNameForSel))
                        {
                            viewNameForSel = GetViewNameByForeignNameChain(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        }
                        if (!string.IsNullOrEmpty(viewNameForSel))
                        {
                            mv = context.ModelViews.Where(v => v.ViewName == viewNameForSel).FirstOrDefault();
                            if (mv != null)
                            {
                                if (!sdViewsDict.Contains(viewNameForSel))
                                {
                                    sdViewsDict.Add(viewNameForSel);
                                }
                            }
                        }
                    }
                }
            }
            return sdViewsDict;
        }
        List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> GetForeignKeyNameChainStructList(ModelViewSerializable model,
                                                                                            DbContextSerializable context,
                                                                                            int currentInputTypeId,
                                                                                            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList)
        {
            if ((model == null) || (context == null) || (foreignKeyNameChainList == null))
            {
                return foreignKeyNameChainList;
            }
            if (model.ScalarProperties == null || model.UIFormProperties == null)
            {
                return foreignKeyNameChainList;
            }
            ModelViewSerializable mv = null;
            foreach (ModelViewUIFormPropertySerializable modelViewUIFormPropertySerializable in model.UIFormProperties)
            {
                if (!modelViewUIFormPropertySerializable.IsShownInView) continue;
                if (currentInputTypeId == 1)
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenAdd == InputTypeEnum.Typeahead))
                    {
                        mv = GetViewByForeignNameChainEx(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        foreignKeyNameChainList.Add(new Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>(
                                modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                                mv,
                                modelViewUIFormPropertySerializable,
                                modelViewUIFormPropertySerializable.InputTypeWhenAdd));
                    }
                }
                else if (currentInputTypeId == 2)
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenUpdate == InputTypeEnum.Typeahead))
                    {
                        mv = GetViewByForeignNameChainEx(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        foreignKeyNameChainList.Add(new Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>(
                                modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                                mv,
                                modelViewUIFormPropertySerializable,
                                modelViewUIFormPropertySerializable.InputTypeWhenUpdate));
                    }
                }
                else
                {
                    if ((modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Combo) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.SearchDialog) ||
                        (modelViewUIFormPropertySerializable.InputTypeWhenDelete == InputTypeEnum.Typeahead))
                    {
                        mv = GetViewByForeignNameChainEx(context, model.ViewName, modelViewUIFormPropertySerializable.ForeignKeyNameChain);
                        foreignKeyNameChainList.Add(new Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>(
                                modelViewUIFormPropertySerializable.ForeignKeyNameChain,
                                mv,
                                modelViewUIFormPropertySerializable,
                                modelViewUIFormPropertySerializable.InputTypeWhenDelete));
                    }
                }
            }
            return foreignKeyNameChainList;
        }
        List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> GetCurrentDirectNavList(List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList, string chain)
        {
            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> rslt = new List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>>();
            if (foreignKeyNameChainList == null) return rslt;
            int len = 1;
            string[] fks = null;
            if (!string.IsNullOrEmpty(chain))
            {
                fks = chain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                len = fks.Length + 1;
            }
            foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in foreignKeyNameChainList)
            {
                fks = fk.Item1.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (fks.Length != len) continue;
                if (len == 1)
                {
                    rslt.Add(fk);
                }
                else
                {
                    if (fk.Item1.StartsWith(chain + "."))
                    {
                        rslt.Add(fk);
                    }
                }
            }
            return rslt;
        }
        string GetCurrentDirectNavs(List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList, string chain)
        {
            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> lst = GetCurrentDirectNavList(foreignKeyNameChainList, chain);
            int len = 0;
            if (!string.IsNullOrEmpty(chain))
            {
                len = chain.Length + 1;
            }
            string rslt = "";
            if (len == 0)
            {
                foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in lst)
                {
                    if (rslt != "") rslt += ", ";
                    rslt += "'" + fk.Item1 + "'";
                }
            }
            else
            {
                foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in lst)
                {
                    if (rslt != "") rslt += ", ";
                    rslt += "'" + fk.Item1.Substring(len) + "'";
                }
            }
            return rslt;
        }
        Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> GetDirectDetail(List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList, string chain)
        {
            if (foreignKeyNameChainList == null) return null;
            if (string.IsNullOrEmpty(chain)) return null; // this is RootDataSource
            string[] fks = chain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            int len = fks.Length;
            if (len == 1) return null; // direct detail is RootDataSource
            foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fk in foreignKeyNameChainList)
            {
                if (chain.StartsWith(fk.Item1 + "."))
                {
                    fks = fk.Item1.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fks.Length == len - 1) return fk;
                }
            }
            return null;
        }
        string GetDirectNavName(string chain)
        {
            if (string.IsNullOrEmpty(chain)) return "";
            string[] fks = chain.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return fks[fks.Length - 1];
        }
        bool IsDatabaseGeneratedProperty(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (HasAtribute(prop, "ConcurrencyCheck") || HasAtribute(prop, "Timestamp"))
            {
                return true;
            }
            if (HasAtributeWithValue(prop, "DatabaseGenerated", "identity") || HasAtributeWithValue(prop, "DatabaseGenerated", "computed"))
            {
                return true;
            }
            if (HasFluentAtribute(prop, new string[] { "UseSqlServerIdentityColumn", "ForSqlServerUseSequenceHiLo", "ValueGeneratedOnAdd", "ValueGeneratedOnAddOrUpdate", "IsConcurrencyToken", "IsRowVersion" }))
            {
                return true;
            }
            return HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "identity") || HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "computed");
        }
        bool IsDatabaseGeneratedPropertyEx(ModelViewUIFormPropertySerializable prop, ModelViewSerializable model)
        {
            if ((model == null) || (prop == null))
            {
                return false;
            }
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return IsDatabaseGeneratedProperty(sclrProp, model);
        }
        string GetDefaultVal(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null)
            {
                return "0";
            }
            string result = "";
            switch (prop.UnderlyingTypeName.ToLower())
            {
                case "system.boolean":
                    result = "false";
                    break;
                case "system.guid":
                    result = "'00000000-0000-0000-0000-000000000000'";
                    break;
                case "system.string":
                    result = "'0'";
                    break;
                default:
                    result = "0";
                    break;
            }
            return result;
        }
        // 01504-Uform.component.ts
        // 01504-Uform.component.html
        bool IsMasterDefinedProperty(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            if (!string.IsNullOrEmpty(prop.ForeignKeyName)) return true;
            if (model.ForeignKeys == null) return false;
            foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
            {
                if (fk.ForeignKeyProps != null)
                {
                    if (fk.ForeignKeyProps.Any(p => p.OriginalPropertyName == prop.OriginalPropertyName)) return true;
                }
            }
            return false;
        }
        // 01502-Aform.component.ts
        // 01420-Sform.component.ts
        List<ModelViewPropertyOfVwSerializable> GetDatabaseGeneratedProp(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> rslt = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null) return null;
            if (model.ScalarProperties == null) return null;
            foreach (ModelViewPropertyOfVwSerializable modelViewPropertyOfVwSerializable in model.ScalarProperties)
            {
                if (IsDatabaseGeneratedProperty(modelViewPropertyOfVwSerializable, model))
                {
                    rslt.Add(modelViewPropertyOfVwSerializable);
                }
            }
            return rslt;
        }
        bool IsBooleanTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null) return false;
            return
                "System.Boolean".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) ||
                "Boolean".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) ||
                "bool".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase);
        }
        bool IsGuidTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop == null) return false;
            return
                "System.Guid".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) ||
                "Guid".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase);
        }
        ModelViewUniqueKeyOfVwSerializable GetModelPrimKeyFromList(List<ModelViewUniqueKeyOfVwSerializable> inuniqueKeys)
        {
            if (inuniqueKeys == null) return null;
            return inuniqueKeys.Where(u => u.IsPrimary).FirstOrDefault();
        }
        ModelViewUniqueKeyOfVwSerializable GetModelUniqueKeyByNameFromList(List<ModelViewUniqueKeyOfVwSerializable> inuniqueKeys, string name)
        {
            if (inuniqueKeys == null) return null;
            if (string.IsNullOrEmpty(name))
            {
                return inuniqueKeys.Where(u => string.IsNullOrEmpty(u.UniqueKeyName)).FirstOrDefault();
            }
            else
            {
                return inuniqueKeys.Where(u => u.UniqueKeyName == name).FirstOrDefault();
            }
        }
        ModelViewUniqueKeySerializable GetModelUniqueKeyByNameFromModel(ModelViewSerializable model, string name)
        {
            if (model == null) return null;
            if (model.UniqueKeys == null) return null;
            if (string.IsNullOrEmpty(name))
            {
                return model.UniqueKeys.Where(u => string.IsNullOrEmpty(u.UniqueKeyName)).FirstOrDefault();
            }
            else
            {
                return model.UniqueKeys.Where(u => u.UniqueKeyName == name).FirstOrDefault();
            }
        }
        List<ModelViewForeignKeySerializable> CollectMasterToClientFieldsMap(ModelViewSerializable model, List<string> errors)
        {
            List<ModelViewForeignKeySerializable> rslt = null;
            if (model == null)
            {
                if (errors != null)
                {
                    errors.Add("// CollectMasterToClientFieldsMap: Input param is not defined");
                }
                return rslt;
            }
            if (model.ForeignKeys == null)
            {
                return rslt;
            }
            List<string> passedViews = new List<string>();
            foreach (ModelViewForeignKeySerializable mlFk in model.ForeignKeys)
            {
                if (string.IsNullOrEmpty(mlFk.ViewName))
                {
                    if (errors != null)
                    {
                        errors.Add("//");
                        errors.Add("// Error: CollectMasterToClientFieldsMap:");
                        errors.Add("//        Foreigkey is not completely defined");
                        errors.Add("//        ViewName = " + mlFk.ViewName);
                        errors.Add("//        NavigationName = " + mlFk.NavigationName);
                        errors.Add("//        NavigationEntityName = " + mlFk.NavigationEntityName);
                        errors.Add("//");
                    }
                    continue;
                }
                if (passedViews.Any(e => mlFk.ViewName == e))
                {
                    continue;
                }
                passedViews.Add(mlFk.ViewName);
                List<ModelViewForeignKeySerializable> intlpFks = model.ForeignKeys.Where(f => f.ViewName == mlFk.ViewName).ToList();
                foreach (ModelViewForeignKeySerializable intlpFk in intlpFks)
                {
                    if (
                        string.IsNullOrEmpty(intlpFk.NavigationName) ||
                        string.IsNullOrEmpty(intlpFk.NavigationEntityName) ||
                        string.IsNullOrEmpty(intlpFk.ViewName) ||
                        intlpFk.PrincipalKeyProps == null ||
                        intlpFk.ForeignKeyProps == null
                       )
                    {
                        if (errors != null)
                        {
                            errors.Add("//");
                            errors.Add("// Error: CollectMasterToClientFieldsMap:");
                            errors.Add("//        Foreigkey is not completely defined");
                            errors.Add("//        ViewName = " + intlpFk.ViewName);
                            errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                            errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                            errors.Add("//");
                        }
                        continue;
                    }
                    if (
                        (intlpFk.PrincipalKeyProps.Count != intlpFk.ForeignKeyProps.Count) ||
                        (intlpFk.PrincipalKeyProps.Count < 1) ||
                        (intlpFk.ForeignKeyProps.Count < 1)
                        )
                    {
                        if (errors != null)
                        {
                            errors.Add("//");
                            errors.Add("// Error: CollectMasterToClientFieldsMap:");
                            errors.Add("//        Foreigkey is not completely defined");
                            errors.Add("//        ViewName = " + intlpFk.ViewName);
                            errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                            errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                            errors.Add("//");
                        }
                        continue;
                    }
                    if (rslt == null)
                    {
                        rslt = new List<ModelViewForeignKeySerializable>();
                    }
                    rslt.Add(intlpFk);
                }
            }
            return rslt;
        }
        List<ModelViewForeignKeySerializable> CollectMasterToClientFieldsMapForMasterView(ModelViewSerializable clentModel, string masterViewName, List<string> errors)
        {
            List<ModelViewForeignKeySerializable> rslt = null;
            if (clentModel == null)
            {
                if (errors != null)
                {
                    errors.Add("// CollectMasterToClientFieldsMapForMasterView: Input param is not defined");
                }
                return rslt;
            }
            if ((clentModel.ForeignKeys == null) || (string.IsNullOrEmpty(masterViewName)))
            {
                return rslt;
            }
            List<ModelViewForeignKeySerializable> intlpFks = clentModel.ForeignKeys.Where(f => f.ViewName == masterViewName).ToList();
            foreach (ModelViewForeignKeySerializable intlpFk in intlpFks)
            {
                if (
                    string.IsNullOrEmpty(intlpFk.NavigationName) ||
                    string.IsNullOrEmpty(intlpFk.NavigationEntityName) ||
                    string.IsNullOrEmpty(intlpFk.ViewName) ||
                    intlpFk.PrincipalKeyProps == null ||
                    intlpFk.ForeignKeyProps == null
                    )
                {
                    if (errors != null)
                    {
                        errors.Add("//");
                        errors.Add("// Error: CollectMasterToClientFieldsMapForMasterView:");
                        errors.Add("//        Foreigkey is not completely defined");
                        errors.Add("//        ViewName = " + intlpFk.ViewName);
                        errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                        errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                        errors.Add("//");
                    }
                    continue;
                }
                if (
                    (intlpFk.PrincipalKeyProps.Count != intlpFk.ForeignKeyProps.Count) ||
                    (intlpFk.PrincipalKeyProps.Count < 1) ||
                    (intlpFk.ForeignKeyProps.Count < 1)
                    )
                {
                    if (errors != null)
                    {
                        errors.Add("//");
                        errors.Add("// Error: CollectMasterToClientFieldsMapForMasterView:");
                        errors.Add("//        Foreigkey is not completely defined");
                        errors.Add("//        ViewName = " + intlpFk.ViewName);
                        errors.Add("//        NavigationName = " + intlpFk.NavigationName);
                        errors.Add("//        NavigationEntityName = " + intlpFk.NavigationEntityName);
                        errors.Add("//");
                    }
                    continue;
                }
                if (rslt == null)
                {
                    rslt = new List<ModelViewForeignKeySerializable>();
                }
                rslt.Add(intlpFk);
            }
            return rslt;
        }
        List<ModelViewSerializable> CollectClientToMasterFieldsMapModelViews(ModelViewSerializable model, DbContextSerializable context, List<string> errors)
        {
            List<ModelViewSerializable> rslt = null;
            if ((model == null) || (context == null))
            {
                if (errors != null)
                {
                    errors.Add("// CollectClientToMasterFieldsMapModelViews: Input param is not defined");
                }
                return rslt;
            }
            if (context.ModelViews == null)
            {
                if (errors != null)
                {
                    errors.Add("// CollectClientToMasterFieldsMapModelViews: Input param is not defined : Context.ModelViews is empty");
                }
                return rslt;
            }
            foreach (ModelViewSerializable modelView in context.ModelViews)
            {
                if (modelView.ForeignKeys == null) continue;
                if (modelView.ForeignKeys.Any(f => f.ViewName == model.ViewName))
                {
                    if (rslt == null) rslt = new List<ModelViewSerializable>();
                    rslt.Add(modelView);
                }
            }
            return rslt;
        }
        string GetTypeScriptPropertyNameByKeyProperty(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        {
            ModelViewPropertyOfVwSerializable prop = GetModelScalarPropByKeyProp(model, pk);
            return GetTypeScriptPropertyName(prop, model);
        }
        ModelViewPropertyOfVwSerializable GetScalarPropertyByViewPropertyName(ModelViewSerializable model, string viewPropertyName)
        {
            if ((model == null) || (string.IsNullOrEmpty(viewPropertyName))) return null;
            return model.ScalarProperties.Where(p => p.ViewPropertyName == viewPropertyName).FirstOrDefault();
        }
        string GetTypeScriptPropertyNameByViewPropertyName(ModelViewSerializable model, string viewPropertyName)
        {
            ModelViewPropertyOfVwSerializable prop = GetScalarPropertyByViewPropertyName(model, viewPropertyName);
            return GetTypeScriptPropertyName(prop, model);
        }
        ModelViewSerializable GetModelViewByViewName(DbContextSerializable context, string ViewName)
        {
            if (context == null) return null;
            if (context.ModelViews == null) return null;
            return context.ModelViews.Where(mv => mv.ViewName == ViewName).FirstOrDefault();
        }
        ModelViewEntityPropertySerializable GetRootEntityProperty(ModelViewPropertySerializable prop, ModelViewSerializable model)
        {
            if (string.IsNullOrEmpty(prop.ForeignKeyNameChain))
            {
                if (model.AllProperties != null)
                {
                    return model.AllProperties.Where(p => p.OriginalPropertyName == prop.OriginalPropertyName).FirstOrDefault();
                }
                return null;
            }
            if (model.ForeignKeys == null) return null;
            ModelViewForeignKeySerializable fk = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyNameChain).FirstOrDefault();
            if (fk == null) return null;
            if ((fk.PrincipalKeyProps == null) || (fk.ForeignKeyProps == null)) return null;
            int cnt = fk.PrincipalKeyProps.Count;
            if (cnt > fk.ForeignKeyProps.Count) cnt = fk.ForeignKeyProps.Count;
            for (int i = 0; i < cnt; i++)
            {
                if (fk.PrincipalKeyProps[i].OriginalPropertyName == prop.OriginalPropertyName)
                {
                    return model.AllProperties.Where(p => p.OriginalPropertyName == fk.ForeignKeyProps[i].OriginalPropertyName).FirstOrDefault();
                }
            }
            return null;
        }
        ModelViewUniqueKeyOfVwSerializable GetIndexByEntityProps(List<ModelViewUniqueKeyOfVwSerializable> inuniqueKeys, List<ModelViewKeyPropertySerializable> entityProps, ModelViewSerializable model)
        {
            if ((inuniqueKeys == null) || (entityProps == null)) return null;
            if (entityProps.Count < 1) return null;
            foreach (ModelViewUniqueKeyOfVwSerializable uk in inuniqueKeys)
            {
                if (uk.UniqueKeyProperties == null) continue;
                if (uk.UniqueKeyProperties.Count != entityProps.Count) continue;
                bool isFound = true;
                foreach (ModelViewPropertyOfVwSerializable ukp in uk.UniqueKeyProperties)
                {
                    ModelViewEntityPropertySerializable entityProp = GetRootEntityProperty(ukp, model);
                    if (!entityProps.Any(p => p.OriginalPropertyName == entityProp.OriginalPropertyName))
                    {
                        isFound = false;
                        break;
                    }
                }
                if (isFound) return uk;
            }
            return null;
        }
        ModelViewPropertyOfVwSerializable GetDirectMasterScalarPropertyByViewPropertyName(ModelViewSerializable model, string viewPropertyName, DbContextSerializable context)
        {
            if (context == null) return null;
            ModelViewPropertyOfVwSerializable prop = GetScalarPropertyByViewPropertyName(model, viewPropertyName);
            if ((prop == null) || (model.ForeignKeys == null) || (context.ModelViews == null)) return null;
            if (string.IsNullOrEmpty(prop.ForeignKeyName) || string.IsNullOrEmpty(prop.ForeignKeyNameChain)) return null;
            ModelViewForeignKeySerializable fk = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyName).FirstOrDefault();
            if (fk == null) return null;
            ModelViewSerializable masterVw = context.ModelViews.Where(m => m.ViewName == fk.ViewName).FirstOrDefault();
            if (masterVw == null) return null;
            //ModelViewPropertyOfVwSerializable masterProp = null;
            if (prop.ForeignKeyName == prop.ForeignKeyNameChain)
            {
                return masterVw.ScalarProperties.Where(p => ((p.OriginalPropertyName == prop.OriginalPropertyName) && string.IsNullOrEmpty(p.ForeignKeyNameChain))).FirstOrDefault();
            }
            else
            {
                string flt = "";
                if (prop.ForeignKeyNameChain.StartsWith(prop.ForeignKeyName + "."))
                {
                    flt = prop.ForeignKeyNameChain.Substring(prop.ForeignKeyName.Length + 1);
                }
                return masterVw.ScalarProperties.Where(p => ((p.OriginalPropertyName == prop.OriginalPropertyName) && (p.ForeignKeyNameChain == flt))).FirstOrDefault();
            }
        }
        // 01418-Datasource.class.ts
        bool IsIdentityProperty(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (HasAtribute(prop, "ConcurrencyCheck") || HasAtribute(prop, "Timestamp"))
            {
                return true;
            }
            if (HasAtributeWithValue(prop, "DatabaseGenerated", "identity") || HasAtributeWithValue(prop, "DatabaseGenerated", "computed"))
            {
                return true;
            }
            if (HasFluentAtribute(prop, new string[] { "UseSqlServerIdentityColumn", "ForSqlServerUseSequenceHiLo", "ValueGeneratedOnAdd", "ValueGeneratedOnAddOrUpdate", "IsConcurrencyToken", "IsRowVersion" }))
            {
                return true;
            }
            return HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "identity") || HasFluentAtributeWithValue(prop, "HasDatabaseGeneratedOption", "computed");
        }
        List<ModelViewPropertyOfVwSerializable> GetModelAllUniqueKeysProps(ModelViewSerializable model)
        {
            List<ModelViewPropertyOfVwSerializable> result = new List<ModelViewPropertyOfVwSerializable>();
            if (model == null)
            {
                return result;
            }
            if (model.UniqueKeys == null) return result;
            foreach (ModelViewUniqueKeySerializable uk in model.UniqueKeys)
            {
                List<ModelViewPropertyOfVwSerializable> ukprops = GetModelUniqueKeyProps(model, uk);
                foreach (ModelViewPropertyOfVwSerializable ukp in ukprops)
                {
                    if (!result.Any(p => p == ukp)) result.Add(ukp);
                }
            }
            return result;
        }
        bool IsUsedByForeignKey(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            if (model.ForeignKeys == null) return false;
            if (model.ForeignKeys.Count < 1) return false;
            if (string.IsNullOrEmpty(prop.ForeignKeyName))
            {
                foreach (ModelViewForeignKeySerializable fk in model.ForeignKeys)
                {
                    if (fk.ForeignKeyProps != null)
                    {
                        if (fk.ForeignKeyProps.Any(k => k.OriginalPropertyName == prop.OriginalPropertyName)) return true;
                    }
                }
            }
            else if (prop.ForeignKeyName == prop.ForeignKeyNameChain)
            {
                ModelViewForeignKeySerializable fk01 = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyName).FirstOrDefault();
                if (fk01 == null) return false;
                if ((fk01.PrincipalKeyProps != null) && (fk01.ForeignKeyProps != null))
                {
                    if (fk01.PrincipalKeyProps.Count == fk01.ForeignKeyProps.Count)
                    {
                        if (fk01.PrincipalKeyProps.Any(k => k.OriginalPropertyName == prop.OriginalPropertyName)) return true;
                    }
                }
            }
            return false;
        }
        // 01400-.service.ts


        string GetDashedName(string aName)
        {
            string result = "";
            if (string.IsNullOrEmpty(aName))
            {
                return result;
            }
            StringBuilder sb = new StringBuilder();
            //bool toUpper = true;
            bool isNotFirst = false;
            foreach (char c in aName)
            {
                if (Char.ToUpper(c) == c)
                {
                    if (isNotFirst) sb.Append('-');
                    sb.Append(Char.ToLower(c));
                }
                else
                {
                    sb.Append(c);
                }
                isNotFirst = true;
            }
            return sb.ToString();
        }
        string GetExtForLkUpMsgConsumerDefinitionClassName(ModelViewSerializable model)
        {
            return GetExtForLkUpMsgConsumerClassName(model) + "Definition";
        }
        string GetExtForLkUpMsgConsumerClassName(ModelViewSerializable model)
        {
            return GetExtForLkUpMsgClassName(model) + "Consumer";
        }
        string GetExtForLkUpMsgClassName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUpMsg";
            return result;
        }
        string GetExtForLkUpMsgInterfaceName(ModelViewSerializable model)
        {
            string result = "I";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUpMsg";
            return result;
        }
        string GetExtForLkUpInterfaceName(ModelViewSerializable model)
        {
            string result = "I";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUp";
            return result;
        }
        String GetM2mStaticClassName(ModelViewSerializable model)
        {
            string result = "M2mUpdater";
            if (model == null) return result;
            return result + model.ViewName;
        }
        String GetM2mStaticUpdateMethodName(ModelViewSerializable model)
        {
            string result = "UpdateFor";
            if (model == null) return result;
            return result + model.ViewName;
        }
        String GetM2mStaticSelDictItemMethodName(ModelViewSerializable model)
        {
            string result = "SelDictItemFor";
            if (model == null) return result;
            return result + model.ViewName;
        }
        String GetM2mStaticInsDictItemMethodName(ModelViewSerializable model)
        {
            string result = "InsDictItemFor";
            if (model == null) return result;
            return result + model.ViewName;
        }
        string AttribToString(ModelViewAttributeSerializable attr)
        {
            if (attr == null) return "";
            string result = "[" + attr.AttrName;
            if (attr.VaueProperties == null)
            {
                return result + "]";
            }
            if (attr.VaueProperties.Count < 1)
            {
                return result + "]";
            }
            result = result + "(";
            bool insComma = false;
            foreach (ModelViewAttributePropertySerializable valProp in attr.VaueProperties)
            {
                if (insComma)
                {
                    result = result + ",";
                }
                else
                {
                    insComma = true;
                }
                if (!string.IsNullOrEmpty(valProp.PropName))
                {
                    if (!valProp.PropName.Contains("..."))
                    {
                        result = result + valProp.PropName + "=";
                    }
                }
                result = result + valProp.PropValue;
            }
            return result + ")]";
        }
        bool IsStringPropertyTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) || "String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        bool IsStringPropertyTypeNameEx(ModelViewPropertySerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) || "String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        String GetDestinationNameSpace(ModelViewSerializable model)
        {
            string result = "";
            if (!string.IsNullOrEmpty(model.WebApiServiceFolder))
            {
                result = model.WebApiServiceFolder.Replace("\\", ".");
            }
            if (!string.IsNullOrEmpty(model.WebApiServiceDefaultProjectNameSpace))
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = model.WebApiServiceDefaultProjectNameSpace;
                }
                else
                {
                    result = model.WebApiServiceDefaultProjectNameSpace + "." + result;
                }
            }
            return result;
        }
        String GetDbContextNameSpace(DbContextSerializable context)
        {
            string result = context.DbContextFullClassName;

            if (!string.IsNullOrEmpty(result))
            {
                if (!string.IsNullOrEmpty(context.DbContextClassName))
                {
                    if (result.EndsWith("." + context.DbContextClassName))
                    {
                        result = result.Substring(0, result.LastIndexOf("." + context.DbContextClassName));
                    }
                }
            }
            return result;
        }
        String GetViewModelNameSpace(ModelViewSerializable model)
        {
            string result = "";
            if (!string.IsNullOrEmpty(model.ViewFolder))
            {
                result = model.ViewFolder.Replace("\\", ".");
            }
            if (!string.IsNullOrEmpty(model.ViewDefaultProjectNameSpace))
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = model.ViewDefaultProjectNameSpace;
                }
                else
                {
                    result = model.ViewDefaultProjectNameSpace + "." + result;
                }
            }
            return result;
        }
        String GetRootEntityNameSpace(ModelViewSerializable model)
        {
            return model.RootEntityFullClassName.Substring(0, model.RootEntityFullClassName.LastIndexOf("." + model.RootEntityClassName));
        }
        List<String> GetNavigationPaths(ModelViewSerializable model)
        {
            List<String> locPaths = new List<String>();
            if (model.ScalarProperties == null) return locPaths;
            foreach (ModelViewPropertyOfVwSerializable prop in model.ScalarProperties)
            {
                if (string.IsNullOrEmpty(prop.ForeignKeyNameChain)) continue;
                if (locPaths.Exists(itm => (itm.StartsWith(prop.ForeignKeyNameChain + ".") || (itm.Equals(prop.ForeignKeyNameChain))))) continue;
                string s = locPaths.Where(itm => (prop.ForeignKeyNameChain.StartsWith(itm + ".") || (itm.Equals(prop.ForeignKeyNameChain)))).FirstOrDefault();
                if (!string.IsNullOrEmpty(s))
                {
                    locPaths.Remove(s);
                }
                locPaths.Add(prop.ForeignKeyNameChain);
            }
            return locPaths;
        }
        string GetTypeNameSpace(ModelViewSerializable model, DbContextSerializable context, string refFolder)
        {
            string result = "";
            if ((model == null) || (context == null) || string.IsNullOrEmpty(refFolder))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (context.ModelViews == null))
            {
                return result;
            }
            CommonStaffSerializable refItem = model.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            if (refItem == null) return result;
            if (string.IsNullOrEmpty(refItem.FileFolder)) return refItem.FileDefaultProjectNameSpace;
            return refItem.FileDefaultProjectNameSpace + "." + refItem.FileFolder.Replace(@"\", ".").Replace(@"/", ".");
        }
        String GenerateIncludePaths(String src)
        {
            if (String.IsNullOrEmpty(src)) return "";
            string[] sa = src.Split(new char[] { '.' });
            StringBuilder sb = new StringBuilder(".Include(p => p." + sa[0] + ")");
            for (int i = 1; i < sa.Length; i++)
            {
                sb.Append(".ThenInclude(p => p." + sa[i] + ")");
            }
            return sb.ToString();
        }
        String GetForeignKeyNameChain(String foreignKeyNameChain)
        {
            if (String.IsNullOrEmpty(foreignKeyNameChain))
            {
                return "";
            }
            else
            {
                return foreignKeyNameChain + ".";
            }
        }
        String GetForeignKeyNameChainAndProp(ModelViewPropertyOfVwSerializable sProp, ModelViewSerializable model)
        {
            if (String.IsNullOrEmpty(sProp.ForeignKeyNameChain))
            {
                return sProp.OriginalPropertyName;
            }
            else
            {
                if ((sProp.ForeignKeyNameChain == sProp.ForeignKeyName) && (model.ForeignKeys != null))
                {
                    ModelViewForeignKeySerializable fk = model.ForeignKeys.Where(f => f.NavigationName == sProp.ForeignKeyName).FirstOrDefault();
                    if (fk != null)
                    {
                        if ((fk.ForeignKeyProps != null) && (fk.PrincipalKeyProps != null))
                        {
                            for (int i = 0; i < fk.PrincipalKeyProps.Count; i++)
                            {
                                if (i < fk.ForeignKeyProps.Count)
                                {
                                    if (fk.PrincipalKeyProps[i].OriginalPropertyName == sProp.OriginalPropertyName)
                                    {
                                        return fk.ForeignKeyProps[i].OriginalPropertyName;
                                    }
                                }
                            }
                        }
                    }
                }
                return sProp.ForeignKeyNameChain + "." + sProp.OriginalPropertyName;
            }
        }
        String GetNullableType(ModelViewPropertySerializable prop)
        {
            if (prop.UnderlyingTypeName.Equals("System.String"))
            {
                return prop.UnderlyingTypeName;
            }
            else
            {
                return prop.UnderlyingTypeName + "?";
            }
        }
        String GetChainedPropertyName(ModelViewPropertySerializable prop)
        {
            if (String.IsNullOrEmpty(prop.ForeignKeyNameChain))
            {
                return prop.OriginalPropertyName;
            }
            else
            {
                return prop.ForeignKeyNameChain + "." + prop.OriginalPropertyName;
            }
        }
        bool IsEntityTypeString(ModelViewPropertySerializable prop)
        {
            return prop.UnderlyingTypeName.Equals("System.String");
        }
        bool IsEntityTypeBoolean(ModelViewPropertySerializable prop)
        {
            return prop.UnderlyingTypeName.Equals("System.Boolean");
        }
        String GetFirstPrimKeyProperty(ModelViewSerializable model)
        {
            return model.PrimaryKeyProperties.FirstOrDefault().OriginalPropertyName;
        }
        bool IsInPrimKeyProperty(ModelViewPropertySerializable prop, ModelViewSerializable model)
        {
            if (model.PrimaryKeyProperties == null) return false;
            return model.PrimaryKeyProperties.Any(k => k.OriginalPropertyName == prop.OriginalPropertyName);
        }
        String GetLowerCasePropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            string result = GetTypeScriptPropertyName(prop, model);
            if (!string.IsNullOrEmpty(result))
            {
                result = result.ToLower();
            }
            return result;
        }
        bool IsRootEntityProperty(ModelViewPropertySerializable prop, ModelViewSerializable model)
        {
            return string.IsNullOrEmpty(prop.ForeignKeyNameChain);
        }
        string GetFilterPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (model.GenerateJSonAttribute)
            {
                return prop.JsonPropertyName;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName);
            }
        }
        string GetFkOriginalPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return null;
            if (model.ForeignKeys == null) return null;
            if (model.ForeignKeys.Count < 1) return null;
            if (string.IsNullOrEmpty(prop.ForeignKeyName))
            {
                return prop.OriginalPropertyName;
            }
            else if (prop.ForeignKeyName == prop.ForeignKeyNameChain)
            {
                ModelViewForeignKeySerializable fk01 = model.ForeignKeys.Where(f => f.NavigationName == prop.ForeignKeyName).FirstOrDefault();
                if (fk01 == null) return null;
                if ((fk01.PrincipalKeyProps != null) && (fk01.ForeignKeyProps != null))
                {
                    if (fk01.PrincipalKeyProps.Count == fk01.ForeignKeyProps.Count)
                    {
                        for (int i = 0; i < fk01.PrincipalKeyProps.Count; i++)
                        {
                            if (fk01.PrincipalKeyProps[i].OriginalPropertyName == prop.OriginalPropertyName) return fk01.ForeignKeyProps[i].OriginalPropertyName;
                        }
                    }
                }
            }
            return null;
        }
        List<string> GetCodeToDefineM2mEntityProps(Tuple<ModelViewSerializable, ModelViewForeignKeySerializable, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>, List<KeyValuePair<ModelViewForeignKeySerializable, int>>> sch, ModelViewSerializable model, string ObjInputParamName)
        {
            List<string> rslt = new List<string>();
            if (sch == null) return rslt;
            ModelViewSerializable m2mModel = sch.Item1;
            ModelViewForeignKeySerializable m2mForeignKey = sch.Item2;
            List<KeyValuePair<ModelViewForeignKeySerializable, int>> externalFks = sch.Item5;
            List<KeyValuePair<ModelViewForeignKeySerializable, int>> otherFks = sch.Item4;
            bool addComma = false;
            for (int i = 0; i < m2mForeignKey.ForeignKeyProps.Count; i++)
            {
                addComma = (i < m2mForeignKey.ForeignKeyProps.Count - 1) || (externalFks == null ? false : externalFks.Count > 0) || (otherFks == null ? false : otherFks.Count > 0);
                if (addComma)
                {
                    rslt.Add(m2mForeignKey.ForeignKeyProps[i].OriginalPropertyName + " = " + ObjInputParamName + "." + GetScalarPropByOriginaPropName(m2mForeignKey.PrincipalKeyProps[i].OriginalPropertyName, model).ViewPropertyName + ",");
                }
                else
                {
                    rslt.Add(m2mForeignKey.ForeignKeyProps[i].OriginalPropertyName + " = " + ObjInputParamName + "." + GetScalarPropByOriginaPropName(m2mForeignKey.PrincipalKeyProps[i].OriginalPropertyName, model).ViewPropertyName);
                }
            }
            if (otherFks != null)
            {
                for (int i = 0; i < otherFks.Count; i++)
                {
                    for (int j = 0; j < otherFks[i].Key.ForeignKeyProps.Count; j++)
                    {
                        addComma = (i < otherFks.Count - 1) || (j < otherFks[i].Key.ForeignKeyProps.Count - 1) || (otherFks == null ? false : otherFks.Count > 0);
                        if (addComma)
                        {
                            // scalar prop names are the same for other foreign keys, 
                            // so "newObjInputParamName.GetScalarPropByOriginaPropName(otherFks[i].Key.ForeignKeyProps[j].OriginalPropertyName, m2mModel)" is correct
                            rslt.Add(otherFks[i].Key.ForeignKeyProps[j].OriginalPropertyName + " = " + ObjInputParamName + "." + GetScalarPropByOriginaPropName(otherFks[i].Key.ForeignKeyProps[j].OriginalPropertyName, m2mModel).ViewPropertyName + ",");
                        }
                        else
                        {
                            rslt.Add(otherFks[i].Key.ForeignKeyProps[j].OriginalPropertyName + " = " + ObjInputParamName + "." + GetScalarPropByOriginaPropName(otherFks[i].Key.ForeignKeyProps[j].OriginalPropertyName, m2mModel).ViewPropertyName);
                        }
                    }
                }
            }
            // references to indirect masters or external foreign keys
            if (externalFks != null)
            {
                for (int i = 0; i < externalFks.Count; i++)
                {
                    for (int j = 0; j < externalFks[i].Key.ForeignKeyProps.Count; j++)
                    {
                        // it is not enoughth to check ForeignKeyProps.Count-1, since internal foreign keys
                        addComma = (i < externalFks.Count - 1) || (j < externalFks[i].Key.ForeignKeyProps.Count - 1);
                        if (addComma)
                        {
                            rslt.Add(externalFks[i].Key.ForeignKeyProps[j].OriginalPropertyName + " = " + ObjInputParamName + "." + GetScalarPropByOriginaPropName(externalFks[i].Key.ForeignKeyProps[j].OriginalPropertyName, m2mModel).ViewPropertyName + ",");
                        }
                        else
                        {
                            rslt.Add(externalFks[i].Key.ForeignKeyProps[j].OriginalPropertyName + " = " + ObjInputParamName + "." + GetScalarPropByOriginaPropName(externalFks[i].Key.ForeignKeyProps[j].OriginalPropertyName, m2mModel).ViewPropertyName);
                        }
                    }
                }
            }
            return rslt;
        }
        string GetExtForLkUpClassName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUp";
            return result;
        }
        string GetExtForLkUpConfName(ModelViewSerializable model)
        {
            string result = "";
            if (model != null) result = result + model.ViewName;
            result = result + "ExtForLkUpConf";
            return result;
        }
        bool IsBooleanInputEx(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return "System.Boolean".Equals(sclrProp.UnderlyingTypeName) || "Boolean".Equals(sclrProp.UnderlyingTypeName) || "bool".Equals(sclrProp.UnderlyingTypeName);
        }
        bool IsStringInputEx(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return ("System.String".Equals(sclrProp.UnderlyingTypeName) || "String".Equals(sclrProp.UnderlyingTypeName) || "string".Equals(sclrProp.UnderlyingTypeName));
        }
        bool IsDateTimeInputEx(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((prop == null) || (model == null)) return false;
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null)
            {
                return false;
            }
            return ("System.DateTime".Equals(sclrProp.UnderlyingTypeName) || "DateTime".Equals(sclrProp.UnderlyingTypeName));
        }
        bool IsCurrencyInputEx(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            if (sclrProp == null) return false;
            string rsltStr = GetAtributeUnNamedValue(sclrProp, "DataType");
            if (string.IsNullOrEmpty(rsltStr)) return false;
            return (rsltStr.IndexOf("Currency", StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
        bool IsFileUpload(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            string attrVal = GetAtributeUnNamedValue(prop, "DataType");
            if (string.IsNullOrEmpty(attrVal))
            {
                return false;
            }
            attrVal = attrVal.ToLower();
            return attrVal.Contains("upload");
        }
        bool HasFileUpload(ModelViewSerializable model)
        {
            if (model.ScalarProperties != null)
            {
                foreach (ModelViewPropertyOfVwSerializable prop in model.ScalarProperties)
                {
                    if (string.IsNullOrEmpty(prop.ForeignKeyName))
                    {
                        if (IsFileUpload(prop, model)) return true;
                    }
                }
            }
            return false;
        }
        string GetFileUploadViewPropertyName(ModelViewSerializable model)
        {
            if (model.ScalarProperties != null)
            {
                foreach (ModelViewPropertyOfVwSerializable prop in model.ScalarProperties)
                {
                    if (string.IsNullOrEmpty(prop.ForeignKeyName))
                    {
                        if (IsFileUpload(prop, model)) return prop.ViewPropertyName;
                    }
                }
            }
            return null;
        }
        string GetDataPipe(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            string result = "";
            if ((prop == null) || (model == null))
            {
                return result;
            }
            string attrVal = GetAtributeUnNamedValue(prop, "DataType");
            if (string.IsNullOrEmpty(attrVal))
            {
                return result;
            }
            attrVal = attrVal.ToLower();
            if (attrVal.Contains("currency"))
            {
                return " | currency";
            }
            if (attrVal.Contains("datetime"))
            {
                return " |  date:'medium'";
            }
            if (attrVal.Contains("date"))
            {
                return " | date: 'mediumDate'";
            }
            if (attrVal.Contains("time"))
            {
                return " |  date:'shortTime'";
            }
            return result;
        }
        string GetDataPipeEx2(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            if ((model == null) || (prop == null)) return "";
            ModelViewPropertyOfVwSerializable sclrProp = model.ScalarProperties.Where(p => p.ViewPropertyName == prop.ViewPropertyName).FirstOrDefault();
            return GetDataPipe(sclrProp, model);
        }
        int GetGridColumnWidthEx(ModelViewUIListPropertySerializable prop, ModelViewSerializable model)
        {
            string s = GetDisplayAttributeValueString2(prop, model, "ShortName");
            if (string.IsNullOrEmpty(s)) return 100;
            int sl = s.Length;
            if (IsBooleanInputEx(prop, model))
            {
                return sl > 5 ? sl * 8 : 44;
            }
            if (IsDateTimeInputEx(prop, model))
            {
                return sl > 30 ? sl * 8 : 250;
            }
            if (IsStringInputEx(prop, model))
            {
                s = GetMaxLenEx2(prop, model);
                int sl1 = 0;
                if (int.TryParse(s, out sl1))
                {
                    if (sl1 > 40) sl1 = 40;
                    return sl > sl1 ? sl * 8 : sl1 * 8;
                }
                return sl > 35 ? sl * 8 : 350;
            }
            return sl > 30 ? sl * 8 : 250;
        }
        string kendoSortHeader(ModelViewUIListPropertySerializable modelViewUIListPropertySerializable, ModelViewSerializable model)
        {
            if (hasMatSortHeader(modelViewUIListPropertySerializable, model))
            {
                return "[sortable]=\"true\"";
            }
            return "[sortable]=\"false\"";
        }

        //////////////////////////////////////////////////////////////////////////////
        bool isRoutedItem(AllowedFileTypesSerializable allowedFileTypes, string fileType)
        {
            if ((allowedFileTypes == null) || string.IsNullOrEmpty(fileType))
            {
                return false;
            }
            if (allowedFileTypes.Items == null)
            {
                return false;
            }
            AllowedFileTypeSerializable rslt = allowedFileTypes.Items.Where(i => i.FileType == fileType).FirstOrDefault();
            if (rslt == null)
            {
                return false;
            }
            return rslt.IsRouted;
        }
        AllowedFileTypeSerializable GetAllowedFileType(AllowedFileTypesSerializable allowedFileTypes, string fileType)
        {
            if ((allowedFileTypes == null) || string.IsNullOrEmpty(fileType))
            {
                return null;
            }
            if (allowedFileTypes.Items == null)
            {
                return null;
            }
            return allowedFileTypes.Items.Where(i => i.FileType == fileType).FirstOrDefault();
        }
        string GetFeatureCommonFolderName(FeatureSerializable feature, DbContextSerializable context, string refFolder, string currFolder)
        {
            string result = "./";
            if ((feature == null) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
        string GetFeatureCommonFolderNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, DbContextSerializable context, string refFolder, string currFolder)
        {
            string result = "./";
            if ((feature == null) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
            return GetFeatureCommonFolderName(feature, context, refFolder, currFolder);
        }
        string GetAllFeatureDefaultIsExp(FeatureSerializable Feature)
        {
            if (Feature == null) return "";
            if (Feature.FeatureItems == null) return "";
            string rslt = "false";
            if (Feature.FeatureItems.Count < 2) return rslt;
            for (int i = 1; i < Feature.FeatureItems.Count; i++)
            {
                rslt += ", false";
            }
            return rslt;
        }
        string GetFeatureComponentClassName(FeatureSerializable feature, string fileType)
        {
            string result = "";
            if ((feature == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".component", "Component").Replace(".", "-");
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
        string GetFeatureComponentClassNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, string fileType, string currFolder)
        {
            string result = GetFeatureComponentClassName(feature, fileType);
            if (feature == null)
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);

        }
        string GetFeatureComponentSelectorCommonPart(FeatureSerializable feature, string fileType)
        {
            string result = "";
            if ((feature == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            return refItem.FileName.Replace(".component", "").Replace(".", "-");
        }
        string GetFeatureFolderName(FeatureSerializable feature, string refFolder, string currFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
        string GetFeatureFolderNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, string refFolder, string currFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(currFolder))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
            return GetFeatureFolderName(feature, refFolder, currFolder);
        }
        string GetCommonServiceClassNameForFeatureWithAnglr(AngularJson anglJson, FeatureSerializable feature, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetCommonServiceClassName(context, fileType);
            if (feature == null)
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetModelClassNameForFeatureWithAnglr(AngularJson anglJson, FeatureSerializable feature, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetModelClassName(context, fileType);
            if (feature == null)
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetFeatureModuleClassName(FeatureSerializable feature, string fileType)
        {
            string result = "";
            if ((feature == null) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            if (refItem == null)
            {
                return result;
            }
            if (string.IsNullOrEmpty(refItem.FileName))
            {
                return result;
            }
            string fn = refItem.FileName.Replace(".module", "Module").Replace(".routing", "Routing").Replace(".", "-");
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
        string GetFeatureModuleClassNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, string fileType, string currFolder)
        {
            string result = GetFeatureModuleClassName(feature, fileType);
            if (feature == null)
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetFeatureContextModuleClassNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, DbContextSerializable context, string fileType, string currFolder)
        {
            string result = GetContextModuleClassName(context, fileType);
            if ((feature == null) || (context == null))
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                context.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }
        string GetFeatureComponentFolderName(FeatureSerializable feature, string fileType, string currFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
        string GetFeatureComponentFolderNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, string fileType, string currFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
            return GetFeatureComponentFolderName(feature, fileType, currFolder);
        }
        string GetFeatureCrossComponentFolderName(FeatureSerializable feature, string currFolder, DbContextSerializable context, string refViewName, string refFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(currFolder) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(refViewName))
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.ModelViews == null))
            {
                return result;
            }
            ModelViewSerializable refModel = context.ModelViews.Where(v => v.ViewName == refViewName).FirstOrDefault();
            if (refModel == null)
            {
                return result;
            }
            if (refModel.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                refModel.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
        string GetFeatureCrossComponentFolderNameEx(FeatureSerializable feature, string currFolder, ModelViewSerializable refModel, string refFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(currFolder) || (refModel == null) || string.IsNullOrEmpty(refFolder))
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (refModel.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                refModel.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
        string GetFeatureCrossComponentFolderNameWithAnglr(AngularJson anglJson, FeatureSerializable feature, string currFolder, DbContextSerializable context, string refViewName, string refFolder)
        {
            string result = "./";
            if ((feature == null) || string.IsNullOrEmpty(currFolder) || (context == null) || string.IsNullOrEmpty(refFolder) || string.IsNullOrEmpty(refViewName))
            {
                return result;
            }
            if ((feature.CommonStaffs == null) || (context.ModelViews == null))
            {
                return result;
            }
            ModelViewSerializable refModel = context.ModelViews.Where(v => v.ViewName == refViewName).FirstOrDefault();
            if (refModel == null)
            {
                return result;
            }
            if (refModel.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                refModel.CommonStaffs.Where(c => c.FileType == refFolder).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
            return GetFeatureCrossComponentFolderName(feature, currFolder, context, refViewName, refFolder);
        }
        string GenerateFeatureLoadChildrenImportWithAnglr(AngularJson anglJson, ModelViewSerializable model, string fileType, FeatureSerializable feature, string currFolder)
        {
            string result = "loadChildren: () => import('').then(m => m.)";
            if ((anglJson == null) || (model == null) || (feature == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (feature.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
                    }
                    else if (refAngularProject.ProjectType == "application")
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
                        if (string.IsNullOrEmpty(aliasNm)) aliasNm = appFl;
                        return "loadChildren: () => loadRemoteModule({type: 'manifest', remoteName: '" + refAngularProject.ProjectName + "', exposedModule: '" + aliasNm + "'}).then(m => m." + GetModuleClassName(model, fileType) + ")";
                    }
                }
            }
            return "loadChildren: () => import('" + GetFeatureCrossComponentFolderNameEx(feature, currFolder, model, fileType) + "').then(m => m." + GetModuleClassName(model, fileType) + ")";
        }
        string GenerateFeatureLoadChildrenImportWithAnglrEx(AngularJson anglJson, FeatureSerializable feature, string fileType, string currFolder)
        {
            string result = "loadChildren: () => import('').then(m => m.)";
            if ((anglJson == null) || (feature == null) || string.IsNullOrEmpty(currFolder) || string.IsNullOrEmpty(fileType))
            {
                return result;
            }
            if (feature.CommonStaffs == null)
            {
                return result;
            }
            CommonStaffSerializable refItem =
                feature.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
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
                        return "loadChildren: () => import('" + refAngularProject.ProjectName + "').then(m => m." + GetFeatureModuleClassNameWithAnglr(anglJson, feature, fileType, currFolder) + ")";
                    }
                    else if (refAngularProject.ProjectType == "application")
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
                        if (string.IsNullOrEmpty(aliasNm)) aliasNm = appFl;
                        return "loadChildren: () => loadRemoteModule({type: 'manifest', remoteName: '" + refAngularProject.ProjectName + "', exposedModule: '" + aliasNm + "'}).then(m => m." + GetFeatureModuleClassNameWithAnglr(anglJson, feature, fileType, fileType) + ")"; // this is correct: fileType, fileType
                    }
                }
            }
            return "loadChildren: () => import('" + GetFeatureComponentFolderNameWithAnglr(anglJson, feature, fileType, currFolder) + "').then(m => m." + GetFeatureModuleClassNameWithAnglr(anglJson, feature, fileType, currFolder) + ")";
        }
        string GetModuleClassNameForFeatureWithAnglrEx(AngularJson anglJson, ModelViewSerializable model, string fileType, FeatureSerializable feature, string currFolder)
        {
            string result = GetModuleClassName(model, fileType);
            if ((model == null) || (feature == null))
            {
                return result;
            }
            if ((model.CommonStaffs == null) || (feature.CommonStaffs == null))
            {
                return result;
            }
            CommonStaffSerializable refItem =
                model.CommonStaffs.Where(c => c.FileType == fileType).FirstOrDefault();
            CommonStaffSerializable curItem =
                feature.CommonStaffs.Where(c => c.FileType == currFolder).FirstOrDefault();
            if ((refItem == null) || (curItem == null))
            {
                return result;
            }
            return GetNameByAngularJson(result, anglJson, refItem, curItem);
        }


        string GetDsClearIgnoryFields(ModelViewSerializable RootModel, Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> CurrFk,
                                     List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> foreignKeyNameChainList)
        {
            string rslt = "";
            if ((RootModel == null) || (CurrFk == null) || (foreignKeyNameChainList == null))
            {
                return rslt;
            }
            string currFkChain = CurrFk.Item1;
            if (string.IsNullOrEmpty(currFkChain)) { return rslt; }
            string[] curFks = currFkChain.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (curFks.Length < 1) { return rslt; }
            ModelViewSerializable directDetailModel = null;
            string directDetailFk = null;
            string directDetailChain = "";
            if (curFks.Length < 2)
            {
                directDetailFk = curFks[0];
                directDetailModel = RootModel;
            }
            else
            {
                directDetailFk = curFks[curFks.Length - 1];
                directDetailChain = string.Join(".", curFks, 0, curFks.Length - 1);
                Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> t =
                    foreignKeyNameChainList.Where(itm => itm.Item1 == directDetailChain).FirstOrDefault();
                if (t == null)
                {
                    return rslt;
                }
                else
                {
                    directDetailModel = t.Item2;
                }
            }
            if (directDetailModel is null) return rslt;
            if (directDetailModel.ForeignKeys is null) return rslt;
            ModelViewForeignKeySerializable currFk = directDetailModel.ForeignKeys.Where(f => f.NavigationName == directDetailFk).FirstOrDefault();
            if (currFk is null) return rslt;
            if (currFk.ForeignKeyProps is null) return rslt;
            List<string> flds = new List<string>();
            foreach (ModelViewForeignKeySerializable fk in directDetailModel.ForeignKeys)
            {
                if (fk.NavigationName == directDetailFk) continue;
                if (fk.ForeignKeyProps is null) continue;
                for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
                {
                    string vpn = fk.ForeignKeyProps[i].ViewPropertyName;
                    int k = -1;
                    for (int j = 0; j < currFk.ForeignKeyProps.Count; j++)
                    {
                        if (currFk.ForeignKeyProps[j].ViewPropertyName == vpn)
                        {
                            k = j;
                            break;
                        }
                    }
                    if (k == -1) continue;
                    if (!flds.Any(fld => fld == currFk.PrincipalKeyProps[k].ViewPropertyName))
                    {
                        flds.Add(currFk.PrincipalKeyProps[k].ViewPropertyName);
                    }
                }
            }
            if ((directDetailModel.PrimaryKeyProperties?.Count > 0) && (currFk.ForeignKeyProps?.Count > 0))
            {
                foreach (ModelViewKeyPropertySerializable pkp in directDetailModel.PrimaryKeyProperties)
                {
                    for (int i = 0; i < currFk.ForeignKeyProps.Count; i++)
                    {
                        if (currFk.ForeignKeyProps[i].OriginalPropertyName == pkp.OriginalPropertyName)
                        {
                            if (!flds.Any(fld => fld == currFk.PrincipalKeyProps[i].ViewPropertyName))
                            {
                                flds.Add(currFk.PrincipalKeyProps[i].ViewPropertyName);
                            }
                        }
                    }
                }
            }
            foreach (string lfld in flds)
            {
                ModelViewPropertyOfVwSerializable sclprp = GetScalarPropertyByViewPropertyName(CurrFk.Item2, lfld);
                if (sclprp is null) continue;
                string propNm = GetTypeScriptPropertyName(sclprp, CurrFk.Item2);
                if (string.IsNullOrEmpty(propNm)) continue;
                if (rslt == "") rslt = "'" + propNm + "'"; else rslt += ", '" + propNm + "'";
            }
            return rslt;
        }


        List<Tuple<string, string, ModelViewSerializable>> GetIntersectedForeigKeysMappings(
            string initialForeignKeyNameChain,
            ModelViewSerializable currModel,
            string currForeignKeyNameChain,
            List<string> currMapflds,
            DbContextSerializable context,
            List<Tuple<string, string, ModelViewSerializable>> rslt = null)
        {
            if (rslt == null)
                rslt = new List<Tuple<string, string, ModelViewSerializable>>();
            if (initialForeignKeyNameChain == null) initialForeignKeyNameChain = "";
            if (currForeignKeyNameChain == null) currForeignKeyNameChain = "";
            if ((currModel == null) || (context == null))
            {
                return rslt;
            }
            if ((currMapflds == null) || (currMapflds?.Count < 1))
            {
                if (currModel.ForeignKeys?.Count > 0)
                {
                    if (currModel.ForeignKeys?.Count > 1)
                    {
                        for (int i = 0; i < currModel.ForeignKeys.Count - 1; i++)
                        {
                            if (currModel.ForeignKeys[i].ForeignKeyProps?.Count > 0)
                            {
                                for (int j = i + 1; j < currModel.ForeignKeys.Count; j++)
                                {
                                    if (currModel.ForeignKeys[j].ForeignKeyProps?.Count > 0)
                                    {
                                        List<string> prpNms = new List<string>();
                                        foreach (ModelViewKeyPropertySerializable iprp in currModel.ForeignKeys[i].ForeignKeyProps)
                                        {
                                            if (currModel.ForeignKeys[j].ForeignKeyProps.Any(p => p.OriginalPropertyName == iprp.OriginalPropertyName))
                                                prpNms.Add(iprp.OriginalPropertyName);
                                        }
                                        if (prpNms.Count > 0)
                                        {
                                            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == currModel.ForeignKeys[i].ViewName).FirstOrDefault();
                                            string nfkch = currForeignKeyNameChain;
                                            if (nfkch != "") nfkch += ".";
                                            nfkch += currModel.ForeignKeys[i].NavigationName;
                                            GetIntersectedForeigKeysMappings(currForeignKeyNameChain, mv, nfkch, prpNms, context, rslt);
                                            nfkch = currForeignKeyNameChain;
                                            if (nfkch != "") nfkch += ".";
                                            nfkch += currModel.ForeignKeys[j].NavigationName;
                                            GetIntersectedForeigKeysMappings(currForeignKeyNameChain, mv, nfkch, prpNms, context, rslt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (currModel.PrimaryKeyProperties?.Count > 0)
                    {
                        foreach (ModelViewKeyPropertySerializable pkp in currModel.PrimaryKeyProperties)
                        {
                            //pk.OriginalPropertyName
                            foreach (var fk in currModel.ForeignKeys)
                            {
                                List<string> prpNms = new List<string>();
                                for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
                                {
                                    if (fk.ForeignKeyProps[i].OriginalPropertyName == pkp.OriginalPropertyName)
                                    {
                                        prpNms.Add(fk.PrincipalKeyProps[i].OriginalPropertyName);
                                    }
                                }
                                if (prpNms.Count > 0)
                                {
                                    ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == fk.ViewName).FirstOrDefault();
                                    string nfkch = currForeignKeyNameChain;
                                    if (nfkch != "") nfkch += ".";
                                    nfkch += fk.NavigationName;
                                    GetIntersectedForeigKeysMappings(currForeignKeyNameChain, mv, nfkch, prpNms, context, rslt);
                                }
                            }
                        }
                    }
                    if (currModel.UniqueKeys?.Count > 0)
                    {
                        foreach (ModelViewUniqueKeySerializable uk in currModel.UniqueKeys)
                        {
                            if (uk.UniqueKeyProperties?.Count > 0)
                            {
                                foreach (ModelViewKeyPropertySerializable pkp in uk.UniqueKeyProperties)
                                {
                                    //pk.OriginalPropertyName
                                    foreach (var fk in currModel.ForeignKeys)
                                    {
                                        List<string> prpNms = new List<string>();
                                        for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
                                        {
                                            if (fk.ForeignKeyProps[i].OriginalPropertyName == pkp.OriginalPropertyName)
                                            {
                                                prpNms.Add(fk.PrincipalKeyProps[i].OriginalPropertyName);
                                            }
                                        }
                                        if (prpNms.Count > 0)
                                        {
                                            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == fk.ViewName).FirstOrDefault();
                                            string nfkch = currForeignKeyNameChain;
                                            if (nfkch != "") nfkch += ".";
                                            nfkch += fk.NavigationName;
                                            GetIntersectedForeigKeysMappings(currForeignKeyNameChain, mv, nfkch, prpNms, context, rslt);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return rslt;
            }

            bool shouldBeAdded = false;
            if (currModel.PrimaryKeyProperties?.Count > 0)
            {
                foreach (ModelViewKeyPropertySerializable pkp in currModel.PrimaryKeyProperties)
                {
                    shouldBeAdded = currMapflds.Contains(pkp.OriginalPropertyName);
                    if (!shouldBeAdded) break;
                }
                if ((!shouldBeAdded) && (currModel.UniqueKeys?.Count > 0))
                {
                    foreach (ModelViewUniqueKeySerializable uk in currModel.UniqueKeys)
                    {
                        if (uk.UniqueKeyProperties?.Count > 0)
                        {
                            foreach (ModelViewKeyPropertySerializable ukp in uk.UniqueKeyProperties)
                            {
                                shouldBeAdded = currMapflds.Contains(ukp.OriginalPropertyName);
                                if (!shouldBeAdded) break;
                            }

                        }
                        if (shouldBeAdded) break;
                    }
                }
            }
            if (shouldBeAdded)
            {
                if (!rslt.Any(r => (r.Item1 == initialForeignKeyNameChain) && (r.Item2 == currForeignKeyNameChain) && (r.Item3 == currModel)))
                    rslt.Add(new Tuple<string, string, ModelViewSerializable>(initialForeignKeyNameChain, currForeignKeyNameChain, currModel));
            }
            if (currModel.ForeignKeys?.Count > 0)
            {
                foreach (ModelViewForeignKeySerializable fk in currModel.ForeignKeys)
                {
                    if (fk.ForeignKeyProps?.Count > 0)
                    {
                        List<string> opnms = new List<string>();
                        for (int i = 0; i < fk.ForeignKeyProps.Count; i++)
                        {
                            if (currMapflds.Contains(fk.ForeignKeyProps[i].OriginalPropertyName))
                            {
                                if (!opnms.Contains(fk.PrincipalKeyProps[i].OriginalPropertyName))
                                {
                                    opnms.Add(fk.PrincipalKeyProps[i].OriginalPropertyName);
                                }
                            }
                        }
                        if (opnms.Count > 0)
                        {
                            ModelViewSerializable mv = context.ModelViews.Where(v => v.ViewName == fk.ViewName).FirstOrDefault();
                            string nfkch = currForeignKeyNameChain;
                            if (nfkch != "") nfkch += ".";
                            nfkch += fk.NavigationName;
                            GetIntersectedForeigKeysMappings(initialForeignKeyNameChain, mv, nfkch, opnms, context, rslt);
                        }
                    }
                }
            }
            return rslt;
        }

        List<Tuple<string, string, ModelViewSerializable>> GetIntersectedForeigKeysMappingsEx(
            ModelViewSerializable model,
            DbContextSerializable context,
            List<Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum>> ForeignKeyNameChainList,
            List<Tuple<string, string, ModelViewSerializable>> rslt = null)
        {
            if (rslt == null)
                rslt = new List<Tuple<string, string, ModelViewSerializable>>();

            if ((model == null) || (context == null) || (ForeignKeyNameChainList == null))
            {
                return rslt;
            }
            foreach (Tuple<string, ModelViewSerializable, ModelViewUIFormPropertySerializable, InputTypeEnum> fkcl in ForeignKeyNameChainList)
            {
                GetIntersectedForeigKeysMappings(
                            null,
                            fkcl.Item2,
                            fkcl.Item1,
                            null,
                            context,
                            rslt);
            }
            return rslt;
        }

        bool CheckRequireTree(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ForeignKeys == null) return rslt;
            return model.ForeignKeys.Any(p => p.ViewName == model.ViewName); 
        }
        string RequireTreeNavName(ModelViewSerializable model)
        {
            string rslt = "";
            if (model == null) return rslt;
            if (model.ForeignKeys == null) return rslt;
            ModelViewForeignKeySerializable fk =  model.ForeignKeys.Where(p => p.ViewName == model.ViewName).FirstOrDefault();
            if (fk == null) return rslt;
            return fk.NavigationName;
        }

        ModelViewFunSerializable GetConstructorWithMaxInputParams(ModelViewSerializable model)
        {
            if (model == null)  return null;
            if (model.RootEntityFunctions == null) return null;
            ModelViewFunSerializable rslt = null;
            foreach(ModelViewFunSerializable func in model.RootEntityFunctions)
            {
                if (!func.IsConstructor) continue;
                if (func.FunParams == null) continue;
                if (func.FunParams.Count < 1) continue;
                if (rslt == null)
                {
                    rslt = func;
                    continue;
                }
                if (rslt.FunParams.Count < func.FunParams.Count)
                {
                    rslt = func;
                }
            }
            return rslt;
        }
        ModelViewFunSerializable GetFuncWithMaxInputParams(ModelViewSerializable model, string funcName)
        {
            if (model == null) return null;
            if (model.RootEntityFunctions == null) return null;
            ModelViewFunSerializable rslt = null;
            foreach(ModelViewFunSerializable  func in model.RootEntityFunctions)
            {
                if (func.IsConstructor) continue;
                if (func.FunParams == null) continue;
                if (func.FunParams.Count < 1) continue;
                if (!string.IsNullOrEmpty(funcName))
                {
                    if (!funcName.Equals(func.FunName,StringComparison.OrdinalIgnoreCase)) continue;
                }
                if (rslt == null)
                {
                    rslt = func;
                    continue;
                }
                if (rslt.FunParams.Count < func.FunParams.Count)
                {
                    rslt = func;
                }
            }
            return rslt;
        }


        ModelViewPropertyOfVwSerializable GetMappedScalarProp(ModelViewSerializable model, string ChangeValPrmPrefix, string prmName)
        {
            if ((model == null) || (string.IsNullOrEmpty(prmName))) return null;
            if (model.ScalarProperties == null) return null;
            foreach(ModelViewPropertyOfVwSerializable prop in model.ScalarProperties)
            {
                ModelViewEntityPropertySerializable entityProp = GetRootEntityProperty(prop, model);
                if (entityProp == null) continue;
                if (prmName.Equals(entityProp.OriginalPropertyName, StringComparison.OrdinalIgnoreCase) ||
                    prmName.Equals(ChangeValPrmPrefix + entityProp.OriginalPropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return prop;
                }
            }
            return null;
        }


        bool HasDescriptionAttribute(ModelViewPropertyOfVwSerializable sp, string prp)
        {
            if (sp == null) return false;
            if (sp.Attributes == null) return false;
            List<ModelViewAttributeSerializable> attrs = sp.Attributes.Where(a => a.AttrName == "Description").ToList();
            if (attrs == null) return false;
            // string flt1 = null;
            // string flt2 = null;
            if (string.IsNullOrEmpty(prp))
            {
                foreach (ModelViewAttributeSerializable a in attrs)
                {
                    if (a.VaueProperties != null)
                    {
                        if (a.VaueProperties.Any(v => v.PropValue is null || v.PropValue == "")) return true;
                    }
                }
            }
            else
            {
                string flt = "\"" + prp + "\"";
                foreach (ModelViewAttributeSerializable a in attrs)
                {
                    if (a.VaueProperties != null)
                    {
                        if (a.VaueProperties.Any(v => v.PropValue == flt || v.PropValue == prp)) return true;
                    }
                }
            }
            return false;
        }
        bool HasDescriptionAttribute(ModelViewEntityPropertySerializable sp, string prp)
        {
            if (sp == null) return false;
            if (sp.Attributes == null) return false;
            List<ModelViewAttributeSerializable> attrs = sp.Attributes.Where(a => a.AttrName == "Description").ToList();
            if (attrs == null) return false;
            // string flt1 = null;
            // string flt2 = null;
            if (string.IsNullOrEmpty(prp))
            {
                foreach (ModelViewAttributeSerializable a in attrs)
                {
                    if (a.VaueProperties != null)
                    {
                        if (a.VaueProperties.Any(v => v.PropValue is null || v.PropValue == "")) return true;
                    }
                }
            }
            else
            {
                string flt = "\"" + prp + "\"";
                foreach (ModelViewAttributeSerializable a in attrs)
                {
                    if (a.VaueProperties != null)
                    {
                        if (a.VaueProperties.Any(v => v.PropValue == flt || v.PropValue == prp)) return true;
                    }
                }
            }
            return false;
        }
        List<ModelViewEntityPropertySerializable> GetDictCommonUniqueKeyProps(ModelViewUniqueKeySerializable UniqueKey, ModelViewSerializable searchMdl)
        {
            List<ModelViewEntityPropertySerializable> rslt = null;
            if ((searchMdl == null) || (UniqueKey == null)) return rslt;
            if ((searchMdl.ScalarProperties == null) || (searchMdl.UniqueKeys == null)) return rslt;
            ModelViewUniqueKeySerializable uk = searchMdl.UniqueKeys.Where(p => p.UniqueKeyName == UniqueKey.UniqueKeyName).FirstOrDefault();
            if (uk == null) return rslt;
            if (uk.UniqueKeyProperties == null) return rslt;
            if (uk.UniqueKeyProperties.Count < 1) return rslt;
            foreach (ModelViewKeyPropertySerializable prop in uk.UniqueKeyProperties)
            {
                if (prop == null) continue;
                ModelViewEntityPropertySerializable sp = searchMdl.AllProperties.FirstOrDefault(p => (p.OriginalPropertyName == prop.OriginalPropertyName));
                if (sp == null) continue;
                if (HasDescriptionAttribute(sp, "DictHelper")) continue;
                if (rslt == null) rslt = new List<ModelViewEntityPropertySerializable>();
                rslt.Add(sp);
            }
            return rslt;
        }
        List<ModelViewPropertyOfVwSerializable> GetDictCommonScalarProps(ModelViewSerializable searchMdl)
        {
            List<ModelViewPropertyOfVwSerializable> rslt = null;
            if (searchMdl == null) return rslt;
            if (searchMdl.ScalarProperties == null) return rslt;
            foreach (ModelViewPropertyOfVwSerializable sp in searchMdl.ScalarProperties)
            {
                if (sp == null) continue;
                if (!string.IsNullOrEmpty(sp.ForeignKeyNameChain)) continue;
                if (HasDescriptionAttribute(sp, "DictHelper")) continue;
                if (rslt == null) rslt = new List<ModelViewPropertyOfVwSerializable>();
                rslt.Add(sp);
            }
            return rslt;
        }
        bool IsLookUpTable(ModelViewSerializable searchMdl)
        {
            if (searchMdl == null) return false;
            if ((searchMdl.ScalarProperties == null) || (searchMdl.PrimaryKeyProperties == null) || (searchMdl.UniqueKeys == null)) return false;
            if ((searchMdl.PrimaryKeyProperties.Count != 1) || (searchMdl.UniqueKeys.Count != 1)) return false;
            List<ModelViewPropertyOfVwSerializable> csps = GetDictCommonScalarProps(searchMdl);
            if (csps == null) return false;
            if (csps.Count != 2) return false;
            List<ModelViewEntityPropertySerializable> cukps = GetDictCommonUniqueKeyProps(searchMdl.UniqueKeys[0], searchMdl);
            if (cukps == null) return false;
            if (cukps.Count != 1) return false;
            if (csps[0].OriginalPropertyName == cukps[0].OriginalPropertyName) return false;
            if (GetScalarPropByOriginaPropName(searchMdl.PrimaryKeyProperties[0].OriginalPropertyName, searchMdl) == null) return false;
            return true;
        }
        //
        // searchUk is a unique key of the searchModel
        //
        bool IsUniqKeyMapedToScalarsEx(ModelViewUniqueKeySerializable searchUk, ModelViewSerializable searchModel, ModelViewSerializable model)
        {
            if ((searchModel == null) || (searchUk == null) || (model == null)) return false;
            if ((searchModel.ScalarProperties == null) || (searchUk.UniqueKeyProperties == null) || (model.ScalarProperties == null)) return false;
            List<ModelViewEntityPropertySerializable> cukps = GetDictCommonUniqueKeyProps(searchUk, searchModel);
            if (cukps == null) return false;
            if ((cukps.Count < 1) || (model.ScalarProperties.Count < 1)) return false;
            foreach (ModelViewEntityPropertySerializable ukp in cukps)
            {
                ModelViewPropertyOfVwSerializable sprp = GetScalarPropByOriginaPropName(ukp.OriginalPropertyName, searchModel);
                if (sprp == null) return false;
                if (!model.ScalarProperties.Any(p => p.ViewPropertyName == sprp.ViewPropertyName)) return false;
            }
            return true;
        }
        ModelViewPropertyOfVwSerializable GetFirstPropOfFirstUniqueKey(ModelViewSerializable model)
        {
            if (model == null) return null;
            if (model.UniqueKeys == null) return null;
            if (model.UniqueKeys.Count < 1) return null;
            if (model.UniqueKeys[0].UniqueKeyProperties == null) return null;
            if (model.UniqueKeys[0].UniqueKeyProperties.Count < 1) return null;
            List<ModelViewEntityPropertySerializable> cukps = GetDictCommonUniqueKeyProps(model.UniqueKeys[0], model);
            if(cukps == null) return null;
            // foreach (ModelViewKeyPropertySerializable ukProp in model.UniqueKeys[0].UniqueKeyProperties)
            foreach (ModelViewEntityPropertySerializable ukProp in cukps)
            {
                return GetScalarPropByOriginaPropName(ukProp.OriginalPropertyName, model);
            }
            return null;
        }

    }

}
