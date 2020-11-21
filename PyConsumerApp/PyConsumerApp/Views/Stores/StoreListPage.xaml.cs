using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.ViewModels;
using PyConsumerApp.ViewModels.Stores;
using PyConsumerApp.Views.Catalog;
using PyConsumerApp.Views.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PyConsumerApp.Views.Stores
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreListPage : ContentPage
    {
        public StoresViewModel viewModels;
        public StoreListPage()
        {
            InitializeComponent();
            viewModels = new StoresViewModel();
            BindingContext = viewModels;
            viewModels.IsLoading = true;
        } 

    }
}