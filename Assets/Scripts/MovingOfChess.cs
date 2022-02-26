using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋子的移动类
/// </summary>
public class MovingOfChess
{
    private GameManager gameManager;
    //当前深度下对应的着法编号计数器
    private int moveCount;

    //存放所有合法走法的列表   第一个索引即行代表搜索深度，第二个索引即列代表当前深度下对应的着法编号
    public ChessReseting.Chess[,] moveList = new ChessReseting.Chess[8, 80];

    public MovingOfChess(GameManager mGameManager)
    {
        gameManager = mGameManager;
    }

    /// <summary>
    /// 棋子的移动方法
    /// </summary>
    /// <param name="chessGo">要移动的棋子游戏物体</param>
    /// <param name="targetGrid">要移动到的格子游戏物体</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void IsMove(GameObject chessGo, GameObject targetGrid, int x1, int y1, int x2, int y2)
    {
        gameManager.ShowLastPositionUI(chessGo.transform.position);
        chessGo.transform.SetParent(targetGrid.transform);
        chessGo.transform.localPosition = Vector3.zero;
        gameManager.chessBoard[x2, y2] = gameManager.chessBoard[x1, y1];
        gameManager.chessBoard[x1, y1] = 0;
    }
    /// <summary>
    /// 棋子的吃子方法
    /// </summary>
    /// <param name="firstChess">想要移动的棋子</param>
    /// <param name="secondChess">想要吃掉的棋子</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void IsEat(GameObject firstChess, GameObject secondChess, int x1, int y1, int x2, int y2)
    {
        gameManager.ShowLastPositionUI(firstChess.transform.position);
        GameObject secondChessGrid = secondChess.transform.parent.gameObject;//得到了第二个棋子的父对象
        firstChess.transform.SetParent(secondChessGrid.transform);
        firstChess.transform.localPosition = Vector3.zero;
        gameManager.chessBoard[x2, y2] = gameManager.chessBoard[x1, y1];
        gameManager.chessBoard[x1, y1] = 0;
        gameManager.BeEat(secondChess);
    }
    /// <summary>
    /// 判断当前点击到的是什么类型的棋子从而执行相应方法
    /// </summary>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    public void ClickChess(int fromX, int fromY)
    {
        int chessID = gameManager.chessBoard[fromX, fromY];
        switch (chessID)
        {
            case 1://黑将
                GetJiangMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 8://红帅
                GetShuaiMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 2://黑車
            case 9:
                GetJuMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 3://黑马
            case 10:
                GetMaMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 4://黑炮
            case 11:
                GetPaoMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 5://黑士
                GetB_ShiMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 12://红仕
                GetR_ShiMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 6://黑象
            case 13:
                GetXiangMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 7://黑卒
                GetB_BingMove(gameManager.chessBoard, fromX, fromY);
                break;
            case 14://红兵
                GetR_BingMove(gameManager.chessBoard, fromX, fromY);
                break;
            default:
                break;
        }
    }

    #region 得到对应种类的棋子当前可以移动的所有路径
    /// <summary>
    /// 将
    /// </summary>
    /// <param name="position"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    private void GetJiangMove(int[,] position, int fromX, int fromY)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
        }
    }
    /// <summary>
    /// 帅
    /// </summary>
    private void GetShuaiMove(int[,] position, int fromX, int fromY)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
        }
    }
    /// <summary>
    /// 红黑車
    /// </summary>
    /// <param name="position"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    private void GetJuMove(int[,] position, int fromX, int fromY)
    {
        int x, y;
        int chessID;
        //得到当前选中棋子的ID，目的是为了遍历时判断第一个不为空格子的棋子跟我们是否是同一边
        chessID = position[fromX, fromY];
        //右
        x = fromX;
        y = fromY + 1;
        while (y < 9)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
                break;
            }
            y++;
        }
        //左
        x = fromX;
        y = fromY - 1;
        while (y >= 0)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
                break;
            }
            y--;
        }
        //下
        x = fromX + 1;
        y = fromY;
        while (x < 10)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
                break;
            }
            x++;
        }
        //上
        x = fromX - 1;
        y = fromY;
        while (x >= 0)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
                break;
            }
            x--;
        }
    }
    /// <summary>
    /// 红黑马
    /// </summary>
    private void GetMaMove(int[,] position, int fromX, int fromY)
    {
        int x, y;
        //竖日
        //右下
        x = fromX + 2;
        y = fromY + 1;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //右上
        x = fromX - 2;
        y = fromY + 1;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //左下
        x = fromX + 2;
        y = fromY - 1;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //左上
        x = fromX - 2;
        y = fromY - 1;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //横日
        //右下
        x = fromX + 1;
        y = fromY + 2;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //右上
        x = fromX - 1;
        y = fromY + 2;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //左下
        x = fromX + 1;
        y = fromY - 2;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //左上
        x = fromX - 1;
        y = fromY - 2;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
    }
    /// <summary>
    /// 红黑炮
    /// </summary>
    private void GetPaoMove(int[,] position, int fromX, int fromY)
    {
        int x, y;
        bool flag;//是否满足翻山的条件
        int chessID;
        chessID = position[fromX, fromY];
        //右
        x = fromX;
        y = fromY + 1;
        flag = false;
        while (y < 9)
        {
            //是空格子
            if (position[x, y] == 0)
            {
                //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                //不可空翻
                if (!flag)
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
            //是棋子
            else
            {
                //条件未满足时，开启条件的满足，可翻山
                if (!flag)
                {
                    flag = true;
                }
                //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                //如果不是，则此子可吃，即可移动到此位置，则需显示
                //结束当前遍历
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, fromX, fromY, x, y);
                    }
                    break;
                }
            }
            y++;
        }
        //左
        y = fromY - 1;
        flag = false;
        while (y >= 0)
        {
            if (position[x, y] == 0)
            {
                if (!flag)
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
            else
            {
                if (!flag)
                {
                    flag = true;
                }
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, fromX, fromY, x, y);
                    }
                    break;
                }
            }
            y--;
        }
        //下
        x = fromX + 1;
        y = fromY;
        flag = false;
        while (x < 10)
        {
            //是空格子
            if (position[x, y] == 0)
            {
                //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                //不可空翻
                if (!flag)
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
            //是棋子
            else
            {
                //条件未满足时，开启条件的满足，可翻山
                if (!flag)
                {
                    flag = true;
                }
                //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                //如果不是，则此子可吃，即可移动到此位置，则需显示
                //结束当前遍历
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, fromX, fromY, x, y);
                    }
                    break;
                }
            }
            x++;
        }
        //上
        x = fromX - 1;
        flag = false;
        while (x >= 0)
        {
            //是空格子
            if (position[x, y] == 0)
            {
                //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                //不可空翻
                if (!flag)
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
            //是棋子
            else
            {
                //条件未满足时，开启条件的满足，可翻山
                if (!flag)
                {
                    flag = true;
                }
                //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                //如果不是，则此子可吃，即可移动到此位置，则需显示
                //结束当前遍历
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        GetCanMovePos(position, fromX, fromY, x, y);
                    }
                    break;
                }
            }
            x--;
        }
    }
    /// <summary>
    /// 黑士
    /// </summary>
    private void GetB_ShiMove(int[,] position, int fromX, int fromY)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
        }
    }
    /// <summary>
    /// 红仕
    /// </summary>
    private void GetR_ShiMove(int[,] position, int fromX, int fromY)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    GetCanMovePos(position, fromX, fromY, x, y);
                }
            }
        }
    }
    /// <summary>
    /// 红相黑象
    /// </summary>
    private void GetXiangMove(int[,] position, int fromX, int fromY)
    {
        int x, y;
        //右下走
        x = fromX + 2;
        y = fromY + 2;
        if (x < 10 && y < 9 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //右上走
        x = fromX - 2;
        y = fromY + 2;
        if (x >= 0 && y < 9 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //左下走
        x = fromX + 2;
        y = fromY - 2;
        if (x < 10 && y >= 0 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //左上走
        x = fromX - 2;
        y = fromY - 2;
        if (x >= 0 && y >= 0 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
    }
    /// <summary>
    /// 黑卒
    /// </summary>
    private void GetB_BingMove(int[,] position, int fromX, int fromY)
    {
        int x, y;
        int chessID;
        chessID = position[fromX, fromY];
        x = fromX + 1;
        y = fromY;
        if (x < 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //过河后
        if (fromX > 4)
        {
            x = fromX;
            y = fromY + 1;//右边
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
            y = fromY - 1;//左边
            if (y >= 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
        }
    }
    /// <summary>
    /// 红兵
    /// </summary>
    private void GetR_BingMove(int[,] position, int fromX, int fromY)
    {
        int x, y;
        int chessID;
        chessID = position[fromX, fromY];
        x = fromX - 1;
        y = fromY;
        if (x > 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            GetCanMovePos(position, fromX, fromY, x, y);
        }
        //过河后
        if (fromX < 5)
        {
            x = fromX;
            y = fromY + 1;//右边
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
            y = fromY - 1;//左边
            if (y >= 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                GetCanMovePos(position, fromX, fromY, x, y);
            }
        }
    }
    #endregion
    #region 添加对应种类的棋子当前可以移动的所有着法到着法列表
    /// <summary>
    /// 将
    /// </summary>
    /// <param name="position"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    private void GetJiangMove(int[,] position, int fromX, int fromY, int depth)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
        }
    }
    /// <summary>
    /// 帅
    /// </summary>
    private void GetShuaiMove(int[,] position, int fromX, int fromY, int depth)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
        }
    }
    /// <summary>
    /// 红黑車
    /// </summary>
    /// <param name="position"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    private void GetJuMove(int[,] position, int fromX, int fromY, int depth)
    {
        int x, y;
        int chessID;
        //得到当前选中棋子的ID，目的是为了遍历时判断第一个不为空格子的棋子跟我们是否是同一边
        chessID = position[fromX, fromY];
        //右
        x = fromX;
        y = fromY + 1;
        while (y < 9)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
                break;
            }
            y++;
        }
        //左
        x = fromX;
        y = fromY - 1;
        while (y >= 0)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
                break;
            }
            y--;
        }
        //下
        x = fromX + 1;
        y = fromY;
        while (x < 10)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
                break;
            }
            x++;
        }
        //上
        x = fromX - 1;
        y = fromY;
        while (x >= 0)
        {
            if (position[x, y] == 0)//当前遍历到的位置ID是否为0（即空格子）
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
            else//不为空格子
            {
                if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
                break;
            }
            x--;
        }
    }
    /// <summary>
    /// 红黑马
    /// </summary>
    private void GetMaMove(int[,] position, int fromX, int fromY, int depth)
    {
        int x, y;
        //竖日
        //右下
        x = fromX + 2;
        y = fromY + 1;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //右上
        x = fromX - 2;
        y = fromY + 1;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //左下
        x = fromX + 2;
        y = fromY - 1;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //左上
        x = fromX - 2;
        y = fromY - 1;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //横日
        //右下
        x = fromX + 1;
        y = fromY + 2;
        if ((x < 10 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //右上
        x = fromX - 1;
        y = fromY + 2;
        if ((x >= 0 && y < 9) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //左下
        x = fromX + 1;
        y = fromY - 2;
        if ((x < 10 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //左上
        x = fromX - 1;
        y = fromY - 2;
        if ((x >= 0 && y >= 0) && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
    }
    /// <summary>
    /// 红黑炮
    /// </summary>
    private void GetPaoMove(int[,] position, int fromX, int fromY, int depth)
    {
        int x, y;
        bool flag;//是否满足翻山的条件
        int chessID;
        chessID = position[fromX, fromY];
        //右
        x = fromX;
        y = fromY + 1;
        flag = false;
        while (y < 9)
        {
            //是空格子
            if (position[x, y] == 0)
            {
                //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                //不可空翻
                if (!flag)
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
            //是棋子
            else
            {
                //条件未满足时，开启条件的满足，可翻山
                if (!flag)
                {
                    flag = true;
                }
                //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                //如果不是，则此子可吃，即可移动到此位置，则需显示
                //结束当前遍历
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, fromX, fromY, x, y, depth);
                    }
                    break;
                }
            }
            y++;
        }
        //左
        y = fromY - 1;
        flag = false;
        while (y >= 0)
        {
            if (position[x, y] == 0)
            {
                if (!flag)
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
            else
            {
                if (!flag)
                {
                    flag = true;
                }
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, fromX, fromY, x, y, depth);
                    }
                    break;
                }
            }
            y--;
        }
        //下
        x = fromX + 1;
        y = fromY;
        flag = false;
        while (x < 10)
        {
            //是空格子
            if (position[x, y] == 0)
            {
                //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                //不可空翻
                if (!flag)
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
            //是棋子
            else
            {
                //条件未满足时，开启条件的满足，可翻山
                if (!flag)
                {
                    flag = true;
                }
                //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                //如果不是，则此子可吃，即可移动到此位置，则需显示
                //结束当前遍历
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, fromX, fromY, x, y, depth);
                    }
                    break;
                }
            }
            x++;
        }
        //上
        x = fromX - 1;
        flag = false;
        while (x >= 0)
        {
            //是空格子
            if (position[x, y] == 0)
            {
                //在未达成翻山条件前，显示所有可以移动的路径，达成之后
                //不可空翻
                if (!flag)
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
            //是棋子
            else
            {
                //条件未满足时，开启条件的满足，可翻山
                if (!flag)
                {
                    flag = true;
                }
                //已开启，判断当前是否为同一方，如果是，此位置不可以移动
                //如果不是，则此子可吃，即可移动到此位置，则需显示
                //结束当前遍历
                else
                {
                    if (!gameManager.rules.IsSameSide(chessID, position[x, y]))
                    {
                        AddMove(position, fromX, fromY, x, y, depth);
                    }
                    break;
                }
            }
            x--;
        }
    }
    /// <summary>
    /// 黑士
    /// </summary>
    private void GetB_ShiMove(int[,] position, int fromX, int fromY, int depth)
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
        }
    }
    /// <summary>
    /// 红仕
    /// </summary>
    private void GetR_ShiMove(int[,] position, int fromX, int fromY, int depth)
    {
        for (int x = 7; x < 10; x++)
        {
            for (int y = 3; y < 6; y++)
            {
                if (gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
                {
                    AddMove(position, fromX, fromY, x, y, depth);
                }
            }
        }
    }
    /// <summary>
    /// 红相黑象
    /// </summary>
    private void GetXiangMove(int[,] position, int fromX, int fromY, int depth)
    {
        int x, y;
        //右下走
        x = fromX + 2;
        y = fromY + 2;
        if (x < 10 && y < 9 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //右上走
        x = fromX - 2;
        y = fromY + 2;
        if (x >= 0 && y < 9 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //左下走
        x = fromX + 2;
        y = fromY - 2;
        if (x < 10 && y >= 0 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //左上走
        x = fromX - 2;
        y = fromY - 2;
        if (x >= 0 && y >= 0 && gameManager.rules.IsValidMove(position, fromX, fromY, x, y))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
    }
    /// <summary>
    /// 黑卒
    /// </summary>
    private void GetB_BingMove(int[,] position, int fromX, int fromY, int depth)
    {
        int x, y;
        int chessID;
        chessID = position[fromX, fromY];
        x = fromX + 1;
        y = fromY;
        if (x < 10 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //过河后
        if (fromX > 4)
        {
            x = fromX;
            y = fromY + 1;//右边
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
            y = fromY - 1;//左边
            if (y >= 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
        }
    }
    /// <summary>
    /// 红兵
    /// </summary>
    private void GetR_BingMove(int[,] position, int fromX, int fromY, int depth)
    {
        int x, y;
        int chessID;
        chessID = position[fromX, fromY];
        x = fromX - 1;
        y = fromY;
        if (x > 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
        {
            AddMove(position, fromX, fromY, x, y, depth);
        }
        //过河后
        if (fromX < 5)
        {
            x = fromX;
            y = fromY + 1;//右边
            if (y < 9 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
            y = fromY - 1;//左边
            if (y >= 0 && !gameManager.rules.IsSameSide(chessID, position[x, y]))
            {
                AddMove(position, fromX, fromY, x, y, depth);
            }
        }
    }
    #endregion
    /// <summary>
    /// 把传递进来的一个可移动路径显示出来
    /// </summary>
    /// <param name="positon"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <param name="toX"></param>
    /// <param name="toY"></param>
    private void GetCanMovePos(int[,] positon, int fromX, int fromY, int toX, int toY)
    {
        if (!gameManager.rules.KingKill(positon, fromX, fromY, toX, toY))
        {
            return;
        }
        GameObject item;
        if (positon[toX, toY] == 0)//是空格子，可移动的位置
        {
            item = gameManager.PopCanMoveUI();
        }
        else//是棋子,代表此棋子可吃
        {
            item = gameManager.canEatPosUIGo;
        }
        item.transform.SetParent(gameManager.boardGrid[toX, toY].transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;
    }
    /// <summary>
    /// AI具体走棋的方法
    /// </summary>
    public void HaveAGoodMove(ChessReseting.Chess aChessStep)
    {
        if (aChessStep.chessTwo == null)//走子
        {
            gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.chessOneID, aChessStep.chessTwoID);
            IsMove(aChessStep.chessOne, aChessStep.gridTwo,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex);
        }
        else//吃子
        {
            gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.chessOneID, aChessStep.chessTwoID);
            IsEat(aChessStep.chessOne, aChessStep.chessTwo,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridOne.GetComponent<ChessOrGrid>().yIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().xIndex,
                aChessStep.gridTwo.GetComponent<ChessOrGrid>().yIndex);
        }
    }
    /// <summary>
    /// 产生当前局面所有棋子可能移动着法的方法
    /// </summary>
    /// <param name="positon">当前棋盘状况</param>
    /// <param name="depth">当期搜索深度</param>
    /// <param name="side">当前是哪方视角,如果是false时则为AI层即奇数层，如果是true时则为玩家层即偶数层</param>
    /// <returns></returns>
    public int CreatePossibleMove(int[,] positon, int depth, bool side)
    {
        int chessID;
        moveCount = 0;
        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (positon[i, j] != 0)
                {
                    chessID = positon[i, j];
                    if (!side && gameManager.rules.IsRed(chessID))
                    {
                        //偶数层AI产生黑棋走法跳过红棋
                        continue;
                    }
                    if (side && gameManager.rules.IsBlack(chessID))
                    {
                        //奇数层玩家产生红棋走法跳过黑棋
                        continue;
                    }
                    switch (chessID)
                    {
                        case 1://黑将
                            GetJiangMove(positon, i, j, depth);
                            break;
                        case 8://红帅
                            GetShuaiMove(positon, i, j, depth);
                            break;
                        case 2://黑車
                        case 9:
                            GetJuMove(positon, i, j, depth);
                            break;
                        case 3://黑马
                        case 10:
                            GetMaMove(positon, i, j, depth);
                            break;
                        case 4://黑炮
                        case 11:
                            GetPaoMove(positon, i, j, depth);
                            break;
                        case 5://黑士
                            GetB_ShiMove(positon, i, j, depth);
                            break;
                        case 12://红仕
                            GetR_ShiMove(positon, i, j, depth);
                            break;
                        case 6://黑象
                        case 13:
                            GetXiangMove(positon, i, j, depth);
                            break;
                        case 7://黑卒
                            GetB_BingMove(positon, i, j, depth);
                            break;
                        case 14://红兵
                            GetR_BingMove(positon, i, j, depth);
                            break;
                        default:
                            break;
                    }
                }

            }
        }
        return moveCount;
    }
    /// <summary>
    /// 当时AI行走时，把所有可能的着法插入着法列表
    /// </summary>
    /// <param name="position"></param>
    /// <param name="fromX"></param>
    /// <param name="fromY"></param>
    /// <param name="toX"></param>
    /// <param name="toY"></param>
    /// <param name="depth"></param>
    private void AddMove(int[,] position, int fromX, int fromY, int toX, int toY, int depth)
    {
        //当前将帅不在一条直线上
        if (gameManager.rules.KingKill(position, fromX, fromY, toX, toY))
        {
            //把当前着法存进着法列表
            moveList[depth, moveCount].from.x = fromX;
            moveList[depth, moveCount].from.y = fromY;
            moveList[depth, moveCount].to.x = toX;
            moveList[depth, moveCount].to.y = toY;
            moveCount++;
            //gameManager.searchEngine.AddHistoryScore(moveList[depth, moveCount],depth);
        }
    }
}







