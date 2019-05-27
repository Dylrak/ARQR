using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateProjectionMatrix : MonoBehaviour
{
    private GameObject webCamImage;

    private RecognizeCorners recognizeCorners;

    public Tuple<List<IntPoint>, Matrix4x4> detected_square;
    
    void Awake()
    {
        webCamImage = GameObject.Find("WebCamImage");
        recognizeCorners = webCamImage.GetComponent<RecognizeCorners>();
        detected_square = new Tuple<List<IntPoint>, Matrix4x4>(null, new Matrix4x4());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        recognizeCorners.getSquares(out detected_square);
        //Calculating a projection matrix:
        /*  +-           -+   +-       -+   +-       -+
            | image_x * w |   | a  b  c |   | world_x |
            | image_y * w | = | d  e  f | * | world_y |
            |       w     |   | g  h  1 |   |    1    |
            +-           -+   +-       -+   +-       -+ */
        Square ds = detected_square;
        float h = (ds.upperLeft.X * (ds.upperRight.Y - ds.lowerRight.Y) +
            ds.upperLeft.Y * (ds.lowerRight.X - ds.upperRight.X) +
            ds.upperRight.X * ds.lowerLeft.Y - ds.upperRight.Y * ds.lowerLeft.X +
            ds.lowerLeft.X * ds.lowerRight.Y - ds.lowerLeft.Y * ds.lowerRight.X) / 
            (ds.upperRight.X * (ds.lowerRight.Y - ds.lowerLeft.Y) +
            ds.upperRight.Y * (ds.lowerLeft.X - ds.lowerRight.X) - 
            ds.lowerLeft.X * ds.lowerRight.Y + ds.lowerLeft.Y * ds.lowerRight.X);
        float g = (-ds.upperLeft.X - (h + 1) * ds.lowerRight.X + h * ds.lowerLeft.X + 
            ds.lowerLeft.X + ds.upperRight.X) / (ds.lowerRight.X - ds.upperRight.X);
        float a = ds.upperRight.X * (g + 1) - ds.upperLeft.X;
        float d = ds.upperRight.Y * (g + 1) - ds.upperLeft.Y;
        float b = ds.lowerLeft.X * (h + 1) - ds.upperLeft.X;
        float e = ds.lowerLeft.Y * (h + 1) - ds.upperLeft.Y;
        float c = ds.upperLeft.X;
        float f = ds.upperLeft.Y;
        //Now that we have a through h, we need to put it into a Matrix4x4:
        /*+-          -+
            | a  b  c  0 |
            | d  e  f  0 |
            | g  h  1  0 |
            | 0  0  0  1 |
            +-          -+*/
        detected_square.Item2.SetRow(0, new Vector4(a, b, c, 0));
        detected_square.Item2.SetRow(1, new Vector4(d, e, f, 0));
        detected_square.Item2.SetRow(2, new Vector4(g, h, 1, 0));
        detected_square.Item2.SetRow(3, new Vector4(0, 0, 0, 1));
    }
}
