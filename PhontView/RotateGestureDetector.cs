using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PhontView
{
    public class RotateGestureDetector
    {

        private static int MaxDegreesStep = 120;

        private IOnRotateListener listener;

        private float prevSlope;
        private float currSlope;

        private float x1;
        private float y1;
        private float x2;
        private float y2;

        public RotateGestureDetector(IOnRotateListener l)
        {
            listener = l;
        }

        public void onTouchEvent(MotionEvent e)
        {
            MotionEventActions action = e.ActionMasked;
            switch (action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Up:
                    if (e.PointerCount == 2) prevSlope = caculateSlope(e);
                    break;
                case MotionEventActions.Move:
                    if (e.PointerCount > 1)
                    {
                        currSlope = caculateSlope(e);

                        double currDegrees = (180 / Math.PI) * Math.Atan(currSlope);
                        double prevDegrees = (180 / Math.PI) * Math.Atan(prevSlope);

                        double deltaSlope = currDegrees - prevDegrees;

                        if (Math.Abs(deltaSlope) <= MaxDegreesStep)
                        {
                            listener.OnRotate((float)deltaSlope, (x2 + x1) / 2, (y2 + y1) / 2);
                        }
                        prevSlope = currSlope;
                    }
                    break;
                default:
                    break;
            }
        }

        private float caculateSlope(MotionEvent e)
        {
            x1 = e.GetX(0);
            y1 = e.GetY(0);
            x2 = e.GetX(1);
            y2 = e.GetY(1);
            return (y2 - y1) / (x2 - x1);
        }
    }
}