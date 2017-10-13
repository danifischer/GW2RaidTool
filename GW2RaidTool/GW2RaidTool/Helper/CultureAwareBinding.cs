using System.Globalization;
using System.Windows.Data;

namespace RaidTool.Helper
{
	public class CultureAwareBinding : Binding
	{
		public CultureAwareBinding()
		{
			ConverterCulture = CultureInfo.CurrentCulture;
		}
	}
}