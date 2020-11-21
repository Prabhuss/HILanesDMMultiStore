﻿using Plugin.Connectivity;
using PyConsumerApp.Controls;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Bookmarks;
using PyConsumerApp.Views.Catalog;
using PyConsumerApp.Views.Forms;
using PyConsumerApp.Views.Stores;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Catalog
{
    [Preserve(AllMembers = true)]
    [DataContract]
    public class SubCategoryPageViewModel : BaseViewModel
    {
        #region Fields

        bool isBusy;

        private int cartItemCount;

        private string selectedStore;

        private bool displayStoreButton;

        private ObservableCollection<SubCategory> categories;

        private Command loadCategoryItemsCommand;

        private Command categorySelectedCommand; 

        private Command expandingCommand; 

        private Command changeStoreCommand;

        private Command notificationCommand;

        private Command backButtonCommand;

        private Command searchButtonCommand;

        private Command cardItemCommand;
        #endregion

        public SubCategoryPageViewModel()
        {
            var app = Application.Current as App;
            if(app.SelectedMerchantStore != null)
            {
                SelectedStore = app.SelectedMerchantStore;
                DisplayStoreButton = true;
            }
        }
        #region Public properties

        /// <summary>
        /// Gets or sets the property that has been bound with StackLayout, which displays the categories using ComboBox.
        /// </summary>
        [DataMember(Name = "categories")]
        public ObservableCollection<SubCategory> Categories
        {
            get { return this.categories; }
            set
            {
                if (this.categories == value)
                {
                    return;
                }

                this.categories = value;
                this.NotifyPropertyChanged();
            }
        }

        #endregion

        #region Command

        /// <summary>
        /// Gets or sets the command that will be executed when the Category is selected.
        /// </summary>
        public Command LoadCategoryItemsCommand
        {
            get { return loadCategoryItemsCommand ?? (loadCategoryItemsCommand = new Command(populateData)); }
        }
        public Command CategorySelectedCommand
        {
            get { return categorySelectedCommand ?? (categorySelectedCommand = new Command(CategorySelected)); }
        }
        public Command ChangeStoreCommand
        {
            get { return changeStoreCommand ?? (changeStoreCommand = new Command(ChangeStoreCommand_Clicked)); }
        }
        /// <summary>
        /// Gets or sets the command that will be executed when the Notification button is clicked.
        /// </summary>
        /// <summary>
        /// Gets or sets the command is executed when the back button is clicked.
        /// </summary>
        public Command BackButtonCommand
        {
            get { return backButtonCommand ?? (this.backButtonCommand = new Command(this.BackButtonClicked)); }
        }

        public Command SearchButtonCommand
        {
            get { return searchButtonCommand ?? (this.searchButtonCommand = new Command(this.SearchButtonClicked)); }
        }

        public Command NotificationCommand
        {
            get { return notificationCommand ?? (notificationCommand = new Command(this.NotificationClicked)); }
        }
        public Command CardItemCommand
        {
            get { return this.cardItemCommand ?? (this.cardItemCommand = new Command(this.CartClicked)); }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Invoked when the Category is selected.
        /// </summary>
        /// <param name="obj">The Object</param>
        /// 
        public bool IsLoading
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsLoading");
            }
        }
        public int CartItemCount
        {
            get => cartItemCount;
            set
            {
                cartItemCount = value;
                NotifyPropertyChanged();
            }
        }
        public string SelectedStore
        {
            get => selectedStore;
            set
            {
                selectedStore = value;
                NotifyPropertyChanged();
            }
        }
        public bool DisplayStoreButton
        {
            get => displayStoreButton;
            set
            {
                displayStoreButton = value;
                NotifyPropertyChanged();
            }
        }
        public async void UpdateCartItemCount()
        {
            try 
            { 
                var cartItems = await CartDataService.Instance.GetCartItemAsync();
                if (cartItems != null) CartItemCount = cartItems.Count;
            } 
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        
        private async void ChangeStoreCommand_Clicked(object obj)
        {
            var app = Application.Current as App;
            bool answer = await Application.Current.MainPage.DisplayAlert("Message", "Please click on 'change' to switch to different store.", "Change", "No");
            if (answer)
            {
                app.MinimumOrderAmount = 0;
                app.Merchantid = null;
                app.SelectedMerchantStore = null;
                await CartDataService.Instance.RemoveCartItemsAsync();
                Application.Current.MainPage = new NavigationPage(new StoreListPage());
                BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
            }
        }
        private async void CategorySelected(object obj)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    IsBusy = true;
                    //var items = await CategoryDataService.Instance.GetSubCategories((Category)obj);
                    var items = await CategoryDataService.Instance.GetSubCategories((SubCategory)obj);
                    string selectedCategoryName = ((SubCategory)obj).Name;
                    if (items != null)
                    {
                        await Navigation.PushAsync(new SubCategoryPage(items));
                        await Task.Delay(100);
                        IsBusy = false;
                    }
                    else
                    {
                        //await Navigation.PushAsync(new CatalogListPage((Category)obj));
                        await Navigation.PushAsync(new CatalogListPage((SubCategory)obj));
                        await Task.Delay(100);
                        IsBusy = false;
                    }
                }
                finally
                {
                    //IsLoading = false;
                    IsBusy = false;
                }
            }
            else
            {
                try
                {
                    DependencyService.Get<IToastMessage>().ShortTime("No Internet Connection");
                }
                catch { }
                IsBusy = false;
            }


        }
        private async void CartClicked(object obj)
        {
            try
            {
                IsBusy = true;
                if (CartItemCount > 0)
                    await Application.Current.MainPage.Navigation.PushAsync(new CartPage());
            }
            catch (Exception e)
            {

            }
            finally
            {
                IsBusy = false;
            }
            

        }


        async private void populateData()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                ObservableCollection<SubCategory> FetchedCategories = await CategoryDataService.Instance.PopulateData();
                if (FetchedCategories != null)
                {
                    Categories = FetchedCategories;
                }
                else
                {
                    var app = Application.Current as App;
                    Log.Debug("[Error]", "Probably an issue with secret key ");
                    app.IsLoggedIn = false;
                    Application.Current.MainPage = new NavigationPage(new LoginPage(true));
                    BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
                }
            }
            else
            {
                try
                {
                    DependencyService.Get<IToastMessage>().LongTime("No Internet Connection");
                }
                catch { }
            }
            IsLoading = false;
        }
        /// <summary>
        /// Invoked when an back button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void BackButtonClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private async void SearchButtonClicked(object obj)
        {
            await Navigation.PushAsync(new SearchItem());
        }
        private async void NotificationClicked(object obj)
        {
            await Application.Current.MainPage.DisplayAlert("Notifications", "You have no new notification.", "ok");
        }

        #endregion
    }
}
