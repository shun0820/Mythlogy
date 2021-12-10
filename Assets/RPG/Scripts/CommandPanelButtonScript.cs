using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandPanelButtonScript : MonoBehaviour,ISelectHandler,IDeselectHandler
{
    //�{�^�����󎞂ɕ\������摜
    private Image selectedImage;
    //�I�����̉���
    private AudioSource selectedAudioSource;

    void Awake()
    {
        selectedImage = transform.Find("Image").GetComponent<Image>();
        selectedAudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //�A�N�e�B�u�ɂȂ����Ƃ��A���g��EventSystem�őI������Ă�����
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            selectedImage.enabled = true;
            selectedAudioSource.Play();
        } else {
            selectedImage.enabled = false;
        }
    }

    //�{�^�����I�����ꂽ�Ƃ��Ɏ��s
    public void OnSelect(BaseEventData eventData)
    {
        selectedImage.enabled = true;
        selectedAudioSource.Play();
    }

    //�{�^�����I���������ꂽ�Ƃ��Ɏ��s
    public void OnDeselect(BaseEventData eventData)
    {
        selectedImage.enabled = false;
    }
    
}
