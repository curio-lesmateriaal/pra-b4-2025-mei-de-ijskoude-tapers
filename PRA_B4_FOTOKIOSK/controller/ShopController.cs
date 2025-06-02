using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class ShopController
    {
        public static Home Window { get; set; }

        public void Start()
        {
            // Vul de productlijst met producten
            ShopManager.Products.Add(new KioskProduct("Foto 10x15", 2.50, "Een mooie foto afdruk 10x15 cm"));
            ShopManager.Products.Add(new KioskProduct("Foto 13x18", 3.50, "Een grote foto afdruk 13x18 cm"));
            ShopManager.Products.Add(new KioskProduct("Foto 20x30", 5.00, "Een posterformaat foto"));

            // Maak prijslijst-string
            string prijsLijst = "Prijzen:\n";
            foreach (KioskProduct product in ShopManager.Products)
            {
                prijsLijst += $"{product.Name} - €{product.Price:F2} - {product.Description}\n";
            }

            // Stel de prijslijst in aan de rechterkant
            ShopManager.SetShopPriceList(prijsLijst);

            // Stel de bon in onderaan het scherm
            ShopManager.SetShopReceipt("Eindbedrag\n€0.00");

            // Update dropdown met producten
            ShopManager.UpdateDropDownProducts();
        }

        // Wordt uitgevoerd wanneer er op de Toevoegen knop is geklikt
        public void AddButtonClick()
        {
            var product = ShopManager.GetSelectedProduct();
            int? fotoId = ShopManager.GetFotoId();
            int? aantal = ShopManager.GetAmount();

            if (product == null || fotoId == null || aantal == null || aantal <= 0)
            {
                MessageBox.Show("Vul een geldig product, fotonummer en aantal in.");
                return;
            }

            double totaal = product.Price * aantal.Value;

            // Haal huidige bon op
            string bon = ShopManager.GetShopReceipt();
            string[] regels = bon.Split('\n');

            // Scheid regels en eindbedrag
            List<string> regelsZonderEind = regels.Where(r => !r.StartsWith("Eindbedrag") && !r.StartsWith("€")).ToList();
            string eindregel = regels.FirstOrDefault(r => r.StartsWith("€")) ?? "€0.00";

            double huidigTotaal = 0;
            double.TryParse(eindregel.Replace("€", "").Trim(), out huidigTotaal);

            double nieuwTotaal = huidigTotaal + totaal;

            // Voeg nieuwe regel toe
            regelsZonderEind.Add($"{aantal}x {product.Name} (€{product.Price:0.00}) Foto #{fotoId} = €{totaal:0.00}");

            // Herbouw bon
            string nieuweBon = string.Join("\n", regelsZonderEind);
            nieuweBon += $"\nEindbedrag\n€{nieuwTotaal:0.00}";

            ShopManager.SetShopReceipt(nieuweBon);
        }

        // Wordt uitgevoerd wanneer er op de Resetten knop is geklikt
        public void ResetButtonClick()
        {
            ShopManager.SetShopReceipt("Eindbedrag\n€0.00");
        }

        // Wordt uitgevoerd wanneer er op de Save knop is geklikt
        public void SaveButtonClick()
        {
            try
            {
                string bonTekst = ShopManager.GetShopReceipt();

                // Map "bonnen" in Documenten
                string rootPad = AppDomain.CurrentDomain.BaseDirectory;
                string mapPad = Path.Combine(rootPad, "bonnen");
                Directory.CreateDirectory(mapPad);


                // Bestandsnaam met timestamp
                string bestandsNaam = $"Bon_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string volledigePad = Path.Combine(mapPad, bestandsNaam);

                File.WriteAllText(volledigePad, bonTekst);

                MessageBox.Show($"Bon succesvol opgeslagen:\n{volledigePad}", "Opslaan gelukt", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij opslaan bon:\n{ex.Message}", "Opslaan mislukt", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
