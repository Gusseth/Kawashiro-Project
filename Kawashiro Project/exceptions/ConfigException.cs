using System;
using System.Collections.Generic;
using System.Text;

namespace Kawashiro_Project.exceptions
{
    class ConfigException : Exception
    {
        public ConfigException(string msg) : base(msg)
        {

        }

        public ConfigException() : base()
        {

        }
    }
}
