
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace NarenT.ContactCaster
{
	public partial class FilesListViewController : DialogViewController
	{
		public FilesListViewController () : base (UITableViewStyle.Plain, null)
		{
			this.Root = new RootElement ("Air Drive 2") {
				new Section (string.Empty){
					new StringElement ("Assignment.doc", () => {
						new UIAlertView ("Hola", "Assignment.doc", null, "Continue").Show ();
					}),
					new StringElement ("Acme-Contract.pdf", () => {
						new UIAlertView ("Hola", "Acme-Contract.pdf", null, "Continue").Show ();
					}),
					new StringElement ("LA-NY Flight.pdf", () => {
						new UIAlertView ("Hola", "LA-NY Flight.pdf", null, "Continue").Show ();
					})
				},
			};

			this.NavigationItem.TitleView = AppDelegate.NavigationBarTitleView;
			this.SetToolbarItems(AppDelegate.ToolbarButtons, false);
		}
	}
}
