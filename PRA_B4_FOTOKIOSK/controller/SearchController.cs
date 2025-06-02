using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class SearchController
    {
        public Home Window { get; set; }

        public SearchController(Home window)
        {
            Window = window;
        }

        public void Start()
        {
            // Indien nodig init
        }

        public void SearchButtonClick()
        {
            try
            {
                string zoekInput = SearchManager.GetSearchInput();
                if (string.IsNullOrWhiteSpace(zoekInput))
                {
                    SearchManager.SetSearchImageInfo("Voer dag en tijd in, bijvoorbeeld: 2 10:05:30");
                    return;
                }

                var delen = zoekInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (delen.Length != 2)
                {
                    SearchManager.SetSearchImageInfo("Onjuist formaat, gebruik: dag tijd (bv. 2 10:05:30)");
                    return;
                }

                if (!int.TryParse(delen[0], out int dag))
                {
                    SearchManager.SetSearchImageInfo($"Ongeldige dag: {delen[0]}");
                    return;
                }

                if (!TimeSpan.TryParse(delen[1], out TimeSpan tijd))
                {
                    SearchManager.SetSearchImageInfo($"Ongeldige tijd: {delen[1]}");
                    return;
                }

                DateTime nu = DateTime.Now;
                int vandaag = (int)nu.DayOfWeek;
                int dagenVerschil = dag - vandaag;
                DateTime datumGezocht = nu.Date.AddDays(dagenVerschil);

                DateTime zoekStart = datumGezocht.Add(tijd);
                DateTime zoekEind = zoekStart.AddMinutes(1);

                List<KioskPhoto> gevondenFotos = new List<KioskPhoto>();

                foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
                {
                    string folderName = Path.GetFileName(dir);
                    string[] parts = folderName.Split('_');

                    if (parts.Length > 0 && int.TryParse(parts[0], out int dagVanMap) && dagVanMap == dag)
                    {
                        foreach (string file in Directory.GetFiles(dir))
                        {
                            if (file.EndsWith(".jpg") || file.EndsWith(".png"))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(file);
                                string[] tijdDelen = fileName.Split('_');

                                if (tijdDelen.Length >= 3 &&
                                    int.TryParse(tijdDelen[0], out int uur) &&
                                    int.TryParse(tijdDelen[1], out int minuut) &&
                                    int.TryParse(tijdDelen[2], out int seconde))
                                {
                                    DateTime fotoTijd = new DateTime(datumGezocht.Year, datumGezocht.Month, datumGezocht.Day, uur, minuut, seconde);

                                    if (fotoTijd >= zoekStart && fotoTijd <= zoekEind)
                                    {
                                        int fotoId = 0;
                                        foreach (string deel in tijdDelen)
                                        {
                                            if (deel.StartsWith("id") && int.TryParse(deel.Substring(2), out int parsedId))
                                            {
                                                fotoId = parsedId;
                                                break;
                                            }
                                        }

                                        gevondenFotos.Add(new KioskPhoto
                                        {
                                            Id = fotoId,
                                            Source = file,
                                            Tijd = fotoTijd
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                if (gevondenFotos.Any())
                {
                    var foto = gevondenFotos.First();
                    if (File.Exists(foto.Source))
                    {
                        SearchManager.SetPicture(foto.Source);
                        string info = $"📸 Foto-informatie:\n- ID: {foto.Id}\n- Tijd: {foto.Tijd:HH:mm:ss}\n- Datum: {foto.Tijd:dd-MM-yyyy}";
                        SearchManager.SetSearchImageInfo(info);
                    }
                    else
                    {
                        SearchManager.SetSearchImageInfo("Bestand bestaat niet.");
                    }
                }
                else
                {
                    SearchManager.SetSearchImageInfo("Geen foto's gevonden voor deze dag en tijd.");
                }
            }
            catch (Exception ex)
            {
                SearchManager.SetSearchImageInfo("Fout bij zoeken: " + ex.Message);
            }
        }
    }
}
