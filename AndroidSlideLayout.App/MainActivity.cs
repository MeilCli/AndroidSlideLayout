using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using System;
using Android.Graphics.Drawables;
using Android.Transitions;

namespace AndroidSlideLayout.App {

    [Activity(Label = "AndroidSlideLayout.App",MainLauncher = true,Icon = "@drawable/icon",Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            using(var imageView = FindViewById<ImageView>(Resource.Id.Transition)) {
                imageView.Click += click;
            }
        }

        protected override void OnDestroy() {
            using(var imageView = FindViewById<ImageView>(Resource.Id.Transition)) {
                imageView.Click -= click;
            }
            base.OnDestroy();

        }

        private void click(object sender,EventArgs args) {
            var imageView = (ImageView)sender;
            TransitionActivity.Start(this,imageView);
        }

    }
}

