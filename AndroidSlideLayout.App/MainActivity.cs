using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace AndroidSlideLayout.App {

    [Activity(Label = "AndroidSlideLayout.App", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity {

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            using (var imageView = FindViewById<ImageView>(Resource.Id.Transition)) {
                imageView.Click += click;
            }
        }

        protected override void OnDestroy() {
            using (var imageView = FindViewById<ImageView>(Resource.Id.Transition)) {
                imageView.Click -= click;
            }
            base.OnDestroy();

        }

        private void click(object sender, EventArgs args) {
            var imageView = (ImageView)sender;
            TransitionActivity.Start(this, imageView);
        }

    }
}

