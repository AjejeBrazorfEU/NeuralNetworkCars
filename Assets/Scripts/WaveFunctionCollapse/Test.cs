using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    const int RIGHT = 0,UP=1,LEFT=2,DOWN=3;
    private int[] directions = {RIGHT,UP,LEFT,DOWN};

    List<Piece> pieces;
    Tile[][] grid;
    void Start()
    {
        TestPiece();   
        TestTile();    
    }

    private void TestPiece(){
        pieces = new List<Piece>();
        // White Piece
        pieces.Add(new Piece(new int[] {0,0,0,0},0));
        // Right Piece
        pieces.Add(new Piece(new int[] {1,1,0,1},1));
        // Up Piece
        pieces.Add(new Piece(new int[] {1,1,1,0},2));
        // Left Piece
        pieces.Add(new Piece(new int[] {0,1,1,1},3));
        // Down Piece
        pieces.Add(new Piece(new int[] {1,0,1,1},4));


        Debug.Assert(pieces[0].getValue() == 0 &&
                     pieces[0].GetConnectionTypeFromDirection(RIGHT) == pieces[0].GetConnectionTypeFromDirection(LEFT));

        Debug.Assert(pieces[1].getValue() == 1 &&
                     pieces[1].GetConnectionTypeFromDirection(UP) == 1 &&
                     pieces[1].GetConnectionTypeFromDirection(LEFT) == 0);

        Debug.Assert(pieces[2].getValue() == 2 &&
                     pieces[2].GetConnectionTypeFromDirection(UP) == 1 &&
                     pieces[2].GetConnectionTypeFromDirection(LEFT) == 1);

        Debug.Assert(pieces[3].getValue() == 3 &&
                     pieces[3].GetConnectionTypeFromDirection(UP) == 1 &&
                     pieces[3].GetConnectionTypeFromDirection(LEFT) == 1);
        
        Debug.Assert(pieces[4].getValue() == 4 &&
                     pieces[4].GetConnectionTypeFromDirection(UP) == 0 &&
                     pieces[4].GetConnectionTypeFromDirection(LEFT) == 1);

    }

    private void TestTile(){
        grid = new Tile[2][];
        for(int i=0;i<2;i++){
            grid[i] = new Tile[2];
            for(int j=0;j<2;j++){
                // Each grid is initialized with all the pieces
                grid[i][j] = new Tile(pieces,i,j,2);
            }
        }

        for(int i=0;i<2;i++){
            for(int j=0;j<2;j++){
                Debug.Assert(grid[i][j].getAvailablePieces() == pieces &&
                             grid[i][j].remainingPieces() == 2 &&
                             grid[i][j].isCollapsed() == false &&
                             grid[i][j].CanCollapse(0,RIGHT) == true &&
                             grid[i][j].CanCollapse(1,RIGHT) == true);
            }
        }

        grid[0][0].SetValue(pieces[0]);
        
    }
}
