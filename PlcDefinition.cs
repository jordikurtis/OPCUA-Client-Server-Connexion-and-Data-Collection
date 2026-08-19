using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace OPCUA_PROJECT
{
    public class PlcDefinition 
    {
        /*
         Represnete la configuration de un PLC A SURVEILLER 

        Contient  L'url de connexion et la liste des noeuds a lire NodesToMonitor
         
         */
        public string Name { get; set; } = " ";
        public string EndpointUrl { get; set; } = " ";

        public bool Enabled { get; set; } = true; // active / desactive sans supprimer // ligne ajouter pour traiter le JSON 

       /*
         Liste dynamique des noeuds surveillés

         Chaque noeud deviendra logiquement une table SQL
        */

        public List<NodeDefinition> Nodes { get; set; } = new(); // ligne ajouter pour traiter le JSON 

      //  public List<string> NodesToMonitor => Nodes.Select(n => n.NodeId).ToList(); // ligne ajouter pour traiter le JSON 

    }

    // Les deux class  ajouter pour traiter le JSON 

    /*
         Représente un noeud OPC UA principal

         (ex: Status, Position, Drehmomente...)
        */

    public class NodeDefinition
    {
        public string Name { get; set; } = ""; //  nom du Noeuds = nom logiquee table SQL 
        public string NodeId { get; set; } = " ";
        public string DataType { get; set; } = "";

        
        /*
         Sous-variables du noeud
         = futures colonnes SQL
        */
        public List<VariableDefinition> Variables { get; set; } = new();

        /* // mapping DB (ESSENTIEL)
         public string TargetTable { get; set; } = "";
         public string TargetColumn { get; set; } = "";

         public bool Enabled { get; set; } = true;
         */


        // public string Name { get; set; } = " ";
        // public string Unit { get; set; } = " ";

    }



    /*
         Représente une variable OPC UA individuelle
        */
    public class VariableDefinition
    {
        /*
         Nom logique variable
         = future colonne SQL
        */
        public string Name { get; set; } = "";

        /*
         NodeId OPC UA réel
         utilisé pour monitoring/lecture
        */
        public string NodeId { get; set; } = "";

        public string DataType { get; set; } = "";

        public bool Enabled { get; set; } = true;
    }



  /*  //Wrapper pour la déserialisation JSON
    public class PlcConfigFile
    { 
    public List<PlcDefinition> Plcs { get; set; } = new();
    
    
    } */

}
