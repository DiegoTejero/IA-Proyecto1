using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Ai : MonoBehaviour{

    public byte MAX_DEPTH = 3;
    public const int INF= -100;
    public const int MINUS_INF= 100;

    public int totalWhite, totalWhiteQ;
    public int totalRed, totalRedQ;

    public Board board;
    private string activePlayer;

    private Text[,] buttonList;
    private GameController gc;

    public void Awake()
    {
        
    }

    public void SetButtonList(Text[,] bl)
    {
        buttonList = bl;
    }

    public void SetGameController(GameController _gc)
    {
        gc = _gc;
    }

    public void Play(string _actPlayer)
    {

        ScoringMove move;
        activePlayer = _actPlayer;
        ObserveBoard();

        move = Minimax(board, 1);

        //Debug.Log("MI MOVIMIENTO FINAL ha sido :" + move.move.moves[0] + ", " + move.move.moves[1] + " desde : " + move.move.moves[2] + ", " + move.move.moves[3] + " con un score de " + move.score);

        Move(move);
    }

    void ObserveBoard()
    {
        board = new Board(gc.rows, gc.columns, gc);
        byte rows = gc.rows;
        byte columns = gc.columns;
        totalRed = 0;
        totalWhite = 0;
        totalRedQ = 0;
        totalWhiteQ = 0;

        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text spaceText = buttonList[row, column];
                board.spaces[row, column] = spaceText.text;
                //Cuenta las fichas rojas y las damas rojas
                if (buttonList[row, column].text.Contains("X"))
                {
                    if (buttonList[row, column].text == "XX")
                    {
                        totalRedQ++;
                    }
                    totalRed++;
                }
                //Cuenta las fichas blancas y las damas blancas
                else if (buttonList[row, column].text.Contains("O"))
                {
                    if (buttonList[row, column].text == "OO")
                    {
                        totalWhiteQ++;
                    }
                    totalWhite++;
                }
            }
        }
        board.activePlayer = activePlayer;
    }


    public void Move(ScoringMove scoringMove)
    {

        Space space = buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].GetComponentInParent<Space>();
        Space newSpace = buttonList[scoringMove.move.moves[0], scoringMove.move.moves[1]].GetComponentInParent<Space>();

        //En caso de que coma
        if ((scoringMove.move.moves[0] + scoringMove.move.moves[2]) % 2 == 0 && (scoringMove.move.moves[1] + scoringMove.move.moves[3]) % 2 == 0)
        {
            Space middleSpace = buttonList[(scoringMove.move.moves[0] + scoringMove.move.moves[2]) / 2, (scoringMove.move.moves[1] + scoringMove.move.moves[3]) / 2].GetComponentInParent<Space>();

            if (space.transform.GetChild(0).name.Contains("Chip"))
            {
                //Move sprite
                GameObject token = space.transform.GetChild(0).gameObject;
                token.transform.SetParent(newSpace.transform);
                token.transform.localPosition = new Vector3(0, 0, 0);

                if (middleSpace.transform.GetChild(0).name.Contains("Chip"))
                {
                    token = middleSpace.transform.GetChild(0).gameObject;
                    Destroy(token);
                }
                else if (middleSpace.transform.GetChild(1).name.Contains("Chip"))
                {
                    token = middleSpace.transform.GetChild(1).gameObject;
                    Destroy(token);
                }

                

                gc.buttonList[scoringMove.move.moves[0], scoringMove.move.moves[1]].text = gc.buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].text;
                gc.buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].text = "";
                gc.buttonList[(scoringMove.move.moves[0] + scoringMove.move.moves[2]) / 2, (scoringMove.move.moves[1] + scoringMove.move.moves[3]) / 2].text = "";
            }

            else if (space.transform.GetChild(1).name.Contains("Chip"))
            {
                //Move sprite
                GameObject token = space.transform.GetChild(1).gameObject;
                token.transform.SetParent(newSpace.transform);
                token.transform.localPosition = new Vector3(0, 0, 0);

                if (middleSpace.transform.GetChild(0).name.Contains("Chip"))
                {
                    token = middleSpace.transform.GetChild(0).gameObject;
                    Destroy(token);
                }
                else if (middleSpace.transform.GetChild(1).name.Contains("Chip"))
                {
                    token = middleSpace.transform.GetChild(1).gameObject;
                    Destroy(token);
                }

                gc.buttonList[scoringMove.move.moves[0], scoringMove.move.moves[1]].text = gc.buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].text;
                gc.buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].text = "";
                gc.buttonList[(scoringMove.move.moves[0] + scoringMove.move.moves[2]) / 2, (scoringMove.move.moves[1] + scoringMove.move.moves[3]) / 2].text = "";
            }

            
        }
        //En caso de que mueva
        else
        {
            if (space.transform.GetChild(1).name.Contains("Chip"))
            {
                //Move sprite
                GameObject token = space.transform.GetChild(1).gameObject;
                token.transform.SetParent(newSpace.transform);
                token.transform.localPosition = new Vector3(0, 0, 0);

                gc.buttonList[scoringMove.move.moves[0], scoringMove.move.moves[1]].text = gc.buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].text;
                gc.buttonList[scoringMove.move.moves[2], scoringMove.move.moves[3]].text = "";
            }

        }
        gc.UpgradeChip((byte)scoringMove.move.moves[0], (byte)scoringMove.move.moves[1], gc.buttonList[scoringMove.move.moves[0], scoringMove.move.moves[1]].text);

        gc.EndTurn();
    }

    ScoringMove Minimax (Board MMboard, byte depth)
    {
        //Debug.Log("IA.MINIMAX Nº: " + depth);
        PossibleMoves bestMove = new PossibleMoves(2000,2000,2000,2000);
        int bestScore = 0;

        ScoringMove scoringMove;
        Board newBoard;
        

        if (MMboard.WinningMove(activePlayer) || depth >= MAX_DEPTH)
        {
            //Debug.Log("Mi active player de board para el if del minimax es: " + board.activePlayer + " y mi active player de IA es: " + activePlayer);

            scoringMove = new ScoringMove(MMboard.Evaluate(activePlayer, totalRed, totalWhite, totalRedQ, totalWhiteQ), new PossibleMoves(0, 0, 0, 0));

            
        }
        else
        {
            if (MMboard.activePlayer == activePlayer) bestScore = INF;
            else bestScore = MINUS_INF;

            List<PossibleMoves> possibleMoves;
            possibleMoves = MMboard.PossibleMoves();
            int contador = 0;
            foreach (PossibleMoves move in possibleMoves)
            {
                //Debug.Log("IA.MINIMAX, FOREACH contiene la ficha "+ buttonList[move.moves[2], move.moves[3]].text + " en " + move.moves[2] + ", " + move.moves[3] + " y su movimiento a " + move.moves[0] + ", " + move.moves[1] );
                contador++;
                
                newBoard = MMboard.GenerateNewBoardFromMove(move);

                //recursividad
                scoringMove = Minimax(newBoard, (byte)(depth + 1));

                //Debug.Log("tras minimaxear en la capa " + depth + " recibo el score de " + scoringMove.score + " Para el player " + MMboard.activePlayer);

                //Actualizamos el mejor movimiento y score
                if (MMboard.activePlayer == activePlayer)
                {
                    //Debug.Log("IA active player " + activePlayer + " es igual a board.activeplayer " + board.activePlayer);
                    if (scoringMove.score > bestScore)
                    {
                        //Debug.Log("En el minimax " + depth + " cuyo player es " + board.activePlayer + " entro en el else para cojer el mas GRANDE de los scores");
                        bestScore = scoringMove.score;
                        bestMove = move;
                    }
                    else if (scoringMove.score == bestScore)
                    {
                        int rand = UnityEngine.Random.Range(0, 2);
                        if (rand == 0)
                        {
                            bestScore = scoringMove.score;
                            bestMove = move;
                        }

                    }
                }

                else if (MMboard.activePlayer == board.Opponent(activePlayer))
                {
                    //Debug.Log("IA active player " + activePlayer + " NO es igual a board.activeplayer " + board.Opponent(activePlayer));
                    if (scoringMove.score < bestScore)
                    {
                        //Debug.Log("En el minimax " + depth + " cuyo player es " + board.activePlayer + " entro en el else para cojer el mas PEQUEÑO de los scores");
                        bestScore = scoringMove.score;
                        bestMove = move;
                    }
                    else if (scoringMove.score == bestScore)
                    {
                        int rand = UnityEngine.Random.Range(0, 2);
                        if (rand == 0)
                        {
                          bestScore = scoringMove.score;
                          bestMove = move;
                        }

                    }
                }

            }
            scoringMove = new ScoringMove(bestScore, bestMove);
            //Debug.Log("Mi score para el minimax Nº: " + (depth) + " cuyo player es " + board.activePlayer + " , ha sido: " + scoringMove.score + " la ficha estaba en la casilla: " + scoringMove.move.moves[2] + " , " + scoringMove.move.moves[3] + "y el movimiento ha sido a: " + scoringMove.move.moves[0] + " , " + scoringMove.move.moves[1]);

        }
        return scoringMove;
    }

}
