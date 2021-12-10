using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class UnityChanTalkScript : MonoBehaviour
{
    //��b����
    private GameObject conversationPartner;
    //��b�\�������A�C�R��
    [SerializeField]
    private GameObject talkEnableIcon;

    //TalkUI�Q�[���I�u�W�F�N�g
    [SerializeField]
    private GameObject talkUI = null;
    //���b�Z�[�WUI
    private Text messageText = null;
    //�\�����郁�b�Z�[�W
    private string allMessage = null;
    // �g�p���镪��������(���̕�����ŉ��s�H���܂�)
    [SerializeField]
    private string splitString = "<>";
    // �����ς݃��b�Z�[�W
    private string[] splitMessage;
    //�����������b�Z�[�W�̉��Ԗڂ�
    private int messageNumber;
    //�e�L�X�g�X�s�[�h
    [SerializeField]
    private float textSpeed = 0.05f;
    //�o�ߎ���
    private float elapsedTime = 0f;
    //�����Ă��镶���ԍ�
    private int nowTextNumber = 0;
    //�}�E�X�N���b�N�𑣂��A�C�R��
    [SerializeField]
    private Image clickIcon = null;
    //�N���b�N�A�C�R���̓_�ŕb��
    [SerializeField]
    private float clickFlashTime = 0.2f;
    //1�񕪂̃��b�Z�[�W��\���������ǂ���
    private bool isOneMessage = false;
    //���b�Z�[�W��S�ĕ\���������ǂ���
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
        //���b�Z�[�W���I����Ă��邩�A����ȍ~���b�Z�[�W���Ȃ��̂Ȃ炱�̌�͉������Ȃ�
        if (isEndMessage || allMessage == null)
        {
            return;
        }

        //1��ɕ\�����郁�b�Z�[�W��\�����Ă��Ȃ�
        if (!isOneMessage)
        {
            //�e�L�X�g�W�����Ԃ��o�߂����烁�b�Z�[�W��ǉ�
            if (elapsedTime >= textSpeed)
            {
                messageText.text += splitMessage[messageNumber][nowTextNumber];

                nowTextNumber++;
                elapsedTime = 0f;

                //���b�Z�[�W�����ׂĕ\���A�܂��͍s�����ő���\�����ꂽ
                if (nowTextNumber >= splitMessage[messageNumber].Length)
                {
                    isOneMessage = true;
                }
            }
            elapsedTime += Time.deltaTime;

            //���b�Z�[�W�\�����Ƀ}�E�X���N���b�N������c���S���\��
            if (Input.GetButtonDown("Action"))
            {
                //�����܂łɕ\�����Ă���e�L�X�g�Ɏc��̃��b�Z�[�W�𑫂�
                messageText.text += splitMessage[messageNumber].Substring(nowTextNumber);
                isOneMessage = true;
            }
        }
        else
        {
            //1��ɕ\�����郁�b�Z�[�W��\������
            elapsedTime += Time.deltaTime;

            //�N���b�N�A�C�R����_�ł��鎞�Ԃ𒴂������A���]������
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

                //���b�Z�[�W�����ׂĕ\������Ă�����A�Q�[���I�u�W�F�N�g(��b���b�Z�[�W�p�l��)���Ԃ��폜
                if (messageNumber >= splitMessage.Length)
                {
                    EndTalking();
                }
            }
        }
    }

    private void LateUpdate()
    {
        //��b���肪����ꍇ��TalkIcon�̈ʒu����b����̓���ɕ\��
        if (conversationPartner != null)
        {
            talkEnableIcon.transform.Find("Panel").position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(conversationPartner.transform.position + Vector3.up * 2f);
        }
    }

    //��b�����ݒ�
    public void SetConversationPartner(GameObject partnerObject)
    {
        talkEnableIcon.SetActive(true);
        conversationPartner = partnerObject;
    }

    //��b��������Z�b�g
    public void ResetConversationPartner(GameObject partnerObject)
    {
        //����������b���肪���Ȃ��ꍇ�͉������Ȃ�
        if (conversationPartner == null)
        {
            return;
        }
        //��b����ƈ����Ŏ󂯎�������肪�����C���X�^���XID�����Ȃ��b������Ȃ���
        if (conversationPartner.GetInstanceID() == partnerObject.GetInstanceID())
        {
            talkEnableIcon.SetActive(false);
            conversationPartner = null;
        }
    }

    //��b�����n��(�Ԃ�)(��Getter���\�b�h)
    public GameObject GetConversationPartner()
    {
        return conversationPartner;
    }

    //��b�J�n
    public void StartTalking()
    {
        var villagerScript = conversationPartner.GetComponent<VillagerScript>();
        villagerScript.SetState(VillagerScript.State.Talk, transform);
        this.allMessage = villagerScript.GetConversation().GetConversationMessage();
        //�����������1��ɕ\�����郁�b�Z�[�W�𕪊�����
        splitMessage = Regex.Split(allMessage, @"\s*" + splitString + @"\s*", RegexOptions.IgnorePatternWhitespace);
        //����������
        nowTextNumber = 0;
        messageNumber = 0;
        messageText.text = "";
        talkUI.SetActive(true);
        talkEnableIcon.SetActive(false);
        isOneMessage = false;
        isEndMessage = false;
        //��b�J�n���̓��͂͂������񃊃Z�b�g
        Input.ResetInputAxes();
    } 

    void EndTalking()
    {
        isEndMessage = true;
        talkUI.SetActive(false);
        //�v���C���[�L�����N�^�[�Ƒ��l�����̏�Ԃ�ύX����
        var villagerScript = conversationPartner.GetComponent<VillagerScript>();
        villagerScript.SetState(VillagerScript.State.Wait);
        GetComponent<UnityChanScript>().SetState(UnityChanScript.State.Normal);
        Input.ResetInputAxes();
    }
}
