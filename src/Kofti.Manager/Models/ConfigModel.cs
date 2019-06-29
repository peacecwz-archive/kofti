using System;

namespace Kofti.Manager.Models
{
    public class ConfigModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int ApplicationId { get; set; }
    }
}