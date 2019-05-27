using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using AForge;

public struct Square
{
    public IntPoint upperLeft;
    public IntPoint upperRight;
    public IntPoint lowerLeft;
    public IntPoint lowerRight;

    public Square(IntPoint upLeft, IntPoint upRight, IntPoint lowLeft, IntPoint lowRight)
    {
        upperLeft = upLeft;
        upperRight = upRight;
        lowerLeft = lowLeft;
        lowerRight = lowRight;
    }
    public static implicit operator Square(Tuple<List<IntPoint>, Matrix4x4> square_points)
    {
        return new Square(square_points.Item1[0], square_points.Item1[1], square_points.Item1[2], square_points.Item1[3]);
    }
}

public class RecognizeCorners : MonoBehaviour
{
    private WebCamTexture camTexture;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        if (camTexture != null)
        {
            camTexture.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Obtain a list of all the squares which can be detected in the camera input image
    public void getSquares(out Tuple<List<IntPoint>, Matrix4x4> detected_square)
    {
        Color[] webcam_image = camTexture.GetPixels();
        QuadrilateralFinder qf = new QuadrilateralFinder();
        List<IntPoint> corners = qf.ProcessImage(webcam_image);
        detected_square = null;
        //try
        //{
        //    var barcodeReader = new BarcodeReader();
        //    var result = barcodeReader.Decode(camTexture.GetPixels32(),
        //        camTexture.width, camTexture.height);
        //    if (result != null)
        //    {
        //        detected_square = new Tuple<List<IntPoint>, Matrix4x4>(result.ResultPoints, new Matrix4x4());
        //    }

        //}
        //catch (Exception ex)
        //{
        //    Debug.LogWarning(ex.Message);
        //    detected_square = null;
        //}
    }
}
