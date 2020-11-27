using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscoverAssociationRuls
{
    public class TablesCandidate
    {
        public List<TableInstance> ListTableInstance { get; set; }
        public TablesCandidate()
        {
            ListTableInstance = new List<TableInstance>();
        }
        public TableInstance this[int indexrange]
        {
            get
            {
                if(indexrange > 0 && indexrange <= ListTableInstance.Count())
                    return ListTableInstance[indexrange-1];
                throw new Exception("not allowed use index out list");
            }
            set
            {
                if (indexrange > 0 && indexrange <= ListTableInstance.Count())
                    ListTableInstance[indexrange-1] = value;
                else
                    throw new Exception("not allowed use index out list");
            }
        }
        public void Add(TableInstance tableInstance)
        {
            this.ListTableInstance.Add(tableInstance);
            foreach (var item in tableInstance.ListInstance)
            {
                Program.SearchInDataAbout(item.ItemSet);
            }
        }
    }
}
