using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NullSoftware.Windows.Extensions
{
    /// <summary>
    /// Defines a extension methods and attached properties
    /// for <see cref="Window"/>.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Identifies the PlacementStorageStrategy attached property.
        /// </summary>
        /// <remarks>
        /// Provides service to handle window placement loading/saving.
        /// </remarks>
        public static readonly DependencyProperty PlacementStorageStrategyProperty
            = DependencyProperty.RegisterAttached(
                "PlacementStorageStrategy",
                typeof(IWindowPlacementStorage),
                typeof(WindowExtensions),
                new UIPropertyMetadata(OnPlacementStorageStrategyChanged));

        /// <summary>
        /// Sets the value of the PlacementStorageStrategy
        /// attached property to a given <see cref="Window"/>.
        /// </summary>
        /// <param name="element">
        /// The element on which to set the CloseCommand attached property.
        /// </param>
        /// <param name="value">
        /// The property value to set.
        /// </param>
        public static void SetPlacementStorageStrategy(DependencyObject element, IWindowPlacementStorage value)
        {
            element.SetValue(PlacementStorageStrategyProperty, value);
        }

        /// <summary>
        /// Gets the value of the PlacementStorageStrategy
        /// attached property from a given <see cref="Window"/>.
        /// </summary>
        /// <param name="element">
        /// The element from which to read the property value.
        /// </param>
        /// <returns>
        /// The value of the PlacementStorageStrategy attached property.
        /// </returns>
        public static IWindowPlacementStorage GetPlacementStorageStrategy(DependencyObject element)
        {
            return (IWindowPlacementStorage)element.GetValue(PlacementStorageStrategyProperty);
        }

        private static void OnPlacementStorageStrategyChanged(DependencyObject sender,
           DependencyPropertyChangedEventArgs e)
        {
            if (DesignModeVerifier.GetIsInDesignMode())
                return;

            Window win = (Window)sender;

            win.SourceInitialized -= OnWindowPlacementSourceInitialized;
            win.Closing -= OnWindowPlacementClosing;

            if (e.NewValue != null)
            {
                win.SourceInitialized += OnWindowPlacementSourceInitialized;
                win.Closing += OnWindowPlacementClosing;
            }
        }

        private static void OnWindowPlacementSourceInitialized(object? sender, EventArgs e)
        {
            if (DesignModeVerifier.GetIsInDesignMode())
                return;

            Window win = (Window)sender!;
            IWindowPlacementStorage placementStorage = GetPlacementStorageStrategy(win);
            byte[]? rawData = placementStorage.LoadPlacement(win);

            if (rawData != null)
                WindowPlacementManager.SetPlacement(win, WindowPlacementManager.Deserialize(rawData));
        }

        private static void OnWindowPlacementClosing(object? sender, CancelEventArgs e)
        {
            if (DesignModeVerifier.GetIsInDesignMode())
                return;

            Window win = (Window)sender!;
            IWindowPlacementStorage placementStorage = GetPlacementStorageStrategy(win);
            byte[] rawData = WindowPlacementManager.Serialize(WindowPlacementManager.GetPlacement(win));

            placementStorage.SavePlacement(win, rawData);
        }
    }
}
