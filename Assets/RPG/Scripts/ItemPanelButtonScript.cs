using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanelButtonScript : MonoBehaviour
{
    private Item item;
    //�A�C�e���^�C�g���\���e�L�X�g
    private Text itemTitleText;
    //�A�C�e������\������e�L�X�g
    private Text itemInformationText;

    private void Awake()
    {
        //�A�C�e�����A�A�C�e�������擾���ăt�B�[���h�ɑ��
        itemTitleText = transform.root.Find("ItemInformationPanel/Title").GetComponent<Text>();
        itemInformationText = transform.root.Find("ItemInformationPanel/Information").GetComponent<Text>();
    }

    //�{�^�����I�����ꂽ�Ƃ��Ɏ��s
    public void OnSelect(BaseEventData eventData)
    {
        ShowItemInformation();
    }

    //�A�C�e�����̕\��
    public void ShowItemInformation()
    {       
        itemTitleText.text = item.GetItemName();
        itemInformationText.text = item.GetItemInformation();
    }

    //�f�[�^���Z�b�g����
    public void SetParam(Item item)
    {
        this.item = item;
    }
}
