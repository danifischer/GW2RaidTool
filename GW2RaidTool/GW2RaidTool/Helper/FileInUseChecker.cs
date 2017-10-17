using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace RaidTool.Helper
{
    internal static class FileInUseChecker
    {
	    const int ErrorSharingViolation = 32;
	    const int ErrorLockViolation = 33;

	    private static bool IsFileLocked(Exception exception)
	    {
		    var errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
		    return errorCode == ErrorSharingViolation || errorCode == ErrorLockViolation;
	    }

		internal static bool CheckFile(string filePath, int waitTime)
	    {
			var i = 0;
		    while (!CanReadFile(filePath) && i < 10)
		    {
			    Thread.Sleep(waitTime);
			    i++;
		    }

		    return i < 10;
		}

	    private static bool CanReadFile(string filePath)
	    {
		    //Try-Catch so we dont crash the program and can check the exception
		    try
		    {
			    //The "using" is important because FileStream implements IDisposable and
			    //"using" will avoid a heap exhaustion situation when too many handles  
			    //are left undisposed.
			    using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
			    {
				    fileStream.Close();  //This line is me being overly cautious, fileStream will never be null unless an exception occurs... and I know the "using" does it but its helpful to be explicit - especially when we encounter errors - at least for me anyway!
			    }
		    }
		    catch (IOException ex)
		    {
			    //THE FUNKY MAGIC - TO SEE IF THIS FILE REALLY IS LOCKED!!!
			    if (IsFileLocked(ex))
			    {
				    // do something, eg File.Copy or present the user with a MsgBox - I do not recommend Killing the process that is locking the file
				    return false;
			    }
		    }
		    return true;
	    }
	}
}
