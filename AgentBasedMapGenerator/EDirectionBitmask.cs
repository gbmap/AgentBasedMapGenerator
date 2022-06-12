

using System;
using System.Linq;
using UnityEngine;

namespace Gmap.ABLG
{
    [Flags]
    public enum EDirectionBitmask
    {
        None = 0,
        Up = 1 << 1,
        Right = 1 << 2,
        Down = 1 << 3,
        Left = 1 << 4
    }

    public static class BitmaskHelper
    {
        public static bool IsSet<T>(T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        public static void Set<T>(ref T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue | flagValue);
        }

        public static void Unset<T>(ref T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue & (~flagValue));
        }
    }

    public static class DirectionHelper
    {
        public static bool IsSet(EDirectionBitmask flags, EDirectionBitmask flag) 
        {
            return BitmaskHelper.IsSet<EDirectionBitmask>(flags, flag);
        }

        public static void Set(ref EDirectionBitmask flags, EDirectionBitmask flag) 
        {
            BitmaskHelper.Set<EDirectionBitmask>(ref flags, flag);
        }

        public static void Unset(ref EDirectionBitmask flags, EDirectionBitmask flag) 
        {
            BitmaskHelper.Unset<EDirectionBitmask>(ref flags, flag);
        }

        private static System.Collections.Generic.Dictionary<EDirectionBitmask, float> DictDirectionToAngle
            = new System.Collections.Generic.Dictionary<EDirectionBitmask, float>
        {
            { EDirectionBitmask.Up, 180f },
            { EDirectionBitmask.Right, -90f },
            { EDirectionBitmask.Down, 0f },
            { EDirectionBitmask.Left, 90f }
        };

        private static System.Collections.Generic.Dictionary<EDirectionBitmask, Vector2Int> DictDirectionToPrefabOffset 
            = new System.Collections.Generic.Dictionary<EDirectionBitmask, Vector2Int>
        {
            { EDirectionBitmask.Up, Vector2Int.up+Vector2Int.left },
            { EDirectionBitmask.Right, Vector2Int.up },
            { EDirectionBitmask.Down, Vector2Int.zero },
            { EDirectionBitmask.Left, Vector2Int.left }
        };

        private static System.Collections.Generic.Dictionary<EDirectionBitmask, Vector2Int> DictDirectionToOffset 
            = new System.Collections.Generic.Dictionary<EDirectionBitmask, Vector2Int>
        {
            { EDirectionBitmask.Up, Vector2Int.up },
            { EDirectionBitmask.Right, Vector2Int.right },
            { EDirectionBitmask.Down, Vector2Int.down },
            { EDirectionBitmask.Left, Vector2Int.left }
        };

        private static System.Collections.Generic.Dictionary<EDirectionBitmask, Vector3> DictDirectionToOffset3d 
            = new System.Collections.Generic.Dictionary<EDirectionBitmask, Vector3>
        {
            { EDirectionBitmask.Up, Vector3.forward },
            { EDirectionBitmask.Right, Vector3.right },
            { EDirectionBitmask.Down, Vector3.back },
            { EDirectionBitmask.Left, Vector3.left }
        };

        

        public static float ToAngle(EDirectionBitmask dir)
        {
            return DictDirectionToAngle[dir];
        }

        public static Vector2Int ToPrefabOffset(EDirectionBitmask dir)
        {
            return DictDirectionToPrefabOffset[dir];
        }

        public static Vector2Int ToOffset(EDirectionBitmask dir)
        {
            return DictDirectionToOffset[dir];
        }

        public static Vector3 ToOffset3D(EDirectionBitmask dir)
        {
            return DictDirectionToOffset3d[dir];
        }

        public static EDirectionBitmask[] GetValues()
        {
            return Enum.GetValues(typeof(EDirectionBitmask)).Cast<EDirectionBitmask>().ToArray();
        }

        public static string GetName(EDirectionBitmask direction)
        {
            return direction.ToString();
        }

        public static string ToString(EDirectionBitmask mask)
        {
            string str = "";
            foreach (var value in GetValues())
            {
                int v = IsSet(mask, value) ? 1 : 0;
                str += v;
            }
            return str;
        }
    }

}