using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RaidTool.Helper
{
	public static class DataProtectionExtensions
	{
		public static string Protect(
			this string clearText,
			string optionalEntropy = null,
			DataProtectionScope scope = DataProtectionScope.CurrentUser)
		{
			if (clearText == null)
			{
				throw new ArgumentNullException(nameof(clearText));
			}

			var clearBytes = Encoding.UTF8.GetBytes(clearText);
			var entropyBytes = string.IsNullOrEmpty(optionalEntropy)
				? null
				: Encoding.UTF8.GetBytes(optionalEntropy);
			var encryptedBytes = ProtectedData.Protect(clearBytes, entropyBytes, scope);
			return Convert.ToBase64String(encryptedBytes);
		}

		public static string Unprotect(
			this string encryptedText,
			string optionalEntropy = null,
			DataProtectionScope scope = DataProtectionScope.CurrentUser)
		{
			if (encryptedText == null)
			{
				throw new ArgumentNullException(nameof(encryptedText));
			}

			var encryptedBytes = Convert.FromBase64String(encryptedText);
			var entropyBytes = string.IsNullOrEmpty(optionalEntropy) ? null : Encoding.UTF8.GetBytes(optionalEntropy);
			var clearBytes = ProtectedData.Unprotect(encryptedBytes, entropyBytes, scope);
			return Encoding.UTF8.GetString(clearBytes);
		}
	}
}
