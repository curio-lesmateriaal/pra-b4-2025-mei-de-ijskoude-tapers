using PRA_B4_FOTOKIOSK.models;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PRA_B4_FOTOKIOSK.magie
{
    public static class SearchManager
    {
        public static Home Instance { get; set; }

        public static void SetPicture(string path)
        {
            if (Instance == null)
            {
                System.Windows.MessageBox.Show("SearchManager.Instance is niet ingesteld.");
                return;
            }

            if (Instance.imgBig == null)
            {
                System.Windows.MessageBox.Show("imgBig is niet ingesteld in Home.");
                return;
            }

            if (!File.Exists(path))
            {
                System.Windows.MessageBox.Show("Afbeeldingsbestand bestaat niet: " + path);
                return;
            }

            Instance.imgBig.Source = PathToImage(path);
        }

        public static BitmapImage PathToImage(string path)
        {
            using var stream = new MemoryStream(File.ReadAllBytes(path));
            var img = new BitmapImage();

            img.BeginInit();
            img.StreamSource = stream;
            img.CacheOption = BitmapCacheOption.OnLoad; // belangrijk om file te sluiten na load
            img.EndInit();
            img.Freeze();

            return img;
        }

        public static string GetSearchInput()
        {
            if (Instance == null)
            {
                System.Windows.MessageBox.Show("SearchManager.Instance is niet ingesteld.");
                return string.Empty;
            }

            if (Instance.tbZoeken == null)
            {
                System.Windows.MessageBox.Show("tbZoeken is niet ingesteld in Home.");
                return string.Empty;
            }

            return Instance.tbZoeken.Text;
        }

        public static void SetSearchImageInfo(string text)
        {
            if (Instance == null || Instance.lbSearchInfo == null)
            {
                return;
            }

            Instance.lbSearchInfo.Content = text;
        }
    }
}
