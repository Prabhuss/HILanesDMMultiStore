using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Plugin.Connectivity;
using PyConsumerApp.Controls;
using PyConsumerApp.DataService;
using PyConsumerApp.Models;
using PyConsumerApp.Views.Catalog;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.ViewModels.Catalog
{
    /// <summary>
    /// ViewModel for Category page.
    /// </summary>
    [Preserve(AllMembers = true)]
    [DataContract]
    public class CategoryPageViewModel : BaseViewModel
    {
        #region Fields

        bool isBusy;
        private ObservableCollection<Category> categories;

        private Command categorySelectedCommand;

        private Command notificationCommand;

        private Command backButtonCommand;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the property that has been bound with StackLayout, which displays the categories using ComboBox.
        /// </summary>
        [DataMember(Name = "categories")]
        public ObservableCollection<Category> Categories
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
        public Command CategorySelectedCommand
        {
            get { return categorySelectedCommand ?? (categorySelectedCommand = new Command(CategorySelected)); }
        }

        /// <summary>
        /// Gets or sets the command that will be executed when the Notification button is clicked.
        /// </summary>
        public Command NotificationCommand
        {
            get { return notificationCommand ?? (notificationCommand = new Command(this.NotificationClicked)); }
        }

        /// <summary>
        /// Gets or sets the command is executed when the back button is clicked.
        /// </summary>
        public Command BackButtonCommand
        {
            get { return backButtonCommand ?? (this.backButtonCommand = new Command(this.BackButtonClicked)); }
        }

        #endregion

        #region Methods

        public bool IsLoading
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsLoading");
            }
        }

        /// <summary>
        /// Invoked when the Category is selected.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void CategorySelected(object obj)
        {
            IsBusy = true;
            //await Navigation.PushAsync(new CatalogListPage((Category)obj));
            if (CrossConnectivity.Current.IsConnected)
            {
                var items = await CategoryDataService.Instance.GetSubCategories((SubCategory)obj);
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
            else
            {
                try
                {
                    DependencyService.Get<IToastMessage>().LongTime("No Internet Connection");
                }
                catch { }
                IsBusy = false;
            }
        }

        /// <summary>
        /// Invoked when the notification button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private async void NotificationClicked(object obj)
        {
            await Application.Current.MainPage.DisplayAlert("Notifications", "You have no new notification.", "ok");
        }

        /// <summary>
        /// Invoked when an back button is clicked.
        /// </summary>
        /// <param name="obj">The Object</param>
        private void BackButtonClicked(object obj)
        {
            // Do something
        }

        #endregion
    }
}