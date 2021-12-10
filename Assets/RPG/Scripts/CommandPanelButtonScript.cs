using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandPanelButtonScript : MonoBehaviour,ISelectHandler,IDeselectHandler
{
    //ボタン洗濯時に表示する画像
    private Image selectedImage;
    //選択時の音声
    private AudioSource selectedAudioSource;

    void Awake()
    {
        selectedImage = transform.Find("Image").GetComponent<Image>();
        selectedAudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //アクティブになったとき、自身がEventSystemで選択されていたら
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            selectedImage.enabled = true;
            selectedAudioSource.Play();
        } else {
            selectedImage.enabled = false;
        }
    }

    //ボタンが選択されたときに実行
    public void OnSelect(BaseEventData eventData)
    {
        selectedImage.enabled = true;
        selectedAudioSource.Play();
    }

    //ボタンが選択解除されたときに実行
    public void OnDeselect(BaseEventData eventData)
    {
        selectedImage.enabled = false;
    }
    
}
