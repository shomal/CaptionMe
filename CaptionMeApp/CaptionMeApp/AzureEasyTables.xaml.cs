using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CaptionMeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AzureEasyTables : ContentPage
    { 
        public AzureEasyTables()
        {
            InitializeComponent();
        }

        async void ClickedAsync(object sender, System.EventArgs e)
        {
            List<photocaptioninformation> captionInformation = await AzureTableManager.AzureTableManagerInstance.GetCaptionInformation();
            CaptionList.ItemsSource = captionInformation;
        }
    }
}
