using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscoverAssociationRuls
{
    public class TablesFrequentItemSetSizeOf
    {
        public List<TableInstance> ListTableInstance { get; set; }
        public TablesFrequentItemSetSizeOf()
        {
            ListTableInstance = new List<TableInstance>();
        }
        public TableInstance this[int indexrange]
        {
            get
            {
                if (indexrange > 0 && indexrange <= ListTableInstance.Count())
                    return ListTableInstance[indexrange - 1];
                throw new Exception("not allowed use index out list");
            }
            set
            {
                if (indexrange > 0 && indexrange <= ListTableInstance.Count())
                    ListTableInstance[indexrange - 1] = value;
                else
                    throw new Exception("not allowed use index out list");
            }
        }
        public TableInstance Add(TableInstance tableCandidate, int MinimumSupport)
        {
            var tablefrequentItem = new TableInstance();
            var frequentItem = tableCandidate.ListInstance.FindAll(item => item.SupportCount >= MinimumSupport).ToList<Instance>();
            tablefrequentItem.ListInstance = frequentItem;
            ListTableInstance.Add(tablefrequentItem);
            return tablefrequentItem;
        }
    }
}
