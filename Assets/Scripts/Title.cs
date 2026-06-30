using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement;


public class Title : MonoBehaviour
{
    //変数宣言
    AudioSource se;
    public AudioClip Titlese;
    float CntTimer;
    bool flag;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //効果音再生用のAudioClipを取得
        se = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //キー入力でインゲームに移行
        if(Gamepad.current.aButton.wasPressedThisFrame)
        {

            //効果音再生
            se.PlayOneShot(Titlese);
            flag = true;
        }
        if(flag)
        {
            //経過時間を加える
            CntTimer += Time.deltaTime;
            //経過時間がgameOverTIme以上になったら
            if (CntTimer >= 0.7f)
            {
                //ゲームオーバーシーンを読み込む
                SceneManager.LoadScene("InGame");

            }
        }
        

    }
}
