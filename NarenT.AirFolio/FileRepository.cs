using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

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

		public static string SaveFile(string name, byte[] data)
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine(documentsPath, name);
			Console.WriteLine (filePath);
			File.WriteAllBytes(filePath, data);
			return filePath;
		}

		public static void SaveFile(HttpListenerContext context)
		{
			SaveFile(context.Request.ContentEncoding, 
			         GetBoundary(context.Request.ContentType), 
			         context.Request.InputStream);
		}

		private static String GetBoundary(String ctype)
		{
		    return "--" + ctype.Split(';')[1].Split('=')[1];
		}

		private static void SaveFile(Encoding enc, string boundary, Stream input)
		{
		    Byte[] boundaryBytes = enc.GetBytes(boundary);
		    Int32 boundaryLen = boundaryBytes.Length;

	        byte[] buffer = new byte[1024];
	        int len = input.Read(buffer, 0, 1024);
	        int startPos = -1;

	        // Find start boundary
	        while (true)
	        {
	            if (len == 0)
	            {
	                throw new Exception("Start Boundary Not Found");
	            }

	            startPos = IndexOf(buffer, len, boundaryBytes);
	            if (startPos >= 0)
	            {
	                break;
	            }
	            else
	            {
	                Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
	                len = input.Read(buffer, boundaryLen, 1024 - boundaryLen);
	            }
	        }

	        // Skip four lines (Boundary, Content-Disposition, Content-Type, and a blank)
			var endOfLineByte = enc.GetBytes("\n")[0];
			var preambleByteArray = new List<byte>();
	        for (int i = 0; i < 4; i++)
	        {
	            while (true)
	            {
	                if (len == 0)
	                {
	                    throw new Exception("Preamble not Found.");
	                }

					var prevStartPos = startPos;
	                startPos = Array.IndexOf(buffer, endOfLineByte, startPos);
	                if (startPos >= 0)
	                {
						var destinationArray = new byte[(startPos - prevStartPos) + 1];
						Array.Copy(buffer, prevStartPos, destinationArray, 0, (startPos - prevStartPos) + 1);
						preambleByteArray.AddRange(destinationArray);
	                    startPos++;
	                    break;
	                }
	                else
	                {
						preambleByteArray.AddRange(buffer);
	                    len = input.Read(buffer, 0, 1024);
	                }
	            }
	        }

			var preamble = enc.GetString(preambleByteArray.ToArray());
			var contentDisposition = preamble.Split(new[] { '\n' })[1];
			var filename = contentDisposition.Split(new[] { ';' }).Last().Split(new[] { '=' }).Last()
				.Trim().TrimStart(new[] {'"'}).TrimEnd(new[] {'"'});
	        
			Array.Copy(buffer, startPos, buffer, 0, len - startPos);
	        len = len - startPos;

			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var filePath = Path.Combine(documentsPath, filename);

			int fileNameIncrement = 0;
			while (File.Exists(filePath))
			{
				fileNameIncrement++;
				var extension = Path.GetExtension(filePath);
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
				filePath = 
					Path.Combine(documentsPath,
					string.Format("{0}-{1}{2}", fileNameWithoutExtension, fileNameIncrement, extension));
			}

			Console.WriteLine ("File path is " + filePath);

			using (FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write))
		    {
		        while (true)
		        {
		            int endPos = IndexOf(buffer, len, boundaryBytes);
		            if (endPos >= 0)
		            {
		                if (endPos > 0) output.Write(buffer, 0, endPos);
		                break;
		            }
		            else if (len <= boundaryLen)
		            {
		                throw new Exception("End Boundary Not Found");
		            }
		            else
		            {
		                output.Write(buffer, 0, len - boundaryLen);
		                Array.Copy(buffer, len - boundaryLen, buffer, 0, boundaryLen);
		                len = input.Read(buffer, boundaryLen, 1024 - boundaryLen) + boundaryLen;
		            }
		        }
		    }
		}

		private static int IndexOf(Byte[] buffer, int len, byte[] boundaryBytes)
		{
		    for (int i = 0; i <= len - boundaryBytes.Length; i++)
		    {
		        Boolean match = true;
		        for (int j = 0; j < boundaryBytes.Length && match; j++)
		        {
		            match = buffer[i + j] == boundaryBytes[j];
		        }

		        if (match)
		        {
		            return i;
		        }
		    }

		    return -1;
		}
	}
}