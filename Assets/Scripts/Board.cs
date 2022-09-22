using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board  {

    private GameController gc;
    public string[,] spaces;
    public string activePlayer;

    public byte rows, columns;

    private const int moveScore = 1;
    private const int eatScore = 5;
    private const int upgradeScore = 3;


    public Board(byte _rows, byte _colummns, GameController _gc)
    {
        gc = _gc;
        rows = _rows;
        columns = _colummns;
        spaces = new string[rows, columns];

    }

    public int Evaluate(string player, int _totalRed, int _totalWhite, int _totalRedQ, int _totalWhiteQ)
    {
        //Debug.Log("MI PLAYER ES " + player);
        if (WinningMove(player))
        {
            return -200;
        }
        if (WinningMove(Opponent(player)))
        {
            return 200;
        }

        int evaluationSum = 0;
        int redQueens = 0;
        int whiteQueens = 0;
        int redChips = 0;
        int whiteChips = 0;
        
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (this.spaces[row, column].Contains("X"))
                {
                    redChips++;
                    if (this.spaces[row, column] == "XX")
                    {
                        redQueens++;

                    }
                }
                else if (this.spaces[row, column].Contains("O"))
                {
                    whiteChips++;
                    if (this.spaces[row, column] == "OO")
                    {
                        whiteQueens++;
                    }                    
                }
            }
        }
        //Debug.Log("hay este numero de damas rojas " + _totalRedQ + " y he contado " + redQueens + " fichas rojas");
        //Debug.Log("hay este numero de damas blancas " + _totalWhiteQ + " y he contado " + whiteQueens+ " fichas blancas");
        if (player == "O")
        {
            if (_totalRedQ < redQueens)
            {
                evaluationSum -= upgradeScore * (redQueens - _totalRedQ);
            }
            if (_totalWhiteQ < whiteQueens)
            {
                evaluationSum += upgradeScore * (whiteQueens - _totalWhiteQ);
            }
            //Debug.Log("hay este numero de fichas rojas " + _totalRed + " y he contado " + redChips +" fichas rojas");
            if (_totalRed > redChips)
            {
                //Debug.Log("hay este numero de fichas rojas " + _totalRed);
                evaluationSum += eatScore * (_totalRed - redChips);
                //Debug.Log("ME SUMO :" + evaluationSum);
            }
            if (_totalWhite > whiteChips)
            {
                //Debug.Log("hay este numero de fichas blancas " + _totalWhite + " y he contado " + whiteChips + " fichas rojas");
                evaluationSum -= (int)(eatScore * 1.20f *(_totalWhite - whiteChips));
                //Debug.Log("ME RESTO :" + evaluationSum);
            }
        }
        if (player == "X")
        {
            if (_totalRedQ < redQueens)
            {
                evaluationSum += upgradeScore * (redQueens - _totalRedQ);
            }
            if (_totalWhiteQ < whiteQueens)
            {
                evaluationSum -= upgradeScore * (whiteQueens - _totalWhiteQ);
            }
            //Debug.Log("hay este numero de fichas blancas " + _totalWhite);
            if (_totalRed > redChips)
            {
                evaluationSum -= (int)(eatScore * 1.20f * (_totalWhite - whiteChips));
            }
            if (_totalWhite > whiteChips)
            {
                evaluationSum += eatScore * (_totalWhite - whiteChips);
            }
        }
        

        //Debug.Log("La evaluation Sum total es:" + evaluationSum);
        return evaluationSum;
    }

    public bool WinningMove(string activePlayer)
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (spaces[row, column].Contains(activePlayer))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Board GenerateNewBoardFromMove(PossibleMoves move)
    {
        Board newBoard = this.DuplicateBoard();
        newBoard.Move(move);
        newBoard.activePlayer = Opponent(newBoard.activePlayer);
        return newBoard;
    }

    public Board DuplicateBoard()
    {
        Board newBoard = new Board(rows, columns, gc);
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                newBoard.spaces[row, column] = this.spaces[row, column];
            }
        }
        newBoard.activePlayer = this.activePlayer;
        return newBoard;
    }

    //Esta funcion comprueba si se puede mover las fichas, ya sea comiendo o movimiento normal
    //esa funcion es la que crea las ramificaciones del Minimax para cada checkeo recursivo
    public List<PossibleMoves> PossibleMoves()
    {
        List<PossibleMoves> moves;
        int count = 0;

        //Rellenamos el array con las fichas que haya en tablero y 
        //que sean del jugador activo y  que por cada movimiento 
        //posible, añadimos +1 a count.
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (spaces[row, column].Contains(activePlayer))
                {
                    if (spaces[row, column] == activePlayer + activePlayer)
                    {
                        count += CheckAllDirections(row, column, activePlayer);
                    }
                    else
                    {
                        count += CheckNorthOrSouth(row, column, activePlayer);
                    }
                }
            }
        }
        
        moves = new List<PossibleMoves>(count);
        moves.Clear();
        count = 0;

        //Debug.Log("La capacidad de moves es " + moves.Capacity);

        //Rellenamos el array Moves con la posicion resultante del movimiento de la ficha del bucle for.
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (spaces[row, column].Contains(activePlayer))
                {
                    //Si es dama
                    if (spaces[row, column] == (activePlayer + activePlayer))
                    {
                        if (CheckSuperiorLeftEat(row, column, activePlayer) == 1)
                        {
                            moves.Add(new PossibleMoves(row - 1, column - 1, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 1) + ", " + (column - 1));

                            count++;
                        }
                        else if (CheckSuperiorLeftEat(row, column, activePlayer) == 2)
                        {
                            moves.Add(new PossibleMoves(row - 2, column - 2, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 2) + ", " + (column - 2));

                            count++;
                        }

                        if (CheckSuperiorRightEat(row, column, activePlayer) == 1)
                        {
                            moves.Add(new PossibleMoves(row - 1, column + 1, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 1) + ", " + (column + 1));

                            count++;
                        }
                        else if (CheckSuperiorRightEat(row, column, activePlayer) == 2)
                        {
                            moves.Add(new PossibleMoves(row - 2, column + 2, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 2) + ", " + (column + 2));

                            count++;
                        }

                        if (CheckInferiorLeftEat(row, column, activePlayer) == 1)
                        {
                            moves.Add(new PossibleMoves(row + 1, column - 1, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 1) + ", " + (column - 1));

                            count++;
                        }
                        else if (CheckInferiorLeftEat(row, column, activePlayer) == 2)
                        {
                            moves.Add(new PossibleMoves(row + 2, column - 2, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 2) + ", " + (column - 2));

                            count++;
                        }
                        if (CheckInferiorRightEat(row, column, activePlayer) == 1)
                        {
                            moves.Add(new PossibleMoves(row + 1, column + 1, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 1) + ", " + (column + 1));

                            count++;
                        }
                        else if (CheckInferiorRightEat(row, column, activePlayer) == 2)
                        {
                            moves.Add(new PossibleMoves(row + 2, column + 2, row, column));

                            //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 2) + ", " + (column + 2));

                            count++;
                        }
                    }
                    //si es peon 
                    if (spaces[row, column] == (activePlayer))
                    {
                        if (spaces[row, column] == "O")
                        {
                            if (CheckSuperiorLeftEat(row, column, activePlayer) == 1)
                            {
                                //Debug.Log(count);
                                moves.Add(new PossibleMoves((row - 1), (column - 1), row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row -1) + ", " + (column -1) );

                                count++;
                            }
                            else if (CheckSuperiorLeftEat(row, column, activePlayer) == 2)
                            {
                                moves.Add(new PossibleMoves((row - 2), (column - 2), row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 2) + ", " + (column - 2));

                                //moves[count] = new PossibleMoves(row - 2, column - 2, row, column);
                                count++;
                            }
                            if (CheckSuperiorRightEat(row, column, activePlayer) == 1)
                            {
                                moves.Add(new PossibleMoves((row - 1), (column + 1), row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 1) + ", " + (column + 1));

                                //moves[count] = new PossibleMoves(row - 1, column + 1, row, column);
                                count++;
                            }
                            else if (CheckSuperiorRightEat(row, column, activePlayer) == 2)
                            {
                                moves.Add(new PossibleMoves((row - 2), (column + 2), row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row - 2) + ", " + (column + 2));

                                //moves[count] = new PossibleMoves(row - 2, column + 2, row, column);
                                count++;
                            }
                        }
                        else if(spaces[row, column] == "X") 
                        {
                            
                            if (CheckInferiorLeftEat(row, column, activePlayer) == 1)
                            {
                                moves.Add(new PossibleMoves(row + 1, column - 1, row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 1) + ", " + (column - 1));

                                //moves[count] = new PossibleMoves(row + 1, column - 1, row, column);
                                count++;
                            }
                            else if (CheckInferiorLeftEat(row, column, activePlayer) == 2)
                            {
                                moves.Add(new PossibleMoves(row + 2, column - 2, row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 2) + ", " + (column - 2));

                                //moves[count] = new PossibleMoves(row + 2, column - 2, row, column);
                                count++;
                            }
                            if(CheckInferiorRightEat(row, column, activePlayer) == 1)
                            {
                                moves.Add(new PossibleMoves(row + 1, column + 1, row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 1) + ", " + (column + 1));

                                //moves[count] = new PossibleMoves(row + 1, column + 1, row, column);
                                count++;
                            }
                            else if (CheckInferiorRightEat(row, column, activePlayer) == 2)
                            {
                                moves.Add(new PossibleMoves(row + 2, column + 2, row, column));

                                //Debug.Log("Para Moves = " + moves.Count + " Relleno con la ficha " + spaces[row, column] + ", " + row + ", " + column + " y su posible movimiento a " + (row + 2) + ", " + (column + 2));

                                //moves[count] = new PossibleMoves(row + 2, column + 2, row, column);
                                count++;
                            }
                        }
                    }
                }
            }
        }
        return moves;
    }

    public void Move(PossibleMoves move)
    {
        //"movemos" la ficha original
        //Debug.Log("FUNCION BOARD.MOVE():  mi row es: " + move.moves[2] + " mi columna es: " +  move.moves[3] + " me voy a mover a:  " + move.moves[0] + " , " + move.moves[1] +   "  y contengo: " + spaces[move.moves[2], move.moves[3]]);

        if (spaces[move.moves[2], move.moves[3]] == "O" && move.moves[0] == 0)
        {
            spaces[move.moves[0], move.moves[1]] = "OO";
            spaces[move.moves[2], move.moves[3]] = "";
        }
        else if (spaces[move.moves[0], move.moves[1]] == "X" && move.moves[0] == 7)
        {
            spaces[move.moves[0], move.moves[1]] = "XX";
            spaces[move.moves[2], move.moves[3]] = "";
        }
        else
        {
            spaces[move.moves[0], move.moves[1]] = spaces[move.moves[2], move.moves[3]];
            spaces[move.moves[2], move.moves[3]] = "";
        }

        //si ha comido, quitamos la ficha comida
        if ((move.moves[0] + move.moves[2]) % 2 == 0 && (move.moves[1] + move.moves[3]) % 2 == 0)
        {
            spaces[(move.moves[0] + move.moves[2]) / 2, (move.moves[1] + move.moves[3]) / 2] = "";
            Debug.Log("VOY A COMEEEEEEER" + (move.moves[0] + move.moves[2]) / 2 + ", " + (move.moves[1] + move.moves[3]) / 2);
        }

    }

    public byte CheckAllDirections(byte row, byte column, string color)
    {
        byte moves = 0;

        if (CheckSuperiorRight(row, column, color))
        {
            moves++;
        }
        if (CheckSuperiorLeft(row, column, color))
        {
            moves++;
        }
        if (CheckInferiorLeft(row, column, color))
        {
            moves++;
        }
        if (CheckInferiorRight(row, column, color))
        {
            moves++;
        }

        return moves;
    }

    public byte CheckNorthOrSouth(byte row, byte column, string color)
    {
        byte moves = 0;

        if (color.Contains("O"))
        {
            if (CheckSuperiorRight(row, column, color))
            {
                moves++;
            }
            if (CheckSuperiorLeft(row, column, color))
            {
                moves++;
            }
        }
        else
        {
            if (CheckInferiorLeft(row, column, color))
            {
                moves++;
            }
            if (CheckInferiorRight(row, column, color))
            {
                moves++;
            }
        }
        return moves;
    }

    public bool CheckSuperiorRight(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 0 && column != 7)
        {
            //si hay aliado en la casilla, devuelve false
            if (spaces[row - 1, column + 1].Contains(color))
            {
                return false;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((spaces[row - 1, column + 1] != System.Convert.ToString(color[0]) && spaces[row - 1, column + 1] != color && spaces[row - 1, column + 1] != "") && (column < 6 && row > 1))
            {
                if (spaces[row - 2, column + 2] == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (spaces[row - 1, column + 1] == "")
            {
                return true;
            }
            return false;
        }
        else
            return false;
    }

    public bool CheckSuperiorLeft(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 0 && column != 0)
        {
            //si hay aliado en la casilla, devuelve false
            if (spaces[row - 1, column - 1].Contains(color))
            {
                return false;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((spaces[row - 1, column - 1] != System.Convert.ToString(color[0]) && spaces[row - 1, column - 1] != color && spaces[row - 1, column - 1] != "") && (column > 1 && row > 1))
            {
                if (spaces[row - 2, column - 2] == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (spaces[row - 1, column - 1] == "")
            {
                return true;
            }
            return false;
        }
        else
            return false;
    }

    public bool CheckInferiorRight(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 7 && column != 7)
        {
            //si hay aliado en la casilla, devuelve false
            if (spaces[row + 1, column + 1].Contains(color))
            {
                return false;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((spaces[row + 1, column + 1] != System.Convert.ToString(color[0]) && spaces[row + 1, column + 1] != color && spaces[row + 1, column + 1] != "") && (column < 6 && row < 6))
            {
                if (spaces[row + 2, column + 2] == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (spaces[row + 1, column + 1] == "")
            {
                return true;
            }
            return false;
        }
        else
            return false;
    }

    public bool CheckInferiorLeft(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 7 && column != 0)
        {
            //si hay aliado en la casilla, devuelve false
            if (spaces[row + 1, column - 1].Contains(color))
            {
                return false;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((spaces[row + 1, column - 1] != System.Convert.ToString(color[0]) && spaces[row + 1, column - 1] != color && spaces[row + 1, column - 1] != "") && (column > 1 && row > 6))
            {
                if (spaces[row + 2, column - 2] == "")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (spaces[row + 1, column - 1] == "")
            {
                return true;
            }
            return false;
        }
        else
            return false;
    }

    public string Opponent(string player)
    {
        if (player == "X")
        {
            return "O";
        }
        else
        {
            return "X";
        }
    }


    public int CheckSuperiorRightEat(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 0 && column != 7)
        {
            //si hay aliado en la casilla, devuelve 0
            if (spaces[row - 1, column + 1].Contains(color))
            {
                return 0;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((spaces[row - 1, column + 1] != System.Convert.ToString(color[0]) && spaces[row - 1, column + 1] != color && spaces[row - 1, column + 1] != "") && (column < 6 && row > 1))
            {
                if (spaces[row - 2, column + 2] == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (spaces[row - 1, column + 1] == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

    public int CheckSuperiorLeftEat(byte row, byte column, string color)
    {
        //Debug.Log("la row que he mirado es: " + row + " y la column que he mirado es: " + column);
        //mientras no este en la linea 0 o a la izquierda del todo, check la casilla superior izquierda
        if (row != 0 && column != 0)
        {
            //si hay aliado en la casilla, devuelve 0
            if (spaces[row - 1, column - 1].Contains(color))
            {
                return 0;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((spaces[row - 1, column - 1] != System.Convert.ToString(color[0]) && spaces[row - 1, column - 1] != color && spaces[row - 1, column - 1] != "") && (column > 1 && row > 1))
            {
                if (spaces[row - 2, column - 2] == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (spaces[row - 1, column - 1] == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

    public int CheckInferiorRightEat(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 7 && column != 7)
        {
            //si hay aliado en la casilla, devuelve falso
            if (spaces[row + 1, column + 1].Contains(color))
            {
                return 0;
            }
            // si hay un enemigo, checkear
            else if ((spaces[row + 1, column + 1] != System.Convert.ToString(color[0]) && spaces[row + 1, column + 1] != color && spaces[row + 1, column + 1] != "") && (column < 6 && row < 6))
            {
                if (spaces[row + 2, column + 2] == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (spaces[row + 1, column + 1] == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

    public int CheckInferiorLeftEat(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 7 && column != 0)
        {
            //si hay aliado en la casilla, devuelve falso
            if (spaces[row + 1, column - 1].Contains(color))
            {
                return 0;
            }
            // si hay un enemigo, checkear
            else if ((spaces[row + 1, column - 1] != System.Convert.ToString(color[0]) && spaces[row + 1, column - 1] != color && spaces[row + 1, column - 1] != "") && (column > 1 && row < 6))
            {
                if (spaces[row + 2, column - 2] == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (spaces[row + 1, column - 1] == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

}
