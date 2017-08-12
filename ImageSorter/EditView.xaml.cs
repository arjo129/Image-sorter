using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ImageSorter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditView : Page
    {
        private ImageItem item;
        private byte[] bits;
        public EditView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            item = (ImageItem)e.Parameter;
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                   AppViewBackButtonVisibility.Collapsed;

            }
        }
        private async void ImageLoaded(object sender, RoutedEventArgs r)
        {
            using (var stream = await item.path.OpenAsync(FileAccessMode.Read))
            {
                var bitmapDecoder = await BitmapDecoder.CreateAsync(stream);
                var pixelProvider = await bitmapDecoder.GetPixelDataAsync();
                bits = pixelProvider.DetachPixelData();
                var softwareBitmap = new SoftwareBitmap(
                  BitmapPixelFormat.Bgra8,
                  (int)bitmapDecoder.PixelWidth,
                  (int)bitmapDecoder.PixelHeight,
                  BitmapAlphaMode.Premultiplied);
                softwareBitmap.CopyFromBuffer(bits.AsBuffer());
                var softwareBitmapSource = new SoftwareBitmapSource();
                await softwareBitmapSource.SetBitmapAsync(softwareBitmap);
                img.Source = softwareBitmapSource;
            }
        }
    }
}
