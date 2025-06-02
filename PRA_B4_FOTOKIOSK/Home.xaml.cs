using PRA_B4_FOTOKIOSK.controller;
using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System.Windows;

namespace PRA_B4_FOTOKIOSK
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public ShopController ShopController { get; set; }
        public PictureController PictureController { get; set; }
        public SearchController SearchController { get; set; }

        public Home()
        {
            InitializeComponent();

            // Stel SearchManager.Instance in voor toegang tot UI controls vanuit SearchManager
            SearchManager.Instance = this;

            // Maak de controllers en geef 'this' door als window
            ShopController = new ShopController(this);
            PictureController = new PictureController(this);
            SearchController = new SearchController(this);

            // Stel de managers in
            PictureManager.Instance = this;
            ShopManager.Instance = this;

            // Start de controllers (check dat Start() bestaat)
            PictureController.Start();
            ShopController.Start();
            // Als SearchController geen Start() heeft, haal deze regel weg
            // SearchController.Start();
        }

        private void btnShopAdd_Click(object sender, RoutedEventArgs e)
        {
            ShopController.AddButtonClick();
        }

        private void btnShopReset_Click(object sender, RoutedEventArgs e)
        {
            ShopController.ResetButtonClick();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            PictureController.RefreshButtonClick();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ShopController.SaveButtonClick();
        }

        private void btnZoeken_Click(object sender, RoutedEventArgs e)
        {
            SearchController.SearchButtonClick();
        }
    }
}
