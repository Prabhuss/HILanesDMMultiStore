using PyConsumerApp.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PyConsumerApp.DataService
{
    [Preserve(AllMembers = true)]
    class MultiStoreDataService : BaseService
    {
        #region fields
        private static MultiStoreDataService instance;
        #endregion

        #region Properties
        public static MultiStoreDataService Instance => instance ?? (instance = new MultiStoreDataService());
        #endregion


        #region Methods
        public async Task<MultiStoreResponse> GetMultiStoreDetails(string merchantId, string phoneNumber)
        {
            var app = Application.Current as App;
            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload.Add("phone_number", phoneNumber);
            payload.Add("merchant_id", merchantId);
            payload.Add("access_key", app.SecurityAccessKey);
            //payload.Add("access_key", "xxyyzz");

            MultiStoreResponse robject = await this.Post<MultiStoreResponse>(this.getAuthUrl("getStoreDetailsforMultiStore"), payload, null);
            if (robject != null)
            {
                return robject;
            }
            return null;
        }
        #endregion

    }

    [DataContract]
    public class MultiStoreResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "data")]
        public List<MerchantContactDetailsModel> Data { get; set; }
    }
}
