using PyConsumerApp.ViewModels.Profile;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PyConsumerApp.Views.Profile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MerchantContactPage : PopupPage
    {
        public MerchantContactPage()
        {
            InitializeComponent();
            BindingContext = new MerchantContactViewModel();
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // enable back button
        }
    }
}