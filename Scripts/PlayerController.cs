using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 移動速度
    public float jumpForce = 5f;   //ジャンプ力
    public float downForce = 1f;    //下入力 足場床用

    public Rigidbody2D rb;  //Rigidbody2D型の変数

    public bool isGrounded; //接地判定
    private Vector2 movement;   //移動方向

    public Transform groundCheck;   //接地判定用オブジェクト
    public float groundCheckRadius = 0.2f;   //接地判定用の円の半径
    public LayerMask groundLayer;   //接地判定対象のレイヤー

    //アニメーション
    Animator animator;  //アニメーターを使用可能に
    public string idleAnime = "Idle";
    public string runAnime = "Run";
    public string jumpAnime = "Jump";
    public string goalAnime = "Goal";
    public string hitAnime = "Hit";

    string nowAnime = "";
    string oldAnime = "";

    public static string gameState = "playing"; //ゲームの状態

    //足場床用変数
    public LayerMask oneWayTile;    //足場床判定対象のレイヤー 足場床用
    public bool isOneWayTile;   //OneWayTile 足場床用
    public float disableCollisionTime = 0.5f; // 衝突無効化時間（秒）

    void Awake()
    {
        // Rigidbody2Dを自動取得
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //アニメ関連
        animator = GetComponent<Animator>();
        nowAnime = idleAnime;
        oldAnime = idleAnime;

        gameState = "playing";  //ゲーム中にする
    }

    void Update()
    {
        if (gameState != "playing")
        {
            return;
        }

        // 入力を取得
        movement.x = Input.GetAxisRaw("Horizontal"); // 左右移動 (A/Dキー、矢印キー)

        //Sが押された場合かつOneWayTileに乗ってる場合はDownメソッドを発動
        if (Input.GetKeyDown(KeyCode.S) && isOneWayTile)
        {
            Down();
        }

        //ジャンプ入力を取得
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        //ゴールやゲームオーバー時には処理を停止
        if (gameState != "playing")
            return;

        //接地判定を更新
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        isOneWayTile = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, oneWayTile);

        if (isGrounded)
        {
            Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            if (groundCollider != null)
            {
                // 接地しているオブジェクトのレイヤー名を取得
                string layerName = LayerMask.LayerToName(groundCollider.gameObject.layer);
                Debug.Log($"Grounded on layer: {layerName}");
            }
        }
        else
        {
            Debug.Log("Not grounded.");
        }

        //空中での慣性をつける
        //地面の上or速度が0ではない場合は速度を更新
        if (isGrounded || movement.x != 0)
        {
            //移動処理
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }

        //Debug.Log($"Is Grounded:{isGrounded}");     //デバッグ 接地判定

        //アニメーション更新
        if (isGrounded)
        {
            //地面の上
            if (movement.x == 0)
            {
                nowAnime = idleAnime;    //停止中のアニメ
            }
            else
            {
                nowAnime = runAnime;    //移動中のアニメ
            }
        }
        else
        {
            //空中
            nowAnime = jumpAnime;   //ジャンプアニメ
        }
        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);    //アニメーション再生
        }

        //左右移動適正なアニメーションにする
        if (movement.x > 0)
        {
            //右移動
            transform.localScale = new Vector2(1, 1);   //絵を右向き
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);   //絵を左向き
        }
    }

    //ジャンプメソッド
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }


    //------------------------------足場床-----------------------------
    //足場床通過メソッド(下(S)を押すと下に移動)　★GPTより
    void Down()
    {
        if (isOneWayTile)
        {
            // 足場床のコライダーを一時的に無効化
            Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, oneWayTile);
            if (groundCollider != null)
            {
                StartCoroutine(DisableOneWayTileCollision(groundCollider));
            }
        }
    }

    // 足場床との衝突を一時的に無効化するコルーチン
    IEnumerator DisableOneWayTileCollision(Collider2D oneWayTileCollider)
    {
        // プレイヤー自身のコライダーを取得
        Collider2D playerCollider = GetComponent<Collider2D>();

        // 足場床との衝突を無効化
        Physics2D.IgnoreCollision(playerCollider, oneWayTileCollider, true);

        // 少しの間だけ衝突を無効にする 秒数をpublic変数で調節可能に
        yield return new WaitForSeconds(disableCollisionTime);

        // 足場床との衝突を再び有効化
        Physics2D.IgnoreCollision(playerCollider, oneWayTileCollider, false);
    }

    //------------------------------足場床-----------------------------↑

    //ゴールかDeadに接触した場合の処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal(); //ゴール
        }
        else if (collision.gameObject.tag == "Dead")
        {
            //衝突した相手の方向を計算 TopViewGameを応用 ★GPTより
            Vector3 v = (transform.position - collision.transform.position).normalized;

            //GameOverメソッドを呼び出し、方向を引数として渡す
            GameOver(v);
        }
    }

    //ゴールメソッド
    public void Goal()
    {
        animator.Play(goalAnime);

        gameState = "gameclear";
        GameStop(); //ゲーム停止

        //上に跳ね上げる(その後消えてクリアにしたい) ★GPTより
        rb.AddForce(new Vector2(0, 10), ForceMode2D.Impulse);

        //StartCoroutinを使ってアニメーション終了後の処理を実行 ★GPTより
        StartCoroutine(GoalAnimationEnd());
    }

    //ゴールアニメーション終了後にキャラクターを消すメソッド ★GPTより
    IEnumerator GoalAnimationEnd()
    {
        //現在のアニメーションが終了するのを待つ
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f || animator.GetCurrentAnimatorStateInfo(0).IsName(goalAnime) == false)
        {
            yield return null;  //1フレーム待機
        }

        //キャラクターを消す(非アクティブ化)
        gameObject.SetActive(false);
    }

    //ゲームオーバー
    public void GameOver(Vector3 hitDirection)
    {
        animator.Play(hitAnime);

        gameState = "gameover";
        GameStop();

        //カメラを揺らす ★GPTより
        Camera.main.GetComponent<CameraShake>().ShakeCamera();

        //Rigidbody2DのfreezeRotationを解除
        rb.freezeRotation = false;

        //ゲームオーバー演出
        //プレイヤーの当たり判定を消す
        GetComponent<CapsuleCollider2D>().enabled = false;

        //引数で渡された方向にヒットバックさせる ★GPTより
        rb.AddForce(new Vector2(hitDirection.x * 4, hitDirection.y * 4), ForceMode2D.Impulse);

    }

    //ゲーム停止
    void GameStop()
    {
        rb.velocity = new Vector2(0, 0);
    }

    private void OnDrawGizmosSelected()
    {
        //接地判定の確認用Gizmoを描画
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}