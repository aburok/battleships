using System.Text.RegularExpressions;

namespace ConsoleApp1;

public class Ship
{
    public readonly string Name;
    public readonly int Size;
    
    public Cell[] Cells { get; private set; }

    public Ship(string name, int size)
    {
        Name = name;
        Size = size;
    }

    public void SetPosition(Cell start, Cell end, Board board)
    {
        Cells = board.GetCells(start, end);
        foreach (var cell in Cells)
        {
            cell.Ship = this;
        }
    }

    public bool IsSunk => Cells.All(c => c.WasShot);
}