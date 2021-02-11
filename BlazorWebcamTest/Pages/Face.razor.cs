using Microsoft.AspNetCore.Components;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using WebcamComponent;
using static LanguageExt.Prelude;

namespace BlazorWebcamTest.Pages
{
    public partial class Face
    {
        // Dependencies
        [Inject] AzureSettings Settings { get; set; }

        // Component References
        TelerikNotification ErrorNotification { get; set; }
        Webcam Camera { get; set; }

        // State
        bool windowVisible;
        bool devMode = true;
        readonly List<(string Data, DetectedFace DetectedFace)> faces = new();
        int imageIndex;

        async Task StartAnalysis()
        {
            var shuffledImageIndicies = Enumerable.Range(1, 8).Suffle();
            windowVisible = true;
            foreach (var i in shuffledImageIndicies)
            {
                imageIndex = i;
                StateHasChanged();
                await Task.Delay(160); // wait for reaction
                await GetSnap();
                StateHasChanged();
                await Task.Delay(100); // rest period
            }
            windowVisible = false;
        }
        async Task GetSnap()
        {
            var data = await Camera.GetSnapshot();
            await TryUploadAndDetectFaces(data);
        }
        private async Task TryUploadAndDetectFaces(string base64encodedstring)
        {
            await TryAsync(() => UploadAndDetectFaces(base64encodedstring))
                .Match(
                    Succ: (face) => faces.Add((base64encodedstring, face)),
                    Fail: (e) => ErrorNotification.Show(new()
                    {
                        Closable = true,
                        Text = e.Message,
                        ThemeColor = Telerik.Blazor.ThemeColors.Error,
                        Icon = "exclamation-circle",
                        ShowIcon = true,
                        CloseAfter = 0
                    })
                );
        }

        private async Task<DetectedFace> UploadAndDetectFaces(string base64encodedstring)
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
            // data:[<mediatype>][;base64],<data>
            // Using split we can remove the data header and get the bytes
            var bytes = Convert.FromBase64String(base64encodedstring.Split(',')[1]);
            var imageFileStream = new MemoryStream(bytes);
            
            // Get a list of detected faces from Azure
            IList<DetectedFace> faceList = 
                    await faceClient.Face.DetectWithStreamAsync(
                        imageFileStream, true, false, faceAttributes);

            // Return the first face found
            return faceList[0];
        }



    }
}

