using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvesationScopeScript : MonoBehaviour
{
    // OnTriggerStayはトリガーオブジェクト(今回の場合はSearchAreaオブジェクト)に侵入している間呼び出される
    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" && col.GetComponent<UnityChanScript>().GetState() != UnityChanScript.State.Talk)
        {
            //プレイヤーキャラ(ユニティちゃん)が近づいたら、会話相手として自分のゲームオブジェクトを返す(渡す)
            col.GetComponent<UnityChanTalkScript>().SetConversationPartner(transform.parent.gameObject);
        }
    }

    //OnTriggerExitはトリガーオブジェクトから抜け出したときに呼び出される
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && col.GetComponent<UnityChanScript>().GetState() != UnityChanScript.State.Talk)
        {
            //プレイヤーキャラが遠ざかったら(SearchAreaから抜けたら)会話相手から外す
            col.GetComponent<UnityChanTalkScript>().ResetConversationPartner(transform.parent.gameObject);
        }
    }
}
