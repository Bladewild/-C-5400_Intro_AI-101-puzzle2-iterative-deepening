/*
* Author: ALAIN MARKUS SANTOS-TANKIA
* File: Board.cs Date: 2/16/20
* Class: CS 5400 Online
* Instructor : Ricardo Morales
*
* Contains Board class and a few other constructors
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace hw1
{

    [Serializable]
    public class Board
    {
        public enum Direction
        {
            swipeUp,
            swipeRight,
            swipeDown,
            swipeLeft,
        };


        //internally  all coordinates start from 0,0
        //displaying should add 1,1

        public struct Coordinates
        {
            public int x, y;//the value is for blocks

            public Coordinates(Coordinates currentCoordinate) : this()
            {
                this.x = currentCoordinate.x;
                this.y = currentCoordinate.y;
            }

            public Coordinates(int p1, int p2)
            {
                x = p1;
                y = p2;
            }
            public override string ToString()
            {
                return "[" + x + "," + y + "]";
            }
            public static bool operator ==(Coordinates obj1, Coordinates obj2)
            {
                if (obj1.x == obj2.x && obj1.y == obj2.y)
                    return true;
                return false;
            }

            public static bool operator !=(Coordinates obj1, Coordinates obj2)
            {
                if (obj1.x != obj2.x && obj1.y != obj2.y)
                    return true;
                return false;
            }
            public override bool Equals(object o)
            {
                return true;
            }
            public override int GetHashCode()
            {
                return 0;
            }


        }
        public struct Property
        {
            public int value;
            public Property(int v)
            {
                value = v;
            }
            public Property(Property p)
            {
                value = p.value;
            }
        }

        public struct Cell
        {
            public Coordinates coordinates;
            public Property property;

            public Cell(Coordinates c, Property p)
            {
                coordinates = c;
                property = p;
            }
            public Cell(Cell c)
            {
                coordinates = c.coordinates;
                property = c.property;
            }

            public override string ToString()
            {
                return property.value.ToString();
            }
        }

        //Configurations
        public int row, coloumn;
        public int[,] GameBoard; //THE MAIN GAMEBOARD
        private int valuetoObtain;
        private bool goalReached = false;
        public Queue<int> tilespawnPool;
        private string solution;        
        public int swipes;
        static int numberIncreasing = 0;
        public int id;
        //public List<Board> _children;

        //empty gameboard;
        public Board(int r, int c, int input_valuetoObtain, Queue<int> input_pooltoSpawn)
        {
            row = r;
            coloumn = c;
            valuetoObtain = input_valuetoObtain;
            tilespawnPool = input_pooltoSpawn;
            swipes=0;
            //id
            id = numberIncreasing;
            numberIncreasing++;
            //_children = new List<Board>();
        }
        public Board(Board b) //update listings
        {
            
            row = b.row;
            coloumn = b.coloumn;
            valuetoObtain = b.valuetoObtain;
            tilespawnPool = new Queue<int>(b.tilespawnPool);
            GameBoard = new int[row, coloumn];
            Array.Copy(b.GameBoard, GameBoard, row * coloumn);
            solution = b.solution;
            swipes= b.swipes;
            //_children = new List<Board>(_children);

            //id
            id = b.id * 10 + numberIncreasing;
            numberIncreasing++;
        }

        //resets Board. fills in with input grid
        public void FillBoard(List<List<int>> input)
        {
            GameBoard = new int[row, coloumn];
            //fillBoard
            for (int yPos = 0; yPos < row; yPos++)
            {
                List<int> input_Row = input[yPos];
                for (int xPos = 0; xPos < coloumn; xPos++)
                {
                    //make coordinate, put value into property, place into board
                    //Coordinates new_c = new Coordinates(xPos, yPos);
                    //Property new_p = new Property(input_Row[xPos]);
                    GameBoard[yPos, xPos] = input_Row[xPos];
                }
            }

        }


        //----debugging methods
        public void DisplayBoard()
        {
            for (int yPos = 0; yPos < row; yPos++)
            {
                for (int xPos = 0; xPos < coloumn; xPos++)
                {
                    string displaytext = GameBoard[yPos, xPos].ToString();
                    Console.Write(displaytext);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void DebugBoard()
        {
            Console.WriteLine("in [yPos,xPos] format");
            for (int yPos = 0; yPos < row; yPos++)
            {
                for (int xPos = 0; xPos < coloumn; xPos++)
                {
                    Console.Write("[" + yPos + "," + xPos + "]");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public void DisplayBoardwithSpaces()
        {
            for (int yPos = 0; yPos < row; yPos++)
            {
                for (int xPos = 0; xPos < coloumn; xPos++)
                {
                    string displaytext = GameBoard[yPos, xPos].ToString();
                    Console.Write(displaytext + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public string DisplayBoardwithSpacesTostring()
        {
            string toReturn="";
            string displaytext="";
            for (int yPos = 0; yPos < row; yPos++)
            {
                for (int xPos = 0; xPos < coloumn; xPos++)
                {
                    displaytext = GameBoard[yPos, xPos].ToString();
                    toReturn+=displaytext+ " ";
                }
                toReturn+= "\n";
            }
            return toReturn;
        }




        public void spawn()
        {
            //top left > top right > bottom right > bottom left
            bool hasSpawned = false;
            int yPos = -1;
            int xPos = -1;
            int valuetoSpawn;
            if (GameBoard[0, 0] == 0) //top left
            {
                hasSpawned = true;
                yPos = 0;
                xPos = 0;
            }
            else if (GameBoard[0, coloumn - 1] == 0) //top right
            {
                hasSpawned = true;
                yPos = 0;
                xPos = coloumn - 1;
            }
            else if (GameBoard[row - 1, coloumn - 1] == 0) //bottom right
            {
                hasSpawned = true;
                yPos = row - 1;
                xPos = coloumn - 1;
            }
            else if (GameBoard[row - 1, 0] == 0) //bottom left
            {
                hasSpawned = true;
                yPos = row - 1;
                xPos = 0;
            }

            if (hasSpawned)
            {
                valuetoSpawn = tilespawnPool.Dequeue();
                tilespawnPool.Enqueue(valuetoSpawn);
                GameBoard[yPos, xPos] = valuetoSpawn;
            }
            //else
            //{
            //    Console.WriteLine("spawnfailed");
            //}
        }
        //returns success of move

        public bool moveBoard(Direction nDirection)
        {
            bool hasMoved = false;
            string Directionmoved = "";


            switch (nDirection)
            {

                case Direction.swipeUp:
                    Directionmoved = "U";
                    for (int j = 0; j < coloumn; j++)  //coloumn selected
                    {
                        for (int i = 0; i < row; i++)
                        {
                            for (int k = i + 1; k < row; k++)
                            {
                                //[k,j] is the target being currently looked at
                                if (GameBoard[k, j] == 0)
                                {
                                    continue;
                                }
                                else if (GameBoard[k, j] == GameBoard[i, j]) //merge
                                {
                                    hasMoved = true;
                                    GameBoard[i, j] *= 2;
                                    GameBoard[k, j] = 0;
                                    break;
                                }
                                else
                                {
                                    if (GameBoard[i, j] == 0 && GameBoard[k, j] != 0) //Move to new location
                                    {
                                        hasMoved = true;
                                        GameBoard[i, j] = GameBoard[k, j];
                                        GameBoard[k, j] = 0;
                                        i--;
                                        break;
                                    }
                                    else if (GameBoard[i, j] != 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Direction.swipeDown:
                    Directionmoved = "D";
                    for (int j = 0; j < coloumn; j++)
                    {
                        for (int i = row - 1; i >= 0; i--)
                        {
                            for (int k = i - 1; k >= 0; k--)
                            {
                                if (GameBoard[k, j] == 0)
                                {
                                    continue;
                                }
                                else if (GameBoard[k, j] == GameBoard[i, j])
                                {
                                    hasMoved = true;
                                    GameBoard[i, j] *= 2;
                                    GameBoard[k, j] = 0;
                                    break;
                                }
                                else
                                {
                                    if (GameBoard[i, j] == 0 && GameBoard[k, j] != 0)
                                    {
                                        hasMoved = true;
                                        GameBoard[i, j] = GameBoard[k, j];
                                        GameBoard[k, j] = 0;
                                        i++;
                                        break;
                                    }
                                    else if (GameBoard[i, j] != 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Direction.swipeRight:
                    Directionmoved = "R";
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = coloumn - 1; j >= 0; j--)
                        {
                            for (int k = j - 1; k >= 0; k--)
                            {
                                if (GameBoard[i, k] == 0)
                                {
                                    continue;
                                }
                                else if (GameBoard[i, k] == GameBoard[i, j])
                                {
                                    hasMoved = true;
                                    GameBoard[i, j] *= 2;
                                    GameBoard[i, k] = 0;
                                    break;
                                }
                                else
                                {
                                    if (GameBoard[i, j] == 0 && GameBoard[i, k] != 0)
                                    {
                                        hasMoved = true;
                                        GameBoard[i, j] = GameBoard[i, k];
                                        GameBoard[i, k] = 0;
                                        j++;
                                        break;
                                    }
                                    else if (GameBoard[i, j] != 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;

                case Direction.swipeLeft:
                    Directionmoved = "L";
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < coloumn; j++)
                        {
                            for (int k = j + 1; k < coloumn; k++)
                            {
                                if (GameBoard[i, k] == 0)
                                {
                                    continue;
                                }
                                else if (GameBoard[i, k] == GameBoard[i, j])
                                {
                                    hasMoved = true;
                                    GameBoard[i, j] *= 2;
                                    GameBoard[i, k] = 0;
                                    break;
                                }
                                else
                                {
                                    if (GameBoard[i, j] == 0 && GameBoard[i, k] != 0)
                                    {
                                        hasMoved = true;
                                        GameBoard[i, j] = GameBoard[i, k];
                                        GameBoard[i, k] = 0;
                                        j--;
                                        break;
                                    }
                                    else if (GameBoard[i, j] != 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
            if (hasMoved) //spawn tile
            {
                solution = solution + Directionmoved;
                CheckGoal();
                spawn();
                swipes++;
            }
            return hasMoved;
        }

        public bool CheckGoal()
        {
            for (int yPos = 0; yPos < row; yPos++)
            {
                for (int xPos = 0; xPos < coloumn; xPos++)
                {
                    int valueFound = GameBoard[yPos, xPos];
                    if (valuetoObtain == valueFound)
                    {
                        goalReached = true;
                        break;
                    }
                }
                if (goalReached)
                {
                    break;
                }
            }
            return goalReached;
        }

        public string ReturnSolution()
        {
            return solution;
        }

        public bool hasGoalReached()
        {
            return goalReached;
        }
    }

}

