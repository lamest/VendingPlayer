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
        private IUIHider _uiHider;
        private FilePickerFragment _filePicker;
        private VideoView _videoView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _uiHider = UIHiderFabric.GetHider();
            _uiHider.Subscribe(Window);

            //final PowerManager pm = (PowerManager)getSystemService(Context.POWER_SERVICE);
            //this.mWakeLock = pm.newWakeLock(PowerManager.SCREEN_DIM_WAKE_LOCK, "My Tag");
            //this.mWakeLock.acquire();
            SetContentView(Resource.Layout.Main);


            _filePicker = new FilePickerFragment();
            _filePicker.FileSelected += (sender1, path) =>
            {
                try
                {
                    SetFile(_videoView, path);
                    SaveFilePath(path);
                    _videoView.Start();
                }
                catch (Exception ex)
                {

                }
            };
            _filePicker.Cancel += sender2 => _filePicker.Dismiss();

            _videoView = FindViewById<VideoView>(Resource.Id.MainVideoView);

            try
            {
                if (SetFile(_videoView))
                {
                    _videoView.Start();
                }
            }
            catch (Exception ex)
            {
            }

            _videoView.Completion += OnCompletion;
            _videoView.Error += OnError;
            _videoView.Touch += OnTouch;
            _videoView.KeepScreenOn = true;
        }

        private void OnTouch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
            {
                ShowFilePickerDialog();
            }
        }

        private bool SetFile(VideoView videoView, string path = null)
        {
            var filePath = path ?? GetDefaultFilePath();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
            var uri = Uri.Parse(filePath);
            videoView.SetVideoURI(uri);
            return true;
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
            var t = Toast.MakeText(Application.Context, $"Error: {e.What.ToString()}", ToastLength.Long);
            t.Show();
            ShowFilePickerDialog();
        }

        private void ShowFilePickerDialog()
        {
            if (_filePicker.IsVisible)
            {
                _filePicker.Dismiss();
            }
            _filePicker.Show(base.FragmentManager, "FilePicker");
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