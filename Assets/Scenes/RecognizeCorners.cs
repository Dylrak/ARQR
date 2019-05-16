using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Square
{
    public Vector2 upperLeft;
    public Vector2 upperRight;
    public Vector2 lowerLeft;
    public Vector2 lowerRight;
}

public class RecognizeCorners : MonoBehaviour
{
    // Obtain a list of all the squares which can be detected in the camera input image
    public void getSquares(out List<Square> detected_squares_list)
    {
        //Placeholder code:
        detected_squares_list = new List<Square>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
