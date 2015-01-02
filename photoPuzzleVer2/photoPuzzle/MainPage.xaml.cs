using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using photoPuzzle.Resources;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;

namespace photoPuzzle
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        void photo_Completed(object sender, PhotoResult e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MyPhoto"))
            {
                settings["MyPhoto"] = e.ChosenPhoto;
            }
            else
            {
                settings.Add("MyPhoto", e.ChosenPhoto);
            }
            this.Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            });
        }

        private void camera_Completed(object sender, PhotoResult e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MyPhoto"))
            {
                settings["MyPhoto"] = e.ChosenPhoto;
            }
            else
            {
                settings.Add("MyPhoto", e.ChosenPhoto);
            }
            this.Dispatcher.BeginInvoke(() =>
            {
                this.NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            });
        }

        private void btnCamera_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CameraCaptureTask camera = new CameraCaptureTask();
            camera.Completed += camera_Completed;
            camera.Show();
        }

        private void btnRandom_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // navigate
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MyPhoto"))
            {
                settings["MyPhoto"] = null;
            }
            else
            {
                settings.Add("MyPhoto", null);
            }

            this.NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void btnLibrary_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhotoChooserTask photo = new PhotoChooserTask();
            photo.Completed += photo_Completed;
            photo.ShowCamera = true;
            photo.Show();
        }

        private void btnGallery_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Gallery.xaml", UriKind.Relative));
        }

        private void btnScores_tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/HighscoresPage.xaml", UriKind.Relative));
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}