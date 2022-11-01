﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfProjectNewsReader.Model
{
    public class InternalResponse
    {
        private readonly Dictionary<bool, string> response = new Dictionary<bool, string>();

        public Dictionary<bool, string> Response { get => response; }

        public InternalResponse(bool result, string message)
        {
            Response.Add(result, message);
        }
    }
}
