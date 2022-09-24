using System.Linq;
using ConsoleApp1;
using NUnit.Framework;
using Shouldly;

namespace BattleShips.Tests;

public class ShipPlacementTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Given_EmptyBoard_When_PlaceShipInValidPosition_Then_Success()
    {
        var board = new Board();

        var ship = board.Ships.First();

        board.TrySetShipOnPosition(ship, "A5A8");

        var start = board.GetCell("A5");
        var end = board.GetCell("A8");
        var cells = board.GetCells(start, end);

        cells.All(c => c.HasShip)
            .ShouldBeTrue();

        var occupiedCells = board.CellList.Where(c => c.HasShip);

        occupiedCells.Count()
            .ShouldBe(cells.Count());
    }

    [Test]
    public void Given_EmptyBoard_When_PlaceShipInInvalidPosition_Then_Error()
    {
        var board = new Board();

        var ship = board.Ships.First();

        board.TrySetShipOnPosition(ship, "A5A11");

        var occupiedCells = board.CellList.Where(c => c.HasShip);

        occupiedCells.ShouldBeEmpty();
    }

    [Test]
    public void Given_BoardWithOneShip_When_PlaceSecondShipInValidPosition_Then_Success()
    {
        var board = new Board();
        var ship = board.Ships.First();
        var result = board.TrySetShipOnPosition(ship, "A5A8");
        result.Success.ShouldBeTrue();
        result.Start.ShouldBe(board.Cells[4, 0]);
        result.End.ShouldBe(board.Cells[7, 0]);

        var ship2 = board.Ships.Skip(1)
            .First();

        result = board.TrySetShipOnPosition(ship2, "I4I7");
        result.Success.ShouldBeTrue();
        result.Start.ShouldBe(board.Cells[3, 8]);
        result.End.ShouldBe(board.Cells[6, 8]);

        var occupiedCells = board.CellList.Where(c => c.HasShip);

        occupiedCells.ShouldNotBeEmpty();
        occupiedCells.Count()
            .ShouldBe(8);
        occupiedCells.ShouldContain(c => (c.X == 4 && c.Y == 0));
        occupiedCells.ShouldContain(c => (c.X == 5 && c.Y == 0));
        occupiedCells.ShouldContain(c => (c.X == 6 && c.Y == 0));
        occupiedCells.ShouldContain(c => (c.X == 7 && c.Y == 0));
    }

    [Test]
    public void Given_BoardWithOneShip_When_PlaceSecondShipInInValidPosition_Then_NothingHappens()
    {
        var board = new Board();
        var ship = board.Ships.First();
        var result = board.TrySetShipOnPosition(ship, "A5A8");
        result.Success.ShouldBeTrue();

        var ship2 = board.Ships.Skip(1)
            .First();

        result = board.TrySetShipOnPosition(ship2, "A5D5");
        result.Success.ShouldBeFalse();
        result.Problem.ShouldBe(ShipPositionProblems.CollisionWithOtherShip);
    }

    [Test]
    public void Given_Board_When_PlaceTooLongShip_Then_NothingHappens()
    {
        var board = new Board();
        var ship = board.Ships.First();
        var result = board.TrySetShipOnPosition(ship, "A5A9");
        result.Success.ShouldBeFalse();
        result.Problem.ShouldBe(ShipPositionProblems.WrongSize);
    }

    [Test]
    public void Given_Board_When_PlaceNotInLine_Then_NothingHappens()
    {
        var board = new Board();
        var ship = board.Ships.First();
        var result = board.TrySetShipOnPosition(ship, "A5D8");
        result.Success.ShouldBeFalse();
        result.Problem.ShouldBe(ShipPositionProblems.NotInLine);
    }

    [Test]
    public void Given_Board_When_WrongCoordinates_Then_NothingHappens()
    {
        var board = new Board();
        var ship = board.Ships.First();
        var result = board.TrySetShipOnPosition(ship, "Z15D8");
        result.Success.ShouldBeFalse();
        result.Problem.ShouldBe(ShipPositionProblems.InvalidCoordinates);
    }
}