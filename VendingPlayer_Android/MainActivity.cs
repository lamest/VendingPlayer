using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Preferences;
using Compass.FilePicker;

namespace VendingPlayer_Android
{
    [Activity(Label = "VendingPlayer_Android", MainLauncher = true, Icon = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private const string _filePathKey = "FilePath";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            
            var videoView = FindViewById<VideoView>(Resource.Id.VendingVideoView);

            SetFile(videoView);
            videoView.Start();
            videoView.Completion += OnCompletion;
            videoView.Error += OnError;
            videoView.Touch += OnTouch;
        }

        private void OnTouch(object sender, View.TouchEventArgs e)
        {
            var videoView = sender as VideoView;

            ShowFilePickerDialog(videoView);
        }

        private void SetFile(VideoView videoView, string path=null)
        {
            string filePath = null;
            if (path == null)
            {
                filePath = GetDefaultFilePath();
            }
            var uri = Android.Net.Uri.Parse(filePath);
            videoView.SetVideoURI(uri);
        }

        private string GetDefaultFilePath()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(this.ApplicationContext);
            var filePath = string.Empty;
            if (prefs.All.ContainsKey(_filePathKey))
            {
                filePath = (string)prefs.All[_filePathKey];
            }

            return filePath;
        }

        private void OnError(object sender, MediaPlayer.ErrorEventArgs e)
        {
            var videoView = sender as VideoView;

            ShowFilePickerDialog(videoView);
        }

        private void ShowFilePickerDialog(VideoView videoView)
        {
            var filePickerFragment = new FilePickerFragment();
            filePickerFragment.FileSelected += (sender1, path) =>
            {
                filePickerFragment.Dismiss();
                SetFile(videoView, path);
            };
            filePickerFragment.Cancel += sender2 => filePickerFragment.Dismiss();
            filePickerFragment.Show(FragmentManager, "FilePicker");
        }

        private void OnCompletion(object sender, EventArgs e)
        {
            var videoView = sender as VideoView;
            videoView?.Start();
        }
    }
}

