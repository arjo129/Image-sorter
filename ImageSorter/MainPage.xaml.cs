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
        FilterableCollection<ImageItem> Images;
        public MainPage()
        {
            Images = new FilterableCollection<ImageItem>();
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
                    if (!(bool)ModeBtn.IsChecked)
                    {
                        Debug.WriteLine(fp.Path);
                        ImageItem img = new ImageItem();
                        if (fp.Path.EndsWith(".jpg") || fp.Path.EndsWith(".jpeg") || fp.Path.EndsWith(".JPG") || fp.Path.EndsWith(".JPEG"))
                        {
                            img.path = fp;
                            img.thmb = new BitmapImage();
                            ImageProperties imageProperties = await fp.Properties.GetImagePropertiesAsync();
                            img.timeTaken = imageProperties.DateTaken;
                            img.thmb.SetSource(await fp.GetThumbnailAsync(ThumbnailMode.PicturesView));
                            img.get_blurriness();
                            Images.Add(img);
                        }
                    } else
                    {
                        ImageItem img = new ImageItem();
                        if (fp.Path.EndsWith(".mp4") || fp.Path.EndsWith(".m4v") || fp.Path.EndsWith(".avi") || fp.Path.EndsWith(".MP4") || fp.Path.EndsWith(".M4V") || fp.Path.EndsWith(".AVI"))
                        {
                            img.path = fp;
                            img.thmb = new BitmapImage();
                            VideoProperties videoProperties = await fp.Properties.GetVideoPropertiesAsync();
                            img.Duration = videoProperties.Duration;
                            img.thmb.SetSource(await fp.GetThumbnailAsync(ThumbnailMode.VideosView));
                            //img.get_blurriness();
                            Images.Add(img);
                        }
                    }
                    n++;
                    PGBar.Value = 100*n / cnt;
                }
                PGBar.Value = 0;
            }   
        }

        private void BlurButton_Click(object sender, RoutedEventArgs e)
        {
            foreach(ImageItem img in Images)
            {
                img.sortstate = 0;
            }
            Images.Sort();
        }

        private void ExposureBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (ImageItem img in Images)
            {
                img.sortstate = 1;
            }
            Images.Sort();
        }

        private void TimeBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (ImageItem img in Images)
            {
                img.sortstate = 2;
            }
            Images.Sort();
        }
    }

}
