﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2048
{
    interface ISearcher: IPlayer
    {   
        int Depth { get; set; }
    }
}
