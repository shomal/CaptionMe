using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Android.Util;

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

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
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

            ImageDisplay.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await CreateImageDescription(file);
            
        }


        async Task postLocationAsync(string caption, float confidence)
        {

            photocaptioninformation captionModel = new photocaptioninformation()
            {
                DateUtc = DateTime.UtcNow,
                Caption = caption,
                Confidence = confidence
            };

            await AzureTableManager.AzureTableManagerInstance.PostCaptionInformation(captionModel);
        }


        //private async void UploadPicture_Clicked(object sender, EventArgs e)
        //{
        //    if(!CrossMedia.Current.IsPickPhotoSupported)
        //    {
        //        await DisplayAlert("No upload", "Picking a photo is not supported.", "OK");
        //        return;
        //    }

        //    MediaFile file = await CrossMedia.Current.PickPhotoAsync();
        //    if (file == null)
        //        return;

        //    ImageDisplay.Source = ImageSource.FromStream(() => file.GetStream());
        //}

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task CreateImageDescription(MediaFile file)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "51d20aa4b6164fabbf162f9d2de32c91");
            string url = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?visualFeatures=Description";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    PhotoCaptionModel model = JsonConvert.DeserializeObject<PhotoCaptionModel>(responseString);
                    string imageCaption = model.description.captions.FirstOrDefault().text;
                    float captionConfidence = model.description.captions.FirstOrDefault().confidence;
                    imageCaption = char.ToUpper(imageCaption[0]) + imageCaption.Substring(1);
                    CaptionLabel.Text = (imageCaption);
                    await postLocationAsync(imageCaption, captionConfidence);
                }
                file.Dispose();
            }
        }
    }
}
