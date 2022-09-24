using System.Text.RegularExpressions;

namespace ConsoleApp1;

public enum ShipPositionProblems
{
    InvalidCoordinates, NotInLine, WrongSize,
    CollisionWithOtherShip
}

public class ShipPositionValidator
{
    private Regex _userInput =
        new Regex("^(?<start>(?<startX>[A-J])(?<startY>[1-9]|10))[- ]?(?<end>(?<endX>[A-J])(?<endY>[1-9]|10))$");
    
    public ShipPositioningAttempt TryPositionShip(Board board, Ship ship, string position)
    {
        var positionMatch = _userInput.Match(position);
        if (positionMatch.Success is false)
        {
            return new ShipPositioningAttempt(false,
                ShipPositionProblems.InvalidCoordinates,
                "Invalid position. Correct format: <start_position><end-position>. E.g. : 'A1A4', 'A1-A4', 'A1 A4'");
        }

        var startCell = board.GetCell(positionMatch.Groups["start"]
            .Value);
        var endCell = board.GetCell(positionMatch.Groups["end"]
            .Value);

        if (startCell.InSameLine(endCell) is false)
            return new ShipPositioningAttempt(false,
                ShipPositionProblems.NotInLine,
                "Ships have to be in same horizontal or vertical line.");

        var cells = board.GetCells(startCell, endCell);
        if (cells.Count() != ship.Size)
            return new ShipPositioningAttempt(false,
                ShipPositionProblems.WrongSize,
                $"Ship has to have exactly {ship.Size} cells. Number of cells provided: {cells.Count()}");

        if (cells.Any(c => c.HasShip))
            return new ShipPositioningAttempt(false,
                ShipPositionProblems.CollisionWithOtherShip,
                $"There is already a ship placed on given position.");

        return new ShipPositioningAttempt(true, Start: startCell, End: endCell);
    }
}

public record ShipPositioningAttempt(bool Success,
    ShipPositionProblems? Problem = null,
    string? Message = null,
    Cell? Start = null, Cell? End = null);