﻿using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using static Gmap.ABLG.LevelGeneration;

namespace Gmap.ABLG
{
    public class LevelGenVisualizer : MonoBehaviour
    {
        public RawImage Image;

        public ELevelLayer Layers;

        private ELevelLayer _lastLayers;

        private Level level;

        void FixedUpdate()
        {
            if (_lastLayers == Layers)
                return;
            else
                UpdateTexture(this.level);                

            _lastLayers = Layers;
        }

        public Color CodeToColor(int code)
        {
            switch (code)
            {
                case LevelGeneration.CODE_EMPTY: return Color.black;
                case LevelGeneration.CODE_HALL: return Color.gray;
                case LevelGeneration.CODE_ROOM: return Color.white;
                case LevelGeneration.CODE_BOSS_ROOM: return new Color(0.25f, 0f, 0f);
                case LevelGeneration.CODE_SPAWNER: return new Color(0.0f, 0.0f, .25f);
                case LevelGeneration.CODE_PLAYER_SPAWN: return new Color(0f, 0.25f, 0f);
                case LevelGeneration.CODE_PROP: return new Color(0.25f, 0.125f, 0.05f);
                case LevelGeneration.CODE_ROOM_ITEM: return new Color(0.933f, 0.890f, 0.286f);
                case LevelGeneration.CODE_ROOM_ENEMIES: return new Color(0.933f, 0.301f, 0.286f);
                case LevelGeneration.CODE_ROOM_DICE: return new Color(0.921f, 0.286f, 0.933f);
                case LevelGeneration.CODE_ROOM_BLOOD_OATH: return new Color(0.8f, 0.141f, 0.501f);
                case LevelGeneration.CODE_ROOM_CHASE: return new Color(0.141f, 0.8f, 0.733f);
                case LevelGeneration.CODE_ROOM_KILL_CHALLENGE: return new Color(0.8f, 0.360f, 0.141f);
                case LevelGeneration.CODE_ENEMY: return new Color(0.55f, 0.25f, 0.25f);
            }

            return Color.black;
        }

        public Color CodeToColor(LevelGeneration.ECellCode code)
        {
            switch (code)
            {
                case LevelGeneration.ECellCode.Empty:             return Color.black;
                case LevelGeneration.ECellCode.Hall:              return Color.gray;
                case LevelGeneration.ECellCode.Room:              return Color.white;
                case LevelGeneration.ECellCode.BossRoom:          return new Color(0.25f, 0f, 0f);
                case LevelGeneration.ECellCode.Spawner:           return new Color(0.0f, 0.0f, .25f);
                case LevelGeneration.ECellCode.PlayerSpawn:       return new Color(0f, 0.25f, 0f);
                case LevelGeneration.ECellCode.Prop:              return new Color(0.25f, 0.125f, 0.05f);
                case LevelGeneration.ECellCode.RoomItem:          return new Color(0.933f, 0.890f, 0.286f);
                case LevelGeneration.ECellCode.RoomEnemies:       return new Color(0.933f, 0.301f, 0.286f);
                case LevelGeneration.ECellCode.RoomDice:          return new Color(0.921f, 0.286f, 0.933f);
                case LevelGeneration.ECellCode.RoomBloodOath:     return new Color(0.8f, 0.141f, 0.501f);
                case LevelGeneration.ECellCode.RoomChase:         return new Color(0.141f, 0.8f, 0.733f);
                case LevelGeneration.ECellCode.RoomKillChallenge: return new Color(0.8f, 0.360f, 0.141f);
                case LevelGeneration.ECellCode.Enemy:             return new Color(0.55f, 0.25f, 0.25f);
                case LevelGeneration.ECellCode.Door:              return Color.magenta;
            }

            return Color.black;
        }

        public static LevelGeneration.ECellCode ColorToCode(Color color)
        {
            Vector3 vc = new Vector3(color.r, color.g, color.b);
            float dt = 0.05f; // distance threshold
            Color c = color - new Color(0.25f, 0f, 0f);

            if (Vector3.Distance(vc, new Vector3(0.5f, 0.5f, 0.5f)) < dt) 
                return ECellCode.Hall;

            if (Vector3.Distance(vc, new Vector3(0f, 0f, 0f)) < dt) 
                return ECellCode.Empty;

            else if (Vector3.Distance(vc, new Vector3(1f, 1f, 1f)) < dt)                       
                return ECellCode.Room;

            else if (Vector3.Distance(vc, new Vector3(0.25f, 0f, 0f)) < dt)                       
                return ECellCode.BossRoom;

            else if (Vector3.Distance(vc, new Vector3(0f, 0f, 0.25f)) < dt)                       
                return ECellCode.Spawner;

            else if (Vector3.Distance(vc, new Vector3(0f, 0.25f, 0f)) < dt)                       
                return ECellCode.PlayerSpawn;

            else if (Vector3.Distance(vc, new Vector3(0.25f, 0.125f, 0.05f)) < dt)                       
                return ECellCode.Prop;

            else if (Vector3.Distance(vc, new Vector3(0.933f, 0.890f, 0.286f)) < dt)                       
                return ECellCode.RoomItem;

            else if (Vector3.Distance(vc, new Vector3(0.933f, 0.301f, 0.286f)) < dt)                       
                return ECellCode.RoomEnemies;

            else if (Vector3.Distance(vc, new Vector3(0.921f, 0.286f, 0.933f)) < dt)                       
                return ECellCode.RoomDice;

            else if (Vector3.Distance(vc, new Vector3(0.8f, 0.141f, 0.501f)) < dt)                       
                return ECellCode.RoomBloodOath;

            else if (Vector3.Distance(vc, new Vector3(0.141f, 0.8f, 0.733f)) < dt)                       
                return ECellCode.RoomChase;

            else if (Vector3.Distance(vc, new Vector3(0.8f, 0.360f, 0.141f)) < dt)                       
                return ECellCode.RoomKillChallenge;

            else if (Vector3.Distance(vc, new Vector3(0.55f, 0.25f, 0.25f)) < dt)                       
                return ECellCode.Enemy;

            else if (Vector3.Distance(vc, new Vector3(1f, 0.0f, 1f)) < dt)
                return ECellCode.Door;

            return ECellCode.Empty;
        }

        public Texture2D LevelToTexture(Level l)
        {
            Texture2D t = new Texture2D(l.Size.x, l.Size.y, TextureFormat.RGBA32, false);
            for (int y = 0; y < l.Size.y; y++)
            {
                for (int x = 0; x < l.Size.x; x++)
                {
                    t.SetPixel(x, y, CodeToColor(l.GetCell(x, y, Layers)));
                }
            }
            t.filterMode = FilterMode.Point;
            t.Apply();
            return t;
        }

        public void UpdateTexture(Level l)
        {
            // TODO: FIX MEM LEAK
            Texture2D txtr = LevelToTexture(l);
            Image.texture = txtr;
            this.level = l;
        }
    }
}