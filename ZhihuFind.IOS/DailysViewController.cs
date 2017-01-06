using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using ZhihuFind.IOS.Classes;

namespace ZhihuFind.IOS
{
    public partial class DailysViewController : UITableViewController
    {
        #region Computed Properties
        public GrowRowTableDataSource DataSource
        {
            get { return TableView.DataSource as GrowRowTableDataSource; }
        }

        public GrowRowTableDelegate TableDelegate
        {
            get { return TableView.Delegate as GrowRowTableDelegate; }
        }
        #endregion

        #region Constructors
        public DailysViewController(IntPtr handle) : base (handle)
		{
        }
        #endregion

        #region Override Methods
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Initialize table
            TableView.DataSource = new GrowRowTableDataSource(this);
            TableView.Delegate = new GrowRowTableDelegate(this);
            TableView.EstimatedRowHeight = 40f;
            TableView.ReloadData();
        }


        #endregion
    }
}