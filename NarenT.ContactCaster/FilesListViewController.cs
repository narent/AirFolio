
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
		public FilesListViewController () : base (UITableViewStyle.Grouped, null)
		{
			this.Root = new RootElement ("Air Drive 2") {
				new Section (string.Empty){
					new StyledStringElement ("Assignment.doc") { Accessory = UITableViewCellAccessory.DisclosureIndicator },
					new StyledStringElement ("Acme-Contract.pdf") { Accessory = UITableViewCellAccessory.DisclosureIndicator },
					new StyledStringElement ("LA-NY Flight.pdf") { Accessory = UITableViewCellAccessory.DisclosureIndicator }
				},
			};

			this.NavigationItem.TitleView = AppDelegate.NavigationBarTitleView;
			this.SetToolbarItems(AppDelegate.ToolbarButtons, false);
			this.TableView.BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
		}
	}
}
