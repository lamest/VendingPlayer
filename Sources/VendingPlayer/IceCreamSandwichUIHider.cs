using System;
using System.Threading.Tasks;
using Android.Views;

namespace VendingPlayer
{
    internal class IceCreamSandwichUIHider : IUIHider
    {
        private TimeSpan _visibilityChangedDelay = TimeSpan.FromSeconds(10);
        private Window _window;

        public void Hide(Window window)
        {
            var decorView = window.DecorView;
            var uiOptions = (int) decorView.SystemUiVisibility;
            var newUiOptions = (int) uiOptions;

            newUiOptions |= (int) SystemUiFlags.Fullscreen;
            newUiOptions |= (int) SystemUiFlags.HideNavigation;

            newUiOptions |= (int) SystemUiFlags.Immersive;

            decorView.SystemUiVisibility = (StatusBarVisibility) newUiOptions;
        }

        public void Subscribe(Window window)
        {
            if (_window == null)
            {
                _window = window;
                _window.AddFlags(WindowManagerFlags.KeepScreenOn |
                                 WindowManagerFlags.DismissKeyguard |
                                 WindowManagerFlags.ShowWhenLocked |
                                 WindowManagerFlags.TurnScreenOn |
                                 WindowManagerFlags.Fullscreen);
            }
            _window.DecorView.SystemUiVisibilityChange += OnVisibilityChange;
        }

        public void Unsubscribe(Window window)
        {
            _window.DecorView.SystemUiVisibilityChange -= OnVisibilityChange;
        }

        private void OnVisibilityChange(object sender, View.SystemUiVisibilityChangeEventArgs e)
        {
            if (e.Visibility == StatusBarVisibility.Visible)
            {
                if (_window != null)
                {
                    Task.Delay(_visibilityChangedDelay).ContinueWith((task) => { Hide(_window); });
                }
            }
        }
    }
}