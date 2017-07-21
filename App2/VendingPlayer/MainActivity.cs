using System;
using Android.App;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using Compass.FilePicker;
using Uri = Android.Net.Uri;

namespace VendingPlayer
{
    [Activity(Label = "VendingPlayer", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private const string _filePathKey = "FilePath";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //final PowerManager pm = (PowerManager)getSystemService(Context.POWER_SERVICE);
            //this.mWakeLock = pm.newWakeLock(PowerManager.SCREEN_DIM_WAKE_LOCK, "My Tag");
            //this.mWakeLock.acquire();
            SetContentView(Resource.Layout.Main);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn |
                            WindowManagerFlags.DismissKeyguard |
                            WindowManagerFlags.ShowWhenLocked |
                            WindowManagerFlags.TurnScreenOn |
                            WindowManagerFlags.Fullscreen);


            var videoView = FindViewById<VideoView>(Resource.Id.MainVideoView);

            SetFile(videoView);
            videoView.Start();
            videoView.Completion += OnCompletion;
            videoView.Error += OnError;
            videoView.Touch += OnTouch;
            videoView.KeepScreenOn = true;
        }

        private void OnTouch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                var videoView = sender as VideoView;

                ShowFilePickerDialog(videoView);
            }
        }

        private void SetFile(VideoView videoView, string path = null)
        {
            string filePath;
            if (path == null)
            {
                filePath = GetDefaultFilePath();
            }
            else
            {
                filePath = path;
            }
            var uri = Uri.Parse(filePath);
            videoView.SetVideoURI(uri);
        }

        private string GetDefaultFilePath()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var filePath = string.Empty;
            if (prefs.All.ContainsKey(_filePathKey))
            {
                filePath = (string) prefs.All[_filePathKey];
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
                SaveFilePath(path);
                videoView.Start();
            };
            filePickerFragment.Cancel += sender2 => filePickerFragment.Dismiss();
            filePickerFragment.Show(FragmentManager, "FilePicker");
        }

        private void SaveFilePath(string path)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var editor = prefs.Edit();
            editor.PutString(_filePathKey, path);
            editor.Apply();
        }

        private void OnCompletion(object sender, EventArgs e)
        {
            var videoView = sender as MediaPlayer;
            videoView?.Start();
        }
    }
}