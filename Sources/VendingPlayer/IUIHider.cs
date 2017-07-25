using Android.Views;

namespace VendingPlayer
{
    internal interface IUIHider
    {
        void Hide(Window window);
        void Subscribe(Window window);
        void Unsubscribe(Window window);
    }
}