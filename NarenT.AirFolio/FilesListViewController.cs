
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;
using System.IO;
using MonoTouch.CoreGraphics;

namespace NarenT.AirFolio
{
	public partial class FilesListViewController : DialogViewController
	{
		public FilesListViewController () : base (UITableViewStyle.Plain, null)
		{
			this.Root = new RootElement ("Air Folio") {
				new Section (string.Empty){
					FileRepository.GetFiles().Select(this.CreateElement)
				},
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.SetToolbarItems(AppDelegate.ToolbarButtons, false);
			this.TableView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
			this.NavigationController.Toolbar.Translucent = true;
			this.NavigationController.Toolbar.Opaque = false;

			var rect = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
			UIGraphics.BeginImageContext(rect.Size);
			var context = UIGraphics.GetCurrentContext();
			context.SetFillColor(UIColor.Clear.CGColor);
			context.FillRect(rect);
			var image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			this.NavigationController.Toolbar.SetBackgroundImage(image, UIToolbarPosition.Any, UIBarMetrics.Default);

		}

		public void ShowFile(string filename) 
		{
			NavigationController.PushViewController(new FileDetailViewController(filename), true);
		}

		private Element CreateElement(FileInfo file)
		{
			var subtitle = "Size: " + GetSizeDisplayString(file.Length) + " Modified:" + file.LastWriteTimeUtc.ToLocalTime().ToString("dd-MM-yy hh:mm");
			var element = new StyledStringElement(file.Name, subtitle, UITableViewCellStyle.Subtitle) 
			{ 
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};
			element.Tapped += () => { 
				this.NavigationController.PushViewController(new FileDetailViewController(file.FullName), true); 
			};
			return element;
		}

		private string GetSizeDisplayString(long length)
		{
			if (length < 1024) {
				return length.ToString() + "b";
			}

			if (length < 1048576) {
				return (length/1024).ToString() + "kb";
			}

			return (length/1048576).ToString() + "mb";
		}
	}
}
