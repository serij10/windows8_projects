using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace photoPuzzle
{
    public partial class Gallery : PhoneApplicationPage
    {
        public Gallery()
        {
            InitializeComponent();
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

        
    }
}