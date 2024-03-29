﻿using System;
using System.Collections.Generic;

namespace CS82ANGULAR.Model.Serializable
{
    [Serializable]
    public class DbContextSerializable
    {
        public bool Localize { get; set; }
        public string DbContextClassName { get; set; }
        public string DbContextFullClassName { get; set; }
        public string DbContextProjectUniqueName { get; set; }
        public List<ModelViewSerializable> ModelViews { get; set; }
        public List<CommonStaffSerializable> CommonStaffs { get; set; }
    }
}
