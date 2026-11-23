using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCUA_PROJECT
{
    public  class AppConfig
    {
        public DatabaseConfig Database { get; set; } = new();


        public List<PlcDefinition> Plcs { get; set; } = new();

    }
}
