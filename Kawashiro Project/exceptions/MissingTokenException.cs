using System;
using System.Collections.Generic;
using System.Text;

namespace Kawashiro_Project.exceptions
{
    class MissingTokenException : ConfigException
    {
        public MissingTokenException(string msg) : base(msg)
        {
            
        }

        public MissingTokenException() : base()
        {

        }
    }
}
