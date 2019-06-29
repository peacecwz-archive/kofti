using System.Collections.Generic;

namespace Kofti.Manager.Data.Entities
{
    public class ApplicationEntity : EntityBase<int>
    {
        public ApplicationEntity()
        {
            Configs = new List<ConfigEnttiy>();
        }

        public string Name { get; set; }

        //Relations
        public virtual IEnumerable<ConfigEnttiy> Configs { get; set; }
    }
}