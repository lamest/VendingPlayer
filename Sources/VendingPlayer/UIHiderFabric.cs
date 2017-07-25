using Android.OS;

namespace VendingPlayer
{
    internal static class UIHiderFabric
    {
        public static IUIHider GetHider()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                return new KitKatUIHider();
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                return new IceCreamSandwichUIHider();
            }
            return null;
        }
    }
}