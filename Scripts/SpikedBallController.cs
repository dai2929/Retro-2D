using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedBallController : MonoBehaviour
{
    public float rotationSpeed = 100f;  //回転速度(度/秒)
    public bool isClockwise = true; //回転方向(true時計回り,false反時計回り)
    public Transform ball;  //鉄球のTransform
    public int gizmoSegments = 100; //動線の分割数

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //回転方向を設定
        float direction = isClockwise ? 1f : -1f;

        //Anchorを中心に回転
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }

}
