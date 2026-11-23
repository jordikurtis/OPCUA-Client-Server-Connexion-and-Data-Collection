
namespace OPCUA_PROJECT
{
    /*
     contrat que tout client OPC UA doit respecter
    * si demain on change de librarie OPC UA ---> ON VA JUSTE CHANGER L'IMPLEMENTATION et PAS LE RESTE DE CODE  .Intefaces
     */
    public  interface IOpcUaClient
    {
        Task ConnectAndMonitorAsync(PlcDefinition plc, Func<MeasurementsData, Task> onDataReceived);
       
        // deconnecte proprement 
        Task DisconnectAsync();

        bool IsConnected { get; }
    }
}
