using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Linq;

namespace PRA_B4_FOTOKIOSK.magie
{
    public class PictureManager
    {
        public static Home Instance { get; set; }

        public static void UpdatePictures(List<KioskPhoto> picturesToDisplay)
        {
            Instance.spPictures.Children.Clear();

            // Sorteer foto's op tijd
            var sorted = picturesToDisplay.OrderBy(p => p.Tijd).ToList();

            // Voeg alle foto's rechtstreeks toe aan WrapPanel (4 per rij door width)
            foreach (var foto in sorted)
            {
                Instance.spPictures.Children.Add(CreateImage(foto));
            }
        }

        private static Image CreateImage(KioskPhoto photo)
        {
            Image image = new Image
            {
                Source = PathToImage(photo.Source),
                Width = 1920 / 4 - 30, // ongeveer 4 foto's per rij, met marge
                Height = 1080 / 4 - 30,
                Margin = new Thickness(10)
            };
            return image;
        }

        public static BitmapImage PathToImage(string path)
        {
            var stream = new MemoryStream(File.ReadAllBytes(path));
            var img = new BitmapImage();

            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = stream;
            img.EndInit();
            img.Freeze();

            return img;
        }
    }
}
