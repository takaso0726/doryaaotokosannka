using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMNG : MonoBehaviour
{
    //変数宣言
    public Text PlayerHP_Text;
    public Text EnemyHP_Text;
    public Text Player_Timer_Text;
    public Text Player_Cnt_Text;

    int PlayerHP;
    int EnemyHP;

    public Slider P_HPbar;
    public Slider E_HPbar;

    float PTimer;
    int PCnt;

    //ゲームオーバーに移行するまでの時間
    public float gameOverTime;
    //プレイヤーが倒されてからの経過時間
    float playerChangeTimer;
    //プレイヤーの状態
    test.Status playerStatus;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerHP_Text.text = "100";
        EnemyHP_Text.text = "100";
        PlayerHP = 100;
        EnemyHP = 100;

        Player_Timer_Text.text = "0";
        Player_Cnt_Text.text = "0";
        PTimer = 0;
        PCnt = 0;

        P_HPbar.value = PlayerHP;
        E_HPbar.value = EnemyHP;

        playerChangeTimer = 0.0f;
        playerStatus = test.Status.Live;
    }

    // Update is called once per frame
    void Update()
    {
        //Playerの状態がDeadなら
        if(playerStatus == test.Status.Dead)
        {
            //経過時間を加える
            playerChangeTimer += Time.deltaTime;
            //経過時間がgameOverTIme以上になったら
            if(playerChangeTimer >= gameOverTime)
            {
                //ゲームオーバーシーンを読み込む
                SceneManager.LoadScene("GameOver");
                //経過時間をリセット
                playerChangeTimer = 0.0f;
            }
        }
        else if(playerStatus == test.Status.Win)
        {
            //経過時間を加える
            playerChangeTimer += Time.deltaTime;
            //経過時間がgameOverTIme以上になったら
            if (playerChangeTimer >= gameOverTime)
            {
                //ゲームオーバーシーンを読み込む
                SceneManager.LoadScene("GameClear");
                //経過時間をリセット
                playerChangeTimer = 0.0f;
            }
        }
            
    }

    public void Player_ReduceHP(int hp)
    {
        //HPを減らす
        PlayerHP = hp;
        //HPを表示
        PlayerHP_Text.text = PlayerHP.ToString();
        P_HPbar.value = PlayerHP;

    }

    public void Enemy_ReduceHP(int hp)
    {
        //HPを減らす
        EnemyHP = hp;
        //HPを表示
        EnemyHP_Text.text = EnemyHP.ToString();
        E_HPbar.value = EnemyHP;

    }

    public void PlayerUI(float Timer,int Cnt)
    {
        PTimer = Timer;
        PCnt = Cnt;

        Player_Timer_Text.text = PTimer.ToString();
        Player_Cnt_Text.text = PCnt.ToString();

    }

    //他のC#スクリプトから呼び出す変数
    public void SettestStatus(test.Status ps)
    {
        playerStatus = ps;
    }
}


