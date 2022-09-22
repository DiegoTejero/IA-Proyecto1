using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public Ai ia;

    public GameObject startRed;
    public GameObject startWhite;
    public GameObject startText;

    public Text winText;
    public GameObject restartButton;

    public GameObject tabletop;
    public GameObject tablero;
    public GameObject prefabSpace;
    public GameObject redChip;
    public GameObject whiteChip;

    public GameObject _tabletop;
    public GameObject _tablero;

    public Text[,] buttonList;
    public byte rows, columns;

    public byte _movingChipRow, _movingChipColumn;
    public string _movingChipColor;
    public string activePlayer, Player, IA;

    private void Awake()
    {
        _tabletop = (GameObject)Instantiate(tabletop, new Vector3(0, 0, 0), Quaternion.identity);
        _tabletop.transform.SetParent(GameObject.Find("Background").transform);
        _tabletop.transform.localPosition = new Vector3(0, 0, 0);

        _tablero = (GameObject)Instantiate(tablero, new Vector3(0, 0, 0), Quaternion.identity);
        _tablero.transform.SetParent(_tabletop.transform);
        _tablero.transform.localPosition = new Vector3(0, 0, 0);

        ia = GameObject.Find("AI").GetComponent<Ai>();

        
    }

    private void StartGame()
    {
        buttonList = new Text[rows, columns];

        //Debug.Log(GameObject.Find("TableTop(Clone)").name);
        //Debug.Log(GameObject.Find("GameGrid(Clone)").name);
        if (_tabletop == null)
        {
            _tabletop = (GameObject)Instantiate(tabletop, new Vector3(0, 0, 0), Quaternion.identity);
            _tabletop.transform.SetParent(GameObject.Find("Background").transform);
            _tabletop.transform.localPosition = new Vector3(0, 0, 0);
        }

        if (_tablero == null)
        { 
            _tablero = (GameObject)Instantiate(tablero, new Vector3(0, 0, 0), Quaternion.identity);
            _tablero.transform.SetParent(_tabletop.transform);
            _tablero.transform.localPosition = new Vector3(0, 0, 0);
        }

        winText.gameObject.SetActive(false);
        restartButton.SetActive(false);

        //Inicializacion del tablero y sus valores
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                GameObject gameObject = (GameObject)Instantiate(prefabSpace, new Vector3(0, 0, 0), Quaternion.identity);
                gameObject.transform.SetParent(_tablero.transform);
                buttonList[row, column] = (Text)gameObject.GetComponentInChildren(typeof(Text));
                Space space = buttonList[row, column].GetComponentInParent<Space>();
                space.SetGameControllerReference(this);
                space.SetRowColumn(row, column);

                //Pinto el tablero con sus colores
                if (row % 2 == 1)
                {
                    if (column % 2 == 0)
                    {
                        gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                        
                    }
                }
                else if (column % 2 == 1)
                {
                    gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                }

                //Situo las fichas de color rojo
                if (row == 0 && column % 2 == 1)
                {
                    GameObject token = (GameObject)Instantiate(redChip, new Vector3(0, 0, 0), Quaternion.identity);
                    token.transform.SetParent(gameObject.transform);
                    buttonList[row, column].text = "X";

                }
                else if (row == 1 && column % 2 == 0)
                {
                    GameObject token = (GameObject)Instantiate(redChip, new Vector3(0, 0, 0), Quaternion.identity);
                    token.transform.SetParent(gameObject.transform);
                    buttonList[row, column].text = "X";
                }

                //Situo las fichas de color Blanco
                if (row == 6 && column % 2 == 1)
                {
                    GameObject token = (GameObject)Instantiate(whiteChip, new Vector3(0, 0, 0), Quaternion.identity);
                    token.transform.SetParent(gameObject.transform);
                    buttonList[row, column].text = "O";
                }
                else if (row == 7 && column % 2 == 0)
                {
                    GameObject token = (GameObject)Instantiate(whiteChip, new Vector3(0, 0, 0), Quaternion.identity);
                    token.transform.SetParent(gameObject.transform);
                    buttonList[row, column].text = "O";
                }
            }
            
        }

        ia.SetGameController(this);
        ia.SetButtonList(buttonList);
    }

    public void EatChip(byte row, byte column)
    {
        int _finalRow = (row + _movingChipRow) / 2;
        int _finalColumn = (column + _movingChipColumn) / 2;

        if (buttonList[_finalRow, _finalColumn].GetComponentInParent<Space>().transform.GetChild(0).name.Contains("Chip"))
        {
            GameObject token = buttonList[_finalRow, _finalColumn].GetComponentInParent<Space>().transform.GetChild(0).gameObject;
            Destroy(token);
            buttonList[_finalRow, _finalColumn].text = "";
            _movingChipRow = row;
            _movingChipColumn = column;
            cleanMoves();
            SetSpacesInteractable(false);
            CheckChip(row, column, _movingChipColor, false);
        }

        else if (buttonList[_finalRow, _finalColumn].GetComponentInParent<Space>().transform.GetChild(1).name.Contains("Chip"))
        {
            GameObject token = buttonList[_finalRow, _finalColumn].GetComponentInParent<Space>().transform.GetChild(1).gameObject;
            Destroy(token);
            buttonList[_finalRow, _finalColumn].text = "";
            _movingChipRow = row;
            _movingChipColumn = column;
            cleanMoves();
            SetSpacesInteractable(false);
            CheckChip(row, column, _movingChipColor, false);
        }
    }

    public void MoveChip(byte row, byte column, string typeOfMovement)
    {
        Space space = buttonList[_movingChipRow, _movingChipColumn].GetComponentInParent<Space>();
        Space newSpace = buttonList[row, column].GetComponentInParent<Space>();

        // en caso de que la chip sea su primer hijo
        if (space.transform.GetChild(0).name.Contains("Chip"))
        {
            //Move sprite
            GameObject token = space.transform.GetChild(0).gameObject;
            token.transform.SetParent(newSpace.transform);
            token.transform.localPosition = new Vector3(0,0,0);
            //si estamos comiendo
            

            //Update board values
            buttonList[_movingChipRow, _movingChipColumn].text = "";
            buttonList[row, column].text = _movingChipColor;

            if (typeOfMovement == "2")
            {
                EatChip(row, column);
                
            }
            else
            {
                cleanMoves();
            }
            UpgradeChip(row, column, _movingChipColor);
            //puede concatenar movimiento?
            //if (!MustEat())
                EndTurn();
            
        }

        //En caso de que la chip sea su segundo hijo
        else if (space.transform.GetChild(1).name.Contains("Chip"))
        {
            //Move sprite
            GameObject token = space.transform.GetChild(1).gameObject;
            token.transform.SetParent(newSpace.transform);
            token.transform.localPosition = new Vector3(0, 0, 0);
            //si estamos comiendo
            

            //update board values
            buttonList[_movingChipRow, _movingChipColumn].text = "";
            buttonList[row, column].text = _movingChipColor;
            
            if (typeOfMovement == "2")
            {
                EatChip(row, column);
                
            }
            else
            {
                cleanMoves();
            }


            UpgradeChip(row, column, _movingChipColor);

            //puede concatenar movimiento?
            //if (!MustEat())
                EndTurn();

        }
        else
            Debug.Log("there is no chip");

    }

    // case 0, no hay movimiento
    // case 1, hay movimiento a un hueco vacio
    // case 2, puede comer
    public void CheckChip(byte row, byte column, string color, bool concatenateJump)
    {
        //si la ficha es una dama (OO o XX) permitir movimiento en las 4 direcciones
        if (color == "OO" || color == "XX")
        {
            //Check superior right
            switch (CheckSuperiorRight(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Derecha");
                    break;
                case 1:
                    //marcar casilla superior derecha
                    if (concatenateJump == false)
                        buttonList[row - 1, column + 1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 superior derecha
                    buttonList[row - 2, column + 2].text = "2";
                    buttonList[row - 2, column + 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
            //Check superior left 
            switch (CheckSuperiorLeft(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Izquierda");
                    break;
                case 1:
                    //marcar casilla superior Izquierda
                    if (concatenateJump == false)
                        buttonList[row - 1, column - 1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 superior Izquierda
                    buttonList[row - 2, column - 2].text = "2";
                    buttonList[row - 2, column - 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
            //Check inferior right
            switch (CheckInferiorRight(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Derecha");
                    break;
                case 1:
                    //marcar casilla inferior derecha
                    if (concatenateJump == false)
                        buttonList[row + 1, column + 1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 inferior derecha
                    buttonList[row + 2, column + 2].text = "2";
                    buttonList[row + 2, column + 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
            //Check inferior left
            switch (CheckInferiorLeft(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Izquierda");
                    break;
                case 1:
                    //marcar casilla inferior derecha
                    if (concatenateJump == false)
                        buttonList[row + 1, column - 1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 inferior derecha
                    buttonList[row + 2, column - 2].text = "2";
                    buttonList[row + 2, column - 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
        }
        //si la ficha es blanca (O) permitir movimiento direccion norte
        else if (color == "O")
        {
            //Check superior right 
            switch (CheckSuperiorRight(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Derecha");
                    break;
                case 1:
                    //marcar casilla superior derecha
                    if (concatenateJump == false)
                        buttonList[row-1, column+1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 superior derecha
                    buttonList[row - 2, column + 2].text = "2";
                    buttonList[row - 2, column + 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
            //Check superior left 
            switch (CheckSuperiorLeft(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Izquierda");
                    break;
                case 1:
                    //marcar casilla superior Izquierda
                    if (concatenateJump == false)
                        buttonList[row-1, column-1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 superior Izquierda
                    buttonList[row - 2, column - 2].text = "2";
                    buttonList[row - 2, column - 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
            

        }
        //si la ficha es roja (X) permitir movimiento direccion sur
        else if (color == "X")
        {
            //Check inferior right
            switch (CheckInferiorRight(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Derecha");
                    break;
                case 1:
                    //marcar casilla inferior derecha
                    if (concatenateJump == false)
                        buttonList[row + 1, column + 1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 inferior derecha
                    buttonList[row + 2, column + 2].text = "2";
                    buttonList[row + 2, column + 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }

            switch (CheckInferiorLeft(row, column, color))
            {
                case 0:
                    Debug.Log("no hay movimiento Izquierda");
                    break;
                case 1:
                    //marcar casilla inferior derecha
                    if (concatenateJump == false)
                        buttonList[row + 1, column - 1].text = "1";
                    break;
                case 2:
                    //marcar casilla +2 inferior derecha
                    buttonList[row + 2, column - 2].text = "2";
                    buttonList[row + 2, column - 2].GetComponentInParent<Button>().interactable = true;
                    break;
            }
        }
    }

    //Funciones que checkean los diferentes movimientos de las damas

    public int CheckSuperiorRight(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 0 && column != 7)
        {
            //si hay aliado en la casilla, devuelve 0
            if (buttonList[row - 1, column + 1].text.Contains(color))
            {
                return 0;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((buttonList[row - 1, column + 1].text != System.Convert.ToString(color[0]) && buttonList[row - 1, column + 1].text != color && buttonList[row - 1, column + 1].text != "") && (column < 6 && row > 1))
            {
                if (buttonList[row - 2, column + 2].text == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (buttonList[row - 1, column + 1].text == "")
            {
                return 1;
            }
            return 0; 
        }
        else
            return 0;  
    }

    public int CheckSuperiorLeft(byte row, byte column, string color)
    {
        //Debug.Log("la row que he mirado es: " + row + " y la column que he mirado es: " + column);
        //mientras no este en la linea 0 o a la izquierda del todo, check la casilla superior izquierda
        if (row != 0 && column != 0)
        {
            //si hay aliado en la casilla, devuelve 0
            if (buttonList[row - 1, column - 1].text.Contains(color))
            {
                return 0;
            }
            // si no es del mismo color ni es un espacio en blanco, es un enemigo
            else if ((buttonList[row - 1, column - 1].text != System.Convert.ToString(color[0]) && buttonList[row - 1, column - 1].text != color && buttonList[row - 1, column - 1].text != "") && (column > 1 && row > 1))
            {
                if (buttonList[row - 2, column - 2].text == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (buttonList[row - 1, column - 1].text == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

    public int CheckInferiorRight(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 7 && column != 7)
        {
            //si hay aliado en la casilla, devuelve falso
            if (buttonList[row + 1, column + 1].text.Contains(color))
            {
                return 0;
            }
            // si hay un enemigo, checkear
            else if ((buttonList[row + 1, column + 1].text != System.Convert.ToString(color[0]) && buttonList[row + 1, column + 1].text != color && buttonList[row + 1, column + 1].text != "") && (column < 6 && row < 6))
            {
                if (buttonList[row + 2, column + 2].text == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (buttonList[row + 1, column + 1].text == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

    public int CheckInferiorLeft(byte row, byte column, string color)
    {
        //mientras no este en la linea 0 o a la derecha del todo, check la casilla superior derecha
        if (row != 7 && column != 0)
        {
            //si hay aliado en la casilla, devuelve falso
            if (buttonList[row + 1, column - 1].text.Contains(color))
            {
                return 0;
            }
            // si hay un enemigo, checkear
            else if ((buttonList[row + 1, column - 1].text != System.Convert.ToString(color[0]) && buttonList[row + 1, column - 1].text != color && buttonList[row + 1, column - 1].text != "") && (column > 1 && row < 6))
            {
                if (buttonList[row + 2, column - 2].text == "")
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            else if (buttonList[row + 1, column - 1].text == "")
            {
                return 1;
            }
            return 0;
        }
        else
            return 0;
    }

    //Funcion que convierte un peon a Dama

    public void UpgradeChip(byte row, byte column, string color)
    {
        if (color == "O")
        {
            if (row == 0)
            {
                
                GameObject token = (GameObject)Instantiate(whiteChip, new Vector3(0, 0, 0), Quaternion.identity);
                token.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                token.transform.SetParent(buttonList[row, column].GetComponentInParent<Space>().transform.GetChild(1));
                token.transform.localPosition = new Vector3(0, 0, 0);
                token.GetComponent<Image>().color = new Color32(230, 230, 230, 255);

                buttonList[row, column].text = "OO";
            }
        }

        else if (color == "X")
        {
            if (row == 7)
            {
                GameObject token = (GameObject)Instantiate(redChip, new Vector3(0, 0, 0), Quaternion.identity);
                token.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                token.transform.SetParent(buttonList[row, column].GetComponentInParent<Space>().transform.GetChild(1));
                token.transform.localPosition = new Vector3(0, 0, 0);
                token.GetComponent<Image>().color = new Color32(170, 0, 0, 255);

                buttonList[row, column].text = "XX";
            }
        }
    }

    public void cleanMoves()
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (buttonList[row, column].text == "1" || buttonList[row, column].text == "2")
                {
                    buttonList[row, column].text = "";

                }
            }
        }
    }

    public bool MustEat()
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (buttonList[row, column].text == "2")
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetStartingSide(string startingSide)
    {
        //ocultar botones de seleccion de ficha
        activePlayer = startingSide;
        Player = activePlayer;
        if (activePlayer == "O")
        {
            IA = "X";
        }
        else
            IA = "O";
        

        winText.gameObject.SetActive(false);
        restartButton.SetActive(false);
        startRed.SetActive(false);
        startWhite.SetActive(false);
        startText.SetActive(false);

        StartGame();
    }

    public void ChangeActivePlayer()
    {
        if (activePlayer == "O")
        {
            activePlayer = "X";
        }
        else
            activePlayer = "O";
    }

    public void SetSpacesInteractable(bool toggle)
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                Text buttonText = buttonList[row, column];
                Button button = buttonText.GetComponentInParent<Button>();
                button.interactable = toggle;
            }
        }
    }

    public void CleanTable()
    {
        Destroy(_tabletop);
        _tabletop = null;
        Destroy(_tablero);
        _tablero = null;
    }

    public bool CheckWin(string color)
    {
        for (byte row = 0; row < rows; row++)
        {
            for (byte column = 0; column < columns; column++)
            {
                if (buttonList[row, column].text.Contains(color))
                {
                    return false;

                }
            }

        }
                return true;
    }

    public void Restart()
    {
        CleanTable();

        StartGame();
    }

    public void EndTurn()
    {

        ChangeActivePlayer();

        if (CheckWin(activePlayer))
        {
            if (activePlayer == "O")
            {
                winText.text = "Ganaron los Rojos";
            }
            else
                winText.text = "Ganan los Blancos";

            winText.gameObject.SetActive(true);
            restartButton.SetActive(true);
        }
        else
        {
            SetSpacesInteractable(true);
            cleanMoves();

            if (activePlayer == IA)
            {
                //ia.SetButtonList(buttonList);
                ia.Play(activePlayer);
            }
            
        }
    }
}
