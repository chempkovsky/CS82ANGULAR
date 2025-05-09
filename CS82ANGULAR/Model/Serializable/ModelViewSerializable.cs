﻿using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class ModelViewSerializable
    {
        public string PluralTitle { get; set; }
        public string Title { get; set; }
        public string ViewName { get; set; }
        public string DomainViewName { get; set; }
        public string PageViewName { get; set; }
        public string DomainPageViewName { get; set; }
        public string RootEntityDbContextPropertyName { get; set; }
        public string RootEntityClassName { get; set; }
        public string RootEntityFullClassName { get; set; }
        public string RootEntityUniqueProjectName { get; set; }
        public string ViewProject { get; set; }
        public string ViewDefaultProjectNameSpace { get; set; }
        public string BaseClass { get; set; }
        public List<GeneratedDtoSerializable> GeneratedDtos { get; set; }
        public List<GeneratedServiceSerializable> GeneratedServices { get; set; }
        public System.String ViewFolder { get; set; }
        public System.Boolean GenerateJSonAttribute { get; set; }
        public System.Boolean UseOnlyRootPropsForSelect { get; set; }

        public List<ModelViewPropertyOfVwSerializable> ScalarProperties { get; set; }
        public List<ModelViewForeignKeySerializable> ForeignKeys { get; set; }
        public List<ModelViewKeyPropertySerializable> PrimaryKeyProperties { get; set; }
        public List<ModelViewEntityPropertySerializable> AllProperties { get; set; }
        public string WebApiRoutePrefix { get; set; }
        public string WebApiServiceName { get; set; }
        public string WebApiServiceProject { get; set; }
        public string WebApiServiceDefaultProjectNameSpace { get; set; }
        public string WebApiServiceFolder { get; set; }
        public bool IsWebApiSelectAll { get; set; }
        public bool IsWebApiSelectManyWithPagination { get; set; }
        public bool IsWebApiSelectOneByPrimarykey { get; set; }
        public bool IsWebApiAdd { get; set; }
        public bool IsWebApiUpdate { get; set; }
        public bool IsWebApiDelete { get; set; }
        public bool IsStandalone { get; set; }
        public List<CommonStaffSerializable> CommonStaffs { get; set; }
        public List<ModelViewUIFormPropertySerializable> UIFormProperties { get; set; }
        public List<ModelViewUIListPropertySerializable> UIListProperties { get; set; }
        public List<ModelViewUniqueKeySerializable> UniqueKeys { get; set; }
        public List<ModelViewFunSerializable> RootEntityFunctions { get; set; }
    }

}
