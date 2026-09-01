namespace OPCUA_PROJECT.Api.DTOs
{
    public class MonitoredVariableDto
    {
        public int Id { get; set; }

        public string VariableName { get; set; } = "";

        public string NodeName { get; set; } = "";

        public bool Enabled { get; set; }
    }
}