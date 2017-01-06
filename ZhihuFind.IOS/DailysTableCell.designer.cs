// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ZhihuFind.IOS
{
    [Register ("DailysTableCell")]
    partial class DailysTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CellDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView CellImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CellTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CellDescription != null) {
                CellDescription.Dispose ();
                CellDescription = null;
            }

            if (CellImage != null) {
                CellImage.Dispose ();
                CellImage = null;
            }

            if (CellTitle != null) {
                CellTitle.Dispose ();
                CellTitle = null;
            }
        }
    }
}