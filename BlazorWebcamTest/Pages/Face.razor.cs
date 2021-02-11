using Microsoft.AspNetCore.Components;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using WebcamComponent;

namespace BlazorWebcamTest.Pages
{
    public partial class Face
    {
        [Inject] AzureSettings Settings { get; set; }
        TelerikNotification ErrorNotification { get; set; }
        bool WindowVisible { get; set; }
        bool DevMode = true;
        Webcam Camera { get; set; }
        List<(string Data, DetectedFace DetectedFace)> faces = new();

        int index;

        async Task GetSnap()
        {
            var data = await Camera.GetSnapshot();
            var face = await UploadAndDetectFaces(data);
            faces.Add(face);
        }

        private async Task<(string, DetectedFace)> UploadAndDetectFaces(string base64encodedstring)
        {
            IFaceClient faceClient = new FaceClient(new ApiKeyServiceClientCredentials(Settings.Key)) { Endpoint = Settings.Uri };

            // The list of Face attributes to return.
            IList<FaceAttributeType?> faceAttributes =
                new FaceAttributeType?[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.Hair,
                };

            // Call the Face API.
            try
            {
                // data:[<mediatype>][;base64],<data>
                // Using split we can remove the data header and get the bytes
                var bytes = Convert.FromBase64String(base64encodedstring.Split(',')[1]);
                var imageFileStream = new MemoryStream(bytes);
                // Whatever else needs to be done here.
                IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes);
                return (base64encodedstring, faceList[0]);
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                ErrorNotification.Show(new()
                {
                    Closable = true,
                    Text = f.Message,
                    ThemeColor = Telerik.Blazor.ThemeColors.Error,
                    Icon = "exclamation-circle",
                    ShowIcon = true,
                    CloseAfter = 0
                });
                return ("", new DetectedFace());
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                ErrorNotification.Show(new()
                {
                    Closable = true,
                    Text = e.Message,
                    ThemeColor = Telerik.Blazor.ThemeColors.Error,
                    Icon = "exclamation-circle",
                    ShowIcon = true,
                    CloseAfter = 0
                });
                return ("", new DetectedFace());
            }
        }

        async Task StartTimer()
        {
            WindowVisible = true;
            for (int i = 0; i < 8; i++)
            {
                index = i;
                StateHasChanged();
                await Task.Delay(160);
                await GetSnap();
                StateHasChanged();
                await Task.Delay(100);
            }
            WindowVisible = false;
        }

    }
}

