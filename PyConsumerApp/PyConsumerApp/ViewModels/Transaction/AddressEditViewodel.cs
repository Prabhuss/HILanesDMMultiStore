using Plugin.Connectivity;
using Plugin.Geolocator;
using PyConsumerApp.Controls;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Linq;

namespace PyConsumerApp.ViewModels.Transaction
{
    [Preserve(AllMembers = true)]
    [DataContract]
    class AddressEditViewodel : BaseViewModel
    {
        public Command ChangeAddressInfo { get; set; }
        public Command UseMyLocationCommand { get; set; }
        public string PageHeader { get; private set; }

        public Address customerAddress;

        public ObservableCollection<string> addressTypesList;
        public ObservableCollection<string> AddressTypesList
        {
            get { return addressTypesList; }
            set
            {
                if (addressTypesList != value)
                {
                    addressTypesList = value;
                    OnPropertyChanged();
                }
            }
        }
        
        string _addressLocationMessage;
        [DataMember(Name = "AddressLocationMessage")]
        public string AddressLocationMessage
        {
            get { return _addressLocationMessage; }
            set
            {
                if (_addressLocationMessage != value)
                {
                    _addressLocationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        string selectedAddressType;
        [DataMember(Name = "selectedAddressType")]
        public string SelectedAddressType
        {
            get { return selectedAddressType; }
            set
            {
                if (selectedAddressType != value)
                {
                    selectedAddressType = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(Name = "customerAddress")]
        public Address CustomerAddress
        {
            get => customerAddress;

            set
            {
                if (customerAddress == value) return;
                customerAddress = value;
                OnPropertyChanged();
                NotifyPropertyChanged();
            }
        }

        private bool _locationCheckValue = true;
        [DataMember(Name = "LocationCheckValue")]
        public bool LocationCheckValue
        {
            get => _locationCheckValue;

            set
            {
                if (_locationCheckValue == value) return;
                _locationCheckValue = value;
                NotifyPropertyChanged();
            }
        }
        public AddressEditViewodel(Address ThisAddress,string PageTitle)
        {
            PageHeader = PageTitle;
            customerAddress = new Address();
            CustomerAddress = ThisAddress;
            addressTypesList = new ObservableCollection<string>();
            addressTypesList.Add("Address 1");
            addressTypesList.Add("Address 2");
            addressTypesList.Add("Address 3");
            selectedAddressType = "";
            InitilizePage();
            ChangeAddressInfo = new Command(this.ChangeAddress_Clicked);
        }

        private Command backButtonCommand;
        [DataMember(Name = "backButtonCommand")]
        public Command BackButtonCommand => backButtonCommand ?? (backButtonCommand = new Command(BackButtonClicked));

        private async void BackButtonClicked(object obj)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task UseMyLocation_Clicked()
        {
            try
            {
                //fetch position
                /*
                var locator = CrossGeolocator.Current;
                var position = await locator.GetLastKnownLocationAsync();
                if (position == null)
                    position = await locator.GetPositionAsync();

                if (position != null)
                {
                    CustomerAddress.Latitude = position.Latitude.ToString();
                    CustomerAddress.Longitude = position.Longitude.ToString();
                }
                else
                {
                    //if error in fetching the location 
                    DependencyService.Get<IToastMessage>().ShortTime("Error L01: Unable to fetch Current location");
                    return;
                }
                */
                //check for cache location because it is faster
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    //if no cache location is stored generate new request for location
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    request.Timeout = TimeSpan.FromSeconds(10);
                    location = await Geolocation.GetLocationAsync(request);
                    if (location == null)
                    {
                        //if erroe in fetching the location 
                        DependencyService.Get<IToastMessage>().LongTime("Error L01: Unable to fetch Current location");
                        return;
                    }
                }
                //assigning location to the Customer address
                CustomerAddress.Latitude = location.Latitude.ToString();
                CustomerAddress.Longitude = location.Longitude.ToString();
                
                //await Application.Current.MainPage.DisplayAlert("Testing", $"position: {position.Latitude},{position.Longitude}; location: {location.Latitude}, {location.Longitude}", "Ok");

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                DependencyService.Get<IToastMessage>().ShortTime("Error L02: Location Feature is not supported by your device.");
                return;
            }
            catch (PermissionException pEx)
            {
                DependencyService.Get<IToastMessage>().ShortTime("Error L03: Seems like your GPS is OFF.");
                return;
            }
            catch (System.Exception ex)
            {
                DependencyService.Get<IToastMessage>().ShortTime("Error L04: Unable to fetch Current Location");
                return;
            }
        }
        private async void ChangeAddress_Clicked(object obj)
        {
            IsBusy = true;
            if (CrossConnectivity.Current.IsConnected)
            {
                if (CustomerAddress.FirstName == null || CustomerAddress.FirstName.Replace(" ", "") == "" ||
                    CustomerAddress.Address2 == null || CustomerAddress.Address2.Replace(" ", "") == "" ||
                    CustomerAddress.TagName == null || CustomerAddress.TagName.Replace(" ", "") == "" ||
                    CustomerAddress.SocietyBuildingNo == null || CustomerAddress.SocietyBuildingNo.Replace(" ", "") == "" ||
                    CustomerAddress.PostalCodeZipCode == null || CustomerAddress.PostalCodeZipCode.Replace(" ", "") == "")
                {
                    await Application.Current.MainPage.DisplayAlert("Fields Empty Error", "Please fill all the Mandatory fields", "Ok");
                    IsBusy = false;
                    return;
                }
                //concatenate floorNoDoorNo and Address
                try
                {
                    if(!(CustomerAddress.FlatNoDoorNo == null || CustomerAddress.FlatNoDoorNo.Trim() == ""))
                    {
                        CustomerAddress.Address1 = CustomerAddress.FlatNoDoorNo + ", " + CustomerAddress.SocietyBuildingNo;
                    }
                    else
                    {
                        CustomerAddress.Address1 = CustomerAddress.SocietyBuildingNo;
                    }
                }
                catch { }
                try
                {
                    //reset Location
                    CustomerAddress.Latitude = null;
                    CustomerAddress.Longitude = null;
                    if (LocationCheckValue)
                    {
                        bool PermissionForLocation = await CheckLocationPermissions();
                        if (PermissionForLocation)
                           await UseMyLocation_Clicked();
                    }
                    bool resonse = await CartDataService.Instance.SaveAddressInfo(CustomerAddress);
                    if (resonse == true)
                    {
                        DependencyService.Get<IToastMessage>().LongTime("Address saved successfully");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortTime("Error AD01: Something went wrong while changing the address details");
                    }
                    IsBusy = false;
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Error AD02", e.Message, "OK");
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

        //we will fetch actomatic location component and Location message here
        public void InitilizePage()
        {
            ///set "use my location" according to existing details
            if(CustomerAddress.FirstName != null && CustomerAddress.Latitude == null)
            {
                LocationCheckValue = false;
            }
            //Location Component
            FetchLocationFieldsFromGPS();
            SetGpsLocationMessage();
        }
        public async void FetchLocationFieldsFromGPS()
        {
            try
            {
                //fetch position
                var locator = CrossGeolocator.Current;
                var position = await locator.GetLastKnownLocationAsync();
                if (position == null)
                    position = await locator.GetPositionAsync(TimeSpan.FromSeconds(30));

                if (position != null)
                {
                    //if position fetched successfully.. Fetch address for the position
                    var address = await locator.GetAddressesForPositionAsync(position, "RJHqIE53Onrqons5CNOx~FrDr3XhjDTyEXEjng-CRoA~Aj69MhNManYUKxo6QcwZ0wmXBtyva0zwuHB04rFYAPf7qqGJ5cHb03RCDw1jIW8l");
                    if (address == null)
                        return;
                    var a = address.FirstOrDefault();
                    var myAddress = new Address();
                    myAddress.PostalCodeZipCode = a.PostalCode;
                    string addressField = null;
                    if (a.Thoroughfare != null)
                    {
                        addressField = a.Thoroughfare;
                        if (a.SubAdminArea != null)
                            addressField = addressField + ", " + a.SubAdminArea;
                        if (a.AdminArea != null)
                            addressField = addressField +  ", " + a.AdminArea;
                    }
                    else if (a.SubAdminArea != null)
                    {
                            addressField = a.SubAdminArea;
                        if (a.AdminArea != null)
                            addressField = addressField + ", " + a.AdminArea;
                    }
                    else if (a.AdminArea != null)
                    {
                        addressField = a.AdminArea;
                    }
                    myAddress.SocietyBuildingNo = addressField;
                    if (CustomerAddress.FirstName == null)
                        CustomerAddress = myAddress;
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        public async void SetGpsLocationMessage()
        {
            Merchantdata MerchantSettingDetails = await CategoryDataService.Instance.GetMerchantSettings("LocationMessage");
            if (MerchantSettingDetails != null)
            {
                if (MerchantSettingDetails.SettingIsActive.ToLower() == "yes")
                {
                    AddressLocationMessage = MerchantSettingDetails.SettingMessage;
                }
            }
        }
        private async Task<bool> CheckLocationPermissions()
        {
            try
            {
                PermissionStatus permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (permission != PermissionStatus.Granted)
                {
                    permission = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                if (permission != PermissionStatus.Granted)
                    return false;
            }
            catch (Exception e)
            {
                var x = e.Message;
                return false;
            }
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<ILocSettings>().OpenSettings();
            }
            catch (Exception e)
            {
            }
            return true;
        }
    }
}
