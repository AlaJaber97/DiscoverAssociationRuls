using System.Collections.Generic;

namespace DiscoverAssociationRuls
{
    public class TableInstance
    {
        public List<Instance> ListInstance { get; set; }
        public TableInstance()
        {
            ListInstance = new List<Instance>();
        }
    }
}
