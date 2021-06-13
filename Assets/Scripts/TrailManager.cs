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

        public Tile trail_S;
        public Tile trail_I;
        public Tile trail_L;

        Tilemap tmTrail;

        void Awake(){
            tmTrail = gameObject.GetComponentInChildren<Tilemap>(true);
        }

        public void DrawTrail(Vector3Int location, ulong slimeChars, Vector3Int _direction, Vector3Int _prevDir){
            Tile targetTile = trail_S;
            TileChar tc = (TileChar)slimeChars;
            var direction = new Vector3Int(_direction.x, _direction.y, 0);
            var prevDir = new Vector3Int(_prevDir.x, _prevDir.y, 0);
            var nowhere = new Vector3(-1, -1, -1);

            if(prevDir == nowhere) {
                var rotation = Vector3.Angle(direction, Vector3Int.left);
                targetTile = trail_S;
                targetTile.transform = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, rotation)));
            }
            else if(Vector3.Cross(direction, prevDir).magnitude < 0.1) {
                var rotation = Vector3.Angle(direction, Vector3Int.left);
                targetTile = trail_I;
                targetTile.transform = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, rotation)));
            }


            /*switch(tc){
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
            }*/


            tmTrail.SetTile(location, targetTile);
        }
    }
}