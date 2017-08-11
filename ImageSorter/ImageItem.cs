using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageSorter
{
    class ImageItem: IComparable
    {
        public StorageFile path { get; set; }
        public BitmapImage thmb { get; set; }
        public BitmapImage fullimg;
        public float blur { get; set; }
        public int exposure { get; set; }
        public int sortstate { get; set; }
        public DateTimeOffset timeTaken { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsVideo { get; set; }
        public string DurationStr { get {
                if (IsVideo)
                {
                    return Duration.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    return "";
                }
            } }

      
        public int CompareTo(object obj)
        {
            var ob = (ImageItem)obj;
            switch (sortstate)
            {
                case 0:
                    return blur.CompareTo(ob.blur);
                case 1:
                    return exposure.CompareTo(ob.exposure);
                case 2:
                    return timeTaken.CompareTo(ob.timeTaken);
                case 3:
                    return Duration.CompareTo(ob.Duration);
            }
            throw new Exception();
        }

        public async void get_blurriness()
        {
            byte[] picdata;
            using (var fp = await path.OpenAsync(FileAccessMode.Read))
            {
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
}
