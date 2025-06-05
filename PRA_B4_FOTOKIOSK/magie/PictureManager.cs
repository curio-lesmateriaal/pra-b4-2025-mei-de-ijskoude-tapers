using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PRA_B4_FOTOKIOSK.magie
{
    public class PictureManager
    {
        public static Home Instance { get; set; }

        public static void UpdatePictures(List<KioskPhoto> picturesToDisplay)
        {
            Instance.spPictures.Children.Clear();

            var sorted = picturesToDisplay.OrderBy(p => p.Tijd).ToList();
            var gebruikt = new HashSet<KioskPhoto>();

            for (int i = 0; i < sorted.Count; i++)
            {
                var foto1 = sorted[i];

                if (gebruikt.Contains(foto1))
                    continue;

                var foto2 = sorted.FirstOrDefault(f =>
                    !gebruikt.Contains(f) &&
                    Math.Abs((f.Tijd - foto1.Tijd).TotalSeconds - 60) < 0.5);

                if (foto2 != null)
                {
                    gebruikt.Add(foto1);
                    gebruikt.Add(foto2);

                    Instance.spPictures.Children.Add(CreateImage(foto1));
                    Instance.spPictures.Children.Add(CreateImage(foto2));
                }
            }
        }

        private static Image CreateImage(KioskPhoto photo)
        {
            return new Image
            {
                Source = PathToImage(photo.Source),
                Width = 450, // voor 4 naast elkaar bij 1920 breed
                Height = 300,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(5)
            };
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
