using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount = 0.1f;    //揺れの大きさ
    public float shakeDuration = 0.5f;  //揺れの時間

    Vector3 originalPosition;
    float currentShakeDuration;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentShakeDuration > 0)
        {
            //ランダムな位置にカメラを移動
            transform.position = originalPosition + new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0);
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            //揺れが終了したらカメラを元の位置に戻す
            transform.position = originalPosition;
        }
    }

    public void ShakeCamera()
    {
        currentShakeDuration = shakeDuration;
    }
}
