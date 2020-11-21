
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Plugin.Connectivity;
using PyConsumerApp.Controls;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Forms;
using PyConsumerApp.Views.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Stores
{
    [Preserve(AllMembers = true)]
    [DataContract]
    public class StoresViewModel : BaseViewModel
    {
        public StoresViewModel()
        {
            _StoreList = new ObservableCollection<MerchantContactDetailsModel>();
        }
        private ObservableCollection<MerchantContactDetailsModel> _StoreList;
        public Command loadCategoryItemsCommand;
        public Command LoadCategoryItemsCommand
        {
            get { return loadCategoryItemsCommand ?? (loadCategoryItemsCommand = new Command(this.PopulateStoreList)); }
        }
        public Command storeItemTappedCommand;
        public Command StoreItemTappedCommand
        {
            get { return storeItemTappedCommand ?? (storeItemTappedCommand = new Command(this.StoreItemTapped)); }
        }


        private bool _IsLoading;
        public bool IsLoading
        {
            get
            { return this._IsLoading; }
            set
            {
                _IsLoading = value;
                NotifyPropertyChanged();
            }
        }
        private bool _IsClicked;
        public bool IsClicked
        {
            get
            { return this._IsClicked; }
            set
            {
                _IsClicked = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<MerchantContactDetailsModel> StoreList
        {

            get
            {
                return this._StoreList;
            }

            set
            {
                if (this._StoreList == value)
                {
                    return;
                }

                this._StoreList = value;
                this.NotifyPropertyChanged();
                //this.GetProducts(this.MyOrders);
            }
        }


        public async void PopulateStoreList()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    var app = Application.Current as App;
                    var StoreResponse = await MultiStoreDataService.Instance.GetMultiStoreDetails(app.ParentMerchantid, app.UserPhoneNumber);
                    StoreList = new ObservableCollection<MerchantContactDetailsModel>(StoreResponse.Data);
                }
                catch (Exception e)
                {
                    var app = Application.Current as App;
                    Log.Debug("[Error]", "Probably an issue with secret key ");
                    app.IsLoggedIn = false;
                    await Application.Current.MainPage.DisplayAlert("Server Time Out", "Your session has expired due to some error. Please login again to continue.", "OK, Got It!");
                    Application.Current.MainPage = new NavigationPage(new LoginPage(true));
                    BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
                }
            }
            else
            {

                try
                {
                    DependencyService.Get<IToastMessage>().LongTime("No Internet Connection! Unable to load Store List.");
                }
                catch { }
            }
            IsLoading = false;
        }


        private async void StoreItemTapped(object sender)
        {
            IsClicked = true; 
            SelectStore(sender);
        }
        private async void SelectStore(object sender)
        {

            var app = App.Current as App;
            var selectedMerchantStore = sender as MerchantContactDetailsModel;
            app.Merchantid = selectedMerchantStore.MerchantBranchId;
            app.SelectedMerchantStore = selectedMerchantStore.ShopName;
            await CartDataService.Instance.RestoreCartItemAsync();
            //await Navigation.PushAsync(new NavigationPage(new BottomNavigationPage()));
            Application.Current.MainPage = new NavigationPage(new BottomNavigationPage());
            BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
            await Task.Delay(10);
            IsClicked = false;
        }
        public void PopulateStoreListDefault()
        {
            StoreList.Clear();
            StoreList.Add(new MerchantContactDetailsModel()
            {
                ShopName = "Test Parent Store 2",
                MerchantBranchId = "2",
                MerchantAddress = "TEST WITH PARENT MERCHANT ID i.e 2",
                MerchantPinCode = "222222"

            });
            StoreList.Add(new MerchantContactDetailsModel()
            {
                ShopName = "Test Store 202",
                MerchantBranchId = "202",
                MerchantAddress = "HNo. 222, Demo Address 07, Locality, city, state",
                MerchantPinCode = "222222"

            });

            StoreList.Add(new MerchantContactDetailsModel()
            {
                ShopName = "Test Store 205",
                MerchantBranchId = "205",
                MerchantAddress = "HNo. 222, Demo Address 08, Locality, city, state",
                MerchantPinCode = "222222"

            });
            StoreList.Add(new MerchantContactDetailsModel()
            {
                ShopName = "Test Store 206",
                MerchantBranchId = "206",
                MerchantAddress = "HNo. 222, Demo Address 07, Locality, city, state",
                MerchantPinCode = "222222"

            });

            StoreList.Add(new MerchantContactDetailsModel()
            {
                ShopName = "Test Store 208",
                MerchantBranchId = "208",
                MerchantAddress = "HNo. 222, Demo Address 08, Locality, city, state",
                MerchantPinCode = "222222"

            });
            StoreList.Add(new MerchantContactDetailsModel()
            {
                ShopName = "Test Store 207",
                MerchantBranchId = "207",
                MerchantAddress = "HNo. 222, Demo Address 07, Locality, city, state",
                MerchantPinCode = "222222"

            });
        }
    }

}
