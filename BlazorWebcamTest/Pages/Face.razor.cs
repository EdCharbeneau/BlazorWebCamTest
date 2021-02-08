using BlazorWebcamTest.Components.Camera;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorWebcamTest.Pages
{
    public partial class Face
    {
        Webcam Camera { get; set; }
        string data;
        IList<DetectedFace> faces;
        string error;

        async Task GetSnap()
        {
            data = await Camera.GetSnapshot();
            faces = await UploadAndDetectFaces(data);
        }
        private readonly IFaceClient faceClient = new FaceClient(new ApiKeyServiceClientCredentials("7b29cd53c8f04812a018d2bbe7d1a182")) { Endpoint = "https://inyoface.cognitiveservices.azure.com/" };

        // The list of descriptions for the detected faces.
        private string[] faceDescriptions;

        // The resize factor for the displayed image.
        private double resizeFactor;

        // Uploads the image file and calls DetectWithStreamAsync.
        private async Task<IList<DetectedFace>> UploadAndDetectFaces(string base64encodedstring)
        {
            // The list of Face attributes to return.
            IList<FaceAttributeType?> faceAttributes =
                new FaceAttributeType?[]
                {
            FaceAttributeType.Gender, FaceAttributeType.Age,
            FaceAttributeType.Smile, FaceAttributeType.Emotion,
            FaceAttributeType.Glasses, FaceAttributeType.Hair
                };

            // Call the Face API.
            try
            {
                var bytes = Convert.FromBase64String(base64encodedstring.Split(',')[1]);
                var imageFileStream = new MemoryStream(bytes);
                // Whatever else needs to be done here.
                IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes);
                return faceList;
            }
            // Catch and display Face API errors.
            catch (APIErrorException f)
            {
                //MessageBox.Show(f.Message);
                return new List<DetectedFace>();
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                error = e.Message;
                //MessageBox.Show(e.Message, "Error");
                return new List<DetectedFace>();
            }
        }
    }
}

