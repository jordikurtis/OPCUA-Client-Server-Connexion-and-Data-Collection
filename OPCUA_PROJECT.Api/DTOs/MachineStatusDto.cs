namespace OPCUA_PROJECT.Api.DTOs
{
    public class MachineStatusDto
    {
        public string PlcName { get; set; } = "";
        public DateTime? LastSeen { get; set; }
        public bool IsOnline { get; set; }
    }
}
