using System;
using System.IO;
using System.Threading;
using RaidTool.Logic.Interfaces;

namespace RaidTool.Logic
{
    public class ParsedFileCopier : IParsedFileCopier
    {
        public string CopyFile(string path, string filePath)
        {
            Thread.Sleep(4000);

            var combine = Path.Combine(path, $"{DateTime.Now.Date:yyyyMMdd}");
            var exists = Directory.Exists(combine);

            if (!exists)
            {
                Directory.CreateDirectory(combine);
            }

            var fileName = Path.GetFileName(filePath);

            if (fileName != null)
            {
                var destFileName = Path.Combine(combine, fileName);
				if (File.Exists(destFileName))
				{
					File.Delete(destFileName);
				}
                File.Move(filePath, destFileName);
                return destFileName;
            }

            throw new NotSupportedException("This should not happen.");
        }
    }
}