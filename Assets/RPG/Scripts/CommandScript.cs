using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandScript : MonoBehaviour
{
    public enum CommandMode
    {
        CommandPanel,
        StatusPanelSelectCharacter,
        StatusPanel
    }

    private CommandMode currentCommand;
    //�v���C���[�L�����R�}���h�X�N���v�g
    private UnityChanCommandScript unityChanCommandScript;

    //�ŏ��ɑI������Button��Transform
    private GameObject firstSelectButton;

    //�R�}���h�p�l��
    private GameObject commandPanel;
    //�X�e�[�^�X�\���p�l��
    private GameObject statusPanel;
    //�L�����N�^�[�I���p�l��
    private GameObject selectCharacterPanel;

    //�R�}���h�p�l����CanvasGroup
    private CanvasGroup commandPanelCanvasGroup;
    //�L�����I���p�l����CanvasGroup
    private CanvasGroup selectCharacterPanelCanvasGroup;

    //�L������
    private Text characterNameText;
    //�X�e�[�^�X�^�C�g���e�L�X�g
    private Text statusTitleText;
    //�X�e�[�^�X�p�����[�^�e�L�X�g�P
    private Text statusParamater1Text;
    //�X�e�[�^�X�p�����[�^�e�L�X�g�Q
    private Text statusParamater2Text;
    //�p�[�e�B�X�e�[�^�X(���)
    [SerializeField]
    private PartyStatus partyStatus = null;

    //�L�����I���{�^���̃v���n�u
    [SerializeField]
    private GameObject characterPanelButtonPrefab = null;

    //�Ō�ɑI�����Ă����Q�[���I�u�W�F�N�g���X�^�b�N
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();

    void Awake()
    {
        //�R�}���h��ʂ��J�����������Ă���UnityChanCommandScript���擾
        unityChanCommandScript = GameObject.FindWithTag("Player").GetComponent<UnityChanCommandScript>();
        //���݂̃R�}���h��������
        currentCommand = CommandMode.CommandPanel;
        //�K�w��H���Ď擾
        firstSelectButton = transform.Find("CommandPanel/StatusButton").gameObject;
        //�p�l���n
        commandPanel = transform.Find("CommandPanel").gameObject;
        statusPanel = transform.Find("StatusPanel").gameObject;
        selectCharacterPanel = transform.Find("SelectCharacterPanel").gameObject;

        //CanvasGroup
        commandPanelCanvasGroup = commandPanel.GetComponent<CanvasGroup>();
        selectCharacterPanelCanvasGroup = selectCharacterPanel.GetComponent<CanvasGroup>();

        //�X�e�[�^�X�p�e�L�X�g
        characterNameText = statusPanel.transform.Find("CharacterNamePanel/Text").GetComponent<Text>();
        statusTitleText = statusPanel.transform.Find("StatusParamaterPanel/Title").GetComponent<Text>();
        statusParamater1Text = statusPanel.transform.Find("StatusParamaterPanel/Paramater1").GetComponent<Text>();
        statusParamater2Text = statusPanel.transform.Find("StatusParamaterPanel/Paramater2").GetComponent<Text>();
    }

    private void OnEnable()
    {
        currentCommand = CommandMode.CommandPanel;

        statusPanel.SetActive(false);
        selectCharacterPanel.SetActive(false);

        for(int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
        }

        selectedGameObjectStack.Clear();

        commandPanelCanvasGroup.interactable = true;
        selectCharacterPanelCanvasGroup.interactable = false;
        EventSystem.current.SetSelectedGameObject(firstSelectButton);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (currentCommand == CommandMode.CommandPanel)
            {
                unityChanCommandScript.ExitCommand();
                gameObject.SetActive(false);
            }else if (currentCommand == CommandMode.StatusPanelSelectCharacter || currentCommand == CommandMode.StatusPanel)
            {
                selectCharacterPanelCanvasGroup.interactable = false;
                selectCharacterPanel.SetActive(false);
                statusPanel.SetActive(false);

                for(int i = selectCharacterPanel.transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(selectCharacterPanel.transform.GetChild(i).gameObject);
                }

                EventSystem.current.SetSelectedGameObject(selectedGameObjectStack.Pop());
                commandPanelCanvasGroup.interactable = true;
                currentCommand = CommandMode.CommandPanel;
            }
        }
    }

    //�I�������R�}���h�ŏ�������
    public void SelectCommand(string command)
    {
        if (command == "Status")
        {
            currentCommand = CommandMode.StatusPanelSelectCharacter;
            //UI�̃I���E�I�t��I���A�C�R���̐ݒ�
            commandPanelCanvasGroup.interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            GameObject characterButtonIns;

            //�p�[�e�B�[�����o�[���̃{�^�����쐬
            foreach(var member in partyStatus.GetAllyStatus())
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, selectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponent<Button>().onClick.AddListener(() => ShowStatus(member));
            }
        }

        //�K�w����ԍŌ�ɕ��בւ�
        selectCharacterPanel.transform.SetAsLastSibling();
        selectCharacterPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.transform.GetChild(0).gameObject);
    }

    //�L�����̃X�e�[�^�X��\��
    public void ShowStatus(AllyStatus allyStatus)
    {
        currentCommand = CommandMode.StatusPanel;
        statusPanel.SetActive(true);
        //�L��������\��
        characterNameText.text = allyStatus.GetCharacterName();

        //�^�C�g���̕\��
        var text = "���x��\n";
        text += "HP\n";
        text += "SP\n";
        text += "�o���l\n";
        text += "STR\n";
        text += "AGI\n";
        text += "VIT\n";
        text += "DEX\n";
        text += "INT\n";
        text += "LUK\n";
        text += "��������\n";
        text += "�����Z\n";
        text += "�U����\n";
        text += "�h���\n";
        statusTitleText.text = text;

        //HP��SP��Division�L���̕\��
        text = "\n";
        text += allyStatus.GetHP() + "\n";
        text += allyStatus.GetSP() + "\n";
        statusParamater1Text.text = text;

        //�X�e�[�^�X�p�����[�^�̕\��
        text = allyStatus.GetLevel() + "\n";
        text += allyStatus.GetMaxHP() + "\n";
        text += allyStatus.GetMaxSP() + "\n";
        text += allyStatus.get_earned_EXP() + "\n";
        text += allyStatus.GetSTR() + "\n";
        text += allyStatus.GetAGI() + "\n";
        text += allyStatus.GetVIT() + "\n";
        text += allyStatus.GetDEX() + "\n";
        text += allyStatus.GetINT() + "\n";
        text += allyStatus.GetLUK() + "\n";
        text += allyStatus?.get_equiped_weapon()?.GetItemName() ?? "";
        text += "\n";
        text += allyStatus.get_equiped_armor()?.GetItemName() ?? "";
        text += "\n";
        text += allyStatus.GetSTR() + (allyStatus.get_equiped_weapon()?.GetAmount() ?? 0) + "\n";
        text += allyStatus.GetVIT() + (allyStatus.get_equiped_armor()?.GetAmount() ?? 0) + "\n";
        statusParamater2Text.text = text;
    }
}
