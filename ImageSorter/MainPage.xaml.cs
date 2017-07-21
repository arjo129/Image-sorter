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
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageSorter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
                foreach (StorageFile fp in files)
                {
                    Debug.WriteLine(fp.Path);
                    mImage img = new mImage();
                    if(fp.Path.EndsWith(".jpg")|| fp.Path.EndsWith(".jpeg") || fp.Path.EndsWith(".JPG") || fp.Path.EndsWith(".JPEG")) {
                        Debug.WriteLine(fp.Path);
                        img.path = fp.Path;
                        img.thmb = new BitmapImage(); 
                        img.thmb.SetSource(await fp.GetThumbnailAsync(ThumbnailMode.PicturesView));
                        Images.Add(img);
                    }
                }
            }   
        }

        private StorageFolder folder { get; set; }
    }

    class mImage
    {
        public string path { get; set; } 
        public BitmapImage thmb { get; set; }
        public int blur { get; set; }
        public int get_blurriness()
        {
            
            return -1;
        }
    }
}
