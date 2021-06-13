using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Mechanics.Tiles;

namespace Mechanics.Trail{
    public class TrailManager : MonoBehaviour{
        public Tile trailR;
        public Tile trailG;
        public Tile trailB;
        public Tile trailC;
        public Tile trailO;
        public Tile trailP;
        public Tile trailW;

        Tilemap tmTrail;

        void Awake(){
            tmTrail = gameObject.GetComponentInChildren<Tilemap>(true);
        }

        public void DrawTrail(Vector3Int location, ulong slimeChars){
            Tile targetTile = trailW;
            TileChar tc = (TileChar)slimeChars;

            switch(tc){
                case TileChar.Red:
                    targetTile = trailR;
                    break;
                case TileChar.Green:
                    targetTile = trailG;
                    break;
                case TileChar.Blue:
                    targetTile = trailB;
                    break;
                case TileChar.Red | TileChar.Green:
                    targetTile = trailO;
                    break;
                case TileChar.Green | TileChar.Blue:
                    targetTile = trailC;
                    break;
                case TileChar.Blue | TileChar.Red:
                    targetTile = trailP;
                    break;
                case TileChar.AllColor:
                    targetTile = trailW;
                    break;
            }

            tmTrail.SetTile(location, targetTile);
        }
    }
}