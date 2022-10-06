using CS82ANGULAR.Model.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{

    public class ContextLevelTemplateCode
    {
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


    }
}
