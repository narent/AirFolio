
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace NarenT.ContactCaster
{
	public partial class FileDetailViewController : DialogViewController
	{
		public FileDetailViewController (string filename) : base (UITableViewStyle.Plain, null, true)
		{
			Root = new RootElement (filename) {
				new Section (string.Empty){
					new StringElement ("Hello", () => {
						new UIAlertView ("Hola", "Thanks for tapping!", null, "Continue").Show (); 
					})
				}
			};

			this.SetToolbarItems(AppDelegate.ToolbarButtons, false);
			this.TableView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
		}
	}
}
