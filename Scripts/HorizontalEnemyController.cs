using System.Collections;
using UnityEngine;

public class HorizontalEnemyController : MonoBehaviour
{
    public float speed = 2f;    //移動速度
    public Transform wallCheck; //壁検知用のポイント
    public float wallCheckDistance = 0.5f;  //壁検知距離
    public LayerMask GroundLayer;   //グラウンド(壁)のレイヤー

    public float pauseDuration = 1f;    //停止時間(秒)
    bool isPaused = false;

    Animator animator;  //アニメーション制御用

    //---------------------------------
    Vector2 initialPosition;    //敵の初期位置を記録
    bool isReturningToPosition = false; //定位置に戻るかどうかのフラグ
    public LayerMask playerLayer;   //プレイヤーを検知するためのレイヤー
    public float detectionRange = 5f;   //プレイヤー検知範囲

    bool isMoving = false;   // プレイヤーを検知した後、移動フラグ

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;   //敵キャラの初期位置を保存
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused || isReturningToPosition) return;   //停止中は処理をスキップ

        // プレイヤーを検知したときの処理
        RaycastHit2D playerHit = Physics2D.Raycast(
            transform.position,
            Vector2.left, detectionRange,
            playerLayer
        );

        if (!isReturningToPosition && playerHit.collider != null)
        {
            // プレイヤーを検知したら移動フラグをtrueにして移動開始
            isMoving = true;
        }

        if (isMoving)
        {
            // 壁を検知
            RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, Vector2.left, wallCheckDistance, GroundLayer);

            if (wallHit.collider != null)
            {
                // 壁に当たったら停止処理
                StartCoroutine(PauseAndReturnToPosition());
                isMoving = false;  // 移動を停止
            }
            else
            {
                // 壁に当たるまで左に移動
                Vector2 direction = Vector2.left;
                transform.Translate(direction * speed * Time.deltaTime);

                // アニメーションを移動状態に設定
                if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                {
                    animator.Play("Move");  // "Move" アニメーションを使用
                }
            }
        }
        else
        {
            // プレイヤーが見当たらないか、壁に当たった場合はアイドル状態に戻す
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.Play("Idle");
            }
        }
    }

    IEnumerator PauseAndReturnToPosition()
    {
        isPaused = true;

        // 壁に当たったアニメーション
        if (animator != null)
        {
            animator.Play("HitAnime");
        }

        // 停止時間の待機
        yield return new WaitForSeconds(pauseDuration);

        // 定位置に戻る処理
        isReturningToPosition = true;
        while ((Vector2)transform.position != initialPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, initialPosition, speed * Time.deltaTime);
            yield return null;
        }

        // 状態をリセット
        isReturningToPosition = false;
        isPaused = false;
    }

    void OnDrawGizmos()
    {
        if (wallCheck == null) return;

        //壁検知用のRayを可視化
        Gizmos.color = Color.red;
        Vector3 direction = Vector3.left;  // 左方向に変更
        Vector3 end = wallCheck.position + direction * wallCheckDistance;

        //線と矢印を描画
        Gizmos.DrawLine(wallCheck.position, end);
        Gizmos.DrawLine(end, end - direction * 0.2f + Vector3.up * 0.1f);
        Gizmos.DrawLine(end, end - direction * 0.2f - Vector3.up * 0.1f);

        // プレイヤー検知用のRayを可視化
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * detectionRange);
    }
}