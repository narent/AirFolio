using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NarenT.Net;
using MonoTouch.AddressBook;
using System.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using NarenT.Net.HttpServerActions;

namespace NarenT.ContactCaster
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UINavigationController navigationController;
		
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			InitHttpServer();

			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var controller = new RootViewController ();
			navigationController = new UINavigationController (controller);
			window.RootViewController = navigationController;

			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}

		private static void InitHttpServer()
		{
			string ipAddress = null;
			var wifi = NetworkInterface.GetAllNetworkInterfaces().Where(iff => iff.Name == "en0").FirstOrDefault();
			if (wifi != null)
			{
				ipAddress = wifi.GetIPProperties()
					.UnicastAddresses
						.Where(address => address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						.Select(address => address.Address.ToString()).FirstOrDefault();
			}

			HttpServer server = new HttpServer(UrlScheme.http, "localhost", 8080, "/");
			Console.WriteLine ("http server running on " + ipAddress + ":8080");
			server.Actions.Add(new FilesAction());
			server.Actions.Add(new StaticFileAction());
			server.Start();
		}
	}

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

