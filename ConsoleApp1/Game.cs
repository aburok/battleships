namespace ConsoleApp1;

public class Game
{
    Board _board = new Board();
    Board _enemyBoard = new Board();
    BoardDrawer _drawer = new BoardDrawer();
    
    ShipRandomInitializer _randomizer = new ShipRandomInitializer();
    PoorsManEnemyAI _ai = new PoorsManEnemyAI();

    private string _playerLastMove = string.Empty;
    private string _enemyLastMove = string.Empty;
    
    public void Play()
    {
        this.SetUpShips();

        while (true)
        {
            if (MakeMove())
            {
                break;
            };
        }

        Console.ReadLine();
    }

    private bool MakeMove()
    {
        DrawBoards();

        string position = null;
        while (true)
        {
            Console.WriteLine("Provide shot position: (e.g. A4)");
            position = Console.ReadLine();
            if (_enemyBoard.IsValidHit(position))
            {
                break;
            }

            Console.WriteLine("Position is incorrect. Please provide valid position e.g. A3, I5, B10, etc.");
        }

        var ship = _enemyBoard.Hit(position);
        var hit = ship is not null ? "hit!!!" : "miss...";
        _playerLastMove = $"{position} was a {hit}.";
        if (ship is not null && ship.IsSunk)
        {
            _playerLastMove += $"Enemy ship '{ship.Name}' of size {ship.Size} was sunk!";
        }
        
        if (_enemyBoard.IsWin())
        {
            Console.WriteLine("You won!");
            return true;
        }

        _enemyLastMove = _ai.EnemyHit(_board);
        
        if (_board.IsWin())
        {
            Console.WriteLine("You lost!");
            return true;
        }

        return false;
    }

    private void SetUpShips()
    {
        Console.WriteLine("Random ship placement (type 'R') or from user input (type 'U')?");
        var decision = Console.ReadLine();

        if (decision == "R")
        {
            _randomizer.InitializeRandomly(_board);
        }
        else
        {
            foreach (var ship in _board.Ships)
            {
                DrawBoards();

                Console.WriteLine(
                    $"Provide position of ${ship.Name} (${ship.Size}) by providing starting cell and end cell (e.g. A1A5)");
                var position = Console.ReadLine();

                TryToPlaceShip(ship, position);
            }
        }

        _randomizer.InitializeRandomly(_enemyBoard);
    }

    private void TryToPlaceShip(Ship ship, string? position)
    {
        while (true)
        {
            var attempt = _board.TrySetShipOnPosition(ship, position);
            Console.Write(_drawer.Draw(_board, _enemyBoard));

            if (attempt.Success is false)
            {
                Console.WriteLine("Problem with placing ship, please try again.");
                Console.WriteLine(attempt.Message);
            }
            else
            {
                return;
            }
        }
    }

    void DrawBoards()
    {
        Console.Clear();
        Console.Write(_drawer.Draw(_board, _enemyBoard));
        Console.WriteLine();
        Console.WriteLine(_playerLastMove);
        Console.WriteLine();
        Console.WriteLine(_enemyLastMove);
    }
}