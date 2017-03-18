using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace AndroidSlideLayout {

    // translated from FrameLayout(http://grepcode.com/file/repository.grepcode.com/java/ext/com.google.android/android/4.0.4_r1.2/android/widget/FrameLayout.java/)
    // Thanks AOSP
    // FrameLayout is under Apache License v2
    // Copyright (C) 2006 The Android Open Source Project

    /// <summary>
    /// The Layout ported FrameLayout to Xamarin
    /// </summary>
    public class FrameLayoutCompat : ViewGroup {

        private const GravityFlags defaultChildGravity = GravityFlags.Top | GravityFlags.Start;
        private static readonly int[] attr = new int[] { Android.Resource.Attribute.Foreground, Android.Resource.Attribute.MeasureAllChildren };

        public bool MeasureAllChildren { get; set; } = false;
        private Drawable _foreground;
        public new Drawable Foreground {
            get {
                return _foreground;
            }
            set {
                setForeground(value);
            }
        }

        private int foregroundPaddingLeft = 0;
        private int foregroundPaddingTop = 0;
        private int foregroundPaddingRight = 0;
        private int foregroundPaddingBottom = 0;

        private Rect selfBounds = new Rect();
        private Rect overlayBounds = new Rect();

        private GravityFlags foregroundGravity = GravityFlags.Fill;
        private bool foregroundInPadding = true;
        private bool foregroundBoundsChanged = false;

        private List<View> matchParentChildren = new List<View>();

        public FrameLayoutCompat(Context context) : base(context) { }

        public FrameLayoutCompat(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }

        public FrameLayoutCompat(Context context, IAttributeSet attrs, int defStyleAttr) : this(context, attrs, defStyleAttr, 0) { }

        public FrameLayoutCompat(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes) {
            var a = context.ObtainStyledAttributes(attrs, attr, defStyleAttr, defStyleRes);

            var d = a.GetDrawable(0);
            if (d != null) {
                setForeground(d);
            }
            if (a.GetBoolean(1, false)) {
                MeasureAllChildren = true;
            }
            a.Recycle();
        }

        public FrameLayoutCompat(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public override void SetForegroundGravity(GravityFlags gravitiy) {
            if (foregroundGravity != gravitiy) {
                if ((foregroundGravity & GravityFlags.RelativeHorizontalGravityMask) == 0) {
                    foregroundGravity |= GravityFlags.Start;
                }

                if ((foregroundGravity & GravityFlags.VerticalGravityMask) == 0) {
                    foregroundGravity |= GravityFlags.Top;
                }

                foregroundGravity = gravitiy;


                if (foregroundGravity == GravityFlags.Fill && Foreground != null) {
                    Rect padding = new Rect();
                    if (Foreground.GetPadding(padding)) {
                        foregroundPaddingLeft = padding.Left;
                        foregroundPaddingTop = padding.Top;
                        foregroundPaddingRight = padding.Right;
                        foregroundPaddingBottom = padding.Bottom;
                    }
                } else {
                    foregroundPaddingLeft = 0;
                    foregroundPaddingTop = 0;
                    foregroundPaddingRight = 0;
                    foregroundPaddingBottom = 0;
                }

                RequestLayout();
            }
        }

        protected override bool VerifyDrawable(Drawable who) {
            return base.VerifyDrawable(who) || (who == Foreground);
        }

        public override void JumpDrawablesToCurrentState() {
            base.JumpDrawablesToCurrentState();
            Foreground?.JumpToCurrentState();
        }

        protected override void DrawableStateChanged() {
            base.DrawableStateChanged();
            if (Foreground != null && Foreground.IsStateful) {
                Foreground.SetState(GetDrawableState());
            }
        }

        protected override ViewGroup.LayoutParams GenerateDefaultLayoutParams() {
            return new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
        }

        private void setForeground(Drawable drawable) {
            if (Foreground != drawable) {
                if (Foreground != null) {
                    Foreground.SetCallback(null);
                    UnscheduleDrawable(Foreground);
                }

                _foreground = drawable;
                foregroundPaddingLeft = 0;
                foregroundPaddingTop = 0;
                foregroundPaddingRight = 0;
                foregroundPaddingBottom = 0;

                if (drawable != null) {
                    SetWillNotDraw(false);
                    drawable.SetCallback(this);
                    if (drawable.IsStateful) {
                        drawable.SetState(GetDrawableState());
                    }
                    if (foregroundGravity == GravityFlags.Fill) {
                        Rect padding = new Rect();
                        if (drawable.GetPadding(padding)) {
                            foregroundPaddingLeft = padding.Left;
                            foregroundPaddingTop = padding.Top;
                            foregroundPaddingRight = padding.Right;
                            foregroundPaddingBottom = padding.Bottom;
                        }
                    }
                } else {
                    SetWillNotDraw(true);
                }
                RequestLayout();
                Invalidate();
            }
        }

        private int getPaddingLeftWithForeground() {
            return foregroundInPadding ? Math.Max(PaddingLeft, foregroundPaddingLeft) : PaddingLeft + foregroundPaddingLeft;
        }

        private int getPaddingRightWithForeground() {
            return foregroundInPadding ? Math.Max(PaddingRight, foregroundPaddingRight) : PaddingRight + foregroundPaddingRight;
        }

        private int getPaddingTopWithForeground() {
            return foregroundInPadding ? Math.Max(PaddingTop, foregroundPaddingTop) : PaddingTop + foregroundPaddingTop;
        }

        private int getPaddingBottomWithForeground() {
            return foregroundInPadding ? Math.Max(PaddingBottom, foregroundPaddingBottom) : PaddingBottom + foregroundPaddingBottom;
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec) {
            int count = ChildCount;

            bool measureMatchParentChildren = MeasureSpec.GetMode(widthMeasureSpec) != MeasureSpecMode.Exactly || MeasureSpec.GetMode(heightMeasureSpec) != MeasureSpecMode.Exactly;
            matchParentChildren.Clear();

            int maxHeight = 0;
            int maxWidth = 0;
            int childState = 0;

            for (int i = 0; i < count; i++) {
                var child = GetChildAt(i);
                if (MeasureAllChildren || child.Visibility != ViewStates.Gone) {
                    MeasureChildWithMargins(child, widthMeasureSpec, 0, heightMeasureSpec, 0);
                    var lp = (MarginLayoutParams)child.LayoutParameters;
                    maxWidth = Math.Max(maxWidth, child.MeasuredWidth + lp.LeftMargin + lp.RightMargin);
                    maxHeight = Math.Max(maxHeight, child.MeasuredHeight + lp.TopMargin + lp.BottomMargin);
                    childState = CombineMeasuredStates(childState, child.MeasuredState);
                    if (measureMatchParentChildren) {
                        if (lp.Width == LayoutParams.MatchParent || lp.Height == LayoutParams.MatchParent) {
                            matchParentChildren.Add(child);
                        }
                    }
                }
            }

            maxWidth += getPaddingLeftWithForeground() + getPaddingRightWithForeground();
            maxHeight += getPaddingTopWithForeground() + getPaddingBottomWithForeground();

            maxHeight = Math.Max(maxHeight, SuggestedMinimumHeight);
            maxWidth = Math.Max(maxWidth, SuggestedMinimumWidth);

            var drawable = Foreground;
            if (drawable != null) {
                maxHeight = Math.Max(maxHeight, drawable.MinimumHeight);
                maxWidth = Math.Max(maxWidth, drawable.MinimumWidth);
            }

            SetMeasuredDimension(ResolveSizeAndState(maxWidth, widthMeasureSpec, childState), ResolveSizeAndState(maxHeight, heightMeasureSpec, childState << MeasuredHeightStateShift));

            count = matchParentChildren.Count;
            if (count > 1) {
                for (int i = 0; i < count; i++) {
                    var child = matchParentChildren[i];
                    var lp = (MarginLayoutParams)child.LayoutParameters;

                    int childWidthMeasureSpec;
                    if (lp.Width == LayoutParams.MatchParent) {
                        int width = Math.Max(0, MeasuredWidth - getPaddingLeftWithForeground() - getPaddingRightWithForeground() - lp.LeftMargin - lp.RightMargin);
                        childWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly);
                    } else {
                        childWidthMeasureSpec = GetChildMeasureSpec(widthMeasureSpec, getPaddingLeftWithForeground() + getPaddingRightWithForeground() + lp.LeftMargin + lp.RightMargin, lp.Width);
                    }

                    int childHeightMeasureSpec;
                    if (lp.Height == LayoutParams.MatchParent) {
                        int height = Math.Max(0, MeasuredHeight - getPaddingTopWithForeground() - getPaddingBottomWithForeground() - lp.TopMargin - lp.BottomMargin);
                        childHeightMeasureSpec = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
                    } else {
                        childHeightMeasureSpec = GetChildMeasureSpec(heightMeasureSpec, getPaddingTopWithForeground() + getPaddingBottomWithForeground() + lp.TopMargin + lp.BottomMargin, lp.Height);
                    }

                    child.Measure(childWidthMeasureSpec, childHeightMeasureSpec);
                }
            }
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom) {
            int count = ChildCount;

            int parentLeft = getPaddingLeftWithForeground();
            int parentRight = right - left - getPaddingRightWithForeground();

            int parentTop = getPaddingTopWithForeground();
            int parentBottom = bottom - top - getPaddingBottomWithForeground();

            foregroundBoundsChanged = true;

            for (int i = 0; i < count; i++) {
                var child = GetChildAt(i);
                if (child.Visibility != ViewStates.Gone) {
                    var lp = (MarginLayoutParams)child.LayoutParameters;

                    int width = child.MeasuredWidth;
                    int height = child.MeasuredHeight;

                    int childLeft;
                    int childTop;

                    GravityFlags gravity = (lp as LayoutParams)?.Gravity ?? GravityFlags.NoGravity;
                    if (gravity == GravityFlags.NoGravity) {
                        gravity = defaultChildGravity;
                    }

                    // not fit code
                    //var layoutDirection = LayoutDirections.Locale;
                    var absoluteGravity = Gravity.GetAbsoluteGravity(gravity, GravityFlags.RelativeLayoutDirection);
                    var verticalGravity = gravity & GravityFlags.VerticalGravityMask;

                    switch (absoluteGravity & GravityFlags.HorizontalGravityMask) {
                        case GravityFlags.CenterHorizontal:
                            childLeft = parentLeft + (parentRight - parentLeft - width) / 2 +
                            lp.LeftMargin - lp.RightMargin;
                            break;
                        case GravityFlags.Right:
                            childLeft = parentRight - width - lp.RightMargin;
                            break;
                        case GravityFlags.Left:
                        default:
                            childLeft = parentLeft + lp.LeftMargin;
                            break;
                    }

                    switch (verticalGravity) {
                        case GravityFlags.Top:
                            childTop = parentTop + lp.TopMargin;
                            break;
                        case GravityFlags.CenterVertical:
                            childTop = parentTop + (parentBottom - parentTop - height) / 2 +
                            lp.TopMargin - lp.BottomMargin;
                            break;
                        case GravityFlags.Bottom:
                            childTop = parentBottom - height - lp.BottomMargin;
                            break;
                        default:
                            childTop = parentTop + lp.TopMargin;
                            break;
                    }

                    child.Layout(childLeft, childTop, childLeft + width, childTop + height);
                }
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh) {
            base.OnSizeChanged(w, h, oldw, oldh);
            foregroundBoundsChanged = true;
        }

        public override void Draw(Canvas canvas) {
            base.Draw(canvas);
            if (Foreground != null) {
                Drawable foreground = Foreground;

                if (foregroundBoundsChanged) {
                    foregroundBoundsChanged = false;
                    Rect selfBounds = this.selfBounds;
                    Rect overlayBounds = this.overlayBounds;

                    int w = Right - Left;
                    int h = Bottom - Top;

                    if (foregroundInPadding) {
                        selfBounds.Set(0, 0, w, h);
                    } else {
                        selfBounds.Set(PaddingLeft, PaddingTop, w - PaddingRight, h - PaddingBottom);
                    }

                    //not fit code
                    int layoutDirection = (int)LayoutDirections.Locale;
                    Gravity.Apply(foregroundGravity, foreground.IntrinsicWidth, foreground.IntrinsicHeight, selfBounds, overlayBounds, layoutDirection);
                    foreground.SetBounds(overlayBounds.Left, overlayBounds.Top, overlayBounds.Right, overlayBounds.Bottom);
                }

                foreground.Draw(canvas);
            }
        }

        public override bool GatherTransparentRegion(Region region) {
            bool opaque = base.GatherTransparentRegion(region);
            if (region != null && Foreground != null) {
                ApplyDrawableToTransparentRegion(Foreground, region);
            }
            return opaque;
        }

        public void ApplyDrawableToTransparentRegion(Drawable dr, Region region) {
            Region r = dr.TransparentRegion;
            Rect db = dr.Bounds;
            if (r != null) {
                var location = new int[2];
                GetLocationInWindow(location);
                r.Translate(location[0], location[1]);
                region.InvokeOp(r, Region.Op.Intersect);
            } else {
                region.InvokeOp(db, Region.Op.Difference);
            }
        }

        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs) {
            return new LayoutParams(Context, attrs);
        }

        public override bool ShouldDelayChildPressedState() {
            return false;
        }

        protected override bool CheckLayoutParams(ViewGroup.LayoutParams p) {
            return p is LayoutParams;
        }

        protected override ViewGroup.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams p) {
            if (p is LayoutParams) {
                return new LayoutParams((LayoutParams)p);
            } else if (p is MarginLayoutParams) {
                return new LayoutParams((MarginLayoutParams)p);
            }
            return new LayoutParams(p);
        }

        public new class LayoutParams : MarginLayoutParams {

            private const GravityFlags unspecifiedGravity = GravityFlags.NoGravity;

            public GravityFlags Gravity { get; } = unspecifiedGravity;

            public LayoutParams(Context c, IAttributeSet attrs) : base(c, attrs) {
                var a = c.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.LayoutGravity });
                int gravity = a.GetInt(0, (int)unspecifiedGravity);
                a.Recycle();
                Gravity = (GravityFlags)Enum.ToObject(typeof(GravityFlags), gravity);
            }

            public LayoutParams(int width, int height) : base(width, height) { }

            public LayoutParams(int width, int height, GravityFlags gravity) : this(width, height) {
                Gravity = gravity;
            }

            public LayoutParams(ViewGroup.LayoutParams source) : base(source) { }

            public LayoutParams(MarginLayoutParams source) : base(source) { }

            public LayoutParams(LayoutParams source) : base(source) {
                Gravity = source.Gravity;
            }

            public LayoutParams(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        }
    }
}