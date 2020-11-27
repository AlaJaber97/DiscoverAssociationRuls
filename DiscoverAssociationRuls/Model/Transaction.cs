using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DiscoverAssociationRuls
{
    public class Transaction
    {
        public string TID { get; set; }
        public string ItemSet { get; set; }

        public Transaction(string TID, string ItemSet)
        {
            this.ItemSet = ItemSet;
            this.TID = TID;
        }
        public List<string> ItemSeparator => ItemSet.Split(',').Select(s => s.Trim()).ToList();
    }
}
