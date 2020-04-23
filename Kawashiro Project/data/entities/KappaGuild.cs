using System;
using System.Collections.Generic;
using System.Text;

namespace Kawashiro_Project.data.entities
{
    public class KappaGuild
    {
        public List<ulong> clearedChannels; // The ID of the channels that are cleared once everyone disconnects from all voice channels
        public KappaGuild(ulong[] clearedChannels)
        {
            this.clearedChannels = new List<ulong>(clearedChannels);
        }
    }
}
