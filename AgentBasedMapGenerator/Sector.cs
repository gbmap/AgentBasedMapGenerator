
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gmap.ABLG
{
    ////////////////////////////// 
    // Represents a sector inside a level. It has
    // also sub-sectors.
    //

    // Also, Room was a better name.
    public class Sector
    {
        private static int _sectorCount;
        public int Id { get; private set; }

        public Level Level { get; private set; }

        private Sector _parent;
        public Sector Parent
        {
            get { return _parent; }
            private set
            {
                if (_parent != null)
                    _parent.RemoveChild(this);

                _parent = value;
                if (_parent != null)
                    _parent.AddChild(this);
            }
        }

        public Vector2Int Pos { get; private set; }
        public Vector2Int Size { get; private set; }

        public List<Sector> Children;

        public List<Connector> Connectors;

        public LevelGeneration.ECellCode Code { get; private set; }

        public Sector(Level l,
                      Vector2Int p, 
                      Vector2Int sz, 
                      LevelGeneration.ECellCode code,
                      Sector parent = null) 
        {
            _sectorCount++;
            Id = _sectorCount;

            Level = l;
            Pos = p;
            Size = sz;
            Children = new List<Sector>();
            Connectors = new List<Connector>();
            Code = code;

            if (parent != null)
                Parent = parent;
            else if (l.BaseSector != null)
                Parent = l.BaseSector;

            FillSector(code);
        }
        
        public bool IsIn(Vector2Int p)
        {
            return IsIn(p.x, p.y);
        }

        public bool IsInFromGlobal(Vector2Int p)
        {
            return IsIn(p - Pos);
        }

        /*
         * Checks if a local space point is inside the sector.
         * */
        public bool IsIn(float x, float y)
        {
            return LevelGeneration.IsInBox(Pos.x + x, Pos.y + y, Pos, Size);
        }

        /*
         * Returns 
         * */
        public Sector GetChildSectorAtPosition(Vector2Int p)
        {
            return null;
        }

        public Vector2Int GetAbsolutePosition(Vector2Int p)
        {
            if (Parent == null)
            {
                return Pos + p;
            }
            return Parent.GetAbsolutePosition(Pos + p);
        }

        public LevelGeneration.ECellCode GetCell(int x, int y, ELevelLayer layer = ELevelLayer.All)
        {
            return GetCell(new Vector2Int(x, y), layer);
        }

        public LevelGeneration.ECellCode GetCell(Vector2Int p, ELevelLayer layer = ELevelLayer.All)
        {
            if (!IsIn(p))
                return LevelGeneration.ECellCode.Error;

            if (Parent == null)
                return Level.GetCell(Pos + p, layer);
            else
                return Parent.GetCell(Pos + p, layer);
        }

        /*
         * Sets a point in local space to the desired value.
         * If overwrite is true, the value will be set even if c < current value in map.
         * */
        public void SetCell(Vector2Int p,
                            LevelGeneration.ECellCode c, 
                            ELevelLayer layer = ELevelLayer.All,
                            bool overwrite = false)
        {
            if (!IsIn(p))
                return; // Do nothing.

            if (Parent == null)
                Level.SetCell(Pos + p, c, layer, overwrite);
            else
                Parent.SetCell(Pos + p, c, layer, overwrite);
        }

        public void CreateSector(Sector s)
        {
            for (int x = 0; x < s.Size.x; x++)
            {
                for (int y = 0; y < s.Size.y; y++)
                {
                    Vector2Int p = new Vector2Int(s.Pos.x + x, s.Pos.y + y);
                    SetCell(p, s.Code);
                }
            }
            s.Parent = this;
            //Children.Add(s);
        }

        public void FillSector(LevelGeneration.ECellCode code = LevelGeneration.ECellCode.Room, ELevelLayer layer = ELevelLayer.All)
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Vector2Int p = new Vector2Int(x, y);
                    SetCell(p, code, layer, true);
                }
            }

            this.Code = code;
        }

        public void DestroySector()
        {
            FillSector(LevelGeneration.CODE_EMPTY);
            if (Parent != null) Parent.RemoveChild(this);
            for (int i = 0; i < Connectors.Count; i++)
            {
                Connector c = Connectors[i];
                DestroyConnector(c);
                i--;
            }
        }

        public void DestroyConnector(Connector c)
        {
            if (!Connectors.Contains(c))
                return;

            c.From.Connectors.Remove(c);
            c.To.Connectors.Remove(c);

            Vector2Int p = c.StartPosition;
            LevelGeneration.ECellCode v = Level.GetCell(p);
            if (v == LevelGeneration.ECellCode.Hall)
                Level.SetCell(p, 0, ELevelLayer.All, true);

            for (int i = 0; i < c.Path.Count; i++)
            {
                p += LevelGeneration.DirectionToVector2(c.Path[i]);
                v = Level.GetCell(p);
                if (v == LevelGeneration.ECellCode.Hall)
                    Level.SetCell(p, 0, ELevelLayer.All, true);
            }
        }

        public void AddChild(Sector s)
        {
            Children.Add(s);
        }

        public void RemoveChild(Sector s)
        {
            Children.Remove(s);
        }

        /*
         * Returns the closest sector with the same parent.
         * */
        public Sector GetClosestSiblingSector()
        {
            if (Parent == null) return null;
            if (Parent.Children.Count == 1) return null;

            return Parent.Children.OrderBy(s => Vector2.Distance(Pos, s.Pos)).ElementAt(1);
        }

        public Sector[] GetSiblings()
        {
            if (Parent == null) return null;
            if (Parent.Children.Count == 1) return null;

            return Parent.Children.OrderBy(s => Vector2.Distance(Pos, s.Pos)).ToArray();
        }

        public Sector GetSectorAt(Vector2Int pos)
        {
            return Children.Where(s => pos.x > s.Pos.x && pos.y > s.Pos.y &&
                                       pos.x < s.Pos.x + s.Size.x && pos.y < s.Pos.y + s.Size.y).FirstOrDefault();
        }

        public HashSet<Sector> ListConnectedSectors()
        {
            return ListConnectedSectors(new HashSet<Sector>(), this);
        }

        public static HashSet<Sector> ListConnectedSectors(HashSet<Sector> sl, Sector sec)
        {
            if (sl.Contains(sec))
                return sl;

            sl.Add(sec);
            foreach (var connector in sec.Connectors)
            {
                if (connector.To != sec)
                    sl.UnionWith(ListConnectedSectors(sl, connector.To));
                else if (connector.From != sec)
                    sl.UnionWith(ListConnectedSectors(sl, connector.From));
            }

            return sl;
        }

        public static int GetNumberOfConnectedSectors(Sector sec)
        {
            return ListConnectedSectors(new HashSet<Sector>(), sec).Count;
        }

    }
}