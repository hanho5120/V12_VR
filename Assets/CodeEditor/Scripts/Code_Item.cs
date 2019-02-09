using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Code_Item
{
    public string CmdType = string.Empty;
    public string CmdName = string.Empty;
    public string CmdValue = string.Empty;

    public bool ValueYN = true;
    public bool OptionYN = false;
    public bool VariableYN = false;

    public string CmdVariable = string.Empty;

    public string Id = string.Empty;

    //옵션을 처리하기 위해 ChildLIst가 사용된다.
    public List<Code_Item> ChildList = new List<Code_Item>();

    public Code_Item ParentItem = null;
    public int IndentationLevel = 0;

    public float Width = 400;
    public float Height = 40;


    //Blockly
    //2018.01.21
    //현재 명령어가 블록 명령어 인지 체크
    public bool BlockYN = false;

    //CmdType -> BLOCK_START, BLOCK_END

    public bool BracketOpenFlag = false;
    public bool BracketCloseFlag = false;

    //자식아이템은 아래 BlockStartItem에 연결된다
    public Code_Item BlockStartItem = null;

    //블록 이후의 명령어들은 BlockEndItem에 형제로 연결된다.
    public Code_Item BlockEndItem = null;

    public int ChildBlockCmdCount = 0;
    public int ChildSingleLineCmdCount = 0;

    public Code_Item PreCmdItem = null;
    public Code_Item NextCmdItem = null;



    public int Bg_Color_Index = 0;


    public Code_Item(string cmd_type, string cmd_name, string cmd_value, Code_Item parent_item)
    {
        CmdType = cmd_type;
        CmdName = cmd_name;
        CmdValue = cmd_value;
        ParentItem = parent_item;
        Id = System.Guid.NewGuid().ToString();
    }


    public Code_Item Clone()
    {
        Code_Item new_instance = new Code_Item(this.CmdType, this.CmdName, this.CmdValue, this.ParentItem);

        new_instance.BlockEndItem = this.BlockEndItem;
        new_instance.BlockStartItem = this.BlockStartItem;
        new_instance.BlockYN = this.BlockYN;
        new_instance.BracketCloseFlag = this.BracketCloseFlag;
        new_instance.BracketOpenFlag = this.BracketOpenFlag;
        new_instance.ChildBlockCmdCount = this.ChildBlockCmdCount;
        new_instance.CmdVariable = this.CmdVariable;
        new_instance.Height = this.Height;
        new_instance.IndentationLevel = this.IndentationLevel;
        new_instance.NextCmdItem = this.NextCmdItem;
        new_instance.OptionYN = this.OptionYN;
        new_instance.PreCmdItem = this.PreCmdItem;
        new_instance.ValueYN = this.ValueYN;
        new_instance.VariableYN = this.VariableYN;
        new_instance.Width = this.Width;
        new_instance.Bg_Color_Index = this.Bg_Color_Index;

        for (int i = 0; i < this.ChildList.Count; i++)
        {
            new_instance.ChildList.Add(this.ChildList[i]);
        }

        return new_instance;
    }


    //2018.01.21
    public void CalcChildCmdCount(bool full_count_mode)
    {
        this.ChildBlockCmdCount = 0;
        this.ChildSingleLineCmdCount = 0;


        //자식 갯수 계산
        if (this.BlockYN)
        {
            this.ChildBlockCmdCount++;

            if (this.BlockStartItem != null && this.BlockStartItem.NextCmdItem != null)
            {
                this.BlockStartItem.NextCmdItem.CalcChildCmdSizeInternal(this);
            }
        }
        else
            this.ChildSingleLineCmdCount++;


        if (full_count_mode)
        {
            //형제 객체
            if (this.NextCmdItem != null)
            {
                this.NextCmdItem.CalcChildCmdSizeInternal(this);
            }
        }
    }

    //2018.01.21
    public void CalcChildCmdSizeInternal(Code_Item parent_item)
    {
        if (this.BlockYN)
            parent_item.ChildBlockCmdCount++;
        else
            parent_item.ChildSingleLineCmdCount++;

        //자식 먼저
        if (this.BlockStartItem != null && this.BlockStartItem.NextCmdItem != null)
        {
            this.BlockStartItem.NextCmdItem.CalcChildCmdSizeInternal(parent_item);
        }

        //형제 객체
        if (this.NextCmdItem != null)
        {
            this.NextCmdItem.CalcChildCmdSizeInternal(parent_item);
        }
    }

}
