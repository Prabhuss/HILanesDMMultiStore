using Microsoft.AppCenter.Analytics;
using Plugin.Connectivity;
using PyConsumerApp.Controls;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Forms;
using PyConsumerApp.Views.History;
using PyConsumerApp.Views.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Profile
{
    [Preserve(AllMembers = true)]
    [DataContract]
    class MerchantContactViewModel : BaseViewModel
    {

        #region Properties

        public Command OpenWebsiteCommand { get; set; }
        public Command OpenEmailCommand { get; set; }
        public Command DialNumberCommand { get; set; }
        public Command OpenLocationCommand { get; set; }
        

        public bool _IsStoreTiming;
        public bool _IsContactNumber;
        public bool _IsLocation;
        public bool _IsWebsite;
        public bool _IsEmail;
        public bool _NoContact;


        public bool IsStoreTiming
        {
            get => _IsStoreTiming;

            set
            {
                if (_IsStoreTiming == value) return;
                _IsStoreTiming = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsContactNumber
        {
            get => _IsContactNumber;

            set
            {
                if (_IsContactNumber == value) return;
                _IsContactNumber = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsLocation
        {
            get => _IsLocation;

            set
            {
                if (_IsLocation == value) return;
                _IsLocation = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsWebsite
        {
            get => _IsWebsite;

            set
            {
                if (_IsWebsite == value) return;
                _IsWebsite = value;
                NotifyPropertyChanged();
            }
        }
        public bool IsEmail
        {
            get => _IsEmail;

            set
            {
                if (_IsEmail == value) return;
                _IsEmail = value;
                NotifyPropertyChanged();
            }
        }
        public bool NoContact
        {
            get => _NoContact;

            set
            {
                if (_NoContact == value) return;
                _NoContact = value;
                NotifyPropertyChanged();
            }
        }
        

        public MerchantContactDetailsModel _MerchantDetails;

        [DataMember(Name = "_MerchantDetails")]
        public MerchantContactDetailsModel MerchantDetails
        {
            get { return _MerchantDetails; }

            set
            {
                if (_MerchantDetails == value) return;
                _MerchantDetails = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance for the <see cref="ProfileViewModel" /> class.
        /// </summary>
        [Obsolete]
        public MerchantContactViewModel()
        {
            var app = Application.Current as App;
            this.OpenWebsiteCommand = new Command(this.OpenWebsiteCommand_Clicked);
            this.DialNumberCommand = new Command(this.DialNumberCommand_Clicked);
            this.OpenLocationCommand = new Command(this.OpenLocationCommand_Clicked);
            this.OpenEmailCommand = new Command(this.OpenEmailCommand_Clicked); 
            LoadMerchantDetails();
            // this.ProfileImage = App.BaseImageUrl + "ProfileImage16.png";
        }
        #endregion
        public async void LoadMerchantDetails()
        {
            MerchantDetails = await ProfileDataService.Instance.GetMerchantContactDetails();
            if(MerchantDetails != null)
            {
                NoContact = true;
                if (!string.IsNullOrEmpty(MerchantDetails.ContactNumber))
                {
                    IsContactNumber = true;
                    NoContact = false;
                }
                if (!string.IsNullOrEmpty(MerchantDetails.Website))
                {
                    IsWebsite = true;
                    NoContact = false;
                }
                if (!string.IsNullOrEmpty(MerchantDetails.Email))
                {
                    IsEmail = true;
                    NoContact = false;
                }
                if (!string.IsNullOrEmpty(MerchantDetails.Location))
                {
                    IsLocation = true;
                    NoContact = false;
                }
                if (!string.IsNullOrEmpty(MerchantDetails.OpenHours))
                {
                    IsStoreTiming = true;
                    NoContact = false;
                }
            }
        }


        private async void DialNumberCommand_Clicked(object sender)
        {
            try
            {
                if (MerchantDetails.ContactNumber != null)
                {
                    Call(MerchantDetails.ContactNumber);
                    return;
                }
                await Application.Current.MainPage.DisplayAlert("Error", "No Number Available to Call", "ok");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Somthing went wrong while opening the Dialer", "ok");
                throw ex;
            }
        }
        public void Call(string number)
        {
            try
            {
                PhoneDialer.Open(number);
                var app = Application.Current as App;
                Analytics.TrackEvent("Merchant Caller Icon clicked", new Dictionary<string, string> {
                        { "UserPhoneNumber", app.UserPhoneNumber }
                        });
            }
            catch (FeatureNotSupportedException ex)
            {
                //txtNum.Text = "Phone Dialer is not supported on this device." + ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Obsolete]
        private void OpenWebsiteCommand_Clicked(object sender)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                if (MerchantDetails.Website != null)
                {
                    try
                    {
                        Device.OpenUri(new Uri(MerchantDetails.Website));
                    }
                    catch(Exception e)
                    {
                        try
                        {
                            DependencyService.Get<IToastMessage>().ShortTime("Webpage is not accessible at the moment. Please try later");
                        }
                        catch { }
                    }
                }
                else
                {
                    try
                    {
                        DependencyService.Get<IToastMessage>().ShortTime("Server Problem. Please try after some time.");
                    }
                    catch { }
                }
                var app = Application.Current as App;
                Analytics.TrackEvent("Open Merchant Website  clicked", new Dictionary<string, string> {
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

        [Obsolete]
        private void OpenEmailCommand_Clicked(object sender)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                if (MerchantDetails.Email != null)
                {
                    try
                    {
                        Device.OpenUri(new Uri("mailto:" + MerchantDetails.Email));
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            DependencyService.Get<IToastMessage>().ShortTime("Email is not accessible at the moment. Please try later");
                        }
                        catch { }
                    }
                }
                else
                {
                    try
                    {
                        DependencyService.Get<IToastMessage>().ShortTime("Server Problem. Please try after some time.");
                    }
                    catch { }
                }
                var app = Application.Current as App;
                Analytics.TrackEvent("Open Merchant Website  clicked", new Dictionary<string, string> {
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
        async void OpenLocationCommand_Clicked(object sender)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    if (!(string.IsNullOrEmpty(MerchantDetails.Longitude) || string.IsNullOrEmpty(MerchantDetails.Latitude)))
                    {
                        double longitudeval = Double.Parse(MerchantDetails.Longitude);
                        double latitudeval = Double.Parse(MerchantDetails.Latitude);
                        var location = new Location(latitudeval, longitudeval);
                        await Map.OpenAsync(location);
                        var app = Application.Current as App;
                        Analytics.TrackEvent("Location Icon for Merchant Address clicked", new Dictionary<string, string> {
                            { "UserPhoneNumber", app.UserPhoneNumber },
                            });
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "No address to display on Maps", "ok");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Somthing went wrong while opening Maps", "ok");
                }


            }
            else
            {
                DependencyService.Get<IToastMessage>().ShortTime("Check your Internet Connection and try again");

            }
        }

    }
}
