using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennecoOPCUA
{
    internal class PlcDefinition
    {
        public string Name { get; set; }
        public string EndpointUrl { get; set; }

        public List<string> NodesToMonitor { get; set; }
      
    }
}
