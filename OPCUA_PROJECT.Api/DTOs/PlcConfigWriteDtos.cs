namespace OPCUA_PROJECT.Api.DTOs
{
    public class PlcConfigCreateDto
    {
        public string Name { get; set; } = "";
        public string EndpointUrl { get; set; } = "";
        public bool Enabled { get; set; } = false; // false par défaut : sécurité, on active explicitement
        public int? GroupId { get; set; }
    }

    public class PlcConfigUpdateDto
    {
        public string Name { get; set; } = "";
        public string EndpointUrl { get; set; } = "";
        public bool Enabled { get; set; }
        public int? GroupId { get; set; }
    }
}
