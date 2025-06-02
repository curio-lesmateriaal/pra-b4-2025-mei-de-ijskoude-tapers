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
            var gebruikt = new HashSet<KioskPhoto>();

            for (int i = 0; i < sorted.Count; i++)
            {
                var foto1 = sorted[i];

                if (gebruikt.Contains(foto1))
                    continue;

                // Zoek een foto die precies 60 seconden later is
                KioskPhoto foto2 = sorted.FirstOrDefault(f =>
                    !gebruikt.Contains(f) &&
                    Math.Abs((f.Tijd - foto1.Tijd).TotalSeconds - 60) < 0.5); // kleine speling

                // Maak rij
                StackPanel rij = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(10)
                };

                rij.Children.Add(CreateImage(foto1));
                gebruikt.Add(foto1);

                if (foto2 != null)
                {
                    rij.Children.Add(CreateImage(foto2));
                    gebruikt.Add(foto2);
                }

                Instance.spPictures.Children.Add(rij);
            }
        }


        private static Image CreateImage(KioskPhoto photo)
        {
            Image image = new Image
            {
                Source = PathToImage(photo.Source),
                Width = 1920 / 4,
                Height = 1080 / 4,
                Margin = new Thickness(5)
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
