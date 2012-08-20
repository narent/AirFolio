using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using System.IO;

namespace NarenT.AirFolio
{
	public partial class FileDetailViewController : UIViewController
	{
		public FileDetailViewController (string filePath)
		{
			var webView = new UIWebView(new RectangleF(0.0f, -44.0f, 320.0f, 460.0f));
			webView.ScalesPageToFit = true;
			webView.ScrollView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
			webView.LoadRequest(new NSUrlRequest(new NSUrl(filePath, false)));
			this.View = webView;
			this.SetToolbarItems(AppDelegate.ToolbarButtons, false);
			this.NavigationItem.Title = Path.GetFileName(filePath);
		}
	}
}