using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    const int RIGHT = 0,UP=1,LEFT=2,DOWN=3;

    private int[] directions = {RIGHT,UP,LEFT,DOWN};
    // Valore del pezzo, può essere immagine, modello3d o altro
    private int value;

    // Anticlockwise starting from RIGHT
    private int[] connectionTypes; // ES: [1,1,2,0] -> RIGHT : connectionType = 1
                                  //                  UP : connectionType = 1
                                  //                  LEFT : connectionType = 2
                                  //                  DOWN : connectionType = 0

    public Piece(int[] connectionTypes,int value){
        this.connectionTypes = connectionTypes;
        this.value = value;
    }

    public int GetConnectionTypeFromDirection(int direction){
        return connectionTypes[direction];
    }

    public int getValue(){
        return value;
    }

    public int[] getConnectionType(){
        return connectionTypes;
    }
}
