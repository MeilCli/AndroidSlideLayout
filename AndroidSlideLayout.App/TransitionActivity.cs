using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Widget;

namespace AndroidSlideLayout.App {

    // ----- Topic (Use Activity Transition on Xamarin) -----
    // 1. set android:windowContentTransitions = true to style(for lolipop) or Activity.Window
    // 2. set android:windowEnterTransition and android:windowExitTransition to style(for lolipop) or Activity.Window
    // 3. when start activity, if lolipop, set parameter value of ActivityOptions.MakeSceneTransitionAnimation().ToBundle() to Activity.StartActivity()
    // 4. when started activity, if use share_transition, call ViewCompat.SetTransitionName()
    // 5. when end activity, call ActivityCompat.FinishAfterTransition()
    // What's happen!!! Why cannot use ActivityOptionsCompat!!!!

    [Activity(Label = "TransitionActivity", Theme = "@style/AppTheme")]
    public class TransitionActivity : AppCompatActivity {

        private const string transitionName = "transitionactivity_transition";

        private SlideLayout slideLayout;

        public static void Start(Activity activity, ImageView imageView) {
            var intent = new Intent(activity, typeof(TransitionActivity));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                var option = ActivityOptions.MakeSceneTransitionAnimation(activity, imageView, transitionName);
                activity.StartActivity(intent, option.ToBundle());
            } else {
                activity.StartActivity(intent);
            }
        }

        /// <summary>
        /// Convert dp to px
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private int convertDensityIndependentPixelToPixel(float dp) {
            var metrics = Resources.DisplayMetrics;
            return (int)(dp * ((int)metrics.DensityDpi / 160f));
        }

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Transition);

            using (var imageView = FindViewById<ImageView>(Resource.Id.ImageView)) {
                ViewCompat.SetTransitionName(imageView, transitionName);
            }

            // must not use `using pattern` and must not call Dispose until called OnDestroy
            // SlideLayoutを使用している間にDisposeするとイベントハンドラーが解除されるようです(検証の必要あり)
            slideLayout = FindViewById<SlideLayout>(Resource.Id.SlideLayout);
            slideLayout.ViewReleased += viewReleased;
        }

        protected override void OnDestroy() {
            slideLayout.ViewReleased -= viewReleased;
            slideLayout.Dispose();
            base.OnDestroy();
        }

        private void viewReleased(object sender, ViewReleasedEventArgs args) {
            var slideLayout = sender as SlideLayout;
            int distance = Math.Abs(slideLayout.CurrentDragChildViewLayoutedTop - slideLayout.CurrentDragChildViewDraggedTop);
            int finishDistance = convertDensityIndependentPixelToPixel(150);
            if (distance > finishDistance) {
                args.Handled = true;
                ActivityCompat.FinishAfterTransition(this);
            }
        }
    }
}