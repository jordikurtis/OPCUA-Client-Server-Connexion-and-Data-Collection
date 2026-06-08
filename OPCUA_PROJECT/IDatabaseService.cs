
namespace OPCUA_PROJECT
{
    /*
     
     Contrat que tout service de base de données doit respecter 
    * si demain onpasse de PostgrSQL a InfluxDB ou SQL Seerver on cree une nouvellle 
    * implementation  sasn toucher au rest 
     */
    public interface IDatabaseService
    {
        // sauvegarde une  measurement  dans la base de données 
        Task SaveAsync(MeasurementsData data );

        // verife que la connection a DB functionne 
        Task<bool> TestConnectionAsync();




    }
}
