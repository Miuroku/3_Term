﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    interface IConfigurationParser
    {
        ParsedObject Parse(string text);
    }
}
