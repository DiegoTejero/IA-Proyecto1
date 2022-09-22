using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Space : MonoBehaviour {

    public Button button;
    public byte row, column;
    private GameController gameController;

    public void SetGameControllerReference(GameController gc)
    {
        gameController = gc;
    }

    public void SetRowColumn(byte _row, byte _column)
    {
        row = _row;
        column = _column;
    }

    public void SetSpace()
    {
        //mover ficha
        if (gameController.buttonList[row, column].text == "1"|| gameController.buttonList[row, column].text == "2")
        {
            //aqui se movería la ficha wey
            gameController.MoveChip(row, column, gameController.buttonList[row, column].text);

        }

        //en caso de que el boton tenga una ficha sobre el, ejecutaremos un movimiento, sino, no hara nada
        else if (gameController.buttonList[row, column].text.Contains(gameController.activePlayer))
        {
            //Guardar los valores de la ficha que se ha clicado
            gameController._movingChipColumn = column;
            gameController._movingChipRow = row;
            gameController._movingChipColor = gameController.buttonList[row, column].text;

            gameController.cleanMoves();
            gameController.CheckChip(row, column, gameController.buttonList[row, column].text, false);
        }
    }

}
