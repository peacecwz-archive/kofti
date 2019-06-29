namespace Kofti.Models
{
    public class KoftiOptions
    {
        public string ApplicationName { get; set; }
        public string OrchestratorName { get; set; }
        public string RedisServers { get; set; }
        public bool RedisAllowAdmin { get; set; }
        public string RedisPassword { get; set; }
    }
}