namespace TicTacToe
{
    internal class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                string[,] gameBoardData = new string[3, 3];
                ClearGameBoardData(gameBoardData);

                int selectedColumn = 0;
                int selectedRow = 0;
                bool isPlayerVictorious = false;
                bool isComputerVictorious = false;

                while (true)
                {
                    Console.Clear();
                    DisplayGameBoard(gameBoardData, selectedRow, selectedColumn);

                    bool isGameBoardDataFilled = IsGameBoardDataFilled(gameBoardData);
                    if (isPlayerVictorious)
                    {
                        Console.WriteLine("Player won.");
                    }
                    else if (isComputerVictorious)
                    {
                        Console.WriteLine("Computer won.");
                    }
                    else if (isGameBoardDataFilled)
                    {
                        Console.WriteLine("Draw.");
                    }


                    ConsoleKey key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            if (selectedRow > 0)
                                selectedRow--;
                            break;
                        case ConsoleKey.DownArrow:
                            if (selectedRow < gameBoardData.GetLength(0) - 1)
                                selectedRow++;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (selectedColumn > 0)
                                selectedColumn--;
                            break;
                        case ConsoleKey.RightArrow:
                            if (selectedColumn < gameBoardData.GetLength(1) - 1)
                                selectedColumn++;
                            break;
                        case ConsoleKey.Enter:
                            if (isComputerVictorious || isPlayerVictorious || isGameBoardDataFilled)
                            {
                                ClearGameBoardData(gameBoardData);

                                isPlayerVictorious = false;
                                isComputerVictorious = false;
                                break;
                            }
                            List<int[]> winningMoves = GetWinningMoves(gameBoardData, "X");
                            if (TryMakePlayerMove(gameBoardData, selectedRow, selectedColumn))
                            {
                                isPlayerVictorious = ContainsMove(winningMoves, selectedRow, selectedColumn);
                                if (isPlayerVictorious)
                                {
                                    break;
                                }
                                List<int[]> computerWinningMoves = GetWinningMoves(gameBoardData, "0");
                                if (TryMakeComputerMove(gameBoardData, out int[] moveCoordinates))
                                {
                                    isComputerVictorious = ContainsMove(computerWinningMoves, moveCoordinates[0], moveCoordinates[1]);
                                    if (isComputerVictorious)
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case ConsoleKey.Q:
                            Environment.Exit(0);
                            break;
                    }
                }
            }
        }

        private static void ClearGameBoardData(string[,] gameBoardData)
        {
            for (int row = 0; row < gameBoardData.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoardData.GetLength(1); col++)
                {
                    gameBoardData[row, col] = " ";
                }
            }
        }

        static void DisplayGameBoard(string[,] gameBoardData, int selectedRow, int selectedCol)
        {
            Console.WriteLine("TicTacToe");
            Console.WriteLine("=========");

            for (int row = 0; row < gameBoardData.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoardData.GetLength(1); col++)
                {
                    if (row == selectedRow && col == selectedCol)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.Write($" {gameBoardData[row, col]} ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            Console.WriteLine("=========");
        }

        static bool TryMakePlayerMove(string[,] gameBoardData, int selectedRow, int selectedCol, string symbol = "X")
        {
            if (string.IsNullOrWhiteSpace(gameBoardData[selectedRow, selectedCol]))
            {
                gameBoardData[selectedRow, selectedCol] = symbol;
                return true;
            }

            return false;
        }

        static bool TryMakeComputerMove(string[,] gameBoardData, out int[] moveCoordinates)
        {
            List<int[]> winningMoves = GetWinningMoves(gameBoardData, "0");

            if (TryGetRandomMove(winningMoves, out int[] winningMoveCoordinates))
            {
                gameBoardData[winningMoveCoordinates[0], winningMoveCoordinates[1]] = "0";
                moveCoordinates = winningMoveCoordinates;
                return true;
            }

            List<int[]> blockingMoves = GetWinningMoves(gameBoardData, "X");

            if (TryGetRandomMove(blockingMoves, out int[] blockingMoveCoordinates))
            {
                gameBoardData[blockingMoveCoordinates[0], blockingMoveCoordinates[1]] = "0";
                moveCoordinates = blockingMoveCoordinates;
                return true;
            }

            List<int[]> moves = GetAllMoves(gameBoardData);

            if (TryGetRandomMove(moves, out int[] randomMoveCoordinates))
            {
                gameBoardData[randomMoveCoordinates[0], randomMoveCoordinates[1]] = "0";
                moveCoordinates = randomMoveCoordinates;
                return true;
            }

            moveCoordinates = Array.Empty<int>();
            return false;
        }

        static bool TryGetRandomMove(List<int[]> moves, out int[] moveCoordinates)
        {
            if (moves.Count <= 0)
            {
                moveCoordinates = Array.Empty<int>();
                return false;
            }

            int randomMoveIndex = new Random().Next(0, moves.Count - 1);
            moveCoordinates = moves[randomMoveIndex];
            return true;
        }

        static List<int[]> GetWinningMoves(string[,] gameBoardData, string playerSymbol)
        {
            List<int[]> verticalWinningMoves = GetVerticalWinningMoves(gameBoardData, playerSymbol);
            List<int[]> horizontalWinningMoves = GetHorizontalWinningMoves(gameBoardData, playerSymbol);
            List<int[]> diagonalWinningMoves = GetDiagonalWinningMoves(gameBoardData, playerSymbol);

            List<int[]> winningMoves = new List<int[]>();
            winningMoves.AddRange(verticalWinningMoves);
            winningMoves.AddRange(horizontalWinningMoves);
            winningMoves.AddRange(diagonalWinningMoves);

            return winningMoves;
        }

        private static List<int[]> GetHorizontalWinningMoves(string[,] gameBoardData, string playerSymbol)
        {
            List<int[]> winningMoves = new List<int[]>();

            for (int selectedRow = 0; selectedRow < gameBoardData.GetLength(0); selectedRow++)
            {
                int symbolCount = 0;
                bool emptySpaceFound = false;

                int emptySpaceRow = -1;
                int emptySpaceColumn = -1;

                for (int selectedColumn = 0; selectedColumn < gameBoardData.GetLength(1); selectedColumn++)
                {
                    if (gameBoardData[selectedRow, selectedColumn] == " ")
                    {
                        emptySpaceRow = selectedRow;
                        emptySpaceColumn = selectedColumn;
                        emptySpaceFound = true;
                    }

                    if (gameBoardData[selectedRow, selectedColumn] == playerSymbol)
                    {
                        symbolCount++;
                    }
                }

                bool isWinningMove = symbolCount >= gameBoardData.GetLength(0) - 1;

                if (isWinningMove && emptySpaceFound)
                {
                    winningMoves.Add(new int[] { emptySpaceRow, emptySpaceColumn });
                }
            }

            return winningMoves;
        }

        private static List<int[]> GetVerticalWinningMoves(string[,] gameBoardData, string playerSymbol)
        {
            List<int[]> winningMoves = new List<int[]>();

            for (int selectedColumn = 0; selectedColumn < gameBoardData.GetLength(1); selectedColumn++)
            {
                int symbolCount = 0;
                bool emptySpaceFound = false;

                int emptySpaceRow = -1;
                int emptySpaceColumn = -1;

                for (int selectedRow = 0; selectedRow < gameBoardData.GetLength(0); selectedRow++)
                {
                    if (gameBoardData[selectedRow, selectedColumn] == " ")
                    {
                        emptySpaceRow = selectedRow;
                        emptySpaceColumn = selectedColumn;
                        emptySpaceFound = true;
                    }

                    if (gameBoardData[selectedRow, selectedColumn] == playerSymbol)
                    {
                        symbolCount++;
                    }
                }

                bool isWinningMove = symbolCount >= gameBoardData.GetLength(1) - 1;
                if (isWinningMove && emptySpaceFound)
                {
                    winningMoves.Add(new int[] { emptySpaceRow, emptySpaceColumn });
                }
            }

            return winningMoves;
        }

        private static List<int[]> GetDiagonalWinningMoves(string[,] gameBoardData, string playerSymbol)
        {
            List<int[]> winningMoves = new List<int[]>();

            int diagonalSymbolCount = 0;
            bool emptySpaceFound = false;

            int emptySpaceRow = -1;
            int emptySpaceColumn = -1;

            for (int selectedColumn = 0; selectedColumn < gameBoardData.GetLength(1); selectedColumn++)
            {
                for (int selectedRow = 0; selectedRow < gameBoardData.GetLength(0); selectedRow++)
                {
                    if (selectedColumn == selectedRow)
                    {
                        if (gameBoardData[selectedRow, selectedColumn] == " ")
                        {
                            emptySpaceFound = true;
                            emptySpaceRow = selectedRow;
                            emptySpaceColumn = selectedColumn;
                        }

                        if (gameBoardData[selectedRow, selectedColumn] == playerSymbol)
                        {
                            diagonalSymbolCount++;
                        }

                        bool isWinningMove = diagonalSymbolCount >= gameBoardData.GetLength(0) - 1;
                        if (isWinningMove && emptySpaceFound)
                        {
                            winningMoves.Add(new int[] { emptySpaceRow, emptySpaceColumn });
                        }
                    }
                }
            }

            diagonalSymbolCount = 0;
            emptySpaceFound = false;

            for (int selectedColumn = 0; selectedColumn < gameBoardData.GetLength(1); selectedColumn++)
            {
                for (int selectedRow = 0; selectedRow < gameBoardData.GetLength(0); selectedRow++)
                {
                    if (selectedColumn == gameBoardData.GetLength(1) - selectedRow - 1)
                    {
                        if (gameBoardData[selectedRow, selectedColumn] == " ")
                        {
                            emptySpaceFound = true;
                            emptySpaceRow = selectedRow;
                            emptySpaceColumn = selectedColumn;
                        }

                        if (gameBoardData[selectedRow, selectedColumn] == playerSymbol)
                        {
                            diagonalSymbolCount++;
                        }

                        bool isWinningMove = diagonalSymbolCount >= gameBoardData.GetLength(0) - 1;
                        if (isWinningMove && emptySpaceFound)
                        {
                            winningMoves.Add(new int[] { emptySpaceRow, emptySpaceColumn });
                        }
                    }
                }
            }

            return winningMoves;
        }

        static List<int[]> GetAllMoves(string[,] gameBoardData)
        {
            List<int[]> moves = new List<int[]>();

            for (int selectedRow = 0; selectedRow < gameBoardData.GetLength(0); selectedRow++)
            {
                for (int selectedColumn = 0; selectedColumn < gameBoardData.GetLength(1); selectedColumn++)
                {
                    if (string.IsNullOrWhiteSpace(gameBoardData[selectedRow, selectedColumn]))
                    {
                        moves.Add(new int[] { selectedRow, selectedColumn });
                    }
                }
            }

            return moves;
        }

        static bool ContainsMove(List<int[]> moves, int selectedRow, int selectedColumn)
        {
            foreach (int[] winningMove in moves)
            {
                if (winningMove[0] == selectedRow && winningMove[1] == selectedColumn)
                {
                    return true;
                }
            }

            return false;
        }

        static bool IsGameBoardDataFilled(string[,] gameBoardData)
        {
            for (int row = 0; row < gameBoardData.GetLength(0); row++)
            {
                for (int column = 0; column < gameBoardData.GetLength(1); column++)
                {
                    if (string.IsNullOrWhiteSpace(gameBoardData[row, column]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
