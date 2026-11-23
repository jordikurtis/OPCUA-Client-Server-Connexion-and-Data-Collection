namespace OPCUA_PROJECT.Api.Models
{
    public class Measurement
    {
        public int Id { get; set; }

        public int VariableId { get; set; }

        public string Value { get; set; } = "";

        public string Status { get; set; } = "";

        public DateTime SourceTimestamp { get; set; }

        public DateTime InsertedAt { get; set; }
    }
}
