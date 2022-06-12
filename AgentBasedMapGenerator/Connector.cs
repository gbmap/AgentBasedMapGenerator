using System.Collections.Generic;
using UnityEngine;

namespace Gmap.ABLG
{
    public enum EDirection 
    {
        Up,
        Right,
        Down,
        Left
    } // DON'T CHANGE THIS ORDER 

    public class Connector
    {
        public Sector From; 
        public Sector To;
        public Vector2Int StartPosition; // Absolute position
        public Vector2Int EndPosition;  // absolute position
        public List<EDirection> Path; // Path is relative movements.

        public Connector(Vector2Int start, Vector2Int end)
        {
            StartPosition = start;
            EndPosition = end;
            Path = new List<EDirection>();
        }

        public Connector(Sector from, Sector to)
        {
            From = from;
            To = to;
            Path = new List<EDirection>();
        }
    }
}