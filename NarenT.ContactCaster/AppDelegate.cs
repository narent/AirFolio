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
using NarenT.ContactCaster.HttpActions;
using MonoTouch.Dialog;
using System.Drawing;
using NarenT.Extensions;

namespace NarenT.ContactCaster
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		private UIWindow window;
		private UINavigationController navigationController;
		private static UIBarButtonItem StartHttpServerButton;
		private HttpServer AirDrive2HttpServer;

		public static UIView NavigationBarTitleView;
		public static UIBarButtonItem[] ToolbarButtons;
		
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			StartHttpServerButton = new UIBarButtonItem(UIBarButtonSystemItem.Play, (sender, args) => { this.StartButtonTapped(); });
			ToolbarButtons = new[] { 
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), 
				StartHttpServerButton,
				new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) 
			};

			NavigationBarTitleView = new UIView(new RectangleF(0, 0, 320, 44));
			var titleLabel = new UILabel();
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.Text = "Air Drive 2";
			titleLabel.Font = UIFont.BoldSystemFontOfSize(23.0f);
			titleLabel.ShadowColor = UIColor.FromWhiteAlpha(0.0f, 0.5f);
			titleLabel.BackgroundColor = UIColor.Clear;
			titleLabel.TextColor = UIColor.White;
			titleLabel.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin;
			titleLabel.SizeToFit();

			titleLabel.Center = NavigationBarTitleView.Center;
			titleLabel.SetDeltaPosition(0.0f, -8.0f);

			var httpAddressLabel = new UILabel();
			titleLabel.TextAlignment = UITextAlignment.Center;
			httpAddressLabel.Text = "http://localhost:8080/index.html";
			httpAddressLabel.BackgroundColor = UIColor.Clear;
			httpAddressLabel.Font = UIFont.SystemFontOfSize(UIFont.SystemFontSize);
			httpAddressLabel.SizeToFit();
			httpAddressLabel.Center = NavigationBarTitleView.Center;
			httpAddressLabel.SetDeltaPosition(0.0f, 10.0f);

			NavigationBarTitleView.AddSubview(titleLabel);
			NavigationBarTitleView.AddSubview(httpAddressLabel);

			window = new UIWindow (UIScreen.MainScreen.Bounds);

			var controller = new FilesListViewController ();
			navigationController = new UINavigationController (controller);
			navigationController.ToolbarHidden = false;
			window.RootViewController = navigationController;

			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}

		private void StartButtonTapped()
		{
			if (this.AirDrive2HttpServer == null || !this.AirDrive2HttpServer.IsListening)
			{
				this.StartHttpServer();
				StartHttpServerButton.TintColor = UIColor.Red;
			} 
			else 
			{
				this.StopHttpServer();
				StartHttpServerButton.TintColor = UIColor.Gray;
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

			this.AirDrive2HttpServer = new HttpServer(UrlScheme.http, "localhost", 8080, "/");
			Console.WriteLine ("http server running on " + ipAddress + ":8080");
			this.AirDrive2HttpServer.Actions.Add(new FilesAction());
			this.AirDrive2HttpServer.Actions.Add(new StaticFileAction());
			this.AirDrive2HttpServer.Start();
		}

		private void StopHttpServer() 
		{
			if (this.AirDrive2HttpServer != null && this.AirDrive2HttpServer.IsListening) {
				this.AirDrive2HttpServer.Stop();
			}
		}
	}
}

