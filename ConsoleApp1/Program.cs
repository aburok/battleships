// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var board = new Board();
var enemyBoard = new Board();
enemyBoard.InitializeRandom();

Console.WriteLine("Random ship placement (type 'R') or from user input (type 'U')?");
var decision = Console.ReadLine();

if (decision == "R")
{
    board.InitializeRandom();
}
else
{
    foreach (var ship in board.Ships)
    {
        DrawBoards();

        Console.WriteLine(
            $"Provide position of ${ship.Name} (${ship.Size}) by providing starting cell and end cell (e.g. A1A5)");
        var position = Console.ReadLine();

        board.SetShipPosition(ship, position);
        Console.Write(board.DrawForMe());
    }
}

while (true)
{
    DrawBoards();

    Console.WriteLine("Provide shot position: (e.g. A4)");
    var position = Console.ReadLine();

    var ship = enemyBoard.Hit(position);
    var hit = ship is not null ? "hit!" : "miss...";
    Console.WriteLine($"It was a {hit}");
    Console.ReadLine();

    board.IsWin();
}

void DrawBoards()
{
    Console.Clear();
    Console.Write(board.DrawForMe());
    Console.WriteLine();
    Console.WriteLine("------------------------------");
    Console.WriteLine();
    Console.Write(enemyBoard.DrawForMe());
}

class Ship
{
    public readonly string Name;
    public readonly int Size;
    private Cell _start;
    private Cell _end;
    private Cell[] _cells;

    public Ship(string name, int size)
    {
        Name = name;
        Size = size;
    }

    private Regex _userInput =
        new Regex("^(?<start>(?<startX>[A-J])(?<startY>\\d))(?<end>(?<endX>[A-J])(?<endY>\\d))$");

    private Regex _userInputDirection =
        new Regex("^(?<start>(?<startX>[A-J])(?<startY>\\d))(?<direction>[NWES])$");

    public void SetPosition(string startAndEnd, Board board)
    {
        var match = _userInput.Match(startAndEnd);
        if (match.Success)
        {
            SetPosition(board.GetCell(match.Groups["start"]
                    .Value),
                board.GetCell(match.Groups["end"]
                    .Value),
                board);
        }
    }

    public void SetPosition(Cell start, Cell end, Board board)
    {
        _start = start;
        _end = end;

        _cells = board.GetCells(_start, _end);
        foreach (var cell in _cells)
        {
            cell.Ship = this;
        }
    }

    public bool IsSunk => _cells.All(c => c.WasShot);
}

record Cell(int X, int Y)
{
    public Ship? Ship { get; set; }

    public bool WasShot { get; set; }

    public bool HasShip => Ship is not null;
}

class Board
{
    public int Size { get; set; }
    public Ship[] Ships { get; set; }

    private readonly Cell[,] _cells;
    private List<Cell> CellList { get; }

    public Board()
    {
        _cells = new Cell[10, 10];

        IterateOver(InitializeCell);

        CellList = _cells.Cast<Cell>()
            .ToList();

        Ships = new Ship[]
        {
            new Ship("Destroyer 1", 4), new Ship("Destroyer 2", 4), new Ship("Battleship Bismarck", 5)
        };
    }

    public void SetShipPosition(Ship ship, string position)
    {
        ship.SetPosition(position, this);
    }

    public Cell[] GetCells(Cell start, Cell end)
    {
        if (start.X == end.X)
        {
            return GetRange(start.Y, end.Y)
                .Select(y => _cells[start.X, y])
                .ToArray();
        }

        return GetRange(start.X, end.X)
            .Select(x => _cells[x, start.Y])
            .ToArray();
    }

    private IEnumerable<int> GetRange(int startIndex, int endIndex)
    {
        var startX = Math.Min(startIndex, endIndex);
        var endX = Math.Max(startIndex, endIndex);
        return Enumerable.Range(startX, endX - startX + 1);
    }

    public Cell GetCell(string position)
    {
        return GetCell(position.Substring(0, 1), position.Substring(1, 1));
    }

    public Cell GetCell(string positionX, string positionY)
    {
        var startX = char.ToUpper(positionX.First()) - 65;
        var startY = int.Parse(positionY);
        var startCell = _cells[startX, startY];
        return startCell;
    }

    private void IterateOver(Action<int, int> action)
    {
        for (var vi = 0; vi < 10; vi++)
        {
            for (var hi = 0; hi < 10; hi++)
            {
                action(vi, hi);
            }
        }
    }

    private void InitializeCell(int x, int y)
    {
        _cells[x, y] = new Cell(x, y);
    }

    readonly Random _random = new Random();

    public void InitializeRandom()
    {
        foreach (var ship in Ships)
        {
            var availableCells = CellList.Where(c => c.Ship is null)
                .ToArray();
            var success = false;
            Cell? start = null;
            Cell? end = null;
            do
            {
                success = TryGetRandomCells(availableCells, ship.Size, out start, out end);
            } while (success is false);

            ship.SetPosition(start, end, this);
        }
    }

    private bool TryGetRandomCells(Cell[] availableCells, int size, out Cell? start, out Cell? end)
    {
        start = null;
        end = null;

        var randomCell = availableCells[_random.Next(availableCells.Count())];

        // 0 - North, 1 - East, 2 - South, 3 - West
        var shipOrientation = _random.Next(3);

        var startPositionX = randomCell.X;
        var startPositionY = randomCell.Y;
        var endPositionX = startPositionX;
        var endPositionY = startPositionY;

        switch (shipOrientation)
        {
            case 0:
                endPositionY = endPositionY - size + 1;
                if (endPositionY < 0) return false;
                break;
            case 1:
                endPositionX = endPositionX + size - 1;
                if (endPositionX > 9) return false;
                break;
            case 2:
                endPositionY = endPositionY + size - 1;
                if (endPositionY > 9) return false;
                break;
            case 3:
                endPositionX = endPositionX - size + 1;
                if (endPositionX < 0) return false;
                break;
        }

        var endCell = _cells[endPositionX, endPositionY];
        var cells = GetCells(randomCell, endCell);

        if (cells.All(c => c.Ship is null))
        {
            start = randomCell;
            end = endCell;
            return true;
        }

        return false;
    }

    public void EnemyHit()
    {
        var availableCells = CellList.Where(c => c.WasShot is false);
        var unsunkedShips = CellList.Where(c => c.HasShip && c.WasShot);
        
    }

    public string DrawForMe()
    {
        var builder = new StringBuilder();
        builder.AppendLine("'-' -> water");
        builder.AppendLine("'*' -> miss");
        builder.AppendLine("'O' -> ship");
        builder.AppendLine("'X' -> hit");
        builder.AppendLine();

        builder.Append("\\");
        int[] numbers = Enumerable.Range(0, 10)
            .ToArray();
        builder.Append(string.Join("", numbers));
        builder.AppendLine();

        foreach (var numbersX in numbers)
        {
            var letter = (char) (numbersX + 65);
            builder.Append(letter);
            foreach (var numberY in numbers)
            {
                var cell = _cells[numbersX, numberY];
                var hasShip = cell.Ship is not null;
                builder.Append(hasShip ? (cell.WasShot ? "X" : "O") : (cell.WasShot ? "*" : "-"));
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    public string DrawEnemy()
    {
        var builder = new StringBuilder();
        builder.AppendLine("'-' -> water");
        builder.AppendLine("'*' -> miss");
        builder.AppendLine("'X' -> hit");
        builder.AppendLine();

        builder.Append("\\");
        int[] numbers = Enumerable.Range(0, 10)
            .ToArray();
        builder.Append(string.Join("", numbers));
        builder.AppendLine();

        foreach (var numberX in numbers)
        {
            var letter = char.ToUpper((char) (numberX + 65));
            builder.Append(letter);

            foreach (var numberY in numbers)
            {
                var cell = _cells[numberX, numberY];
                if (cell.Ship is not null)
                {
                    builder.Append(cell.WasShot ? "X" : "-");
                }
                else
                {
                    builder.Append(cell.WasShot ? "*" : "-");
                }
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    public Ship? Hit(string? position)
    {
        var getCell = GetCell(position);
        getCell.WasShot = true;

        return getCell.Ship;
    }

    public bool IsWin()
    {
        return Ships.All(s => s.IsSunk);
    }
}