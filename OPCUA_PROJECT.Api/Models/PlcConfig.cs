namespace OPCUA_PROJECT.Api.Models
{
    public class PlcConfig
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string EndpointUrl { get; set; } = "";

        public bool Enabled { get; set; }

        public int? GroupId {  get; set; }
       
    }
}
