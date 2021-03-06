﻿using Microsoft.AppCenter.Analytics;
using Plugin.Connectivity;
using PyConsumerApp.Controls;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Forms;
using PyConsumerApp.Views.History;
using PyConsumerApp.Views.Profile;
using PyConsumerApp.Views.Stores;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Profile
{
    /// <summary>
    /// ViewModel for health profile page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class ProfileViewModel : BaseViewModel
    {

        //private ObservableCollection<CustomerProfile> profile;
        private CustomerProfile profile;
        public Command LogOutCommand { get; set; }
        public Command MyOrdersCommand { get; set; }
        public Command ChangeCustomerInfo { get; set; }
        public Command RedirectToMerchantUrl { get; set; }
        public Command RedirectToMerchantContactPage { get; set; }

        App app;

        private Command backButtonCommand; 

        private Command changeStoreCommand; 
        public Command BackButtonCommand//{ get; set; }
        {
            get { return backButtonCommand ?? (this.backButtonCommand = new Command(this.BackButtonClicked)); }
        }

        public Command ChangeStoreCommand
        {
            get { return changeStoreCommand ?? (changeStoreCommand = new Command(ChangeStoreCommand_Clicked)); }
        }
        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="ProfileViewModel" /> class.
        /// </summary>
        public ProfileViewModel()
        {
            app = Application.Current as App;
            if(app.SelectedMerchantStore != null)
            {
                DisplayStoreButton = true;
            }
            this.EditProfileCommand = new Command(this.EditProfileClicked);
            this.RedirectToMerchantContactPage = new Command(this.RedirectToMerchantContactPage_Clicked);
            this.ProfileImage = App.profileImage;
            LogOutCommand = new Command(this.Logout_Clicked);
            MyOrdersCommand = new Command(this.MyOrders_Clicked);
            this.AskEditProfileCommand = new Command(this.ChangeInfoDialogBox_Clicked);
            ChangeCustomerInfo = new Command(this.ChangeInfo_Clicked);
            RedirectToMerchantUrl = new Command(this.RedirectToMerchantUrl_Clicked);
        }
        #endregion


        private async void Logout_Clicked(object obj)
        {
            var app = Application.Current as App;
            bool answer = await Application.Current.MainPage.DisplayAlert("Log Out", "Do you really want to log out?", "Yes", "no");
            if (answer)
            {
                app.IsLoggedIn = false;
                app.UserPhoneNumber = null;
                app.SecurityAccessKey = null;
                app.SelectedMerchantStore = null;
                app.MinimumOrderAmount = 0;
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
            }
        }

        private async void ChangeInfo_Clicked(object obj)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                Analytics.TrackEvent("ModifyUserInfo_Clicked", new Dictionary<string, string> {
                            { "MerchantBranchId", app.Merchantid},
                            { "UserPhoneNumber", app.UserPhoneNumber},
                            });
                bool resonse = await ProfileDataService.Instance.SaveCustomerInfo(Profile);
                GetUserProfile();
                if (resonse == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Message", "Profile changed successfully", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error Message", "Something went Wrong", "Ok");
                    await Application.Current.MainPage.Navigation.PopAsync();
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
        }

        private async void ChangeStoreCommand_Clicked(object obj)
        {
            var app = Application.Current as App;
            bool answer = await Application.Current.MainPage.DisplayAlert("Switch Store", "Do you really want to Switch to different store", "Yes", "no");
            if (answer)
            {
                app.MinimumOrderAmount = 0;
                app.Merchantid = null;
                app.SelectedMerchantStore = null;
                await CartDataService.Instance.RemoveCartItemsAsync();
                Application.Current.MainPage = new NavigationPage(new StoreListPage());
                BaseViewModel.Navigation = Application.Current.MainPage.Navigation;
                await Task.Delay(100);
            }
        }

        public async void GetUserProfile()
        {
            try
            {
                CustomerProfile customerProfile = await ProfileDataService.Instance.GetUserInfo();
                if (customerProfile != null)
                {
                    Profile = customerProfile;
                }
                var GstStatus = await ProfileDataService.Instance.getGSTSettingDetails();
                if(GstStatus!= null)
                {
                    if(GstStatus.GST_Active.ToLower() == "yes")
                    {
                        DisplayGstNumber = true;
                    }
                    else
                    {
                        DisplayGstNumber = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"error "+ex.Message);
            }
            
        }
        [DataMember(Name = "profile")]
        public CustomerProfile Profile
        {
            get => profile;

            set
            {
                if (profile == value) return;

                profile = value;
                NotifyPropertyChanged();
            }
        }
        
        private bool _DisplayGstNumber = false;
        public bool DisplayGstNumber
        {
            get
            {
                return _DisplayGstNumber;
            }
            set
            {
                if (_DisplayGstNumber == value)
                {
                    return;
                }

                _DisplayGstNumber = value;
                NotifyPropertyChanged();
            }
        }
        private string _TnCTitle;
        public string TnCTitle
        {
            get
            {
                return _TnCTitle;
            }
            set
            {
                if (_TnCTitle == value)
                {
                    return;
                }

                _TnCTitle = value;
                NotifyPropertyChanged();
            }
        }
        private bool _DisplayStoreButton;
        public bool DisplayStoreButton
        {
            get
            {
                return _DisplayStoreButton;
            }
            set
            {
                if (_DisplayStoreButton == value)
                {
                    return;
                }
                _DisplayStoreButton = value;
                this.NotifyPropertyChanged();
            }
        }

        private string _TnCUrl;
        public string TnCUrl
        {
            get
            {
                return _TnCUrl;
            }
            set
            {
                if (_TnCUrl == value)
                {
                    return;
                }
                _TnCUrl = value;
                this.NotifyPropertyChanged();
            }
        }
        /* public ObservableCollection<CustomerProfile> Profile
         {
             get { return this.profile; }
             set
             {
                 if (this.profile == value)
                 {
                     return;
                 }

                 this.profile = value;
                 this.NotifyPropertyChanged();
             }
         }*/


        private async void ChangeInfoDialogBox_Clicked()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Message", "Click on 'Edit Profile' to Add/Change details", "Edit Profile", "Cancel");
            if (answer)
            {
                ProfileEditPage view = new ProfileEditPage();
                await Navigation.PushAsync(view);
                await Task.Delay(10);
            }
        }
        private async void EditProfileClicked()
        {
            ProfileEditPage view = new ProfileEditPage();
            await Navigation.PushAsync(view);
            await Task.Delay(10);
        }
        public async void RedirectToMerchantContactPage_Clicked()
        {
            MerchantContactPage view = new MerchantContactPage();
            await Navigation.PushPopupAsync(view, true);
            await Task.Delay(10);
        }
        private async void MyOrders_Clicked()
        {
            MyOrdersPage view = new MyOrdersPage();
            await Navigation.PushAsync(view);
            await Task.Delay(10);
        }

        public Command EditProfileCommand { get; set; }
        public Command AskEditProfileCommand { get; set; }

        public string ProfileImage { get; set; }

        public void RedirectToMerchantUrl_Clicked()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                if(TnCUrl != null)
                    Device.OpenUri(new Uri(TnCUrl));
                else
                {
                    try
                    {
                        DependencyService.Get<IToastMessage>().ShortTime("Server Problem. Please try after some time.");
                    }
                    catch { }
                }
                var app = Application.Current as App;
                Analytics.TrackEvent("Terms & Conditions Button clicked", new Dictionary<string, string> {
                            { "UserPhoneNumber", app.UserPhoneNumber }
                            });
            }
            else
            {
                try
                {
                    DependencyService.Get<IToastMessage>().ShortTime("Check your Internet Connection and try again");
                }
                catch { }
            }
        }
        public async void GetTnCMessage()
        {
            Merchantdata MerchantSettingDetails = await CategoryDataService.Instance.GetMerchantSettings("TnCMessage");
            if (MerchantSettingDetails != null)
            {
                if (MerchantSettingDetails.SettingIsActive.ToLower() == "yes")
                {
                    TnCTitle = MerchantSettingDetails.SettingValue;
                    TnCUrl = MerchantSettingDetails.SettingMessage;
                }
            }
        }
        private async void BackButtonClicked(object obj)
        {
            // Do something
            //await Navigation.PopAsync();
            await Application.Current.MainPage.Navigation.PopAsync();
        }

    }
}