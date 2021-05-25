using cmdblockbuster.Common;
using cmdblockbuster.Tetrominoes;
using System;

namespace cmdblockbuster
{
    class Program
    {
        static void Main(string[] args)
        {
            ////new TetrominoO().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();
            ////new TetrominoT().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();
            ////new TetrominoL().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();
            ////new TetrominoS().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();
            ////new TetrominoZ().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();
            ////new TetrominoJ().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();
            ////new TetrominoI().CellsWithoutEmptyRowsAndColumns.Print();
            ////Console.WriteLine();

            var game = new Game();
            game.Start();
        }
    }
}
