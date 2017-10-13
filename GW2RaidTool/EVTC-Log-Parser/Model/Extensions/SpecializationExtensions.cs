using System;
using System.Text;

namespace EVTC_Log_Parser.Model
{
    public static class SpecializationExtensions
    {
	    public static Profession ToProfession(this Specialization value)
        {
            return (Profession)Enum.Parse(typeof(Profession), value.ToString());
        }
    }
}
