using System.Text.RegularExpressions;

namespace ConsoleApp1;

public class Ship
{
    public readonly string Name;
    public readonly int Size;
    
    private Cell[] _cells;

    public Ship(string name, int size)
    {
        Name = name;
        Size = size;
    }

    public void SetPosition(Cell start, Cell end, Board board)
    {
        _cells = board.GetCells(start, end);
        foreach (var cell in _cells)
        {
            cell.Ship = this;
        }
    }

    public bool IsSunk => _cells.All(c => c.WasShot);
}