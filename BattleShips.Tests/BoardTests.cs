using System.Linq;
using System.Security.Cryptography;
using ConsoleApp1;
using NUnit.Framework;
using Shouldly;

namespace BattleShips.Tests;

public class BoardTests
{
    [Test]
    [TestCase("A1", 0, 0)]
    [TestCase("A10", 9, 0)]
    [TestCase("J10", 9, 9)]
    [TestCase("J1", 0, 9)]
    [TestCase("F5", 4, 5)]
    public void Given_Board_When_GetCells_Then_ReturnCells(string position, int x, int y)
    {
        var board = new Board();

        var cell = board.GetCell(position);

        cell.ShouldNotBeNull();
        cell.X.ShouldBe(x);
        cell.Y.ShouldBe(y);
    }

    [Test]
    public void Given_Board_When_GetCellsOutsideBoard_Then_ReturnException()
    {
        var board = new Board();

        var cell = board.GetCell("Z5");

        cell.ShouldBeNull();
    }

    [Test]
    [TestCase("A1", "A4", new[] {0, 0, 1, 0, 2, 0, 3, 0})]
    [TestCase("I1", "I10", new[] {0, 8, 1, 8, 2, 8, 3, 8, 4, 8, 5, 8, 6, 8, 7, 8, 8, 8, 9, 8})]
    [TestCase("C3", "F3", new[] {2, 2, 2, 3, 2, 4, 2, 5})]
    public void Given_Board_When_GetCellsOutsideBoard_Then_ReturnException(string start, string end, int[] cells)
    {
        var board = new Board();

        var startCell = board.GetCell(start);
        var endCell = board.GetCell(end);
        startCell.ShouldNotBeNull();
        endCell.ShouldNotBeNull();

        var range = board.GetCells(startCell, endCell);

        var positions = cells.SplitBy(2).ToArray();
        range.Length.ShouldBe(positions.Count());

        foreach (var position in positions)
        {
            var x = position[0];
            var y = position[1];
            range.ShouldContain(c => c.X == x && c.Y == y,
                $"Should contain cell with x={x} and y={y}");
        }
    }

    [Test]
    public void Given_AllShipSunk_When_IsWin_Then_Success()
    {
        var board = new Board();

        board.TrySetShipOnPosition(board.Ships.First(), "A1A4");
        board.TrySetShipOnPosition(board.Ships.Skip(1).First(), "B1B4");
        board.TrySetShipOnPosition(board.Ships.Last(), "C1C5");

        board.IsWin().ShouldBeFalse();
        foreach (var ship in board.Ships)
        {
            ship.Cells.ToList().ForEach(s => s.WasShot = true);
        }
        
        board.IsWin().ShouldBeTrue();
    }
}