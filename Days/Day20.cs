using System.Data;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2020.Days
{
    public static class Day20
    {
        public static void Run()
        {
            var tiles = ParseTiles(Utils.ReadFileAsString("Day20.txt")).ToList();
            Console.WriteLine($"Part 1: {Part1(tiles)}");
            Console.WriteLine($"Part 2: {Part2(tiles)}");
        }

        private static long Part1(List<Tile> tiles)
        {
            foreach (var tile in tiles)
            {
                var reversedTopEdge = tile.TopEdge.Reverse();
                var reversedBottomEdge = tile.BottomEdge.Reverse();
                var reversedLeftEdge = tile.LeftEdge.Reverse();
                var reversedRightEdge = tile.RightEdge.Reverse();

                //Can optimize by not looking at previous ones, since they are already compared
                foreach (var otherTile in tiles)
                {
                    if (tile == otherTile)
                        continue;

                    if (tile.TopTileMatch is null)
                        tile.TopTileMatch = FindTileMatch(tile.TopEdge, reversedTopEdge, otherTile, true);

                    if (tile.BottomTileMatch is null)
                        tile.BottomTileMatch = FindTileMatch(tile.BottomEdge, reversedBottomEdge, otherTile, false);

                    if (tile.LeftTileMatch is null)
                        tile.LeftTileMatch = FindTileMatch(tile.LeftEdge, reversedLeftEdge, otherTile, false);

                    if (tile.RightTileMatch is null)
                        tile.RightTileMatch = FindTileMatch(tile.RightEdge, reversedRightEdge, otherTile, true);
                }

                tile.SetTileType();
            }

            return tiles.Where(t => t.TileType == TileType.Corner).Select(t => (long)t.Id).Aggregate((id1, id2) => id1 * id2);
        }

        private static TileMatch FindTileMatch(bool[] tileEdge, IEnumerable<bool> reversedTileEdge, Tile otherTile, bool isTopOrRight)
        {
            if (Enumerable.SequenceEqual(tileEdge, otherTile.TopEdge) ||
                Enumerable.SequenceEqual(tileEdge, otherTile.RightEdge) ||
                Enumerable.SequenceEqual(reversedTileEdge, otherTile.LeftEdge) ||
                Enumerable.SequenceEqual(reversedTileEdge, otherTile.BottomEdge))
            {
                return new TileMatch(otherTile.Id, isTopOrRight ? false : true);
            }
            else if (Enumerable.SequenceEqual(tileEdge, otherTile.BottomEdge) ||
                    Enumerable.SequenceEqual(tileEdge, otherTile.LeftEdge) ||
                    Enumerable.SequenceEqual(reversedTileEdge, otherTile.TopEdge) ||
                    Enumerable.SequenceEqual(reversedTileEdge, otherTile.RightEdge))
            {
                return new TileMatch(otherTile.Id, isTopOrRight ? true : false);
            }

            return null;
        }

        private static long Part2(List<Tile> tiles)
        {
            var board = OrientAndPositionPieces(tiles);

            var image = TileBoardToImage(board);
            var imageSize = image.GetLength(0);

            var lengthOfMonster = 20;
            var heightOfMonster = 3;

            var monsterDeltas = new List<(int Y, int X)>
            {
                (0,18),
                (1,0), (1,5), (1,6), (1,11), (1,12), (1, 17), (1,18), (1, 19),
                (2,1), (2,4), (2,7), (2,10), (2,13), (2, 16)
            };

            //Maximum of 8 times to check image (4 rotations, and 4 rotations after flip)
            for (var check = 1; check <= 8; check++)
            {
                if (check == 5)
                    image = image.FlipVertical();

                var monstersFound = false;
                for (var i = 0; i < imageSize - heightOfMonster; i++)
                {
                    for (var j = 0; j < imageSize - lengthOfMonster; j++)
                    {
                        var positions = monsterDeltas.Select(d => (Y: i + d.Y, X: j + d.X)).ToList();
                        if (positions.All(d => image[d.Y][d.X] is '#'))
                        {
                            positions.ForEach(d => image[d.Y][d.X] = 'O');
                            monstersFound = true;
                        }
                    }
                }

                if (monstersFound)
                {
                    //This part was just for fun!
                    DrawImage(image);
                    return image.Select(row => row.Count(c => c is '#')).Sum();
                }

                image.RotateCounterClockwise(imageSize);
            }

            return -1;
        }

        private static void DrawImage(char[][] image)
        {
            var size = image.GetLength(0);

            var bitmap = new Bitmap(size, size);

            for (var i = 0; i < size; i++)
                for (var j = 0; j < size; j++)
                    bitmap.SetPixel(j, i, GetColor(image[i][j]));

            Color GetColor(char c) => c switch
            {
                '.' => Color.LightSeaGreen,
                '#' => Color.Aquamarine,
                'O' => Color.DarkOliveGreen
            };

            bitmap.Save("Resources/Day20_lochness.bmp");
        }

        private static Tile[][] OrientAndPositionPieces(List<Tile> tiles)
        {
            var sideEdgePieceCount = tiles.Count(t => t.TileType is TileType.Edge) / 4;
            var size = sideEdgePieceCount + 2;
            var board = new Tile[size][];

            for (var i = 0; i < size; i++)
                board[i] = new Tile[size];

            ProcessTopLeftCornerPiece();

            for (var column = 1; column <= sideEdgePieceCount; column++)
                ProcessTopEdgePiece(column);

            ProcessTopRightCornerPiece(size - 1);

            for (var row = 1; row <= sideEdgePieceCount; row++)
            {
                ProcessLeftEdgePiece(row);
                for (var column = 1; column <= sideEdgePieceCount; column++)
                    ProcessCenterPiece(row, column);

                ProcessRightEdgePiece(row, size - 1);
            }

            ProcessBottomLeftCornerPiece(size - 1);

            for (var column = 1; column < 1 + sideEdgePieceCount; column++)
                ProcessBottomEdgePiece(size - 1, column);

            ProcessBottomRightCornerPiece(size - 1, size - 1);

            return board;

            #region ProcessPieces

            void ProcessTopLeftCornerPiece()
            {
                var cornerPiece = tiles.First(t => t.TileType is TileType.Corner);

                FlipIfRequired(cornerPiece);

                while (cornerPiece.RightTileMatch is null || cornerPiece.BottomTileMatch is null)
                    cornerPiece.RotateCounterClockWise();

                board[0][0] = cornerPiece;
                tiles.Remove(cornerPiece);
            }

            void ProcessTopRightCornerPiece(int column)
            {
                var leftPiece = board[0][column - 1];
                var cornerPiece = tiles.Where(t => t.TileType is TileType.Corner).Single(e => e.Id == leftPiece.RightTileMatch.Id);

                FlipIfRequired(cornerPiece);
                FlipMatchIfRequired(cornerPiece, leftPiece);

                while (cornerPiece.LeftTileMatch is null || cornerPiece.BottomTileMatch is null)
                    cornerPiece.RotateCounterClockWise();

                if (cornerPiece.LeftTileMatch.Id != leftPiece.Id)
                {
                    cornerPiece.RotateCounterClockWise();

                    CheckIfFlipRequiredFlag(cornerPiece.LeftTileMatch);
                    CheckIfFlipRequiredFlag(cornerPiece.RightTileMatch);

                    cornerPiece.FlipVertical();
                }

                board[0][column] = cornerPiece;
                tiles.Remove(cornerPiece);
            }

            void ProcessBottomLeftCornerPiece(int row)
            {
                var topPiece = board[row - 1][0];
                var cornerPiece = tiles.Where(t => t.TileType is TileType.Corner).Single(e => e.Id == topPiece.BottomTileMatch.Id);

                FlipIfRequired(cornerPiece);
                FlipMatchIfRequired(cornerPiece, topPiece);

                while (cornerPiece.RightTileMatch is null || cornerPiece.TopTileMatch is null)
                    cornerPiece.RotateCounterClockWise();

                if (cornerPiece.TopTileMatch.Id != topPiece.Id)
                {
                    cornerPiece.RotateCounterClockWise();

                    CheckIfFlipRequiredFlag(cornerPiece.TopTileMatch);
                    CheckIfFlipRequiredFlag(cornerPiece.BottomTileMatch);

                    cornerPiece.FlipHorizontal();
                }

                board[row][0] = cornerPiece;
                tiles.Remove(cornerPiece);
            }

            void ProcessBottomRightCornerPiece(int row, int column)
            {
                var topPiece = board[row - 1][column];
                var leftPiece = board[row][column - 1];
                var cornerPiece = tiles.Where(t => t.TileType is TileType.Corner).Single();

                FlipIfRequired(cornerPiece);
                FlipMatchIfRequired(cornerPiece, topPiece);
                FlipMatchIfRequired(cornerPiece, leftPiece);

                while (cornerPiece.LeftTileMatch is null || cornerPiece.TopTileMatch is null)
                    cornerPiece.RotateCounterClockWise();

                if (cornerPiece.LeftTileMatch.Id != leftPiece.Id)
                {
                    cornerPiece.RotateCounterClockWise();

                    CheckIfFlipRequiredFlag(cornerPiece.LeftTileMatch);
                    CheckIfFlipRequiredFlag(cornerPiece.RightTileMatch);

                    cornerPiece.FlipVertical();
                }

                board[row][column] = cornerPiece;
                tiles.Remove(cornerPiece);
            }

            void ProcessTopEdgePiece(int column)
            {
                var leftPiece = board[0][column - 1];
                var edgePiece = tiles.Where(t => t.TileType is TileType.Edge).Single(e => e.Id == leftPiece.RightTileMatch.Id);

                FlipIfRequired(edgePiece);
                FlipMatchIfRequired(edgePiece, leftPiece);

                while (edgePiece.LeftTileMatch is null || edgePiece.RightTileMatch is null || edgePiece.BottomTileMatch is null)
                    edgePiece.RotateCounterClockWise();

                if (edgePiece.LeftTileMatch.Id != leftPiece.Id)
                {
                    CheckIfFlipRequiredFlag(edgePiece.TopTileMatch);
                    CheckIfFlipRequiredFlag(edgePiece.BottomTileMatch);

                    edgePiece.FlipHorizontal();
                }

                board[0][column] = edgePiece;
                tiles.Remove(edgePiece);
            }

            void ProcessLeftEdgePiece(int row)
            {
                var topPiece = board[row - 1][0];
                var edgePiece = tiles.Where(t => t.TileType is TileType.Edge).Single(e => e.Id == topPiece.BottomTileMatch.Id);

                FlipIfRequired(edgePiece);
                FlipMatchIfRequired(edgePiece, topPiece);

                while (edgePiece.TopTileMatch is null || edgePiece.BottomTileMatch is null || edgePiece.RightTileMatch is null)
                    edgePiece.RotateCounterClockWise();

                if (edgePiece.TopTileMatch.Id != topPiece.Id)
                {
                    CheckIfFlipRequiredFlag(edgePiece.LeftTileMatch);
                    CheckIfFlipRequiredFlag(edgePiece.RightTileMatch);

                    edgePiece.FlipVertical();
                }

                board[row][0] = edgePiece;
                tiles.Remove(edgePiece);
            }

            void ProcessRightEdgePiece(int row, int column)
            {
                var topPiece = board[row - 1][column];
                var leftPiece = board[row][column - 1];
                var edgePiece = tiles.Where(t => t.TileType is TileType.Edge).Single(e => e.Id == topPiece.BottomTileMatch.Id);

                FlipIfRequired(edgePiece);
                FlipMatchIfRequired(edgePiece, topPiece);
                FlipMatchIfRequired(edgePiece, leftPiece);

                while (edgePiece.TopTileMatch is null || edgePiece.BottomTileMatch is null || edgePiece.LeftTileMatch is null)
                    edgePiece.RotateCounterClockWise();

                if (edgePiece.TopTileMatch.Id != topPiece.Id)
                {
                    CheckIfFlipRequiredFlag(edgePiece.LeftTileMatch);
                    CheckIfFlipRequiredFlag(edgePiece.RightTileMatch);

                    edgePiece.FlipVertical();
                }

                board[row][column] = edgePiece;
                tiles.Remove(edgePiece);
            }

            void ProcessBottomEdgePiece(int row, int column)
            {
                var topPiece = board[row - 1][column];
                var leftPiece = board[row][column - 1];
                var edgePiece = tiles.Where(t => t.TileType is TileType.Edge).Single(e => e.Id == leftPiece.RightTileMatch.Id);

                FlipIfRequired(edgePiece);
                FlipMatchIfRequired(edgePiece, topPiece);
                FlipMatchIfRequired(edgePiece, leftPiece);

                while (edgePiece.LeftTileMatch is null || edgePiece.RightTileMatch is null || edgePiece.TopTileMatch is null)
                    edgePiece.RotateCounterClockWise();

                if (edgePiece.LeftTileMatch.Id != leftPiece.Id)
                {
                    CheckIfFlipRequiredFlag(edgePiece.TopTileMatch);
                    CheckIfFlipRequiredFlag(edgePiece.BottomTileMatch);

                    edgePiece.FlipHorizontal();
                }

                board[row][column] = edgePiece;
                tiles.Remove(edgePiece);
            }

            void ProcessCenterPiece(int row, int column)
            {
                var topPiece = board[row - 1][column];
                var leftPiece = board[row][column - 1];
                var centerPiece = tiles.Where(t => t.TileType is TileType.Center).Single(e => e.Id == leftPiece.RightTileMatch.Id);

                FlipIfRequired(centerPiece);
                FlipMatchIfRequired(centerPiece, topPiece);
                FlipMatchIfRequired(centerPiece, leftPiece);

                while (!(centerPiece.LeftTileMatch.Id == leftPiece.Id && centerPiece.TopTileMatch.Id == topPiece.Id))
                {
                    while (centerPiece.LeftTileMatch.Id != leftPiece.Id)
                        centerPiece.RotateCounterClockWise();

                    if (centerPiece.TopTileMatch.Id != topPiece.Id)
                    {
                        CheckIfFlipRequiredFlag(centerPiece.LeftTileMatch);
                        CheckIfFlipRequiredFlag(centerPiece.RightTileMatch);

                        centerPiece.FlipVertical();
                    }
                }

                board[row][column] = centerPiece;
                tiles.Remove(centerPiece);
            }

            void CheckIfFlipRequiredFlag(TileMatch otherTileMatch)
            {
                if (otherTileMatch is null)
                    return;

                var otherTileId = otherTileMatch.Id;
                var piece = tiles.SingleOrDefault(t => t.Id == otherTileId);
                if (piece is not null)
                {
                    if ((piece.TopTileMatch is not null && piece.TopTileMatch.Id == otherTileId) ||
                        piece.BottomTileMatch is not null && piece.BottomTileMatch.Id == otherTileId)
                    {
                        piece.TopTileMatch.RequiresFlip = !piece.TopTileMatch.RequiresFlip;
                        piece.BottomTileMatch.RequiresFlip = !piece.BottomTileMatch.RequiresFlip;
                    }
                    if ((piece.LeftTileMatch is not null && piece.LeftTileMatch.Id == otherTileId) ||
                        (piece.RightTileMatch is not null && piece.RightTileMatch.Id == otherTileId))
                    {
                        piece.LeftTileMatch.RequiresFlip = !piece.LeftTileMatch.RequiresFlip;
                        piece.RightTileMatch.RequiresFlip = !piece.RightTileMatch.RequiresFlip;
                    }
                }
            }

            void FlipIfRequired(Tile tile)
            {
                if ((tile.TopTileMatch is not null && tile.TopTileMatch.RequiresFlip) ||
                    (tile.BottomTileMatch is not null && tile.BottomTileMatch.RequiresFlip))
                {
                    CheckIfFlipRequiredFlag(tile.TopTileMatch);
                    CheckIfFlipRequiredFlag(tile.BottomTileMatch);

                    tile.FlipHorizontal();
                }

                if ((tile.LeftTileMatch is not null && tile.LeftTileMatch.RequiresFlip) ||
                    (tile.RightTileMatch is not null && tile.RightTileMatch.RequiresFlip))
                {
                    CheckIfFlipRequiredFlag(tile.LeftTileMatch);
                    CheckIfFlipRequiredFlag(tile.RightTileMatch);

                    tile.FlipVertical();
                }
            }

            void FlipMatchIfRequired(Tile tile, Tile otherTile)
            {
                if ((tile.TopTileMatch is not null && (tile.TopTileMatch.Id == otherTile.Id && tile.TopTileMatch.RequiresFlip)) ||
                    (tile.BottomTileMatch is not null && (tile.BottomTileMatch.Id == otherTile.Id && tile.BottomTileMatch.RequiresFlip)))
                {
                    CheckIfFlipRequiredFlag(tile.TopTileMatch);
                    CheckIfFlipRequiredFlag(tile.BottomTileMatch);

                    tile.FlipHorizontal();
                }
                else if ((tile.LeftTileMatch is not null && (tile.LeftTileMatch.Id == otherTile.Id && tile.LeftTileMatch.RequiresFlip)) ||
                        (tile.RightTileMatch is not null && (tile.RightTileMatch.Id == otherTile.Id && tile.RightTileMatch.RequiresFlip)))
                {
                    CheckIfFlipRequiredFlag(tile.LeftTileMatch);
                    CheckIfFlipRequiredFlag(tile.RightTileMatch);

                    tile.FlipVertical();
                }
            }

            #endregion
        }

        private static char[][] TileBoardToImage(Tile[][] board)
        {
            var size = board.GetLength(0);
            var imageSize = size * 8;
            var image = new char[imageSize][];

            for (var i = 0; i < imageSize; i++)
                image[i] = new char[imageSize];

            for (var rowIndex = 0; rowIndex < size; rowIndex++)
                for (var colIndex = 0; colIndex < size; colIndex++)
                    for (var i = 1; i < 9; i++)
                        for (var j = 1; j < 9; j++)
                            image[rowIndex * 8 + (i - 1)][colIndex * 8 + (j - 1)] = board[rowIndex][colIndex].Data[i][j] ? '#' : '.';

            return image;
        }

        private static IEnumerable<Tile> ParseTiles(string input)
        {
            var tiles = input.Split(Environment.NewLine + Environment.NewLine);

            foreach (var tile in tiles)
            {
                var tileData = tile.Split(Environment.NewLine);

                var id = Int32.Parse(Regex.Match(tileData.First(), @"\d+").Value);

                var data = tileData.Skip(1).Select(s => s.Select(c => c is '#').ToArray()).ToArray();

                yield return new Tile(id, data);
            }
        }

        private enum TileType
        {
            Center,
            Edge,
            Corner
        }

        private class TileMatch
        {
            public TileMatch(int id, bool requiresFlip)
            {
                Id = id;
                RequiresFlip = requiresFlip;
            }

            public int Id { get; set; }
            public bool RequiresFlip { get; set; }
        }

        private class Tile
        {
            private int _size;
            private TileType _tileType;

            public Tile(int id, bool[][] data)
            {
                Id = id;
                Data = data;
                _size = data.Length;
            }

            public int Id { get; }
            public bool[][] Data { get; set; }

            public TileType TileType => _tileType;

            public bool[] TopEdge => Data[0];
            public bool[] BottomEdge => Data[_size - 1];
            public bool[] LeftEdge => Data.Select(d => d[0]).ToArray();
            public bool[] RightEdge => Data.Select(d => d[_size - 1]).ToArray();

            public TileMatch TopTileMatch { get; set; }
            public TileMatch BottomTileMatch { get; set; }
            public TileMatch LeftTileMatch { get; set; }
            public TileMatch RightTileMatch { get; set; }

            public void SetTileType()
            {
                var count = 0;
                if (TopTileMatch is not null)
                    count++;
                if (BottomTileMatch is not null)
                    count++;
                if (LeftTileMatch is not null)
                    count++;
                if (RightTileMatch is not null)
                    count++;

                _tileType = count switch
                {
                    2 => TileType.Corner,
                    3 => TileType.Edge,
                    4 => TileType.Center,
                    _ => throw new ArgumentException("Piece cannot have one connection"),
                };
            }

            public void FlipVertical()
            {
                var tempTileMatch = TopTileMatch;
                TopTileMatch = BottomTileMatch;
                BottomTileMatch = tempTileMatch;

                if (LeftTileMatch is not null)
                    LeftTileMatch.RequiresFlip = !LeftTileMatch.RequiresFlip;

                if (RightTileMatch is not null)
                    RightTileMatch.RequiresFlip = !RightTileMatch.RequiresFlip;

                Data = Data.FlipVertical();
            }

            public void FlipHorizontal()
            {
                var tempTileMatch = LeftTileMatch;
                LeftTileMatch = RightTileMatch;
                RightTileMatch = tempTileMatch;

                if (TopTileMatch is not null)
                    TopTileMatch.RequiresFlip = !TopTileMatch.RequiresFlip;

                if (BottomTileMatch is not null)
                    BottomTileMatch.RequiresFlip = !BottomTileMatch.RequiresFlip;

                Data = Data.Select(row => row.Reverse().ToArray()).ToArray();
            }

            public void RotateCounterClockWise()
            {
                var tempTileMatch = TopTileMatch;
                TopTileMatch = RightTileMatch;
                RightTileMatch = BottomTileMatch;
                BottomTileMatch = LeftTileMatch;
                LeftTileMatch = tempTileMatch;

                Data.RotateCounterClockwise(_size);
            }

            public IEnumerable<string> GetRowsAsStrings()
            {
                for (var i = 0; i < 10; i++)
                {
                    var s = string.Empty;
                    for (var j = 0; j < 10; j++)
                        s += ($"{(Data[i][j] ? "#" : ".")}");
                    yield return s;
                }
            }

            public override string ToString()
            {
                return Id.ToString();
            }
        }

        private static T[][] FlipVertical<T>(this T[][] array) => array.Reverse().ToArray();

        private static void RotateCounterClockwise<T>(this T[][] array, int size)
        {
            for (var x = 0; x < size / 2; x++)
                for (var y = x; y < size - x - 1; y++)
                {
                    var temp = array[x][y];
                    array[x][y] = array[y][size - 1 - x];
                    array[y][size - 1 - x] = array[size - 1 - x][size - 1 - y];
                    array[size - 1 - x][size - 1 - y] = array[size - 1 - y][x];
                    array[size - 1 - y][x] = temp;
                }
        }
    }
}