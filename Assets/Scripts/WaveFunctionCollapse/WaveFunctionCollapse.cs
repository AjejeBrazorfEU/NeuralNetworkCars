using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    const int RIGHT = 0,UP=1,LEFT=2,DOWN=3;
    private int[] directions = {RIGHT,UP,LEFT,DOWN};
    // Assuming square grid
    private Tile[][] grid;
    public int DIM;

    private List<Piece> pieces;

    /*public WaveFunctionCollapse(int DIM){
        this.DIM = DIM;
        createPieces();
        initializeGrid();
    }*/

    void Start(){
        createPieces();
        initializeGrid();
        if(functionCollapse(grid,new Queue<Tile>(),0)){
            Debug.Log("Solution Found");
        }else{
            Debug.Log("No Solution Found");
        }

        printFinalGrid();
    }

    private void printFinalGrid(){
        string res = "";
        for(int i=0;i<DIM;i++){
            for(int j=0;j<DIM;j++){
                res += grid[i][j].finalValue.getValue() + " ";
            }
            res += "\n";
        }
        Debug.Log(res);
    }

    private void createPieces(){
        /*
         * Creating a test set of pieces
         */
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
    }

    private void initializeGrid(){
        grid = new Tile[DIM][];
        for(int i=0;i<DIM;i++){
            grid[i] = new Tile[DIM];
            for(int j=0;j<DIM;j++){
                // Each grid is initialized with all the pieces
                grid[i][j] = new Tile(pieces,i,j,DIM);
            }
        }

        //Test();
    }

    /*private void functionCollapse(){
        int iterations = 0;
        bool finished = false;
        while(!finished && iterations < 1000){
            // Choose the tile with the less entropy
            Tile minorEntropyTile = MinorEntropyTile();

            // Collapsing
            if(minorEntropyTile.remainingPieces() == 1){
                minorEntropyTile.SetValue(minorEntropyTile.getAvailablePieces()[0]);
            }else{
                // Choose randomly from all the possible pieces
                System.Random rnd = new System.Random();
                int index = rnd.Next(minorEntropyTile.remainingPieces());
                Debug.Log("index : " + index + " list size : " + minorEntropyTile.remainingPieces());
                minorEntropyTile.SetValue(minorEntropyTile.getAvailablePieces()[index]);
            }
            Queue<Tile> collapseQueue = new Queue<Tile>();

            collapseNeighbors(minorEntropyTile,collapseQueue);

            while(collapseQueue.Count > 0 && iterations > 1000){
                iterations++;
                Tile t = collapseQueue.Dequeue();
                if(t.remainingPieces() == 1){
                    t.SetValue(t.getAvailablePieces()[0]);
                    collapseNeighbors(t,collapseQueue);
                }
            }
            iterations++;
            finished = true;
            for(int i=0;i<DIM;i++){
                for(int j=0;j<DIM;j++){
                    if(!grid[i][j].isCollapsed()){
                        finished = false;
                    }
                }
            }
        }
    }*/

    private bool functionCollapse(Tile[][] grid,Queue<Tile> collapseQueue,int iterations){
        bool finished = false;
        Tile[][] gridCopy = cloneMatrix(grid);
        while(!finished && iterations < 1000){
            while(collapseQueue.Count > 0 && iterations < 1000){
                iterations++;
                Tile t = collapseQueue.Dequeue();
                if(t.remainingPieces()==0){
                    // C'è un fallimento, bisogna tornare indietro
                    return false;
                }
                if(t.remainingPieces() == 1){
                    t.SetValue(t.getAvailablePieces()[0]);
                    collapseNeighbors(t,collapseQueue,gridCopy);
                    // Attenzione alla copia di collapse queue
                    if(functionCollapse(gridCopy,new Queue<Tile>(collapseQueue),iterations+1)){
                        // L'algoritmo ha finito di fare tutto
                    }else{
                        // Siamo arrivati ad un punto cieco, resetto la grid
                    }
                }
            }
            // Choose the tile with the less entropy
            Tile minorEntropyTile = MinorEntropyTile(gridCopy);

            // Collapsing
            if(minorEntropyTile.remainingPieces() == 0){
                // C'è un fallimento, bisogna tornare indietro
                return false;
            }
            if(minorEntropyTile.remainingPieces() == 1){
                minorEntropyTile.SetValue(minorEntropyTile.getAvailablePieces()[0]);
            }else{
                // Choose randomly from all the possible pieces
                System.Random rnd = new System.Random();
                int index = rnd.Next(minorEntropyTile.remainingPieces());
                Debug.Log("index : " + index + " list size : " + minorEntropyTile.remainingPieces());
                minorEntropyTile.SetValue(minorEntropyTile.getAvailablePieces()[index]);
            }
            collapseNeighbors(minorEntropyTile,collapseQueue,gridCopy);

            
            iterations++;
            finished = true;
            for(int i=0;i<DIM;i++){
                for(int j=0;j<DIM;j++){
                    if(!grid[i][j].isCollapsed()){
                        finished = false;
                    }
                }
            }
        }
    }

    private void collapseNeighbors(Tile tile,Queue<Tile> collapseQueue, Tile[][] grid){
        foreach (int direction in directions)
            {
                if(tile.HasDirection(direction)){
                    switch (direction)
                    {
                        case RIGHT:
                            grid[tile.positionX+1][tile.positionY].Collapse(tile.finalValue.GetConnectionTypeFromDirection(RIGHT),LEFT);
                            collapseQueue.Enqueue(grid[tile.positionX+1][tile.positionY]);
                            break;
                        case UP:
                            grid[tile.positionX][tile.positionY-1].Collapse(tile.finalValue.GetConnectionTypeFromDirection(UP),DOWN);
                            collapseQueue.Enqueue(grid[tile.positionX][tile.positionY-1]);
                            break;
                        case LEFT:
                            grid[tile.positionX-1][tile.positionY].Collapse(tile.finalValue.GetConnectionTypeFromDirection(LEFT),RIGHT);
                            collapseQueue.Enqueue(grid[tile.positionX-1][tile.positionY]);
                            break;
                        case DOWN:
                            grid[tile.positionX][tile.positionY+1].Collapse(tile.finalValue.GetConnectionTypeFromDirection(DOWN),UP);
                            collapseQueue.Enqueue(grid[tile.positionX][tile.positionY+1]);
                            break;
                        default:
                            break;
                    }       
                }
            }
    }
    private Tile MinorEntropyTile(Tile[][] grid){
        Tile minorEntropyTile = null;
        for(int i=0;i<DIM;i++){
            for(int j=0;j<DIM;j++){
                if(minorEntropyTile == null){
                    minorEntropyTile = grid[i][j];
                }else{
                    if(grid[i][j].remainingPieces() < minorEntropyTile.remainingPieces()){
                        minorEntropyTile = grid[i][j];
                    }
                }
            }
        }
        return minorEntropyTile;
    }
    private void Test(){
        List<int>[] test = grid[0][0].GetConnections();
        string output = "";
        foreach(int direction in directions){
            output += "{";
            foreach(int v in test[direction]){
                output += v + " ";
            }   
            output += "}";
        }
        Debug.Log(output);
    }

    private Tile[] cloneMatrix(Tile[] source){
        Tile[] result = new Tile[source.Length];
        for(int i=0;i<source.Length;i++){
            result[i] = source[i].copy();
        }
        return result;
    }
    private Tile[][] cloneMatrix(Tile[][] source){
        Tile[][] result = new Tile[source.Length][];
        for(int i=0;i<source.Length;i++){
            Tile[] row = cloneMatrix(source[i]);
            result[i] = row;
        }
        return result;
    }
}
