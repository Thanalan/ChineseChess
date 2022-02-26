using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 检测是否将军的类
/// </summary>
public class Checkmate
{
    private GameManager gameManager;
    private UIManager uiManager;

    private int jiangX, jiangY, shuaiX, shuaiY;

    public Checkmate()
    {
        gameManager = GameManager.Instance;
        uiManager = UIManager.Instance;
    }

    /// <summary>
    /// 是否将军的检测方法
    /// </summary>
    public void JudgeIfCheckmate()
    {
        GetKingPosition();
        //如果从上边方法遍历获取到的索引位置上没有将,将不存在，已经被吃掉了
        if (gameManager.chessBoard[jiangX,jiangY]!=1)
        {
            uiManager.ShowTip("红色棋子胜利");
            gameManager.gameOver = true;
            return;
        }
        //帅不存在，已经被吃掉了
        else if (gameManager.chessBoard[shuaiX,shuaiY]!=8)
        {
            uiManager.ShowTip("黑色棋子胜利");
            gameManager.gameOver = true;
            return;
        }
        //以下是将军的判定
        bool ifCheckmate;//是否将军
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                switch (gameManager.chessBoard[i,j])
                {
                    case 2:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard,i,j,shuaiX,shuaiY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("帅被車将军了");
                        }
                        break;
                    case 3:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("帅被马将军了");
                        }
                        break;
                    case 4:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("帅被炮将军了");
                        }
                        break;
                    case 7:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, shuaiX, shuaiY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("帅被卒将军了");
                        }
                        break;
                    case 9:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("将被車将军了");
                        }
                        break;
                    case 10:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("将被马将军了");
                        }
                        break;
                    case 11:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("将被炮将军了");
                        }
                        break;
                    case 14:
                        ifCheckmate = gameManager.rules.IsValidMove(gameManager.chessBoard, i, j, jiangX, jiangY);
                        if (ifCheckmate)
                        {
                            uiManager.ShowTip("将被兵将军了");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    /// <summary>
    /// 获取将帅坐标位置的方法
    /// </summary>
    private void GetKingPosition()
    {
        //获取黑将的坐标
        for (int i = 0; i < 3; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (gameManager.chessBoard[i,j]==1)
                {
                    jiangX = i;
                    jiangY = j;
                }
            }
        }
        //获取红帅的坐标
        for (int i = 7; i < 10; i++)
        {
            for (int j = 3; j < 6; j++)
            {
                if (gameManager.chessBoard[i,j]==8)
                {
                    shuaiX = i;
                    shuaiY = j;
                }
            }
        }
    }
}
