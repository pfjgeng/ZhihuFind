using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace ZhihuFind.IOS.Classes
{
    public class GrowRowTableDelegate : UITableViewDelegate
    {
        #region Private Variables
        private DailysViewController Controller;
        #endregion

        #region Constructors
        public GrowRowTableDelegate()
        {
        }

        public GrowRowTableDelegate(DailysViewController controller)
        {
            // Initialize
            this.Controller = controller;
        }
        #endregion

        #region Override Methods
        public override nfloat EstimatedHeight(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return 40f;
        }
        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            // Output selected row
            Console.WriteLine("Row selected: {0}", indexPath.Row);
        }
        #endregion
    }
}
