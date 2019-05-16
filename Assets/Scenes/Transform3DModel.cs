using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transform3DModel : MonoBehaviour
{
    public GameObject webCamImage;
    public GameObject eventSystem;

    private RecognizeCorners recognizeCorners;
    private CalculateProjectionMatrix calculateProjectionMatrix;

    public List<Square> detected_squares_list;

    void Awake()
    {
        webCamImage = GameObject.Find("WebCamImage");
        eventSystem = GameObject.Find("EventSystem");
        recognizeCorners = webCamImage.GetComponent<RecognizeCorners>();
        calculateProjectionMatrix = eventSystem.GetComponent<CalculateProjectionMatrix>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(1f, 1f, 1f);
        recognizeCorners.getSquares(out detected_squares_list);
        foreach (Square square in detected_squares_list)
        {

        }
    }
}
