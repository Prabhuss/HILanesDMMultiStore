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
    public partial class StoreListTemplate : Grid
    {
        public StoreListTemplate()
        {
            InitializeComponent();
        }
    }
}