﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS82ANGULAR.Model
{
    public enum NavigationTypeEnum : int
    {
        Unckown = 0,
        OneToMany = 1,
        OptionalToMany = 2,
        OneToOne = 3,
        OptionalToOne = 4
    }
}
