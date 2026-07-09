using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// タイトルのテキスト
public class TitileText : MonoBehaviour
{
    //変数宣言
    float alpha;        //アルファ値用の変数
    private int clickCount = 0;　//クリック回数をカウント
    private Title titleScript; // タイトルのスクリプト
    private TextMeshProUGUI textMeshPro; // コンポーネントの保存

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //コンポーネントを保存
        textMeshPro = GetComponent<TextMeshProUGUI>();

        //シーン内からTitleスクリプトを紐付け
        titleScript = UnityEngine.Object.FindAnyObjectByType<Title>();
    }

    // Update is called once per frame
    void Update()
    {
        alpha = (Mathf.Sin(Time.time * 2.0f) + 1.0f) / 2.0f;

        if (textMeshPro != null)
        {
            // アルファ値を反映
            textMeshPro.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    //クリック回数を確認
    public void OnCursorClick()
    {
        clickCount++;
        Debug.Log("クリックされました");

        if (clickCount >= 3)
        {
            if (titleScript != null)
            {
                titleScript.OnTextThriceClicked();
            }
            clickCount = 0;
        }
    }
}
