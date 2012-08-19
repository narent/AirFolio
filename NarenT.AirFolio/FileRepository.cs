using System;
using System.Collections.Generic;
using System.IO;

namespace NarenT.AirFolio
{
	public static class FileRepository
	{
		public static IEnumerable<FileInfo> GetFiles()
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var directory = new DirectoryInfo(documentsPath);
			return directory.EnumerateFiles();
		}
	}
}

