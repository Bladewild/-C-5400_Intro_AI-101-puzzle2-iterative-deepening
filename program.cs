/*
* Author: ALAIN MARKUS SANTOS-TANKIA
* File: program.cs Date: 2/23/20
* Class: CS 5400 Online
* Instructor : Ricardo Morales
*
* main file
* also has id search
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using static hw1.Board;

namespace hw1
{
    class Program
    {
        static int valuetoObtain;
        public static string g_empty = "no solution";
        static Coordinates gridSize;
        static List<List<int>> puzzle_input = new List<List<int>>();
        static Queue<int> readIn_SpawnPool = new Queue<int>();

        static void Main(string[] args)
        {
            string readinFile;
            if (args.Length == 1)//one Arguements
            {
                readinFile = args[0];
            }
            else //Zero Arguements
            {
                readinFile = "puzzle1.txt";
            }
            ReadFile(readinFile);
            Board Puzzle_Board = new Board(gridSize.y, gridSize.x, valuetoObtain, readIn_SpawnPool);
            Puzzle_Board.FillBoard(puzzle_input);

            //Puzzle_Board.DisplayBoard();
            //setup time
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            ////start solving
            string foundsolution = IdSearch(ref Puzzle_Board);

            ////done
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            int time = ts.Minutes*60000000 + ts.Seconds*1000000 + ts.Milliseconds * 1000;
            Console.WriteLine(time);

            bool success = true;
            if(foundsolution == g_empty)
            {
                success = false;
            }

            if (success)
            {
                Console.WriteLine(Puzzle_Board.swipes);
                Console.WriteLine(foundsolution);
                Puzzle_Board.DisplayBoardwithSpaces();

                string filesolution = Puzzle_Board.DisplayBoardwithSpacesTostring();

                string solutiontxt="solution"+readinFile;
                File.WriteAllText(solutiontxt, String.Empty);
                using (StreamWriter file =
                    new StreamWriter(solutiontxt, true))
                {
                    file.WriteLine(time);             
                    file.WriteLine(Puzzle_Board.swipes);
                    file.WriteLine(foundsolution);
                    file.NewLine = "\r\n";
                    file.NewLine = "\r\n";
                    file.WriteLine(filesolution);
                }
            }
            else
            {
                string solutiontxt="solution"+readinFile;
                File.WriteAllText(solutiontxt, String.Empty);
                using (StreamWriter file =
                    new StreamWriter(solutiontxt, true))
                {
                    file.WriteLine(time);                    
                    file.WriteLine("Unable to solve");
                }
                Console.WriteLine("Unable to solve");
            }

        }
        private static void ReadFile(string input_textfile)
        {
            // Taking a new input stream i.e.  
            // geeksforgeeks.txt and opens it 
            StreamReader sr = new StreamReader(input_textfile);

            // This is use to specify from where  
            // to start reading input stream 
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            int[] temp_converted;
            List<int> converted;
            string[] input;
            //1st line
            input = sr.ReadLine().Split(' ');
            valuetoObtain = Int32.Parse(input[0]);
            //2nd line
            input = sr.ReadLine().Split(' ');
            gridSize = new Coordinates(Int32.Parse(input[0]), Int32.Parse(input[1]));
            //3rd line
            input = sr.ReadLine().Split(' ');
            temp_converted = Array.ConvertAll(input, int.Parse);
            converted = new List<int>(temp_converted);
            readIn_SpawnPool = new Queue<int>(converted);
            //Rest of grid
            while (sr.EndOfStream == false)
            {
                input = sr.ReadLine().Split(' ');
                temp_converted = Array.ConvertAll(input, int.Parse);
                converted = new List<int>(temp_converted);
                puzzle_input.Add(converted);
            }

            // to close the stream 
            sr.Close();

        }

        public static string IdSearch(ref Board startingBoard)
        {
            bool natural_failure = false;
            //           bound: integer
            int depth = 0;
            bool depth_hit=false;

            //failure without reaching the depth bound is failing naturally.
            //increase bounds by 1 and search till natural failure
            while (!natural_failure)
            {
                string proposedSolution = dbsearch(ref startingBoard, depth,ref depth_hit);
                if (proposedSolution != g_empty)
                {
                    return proposedSolution;
                }
                else if (depth_hit == false)
                {
                    natural_failure = true;
                }
                depth += 1;
            }
            
            return g_empty;
        }


        public static string dbsearch(ref Board startBoard, int input_bounds, ref bool depth_hit)
        {
            depth_hit = false;
            //Console.WriteLine(input_bounds);
            Stack<Board> frontier = new Stack<Board>();
            frontier.Push(startBoard);
            while (frontier.Count > 0) //not empty
            {
                Board topofStack = frontier.Pop();
                string proposedSolution = topofStack.ReturnSolution();
                if(proposedSolution == null) //base
                {
                    proposedSolution="";
                }
                if (proposedSolution.Length == input_bounds) //reached depth
                {
                    bool isGoal = topofStack.CheckGoal();
                    if (isGoal)//goal reached
                    {
                        startBoard = topofStack;
                        return proposedSolution;
                    }
                    else
                    {
                        //check has children, do not modify top of stack               
                        List<Board> children = swipeFourDirections(topofStack);
                        if (children.Count > 0)
                        {
                            depth_hit = true;
                        }
                    }
                }
                else
                {
                    List<Board> _children = swipeFourDirections(topofStack);
                    foreach(Board b in _children)
                    {
                        frontier.Push(b);
                    }
                }
            }
            return g_empty;
        }



        public static List<Board> swipeFourDirections(Board input_board)
        {
            List<Board> created_children = new List<Board>();
            bool moved;
            bool hasReached = false;

            Board temp_board = new Board(input_board);
            moved = temp_board.moveBoard(Board.Direction.swipeUp);
            if (moved)
            {
                created_children.Add(temp_board);
                hasReached = temp_board.CheckGoal();
            }
            //Down
            if (!hasReached)
            {
                temp_board = new Board(input_board);
                moved = temp_board.moveBoard(Board.Direction.swipeDown);
                if (moved)
                {
                    created_children.Add(temp_board);
                    hasReached = temp_board.CheckGoal();
                }
            }

            if (!hasReached)
            {
                //left
                temp_board = new Board(input_board);
                moved = temp_board.moveBoard(Board.Direction.swipeLeft);
                if (moved)
                {
                    created_children.Add(temp_board);
                    hasReached = temp_board.hasGoalReached();
                }
            }

            if (!hasReached)
            {
                //right
                temp_board = new Board(input_board);
                moved = temp_board.moveBoard(Board.Direction.swipeRight);
                if (moved && !hasReached)
                {
                    created_children.Add(temp_board);
                }

            }

            return created_children;
        }
    }
}
