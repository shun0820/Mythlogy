using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class UnityChanTalkScript : MonoBehaviour
{
    //会話相手
    private GameObject conversationPartner;
    //会話可能を示すアイコン
    [SerializeField]
    private GameObject talkEnableIcon;

    //TalkUIゲームオブジェクト
    [SerializeField]
    private GameObject talkUI = null;
    //メッセージUI
    private Text messageText = null;
    //表示するメッセージ
    private string allMessage = null;
    // 使用する分割文字列(この文字列で改行？します)
    [SerializeField]
    private string splitString = "<>";
    // 分割済みメッセージ
    private string[] splitMessage;
    //分割したメッセージの何番目か
    private int messageNumber;
    //テキストスピード
    [SerializeField]
    private float textSpeed = 0.05f;
    //経過時間
    private float elapsedTime = 0f;
    //今見ている文字番号
    private int nowTextNumber = 0;
    //マウスクリックを促すアイコン
    [SerializeField]
    private Image clickIcon = null;
    //クリックアイコンの点滅秒数
    [SerializeField]
    private float clickFlashTime = 0.2f;
    //1回分のメッセージを表示したかどうか
    private bool isOneMessage = false;
    //メッセージを全て表示したかどうか
    private bool isEndMessage = false;


    // Start is called before the first frame update
    void Start()
    {
        clickIcon.enabled = false;
        messageText = talkUI.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //メッセージが終わっているか、これ以降メッセージがないのならこの後は何もしない
        if (isEndMessage || allMessage == null)
        {
            return;
        }

        //1回に表示するメッセージを表示していない
        if (!isOneMessage)
        {
            //テキスト標示時間を経過したらメッセージを追加
            if (elapsedTime >= textSpeed)
            {
                messageText.text += splitMessage[messageNumber][nowTextNumber];

                nowTextNumber++;
                elapsedTime = 0f;

                //メッセージをすべて表示、または行数が最大限表示された
                if (nowTextNumber >= splitMessage[messageNumber].Length)
                {
                    isOneMessage = true;
                }
            }
            elapsedTime += Time.deltaTime;

            //メッセージ表示中にマウスをクリックしたら残りを全部表示
            if (Input.GetButtonDown("Action"))
            {
                //ここまでに表示しているテキストに残りのメッセージを足す
                messageText.text += splitMessage[messageNumber].Substring(nowTextNumber);
                isOneMessage = true;
            }
        }
        else
        {
            //1回に表示するメッセージを表示した
            elapsedTime += Time.deltaTime;

            //クリックアイコンを点滅する時間を超えた時、反転させる
            if (elapsedTime >= clickFlashTime)
            {
                clickIcon.enabled = !clickIcon.enabled;
                elapsedTime = 0f;
            }

            if (Input.GetButtonDown("Action"))
            {
                nowTextNumber = 0;
                messageNumber++;
                messageText.text = "";
                clickIcon.enabled = false;
                elapsedTime = 0f;
                isOneMessage = false;

                //メッセージがすべて表示されていたら、ゲームオブジェクト(会話メッセージパネル)事態を削除
                if (messageNumber >= splitMessage.Length)
                {
                    EndTalking();
                }
            }
        }
    }

    private void LateUpdate()
    {
        //会話相手がいる場合はTalkIconの位置を会話相手の頭上に表示
        if (conversationPartner != null)
        {
            talkEnableIcon.transform.Find("Panel").position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(conversationPartner.transform.position + Vector3.up * 2f);
        }
    }

    //会話相手を設定
    public void SetConversationPartner(GameObject partnerObject)
    {
        talkEnableIcon.SetActive(true);
        conversationPartner = partnerObject;
    }

    //会話相手をリセット
    public void ResetConversationPartner(GameObject partnerObject)
    {
        //そもそも会話相手がいない場合は何もしない
        if (conversationPartner == null)
        {
            return;
        }
        //会話相手と引数で受け取った相手が同じインスタンスIDを持つなら会話相手をなくす
        if (conversationPartner.GetInstanceID() == partnerObject.GetInstanceID())
        {
            talkEnableIcon.SetActive(false);
            conversationPartner = null;
        }
    }

    //会話相手を渡す(返す)(＝Getterメソッド)
    public GameObject GetConversationPartner()
    {
        return conversationPartner;
    }

    //会話開始
    public void StartTalking()
    {
        var villagerScript = conversationPartner.GetComponent<VillagerScript>();
        villagerScript.SetState(VillagerScript.State.Talk, transform);
        this.allMessage = villagerScript.GetConversation().GetConversationMessage();
        //分割文字列で1回に表示するメッセージを分割する
        splitMessage = Regex.Split(allMessage, @"\s*" + splitString + @"\s*", RegexOptions.IgnorePatternWhitespace);
        //初期化処理
        nowTextNumber = 0;
        messageNumber = 0;
        messageText.text = "";
        talkUI.SetActive(true);
        talkEnableIcon.SetActive(false);
        isOneMessage = false;
        isEndMessage = false;
        //会話開始時の入力はいったんリセット
        Input.ResetInputAxes();
    } 

    void EndTalking()
    {
        isEndMessage = true;
        talkUI.SetActive(false);
        //プレイヤーキャラクターと村人両方の状態を変更する
        var villagerScript = conversationPartner.GetComponent<VillagerScript>();
        villagerScript.SetState(VillagerScript.State.Wait);
        GetComponent<UnityChanScript>().SetState(UnityChanScript.State.Normal);
        Input.ResetInputAxes();
    }
}
