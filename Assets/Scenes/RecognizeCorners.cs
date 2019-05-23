using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using ZXing;
using ZXing.QrCode;
using ZXing.Common;


public struct Square
{
    public Vector2 upperLeft;
    public Vector2 upperRight;
    public Vector2 lowerLeft;
    public Vector2 lowerRight;
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
    public void getSquares(out Tuple<ResultPoint[], Matrix4x4> detected_square)
    {
        detected_square = null;
        try
        {
            var barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(camTexture.GetPixels32(),
                camTexture.width, camTexture.height);
            if (result != null)
            {
                detected_square = new Tuple<ResultPoint[], Matrix4x4>(result.ResultPoints, new Matrix4x4());
            }

        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
            detected_square = null;
        }
    }
}
