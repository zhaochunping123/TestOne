using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeRotate : MonoBehaviour
{
    public GameObject Cube;
    public Camera mainCamera;
    public RawImage TargetImg;

    public void Start()
    {
        Debug.Log("zzzz Cube Pos=  "+Cube.transform.position);
    }
    public void SmallFun()
    {
        Vector3 scale = Cube.transform.localScale;
        Cube.transform.localScale = scale * 0.9f;
    }
    public void BigFun()
    {
        Vector3 scale = Cube.transform.localScale;
        Cube.transform.localScale = scale * 1.1f;
    }

    public void RotateFun()
    {
        Cube.transform.Rotate(Vector3.up,20f, Space.Self);
    }

    public void ScreenCapture()
    {
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16);
        mainCamera.targetTexture = rt;
        mainCamera.Render();

        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        tex.Apply();
        TargetImg.texture = tex;

        mainCamera.targetTexture = null;
    }
}
