using UnityEngine;
using System;

namespace Gmap.ABLG
{
    [System.Serializable]
    public class LevelGenerationParams
    {
        public enum ELevelType
        {
            Dungeon,
            Cave,
            Test
        }

        public ELevelType            LevelType;
        public Vector2Int            LevelSize = new Vector2Int(50, 50);
        [Range(0f, 1f)] public float PropChance = 0.65f;
        [Range(0f, 1f)] public float EnemyChance = 0.65f;
        public bool                  GenerateMesh = false;
        public string                PreloadedLevel;
    }

    public class LevelGeneration : MonoBehaviour
    {
        public enum ECellCode
        {
            Error = -1,
            Empty,
            Hall,
            Room,
            BossRoom,
            Spawner,
            PlayerSpawn,
            Prop,
            RoomItem,
            RoomPrison,
            RoomEnemies,
            RoomDice,
            RoomBloodOath,
            RoomKillChallenge,
            RoomChase,
            Enemy,
            Door
        }

        public const int CODE_ERROR               = -1;
        public const int CODE_EMPTY               = 0;
        public const int CODE_HALL                = 1;
        public const int CODE_ROOM                = 2;
        public const int CODE_BOSS_ROOM           = 3;
        public const int CODE_SPAWNER             = 4;
        public const int CODE_PLAYER_SPAWN        = 5;
        public const int CODE_PROP                = 6;
        public const int CODE_ROOM_ITEM           = 7;
        public const int CODE_ROOM_PRISON         = 8;
        public const int CODE_ROOM_ENEMIES        = 9;
        public const int CODE_ROOM_DICE           = 10;
        public const int CODE_ROOM_BLOOD_OATH     = 11;
        public const int CODE_ROOM_KILL_CHALLENGE = 12;
        public const int CODE_ROOM_CHASE          = 13;
        public const int CODE_ENEMY               = 14;

        public LevelGenerationParams Params;

        public static Level Level;

        public bool GenerateOnStart;

        void Start()
        {
            if (GenerateOnStart)
                Generate(Params);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
                Generate(Params);
        }

        ///////////////////////
        /// GENERATION
        /// 
        public void Generate(LevelGenerationParams p,
                                    System.Action<Level, LevelGenerationParams> OnCompleted = null)
        {
            StartCoroutine(CGenerate(p, OnCompleted));
        }

        public static System.Collections.IEnumerator CGenerate(LevelGenerationParams p,
                                                       System.Action<Level, LevelGenerationParams> OnCompleted)
        {
            Debug.Log("Level generation started.");

            Level level = null;
            if (!string.IsNullOrEmpty(p.PreloadedLevel))
                level = LevelBitmapLoader.Load(p.PreloadedLevel);
            else
                level = new Level(p.LevelSize);

            UpdateVis(level);

            ILevelGenAlgo[] genSteps = {
                GetLevelGenerationAlgorithm(p),
                new LevelGenAlgoPerlinMaskAdd(ECellCode.Hall, ECellCode.Prop, 1f - p.PropChance, 0.5f, 0.5f, ELevelLayer.All),
                new LevelGenAlgoPerlinMaskAdd(ECellCode.Hall, ECellCode.Enemy, 1f - p.EnemyChance, 0.5f, 0.5f, ELevelLayer.All),
                new LevelGenAlgoAddDoors()
            };
            
            foreach (ILevelGenAlgo algo in genSteps)
                yield return algo.Run(level, UpdateVis);

            /////////////////
            /// PLAYER POSITION

            UpdateVis(level);

            OnCompleted?.Invoke(level, p);

            Debug.Log("Level generation ended.");
            Level = level;
        }

        private static ILevelGenAlgo GetLevelGenerationAlgorithm(LevelGenerationParams p)
        {
            if (!string.IsNullOrEmpty(p.PreloadedLevel))
                return new LevelGenAlgoEmpty(); 

            switch (p.LevelType)
            {
                case LevelGenerationParams.ELevelType.Dungeon: return new LevelGenAlgoWalkers();
                case LevelGenerationParams.ELevelType.Cave: return new LevelGenAlgoPerlin();
                case LevelGenerationParams.ELevelType.Test: return new LevelGenAlgoTest();
                default: return new LevelGenAlgoWalkers();
            }
        }

        public static Vector2Int SelectPlayerStartPosition(Level l)
        {
            Vector2Int startPosition = l.Size/2;
            ECellCode cell = l.GetCell(startPosition);
            if (cell != ECellCode.Empty) return startPosition;

            for (int searchRadius = 1; searchRadius < Mathf.Min(l.Size.x, l.Size.y)/2; searchRadius++)
            {
                for (int x = -searchRadius; x < searchRadius; x++)
                {
                    for (int y = -searchRadius; x < searchRadius; y++)
                    {
                        Vector2Int position = startPosition + new Vector2Int(x, y);
                        cell = l.GetCell(position);
                        if (cell != ECellCode.Empty) return position;
                    }
                }
            }

            return startPosition;
        }

        public static bool IsValidPosition(Level l, Vector2Int p)
        {
            return (p.x >= 0 && p.x < l.Size.x) &&
                 (p.y >= 0 && p.y < l.Size.y);
        }

        public static bool IsInBox(Vector2Int p, Vector2Int bp, Vector2Int bsz)
        {
            return IsInBox(p.x, p.y, bp, bsz);
        }

        public static bool IsInBox(float x, float y, Vector2Int bp, Vector2Int bsz)
        {
            return ((x >= bp.x && x < bp.x + bsz.x) &&
                    (y >= bp.y && y < bp.y + bsz.y));
        }

        public static bool AABB(Rect a, Rect b)
        {
            return a.x < b.x + b.width &&
                   a.x + a.width > b.x &&
                   a.y < b.y + b.height &&
                   a.y + a.height > b.y;
        }

        public static void SetCell(Level l, Vector2Int p, LevelGeneration.ECellCode code)
        {
            if (!IsValidPosition(l, p) || (int)code < (int)l.GetCell(p.x, p.y))
                return;

            l.SetCell(p.x, p.y, code);
        }


        public static void UpdateVis(Level l)
        {
#if UNITY_EDITOR
            LevelGenVisualizer[] vis = GameObject.FindObjectsOfType<LevelGenVisualizer>();
            System.Array.ForEach(vis, v => v.UpdateTexture(l));
#endif
        }

        public static Vector2Int DirectionToVector2(EDirection d)
        {
            // TODO: make this array a const
            return new Vector2Int[] {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            }[(int)d];
        }
    }
}
