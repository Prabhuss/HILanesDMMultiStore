using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PyConsumerApp.Views.Detail
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayImagePage : ContentPage
    {
        public DisplayImagePage(Uri Imagestream = null)
        {
            InitializeComponent();
            ImageCropper.ToolbarSettings.IsVisible = false;
             ImageCropper.Source = ImageSource.FromUri(Imagestream);
        }

        private async void backButton_Clicked(object sender, EventArgs e)
        {

            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}


