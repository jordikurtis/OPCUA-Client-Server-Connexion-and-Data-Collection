
namespace OPCUA_PROJECT
{
    public  class MeasurementsData
    {
        /*
         
         Represente une measure lue depuis un Noeuds

         - ENgros l'object qui voyage entre OPCUClient et PostgreSQL
         
         */
        //indentité
        public string PlcName { get; set; } = " ";
        
        // Table logique Du JSON 
        public string NodeName { get; set; } = "";

        // Colonne logique DU JSON 
        public string VariableName { get; set; } = "";

        public string NodeId { get; set; } = " ";

        //Donnée OPC UA 
        public object? Value { get; set; } 
        public string Status { get; set; } = " ";
        public DateTime SourceTimestamp { get; set; }

       // public DateTime InsertdAt { get; set; } = DateTime.UtcNow;

        //Métadonnées DB(VENUES DU JSON)
        /*
        public string TargetTable { get; set; } = "";
        public string TargetColumn { get; set; } = "";
        */

        //ligne ajouter 
       // public string Unit { get; set; } = "";

        /*  public override string ToString() {
              return $"[{SourceTimestamp: HH:mm:ss} {PlcName} | {NodeId} = {Value} {Unit} ({Status})]";
          }
        */
        public override string ToString()
        {
            return $"[{SourceTimestamp:HH:mm:ss}] {PlcName} . {NodeName} . {VariableName} = {Value} ({Status})";
        }

        

    }
}
