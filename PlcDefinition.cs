namespace OPCUA_PROJECT
{
    public class PlcDefinition
    {
        public int Id { get; set; }

        public string Name { get; set; } = " ";
        public string EndpointUrl { get; set; } = " ";

        public bool Enabled { get; set; } = true;

        public List<NodeDefinition> Nodes { get; set; } = new();
    }

    public class NodeDefinition
    {
        public string Name { get; set; } = "";
        public string NodeId { get; set; } = " ";
        public string DataType { get; set; } = "";

        public List<VariableDefinition> Variables { get; set; } = new();
    }

    public class VariableDefinition
    {
        public string Name { get; set; } = "";
        public string NodeId { get; set; } = "";
        public string DataType { get; set; } = "";
        public bool Enabled { get; set; } = true;

        // Ajouté pour la Fehlermeldung (cas "tags simples") : si true, chaque
        // valeur reçue pour cette variable est en plus passée à
        // FaultEventService pour détecter apparition/résolution d'un défaut.
        public bool IsFaultIndicator { get; set; } = false;
    }
}
