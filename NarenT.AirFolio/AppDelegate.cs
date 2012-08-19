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
using NarenT.AirFolio.HttpActions;
using MonoTouch.Dialog;
using System.Drawing;
using NarenT.Extensions;
using System.IO;

namespace NarenT.AirFolio
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public static UIView NavigationBarTitleView;
		public static UIBarButtonItem[] ToolbarButtons;
		public static Section SpacerSection = new Section(string.Empty);

		// class-level declarations
		private UIWindow window;
		private UINavigationController navigationController;
		private static UIBarButtonItem StartHttpServerButton;
		private HttpServer AirFolioHttpServer;
		private UILabel AddressLabel;
		private static UIBarButtonItem HttpServerSwitchBarButtonItem;

		
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{

			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			var testPaths = new string[] { 
				Path.Combine(documentsPath, "Assignment.docx"),
				Path.Combine(documentsPath, "Notes.txt"),
				Path.Combine(documentsPath, "NY - LA Air Ticket.pdf")
			};

			foreach (var testPath in testPaths) {
				if (File.Exists(testPath)) {
					File.Delete(testPath);
				}
				File.WriteAllText(testPath, "Howdy, world.");
			}

			StartHttpServerButton = new UIBarButtonItem(UIBarButtonSystemItem.Play, (sender, args) => { this.StartButtonTapped(); });
			var startHttpServerSwitch = new UISwitch();
			startHttpServerSwitch.ValueChanged += (sender, e) => { this.StartButtonTapped(); };
			HttpServerSwitchBarButtonItem = new UIBarButtonItem(startHttpServerSwitch);

			ToolbarButtons = new[] { 
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), 
				HttpServerSwitchBarButtonItem,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
			};

			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var controller = new FilesListViewController ();
			navigationController = new UINavigationController (controller);
			navigationController.ToolbarHidden = false;
			window.AddSubview(navigationController.View);
			//window.RootViewController = navigationController;

			AddressLabel = new UILabel(new RectangleF(0.0f, 460.0f, 320.0f, 20.0f)) { BackgroundColor = UIColor.Black };
			AddressLabel.TextColor = UIColor.White;
			AddressLabel.TextAlignment = UITextAlignment.Center;
			window.AddSubview(AddressLabel);
			window.SendSubviewToBack(AddressLabel);

			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}

		private void StartButtonTapped()
		{
			if (this.AirFolioHttpServer == null || !this.AirFolioHttpServer.IsListening)
			{
				this.StartHttpServer();
				StartHttpServerButton.TintColor = UIColor.Gray;
				ShowHttpAddress();
			} 
			else 
			{
				this.StopHttpServer();
				StartHttpServerButton.TintColor = UIColor.Gray;
				HideHttpAddress();
			}
		}

		private void StartHttpServer()
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

			this.AirFolioHttpServer = new HttpServer(UrlScheme.http, "localhost", 8080, "/");
			Console.WriteLine ("http server running on " + ipAddress + ":8080");
			this.AirFolioHttpServer.Actions.Add(new FilesAction());
			var staticFileAction = new StaticFileAction();
			StaticFileAction.AddStaticFileMapping("files/", Environment.GetFolderPath (Environment.SpecialFolder.Personal));
			this.AirFolioHttpServer.Actions.Add(staticFileAction);
			this.AirFolioHttpServer.Start();
		}

		private void StopHttpServer() 
		{
			if (this.AirFolioHttpServer != null && this.AirFolioHttpServer.IsListening) {
				this.AirFolioHttpServer.Stop();
			}
		}

		private void ShowHttpAddress()
		{
			var dialogViewController = this.navigationController.TopViewController as DialogViewController;
			UIView.Animate(0.5, () => {
				AddressLabel.Text = AirFolioHttpServer.Prefix;
				this.navigationController.View.SetDeltaHeight(-20.0f);
				if (dialogViewController.Root[0] != SpacerSection)
				{
					dialogViewController.Root.Insert(0, UITableViewRowAnimation.Top, SpacerSection);
				}
			});
		}

		private void HideHttpAddress()
		{
			UIView.Animate(0.5, () => {
				this.navigationController.View.SetDeltaHeight(20.0f);
				var dialogViewController = this.navigationController.TopViewController as DialogViewController;
				if (dialogViewController.Root[0] == SpacerSection) 
				{
					dialogViewController.Root.RemoveAt(0, UITableViewRowAnimation.Bottom);
				}
			});
		}
	}
}

