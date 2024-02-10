﻿using CS82ANGULAR.Model.Serializable;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

namespace CS82ANGULAR.Model
{




    public class EntityLevelTemplateCode
    {

        string GetPropertyTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if (prop.IsNullable || (!prop.IsRequiredInView))
            {
                return prop.UnderlyingTypeName + " ?";
            }
            return prop.UnderlyingTypeName;
        }
        bool IsStringPropertyTypeName(ModelViewPropertyOfVwSerializable prop)
        {
            if ("System.String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase) || "String".Equals(prop.UnderlyingTypeName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        string GetModelNameSpace(ModelViewSerializable model)
        {
            string result = model.ViewFolder;
            if (string.IsNullOrEmpty(result))
            {
                result = "";
            }
            else
            {
                result = "." + result.Replace("\\", ".");
            }
            return model.ViewDefaultProjectNameSpace + result;
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
        string GetTypeScriptPropertyName(ModelViewPropertyOfVwSerializable prop, ModelViewSerializable model)
        {
            if (model.GenerateJSonAttribute)
            {
                return prop.ViewPropertyName;
            }
            else
            {
                return FirstLetterToLower(prop.ViewPropertyName);
            }
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






        /// ///////////////////////////////////////////////////////////////////////////////////
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
        String GetDomainViewModelNameSpace(ModelViewSerializable model)
        {
            if (model.GeneratedDtos != null)
            {
                GeneratedDtoSerializable gds = model.GeneratedDtos.FirstOrDefault(d => d.ViewType == "Domain");
                if (gds != null)
                {
                    return gds.ViewDefaultProjectNameSpace;
                }
            }
            return "";
        }
        String GetRepoInterfaceNameSpace(ModelViewSerializable model)
        {
            if (model.GeneratedServices != null)
            {
                GeneratedServiceSerializable gss = model.GeneratedServices.FirstOrDefault(d => d.SrvType == "IRepo");
                if (gss != null)
                {
                    string result = "";
                    if (!string.IsNullOrEmpty(gss.SrvFolder))
                    {
                        result = gss.SrvFolder.Replace("\\", ".");
                    }

                    return gss.SrvDefaultProjectNameSpace + result;
                }
            }
            return "";
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
        String GetWebApiRoutePrefix(ModelViewSerializable model)
        {
            string result = model.WebApiRoutePrefix;
            if (string.IsNullOrEmpty(result))
            {
                result = "";
            }
            return result;
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
        bool IsEntityTypeGuid(ModelViewPropertySerializable prop)
        {
            return prop.UnderlyingTypeName.Equals("System.Guid");
        }
        String GetFirstPrimKeyProperty(ModelViewSerializable model)
        {
            return model.PrimaryKeyProperties.FirstOrDefault().OriginalPropertyName;
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
        ModelViewPropertyOfVwSerializable GetModelScalarPropByKeyProp(ModelViewSerializable model, ModelViewKeyPropertySerializable pk)
        {
            ModelViewPropertyOfVwSerializable rslt = null;
            if ((model == null) || (pk == null)) return null;
            if (model.ScalarProperties == null) return null;
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
        ModelViewUniqueKeyOfVwSerializable GetModelPrimaryKey(ModelViewSerializable model)
        {
            ModelViewUniqueKeyOfVwSerializable rslt = null;
            if (model == null) return rslt;
            if ((model.PrimaryKeyProperties == null) || (model.ScalarProperties == null)) return rslt;
            if ((model.PrimaryKeyProperties.Count < 1) || (model.ScalarProperties.Count < 1)) return rslt;
            foreach (ModelViewKeyPropertySerializable pk in model.PrimaryKeyProperties)
            {
                ModelViewPropertyOfVwSerializable scProp =
                    model.ScalarProperties.Where(p => ((p.OriginalPropertyName == pk.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))).FirstOrDefault();
                if (scProp == null)
                {
                    scProp = GetModelScalarPropByKeyProp(model, pk);
                }
                if (scProp != null)
                {
                    if (rslt == null) rslt = new ModelViewUniqueKeyOfVwSerializable()
                    {
                        UniqueKeyName = null,
                        IsPrimary = true,
                        UniqueKeyProperties = new List<ModelViewPropertyOfVwSerializable>()
                    };
                    rslt.UniqueKeyProperties.Add(scProp);
                }
            }
            return rslt;
        }
        List<ModelViewUniqueKeyOfVwSerializable> GetModelUniqueKeys(ModelViewSerializable model, List<ModelViewUniqueKeyOfVwSerializable> rsltKeys)
        {
            if ((model == null) || (rsltKeys == null)) return rsltKeys;
            if ((model.UniqueKeys == null) || (model.ScalarProperties == null)) return rsltKeys;
            foreach (ModelViewUniqueKeySerializable uk in model.UniqueKeys)
            {
                if (uk.UniqueKeyProperties == null) continue;
                if (uk.UniqueKeyProperties.Count < 1) continue;
                ModelViewUniqueKeyOfVwSerializable rslt = null;
                foreach (ModelViewKeyPropertySerializable pk in uk.UniqueKeyProperties)
                {
                    ModelViewPropertyOfVwSerializable scProp =
                        model.ScalarProperties.Where(p => ((p.OriginalPropertyName == pk.OriginalPropertyName) && (string.IsNullOrEmpty(p.ForeignKeyNameChain)))).FirstOrDefault();
                    if (scProp == null)
                    {
                        scProp = GetModelScalarPropByKeyProp(model, pk);
                    }
                    if (scProp != null)
                    {
                        if (rslt == null) rslt = new ModelViewUniqueKeyOfVwSerializable()
                        {
                            UniqueKeyName = uk.UniqueKeyName,
                            IsPrimary = false,
                            UniqueKeyProperties = new List<ModelViewPropertyOfVwSerializable>()
                        };
                        rslt.UniqueKeyProperties.Add(scProp);
                    }
                }
                if (rslt != null)
                {
                    rsltKeys.Add(rslt);
                }
            }
            return rsltKeys;
        }
        ModelViewUniqueKeyOfVwSerializable GetModelPrimKeyFromList(List<ModelViewUniqueKeyOfVwSerializable> uniqueKeys)
        {
            if (uniqueKeys == null) return null;
            return uniqueKeys.Where(u => u.IsPrimary).FirstOrDefault();
        }
        ModelViewUniqueKeyOfVwSerializable GetModelUniqueKeyByNameFromList(List<ModelViewUniqueKeyOfVwSerializable> uniqueKeys, string name)
        {
            if (uniqueKeys == null) return null;
            if (string.IsNullOrEmpty(name))
            {
                return uniqueKeys.Where(u => string.IsNullOrEmpty(u.UniqueKeyName)).FirstOrDefault();
            }
            else
            {
                return uniqueKeys.Where(u => u.UniqueKeyName == name).FirstOrDefault();
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







        ///////////////////////////////////////////////////////////////////////////////////// 


        bool AbpIsEntityWithId(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.PrimaryKeyProperties == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return model.PrimaryKeyProperties.Any(p => p.OriginalPropertyName == "Id") &&
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "Id" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == ""))) &&
                (model.PrimaryKeyProperties.Count == 1);
        }
        string GetAbpIdDataType(ModelViewSerializable model)
        {
            string rslt = "";
            if (model.PrimaryKeyProperties != null)
            {
                ModelViewKeyPropertySerializable pkItm = model.PrimaryKeyProperties.FirstOrDefault(p => p.OriginalPropertyName == "Id");
                if (pkItm != null)
                {
                    rslt = pkItm.UnderlyingTypeName;
                }
            }
            return rslt;
        }
        bool AbpIsAbpEntityWithOutId(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.PrimaryKeyProperties == null) return rslt;
            return model.PrimaryKeyProperties.Count > 1;
        }
        bool AbpIsCreationAuditedRoot(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "CreatorId" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")))
                &&
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "CreationTime" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        bool AbpIsModificationAuditedObject(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "LastModifierId" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")))
                &&
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "LastModificationTime" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        bool AbpIsAuditedObject(ModelViewSerializable model)
        {
            return AbpIsModificationAuditedObject(model) &&
            AbpIsCreationAuditedRoot(model);
        }
        bool AbpIsDeletionAuditedObject(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "IsDeleted" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")))
                &&
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "DeleterId" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")))
                &&
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "DeletionTime" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        bool AbpIsFullAuditedObject(ModelViewSerializable model)
        {
            return AbpIsAuditedObject(model) && AbpIsDeletionAuditedObject(model);
        }
        bool AbpHasEntityVersion(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "EntityVersion" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        bool AbpHasMultiTenant(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "TenantId" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        bool AbpHasConcurrencyStamp(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "ConcurrencyStamp" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        bool AbpHasExtraProperties(ModelViewSerializable model)
        {
            bool rslt = false;
            if (model == null) return rslt;
            if (model.ScalarProperties == null) return rslt;
            return
                model.ScalarProperties.Any(p => p.OriginalPropertyName == "ExtraProperties" && ((p.ForeignKeyName == null) || (p.ForeignKeyName == "")));
        }
        string AbpFirstItemOfNameSpace(string inpt)
        {
            string rslt = "";
            if (!string.IsNullOrEmpty(inpt))
            {
                string[] itms = inpt.Split('.');
                if(itms.Length > 0)
                {
                    rslt = itms[0];
                }
            }
            return rslt;
        }

    }
}
