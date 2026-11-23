namespace OPCUA_PROJECT.Api.Models
{
    public class MachineStatus
    {
        public string PlcName { get; set; } = "";
        public string EndpointUrl { get; set; } = "";
        public DateTime? LastSeen { get; set; }
        public bool IsOnline { get; set; }
    }
}
