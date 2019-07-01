using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateProjectionMatrix : MonoBehaviour
{
    private GameObject webCamImage;

    private RecognizeCorners recognizeCorners;

    public Tuple<Rectangle, Matrix4x4> detected_rectangle;
    
    void Awake()
    {
        webCamImage = GameObject.Find("WebCamImage");
        recognizeCorners = webCamImage.GetComponent<RecognizeCorners>();
        detected_rectangle = new Tuple<Rectangle, Matrix4x4>(new Rectangle(), new Matrix4x4());
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //recognizeCorners.getRectangle(detected_rectangle);
        //Calculating a projection matrix:
        /*  +-           -+   +-       -+   +-       -+
            | image_x * w |   | a  b  c |   | world_x |
            | image_y * w | = | d  e  f | * | world_y |
            |       w     |   | g  h  1 |   |    1    |
            +-           -+   +-       -+   +-       -+ */
        // Rectangle dr = detected_rectangle;
        // float h = (dr.upperLeft.x * (dr.upperRight.y - dr.lowerRight.y) +
        //     dr.upperLeft.y * (dr.lowerRight.x - dr.upperRight.x) +
        //     dr.upperRight.x * dr.lowerLeft.y - dr.upperRight.y * dr.lowerLeft.x +
        //     dr.lowerLeft.x * dr.lowerRight.y - dr.lowerLeft.y * dr.lowerRight.x) / 
        //     (dr.upperRight.x * (dr.lowerRight.y - dr.lowerLeft.y) +
        //     dr.upperRight.y * (dr.lowerLeft.x - dr.lowerRight.x) - 
        //     dr.lowerLeft.x * dr.lowerRight.y + dr.lowerLeft.y * dr.lowerRight.x);
        // float g = (-dr.upperLeft.x - (h + 1) * dr.lowerRight.x + h * dr.lowerLeft.x + 
        //     dr.lowerLeft.x + dr.upperRight.x) / (dr.lowerRight.x - dr.upperRight.x);
        // float a = dr.upperRight.x * (g + 1) - dr.upperLeft.x;
        // float d = dr.upperRight.y * (g + 1) - dr.upperLeft.y;
        // float b = dr.lowerLeft.x * (h + 1) - dr.upperLeft.x;
        // float e = dr.lowerLeft.y * (h + 1) - dr.upperLeft.y;
        // float c = dr.upperLeft.x;
        // float f = dr.upperLeft.y;
        //Now that we have a through h, we need to put it into a Matrix4x4:
        /*+-          -+
            | a  b  c  0 |
            | d  e  f  0 |
            | g  h  1  0 |
            | 0  0  0  1 |
            +-          -+*/
        // detected_rectangle.Item2.SetRow(0, new Vector4(a, b, c, 0));
        // detected_rectangle.Item2.SetRow(1, new Vector4(d, e, f, 0));
        // detected_rectangle.Item2.SetRow(2, new Vector4(g, h, 1, 0));
        // detected_rectangle.Item2.SetRow(3, new Vector4(0, 0, 0, 1));
    }
}
