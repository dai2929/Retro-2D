using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   //UI使うのに必要
using TMPro;    

public class GameManager : MonoBehaviour
{
    public GameObject mainImage;    //イラスト文字を持つGameObject

    //画像ではなく作成済みのテキストUIオブジェクトを使う場合は
    //GameObject ◯◯Text;になるのもある

    //この場合本来クリアとオーバー両方にあるmainImageを入れ替える事で解決できそう
    public GameObject clearImage;
    public GameObject overImage; 

    public GameObject panel;    //パネル
    public GameObject restartButton;    //ReStartボタン
    public GameObject nextButton;   //Nextボタン

    // Start is called before the first frame update
    void Start()
    {
        //画像を1秒後に非表示にする
        Invoke("InactiveImage", 1.0f);
        //ボタン(パネル)を非表示にする
        panel.SetActive(false);

        //クリアとオーバーを非表示 できなかった
        //StatusImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerController.gameState == "gameclear")
        {
            //ゲームクリア
            clearImage.SetActive(true);  //GAME CLEARオブジェクト
            panel.SetActive(true);  //パネル(ボタン)を表示

            //これで両方表示されるのを防ぐ
            overImage.SetActive(false );    //オーバーは表示しない

            //ReStartボタンの無効化
            Button bt = restartButton.GetComponent<Button>();
            bt.interactable = false;    //Buttonコンポーネントのボタンが有効化の変数をfalse

            PlayerController.gameState = "gameend";    //何回もこの一連の処理を繰り返さないようにするため
        }
        else if(PlayerController.gameState == "gameover")
        {
            //ゲームクリア
            overImage.SetActive(true) ; //GAME OVERオブジェクト
            panel.SetActive(true) ; //パネル(子も含める)を表示

            clearImage.SetActive(false) ;   //同時表示を避ける

            //Nextボタンを無効化する
            Button bt = nextButton.GetComponent<Button>();
            bt.interactable = false;    //Buttonコンポーネントのボタン有効化の変数をfalse

            PlayerController.gameState = "gameend";    //何回もこの一連の処理を繰り返さないようにするため

        }
        else if(PlayerController.gameState == "playing")
        {
            //ゲーム中
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            //PlayerControllerを取得する
            PlayerController playerCnt = player.GetComponent<PlayerController>();
        }
    }

    void InactiveImage()
    {
        mainImage.SetActive(false );
    }

    //クリアとオーバーの画像を指定するメソッド
    //void StatusImage()
    //{
    //    clearImage.SetActive(false) ;
    //    overImage.SetActive(false ) ;
    //}
}
