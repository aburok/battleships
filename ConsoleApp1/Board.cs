using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ConsoleApp1;

public class Board
{
    public Ship[] Ships { get; set; }

    public Cell[,] Cells { get; }
    public List<Cell> CellList { get; }

    private ShipPositionValidator _validator = new ShipPositionValidator();

    public Board()
    {
        Cells = new Cell[10, 10];

        IterateOver(InitializeCell);
        IterateOver(AssignNeighbours);

        CellList = Cells.Cast<Cell>()
            .ToList();

        Ships = new Ship[]
        {
            new Ship("Destroyer 1", 4), new Ship("Destroyer 2", 4), new Ship("Battleship Bismarck", 5)
        };
    }

    public ShipPositioningAttempt TrySetShipOnPosition(Ship ship, string position)
    {
        var result = _validator.TryPositionShip(this, ship, position);
        if (result.Success)
        {
            ship.SetPosition(result.Start, result.End, this);
        }

        return result;
    }
 
    public Cell? GetCell(string position)
    {
        
        if (IsValidPosition(position) is false)
        {
            return null;
        }
        var match = _positionValidation.Match(position);
        return GetCell(match.Groups["letter"].Value, match.Groups["number"].Value);
    }

    private Cell GetCell(string letter, string number)
    {
        var startY = char.ToUpper(letter.First()) - 65;
        var startX = int.Parse(number) - 1;
        var startCell = Cells[startX, startY];
        return startCell;
    }

    private IEnumerable<int> GetRange(int startIndex, int endIndex)
    {
        var startX = Math.Min(startIndex, endIndex);
        var endX = Math.Max(startIndex, endIndex);
        return Enumerable.Range(startX, endX - startX + 1);
    }

    public Cell[] GetCells(Cell start, Cell end)
    {
        if (start.X == end.X)
        {
            return GetRange(start.Y, end.Y)
                .Select(y => Cells[start.X, y])
                .ToArray();
        }

        return GetRange(start.X, end.X)
            .Select(x => Cells[x, start.Y])
            .ToArray();
    }

    private void IterateOver(Action<int, int> action)
    {
        for (var x = 0; x < 10; x++)
        {
            for (var y = 0; y < 10; y++)
            {
                action(x, y);
            }
        }
    }

    private void InitializeCell(int x, int y)
    {
        Cells[x, y] = new Cell(x, y);
    }

    private void AssignNeighbours(int x, int y)
    {
        var cell = Cells[x, y];
        if (x > 0)
        {
            cell.Left = Cells[x - 1, y];
            cell.Neighbours.Add(cell.Left);
        }

        if (y > 0)
        {
            cell.Above = Cells[x, y - 1];
            cell.Neighbours.Add(cell.Above);
        }

        if (x < 9)
        {
            cell.Right = Cells[x + 1, y];
            cell.Neighbours.Add(cell.Right);
        }

        if (y < 9)
        {
            cell.Below = Cells[x, y + 1];
            cell.Neighbours.Add(cell.Below);
        }
    }

    private Regex _positionValidation = new Regex("^(?<letter>[A-J])(?<number>[1-9]|10)$");

    public bool IsValidPosition(string? position)
    {
        return _positionValidation.IsMatch(position);
    }

    public Ship? Hit(string? position)
    {
        var getCell = GetCell(position);
        return Hit(getCell);
    }

    public Ship? Hit(Cell cell)
    {
        cell.WasShot = true;

        return cell.Ship;
    }

    public bool IsWin()
    {
        return Ships.All(s => s.IsSunk);
    }
}