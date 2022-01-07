using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanelButtonScript : MonoBehaviour
{
    private Item item;
    //アイテムタイトル表示テキスト
    private Text itemTitleText;
    //アイテム情報を表示するテキスト
    private Text itemInformationText;

    private void Awake()
    {
        //アイテム名、アイテム情報を取得してフィールドに代入
        itemTitleText = transform.root.Find("ItemInformationPanel/Title").GetComponent<Text>();
        itemInformationText = transform.root.Find("ItemInformationPanel/Information").GetComponent<Text>();
    }

    //ボタンが選択されたときに実行
    public void OnSelect(BaseEventData eventData)
    {
        ShowItemInformation();
    }

    //アイテム情報の表示
    public void ShowItemInformation()
    {       
        itemTitleText.text = item.GetItemName();
        itemInformationText.text = item.GetItemInformation();
    }

    //データをセットする
    public void SetParam(Item item)
    {
        this.item = item;
    }
}
