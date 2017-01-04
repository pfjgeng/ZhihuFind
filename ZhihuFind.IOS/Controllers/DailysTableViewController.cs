using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using System.IO;

namespace ZhihuFind.IOS.Controllers
{
    public class DailysTableViewController : UITableViewController
    {

        protected List<string> people = new List<string>();
        protected TableSource tableSource;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            for (int i = 0; i < 5; i++)
            {
                people.Add(i.ToString());
            }

            tableSource = new DailysTableViewController.TableSource(people);

            // initialize the table view and set the source
            TableView = new UITableView();
            TableView.Source = tableSource;
        }

        protected class TableSource : UITableViewSource
        {
            List<string> items;

            public TableSource(List<string> items) : base() { this.items = items; }

            public override nint NumberOfSections(UITableView tableView) { return 1; }

            public override nint RowsInSection(UITableView tableview, nint section) { return this.items.Count; }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                UITableViewCell cell;
                cell = tableView.DequeueReusableCell("item");
                if (cell == null)
                    cell = new UITableViewCell(UITableViewCellStyle.Default, "item");
                cell.TextLabel.Text = this.items[indexPath.Row];
                return cell;
            }

        }

    }
}