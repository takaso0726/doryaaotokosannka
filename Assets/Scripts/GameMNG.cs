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

    [Header("漢気ゲージ(0～200%、1本100%の2本ストック制)")]
    //プレイヤー側 漢気ゲージ(左右2本)
    public Slider P_KankiBar1;
    public Slider P_KankiBar2;
    //敵側 漢気ゲージ(左右2本)
    public Slider E_KankiBar1;
    public Slider E_KankiBar2;

    float PTimer;
    int PCnt;

    AudioSource BGM_Lv1;
    public AudioClip BGM;

    //ゲームオーバーに移行するまでの時間
    public float gameOverTime;
    //プレイヤーが倒されてからの経過時間
    float playerChangeTimer;
    //プレイヤーの状態
    test.Status playerStatus;

    [Header("勝敗カメラ演出")]
    //プレイヤーのTransform（勝利時にカメラがズームする対象）
    public Transform PlayerTransform;
    //敵のTransform（プレイヤー敗北＝敵勝利時にカメラがズームする対象）
    public Transform EnemyTransform;
    //シーン中のカメラコントローラー
    public FightingCameraController cameraController;


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

        //漢気ゲージの初期化(1本あたり0～100%)
        P_KankiBar1.maxValue = 100f;
        P_KankiBar1.value = 0f;
        P_KankiBar2.maxValue = 100f;
        P_KankiBar2.value = 0f;

        E_KankiBar1.maxValue = 100f;
        E_KankiBar1.value = 0f;
        E_KankiBar2.maxValue = 100f;
        E_KankiBar2.value = 0f;

        playerChangeTimer = 0.0f;
        playerStatus = test.Status.Live;

        //効果音再生用のAudioClipを取得
        BGM_Lv1 = GetComponent<AudioSource>();

        BGM_Lv1.loop = true;

        //BGM再生
        BGM_Lv1.Play();
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

    //プレイヤーの漢気ゲージを更新(gaugeは0～200%を渡す)
    public void Player_SetKankiGauge(float gauge)
    {
        gauge = Mathf.Clamp(gauge, 0f, 200f);
        //1本目(0～100%)
        P_KankiBar1.value = Mathf.Clamp(gauge, 0f, 100f);
        //2本目(100～200%)
        P_KankiBar2.value = Mathf.Clamp(gauge - 100f, 0f, 100f);
    }

    //敵の漢気ゲージを更新(gaugeは0～200%を渡す)
    public void Enemy_SetKankiGauge(float gauge)
    {
        gauge = Mathf.Clamp(gauge, 0f, 200f);
        //1本目(0～100%)
        E_KankiBar1.value = Mathf.Clamp(gauge, 0f, 100f);
        //2本目(100～200%)
        E_KankiBar2.value = Mathf.Clamp(gauge - 100f, 0f, 100f);
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

        //勝敗が決まったら、勝った方にカメラをズームする
        if (cameraController != null)
        {
            if (playerStatus == test.Status.Win)
            {
                //プレイヤーが勝利
                cameraController.FocusOnTarget(PlayerTransform);
            }
            else if (playerStatus == test.Status.Dead)
            {
                //プレイヤーが敗北＝敵の勝利
                cameraController.FocusOnTarget(EnemyTransform);
            }
        }
    }
}


