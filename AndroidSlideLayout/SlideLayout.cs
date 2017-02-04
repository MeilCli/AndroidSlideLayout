using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AndroidSlideLayout {


    // エラー: SlideLayoutのgenerateDefaultLayoutParams()はFrameLayoutのgenerateDefaultLayoutParams()をオーバ�Eライドできません
    // public android.view.ViewGroup.LayoutParams generateDefaultLayoutParams () 戻り�?�の型android.view.ViewGroup.LayoutParamsはandroid.widget.FrameLayout.LayoutParamsと互換性がありません
    //
    // ↑ なんなんだよこれ
    // ↓ ということでFrameLayout移植で応急処置
    public class SlideLayout : FrameLayoutCompat, IDragCallback {

        /// <summary>
        /// A view is not currently being dragged or animating as a result of a fling/snap.
        /// </summary>
        public const int StateIdle = ViewDragHelper.StateIdle;

        /// <summary>
        /// A view is currently being dragged.
        /// </summary>
        public const int StateDragging = ViewDragHelper.StateDragging;

        /// <summary>
        /// A view is currently settling into place as a result of a fling or predefined non-interactive motion.
        /// </summary>
        public const int StateSetting = ViewDragHelper.StateSettling;

        private const float sensitivity = 1.0f;


        /// <summary>
        /// Event of a child view is captured for dragging or settling.
        /// </summary>
        public event EventHandler<ViewCapturedEventArgs> ViewCaptured;

        /// <summary>
        /// Event of the captured view position changed.
        /// </summary>
        public event EventHandler<ViewPositionChangedEventArgs> ViewPositionChanged;

        /// <summary>
        /// Event of the captured view is no longer being actively dragged.
        /// </summary>
        public event EventHandler<ViewReleasedEventArgs> ViewReleased;

        /// <summary>
        /// Event of the drag state changed.
        /// </summary>
        public event EventHandler<ViewDragStateChangedEventArgs> ViewDragStateChanged;

        private ViewDragHelper viewDragHelper;
        private Dictionary<View,Point> layoutedChildViewPosition = new Dictionary<View,Point>();
        private Dictionary<View,Point> childViewPosition = new Dictionary<View,Point>();


        /// <summary>
        /// Current drag captured view's layouted top position
        /// </summary>
        public int CurrentDragChildViewLayoutedTop { get; private set; }

        /// <summary>
        /// Current drag captured view's layouted left position
        /// </summary>
        public int CurrentDragChildViewLayoutedLeft { get; private set; }

        /// <summary>
        /// Current drag captured view's top position
        /// </summary>
        public int CurrentDragChildViewTop { get; private set; }

        /// <summary>
        /// Current drag captured view's left position
        /// </summary>
        public int CurrentDragChildViewLeft { get; private set; }

        /// <summary>
        /// Current drag captured view's top position in drag motion
        /// </summary>
        public int CurrentDragChildViewDraggedTop { get; private set; }

        /// <summary>
        /// Current drag captured view's left position in drag motion
        /// </summary>
        public int CurrentDragChildViewDraggedLeft { get; private set; }

        public SlideLayout(Context context) : this(context,null) { }

        public SlideLayout(Context context,IAttributeSet attr) : this(context,attr,0) { }

        public SlideLayout(Context context,IAttributeSet attr,int defStyle) : base(context,attr,defStyle) {
            setUpViewDragHelper();
        }

        public SlideLayout(IntPtr javaReference,JniHandleOwnership transfer) : base(javaReference,transfer) { }

        private void setUpViewDragHelper() {
            if(viewDragHelper != null) {
                return;
            }
            viewDragHelper = ViewDragHelper.Create(this,sensitivity,new ViewDragHelperCallback(this));
        }

        protected override void OnAttachedToWindow() {
            setUpViewDragHelper();
            base.OnAttachedToWindow();
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev) {
            switch(ev.Action) {
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    viewDragHelper.Cancel();
                    return false;
            }
            return viewDragHelper.ShouldInterceptTouchEvent(ev);
        }

        public override bool OnTouchEvent(MotionEvent e) {
            viewDragHelper.ProcessTouchEvent(e);
            return true;
        }

        public override void ComputeScroll() {
            base.ComputeScroll();
            if(viewDragHelper.ContinueSettling(true)) {
                Invalidate();
            }
        }

        protected override void OnLayout(bool changed,int left,int top,int right,int bottom) {
            base.OnLayout(changed,left,top,right,bottom);
            layoutedChildViewPosition.Clear();
            for(int i = 0;i < ChildCount;i++) {
                var child = GetChildAt(i);
                layoutedChildViewPosition.Add(child,new Point(child.Left,child.Top));
            }
        }

        protected override void OnDetachedFromWindow() {
            base.OnDetachedFromWindow();
            layoutedChildViewPosition.Clear();
            childViewPosition.Clear();
        }

        public virtual bool TryCaptureView(View child,int pointerId) {
            var parameters = child.LayoutParameters as LayoutParams;
            if(parameters == null) {
                return false;
            }
            return parameters.IsDraggableTopDirection || parameters.IsDraggableBottomDirection || parameters.IsDraggableLeftDirection || parameters.IsDraggableRightDirection;
        }

        /// <summary>
        /// Called when a child view is captured for dragging or settling.
        /// </summary>
        /// <param name="capturedChild">Child view that was captured</param>
        /// <param name="activePointerId">Pointer id tracking the child capture</param>
        public virtual void OnViewCaptured(View capturedChild,int activePointerId) {
            CurrentDragChildViewLayoutedLeft = layoutedChildViewPosition[capturedChild].X;
            CurrentDragChildViewLayoutedTop = layoutedChildViewPosition[capturedChild].Y;
            if(childViewPosition.ContainsKey(capturedChild)) {
                CurrentDragChildViewLeft = childViewPosition[capturedChild].X;
                CurrentDragChildViewTop = childViewPosition[capturedChild].Y;
            } else {
                CurrentDragChildViewLeft = CurrentDragChildViewLayoutedLeft;
                CurrentDragChildViewTop = CurrentDragChildViewLayoutedTop;
            }
            ViewCaptured?.Invoke(this,new ViewCapturedEventArgs(capturedChild,activePointerId));
        }

        /// <summary>
        /// Decide next cordination y position in drag event.
        /// The default implementation provide <see cref="LayoutParams"/>'s draggable direction motion.
        /// </summary>
        /// <param name="child">The captured view</param>
        /// <param name="top">New y coordinate of the top edge of the view</param>
        /// <param name="dy">Change in y position from the last call</param>
        /// <returns></returns>
        public virtual int ClampViewPositionVertical(View child,int top,int dy) {
            var parameter = child.LayoutParameters as LayoutParams;
            if(top < CurrentDragChildViewLayoutedTop || (top == CurrentDragChildViewLayoutedTop && dy < 0)) {
                // 上方向
                if(parameter.IsDraggableTopDirection == false) {
                    return CurrentDragChildViewLayoutedTop;
                }
                if(top > CurrentDragChildViewLayoutedTop && parameter.IsDraggableBottomDirection == false) {
                    return CurrentDragChildViewLayoutedTop;
                }
                return top;
            }
            if(top > CurrentDragChildViewLayoutedTop || (top == CurrentDragChildViewLayoutedTop && dy > 0)) {
                // 下方向
                if(parameter.IsDraggableBottomDirection == false) {
                    return CurrentDragChildViewLayoutedTop;
                }
                if(top < CurrentDragChildViewLayoutedTop && parameter.IsDraggableTopDirection == false) {
                    return CurrentDragChildViewLayoutedTop;
                }
                return top;
            }
            return CurrentDragChildViewLayoutedTop;
        }

        /// <summary>
        /// Decide next cordination x position in drag event.
        /// The default implementation provide <see cref="LayoutParams"/>'s draggable direction motion.
        /// </summary>
        /// <param name="child">The captured view</param>
        /// <param name="left">New x coordinate of the left edge of the view</param>
        /// <param name="dx">Change in x position from the last call</param>
        /// <returns>The new cordination y position</returns>
        public virtual int ClampViewPositionHorizontal(View child,int left,int dx) {
            var parameter = child.LayoutParameters as LayoutParams;
            if(left < CurrentDragChildViewLayoutedLeft || (left == CurrentDragChildViewLayoutedLeft && dx < 0)) {
                // 左方向
                if(parameter.IsDraggableLeftDirection == false) {
                    return CurrentDragChildViewLayoutedLeft;
                }
                if(left > CurrentDragChildViewLayoutedLeft && parameter.IsDraggableRightDirection == false) {
                    return CurrentDragChildViewLayoutedLeft;
                }
                return left;
            }
            if(left > CurrentDragChildViewLayoutedLeft || (left == CurrentDragChildViewLayoutedLeft && dx > 0)) {
                // 右方向
                if(parameter.IsDraggableRightDirection == false) {
                    return CurrentDragChildViewLayoutedLeft;
                }
                if(left < CurrentDragChildViewLayoutedLeft && parameter.IsDraggableLeftDirection == false) {
                    return CurrentDragChildViewLayoutedLeft;
                }
                return left;
            }
            return CurrentDragChildViewLayoutedLeft;
        }

        /// <summary>
        /// Decide vertical draggable range.
        /// If overriding method, shoul use result of base method.
        /// </summary>
        /// <param name="child">The captured view</param>
        /// <returns>The draggable range</returns>
        public virtual int GetViewVerticalDragRange(View child) {
            var parameter = child.LayoutParameters as LayoutParams;
            if(parameter.IsDraggableTopDirection == false && parameter.IsDraggableBottomDirection == false) {
                return 0;
            }
            return Height;
        }

        /// <summary>
        /// Decide horizontal draggable range.
        /// If overriding method, shoul use result of base method.
        /// </summary>
        /// <param name="child">The captured view</param>
        /// <returns>The draggable range</returns>
        public virtual int GetViewHorizontalDragRange(View child) {
            var parameter = child.LayoutParameters as LayoutParams;
            if(parameter.IsDraggableLeftDirection == false && parameter.IsDraggableRightDirection == false) {
                return 0;
            }
            return Width;
        }

        /// <summary>
        /// Called when the captured view's position changes in drag event.
        /// If override method, should call base method.
        /// </summary>
        /// <param name="changedView">The position changed view</param>
        /// <param name="left">New x coordinate of the left edge of the view</param>
        /// <param name="top">New y coordinate of the top edge of the view</param>
        /// <param name="dx">Change in x position from the last call</param>
        /// <param name="dy">Change in y position from the last call</param>
        public virtual void OnViewPositionChanged(View changedView,int left,int top,int dx,int dy) {
            CurrentDragChildViewDraggedLeft = left;
            CurrentDragChildViewDraggedTop = top;
            ViewPositionChanged?.Invoke(this,new ViewPositionChangedEventArgs(changedView,left,top,dx,dy));
        }

        /// <summary>
        /// Called when captured view is no longer being actively dragged.
        /// If overriding method, you think use <see cref="ViewReleased"/> event.
        /// </summary>
        /// <param name="releasedChild">The released view</param>
        /// <param name="xvel">X velocity of the pointer in pixels per second.</param>
        /// <param name="yvel">Y velocity of the pointer in pixels per second.</param>
        public virtual void OnViewReleased(View releasedChild,float xvel,float yvel) {
            var viewReleasedEventArgs = new ViewReleasedEventArgs(releasedChild,xvel,yvel);
            ViewReleased?.Invoke(this,viewReleasedEventArgs);
            if(viewReleasedEventArgs.Handled) {
                return;
            }
            SmoothSlideViewTo(releasedChild,CurrentDragChildViewLayoutedLeft,CurrentDragChildViewLayoutedTop);
        }

        /// <summary>
        /// Start smooth slid view animation and set view position.
        /// </summary>
        /// <param name="view">The slide view</param>
        /// <param name="finalLeft">The target view position left</param>
        /// <param name="finalTop">The target view position top</param>
        public void SmoothSlideViewTo(View view,int finalLeft,int finalTop) {
            viewDragHelper.SmoothSlideViewTo(view,finalLeft,finalTop);
            Invalidate();
            childViewPosition[view] = new Point(finalLeft,finalTop);
        }

        /// <summary>
        /// Called when the drag state changes. See the <see cref="StateIdle"/>, <see cref="StateDragging"/> and <see cref="StateSetting"/> for more information.
        /// If override method, should call base method.
        /// </summary>
        /// <param name="state">The new drag state</param>
        public virtual void OnViewDragStateChanged(int state) {
            ViewDragStateChanged?.Invoke(this,new ViewDragStateChangedEventArgs(state));
        }

        protected override bool CheckLayoutParams(ViewGroup.LayoutParams p) {
            return p is LayoutParams;
        }

        protected override ViewGroup.LayoutParams GenerateDefaultLayoutParams() {
            return new LayoutParams();
        }

        public override ViewGroup.LayoutParams GenerateLayoutParams(IAttributeSet attrs) {
            return new LayoutParams(Context,attrs);
        }

        protected override ViewGroup.LayoutParams GenerateLayoutParams(ViewGroup.LayoutParams p) {
            if(p is LayoutParams) {
                return new LayoutParams(p as LayoutParams);
            }
            if(p is FrameLayout.LayoutParams) {
                return new LayoutParams(p as FrameLayout.LayoutParams);
            }
            if(p is MarginLayoutParams) {
                return new LayoutParams(p as MarginLayoutParams);
            }
            return new LayoutParams(p);
        }

        /// <summary>
        /// Base of FrameLayout.LayoutParams, can set draggable direction parameter. 
        /// </summary>
        public new class LayoutParams : FrameLayout.LayoutParams {

            /// <summary>
            /// If set true, draggable top direction
            /// </summary>
            public bool IsDraggableTopDirection { get; set; }

            /// <summary>
            /// If set true, draggable bottom direction
            /// </summary>
            public bool IsDraggableBottomDirection { get; set; }

            /// <summary>
            /// If set true, draggable left direction
            /// </summary>
            public bool IsDraggableLeftDirection { get; set; }

            /// <summary>
            /// If set true, draggable right direction
            /// </summary>
            public bool IsDraggableRightDirection { get; set; }

            public LayoutParams() : base(MatchParent,MatchParent) { }

            public LayoutParams(Context context,IAttributeSet attr) : base(context,attr) {
                init(context,attr);
            }

            public LayoutParams(ViewGroup.LayoutParams source) : base(source) { }

            public LayoutParams(MarginLayoutParams souce) : base(souce) { }

            public LayoutParams(FrameLayout.LayoutParams source) : base(source) { }

            public LayoutParams(LayoutParams source) : base(source) {
                IsDraggableTopDirection = source.IsDraggableTopDirection;
                IsDraggableBottomDirection = source.IsDraggableBottomDirection;
                IsDraggableLeftDirection = source.IsDraggableLeftDirection;
                IsDraggableRightDirection = source.IsDraggableRightDirection;
            }

            public LayoutParams(IntPtr javaReference,JniHandleOwnership transfer) : base(javaReference,transfer) { }

            private void init(Context context,IAttributeSet attr) {
                var ar = context.ObtainStyledAttributes(attr,Resource.Styleable.SlideLayout);
                try {
                    IsDraggableTopDirection = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_top_direction,false);
                    IsDraggableBottomDirection = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_bottom_direction,false);
                    IsDraggableLeftDirection = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_left_direction,false);
                    IsDraggableRightDirection = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_right_direction,false);

                    bool isDraggableVerticalDirection = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_vertical_direction,false);
                    bool isDraggableHorizontalDirecion = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_horizontal_direction,false);
                    bool isDraggableAllDirection = ar.GetBoolean(Resource.Styleable.SlideLayout_draggable_all_direction,false);

                    IsDraggableTopDirection = IsDraggableTopDirection || isDraggableVerticalDirection || isDraggableAllDirection;
                    IsDraggableBottomDirection = IsDraggableBottomDirection || isDraggableVerticalDirection || isDraggableAllDirection;
                    IsDraggableLeftDirection = IsDraggableLeftDirection || isDraggableHorizontalDirecion || isDraggableAllDirection;
                    IsDraggableRightDirection = IsDraggableRightDirection || isDraggableHorizontalDirecion || isDraggableAllDirection;
                } finally {
                    ar.Recycle();
                }
            }
        }

    }
}