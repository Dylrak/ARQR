using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

using OpenCvSharp;

public struct Square
{
    public Vector2 upperLeft;
    public Vector2 upperRight;
    public Vector2 lowerLeft;
    public Vector2 lowerRight;

    public Square(Vector2 upLeft, Vector2 upRight, Vector2 lowLeft, Vector2 lowRight)
    {
        upperLeft = upLeft;
        upperRight = upRight;
        lowerLeft = lowLeft;
        lowerRight = lowRight;
    }
    public static implicit operator Square(Tuple<Square, Matrix4x4> square_points)
    {
        return square_points.Item1;
    }

    //Convert list of list of points to a singular square
    public static implicit operator Square(Point[][] contours)
    {
        return new Square(new Vector2(contours[0][0].X, contours[0][0].Y),
            new Vector2(contours[0][1].X, contours[0][1].Y),
            new Vector2(contours[0][2].X, contours[0][2].Y),
            new Vector2(contours[0][3].X, contours[0][3].Y));
    }
}

public class RecognizeCorners : MonoBehaviour
{
    public RawImage rawImage;

    private WebCamTexture camTexture;

    private int imWidth = Screen.width;
    private int imHeight = Screen.height;

    // OpenCVSharp parameters
    private Mat videoSourceImage;
    private Mat cannyImage;
    private Texture2D processedTexture;
    private Vec3b[] videoSourceImageData;
    private byte[] cannyImageData;
    private Scalar contourColor = new Scalar(0, 255, 0);

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = imHeight;
        camTexture.requestedWidth = imWidth;
        rawImage = GameObject.Find("WebCamImage").GetComponent<RawImage>();
        rawImage.material.mainTexture = camTexture;
        if (camTexture != null)
        {
            camTexture.Play();
        }
        // initialize video / image with given size
        videoSourceImage = new Mat(imHeight, imWidth, MatType.CV_8UC3);
        videoSourceImageData = new Vec3b[imHeight * imWidth];
        cannyImage = new Mat(imHeight, imWidth, MatType.CV_8UC1);
        cannyImageData = new byte[imHeight * imWidth];

        processedTexture = new Texture2D(imWidth, imHeight, TextureFormat.RGBA32, true, true);
    }

    // Update is called once per frame
    void Update()
    {
    }
    // Obtain a list of all the squares which can be detected in the camera input image
    public void getSquares(Tuple<Square, Matrix4x4> detected_square)
    {
        if (camTexture.isPlaying && camTexture.didUpdateThisFrame)
        {
            // convert texture of original video to OpenCVSharp Mat object
            TextureToMat();
            Square ds = ProcessImage(videoSourceImage);
            detected_square = new Tuple<Square, Matrix4x4>(ds, new Matrix4x4());
            MatToTexture();
        } else
        {
            detected_square = null;
        }
    }

    // Convert Unity Texture2D object to OpenCVSharp Mat object
    void TextureToMat()
    {
        // Color32 array : r, g, b, a
        Color32[] c = camTexture.GetPixels32();
        
        // convert Color32 object to Vec3b object
        // Vec3b is the representation of pixel for Mat
        for (var i = 0; i < imHeight; i++) {
            for (var j = 0; j < imWidth; j++)
            {
                var col = c[j + i * imWidth];
                var vec3 = new Vec3b
                {
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
    void MatToTexture()
    {
        // cannyImageData is byte array, because canny image is grayscale
        videoSourceImage.GetArray(0, 0, cannyImageData);
        // create Color32 array that can be assigned to Texture2D directly
        Color32[] c = new Color32[imHeight * imWidth];

        for (var i = 0; i < imHeight; i++) {
            for (var j = 0; j < imWidth; j++)
            {
                byte vec = cannyImageData[j + i * imWidth];
                var color32 = new Color32
                {
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

    Point[][] ProcessImage(Mat _image)
    {
        Point[][] contours;
        HierarchyIndex[] hierarchy;
        Cv2.Flip(_image, _image, FlipMode.X);
        Cv2.Canny(_image, cannyImage, 100, 100);
        Cv2.FindContours(cannyImage, out contours, out hierarchy, OpenCvSharp.RetrievalModes.List, OpenCvSharp.ContourApproximationModes.ApproxSimple);
        Cv2.DrawContours(cannyImage, contours, -1, contourColor);
        return contours;
    }
}
