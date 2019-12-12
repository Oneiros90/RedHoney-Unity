using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace RedHoney.Utilities
{
    using Debug = Log.ContextDebug<Webcam>;

    ///////////////////////////////////////////////////////////////////////////
    public class Webcam : MonoBehaviour
    {
        [Tooltip("The image on which the webcam will render")]
        public RawImage RawImage;


        private Color imageColor;

        ///////////////////////////////////////////////////////////////////////////
        IEnumerator Start()
        {
            // Turning the image black while we wait for the webcam
            imageColor = RawImage.color;
            RawImage.color = Color.black;

            // Check if at least one webcam is present
            if (WebCamTexture.devices.Length > 0)
            {
                WebCamDevice webcam = WebCamTexture.devices[0];

                // Request authorization
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
                if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                {
                    Debug.Log($"Webcam {webcam.name} found and authorized");

                    // Setup the texture
                    WebCamTexture webcamTexture = new WebCamTexture();
                    RawImage.texture = webcamTexture;
                    RawImage.material.mainTexture = webcamTexture;
                    webcamTexture.Play();

                    // Restore original image color
                    RawImage.color = imageColor;
                }
                else
                {
                    Debug.LogError($"Webcam {webcam.name} found but not authorized");
                }
            }
            else
            {
                Debug.LogError("No webcam found");
            }
        }

    }
}