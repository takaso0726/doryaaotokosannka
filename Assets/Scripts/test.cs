using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;

public class test : MonoBehaviour
{
    //変数宣言
    public float moveSpeed;
    public bool Jumpflag;
    bool Controlflag;
    bool Menflag;
    public int atk;                 //攻撃力
    int AttackCnt;

    Rigidbody rb;                 //Rigidbody型の変数
    public Vector3 force;

    float AttackTimer;
    public int HP;
    int RebornCnt;                 //復活回数のカウント

    int Control_I;                  //操作の入力から分岐するための変数

    //効果音用
    AudioSource se;
    public AudioClip MenBlock_se;

    public test.Status Player_status;

    float RebornTimer = 0.0f;
    int MenCnt = 0;

    //プレイヤーの状態
    public enum Status
    {
        Neutral,    //待機(ニュートラル)
        Attack,     //攻撃
        Stand,      //仁王立ち
        Throw,      //投げ(つかみ)
        Live,       //生存
        Reborn,     //復活
        Dead,       //死亡
        Win,        //勝利
    };


    public ParticleSystem Men_particle;     //仁王立ち用のパーティクル
    public ParticleSystem Hit_particle;     //ヒット時用のパーティクル

    public Enemy enemy;
    public Animator animator;

    bool flag;
    



    //当たり判定の子オブジェクト
    CapsuleCollider Head;
    CapsuleCollider RightArm;
    CapsuleCollider RightForeArm;
    CapsuleCollider RightHand;
    CapsuleCollider RightFoot;
    CapsuleCollider RightUpLeg;
    CapsuleCollider RightLeg;
    CapsuleCollider LeftArm;
    CapsuleCollider LeftForeArm;
    CapsuleCollider LeftHand;
    CapsuleCollider LeftFoot;
    CapsuleCollider LeftUpLeg;
    CapsuleCollider LeftLeg;

    CapsuleCollider Player_Collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Jumpflag = true;
        Controlflag = true;
        Menflag = false;
        AttackTimer = 0.0f;
        HP = 100;
        atk = 10;
        RebornCnt = 1;
        Control_I = 0;
        AttackCnt = 0;

       

        rb = GetComponent<Rigidbody>();		//PlayerのRigidbodyを取得

        animator = GetComponent<Animator>();

        //<当たり判定の子オブジェクトの取得>
        //頭の当たり判定
        Head = GameObject.Find("P-Head").gameObject.GetComponent<CapsuleCollider>();


        //右腕の当たり判定
        RightArm = GameObject.Find("P-RightArm").gameObject.GetComponent<CapsuleCollider>();
        //右前腕の当たり判定
        RightForeArm = GameObject.Find("P-RightForeArm").gameObject.GetComponent<CapsuleCollider>();
        //右手の当たり判定
        RightHand = GameObject.Find("P-RightHand").gameObject.GetComponent<CapsuleCollider>();

        //右足の当たり判定
        RightFoot = GameObject.Find("P-RightFoot").gameObject.GetComponent<CapsuleCollider>();
        //右ふとももの当たり判定
        RightUpLeg = GameObject.Find("P-RightUpLeg").gameObject.GetComponent<CapsuleCollider>();
        //右ふくらはぎの当たり判定
        RightLeg = GameObject.Find("P-RightLeg").gameObject.GetComponent<CapsuleCollider>();

        //左腕の当たり判定
        LeftArm = GameObject.Find("P-LeftArm").gameObject.GetComponent<CapsuleCollider>();
        //左前腕の当たり判定
        LeftForeArm = GameObject.Find("P-LeftForeArm").gameObject.GetComponent<CapsuleCollider>();
        //左手の当たり判定
        LeftHand = GameObject.Find("P-LeftHand").gameObject.GetComponent<CapsuleCollider>();

        //左足の当たり判定
        LeftFoot = GameObject.Find("P-LeftFoot").gameObject.GetComponent<CapsuleCollider>();
        //左ふとももの当たり判定
        LeftUpLeg = GameObject.Find("P-LeftUpLeg").gameObject.GetComponent<CapsuleCollider>();
        //左ふくらはぎの当たり判定
        LeftLeg = GameObject.Find("P-LeftLeg").gameObject.GetComponent<CapsuleCollider>();

        Player_Collider = gameObject.GetComponent<CapsuleCollider>();

        //効果音再生用のAudioClipを取得
        se = GetComponent<AudioSource>();

        flag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP > 0)
        {
            if (AttackTimer <= 0.0f)
            {
                AttackTimer += Time.deltaTime;
            }
            else
            {
                if(!Controlflag)
                {
                    Controlflag = true;
                }
                Menflag = false;
                AttackCnt = 0;
                //攻撃用の当たり判定を非Activeにする
                AtkHitboxOFF();
                flag = true;
            }
        }
        else//HPが0か
        {
            //復活チャレンジに移行
            RebornCh(RebornCnt);
        }

        //<入力チェック>
        //右移動(前)
        if(Gamepad.current != null)
        {
            if //(Keyboard.current.dKey.wasPressedThisFrame && Controlflag)
            (Gamepad.current.leftStick.value.x >= 0.03 && Controlflag)
            {
                //操作用変数に代入
                Control_I = 1;
                //操作可能フラグをOFF
                //Controlflag = false;
            }
            //左移動(後ろ)
            if //(Keyboard.current.aKey.wasPressedThisFrame && Controlflag)
            (Gamepad.current.leftStick.value.x <= -0.03 && Controlflag)
            {
                //操作用変数に代入
                Control_I = 2;
                //操作可能フラグをOFF
                //Controlflag = false;

            }
            //ジャンプ
            if //(Keyboard.current.wKey.wasPressedThisFrame && Controlflag && Jumpflag)
            (Gamepad.current.xButton.wasPressedThisFrame && (Jumpflag && Controlflag))
            {
                //デバックログの表示
                Debug.Log("ジャンプ");
                //操作用変数に代入
                Control_I = 3;
                //操作可能フラグをOFF
                Controlflag = false;
            }
            //しゃがみ
            /*if (Gamepad.current.leftStick.value.y <= -0.43 && Controlflag)
            //(Keyboard.current.sKey.wasPressedThisFrame && Controlflag && Jumpflag)
            {
                //操作用変数に代入
                Control_I = 10;
            }
            else if (Gamepad.current.leftStick.value.y >= 0.0 && Controlflag)
            {
                Control_I = 0;
                animator.SetBool("Crouch", false);

                //当たり判定を上げる
                Player_Collider.height = 2.0f;
                Player_Collider.center = new Vector3(0, 1.0f, 0);

            }
            */
            bool isCrouching = Gamepad.current.leftStick.value.y <= -0.43f;

            if (isCrouching && Controlflag)
            {
                Control_I = 10;
            }
            else
            {
                // しゃがみ解除の見た目処理だけ行い、Control_Iは触らない
                if (animator.GetBool("Crouch"))
                {
                    animator.SetBool("Crouch", false);
                    Player_Collider.height = 2.0f;
                    Player_Collider.center = new Vector3(0, 1.0f, 0);
                }
            }

            //パンチ
            if //(Keyboard.current.qKey.wasPressedThisFrame && Controlflag)
            ((Gamepad.current.bButton.wasPressedThisFrame) && Controlflag)
            {
                //デバックログの表示
                Debug.Log("パンチ");

                // 予約をすべてクリア
                animator.ResetTrigger("Punch");
                animator.ResetTrigger("Flying-kick");
                animator.ResetTrigger("Kick");
                animator.ResetTrigger("Jump");
                //操作用変数に代入
                Control_I = 4;
                //操作可能フラグをOFF
                Controlflag = false;

            }
            //キック
            if  //(Keyboard.current.zKey.wasPressedThisFrame && Controlflag)
               (Gamepad.current.aButton.wasPressedThisFrame && Controlflag)
            {
                // 予約をすべてクリア
                animator.ResetTrigger("Punch");
                animator.ResetTrigger("Flying-kick");
                animator.ResetTrigger("Kick");
                animator.ResetTrigger("Jump");

                if//(Keyboard.current.wKey.wasPressedThisFrame)
                    (Gamepad.current.leftStick.value.y < -0.43f)
                {
                    //デバックログの表示
                    Debug.Log("下キック");
                    Control_I = 8;
                }
                else if//(Keyboard.current.sKey.wasPressedThisFrame)
                    (Gamepad.current.leftStick.value.y > 0.25f)
                {
                    //デバックログの表示
                    Debug.Log("上キック");

                    Control_I = 9;
                }
                else
                {
                    //デバックログの表示
                    Debug.Log("キック");

                    //操作用変数に代入
                    Control_I = 5;
                }

                //操作可能フラグをOFF
                Controlflag = false;
            }
            //仁王立ち
            if //(Keyboard.current.eKey.wasPressedThisFrame && Controlflag)
            (Gamepad.current.yButton.wasPressedThisFrame && Controlflag)
            {
                //デバックログの表示
                Debug.Log("仁王立ち");
                //操作用変数に代入
                Control_I = 6;
                Controlflag = false;
            }
            //投げ
            if//(Keyboard.current.xKey.wasPressedThisFrame && Controlflag)
            (Gamepad.current.rightShoulder.wasPressedThisFrame && Controlflag)
            {
                //デバックログの表示
                Debug.Log("掴み");

                //操作用変数に代入
                Control_I = 7;
                Controlflag = false;
            }
        }

        switch (Control_I)
        {
            case 0:
                //待機
                AttackCnt = 0;
                break;

            case 1:
                //前に移動
                Movefront();
                break;

            case 2:
                //後ろに移動
                Moveback();
                break;

            case 3:
                //ジャンプ
                Jumpflag = false;
                Movejump();
                break;

            case 4:
                //弱攻撃(パンチ)
                //0.5秒遅延してから実行
                Attack_punch();
                break;

            case 5:
                //強攻撃(キック)
                Attack_kick();
                break;

            case 6:
                //仁王立ち
                Standing();
                break;

            case 7:
                //投げ
                //□秒遅延してから実行
                Throwing();
                break;

            case 8:
                //下キック
                //遅延無し実行
                DonwnKick();
                break;

            case 9:
                //下キック
                //遅延0.4秒で実行　
                UpKick();
                break;

            case 10:
                //しゃがみ
                //遅延無し実行
                Crouch();
                break;
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            //ジャンプフラグをあげる
            Jumpflag = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //当たった対象物の[tag]がEAttack (エネミーによる攻撃)だった場合は処理する
        if (collision.gameObject.CompareTag("EAttack") && HP > 0)
        {
            //体力を表示
            Debug.Log("プレイヤーのHP : " + HP);

            if (Menflag)
            {
                //攻撃力を追加
                atk += enemy.atk;
                Debug.Log("漢!!");
                se.PlayOneShot(MenBlock_se);
                HP -= enemy.atk / 2;
            }
            else
            {
                //ヒット時のアニメーション再生
                animator.SetTrigger("Hit");


                //衝突位置を取得
                Vector3 HitPoint = collision.ClosestPoint(collision.transform.position);

                // パーティクルシステムのインスタンスを生成する。
                ParticleSystem HitParticle = Instantiate(Hit_particle, HitPoint,Quaternion.Euler(-90.0f, 0.0f, 0.0f));

                // パーティクルを発生させる。
                HitParticle.Play();
                // ※第一引数をHitParticleだけにするとコンポーネントしか削除されない。
                Destroy(HitParticle.gameObject, 1.0f);

                //HPを減らす
                HP -= enemy.atk;
                //ノックバック
                //transform.Translate(0.0f, 0.0f, -0.65f);
            }
            //UI表示
            GameMNG mng = GameObject.Find("ManagerObject").GetComponent<GameMNG>();
            mng.Player_ReduceHP(HP);
            enemy.atk = 10;

        }
    }

    //復活チャレンジ
    void RebornCh(int RebornCnt)
    {
        GameMNG mng = GameObject.Find("ManagerObject").GetComponent<GameMNG>();
        mng.PlayerUI(RebornTimer, MenCnt);
        //時間を加算
        RebornTimer += Time.deltaTime;
        //コントロールフラグをさげる
        Controlflag = false;
        //マネージャーに「復活状態」を設定する
        mng.SettestStatus(test.Status.Reborn);
        Debug.Log(RebornTimer);

        if (RebornTimer < 5.0f)
        {
            //連打回数が15回か
            if (MenCnt <= 11 + ( 3 * (RebornCnt)))
            {
                //エンターキーで連打
                if (Gamepad.current.bButton.wasPressedThisFrame)
                {
                    //カウントを加算
                    ++MenCnt;
                }
            }
            else
            {
                //復活
                HP = 30;
                //復活カウントを加算
                ++RebornCnt;
                MenCnt = 0;
                RebornTimer = 0.0f;
                Controlflag = true;
            }
        }
        else
        {
            //マネージャーに「死亡状態」を設定する
            mng.SettestStatus(test.Status.Dead);
        }
        Debug.Log(MenCnt);
    }

    void Movefront()//前移動
    {
        //プレイヤーを前(右)に移動させる
        transform.Translate(0.0f, 0.0f, moveSpeed);
        //操作用変数をリセット
        Control_I = 0;
    }

    void Moveback()//後ろ移動
    {
        //プレイヤーを後ろ(左)に移動させる
        transform.Translate(0.0f, 0.0f, -moveSpeed);
        //操作用変数をリセット
        Control_I = 0;
    }

    void Movejump()
    {
        //ジャンプ
        animator.SetTrigger("Jump");
        //プレイヤーをジャンプさせる(Forceを使う)
        rb.AddForce(force);
        //操作用変数をリセット
        Control_I = 0;
        //フラグ 
        Jumpflag = false;
    }

    void Attack_punch()
    {
        Controlflag = false;
        Control_I = 0;

        // 連打防止のためトリガーをリセット
        animator.ResetTrigger("Punch");
        animator.ResetTrigger("Flying-kick");

        //タイマーをリセット
        AttackTimer = -0.5f;
        if(Jumpflag)
        {
            //弱攻撃(パンチ)
            animator.SetTrigger("Punch");
            //攻撃箇所の当たり判定をActiveにする
            LeftHand.enabled = true;        //左手
            RightHand.enabled = true;       //右手
        }
        else
        {
            //空中攻撃
            animator.SetTrigger("Flying-kick");
            LeftFoot.enabled = true;
            LeftLeg.enabled = true;
            RightFoot.enabled = true;
        }   
    }

    void Attack_kick()
    {
        //コントロールフラグをOFF
        Controlflag = false;
        //トリガーをリセット
        animator.ResetTrigger("Kick");

        //強攻撃(キック)
        animator.SetTrigger("Kick");
        //タイマーをリセット
        AttackTimer = -0.6f;
        //攻撃用の当たり判定をON
        LeftFoot.enabled = true;       //右足
        LeftUpLeg.enabled = true;      //右太もも
        LeftLeg.enabled = true;        //右ふくらはぎ
        //操作用変数をリセット
        Control_I = 0;
    }

    void Standing()
    {
        Controlflag = false;
        //仁王立ち
        AttackTimer = -0.5f;
        Menflag = true;
        Debug.Log("仁王立ち");

        // パーティクルシステムのインスタンスを生成する。
        ParticleSystem newParticle = Instantiate(Men_particle, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z),
            Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        // パーティクルを発生させる。
        newParticle.Play();
        // ※第一引数をnewParticleだけにするとコンポーネントしか削除されない。
        Destroy(newParticle.gameObject, 1.0f);

        //操作用変数をリセット
        Control_I = 0;
    }

    void Throwing()
    {
        Controlflag = false;
        //トリガーをリセット
        animator.ResetTrigger("Throw-start");

        //投げアニメーション再生
        animator.SetTrigger("Throw-start");
        AttackTimer = -1.5f;

        // 掴む時の距離
        float distanceZ = Mathf.Abs(enemy.transform.position.z - transform.position.z);

        //操作用変数をリセット
        Control_I = 0;

        //投げれるか距離でチェック(距離と相手の状態で判断したい)
        if (enemy.Enemy_Status != Enemy.Status.Attack && (enemy.transform.position.z - transform.position.z < 1.75f) && flag)
        {
            Debug.Log("投げ成功");
            enemy.transform.Translate(0.0f, 0.0f, -0.0025f);
            enemy.animator.SetTrigger("Thrown");
            enemy.damege(5);
            flag = false;
        }
    }

    //下キック
    void DonwnKick()
    {
        //上書き防止のため固定
        Controlflag = false;
        //トリガーをリセット
        animator.ResetTrigger("DownKick");

        animator.SetTrigger("DownKick");
        AttackTimer = -0.5f;
        Control_I = 0;
        //当たり判定ON
        RightFoot.enabled = true;
        RightLeg.enabled = true;
    }

    //上キック
    void UpKick()
    {
        //上書き防止のため固定
        Controlflag = false;
        //トリガーをリセット
        animator.ResetTrigger("UpKick");

        animator.SetTrigger("UpKick");
        AttackTimer = -0.7f;
        Control_I = 0;

        //当たり判定ON
        RightFoot.enabled = true;
        RightLeg.enabled = true;
    }

    //しゃがみ
    void Crouch()
    {
      
        //当たり判定を下げる
        Player_Collider.height = 0.65f;
        Player_Collider.center = new Vector3(0, 0.5f, 0);

        //アニメーション再生
        animator.SetBool("Crouch", true);

    }


    void AtkHitboxOFF()
    {
        //全ての攻撃用当たり判定をOFF
        LeftHand.enabled = false;
        LeftForeArm.enabled = false;
        LeftArm.enabled = false;
        Head.enabled = false;
        RightArm.enabled = false;
        RightForeArm.enabled = false;
        RightHand.enabled = false;
        RightFoot.enabled = false;
        RightUpLeg.enabled = false;
        RightLeg.enabled = false;
        LeftArm.enabled = false;
        LeftForeArm.enabled = false;
        LeftHand.enabled = false;
        LeftFoot.enabled = false;
        LeftUpLeg.enabled = false;
        LeftLeg.enabled = false;
    }

    public void damege(int n)
    {
        HP -= n;
        GameMNG mng = GameObject.Find("ManagerObject").GetComponent<GameMNG>();
        mng.Enemy_ReduceHP(HP);
    }


}