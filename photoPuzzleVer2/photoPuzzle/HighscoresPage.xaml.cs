using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace photoPuzzle
{
    public partial class HighscoresPage : PhoneApplicationPage
    {
        public HighscoresPage()
        {
            InitializeComponent();
            App.ViewModel.LoadCollectionsFromDatabase();
            this.DataContext = App.ViewModel;         
        }
    }
}