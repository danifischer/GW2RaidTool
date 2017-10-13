using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVTC_Log_Parser.Model
{
	public class SharedValues
	{
		public double FightDuration { get; set; }
		public List<FinalDPS> PlayerValues { get; set; }
		public bool Success { get; set; }
		public string Target { get; set; }
		public int Start { get; set; }
		public int End { get; set; }
		public DateTime LogStart { get; set; }
	}
}
