using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;

//Struct which contains four corner pixel points, creating a quarternion

public struct Quarternion
{
    public Vector2 upperLeft;
    public Vector2 upperRight;
    public Vector2 lowerLeft;
    public Vector2 lowerRight;

    public Quarternion(Vector2 upLeft, Vector2 upRight, Vector2 lowLeft, Vector2 lowRight)
    {
        upperLeft = upLeft;
        upperRight = upRight;
        lowerLeft = lowLeft;
        lowerRight = lowRight;
    }
    public static implicit operator Quarternion(Tuple<Quarternion, Matrix4x4> quarternion_points)
    {
        return quarternion_points.Item1;
    }

    //Convert list of list of points to a singular Quarternion
    public static implicit operator Quarternion(Point[][] contours)
    {
        return new Quarternion(new Vector2(contours[0][0].X, contours[0][0].Y),
            new Vector2(contours[0][1].X, contours[0][1].Y),
            new Vector2(contours[0][2].X, contours[0][2].Y),
            new Vector2(contours[0][3].X, contours[0][3].Y));
    }
}

// class for video display and processed video display
// current process is canny edge detection
public class RecognizeCorners : MonoBehaviour
{

    // Video parameters
    public RawImage WebCamTextureRenderer;
    public RawImage ProcessedTextureRenderer;
    public int deviceNumber;
    private WebCamTexture _webcamTexture;

    // Video size
    private const int imWidth = 1280;
    private const int imHeight = 720;
    private int imFrameRate;

    // OpenCVSharp parameters
    private Mat videoSourceImage;
    private Mat cannyImage;
    private Texture2D processedTexture;
    private Vec3b[] videoSourceImageData;
    private byte[] cannyImageData;
    private GeneralizedHough Gh;
    OutputArray positions;

    // Frame rate parameter
    private int updateFrameCount = 0;
    private int textureCount = 0;
    private int displayCount = 0;

    void Start() {

        // create a list of webcam devices that is available
        WebCamDevice[] devices = WebCamTexture.devices;
        
        if (devices.Length > 0) { 
            Debug.Log("Device found!");
            // initialized the webcam texture by the specific device number
            _webcamTexture = new WebCamTexture(devices[deviceNumber].name, imWidth, imHeight);
            // assign webcam texture to the meshrenderer for display
            WebCamTextureRenderer = GameObject.Find("WebCamImage").GetComponent<RawImage>();
            // WebCamTextureRenderer.material.mainTexture = _webcamTexture;
            WebCamTextureRenderer.texture = _webcamTexture;

            // Play the video source
            _webcamTexture.Play();

            // initialize video / image with given size
            videoSourceImage = new Mat(imHeight, imWidth, MatType.CV_8UC3);
            videoSourceImageData = new Vec3b[imHeight * imWidth];
            cannyImage = new Mat(imHeight, imWidth, MatType.CV_8UC1);
            cannyImageData = new byte[imHeight * imWidth];

            // create processed video texture as Texture2D object
            processedTexture = new Texture2D(imWidth, imHeight, TextureFormat.RGBA32, true, true);

            // assign the processedTexture to the meshrenderer for display
            ProcessedTextureRenderer.texture = processedTexture;

            // Assign a HoughlinesP to GeneralizedHough
            Gh = GeneralizedHoughBallard.Create();
        }

        // create opencv window to display the original video
        // Cv2.NamedWindow("Copy video");
        

    }


    
    void Update() {

        updateFrameCount++;

        if (_webcamTexture.isPlaying) {

            if (_webcamTexture.didUpdateThisFrame) {

                textureCount++;

                // convert texture of original video to OpenCVSharp Mat object
                TextureToMat();
                // update the opencv window of source video
                // UpdateWindow(videoSourceImage);
                // create the canny edge image out of source image
                ProcessImage(videoSourceImage);
                // convert the OpenCVSharp Mat of canny image to Texture2D
                // the texture will be displayed automatically
                MatToTexture();

            }

        }
        else {
            Debug.Log("Can't find camera!");
        }


        // output frame rate information
        if (updateFrameCount % 30 == 0) {
            Debug.Log("Frame count: " + updateFrameCount + ", Texture count: " + textureCount + ", Display count: " + displayCount);
        }


    }


    // Convert Unity Texture2D object to OpenCVSharp Mat object
    void TextureToMat() {
        // Color32 array : r, g, b, a
        Color32[] c = _webcamTexture.GetPixels32();
        for (var i = 0; i < imHeight; i++) {
            for (var j = 0; j < imWidth; j++) {
                var col = c[j + i * imWidth];
                var vec3 = new Vec3b {
                    Item0 = col.b,
                    Item1 = col.g,
                    Item2 = col.r
                };
                // set pixel to an array
                videoSourceImageData[j + i * imWidth] = vec3;
            }
        }
        // assign the Vec3b array to Mat
        videoSourceImage.SetArray(0, 0, videoSourceImageData);
    }



    // Convert OpenCVSharp Mat object to Unity Texture2D object
    void MatToTexture() {
        // cannyImageData is byte array, because canny image is grayscale
        cannyImage.GetArray(0, 0, cannyImageData);
        // create Color32 array that can be assigned to Texture2D directly
        Color32[] c = new Color32[imHeight * imWidth];

        for(var i = 0; i < imHeight; i++) {
            for (var j = 0; j < imWidth; j++) {
                byte vec = cannyImageData[j + i * imWidth];
                var color32 = new Color32 {
                    r = vec,
                    g = vec,
                    b = vec,
                    a = 0
                };
                c[j + i * imWidth] = color32;
            }
        }

        processedTexture.SetPixels32(c);
        // to update the texture, OpenGL manner
        processedTexture.Apply();
    }



    // Simple example of canny edge detect
    OutputArray ProcessImage(Mat _image) {
        Cv2.Flip(_image, _image, FlipMode.X);
        Cv2.Canny(_image, cannyImage, 100, 100);
        Gh.Detect(cannyImage, positions);
        Debug.Log(positions);
        return positions;
    }


    // Display the original video in a opencv window
    void UpdateWindow(Mat _image) {
        Cv2.Flip(_image, _image, FlipMode.X);
        Cv2.ImShow("Copy video", _image);
        displayCount++;
    }
    
    // close the opencv window
    public void OnDestroy() {
        Cv2.DestroyAllWindows();
    }

    
}