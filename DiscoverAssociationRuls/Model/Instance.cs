namespace DiscoverAssociationRuls
{
    public class Instance
    {
        public string ItemSet { get; set; }
        public int SupportCount { get; set; } = 0;
        public Instance(string ItemSet, int SupportCount)
        {
            this.ItemSet = ItemSet;
            this.SupportCount = SupportCount;
        }
    }
}
