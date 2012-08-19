
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Drawing;

namespace NarenT.ContactCaster
{
	public partial class FilesListViewController : DialogViewController
	{
		public FilesListViewController () : base (UITableViewStyle.Grouped, null)
		{
			this.Root = new RootElement ("Air Folio") {
				new Section (string.Empty){
					CreateElement ("Assignment.doc"),
					CreateElement ("Acme-Contract.pdf"),
					CreateElement ("LA-NY Flight.pdf")
				},
			};
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.SetToolbarItems(AppDelegate.ToolbarButtons, false);
			this.TableView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
		}

		public void ShowFile(string filename) 
		{
			NavigationController.PushViewController(new FileDetailViewController(filename), true);
		}

		private StyledStringElement CreateElement(string filename)
		{
			return new StyledStringElement(filename, () => { ShowFile(filename); }) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
		}
	}
}
