using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageSorter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private StorageFolder folder { get; set; }
        ObservableCollection<mImage> Images;
        public MainPage()
        {
            Images = new ObservableCollection<mImage>();
            this.InitializeComponent();
        }

        private async void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker pk = new FolderPicker();
            pk.FileTypeFilter.Add("*");
            var vfolder = await pk.PickSingleFolderAsync();
            if (vfolder != null)
            {
                folder = vfolder;
            }

            if (folder != null)
            {
                IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
                Images.Clear();
                int n = 0;
                int cnt = files.Count();
                foreach (StorageFile fp in files)
                {
                    Debug.WriteLine(fp.Path);
                    mImage img = new mImage();
                    if(fp.Path.EndsWith(".jpg")|| fp.Path.EndsWith(".jpeg") || fp.Path.EndsWith(".JPG") || fp.Path.EndsWith(".JPEG")) {
                        img.path = fp;
                        img.thmb = new BitmapImage();
                        ImageProperties imageProperties = await fp.Properties.GetImagePropertiesAsync();
                        img.timeTaken = imageProperties.DateTaken;
                        img.thmb.SetSource(await fp.GetThumbnailAsync(ThumbnailMode.PicturesView));
                        img.get_blurriness();
                        Images.Add(img);
                    }
                    n++;
                    PGBar.Value = 100*n / cnt;
                }
                PGBar.Value = 0;
            }   
        }

        private void BlurButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(mImage img in Images)
            {
                img.sortstate = 0;
            }
            Images.Sort();
        }

        private void ExposureBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (mImage img in Images)
            {
                img.sortstate = 1;
            }
            Images.Sort();
        }

        private void TimeBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (mImage img in Images)
            {
                img.sortstate = 2;
            }
            Images.Sort();
        }
    }

    static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sorted = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }

    class mImage : IComparable
    {
        public StorageFile path { get; set; } 
        public BitmapImage thmb { get; set; }
        public float blur { get; set; }
        public int exposure { get; set; }
        public int sortstate { get; set; }
        public DateTimeOffset timeTaken { get; set; }
        public int CompareTo(object obj)
        {
            var ob= (mImage)obj;
            switch (sortstate)
            {
                case 0:
                    return blur.CompareTo(ob.blur);
                case 1:
                    return exposure.CompareTo(ob.exposure);
                case 2:
                    return timeTaken.CompareTo(ob.timeTaken);
            }
            throw new Exception();
        }

        public async void get_blurriness()
        {
            byte[] picdata;
            var fp = await path.OpenAsync(FileAccessMode.Read);
            BitmapDecoder bd = await BitmapDecoder.CreateAsync(fp);
            BitmapPixelFormat fmt = bd.BitmapPixelFormat;
            uint width = bd.PixelWidth;
            uint height = bd.PixelHeight;
            PixelDataProvider pd = await bd.GetPixelDataAsync();
            picdata = pd.DetachPixelData();
            exposure = 0;
            float mean = 0, M2 = 0;
            int n = 0;
            switch (fmt)
            {
                case BitmapPixelFormat.Bgra8:
                case BitmapPixelFormat.Rgba8:
                    for (uint x = 1; x < width - 1; x++)
                    {
                        for (uint y = 1; y < height - 1; y++)
                        {
                            n++;
                            byte c1 = picdata[(x + width * y) * 4];
                            byte c2 = picdata[(x + width * y) * 4 + 1];
                            byte c3 = picdata[(x + width * y) * 4 + 2];
                            int brightness = c1 + c2 + c3;
                            exposure += 255 - brightness / 3;
                            byte ct1 = picdata[(x + width * (y - 1)) * 4];
                            byte ct2 = picdata[(x + width * (y - 1)) * 4 + 1];
                            byte ct3 = picdata[(x + width * (y - 1)) * 4 + 2];
                            int top = ct1 + ct2 + ct3;
                            byte cb1 = picdata[(x + width * (y + 1)) * 4];
                            byte cb2 = picdata[(x + width * (y + 1)) * 4 + 1];
                            byte cb3 = picdata[(x + width * (y + 1)) * 4 + 2];
                            int bottom = cb1 + cb2 + cb3;
                            byte cl1 = picdata[(x + width * y - 1) * 4];
                            byte cl2 = picdata[(x + width * y - 1) * 4 + 1];
                            byte cl3 = picdata[(x + width * y - 1) * 4 + 2];
                            int left = cl1 + cl2 + cl3;
                            byte cr1 = picdata[(x + width * y - 1) * 4];
                            byte cr2 = picdata[(x + width * y - 1) * 4 + 1];
                            byte cr3 = picdata[(x + width * y - 1) * 4 + 2];
                            int right = cr1 + cr2 + cr3;
                            int laplacian = left + top + right + bottom - brightness * 4;
                            float delta = laplacian - mean;
                            mean += delta / n;
                            float delta2 = laplacian - mean;
                            M2 += delta2 * delta;
                        }
                    }
                    blur = M2 / (n - 1);
                    exposure /= n;
                    break;
                case BitmapPixelFormat.Rgba16:
                    break;
                default:
                    break;
            }
        }
    }
}
