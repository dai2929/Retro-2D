using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    public float moveX = 0.0f;  //X方向の距離
    public float moveY = 0.0f;  //Y方向の距離
    public float times = 0.0f;  //どれくらいの速さで動くか

    Vector3 startPos;   //初期位置の記憶
    Vector3 endPos; //ゴール地点

    public bool isReverse = true; //移動方向の反転フラグ
    public bool isCanMove = true;   //アクション中かどうかのフラグ

    float movep = 0;    //Leapメソッドに使う移動の補完地。開始地点からゴール地点までの現在移動量の割合(0～1の間)

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;  //初期位置の記憶
        endPos = new Vector2(startPos.x + moveX, startPos.y + moveY);//ゴール地点の座標の設定
    }

    // Update is called once per frame
    void Update()
    {
        if (isCanMove)
        {
            float distance = Vector2.Distance(startPos, endPos);    //始点からゴール地点までの距離計算
            float ds = distance / times;    //1秒あたりの移動距離目標
            float df = ds * Time.deltaTime; //その時々の1フレームあたりに進むべき移動量

            //全体の距離(distance)に対して、1フレームに進んだ距離の割合を蓄積
            movep += df / distance;

            if (isReverse)   //もし逆方向の移動フラグがtrueなら(反転)
            {
                transform.position = Vector2.Lerp(endPos, startPos, movep);   //百方向への移動
            }
            else    //逆方向フラグがfalseなら(順転)
            {
                transform.position = Vector2.Lerp(startPos, endPos, movep);   //順方向への移動
            }

            //移動の割合が100%に届いた = ゴールに到着したら
            if(movep >= 1.0f)
            {
                movep = 0.0f;   //移動の割合はリセット
                isReverse = !isReverse;  //逆方向への移動フラグを反転
                isCanMove = false;  //一旦停止

                //Debug.Log
            }
        }
    }

    //止まっているフラグをtrueに戻すメソッド
    public void OnAnimatorMove()
    {
        isCanMove = true;
    }

    //動いているフラグをfalseにして止めるメソッド
    public void OnParticleSystemStopped()
    {
        isCanMove = false;
    }

    //移動のシミュレーションをギズモを使って描画
    private void OnDrawGizmosSelected()
    {
        //描画の初期位置
        Vector2 fromPos;

        //記録した初期地点(0,0,0)だったら
        //ブロックの位置がfromPos
        if(startPos == Vector3.zero)
        {
            fromPos = transform.position;
        }
        else
        {
            fromPos = startPos; //でなければ初期地点が描画上も初期地点
        }

        //移動の線を描画
        Gizmos.DrawLine(fromPos,new Vector2(fromPos.x + moveX, fromPos.y + moveY));
        //移動の先のブロックの形=自分の形そのもの
        Vector2 size = GetComponent<SpriteRenderer>().size;
        //初期地点のブロックの形を描画
        Gizmos.DrawWireCube(fromPos,new Vector2(size.x,size.y));

        //ゴール地点の座標を確認
        Vector2 toPos = new Vector3(fromPos.x + moveX,fromPos.y + moveY);
        //ゴール地点のブロックの形を描画
        Gizmos.DrawWireCube(toPos,new Vector2(size.x,size.y));
    }
}
