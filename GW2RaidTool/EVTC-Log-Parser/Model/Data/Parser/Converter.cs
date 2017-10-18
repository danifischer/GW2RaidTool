using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVTC_Log_Parser.Model.Data.Table;

namespace EVTC_Log_Parser.Model
{
	public class Converter
	{
		private readonly Parser _parser;
		private readonly List<Player> _players;
		private readonly NPC _target;
		private readonly int _time;
		private List<NPC> _npcs;

		public Converter(Parser parser)
		{
			_parser = parser;
			_target = parser.NPCs.Find(n => n.SpeciesId == parser.Metadata.TargetSpeciesId);
			_time = _target.LastAware - _target.FirstAware;
			_players = parser.Players;
			_npcs = parser.NPCs;

			foreach (var p in _players)
			{
				p.LoadEvents(_target, parser.Events, parser.NPCs);
			}
		}

		public SharedValues GetFinalDPS()
		{
			var filteredSkillIds = new List<int>();
			filteredSkillIds.Add(38153); // Cairn: Unseen Burden (area effect) 
			filteredSkillIds.Add(38266); // Deimos (name: ??, unknown skill)
			filteredSkillIds.Add(37792); // Deimos (unknown skill)

			var sharedValues = new SharedValues();
			sharedValues.FightDuration = _time / 1000.0;
			sharedValues.Success = _target.Died;
			sharedValues.Target = _target.Name;
			sharedValues.LogStart = _parser.Metadata.LogStart;
			sharedValues.Start = _target.FirstAware;
			sharedValues.End = _target.LastAware;

			// Calculate DPS
			var rows = new List<FinalDPS>();
			_players.ForEach(p => rows.Add(new FinalDPS(sharedValues.FightDuration)
			{
				Group = p.Group,
				Character = p.Character,
				Account = p.Account,
				Profession = p.Profession.ToString(),
				PowerAll = p.DamageEvents.Where(e => !e.IsBuff && !filteredSkillIds.Contains(e.SkillId)).Sum(e => e.Damage),
				PowerBoss = p.DamageEvents
					.Where(e => !e.IsBuff && !filteredSkillIds.Contains(e.SkillId) && e.Target == _target.Instid).Sum(e => e.Damage),
				CondiAll = p.DamageEvents.Where(e => e.IsBuff && !filteredSkillIds.Contains(e.SkillId)).Sum(e => e.Damage),
				CondiBoss = p.DamageEvents
					.Where(e => e.IsBuff && !filteredSkillIds.Contains(e.SkillId) && e.Target == _target.Instid).Sum(e => e.Damage),
				Down = p.StateEvents.Count(e => e.StateChange == StateChange.ChangeDown),
				Dead = p.StateEvents.Any(e => e.StateChange == StateChange.ChangeDead),
				DeadTime = p.StateEvents.LastOrDefault(e => e.StateChange == StateChange.ChangeDead)?.Time,
				FightDurationPlayer = p.StateEvents.LastOrDefault(e => e.StateChange == StateChange.ChangeDead) != null
					? (p.StateEvents.Last(e => e.StateChange == StateChange.ChangeDead).Time - _target.FirstAware) / 1000.0
					: -1.0,
				Skills = DamageEventsToSkillDamage(p.DamageEvents.Where(i => !filteredSkillIds.Contains(i.SkillId)), _target.Instid, sharedValues.FightDuration)
			}));
			rows = rows.OrderByDescending(f => f.TotalBoss).ToList();

			// Keep for debugging
			//     foreach (var player in _players)
			//     {
			//var groupBy = player.DamageEvents;
			//      var enumerable = groupBy.Where(i => i.Target == _target.Instid).OrderByDescending(i => i.Damage).GroupBy(i => i.SkillId);

			//      var select = enumerable
			//	.Select(k => Tuple.Create(k.Key, _parser.Skills.FirstOrDefault(i => i.Id == k.Key)?.Name, groupBy.Where(j => j.SkillId == k.Key).Sum(j => j.Damage)))
			//	.ToList();

			//      var first = select.FirstOrDefault();
			//      var second = select.Skip(1).FirstOrDefault();
			//      var third = select.Skip(2).FirstOrDefault();

			//     }


			sharedValues.PlayerValues = rows;

			return sharedValues;
		}

		private IEnumerable<SkillDps> DamageEventsToSkillDamage(IEnumerable<CombatEvent> damageEvents, int targetId, double fightDuration)
		{
			var groupBySkills = damageEvents.GroupBy(i => i.SkillId);

			return groupBySkills.Select(skill =>
			{
				return new SkillDps(fightDuration)
				{
					Name = _parser.Skills.FirstOrDefault(k => skill.Key == k.Id)?.Name ?? "<unknown>",
					SkillId = skill.Key,
					CondiAll = skill.Where(e => e.IsBuff).Sum(e => e.Damage),
					CondiBoss = skill.Where(e => e.IsBuff && e.Target == targetId).Sum(e => e.Damage),
					PowerAll = skill.Where(e => !e.IsBuff).Sum(e => e.Damage),
					PowerBoss = skill.Where(e => !e.IsBuff && e.Target == targetId).Sum(e => e.Damage)
				};
			});
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
			sb.Append(p.Condition > 5 ? "C," : "P,");
		}

		private void ConvertDamage(StringBuilder sb, Player p)
		{
			sb.Append(Math.Round(p.DamageEvents.Sum(e => e.Damage) / (_time / 1000.0), 2)); // DPS
			sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff).Sum(e => e.Damage) / (_time / 1000.0), 2)); // DPS
			sb.Append(Math.Round(p.DamageEvents.Where(e => e.IsBuff).Sum(e => e.Damage) / (_time / 1000.0), 2)); // DPS
		}

		private void ConvertBoons(StringBuilder sb, Player p)
		{
			var be = p.BoonEvents;
			foreach (var b in Boon.Values)
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
			sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.Result == Result.Critical).Count() / n,
				2)); // Critical Rate
			sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsNinety).Count() / n, 2)); // Scholar Rate
			sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsFlanking).Count() / n, 2)); // Flanking Rate
			sb.Append(Math.Round(p.DamageEvents.Where(e => !e.IsBuff && e.IsMoving).Count() / n, 2)); // Moving Rate
			sb.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int) CustomSkill.Dodge)
				.Count()); // Dodge Count
			sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.WeaponSwap).Count()); // Weapon Swap Count
			sb.Append(_parser.Events.Where(e => e.SrcInstid == p.Instid && e.SkillId == (int) CustomSkill.Resurrect)
				.Count()); // Resurrect Count
			sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.ChangeDown).Count()); // Downed Count
			sb.Append(p.StateEvents.Where(s => s.StateChange == StateChange.ChangeDead).Count()); // Died
		}

		private double BoonDurationRates(Boon b, List<BoonEvent> boonEvents)
		{
			if (boonEvents.Count == 0)
			{
				return 0.0;
			}
			var prev = 0;
			var curr = 0;
			var bi = new List<Interval>();
			BoonStack bs = new BoonStackDuration(b.Capacity);
			foreach (var be in boonEvents)
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
			var mbi = bi.Count == 1 ? bi : new List<Interval>();
			if (mbi.Count == 0)
			{
				var a = bi[0];
				var s = a.Start;
				var e = a.End;
				for (var i = 1; i < bi.Count; i++)
				{
					var c = bi[i];
					if (c.Start <= e)
					{
						e = Math.Max(c.End, e);
					}
					else
					{
						mbi.Add(new Interval {Start = s, End = e});
						s = c.Start;
						e = c.End;
					}
				}
				mbi.Add(new Interval {Start = s, End = e});
			}
			var z = mbi[mbi.Count - 1];
			if (z.End > _time)
			{
				z.End = _time;
			}
			var duration = 0.0;
			foreach (var i in mbi)
			{
				duration += i.End - i.Start;
			}
			return duration / _time;
		}

		private double BoonIntensityStacks(Boon b, List<BoonEvent> boonEvents)
		{
			if (boonEvents.Count == 0)
			{
				return 0.0;
			}
			var prev = 0;
			var curr = 0;
			var s = new List<int> {0};

			var bs = new BoonStackIntensity(b.Capacity);
			foreach (var be in boonEvents)
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