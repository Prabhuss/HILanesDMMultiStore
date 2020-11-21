
using Plugin.Connectivity;
using PyConsumerApp.Models;
using PyConsumerApp.ViewModels.Transaction;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PyConsumerApp.Views.Transaction
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressEditPage : ContentPage
    {
        public AddressEditPage(Address ThisAddress, string PageTitle) 
        {
            
            AddressEditViewodel viewModel = new AddressEditViewodel(ThisAddress, PageTitle);
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}