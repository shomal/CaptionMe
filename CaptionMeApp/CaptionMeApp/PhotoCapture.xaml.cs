using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;

namespace CaptionMeApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoCapture : ContentPage
    {
        public PhotoCapture()
        {
            InitializeComponent();
        }

        private async void TakePicture_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "App Gallery",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            ImageDisplay.Source = ImageSource.FromStream(() => file.GetStream());
        }

        private async void UploadPicture_Clicked(object sender, EventArgs e)
        {
            if(!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;

            ImageDisplay.Source = ImageSource.FromStream(() => file.GetStream());


        }

    }
}
