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
    //プレイヤーキャラコマンドスクリプト
    private UnityChanCommandScript unityChanCommandScript;

    //最初に選択するButtonのTransform
    private GameObject firstSelectButton;

    //コマンドパネル
    private GameObject commandPanel;
    //ステータス表示パネル
    private GameObject statusPanel;
    //キャラクター選択パネル
    private GameObject selectCharacterPanel;

    //コマンドパネルのCanvasGroup
    private CanvasGroup commandPanelCanvasGroup;
    //キャラ選択パネルのCanvasGroup
    private CanvasGroup selectCharacterPanelCanvasGroup;

    //キャラ名
    private Text characterNameText;
    //ステータスタイトルテキスト
    private Text statusTitleText;
    //ステータスパラメータテキスト１
    private Text statusParamater1Text;
    //ステータスパラメータテキスト２
    private Text statusParamater2Text;
    //パーティステータス(後で)
    [SerializeField]
    private PartyStatus partyStatus = null;

    //キャラ選択ボタンのプレハブ
    [SerializeField]
    private GameObject characterPanelButtonPrefab = null;

    //最後に選択していたゲームオブジェクトをスタック
    private Stack<GameObject> selectedGameObjectStack = new Stack<GameObject>();

    void Awake()
    {
        //コマンド画面を開く処理をしているUnityChanCommandScriptを取得
        unityChanCommandScript = GameObject.FindWithTag("Player").GetComponent<UnityChanCommandScript>();
        //現在のコマンドを初期化
        currentCommand = CommandMode.CommandPanel;
        //階層を辿って取得
        firstSelectButton = transform.Find("CommandPanel/StatusButton").gameObject;
        //パネル系
        commandPanel = transform.Find("CommandPanel").gameObject;
        statusPanel = transform.Find("StatusPanel").gameObject;
        selectCharacterPanel = transform.Find("SelectCharacterPanel").gameObject;

        //CanvasGroup
        commandPanelCanvasGroup = commandPanel.GetComponent<CanvasGroup>();
        selectCharacterPanelCanvasGroup = selectCharacterPanel.GetComponent<CanvasGroup>();

        //ステータス用テキスト
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

    //選択したコマンドで処理分け
    public void SelectCommand(string command)
    {
        if (command == "Status")
        {
            currentCommand = CommandMode.StatusPanelSelectCharacter;
            //UIのオン・オフや選択アイコンの設定
            commandPanelCanvasGroup.interactable = false;
            selectedGameObjectStack.Push(EventSystem.current.currentSelectedGameObject);

            GameObject characterButtonIns;

            //パーティーメンバー分のボタンを作成
            foreach(var member in partyStatus.GetAllyStatus())
            {
                characterButtonIns = Instantiate<GameObject>(characterPanelButtonPrefab, selectCharacterPanel.transform);
                characterButtonIns.GetComponentInChildren<Text>().text = member.GetCharacterName();
                characterButtonIns.GetComponent<Button>().onClick.AddListener(() => ShowStatus(member));
            }
        }

        //階層を一番最後に並べ替え
        selectCharacterPanel.transform.SetAsLastSibling();
        selectCharacterPanel.SetActive(true);
        selectCharacterPanelCanvasGroup.interactable = true;
        EventSystem.current.SetSelectedGameObject(selectCharacterPanel.transform.GetChild(0).gameObject);
    }

    //キャラのステータスを表示
    public void ShowStatus(AllyStatus allyStatus)
    {
        currentCommand = CommandMode.StatusPanel;
        statusPanel.SetActive(true);
        //キャラ名を表示
        characterNameText.text = allyStatus.GetCharacterName();

        //タイトルの表示
        var text = "レベル\n";
        text += "HP\n";
        text += "SP\n";
        text += "経験値\n";
        text += "STR\n";
        text += "AGI\n";
        text += "VIT\n";
        text += "DEX\n";
        text += "INT\n";
        text += "LUK\n";
        text += "装備武器\n";
        text += "装備鎧\n";
        text += "攻撃力\n";
        text += "防御力\n";
        statusTitleText.text = text;

        //HPとSPのDivision記号の表示
        text = "\n";
        text += allyStatus.GetHP() + "\n";
        text += allyStatus.GetSP() + "\n";
        statusParamater1Text.text = text;

        //ステータスパラメータの表示
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
