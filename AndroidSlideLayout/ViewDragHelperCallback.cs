using System;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;

namespace AndroidSlideLayout {

    /// <summary>
    /// The rapped <see cref="ViewDragHelper.Callback"/> class
    /// </summary>
    public class ViewDragHelperCallback : ViewDragHelper.Callback {

        private IDragCallback dragCallback;

        public ViewDragHelperCallback(IDragCallback dragCallback) : base() {
            this.dragCallback = dragCallback;
        }

        public ViewDragHelperCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        public override bool TryCaptureView(View child, int pointerId) {
            return dragCallback.TryCaptureView(child, pointerId);
        }

        public override void OnViewCaptured(View capturedChild, int activePointerId) {
            dragCallback.OnViewCaptured(capturedChild, activePointerId);
        }

        public override int ClampViewPositionVertical(View child, int top, int dy) {
            return dragCallback.ClampViewPositionVertical(child, top, dy);
        }

        public override int ClampViewPositionHorizontal(View child, int left, int dx) {
            return dragCallback.ClampViewPositionHorizontal(child, left, dx);
        }

        public override int GetViewVerticalDragRange(View child) {
            return dragCallback.GetViewVerticalDragRange(child);
        }

        public override int GetViewHorizontalDragRange(View child) {
            return dragCallback.GetViewHorizontalDragRange(child);
        }

        public override void OnViewPositionChanged(View changedView, int left, int top, int dx, int dy) {
            dragCallback.OnViewPositionChanged(changedView, left, top, dx, dy);
        }

        public override void OnViewReleased(View releasedChild, float xvel, float yvel) {
            dragCallback.OnViewReleased(releasedChild, xvel, yvel);
        }

        public override void OnViewDragStateChanged(int state) {
            dragCallback.OnViewDragStateChanged(state);
        }
    }
}