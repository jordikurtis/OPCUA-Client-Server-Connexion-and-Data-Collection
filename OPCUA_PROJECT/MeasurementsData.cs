
namespace OPCUA_PROJECT
{
    public  class MeasurementsData
    {
        /*
         
         Represnte une measure lue depuis un Noeuds 
         - ENgros l'object qui voyage entre OPCUClient et PostgreSQL
         
         */
        public string PlcName { get; set; } = " ";
        public string NodeId { get; set; } = " ";

        public object? Value { get; set; } 

        public string Status { get; set; } = " ";

        public DateTime SourceTimestamp { get; set; }

        public DateTime InsertdAt { get; set; } = DateTime.UtcNow;


        //ligne ajouter 
        public string Unit { get; set; } = "";

        public override string ToString() {
            return $"[{SourceTimestamp: HH:mm:ss} {PlcName} | {NodeId} = {Value} {Unit} ({Status})]";
        }
    }
}
