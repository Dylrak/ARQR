using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZXing;

public class CalculateProjectionMatrix : MonoBehaviour
{
    private GameObject webCamImage;

    private RecognizeCorners recognizeCorners;

    public Tuple<ResultPoint[], Matrix4x4> detected_square;
    
    void Awake()
    {
        webCamImage = GameObject.Find("WebCamImage");
        recognizeCorners = webCamImage.GetComponent<RecognizeCorners>();
        detected_square = new Tuple<ResultPoint[], Matrix4x4>(null, new Matrix4x4());
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
        float h = (smp.Key.upperLeft.x * (smp.Key.upperRight.y - smp.Key.lowerRight.y) +
            smp.Key.upperLeft.y * (smp.Key.lowerRight.x - smp.Key.upperRight.x) +
            smp.Key.upperRight.x * smp.Key.lowerLeft.y - smp.Key.upperRight.y * smp.Key.lowerLeft.x +
            smp.Key.lowerLeft.x * smp.Key.lowerRight.y - smp.Key.lowerLeft.y * smp.Key.lowerRight.x) / 
            (smp.Key.upperRight.x * (smp.Key.lowerRight.y - smp.Key.lowerLeft.y) +
            smp.Key.upperRight.y * (smp.Key.lowerLeft.x - smp.Key.lowerRight.x) - 
            smp.Key.lowerLeft.x * smp.Key.lowerRight.y + smp.Key.lowerLeft.y * smp.Key.lowerRight.x);
        float g = (-smp.Key.upperLeft.x - (h + 1) * smp.Key.lowerRight.x + h * smp.Key.lowerLeft.x + 
            smp.Key.lowerLeft.x + smp.Key.upperRight.x) / (smp.Key.lowerRight.x - smp.Key.upperRight.x);
        float a = smp.Key.upperRight.x * (g + 1) - smp.Key.upperLeft.x;
        float d = smp.Key.upperRight.y * (g + 1) - smp.Key.upperLeft.y;
        float b = smp.Key.lowerLeft.x * (h + 1) - smp.Key.upperLeft.x;
        float e = smp.Key.lowerLeft.y * (h + 1) - smp.Key.upperLeft.y;
        float c = smp.Key.upperLeft.x;
        float f = smp.Key.upperLeft.y;
        //Now that we have a through h, we need to put it into a Matrix4x4:
        /*+-          -+
            | a  b  c  0 |
            | d  e  f  0 |
            | g  h  1  0 |
            | 0  0  0  1 |
            +-          -+*/
        smp.Value.SetRow(0, new Vector4(a, b, c, 0));
        smp.Value.SetRow(1, new Vector4(d, e, f, 0));
        smp.Value.SetRow(2, new Vector4(g, h, 1, 0));
        smp.Value.SetRow(3, new Vector4(0, 0, 0, 1));
    }
}
