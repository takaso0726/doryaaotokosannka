using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // コルーチン用
using TMPro; // TextMeshPro

public class Buttom : MonoBehaviour, ICursorClickable
{
    //インスペクターからシーン名を自由に設定できるよう
    [SerializeField] private string targetScene;

    //点滅させたいコンポーネントを指定
    [SerializeField] private TextMeshProUGUI buttonText;

    // 点滅のスピード
    [SerializeField] private float blinkSpeed = 5.0f;

    private Coroutine saveCoroutine; // 点滅するコルーチンの管理
    private Color originColor; // 初期のカラー

    void Start()
    {
        if (buttonText != null)
        {
            originColor = buttonText.color;
        }
    }

    void Update()
    {
    }

    //カーソルが重なった時
    public void OnCursorEnter()
    {
        Debug.Log("ボタンにカーソルが重なりました");

        //テキストを点滅させる
        //すでにコルーチンが動いていたら止める
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
        }

        //点滅処理をスタート
        if (buttonText != null)
        {
            saveCoroutine = StartCoroutine(BlinkText());
        }

    }

    //カーソルが離れた時
    public void OnCursorExit()
    {
        Debug.Log("ボタンのカーソルが離れました");

        //テキストを正常化させる
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }

        //テキストの色と透明度を元に戻す
        if (buttonText != null)
        {
            buttonText.color = originColor;
        }
    }

    //カーソルが重なった状態で押された時
    public void OnCursorClick()
    {
        
        // 設定されたシーン名が空でなければ遷移する
        if (!string.IsNullOrEmpty(targetScene))
        {
            LoadNextScene(targetScene);
        }
    }

    // シーン移動を実行する関数
    public void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // コルーチンで点滅させる関数
    private IEnumerator BlinkText()
    {
        while (true)
        {
            //Mathf.Sinで0.0 ～ 1.0 の間で変化
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1.0f) / 2.0f;

            //テキストのアルファ値（透明度）だけを書き換える
            Color targetColor = originColor;
            targetColor.a = alpha;
            buttonText.color = targetColor;

            // 1フレーム待つ
            yield return null;
        }
    }
}
