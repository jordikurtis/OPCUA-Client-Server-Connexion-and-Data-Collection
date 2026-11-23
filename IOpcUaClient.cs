
namespace OPCUA_PROJECT
{
    /*
     contrat que tout client OPC UA doit respecter
    * si demain on change de librarie OPC UA ---> ON VA JUSTE CHANGER L'IMPLEMENTATION et PAS LE RESTE DE CODE  .Intefaces
     */
    public interface IOpcUaClient
    {
        // CancellationToken ajouté pour le rechargement config léger :
        // permet à PlcConnectionManager d'arrêter proprement une connexion
        // individuelle sans affecter les autres PLCs en cours.
        Task ConnectAndMonitorAsync(
            PlcDefinition plc,
            Func<MeasurementsData, Task> onDataReceived,
            CancellationToken cancellationToken);
        // deconnecte proprement
        Task DisconnectAsync();
        bool IsConnected { get; }
    }

}
