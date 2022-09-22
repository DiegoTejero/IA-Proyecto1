using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleMoves{

    public int[] moves;

	// Use this for initialization
	public PossibleMoves (int row, int col, int initialRow, int initialCol )
    {
        moves = new int[4];

        moves[0] = row;
        moves[1] = col;
        moves[2] = initialRow;
        moves[3] = initialCol;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
