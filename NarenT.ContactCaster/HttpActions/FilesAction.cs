using System;
using NarenT.Net;
using System.Json;

namespace NarenT.ContactCaster.HttpActions
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
			files.Add(new JsonObject { 
				{ "Name", "Assignment.doc" },
				{ "Size", "100Kb" },
				{ "Type", "DOC" }
			});

			files.Add(new JsonObject { 
				{ "Name", "Acme - Contract.pdf" },
				{ "Size", "100Kb" },
				{ "Type", "PDF" }
			});

			files.Add(new JsonObject { 
				{ "Name", "Air Ticket.pdf" },
				{ "Size", "100Kb" },
				{ "Type", "PDF" }
			});

			var result = new ActionResult();
			result.Data = System.Text.Encoding.UTF8.GetBytes(files.ToString());
			result.ContentType = "application/json";
			return result;
		}

		public override ActionResult POST (System.Net.HttpListenerContext context, string httpActionPath)
		{
			return new ActionResult();
		}

		public override bool WillProcess (string requestPath)
		{
			return requestPath == "/files";
		}
		#endregion
	}
}

