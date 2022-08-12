using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    int DIM;
    public int positionX,positionY;
    const int RIGHT = 0,UP=1,LEFT=2,DOWN=3;

    private int[] directions = {RIGHT,UP,LEFT,DOWN};
    // Un array (4 direzioni fisse), ognuna con una lista dei possibili tipi di connesione
    private List<int>[] connections; // Contains the possible connection types in the 4 directions
    private List<Piece> availablePieces;

    public bool collapsed = false;
    public Piece finalValue;

    public Tile(List<Piece> availablePieces,int positionX,int positionY,int DIM){
        this.availablePieces = availablePieces;
        this.positionX = positionX;
        this.positionY = positionY;
        this.DIM = DIM;
        initializeConnections();
    }

    private void initializeConnections(){
        connections = new List<int>[4];
        foreach(int direction in directions){
            connections[direction] = new List<int>();
        }
        foreach(Piece piece in availablePieces){
            foreach(int direction in directions){
                // Aggiungo il tipo di connessione solo se non è già stato aggiunto
                if(!connections[direction].Contains(piece.GetConnectionTypeFromDirection(direction))){
                    connections[direction].Add(piece.GetConnectionTypeFromDirection(direction));
                }
            }
        }
    }

    // Return the number of pieces left after the collapse
    public int Collapse(int connectionType,int myDirection){   
        List<Piece> availablePiecesCopy = new List<Piece>(availablePieces);    
        for(int i=0;i<availablePieces.Count;i++){
            // Se un Piece ha un connectionType diverso da quello richiesto, esso viene scartato
            if(availablePieces[i].GetConnectionTypeFromDirection(myDirection) != connectionType){
                availablePiecesCopy.Remove(availablePieces[i]);
            }
        }
        availablePieces = availablePiecesCopy;

        updateConnections();
        return availablePieces.Count;
    }

    // Just recalculate all the connections
    private void updateConnections(){
        initializeConnections();
    }

    public bool CanCollapse(int connectionType,int myDirection){
        return this.connections[myDirection].Contains(connectionType);
    }
    public List<int>[] GetConnections(){
        return connections;
    }
    public int remainingPieces(){
        return availablePieces.Count;
    }
    public void SetValue(Piece piece){
        this.collapsed = true;
        this.finalValue = piece;
        if(!this.availablePieces.Contains(piece)){
            // ECCEZIONE, NON SO COME SI TIRANO IN C# :)
            Debug.LogError("Errore, Tile non contiene il piece settato");
        }
        this.availablePieces = new List<Piece>(new Piece[] {piece});
    }

    public bool isCollapsed(){
        return collapsed;
    }

    public List<Piece> getAvailablePieces(){
        return availablePieces;
    }

    public bool HasDirection(int direction){
        switch (direction)
        {
            case RIGHT:
                return positionX < DIM-1;
            case UP:
                return positionY > 0;
            case LEFT:
                return positionX > 0;
            case DOWN:
                return positionY < DIM - 1;
            default:
                return false;
        }
    }

    public Tile copy(){
        return new Tile(copyPieces(),positionX,positionY,DIM);
    }

    private List<Piece> copyPieces(){
        List<Piece> pieces = new List<Piece>();
        for(int i=0;i<availablePieces.Count;i++){
            pieces.Add(new Piece(this.availablePieces[i].getConnectionType(),this.availablePieces[i].getValue()));
        }
        return pieces;
    }
}
