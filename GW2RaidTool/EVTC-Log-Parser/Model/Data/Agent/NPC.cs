namespace EVTC_Log_Parser.Model
{
    public class NPC : IAgent
    {
	    public string Address { get; set; }
        public int FirstAware { get; set; }
        public int LastAware { get; set; }
        public int Instid { get; set; }
        public int SpeciesId { get; set; }
        public string Name { get; set; }
        public int Toughness { get; set; }
        public int Healing { get; set; }
        public int Condition { get; set; }
        public bool Died { get; set; }
    }
}
