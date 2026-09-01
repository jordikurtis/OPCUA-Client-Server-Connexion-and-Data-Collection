namespace OPCUA_PROJECT.Api.Models
{
    public class MonitoredVariable
    {
        public int Id { get; set; }

        public string PlcName { get; set; } = "";

        public int? GroupId { get; set; }

        public string NodeName { get; set; } = "";

        public string VariableName { get; set; } = "";

        public string NodeId { get; set; } = "";

        public string DataType { get; set; } = "";

        public bool Enabled { get; set; }
    }
}