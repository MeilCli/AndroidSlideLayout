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

namespace AndroidSlideLayout {

    /// <summary>
    /// Event of a child view is captured for dragging or settling.
    /// </summary>
    public class ViewCapturedEventArgs : EventArgs {

        /// <summary>
        /// Child view that was captured
        /// </summary>
        public View CapturedChild { get; }

        /// <summary>
        /// Pointer id tracking the child capture
        /// </summary>
        public int ActivePointerId { get; }

        public ViewCapturedEventArgs(View capturedChild,int activePointerId) {
            CapturedChild = capturedChild;
            ActivePointerId = activePointerId;
        }
    }

    /// <summary>
    /// Event of the captured view position changed.
    /// </summary>
    public class ViewPositionChangedEventArgs : EventArgs {

        /// <summary>
        /// The position changed view
        /// </summary>
        public View ChangedView { get; }

        /// <summary>
        /// New x coordinate of the left edge of the view
        /// </summary>
        public int Left { get; }

        /// <summary>
        /// New y coordinate of the top edge of the view
        /// </summary>
        public int Top { get; }

        /// <summary>
        /// Change in x position from the last call
        /// </summary>
        public int Dx { get; }

        /// <summary>
        /// Change in y position from the last call
        /// </summary>
        public int Dy { get; }

        public ViewPositionChangedEventArgs(View changedView,int left,int top,int dx,int dy) {
            ChangedView = changedView;
            Left = left;
            Top = top;
            Dx = dx;
            Dy = dy;
        }
    }

    /// <summary>
    /// Event of the captured view is no longer being actively dragged.
    /// </summary>
    public class ViewReleasedEventArgs : EventArgs {

        /// <summary>
        /// The released view
        /// </summary>
        public View ReleasedChild { get; }

        /// <summary>
        /// X velocity of the pointer in pixels per second.
        /// </summary>
        public float XVelocity { get; }

        /// <summary>
        /// Y velocity of the pointer in pixels per second.
        /// </summary>
        public float YVelocity { get; }

        /// <summary>
        /// This event should  decide final view position.
        /// In default, the captured child view return layouted position.
        /// If you decided view posiotion, return true.
        /// </summary>
        public bool Handled { get; set; } = false;

        public ViewReleasedEventArgs(View releasedChild,float xvel,float yvel) {
            ReleasedChild = releasedChild;
            XVelocity = xvel;
            YVelocity = yvel;
        }
    }

    /// <summary>
    /// Event of the drag state changed.
    /// </summary>
    public class ViewDragStateChangedEventArgs : EventArgs {

        /// <summary>
        /// The drag state. See the <see cref="StateIdle"/>, <see cref="StateDragging"/> and <see cref="StateSetting"/> for more information.
        /// </summary>
        public int State { get; }

        public ViewDragStateChangedEventArgs(int state) {
            State = state;
        }
    }
}