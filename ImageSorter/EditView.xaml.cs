using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
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
        private BitmapImage bmp;
        public EditView()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            item = (ImageItem)e.Parameter;
        }
        private async void ImageLoaded(object sender, RoutedEventArgs r)
        {
            var fp = await item.path.OpenAsync(Windows.Storage.FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(fp);
            var pd = await decoder.GetPixelDataAsync();
            var dat = pd.DetachPixelData();
            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
            writer.WriteBytes(dat);
            await writer.StoreAsync();
            await bmp.SetSourceAsync(stream);
            img.Source = bmp;
        }
    }
}
