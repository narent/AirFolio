using System;
using NarenT.Net;
using System.Json;
using System.Linq;
using System.IO;
using System.Text;

namespace NarenT.AirFolio.HttpActions
{
	public class FilesAction : HttpAction
	{
		public FilesAction() : base("ContactsAction")
		{
		}

		#region implemented abstract members of NarenT.Net.HttpAction
		public override ActionResult GET (System.Net.HttpListenerContext context, string httpActionPath)
		{
			JsonArray files = new JsonArray();
			files.AddRange(
				FileRepository.GetFiles()
				.Select(f => (JsonValue)new JsonObject { 
					{ "Name", f.Name },
					{ "Size", f.Length },
					{ "Type", f.Extension },
					{ "Url", "/files/" + f.Name }
				})
			);

			var result = new ActionResult();
			result.Data = System.Text.Encoding.UTF8.GetBytes(files.ToString());
			result.ContentType = "application/json";
			return result;
		}

		public override ActionResult POST (System.Net.HttpListenerContext context, string httpActionPath)
		{
			FileRepository.SaveFile(context);
			var result = new ActionResult();
			result.HttpStatusCode = System.Net.HttpStatusCode.Redirect;
			result.Headers.Add(Tuple.Create("Location", "/"));
			return result;
		}

		public override bool WillProcess (string requestPath)
		{
			return requestPath == "/files";
		}
		#endregion
	}
}

