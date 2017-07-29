using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptionMeApp
{
    public class AzureTableManager
    {
        private static AzureTableManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<photocaptioninformation> photoCaptionTable;


        private AzureTableManager()
        {
           this.client = new MobileServiceClient("https://captionme.azurewebsites.net");
           this.photoCaptionTable = this.client.GetTable<photocaptioninformation>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureTableManager AzureTableManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureTableManager();
                }

                return instance;
            }
        }

        public async Task<List<photocaptioninformation>> GetCaptionInformation()
        {
            return await this.photoCaptionTable.OrderBy(c => c.DateUtc).ToListAsync();
        }

        public async Task PostCaptionInformation(photocaptioninformation captionModel)
        {
            await this.photoCaptionTable.InsertAsync(captionModel);
        }

        public async Task UpdateCaptionInformation(photocaptioninformation captionModel)
        {
            await this.photoCaptionTable.UpdateAsync(captionModel);
        }

        public async Task DeleteCaptionInformation(photocaptioninformation captionModel)
        {
            await this.photoCaptionTable.DeleteAsync(captionModel);
        }

    }
}
