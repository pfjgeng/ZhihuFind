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
using Android.Graphics;
using Java.Lang;
using Android.Util;
using Android.Graphics.Drawables;

namespace PhontView
{
    public class PhotoView : ImageView
    {
        private static int MIN_ROTATE = 35;
        private static int ANIMA_DURING = 340;
        private static float MAX_SCALE = 2.5f;

        private int mMinRotate;
        private int mAnimaDuring;
        private float mMaxScale;

        private int MAX_OVER_SCROLL = 0;
        private int MAX_FLING_OVER_SCROLL = 0;
        private int MAX_OVER_RESISTANCE = 0;
        private int MAX_ANIM_FROM_WAITE = 500;

        private Matrix mBaseMatrix = new Matrix();
        private Matrix mAnimaMatrix = new Matrix();
        private Matrix mSynthesisMatrix = new Matrix();
        private Matrix mTmpMatrix = new Matrix();

        private RotateGestureDetector mRotateDetector;
        private GestureDetector mDetector;
        private ScaleGestureDetector mScaleDetector;
        private IOnClickListener mClickListener;

        private ScaleType mScaleType;

        private bool hasMultiTouch;
        private bool hasDrawable;
        private bool isKnowSize;
        private bool hasOverTranslate;
        private bool isEnable = false;
        private bool isRotateEnable = false;
        private bool isInit;
        private bool mAdjustViewBounds;
        // 当前是否处于放大状态
        private bool isZoonUp;
        private bool canRotate;

        private bool imgLargeWidth;
        private bool imgLargeHeight;

        private float mRotateFlag;
        private float mDegrees;
        private float mScale = 1.0f;
        private int mTranslateX;
        private int mTranslateY;

        private float mHalfBaseRectWidth;
        private float mHalfBaseRectHeight;

        private RectF mWidgetRect = new RectF();
        private RectF mBaseRect = new RectF();
        private RectF mImgRect = new RectF();
        private RectF mTmpRect = new RectF();
        private RectF mCommonRect = new RectF();

        private PointF mScreenCenter = new PointF();
        private PointF mScaleCenter = new PointF();
        private PointF mRotateCenter = new PointF();

        private Transform mTranslate = new Transform();

        private RectF mClip;
        private Info mFromInfo;
        private long mInfoTime;
        private Runnable mCompleteCallBack;

        private IOnLongClickListener mLongClick;

        public PhotoView(Context context) : base(context)
        {
            init();
        }

        public PhotoView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public PhotoView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init();
        }
        private void init()
        {
            base.SetScaleType(ScaleType.Matrix);
            if (mScaleType == null) mScaleType = ScaleType.CenterInside;
            mRotateDetector = new RotateGestureDetector(mRotateListener);
            mDetector = new GestureDetector(Context, mGestureListener);
            mScaleDetector = new ScaleGestureDetector(Context, mScaleListener);
            float density = Resources.DisplayMetrics.Density;
            MAX_OVER_SCROLL = (int)(density * 30);
            MAX_FLING_OVER_SCROLL = (int)(density * 30);
            MAX_OVER_RESISTANCE = (int)(density * 140);

            mMinRotate = MIN_ROTATE;
            mAnimaDuring = ANIMA_DURING;
            mMaxScale = MAX_SCALE;
        }
        /**
         * 获取默认的动画持续时间
         */
        public int GetDefaultAnimaDuring()
        {
            return ANIMA_DURING;
        }
        public override void SetOnClickListener(IOnClickListener l)
        {
            base.SetOnClickListener(l);
            mClickListener = l;
        }
        public override void SetScaleType(ScaleType scaleType)
        {
            base.SetScaleType(scaleType);
            if (scaleType == ScaleType.Matrix) return;

            if (scaleType != mScaleType)
            {
                mScaleType = scaleType;

                if (isInit)
                {
                    initBase();
                }
            }
        }
        public override void SetOnLongClickListener(IOnLongClickListener l)
        {
            mLongClick = l;
        }

        /**
         * 设置动画的插入器
         */
        public void SetInterpolator(Interpolator interpolator)
        {
            mTranslate.SetInterpolator(interpolator);
        }

        /**
         * 获取动画持续时间
         */
        public int GetAnimaDuring()
        {
            return mAnimaDuring;
        }

        /**
         * 设置动画的持续时间
         */
        public void SetAnimaDuring(int during)
        {
            mAnimaDuring = during;
        }

        /**
         * 设置最大可以缩放的倍数
         */
        public void SetMaxScale(float maxScale)
        {
            mMaxScale = maxScale;
        }

        /**
         * 获取最大可以缩放的倍数
         */
        public float GetMaxScale()
        {
            return mMaxScale;
        }

        /**
         * 启用缩放功能
         */
        public void Enable()
        {
            isEnable = true;
        }

        /**
         * 禁用缩放功能
         */
        public void Disenable()
        {
            isEnable = false;
        }

        /**
         * 启用旋转功能
         */
        public void EnableRotate()
        {
            isRotateEnable = true;
        }

        /**
         * 禁用旋转功能
         */
        public void DisableRotate()
        {
            isRotateEnable = false;
        }

        /**
         */
        public void SetMaxAnimFromWaiteTime(int wait)
        {
            MAX_ANIM_FROM_WAITE = wait;
        }
        public override void SetImageResource(int resId)
        {
            Drawable drawable = null;
            try
            {
                drawable = Resources.GetDrawable(resId);
            }
            catch (System.Exception e)
            {
            }
            SetImageDrawable(drawable);
        }
        public override void SetImageDrawable(Drawable drawable)
        {
            base.SetImageDrawable(drawable);
            if (drawable == null)
            {
                hasDrawable = false;
                return;
            }

            if (!hasSize(drawable))
                return;

            if (!hasDrawable)
            {
                hasDrawable = true;
            }

            initBase();
        }
        private bool hasSize(Drawable d)
        {
            if ((d.IntrinsicHeight <= 0 || d.IntrinsicWidth <= 0)
                    && (d.MinimumWidth <= 0 || d.MinimumHeight <= 0)
                    && (d.Bounds.Width() <= 0 || d.Bounds.Height() <= 0))
            {
                return false;
            }
            return true;
        }

        private static int getDrawableWidth(Drawable d)
        {
            int width = d.IntrinsicWidth;
            if (width <= 0) width = d.MinimumWidth;
            if (width <= 0) width = d.Bounds.Width();
            return width;
        }

        private static int getDrawableHeight(Drawable d)
        {
            int height = d.IntrinsicHeight;
            if (height <= 0) height = d.MinimumHeight;
            if (height <= 0) height = d.Bounds.Height();
            return height;
        }

        private void initBase()
        {
            if (!hasDrawable) return;
            if (!isKnowSize) return;

            mBaseMatrix.Reset();
            mAnimaMatrix.Reset();

            isZoonUp = false;

            Drawable img = Drawable;

            int w = Width;
            int h = Height;
            int imgw = getDrawableWidth(img);
            int imgh = getDrawableHeight(img);

            mBaseRect.Set(0, 0, imgw, imgh);

            // 以图片中心点居中位移
            int tx = (w - imgw) / 2;
            int ty = (h - imgh) / 2;

            float sx = 1;
            float sy = 1;

            // 缩放，默认不超过屏幕大小
            if (imgw > w)
            {
                sx = (float)w / imgw;
            }

            if (imgh > h)
            {
                sy = (float)h / imgh;
            }

            float scale = sx < sy ? sx : sy;

            mBaseMatrix.Reset();
            mBaseMatrix.PostTranslate(tx, ty);
            mBaseMatrix.PostScale(scale, scale, mScreenCenter.X, mScreenCenter.Y);
            mBaseMatrix.MapRect(mBaseRect);

            mHalfBaseRectWidth = mBaseRect.Width() / 2;
            mHalfBaseRectHeight = mBaseRect.Height() / 2;

            mScaleCenter.Set(mScreenCenter);
            mRotateCenter.Set(mScaleCenter);

            ExecuteTranslate();

            if (mScaleType == ScaleType.Center)
            {
                initCenter();
            }
            else if (mScaleType == ScaleType.CenterCrop)
            {
                initCenterCrop();
            }
            else if (mScaleType == ScaleType.CenterInside)
            {
                initCenterInside();
            }
            else if (mScaleType == ScaleType.FitCenter)
            {
                initFitCenter();
            }
            else if (mScaleType == ScaleType.FitEnd)
            {
                initFitEnd();
            }
            else if (mScaleType == ScaleType.FitStart)
            {
                initFitStart();
            }
            else if (mScaleType == ScaleType.FitXy)
            {
                initFitXY();
            }

            isInit = true;

            if (mFromInfo != null && DateTime.Now.Ticks - mInfoTime < MAX_ANIM_FROM_WAITE)
            {
                animaFrom(mFromInfo);
            }

            mFromInfo = null;
        }

        private void initCenter()
        {
            if (!hasDrawable) return;
            if (!isKnowSize) return;

            Drawable img = Drawable;

            int imgw = getDrawableWidth(img);
            int imgh = getDrawableHeight(img);

            if (imgw > mWidgetRect.Width() || imgh > mWidgetRect.Height())
            {
                float scaleX = imgw / mImgRect.Width();
                float scaleY = imgh / mImgRect.Height();

                mScale = scaleX > scaleY ? scaleX : scaleY;

                mAnimaMatrix.PostScale(mScale, mScale, mScreenCenter.X, mScreenCenter.Y);

                executeTranslate();

                resetBase();
            }
        }

        private void initCenterCrop()
        {
            if (mImgRect.Width() < mWidgetRect.Width() || mImgRect.Height() < mWidgetRect.Height())
            {
                float scaleX = mWidgetRect.Width() / mImgRect.Width();
                float scaleY = mWidgetRect.Height() / mImgRect.Height();

                mScale = scaleX > scaleY ? scaleX : scaleY;

                mAnimaMatrix.PostScale(mScale, mScale, mScreenCenter.X, mScreenCenter.Y);

                executeTranslate();
                resetBase();
            }
        }

        private void initCenterInside()
        {
            if (mImgRect.Width() > mWidgetRect.Width() || mImgRect.Height() > mWidgetRect.Height())
            {
                float scaleX = mWidgetRect.Width() / mImgRect.Width();
                float scaleY = mWidgetRect.Height() / mImgRect.Height();

                mScale = scaleX < scaleY ? scaleX : scaleY;

                mAnimaMatrix.PostScale(mScale, mScale, mScreenCenter.X, mScreenCenter.Y);

                executeTranslate();
                resetBase();
            }
        }

        private void initFitCenter()
        {
            if (mImgRect.Width() < mWidgetRect.Width())
            {
                mScale = mWidgetRect.Width() / mImgRect.Width();

                mAnimaMatrix.PostScale(mScale, mScale, mScreenCenter.X, mScreenCenter.Y);

                executeTranslate();
                resetBase();
            }
        }

        private void initFitStart()
        {
            initFitCenter();

            float ty = -mImgRect.Top;
            mAnimaMatrix.PostTranslate(0, ty);
            executeTranslate();
            resetBase();
            mTranslateY += (int)ty;
        }

        private void initFitEnd()
        {
            initFitCenter();

            float ty = (mWidgetRect.Bottom - mImgRect.Bottom);
            mTranslateY += (int)ty;
            mAnimaMatrix.PostTranslate(0, ty);
            executeTranslate();
            resetBase();
        }

        private void initFitXY()
        {
            float scaleX = mWidgetRect.Width() / mImgRect.Width();
            float scaleY = mWidgetRect.Height() / mImgRect.Height();

            mAnimaMatrix.PostScale(scaleX, scaleY, mScreenCenter.X, mScreenCenter.Y);

            executeTranslate();
            resetBase();
        }

        private void resetBase()
        {
            Drawable img = Drawable;
            int imgw = getDrawableWidth(img);
            int imgh = getDrawableHeight(img);
            mBaseRect.Set(0, 0, imgw, imgh);
            mBaseMatrix.Set(mSynthesisMatrix);
            mBaseMatrix.MapRect(mBaseRect);
            mHalfBaseRectWidth = mBaseRect.Width() / 2;
            mHalfBaseRectHeight = mBaseRect.Height() / 2;
            mScale = 1;
            mTranslateX = 0;
            mTranslateY = 0;
            mAnimaMatrix.Reset();
        }

        private void executeTranslate()
        {
            mSynthesisMatrix.Set(mBaseMatrix);
            mSynthesisMatrix.PostConcat(mAnimaMatrix);
            setImageMatrix(mSynthesisMatrix);

            mAnimaMatrix.MapRect(mImgRect, mBaseRect);

            imgLargeWidth = mImgRect.Width() > mWidgetRect.Width();
            imgLargeHeight = mImgRect.Height() > mWidgetRect.Height();
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            if (!hasDrawable)
            {
                base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                return;
            }

            Drawable d = Drawable;
            int drawableW = getDrawableWidth(d);
            int drawableH = getDrawableHeight(d);

            int pWidth = MeasureSpec.GetSize(widthMeasureSpec);
            int pHeight = MeasureSpec.GetSize(heightMeasureSpec);

            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            int width = 0;
            int height = 0;

            ViewGroup.LayoutParams p = LayoutParameters;

            if (p == null)
            {
                p = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            }

            if (p.Width == ViewGroup.LayoutParams.MatchParent)
            {
                if (widthMode == MeasureSpecMode.Unspecified)
                {
                    width = drawableW;
                }
                else
                {
                    width = pWidth;
                }
            }
            else
            {
                if (widthMode == MeasureSpecMode.Exactly)
                {
                    width = pWidth;
                }
                else if (widthMode == MeasureSpecMode.AtMost)
                {
                    width = drawableW > pWidth ? pWidth : drawableW;
                }
                else
                {
                    width = drawableW;
                }
            }

            if (p.Height == ViewGroup.LayoutParams.MatchParent)
            {
                if (heightMode == MeasureSpecMode.Unspecified)
                {
                    height = drawableH;
                }
                else
                {
                    height = pHeight;
                }
            }
            else
            {
                if (heightMode == MeasureSpecMode.Exactly)
                {
                    height = pHeight;
                }
                else if (heightMode == MeasureSpecMode.AtMost)
                {
                    height = drawableH > pHeight ? pHeight : drawableH;
                }
                else
                {
                    height = drawableH;
                }
            }

            if (mAdjustViewBounds && (float)drawableW / drawableH != (float)width / height)
            {

                float hScale = (float)height / drawableH;
                float wScale = (float)width / drawableW;

                float scale = hScale < wScale ? hScale : wScale;
                width = p.Width == ViewGroup.LayoutParams.MatchParent ? width : (int)(drawableW * scale);
                height = p.Height == ViewGroup.LayoutParams.MatchParent ? height : (int)(drawableH * scale);
            }

            SetMeasuredDimension(width, height);
        }
        public override void SetAdjustViewBounds(bool adjustViewBounds)
        {
            base.SetAdjustViewBounds(adjustViewBounds);
            mAdjustViewBounds = adjustViewBounds;
        }
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            mWidgetRect.Set(0, 0, w, h);
            mScreenCenter.Set(w / 2, h / 2);

            if (!isKnowSize)
            {
                isKnowSize = true;
                initBase();
            }
        }
        public override void Draw(Canvas canvas)
        {
            if (mClip != null)
            {
                canvas.ClipRect(mClip);
                mClip = null;
            }
            base.Draw(canvas);
        }
        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (isEnable)
            {
                MotionEventActions Action = e.ActionMasked;
                if (e.PointerCount >= 2) hasMultiTouch = true;

                mDetector.OnTouchEvent(e);
                if (isRotateEnable)
                {
                    mRotateDetector.onTouchEvent(e);
                }
                mScaleDetector.OnTouchEvent(e);

                if (Action == MotionEventActions.Up || Action == MotionEventActions.Cancel) onUp();

                return true;
            }
            else
            {
                return base.DispatchTouchEvent(e);
            }
        }
        private void onUp()
        {
            if (mTranslate.IsRuning) return;

            if (canRotate || mDegrees % 90 != 0)
            {
                float toDegrees = (int)(mDegrees / 90) * 90;
                float remainder = mDegrees % 90;

                if (remainder > 45)
                    toDegrees += 90;
                else if (remainder < -45)
                    toDegrees -= 90;

                mTranslate.withRotate((int)mDegrees, (int)toDegrees);

                mDegrees = toDegrees;
            }

            float scale = mScale;

            if (mScale < 1)
            {
                scale = 1;
                mTranslate.withScale(mScale, 1);
            }
            else if (mScale > mMaxScale)
            {
                scale = mMaxScale;
                mTranslate.withScale(mScale, mMaxScale);
            }

            float cx = mImgRect.Left + mImgRect.Width() / 2;
            float cy = mImgRect.Top + mImgRect.Height() / 2;

            mScaleCenter.Set(cx, cy);
            mRotateCenter.Set(cx, cy);

            mTranslateX = 0;
            mTranslateY = 0;

            mTmpMatrix.Reset();
            mTmpMatrix.PostTranslate(-mBaseRect.Left, -mBaseRect.Top);
            mTmpMatrix.PostTranslate(cx - mHalfBaseRectWidth, cy - mHalfBaseRectHeight);
            mTmpMatrix.PostScale(scale, scale, cx, cy);
            mTmpMatrix.PostRotate(mDegrees, cx, cy);
            mTmpMatrix.MapRect(mTmpRect, mBaseRect);

            oTranslateReset(mTmpRect);
            mTranslate.start();
        }
        private void doTranslateReset(RectF imgRect)
        {
            int tx = 0;
            int ty = 0;

            if (imgRect.Width() <= mWidgetRect.Width())
            {
                if (!isImageCenterWidth(imgRect))
                    tx = -(int)((mWidgetRect.Width() - imgRect.Width()) / 2 - imgRect.Left);
            }
            else
            {
                if (imgRect.Left > mWidgetRect.Left)
                {
                    tx = (int)(imgRect.Left - mWidgetRect.Left);
                }
                else if (imgRect.Right < mWidgetRect.Right)
                {
                    tx = (int)(imgRect.Right - mWidgetRect.Right);
                }
            }

            if (imgRect.Height() <= mWidgetRect.Height())
            {
                if (!isImageCenterHeight(imgRect))
                    ty = -(int)((mWidgetRect.Height() - imgRect.Height()) / 2 - imgRect.Top);
            }
            else
            {
                if (imgRect.Top > mWidgetRect.Top)
                {
                    ty = (int)(imgRect.Top - mWidgetRect.Top);
                }
                else if (imgRect.Bottom < mWidgetRect.Bottom)
                {
                    ty = (int)(imgRect.Bottom - mWidgetRect.Bottom);
                }
            }

            if (tx != 0 || ty != 0)
            {
                if (!mTranslate.mFlingScroller.isFinished()) mTranslate.mFlingScroller.abortAnimation();
                mTranslate.withTranslate(mTranslateX, mTranslateY, -tx, -ty);
            }
        }

        private bool isImageCenterHeight(RectF rect)
        {
            return Java.Lang.Math.Abs(Java.Lang.Math.Round(rect.Top) - (mWidgetRect.Height() - rect.Height()) / 2) < 1;
        }

        private bool isImageCenterWidth(RectF rect)
        {
            return Java.Lang.Math.Abs(Java.Lang.Math.Round(rect.Left) - (mWidgetRect.Width() - rect.Width()) / 2) < 1;
        }

    }
}