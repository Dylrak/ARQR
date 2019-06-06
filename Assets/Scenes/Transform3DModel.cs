using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

//Transform 'this' according to Matrix4x4 in parameter
public static class TransformExtensions
{
    public static void FromMatrix(this Transform transform, Matrix4x4 matrix)
    {
        transform.localScale = matrix.ExtractScale();
        transform.rotation = matrix.ExtractRotation();
        transform.position = matrix.ExtractPosition();
    }
}

//Extract Scale, Rotation and Position from Matrix4x4
public static class MatrixExtensions
{
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }

    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}

public class Transform3DModel : MonoBehaviour
{
    public GameObject eventSystem;
    
    private CalculateProjectionMatrix calculateProjectionMatrix;

    void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        calculateProjectionMatrix = eventSystem.GetComponent<CalculateProjectionMatrix>();
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // LateUpdate is called once per frame, after all other Update()-calls are finished.
    void LateUpdate()
    {
        TransformExtensions.FromMatrix(transform, calculateProjectionMatrix.detected_square.Item2);
        //Placeholder:
        //transform.Rotate(1f, 1f, 1f);
    }
}
