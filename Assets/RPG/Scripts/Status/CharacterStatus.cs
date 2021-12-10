using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public abstract class CharacterStatus : ScriptableObject
{
    //フィールド
    [SerializeField]
    private string characterName = "";
    [SerializeField]
    private int level = 1;
    [SerializeField]
    private int maxHP = 100;
    [SerializeField]
    private int HP = 100;
    [SerializeField]
    private int maxSP = 100;
    [SerializeField]
    private int SP = 100;
    [SerializeField]
    private int str = 10;
    [SerializeField]
    private int agi = 10;
    [SerializeField]
    private int vit = 10;
    [SerializeField]
    private int dex = 10;
    [SerializeField]
    private int intelligence = 10;
    [SerializeField]
    private int luk = 10;

    public void SetCharacterName(string Name)
    {
        this.characterName = Name;
    }
    public string GetCharacterName()
    {
        return characterName;
    }
    
    public void SetLevel(int characterLevel)
    {
        this.level = characterLevel;
    }
    public int GetLevel()
    {
        return level;
    }

    public void SetMaxHP(int charcterMaxHP)
    {
        this.maxHP = charcterMaxHP;
    }
    public int GetMaxHP()
    {
        return maxHP;
    }

    public void SetHP(int characterHP)
    {
        this.HP = Mathf.Max(0, Mathf.Min(GetMaxHP(), HP));
    }
    public int GetHP()
    {
        return HP;
    }

    public void SetMaxSP(int characterMaxSP)
    {
        this.maxSP = characterMaxSP;
    }
    public int GetMaxSP()
    {
        return maxSP;
    }

    public void SetSP(int characterSP)
    {
        this.SP = Mathf.Max(0, Mathf.Min(GetMaxSP(), SP));
    }
    public int GetSP()
    {
        return SP;
    }

    public void SetSTR(int characterSTR)
    {
        this.str = characterSTR;
    }
    public int GetSTR()
    {
        return str;
    }

    public void SetAGI(int characterAGI)
    {
        this.agi = characterAGI;
    }
    public int GetAGI()
    {
        return agi;
    }

    public void SetVIT(int characterVIT)
    {
        this.vit = characterVIT;
    }
    public int GetVIT()
    {
        return vit;
    }

    public void SetDEX(int characterDEX)
    {
        this.dex = characterDEX;
    }
    public int GetDEX()
    {
        return dex;
    }

    public void SetINT(int characterINT)
    {
        this.intelligence = characterINT;
    }
    public int GetINT()
    {
        return intelligence;
    }
    
    public void SetLUK(int characterLUK)
    {
        this.luk = characterLUK;
    }
    public int GetLUK()
    {
        return luk;
    }
    
}
