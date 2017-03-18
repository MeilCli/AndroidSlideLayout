using Android.Views;

namespace AndroidSlideLayout {

    /// <summary>
    /// The defined <see cref="ViewDragHelper.Callback"/> interface
    /// </summary>
    public interface IDragCallback {

        bool TryCaptureView(View child, int pointerId);

        void OnViewCaptured(View capturedChild, int activePointerId);

        int ClampViewPositionVertical(View child, int top, int dy);

        int ClampViewPositionHorizontal(View child, int left, int dx);

        int GetViewVerticalDragRange(View child);

        int GetViewHorizontalDragRange(View child);

        void OnViewPositionChanged(View changedView, int left, int top, int dx, int dy);

        void OnViewReleased(View releasedChild, float xvel, float yvel);

        void OnViewDragStateChanged(int state);
    }
}