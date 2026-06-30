using TMPro;
using UnityEngine;

public class TitileText : MonoBehaviour
{
    //変数宣言
    float alpha;        //アルファ値用の変数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

        alpha =  Mathf.Sin(Time.time);
        gameObject.GetComponent<TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
    }
}
