using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateProjectionMatrix : MonoBehaviour
{
    private GameObject webCamImage;

    private RecognizeCorners recognizeCorners;

    public Tuple<Square, Matrix4x4> detected_square;
    
    void Awake()
    {
        webCamImage = GameObject.Find("WebCamImage");
        recognizeCorners = webCamImage.GetComponent<RecognizeCorners>();
        detected_square = new Tuple<Square, Matrix4x4>(new Square(), new Matrix4x4());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        recognizeCorners.getSquares(detected_square);
        //Calculating a projection matrix:
        /*  +-           -+   +-       -+   +-       -+
            | image_x * w |   | a  b  c |   | world_x |
            | image_y * w | = | d  e  f | * | world_y |
            |       w     |   | g  h  1 |   |    1    |
            +-           -+   +-       -+   +-       -+ */
        Square ds = detected_square;
        float h = (ds.upperLeft.x * (ds.upperRight.y - ds.lowerRight.y) +
            ds.upperLeft.y * (ds.lowerRight.x - ds.upperRight.x) +
            ds.upperRight.x * ds.lowerLeft.y - ds.upperRight.y * ds.lowerLeft.x +
            ds.lowerLeft.x * ds.lowerRight.y - ds.lowerLeft.y * ds.lowerRight.x) / 
            (ds.upperRight.x * (ds.lowerRight.y - ds.lowerLeft.y) +
            ds.upperRight.y * (ds.lowerLeft.x - ds.lowerRight.x) - 
            ds.lowerLeft.x * ds.lowerRight.y + ds.lowerLeft.y * ds.lowerRight.x);
        float g = (-ds.upperLeft.x - (h + 1) * ds.lowerRight.x + h * ds.lowerLeft.x + 
            ds.lowerLeft.x + ds.upperRight.x) / (ds.lowerRight.x - ds.upperRight.x);
        float a = ds.upperRight.x * (g + 1) - ds.upperLeft.x;
        float d = ds.upperRight.y * (g + 1) - ds.upperLeft.y;
        float b = ds.lowerLeft.x * (h + 1) - ds.upperLeft.x;
        float e = ds.lowerLeft.y * (h + 1) - ds.upperLeft.y;
        float c = ds.upperLeft.x;
        float f = ds.upperLeft.y;
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
