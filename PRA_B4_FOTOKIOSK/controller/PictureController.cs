using System.Linq;
using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class PictureController
    {
        public static Home Window { get; set; }

        public List<KioskPhoto> PicturesToDisplay = new List<KioskPhoto>();

        public void Start()
        {
            int vandaag = (int)DateTime.Now.DayOfWeek;
            PicturesToDisplay.Clear();

            DateTime nu = DateTime.Now;
            DateTime ondergrens = nu.AddMinutes(-500); // Voor test, evt -30 bij oplevering
            DateTime bovengrens = nu.AddMinutes(-2);

            List<KioskPhoto> alleFotos = new();

            foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
            {
                string folderName = Path.GetFileName(dir);
                string[] parts = folderName.Split('_');

                if (parts.Length > 0 && int.TryParse(parts[0], out int dagVanMap) && dagVanMap == vandaag)
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        if (file.EndsWith(".jpg") || file.EndsWith(".png"))
                        {
                            string fileName = Path.GetFileNameWithoutExtension(file); // bv. "10_05_30_id3847"
                            string[] tijdDelen = fileName.Split('_');

                            if (tijdDelen.Length >= 3 &&
                                int.TryParse(tijdDelen[0], out int uur) &&
                                int.TryParse(tijdDelen[1], out int minuut) &&
                                int.TryParse(tijdDelen[2], out int seconde))
                            {
                                DateTime fotoTijd = new DateTime(nu.Year, nu.Month, nu.Day, uur, minuut, seconde);

                                if (fotoTijd >= ondergrens && fotoTijd <= bovengrens)
                                {
                                    alleFotos.Add(new KioskPhoto()
                                    {
                                        Id = 0,
                                        Source = file,
                                        Tijd = fotoTijd
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // Sorteer op tijd
            var gesorteerd = alleFotos.OrderBy(f => f.Tijd).ToList();

            var gebruikt = new HashSet<string>();

            for (int i = 0; i < gesorteerd.Count; i++)
            {
                var eerste = gesorteerd[i];
                if (gebruikt.Contains(eerste.Source))
                    continue;

                var tweede = gesorteerd.FirstOrDefault(f =>
                    !gebruikt.Contains(f.Source) &&
                    Math.Abs((f.Tijd - eerste.Tijd).TotalSeconds - 60) < 0.5);

                if (tweede != null)
                {
                    PicturesToDisplay.Add(eerste);
                    PicturesToDisplay.Add(tweede);
                    gebruikt.Add(eerste.Source);
                    gebruikt.Add(tweede.Source);
                }
            }




            PictureManager.UpdatePictures(PicturesToDisplay);
        }


        public void RefreshButtonClick()
        {
            Start(); // Herlaad de foto's van vandaag
        }
    }
}
