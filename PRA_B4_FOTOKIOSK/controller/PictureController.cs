using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class PictureController
    {
        // De window die we laten zien op het scherm
        public static Home Window { get; set; }

        // De lijst met fotos die we laten zien
        public List<KioskPhoto> PicturesToDisplay = new List<KioskPhoto>();

        // Start methode die wordt aangeroepen wanneer de foto pagina opent
        public void Start()
        {
            int vandaag = (int)DateTime.Now.DayOfWeek;
            PicturesToDisplay.Clear();

            DateTime nu = DateTime.Now;
            DateTime ondergrens = nu.AddMinutes(-500);// Nog wel aanpassen voor de oplevering naar -30
            DateTime bovengrens = nu.AddMinutes(-2);

            foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
            {
                string folderName = Path.GetFileName(dir);
                string[] parts = folderName.Split('_');

                if (parts.Length > 0 && int.TryParse(parts[0], out int dagVanMap))
                {
                    if (dagVanMap == vandaag)
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
                                        PicturesToDisplay.Add(new KioskPhoto() { Id = 0, Source = file });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            PictureManager.UpdatePictures(PicturesToDisplay);
        }

        // Wordt uitgevoerd wanneer er op de Refresh knop is geklikt
        public void RefreshButtonClick()
        {
            Start(); // Herlaad de foto's van vandaag
        }
    }
}
