using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 棋子或者格子的脚本
/// </summary>
public class ChessOrGrid : MonoBehaviour
{
    //格子索引
    public int xIndex, yIndex;
    //是红棋还是黑棋
    public bool isRed;
    //是否是格子
    public bool isGrid;
    //游戏管理的引用
    private GameManager gameManager;
    //将来移动的时候需要设置的父对象，如果当前对象是棋子，那么需要
    //得到的不是它本身而是它的父对象
    private GameObject gridGo;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gridGo = gameObject;
    }
    /// <summary>
    /// 点击棋子格子时触发的检测方法
    /// </summary>
    public void ClickCheck()
    {
        if (gameManager.gameOver)
        {
            return;
        }
        int itemColorId;
        if (isGrid)
        {
            itemColorId = 0;
        }
        else
        {
            gridGo = transform.parent.gameObject;//得到他的父容器
            ChessOrGrid chessOrGrid = gridGo.GetComponent<ChessOrGrid>();
            xIndex = chessOrGrid.xIndex;
            yIndex = chessOrGrid.yIndex;
            if (isRed)
            {
                itemColorId = 2;
            }
            else
            {
                itemColorId = 1;
            }
        }
        GridOrChessBehavior(itemColorId,xIndex,yIndex);
    }
    /// <summary>
    /// 格子与棋子行为逻辑
    /// </summary>
    /// <param name="itemColorID">格子颜色ID</param>
    /// <param name="x">当前格子的X索引</param>
    /// <param name="y">当前格子的Y索引</param>
    private void GridOrChessBehavior(int itemColorID,int x,int y)
    {
        int FromX, FromY, ToX, ToY;
        gameManager.HideCanEatUI();
        switch (itemColorID)
        {
            //空格子
            case 0:
                gameManager.ClearCurrentCanMoveUIStack();
                ToX = x;
                ToY = y;
                //第一次点到空格子
                if (gameManager.lastChessOrGrid == null)
                {
                    gameManager.lastChessOrGrid = this;
                    return;
                }
                if (gameManager.chessMove)//红色轮次
                {
                    if (gameManager.lastChessOrGrid.isGrid)//上一次点击是否为格子
                    {
                        return;
                    }
                    if (!gameManager.lastChessOrGrid.isRed)//上一次选中是否为黑色
                    {
                        gameManager.lastChessOrGrid = null;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    //当前的移动是否合法
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard,FromX,FromY,ToX,ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    //棋子进行移动
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount,FromX,FromY,ToX,ToY,chessOneID,chessTwoID);
                    gameManager.movingOfChess.IsMove(gameManager.lastChessOrGrid.gameObject,gridGo,FromX,FromY,ToX,ToY);
                    UIManager.Instance.ShowTip("黑方走");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.chessMove = false;
                    gameManager.lastChessOrGrid = this;
                    gameManager.HideClickUI();
                    if (gameManager.gameOver)//游戏结束，AI不需要下棋
                    {
                        return;
                    }
                    if (gameManager.chessPeople==2)//PVP模式，AI不需要下棋
                    {
                        return;
                    }
                    if (!gameManager.chessMove)//黑棋移动轮次
                    {
                        //AI下棋
                        StartCoroutine("Robot");
                    }
                }
                else//黑色轮次
                {
                    if (gameManager.lastChessOrGrid.isGrid)
                    {
                        return;
                    }
                    if (gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = null;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard,FromX,FromY,ToX,ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount, FromX, FromY, ToX, ToY, chessOneID, chessTwoID);
                    gameManager.movingOfChess.IsMove(gameManager.lastChessOrGrid.gameObject,gridGo,FromX,FromY,ToX,ToY);
                    UIManager.Instance.ShowTip("红方走");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.chessMove = true;
                    gameManager.lastChessOrGrid = this;
                    gameManager.HideClickUI();
                }
                break;
            //黑色棋子
            case 1:
                gameManager.ClearCurrentCanMoveUIStack();
                if (!gameManager.chessMove)//黑色轮次
                {
                    FromX = x;
                    FromY = y;
                    gameManager.movingOfChess.ClickChess(FromX,FromY);
                    gameManager.lastChessOrGrid = this;
                    gameManager.ShowClickUI(transform);
                }
                else//红色轮次
                {
                    //红色棋子将要吃黑色棋子
                    if (gameManager.lastChessOrGrid==null)
                    {
                        return;
                    }
                    if (!gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = this;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    ToX = x;
                    ToY = y;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard,FromX,FromY,ToX,ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount, FromX, FromY, ToX, ToY, chessOneID, chessTwoID);
                    gameManager.movingOfChess.IsEat(gameManager.lastChessOrGrid.gameObject,gameObject, FromX, FromY, ToX, ToY);
                    gameManager.chessMove = false;
                    UIManager.Instance.ShowTip("黑色走");
                    gameManager.lastChessOrGrid = null;
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.HideClickUI();
                    if (gameManager.gameOver)//游戏结束，AI不需要下棋
                    {
                        return;
                    }
                    if (gameManager.chessPeople == 2)//PVP模式，AI不需要下棋
                    {
                        return;
                    }
                    if (!gameManager.chessMove)//黑棋移动轮次
                    {
                        //AI下棋
                        StartCoroutine("Robot");
                    }
                }
                break;
            //红色棋子
            case 2:
                gameManager.ClearCurrentCanMoveUIStack();
                if (gameManager.chessMove)//红色轮次
                {
                    FromX = x;
                    FromY = y;
                    gameManager.movingOfChess.ClickChess(FromX, FromY);
                    gameManager.lastChessOrGrid = this;
                    gameManager.ShowClickUI(transform);
                }
                else//黑色轮次
                {
                    //黑吃红
                    if (gameManager.lastChessOrGrid==null)
                    {
                        return;
                    }
                    if (gameManager.lastChessOrGrid.isRed)
                    {
                        gameManager.lastChessOrGrid = this;
                        return;
                    }
                    FromX = gameManager.lastChessOrGrid.xIndex;
                    FromY = gameManager.lastChessOrGrid.yIndex;
                    ToX = x;
                    ToY = y;
                    bool canMove = gameManager.rules.IsValidMove(gameManager.chessBoard,FromX,FromY,ToX,ToY);
                    if (!canMove)
                    {
                        return;
                    }
                    int chessOneID = gameManager.chessBoard[FromX, FromY];
                    int chessTwoID = gameManager.chessBoard[ToX, ToY];
                    gameManager.chessReseting.AddChess(gameManager.chessReseting.resetCount, FromX, FromY, ToX, ToY, chessOneID, chessTwoID);
                    gameManager.movingOfChess.IsEat(gameManager.lastChessOrGrid.gameObject,gameObject,FromX,FromY,ToX,ToY);
                    gameManager.chessMove = true;
                    gameManager.lastChessOrGrid = null;
                    UIManager.Instance.ShowTip("红方走");
                    gameManager.checkmate.JudgeIfCheckmate();
                    gameManager.HideClickUI();
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 开始AI下棋的协程
    /// </summary>
    /// <returns></returns>
    IEnumerator Robot()
    {
        UIManager.Instance.ShowTip("对方正在思考");
        yield return new WaitForSeconds(0.2f);
        RobortMove();
    }
    /// <summary>
    /// AI下棋的方法
    /// </summary>
    private void RobortMove()
    {
        gameManager.movingOfChess.HaveAGoodMove(
            gameManager.searchEngine.SearchAGoodMove(gameManager.chessBoard));
        gameManager.chessMove = true;
        UIManager.Instance.ShowTip("红方走"); ;
        gameManager.checkmate.JudgeIfCheckmate();
    }
}
