namespace Kofti.Manager.Data.Entities
{
    public class ConfigEnttiy : EntityBase<int>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int ApplicationId { get; set; }

        //Relations
        public virtual ApplicationEntity Application { get; set; }
    }
}