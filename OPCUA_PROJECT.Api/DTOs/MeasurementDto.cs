using System.Security;

namespace OPCUA_PROJECT.Api.DTOs
{
    public class MeasurementDto
    {
        public int VariableId { get; set; }

        public string Value { get; set; } = "";

        public string Status { get; set; } = "";

        public DateTime Timestamp { get; set; } 
    }
}
