using EVTC_Log_Parser.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EVTC_Log_Parser.Model
{
    public class Converter
    {
	    private readonly int _time;
        private readonly NPC _target;
        private readonly Parser _parser;
        private readonly List<Player> _players;
	    private List<NPC> _npcs;

	    public Converter(Parser parser)
        {
            _parser = parser;
            _target = parser.NPCs.Find(n => n.SpeciesId == parser.Metadata.TargetSpeciesId);
            _time = _target.LastAware - _target.FirstAware;
            _players = parser.Players;

			foreach (Player p in _players)
            {
                p.LoadEvents(_target, parser.Events, parser.NPCs);
            }
        }

	    public SharedValues GetFinalDPS()
        {
	        var sharedValues = new SharedValues();
			sharedValues.FightDuration = _time / 1000.0;
	        sharedValues.Success = _target.Died;
	        sharedValues.Target = _target.Name;
	        sharedValues.LogStart = _parser.Metadata.LogStart;
	        sharedValues.Start = _target.FirstAware;
	        sharedValues.End = _target.LastAware;

            // Calculate DPS
            List<FinalDPS> rows = new List<FinalDPS>();
            _players.ForEach(p => rows.Add(new FinalDPS(sharedValues.FightDuration)
            {
				Group = p.Group,
                Character = p.Character,
                Account = p.Account,
                Profession = p.Profession.ToString(),
                PowerAll = p.DamageEvents.Where(e => !e.IsBuff).Sum(e => e.Damage),
                PowerBoss = p.DamageEvents.Where(e => !e.IsBuff && e.Target == _target.Instid).Sum(e => e.Damage),
                CondiAll = p.DamageEvents.Where(e => e.IsBuff).Sum(e => e.Damage),
                CondiBoss = p.DamageEvents.Where(e => e.IsBuff && e.Target == _target.Instid).Sum(e => e.Damage),
				Down = p.StateEvents.Count(e => e.StateChange == StateChange.ChangeDown),
				Dead = p.StateEvents.Any(e => e.StateChange == StateChange.ChangeDead),
				DeadTime = p.StateEvents.SingleOrDefault(e => e.StateChange == StateChange.ChangeDead)?.Time,
				FightDurationPlayer = p.StateEvents.SingleOrDefault(e => e.StateChange == StateChange.ChangeDead) != null ? (p.StateEvents.Single(e => e.StateChange == StateChange.ChangeDead).Time - _target.FirstAware)/1000.0 : -1.0,

			}));
            rows = rows.OrderByDescending(f => f.TotalBoss).ToList();

	        sharedValues.PlayerValues = rows;

	        return sharedValues;
        }

	    private void ConvertMetadata(StringBuilder sb)
        {
            sb.Append(_parser.Metadata.ArcVersion);
            sb.Append(_parser.Metadata.LogStart.ToString("yyyy-MM-dd,"));
            sb.Append(_parser.Metadata.GWBuild);
        }

        private void ConvertTarget(StringBuilder sb)
        {
            sb.Append(_target.SpeciesId);
            sb.Append(_target.Name);
            sb.Append(_time / 1000.0);
        }

        private void ConvertGroup(StringBuilder sb, Player p)
        {
            sb.Append(p.Account);
            sb.Append(p.Character);
            sb.Append(p.Profession);
            sb.Append(((p.Condition > 5) ? "C," : "P,"));
        }

        private void ConvertDamage(StringBuilder sb, Player p)
        {
            sb.Append(Math.Round(p.DamageEvents.Sum(e => e.Damage) / (_time / 1000.0), 2)); // DPS
            sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff).Sum(e => e.Damage) / (_time / 1000.0), 2)); // DPS
            sb.Append(Math.Round(p.DamageEvents.Where(e => e.IsBuff).Sum(e => e.Damage) / (_time / 1000.0), 2)); // DPS
        }

        private void ConvertBoons(StringBuilder sb, Player p)
        {
            Dictionary<int, List<BoonEvent>> be = p.BoonEvents;
            foreach (Boon b in Boon.Values)
            {
                if (b.IsDuration)
                {
                    sb.Append(Math.Round(BoonDurationRates(b, be[b.SkillId]), 2)); // Duration Boon Rates
                }
                else
                {
                    sb.Append(Math.Round(BoonIntensityStacks(b, be[b.SkillId]), 2)); // Duration Boon Stacks
                }
            }
        }

        private void ConvertStatistics(StringBuilder sb, Player p)
        {
            double n = p.DamageEvents.Where(e => !e.IsBuff).Count(); // Power Damage Events
            sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.Result == Result.Critical).Count() / n, 2)); // Critical Rate
            sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsNinety).Count() / n, 2)); // Scholar Rate
            sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsFlanking).Count() / n, 2)); // Flanking Rate
            sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsMoving).Count() / n, 2)); // Moving Rate
            sb.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Dodge).Count()); // Dodge Count
            sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.WeaponSwap).Count()); // Weapon Swap Count
            sb.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int)CustomSkill.Resurrect).Count()); // Resurrect Count
            sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.ChangeDown).Count()); // Downed Count
            sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.ChangeDead).Count()); // Died
        }

        private double BoonDurationRates(Boon b, List<BoonEvent> boonEvents)
        {
            if (boonEvents.Count == 0) { return 0.0; }
            int prev = 0;
            int curr = 0;
            List<Interval> bi = new List<Interval>();
            BoonStack bs = new BoonStackDuration(b.Capacity);
            foreach (BoonEvent be in boonEvents)
            {
                curr = be.Time;
                bs.Update(curr - prev);
                bs.Add(be.Duration);
                bi.Add(new Interval
                {
                    Start = curr,
                    End = curr + bs.CalculateValue()
                });
                prev = curr;
            }
            List<Interval> mbi = (bi.Count == 1) ? bi : new List<Interval>();
            if (mbi.Count == 0)
            {
                Interval a = bi[0];
                int s = a.Start;
                int e = a.End;
                for (int i = 1; i < bi.Count; i++)
                {
                    Interval c = bi[i];
                    if (c.Start <= e)
                    {
                        e = Math.Max(c.End, e);
                    }
                    else
                    {
                        mbi.Add(new Interval() { Start = s, End = e });
                        s = c.Start;
                        e = c.End;
                    }
                }
                mbi.Add(new Interval() { Start = s, End = e });
            }
            Interval z = mbi[mbi.Count - 1];
            if (z.End > _time) { z.End = _time; }
            double duration = 0.0;
            foreach (Interval i in mbi)
            {
                duration += (i.End - i.Start);
            }
            return duration / _time;
        }

        private double BoonIntensityStacks(Boon b, List<BoonEvent> boonEvents)
        {
            if (boonEvents.Count == 0) { return 0.0; }
            int prev = 0;
            int curr = 0;
            List<int> s = new List<int> { 0 };

            BoonStackIntensity bs = new BoonStackIntensity(b.Capacity);
            foreach (BoonEvent be in boonEvents)
            {
                curr = be.Time;
                bs.SimulateTimePassed(curr - prev, s);
                bs.Update(curr - prev);
                bs.Add(be.Duration);
                if (prev != curr)
                {
                    s.Add(bs.CalculateValue());
                }
                else
                {
                    s[s.Count - 1] = bs.CalculateValue();
                }
                prev = curr;
            }
            bs.SimulateTimePassed(_time - prev, s);
            bs.Update(1);
            s.Add(bs.CalculateValue());
            return s.Average();
        }
    }
}
