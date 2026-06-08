
namespace OPCUA_PROJECT
{
    public class PlcDefinition 
    {
        /*
         Represnete la configuration de un PLC A SURVEILLER 
        Contient  L'url de connexion et la list edes noeuds a lire NodesToMonitor
         
         */
        public string Name { get; set; } = " ";
        public string EndpointUrl { get; set; } = " ";

        public bool Enabled { get; set; } = true; // active / desactive sans supprimer // ligne ajouter pour traiter le JSON 

        public List<NodeDefinition> Nodes { get; set; } = new(); // ligne ajouter pour traiter le JSON 

        public List<string> NodesToMonitor => Nodes.Select(n => n.NodeId).ToList(); // ligne ajouter pour traiter le JSON 

    }

    // Les deux class  ajouter pour traiter le JSON 
    public class NodeDefinition
    {
        public string NodeId { get; set; } = " ";
        public string Name { get; set; } = " ";
        public string Unit { get; set; } = " ";

    }

    //Wrapper pour la déserialisation JSON
    public class PlcConfigFile
    { 
    public List<PlcDefinition> Plcs { get; set; } = new();
    
    
    }

}
