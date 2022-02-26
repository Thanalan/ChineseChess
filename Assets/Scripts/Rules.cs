using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋子的规则类
/// </summary>
public class Rules
{

    /// <summary>
    /// 检查当前此次移动是否合法
    /// </summary>
    /// <param name="position">当前棋盘的状况</param>
    /// <param name="FromX">来的位置X索引</param>
    /// <param name="FromY">来的位置Y索引</param>
    /// <param name="ToX">去的位置X索引</param>
    /// <param name="ToY">去的位置Y索引</param>
    /// <returns></returns>
    public bool IsValidMove(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        int moveChessID, targetID;
        moveChessID = position[FromX, FromY];
        targetID = position[ToX, ToY];
        if (IsSameSide(moveChessID, targetID))
        {
            return false;
        }
        return IsVaild(moveChessID, position, FromX, FromY, ToX, ToY);
    }
    /// <summary>
    /// 判断选中的两个游戏物体是否同为空格，同为红棋或者同为黑棋
    /// </summary>
    /// <returns></returns>
    public bool IsSameSide(int x, int y)
    {
        if (IsBlack(x) && IsBlack(y) || IsRed(x) && IsRed(y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 判断当前游戏物体是否是黑棋
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public bool IsBlack(int x)
    {
        if (x > 0 && x < 8)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 判断当前游戏物体是否是红棋
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public bool IsRed(int x)
    {
        if (x >= 8 && x < 15)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 所有种类棋子的走法是否合法
    /// </summary>
    /// <param name="moveChessID"></param>
    /// <param name="postion"></param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="ToX"></param>
    /// <param name="ToY"></param>
    public bool IsVaild(int moveChessID, int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        //目的地与原位置相同
        if (FromX == ToX && FromY == ToY)
        {
            return false;
        }
        //将帅是否在同一直线上
        if (!KingKill(position, FromX, FromY, ToX, ToY))
        {
            return false;
        }
        int i = 0, j = 0;
        switch (moveChessID)
        {
            //分红黑棋子处理的情况
            case 1://黑将
                //出九宫格
                if (ToX > 2 || ToY > 5 || ToY < 3)
                {
                    return false;
                }
                //横纵移动只能是一个单元格
                if ((Mathf.Abs(ToX - FromX) + Mathf.Abs(ToY - FromY)) > 1)
                {
                    return false;
                }
                break;
            case 8://红帅
                //出九宫格
                if (ToX < 7 || ToY > 5 || ToY < 3)
                {
                    return false;
                }
                //横纵移动只能是一个单元格
                if ((Mathf.Abs(ToX - FromX) + Mathf.Abs(ToY - FromY)) > 1)
                {
                    return false;
                }
                break;
            case 5://黑士
                //出九宫格
                if (ToX > 2 || ToY > 5 || ToY < 3)
                {
                    return false;
                }
                //士走斜线
                if (Mathf.Abs(FromX - ToX) != 1 || Mathf.Abs(FromY - ToY) != 1)
                {
                    return false;
                }
                break;
            case 12://红仕
                //出九宫格
                if (ToX < 7 || ToY > 5 || ToY < 3)
                {
                    return false;
                }
                //士走斜线
                if (Mathf.Abs(FromX - ToX) != 1 || Mathf.Abs(FromY - ToY) != 1)
                {
                    return false;
                }
                break;
            case 6://黑象
                //象不能过河
                if (ToX > 4)
                {
                    return false;
                }
                //象走田
                if (Mathf.Abs(FromX - ToX) != 2 || Mathf.Abs(FromY - ToY) != 2)
                {
                    return false;
                }
                //塞象眼
                if (position[(FromX + ToX) / 2, (FromY + ToY) / 2] != 0)
                {
                    return false;
                }
                break;
            case 13://红相
                //象不能过河
                if (ToX < 5)
                {
                    return false;
                }
                //象走田
                if (Mathf.Abs(FromX - ToX) != 2 || Mathf.Abs(FromY - ToY) != 2)
                {
                    return false;
                }
                //塞象眼
                if (position[(FromX + ToX) / 2, (FromY + ToY) / 2] != 0)
                {
                    return false;
                }
                break;
            case 7://黑卒
                //兵不回头
                if (ToX < FromX)
                {
                    return false;
                }
                //兵过河前只能走竖线
                if (FromX < 5 && FromX == ToX)
                {
                    return false;
                }
                //兵只能走一格
                if (ToX - FromX + Mathf.Abs(ToY - FromY) > 1)
                {
                    return false;
                }
                break;
            case 14://红兵
                //兵不回头
                if (ToX > FromX)
                {
                    return false;
                }
                //兵过河前只能走竖线
                if (FromX > 4 && FromX == ToX)
                {
                    return false;
                }
                //兵只能走一格
                if (FromX - ToX + Mathf.Abs(ToY - FromY) > 1)
                {
                    return false;
                }
                break;
            //不分红黑棋子处理的情况
            case 2:
            case 9://红黑車
                //車走直线
                if (FromY != ToY && FromX != ToX)
                {
                    return false;
                }
                //判断当前移动路径上是否有其他棋子
                if (FromX == ToX)//走横线
                {
                    if (FromY < ToY)//右走
                    {
                        for (i = FromY + 1; i < ToY; i++)
                        {
                            if (position[FromX, i] != 0)//代表移动路径上有棋子
                            {
                                return false;
                            }
                        }
                    }
                    else//左走
                    {
                        for (i = ToY + 1; i < FromY; i++)
                        {
                            if (position[FromX, i] != 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                else//走竖线
                {
                    if (FromX < ToX)//下走
                    {
                        for (j = FromX + 1; j < ToX; j++)
                        {
                            if (position[j, FromY] != 0)
                            {
                                return false;
                            }
                        }
                    }
                    else//上走
                    {
                        for (j = ToX + 1; j < FromX; j++)
                        {
                            if (position[j, FromY] != 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                break;
            case 3:
            case 10://红黑马
                //马走日字
                //竖日                                                
                if (!((Mathf.Abs(ToY - FromY) == 1 && Mathf.Abs(ToX - FromX) == 2) ||
                //横日    
                    (Mathf.Abs(ToY - FromY) == 2 && Mathf.Abs(ToX - FromX) == 1)))
                {
                    return false;
                }
                //马蹩腿
                if (ToY - FromY == 2)//右横日
                {
                    i = FromY + 1;
                    j = FromX;
                }
                else if (FromY - ToY == 2)//左横日
                {
                    i = FromY - 1;
                    j = FromX;
                }
                else if (ToX - FromX == 2)//下竖日
                {
                    i = FromY;
                    j = FromX + 1;
                }
                else if (FromX - ToX == 2)//上竖日
                {
                    i = FromY;
                    j = FromX - 1;
                }
                if (position[j, i] != 0)
                {
                    return false;
                }
                break;
            case 4:
            case 11://红黑炮
                //炮走直线
                if (FromY != ToY && FromX != ToX)
                {
                    return false;
                }
                //炮是走棋还是翻山吃子
                //炮移动
                if (position[ToX, ToY] == 0)
                {
                    if (FromX == ToX)//炮走横线
                    {
                        if (FromY < ToY)//右走
                        {
                            for (i = FromY + 1; i < ToY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                        else//左走
                        {
                            for (i = ToY + 1; i < FromY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else//炮走竖线
                    {
                        if (FromX < ToX)//下走
                        {
                            for (j = FromX + 1; j < ToX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                        else//上走
                        {
                            for (j = ToX + 1; j < FromX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
                //炮翻山吃子
                else
                {
                    int count = 0;
                    if (FromX == ToX)//走横线
                    {
                        if (FromY < ToY)//右走
                        {
                            for (i = FromY + 1; i < ToY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    count++;
                                }
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                        else//左走
                        {
                            for (i = ToY + 1; i < FromY; i++)
                            {
                                if (position[FromX, i] != 0)
                                {
                                    count++;
                                }
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                    }
                    else//走竖线
                    {
                        if (FromX < ToX)//下走
                        {
                            for (j = FromX + 1; j < ToX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    count++;
                                }
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                        else//上走
                        {
                            for (j = ToX + 1; j < FromX; j++)
                            {
                                if (position[j, FromY] != 0)
                                {
                                    count++;
                                }
                            }
                            if (count != 1)
                            {
                                return false;
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        return true;

    }
    /// <summary>
    /// 判断将帅是否是在同一直线上
    /// </summary>
    /// <param name="position"></param>
    /// <param name="FromX"></param>
    /// <param name="FromY"></param>
    /// <param name="ToX"></param>
    /// <param name="ToY"></param>
    /// <returns></returns>
    public bool KingKill(int[,] position, int FromX, int FromY, int ToX, int ToY)
    {
        int jiangX = 0, jiangY = 0, shuaiX = 0, shuaiY = 0;
        int count = 0;
        //假设的思想
        int[,] position1 = new int[10, 9];
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                position1[i, j] = position[i, j];
            }
        }
        //假设它已经走到了那个位置
        position1[ToX, ToY] = position1[FromX, FromY];
        position1[FromX, FromY] = 0;
        //获取将位置
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (position1[i, j] == 1)
                {
                    jiangX = i;
                    jiangY = j;
                }
            }
        }
        //获取帅位置
        for (int i = 7; i < 10; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (position1[i, j] == 8)
                {
                    shuaiX = i;
                    shuaiY = j;
                }
            }
        }
        if (jiangY == shuaiY)//将帅在一条直线上
        {
            for (int i = jiangX + 1; i < shuaiX; i++)
            {
                if (position1[i, jiangY] != 0)
                {
                    count++;
                }
            }
        }
        else//不在一条直线上
        {
            count = -1;
        }
        if (count == 0)//不合法
        {
            return false;
        }
        //其他移动都合法
        return true;
    }
}









