
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
	public partial class FilesListViewController : DialogViewController
	{
		public FilesListViewController () : base (UITableViewStyle.Grouped, null)
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
		}

		public void ShowFile(string filename) 
		{
			NavigationController.PushViewController(new FileDetailViewController(filename), true);
		}

		private Element CreateElement(FileInfo file)
		{
			return new StyledStringElement(file.Name, () => { ShowFile(file.Name); }) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
		}
	}
}
