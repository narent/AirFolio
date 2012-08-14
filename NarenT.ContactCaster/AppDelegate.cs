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
		private static UILabel HttpAddressLabel;
		private static UILabel TitleLabel;
		public static UIView NavigationBarTitleView;
		public static UIBarButtonItem[] ToolbarButtons;

		// class-level declarations
		private UIWindow window;
		private UINavigationController navigationController;
		private static UIBarButtonItem StartHttpServerButton;
		private HttpServer AirDrive2HttpServer;
		
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
			TitleLabel = new UILabel();
			TitleLabel.TextAlignment = UITextAlignment.Center;
			TitleLabel.Text = "Air Drive 2";
			TitleLabel.Font = UIFont.BoldSystemFontOfSize(23.0f);
			TitleLabel.ShadowColor = UIColor.FromWhiteAlpha(0.0f, 0.5f);
			TitleLabel.BackgroundColor = UIColor.Clear;
			TitleLabel.TextColor = UIColor.White;
			TitleLabel.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin;
			TitleLabel.SizeToFit();

			TitleLabel.Center = NavigationBarTitleView.Center;

			HttpAddressLabel = new UILabel();
			HttpAddressLabel.TextAlignment = UITextAlignment.Center;
			//HttpAddressLabel.Text = "http://localhost:8080/index.html";
			HttpAddressLabel.BackgroundColor = UIColor.Clear;
			HttpAddressLabel.Font = UIFont.SystemFontOfSize(UIFont.SystemFontSize);
			HttpAddressLabel.SizeToFit();
			HttpAddressLabel.Center = NavigationBarTitleView.Center;
			HttpAddressLabel.SetDeltaPosition(0.0f, 10.0f);

			NavigationBarTitleView.AddSubview(TitleLabel);
			NavigationBarTitleView.AddSubview(HttpAddressLabel);

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

		private void ShowHttpAddress()
		{
			HttpAddressLabel.Text = this.AirDrive2HttpServer.Prefix;
			HttpAddressLabel.SizeToFit();
			HttpAddressLabel.Center = NavigationBarTitleView.Center;
			HttpAddressLabel.Alpha = 0.0f;
			HttpAddressLabel.SetDeltaPosition(0.0f, 10.0f);
			UIView.Animate(0.5, () => {
				HttpAddressLabel.Alpha = 1.0f;
				TitleLabel.SetDeltaPosition(0.0f, -8.0f);
			});
		}

		private void HideHttpAddress()
		{
			UIView.Animate(0.5, () => {
				TitleLabel.SetDeltaPosition(0.0f, 8.0f);
				HttpAddressLabel.Alpha = 0.0f;
			});
		}
	}
}

