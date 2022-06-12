

using System;
using UnityEngine;

namespace Gmap.ABLG
{
    public static class SectorIterator
    {
        public class SectorCellIteration
        {
            public int iterationNumber { get; set; }
            public LevelGeneration.ECellCode cell {get; set;}
            public Vector2Int cellPosition { get; set; }
            public Sector sector {get; set; }
            public ELevelLayer layer { get; set; }
        }

        public class SectorIterationArgs
        {
            public Sector Sector;
            public Action<SectorCellIteration>[] Functions;
            public ELevelLayer Layer;
        }

        /*
        *   Iterates over every cell in <sector> and
        *   runs <functions> with the cell.
        */
        public static void IterateSector(Sector sector,
                                         Action<SectorCellIteration> iterator,
                                         ELevelLayer layer = ELevelLayer.All)
        {
            IterateSector(sector, new Action<SectorCellIteration>[] { iterator }, layer);
        }

        public static void IterateSector(Sector sector, 
                                         Action<SectorCellIteration>[] functions,
                                         ELevelLayer layer = ELevelLayer.All)
        {
            Vector2Int pos = sector.Pos; //sec.GetAbsolutePosition(sec.Pos);
            Vector2Int sz = sector.Size;
            
            int i = 0;
            for (int x = 0; x < sector.Size.x; x++)
            {
                for (int y = 0; y < sector.Size.y; y++)
                {
                    Vector2Int cellPosition = new Vector2Int(x, y);
                    LevelGeneration.ECellCode cell = sector.GetCell(cellPosition, layer);

                    SectorCellIteration param = new SectorCellIteration()
                    {
                        iterationNumber = i,
                        cell = cell,
                        cellPosition = cellPosition,
                        sector = sector,
                        layer = layer
                    };
                    Array.ForEach(functions, function => function(param));

                    i++;
                }
            }
        }

                public class CheckNeighborsComparerParams
        {
            public Sector sector;
            public ELevelLayer layer;
            public LevelGeneration.ECellCode originalCell;
            public LevelGeneration.ECellCode neighborCell;
            public Vector2Int originalPosition;
            public Vector2Int neighborPosition;
            public EDirectionBitmask direction;

            public CheckNeighborsComparerParams() {}

            public CheckNeighborsComparerParams(Sector sector, 
                                                Vector2Int originalPosition,
                                                LevelGeneration.ECellCode originalCell,
                                                Vector2Int neighborPosition,
                                                LevelGeneration.ECellCode neighborCell,
                                                ELevelLayer layer,
                                                EDirectionBitmask direction)
            {
                this.sector           = sector;
                this.layer            = layer;
                this.originalPosition = originalPosition;
                this.neighborPosition = neighborPosition;
                this.originalCell     = originalCell;
                this.neighborCell     = neighborCell;
                this.direction        = direction;
            }
        }

        /*
        *   Iterates over neighbors at position <p> and compares
        *   with neighbor by using the provided <comparer> function.
        *
        *   Returns a bitmask with directions where comparer == true.
        */
        public static EDirectionBitmask CheckNeighbors(Sector l,
                                                       Vector2Int p,
                                                       Func<CheckNeighborsComparerParams, bool> comparer,
                                                       ELevelLayer layer = ELevelLayer.All)
        {
            EDirectionBitmask   directions     = EDirectionBitmask.None;
            Vector2Int[]        vecDirections  = GenerateNeighborOffsets(p);
            EDirectionBitmask[] enumDirections = GenerateNeighborDirections();
            
            LevelGeneration.ECellCode originalCell = l.GetCell(p, layer);

            for (int i = 0; i < vecDirections.Length; i++)
            {
                LevelGeneration.ECellCode eDir = l.Level.GetCell(l.GetAbsolutePosition(vecDirections[i]), layer);
                CheckNeighborsComparerParams param = new CheckNeighborsComparerParams
                {
                    sector           = l,
                    layer            = layer,
                    originalPosition = p,
                    originalCell     = originalCell,
                    neighborPosition = vecDirections[i],
                    neighborCell     = eDir,
                    direction        = enumDirections[i]
                };

                if (comparer(param))
                    DirectionHelper.Set(ref directions, enumDirections[i]);
            }

            return directions;
        }

        public static EDirectionBitmask CheckNeighborsGlobal(Sector sec,
                                                            Vector2Int pos,
                                                            Func<CheckNeighborsComparerParams, bool> comparer,
                                                            ELevelLayer layer = ELevelLayer.All)
        {
            return CheckNeighbors(sec.Level.BaseSector, sec.GetAbsolutePosition(pos), comparer, layer);
        }

        private static Vector2Int[] GenerateNeighborOffsets(Vector2Int p)
        {
            Vector2Int[] vecDirections = {
                new Vector2Int(p.x, p.y - 1),
                new Vector2Int(p.x, p.y + 1),
                new Vector2Int(p.x-1, p.y),
                new Vector2Int(p.x+1, p.y)
            };
            return vecDirections;
        }

        private static EDirectionBitmask[] GenerateNeighborDirections()
        {
            EDirectionBitmask[] enumDirections = {
                EDirectionBitmask.Down,
                EDirectionBitmask.Up,
                EDirectionBitmask.Left,
                EDirectionBitmask.Right
            };
            return enumDirections;
        }
    }
}