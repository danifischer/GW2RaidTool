using System;
using System.Collections.Generic;
using System.Linq;

namespace EVTC_Log_Parser.Model
{
    public class Player : IAgent
    {
	    public string Address { get; set; }
        public int FirstAware { get; set; }
        public int LastAware { get; set; }
        public int Instid { get; set; }
        public Profession Profession { get; set; }
        public string Character { get; set; }
        public string Account { get; set; }
        public string Group { get; set; }
        public int Toughness { get; set; }
        public int Healing { get; set; }
        public int Condition { get; set; }
        public List<CombatEvent> DamageEvents { get; set; } = new List<CombatEvent>();
        public List<StateChangeEvent> StateEvents { get; set; } = new List<StateChangeEvent>();
        public Dictionary<int, List<BoonEvent>> BoonEvents { get; set; } = new Dictionary<int, List<BoonEvent>>();

	    public Player(string agentName)
        {
            string[] splitName = agentName.Split('\0');
            Character = splitName[0];
            Account = splitName[1] ?? ":?.????";
            Group = splitName[2] ?? "?";
        }

	    public void LoadEvents(NPC target, List<Event> events, List<NPC> npcs)
        {
            SetDamageEvents(events, npcs);
            SetBoonEvents(events, target);
            SetStateChangeEvents(events);
        }


	    private void SetDamageEvents(List<Event> events, List<NPC> npcs)
        {
	        foreach (Event e in events)
            {
                if (e.SrcInstid == Instid || e.SrcMasterInstid == Instid)
                {
                    if (e.IFF == IFF.Foe && e.DstInstid != 0)
                    {
                        if (e.StateChange == StateChange.None)
                        {
                            if (e.IsBuff && e.BuffDmg != 0)
                            {
                                DamageEvents.Add(new CombatEvent()
                                {
                                    Time = e.Time,
                                    Damage = e.BuffDmg,
                                    SkillId = e.SkillId,
                                    IsBuff = e.IsBuff,
                                    Result = e.Result,
                                    IsNinety = e.IsNinety,
                                    IsMoving = e.IsMoving,
                                    IsFlanking = e.IsFlanking,
	                                Target = e.DstInstid
								});
                            }
                            else if (!e.IsBuff && e.Value != 0)
                            {
                                DamageEvents.Add(new CombatEvent()
                                {
                                    Time = e.Time,
                                    Damage = e.Value,
                                    SkillId = e.SkillId,
                                    IsBuff = e.IsBuff,
                                    Result = e.Result,
                                    IsNinety = e.IsNinety,
                                    IsMoving = e.IsMoving,
                                    IsFlanking = e.IsFlanking,
	                                Target = e.DstInstid
								});
                            }
                        }
                    }
                }
            }
        }

        private void SetBoonEvents(List<Event> events, NPC target)
        {
            foreach (Boon b in Boon.Values)
            {
                BoonEvents.Add(b.SkillId, new List<BoonEvent>());
            }
            int time = target.LastAware - target.FirstAware;
            foreach (Event e in events)
            {
                if (e.DstInstid == Instid && BoonEvents.ContainsKey(e.SkillId) && e.Time < time)
                {
                    BoonEvents[e.SkillId].Add(new BoonEvent()
                    {
                        Time = e.Time,
                        Duration = e.Value
                    });
                }
            }
            //Console.WriteLine();
            //Console.WriteLine(this.Character + "|" + BoonEvents[740].Count);
            //Console.Read();
        }

        private void SetBoonEvents(List<Event> events)
        {
            foreach (Boon b in Boon.Values)
            {
                BoonEvents.Add(b.SkillId, new List<BoonEvent>());
            }
            foreach (Event e in events)
            {
                if (e.DstInstid == Instid && BoonEvents.ContainsKey(e.SkillId))
                {
                    BoonEvents[e.SkillId].Add(new BoonEvent()
                    {
                        Time = e.Time,
                        Duration = e.Value
                    });
                }
            }
            //Console.WriteLine();
            //Console.WriteLine(this.Character + "|" + BoonEvents[740].Count);
            //Console.Read();
        }

        private void SetStateChangeEvents(List<Event> events)
        {
            foreach (Event e in events)
            {
                if (e.StateChange != StateChange.None && e.SrcInstid == Instid)
                {
                    StateEvents.Add(new StateChangeEvent()
                    {
                        Time = e.Time,
                        StateChange = e.StateChange
                    });
                }
            }
        }
    }
}
