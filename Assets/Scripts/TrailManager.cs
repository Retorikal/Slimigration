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

        private static float GetAngle(Vector2Int v1, Vector2Int v2){
            var sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
            return Vector2.Angle(v1, v2) * sign;
        }

        public void DrawTrail(Vector3Int location, ulong slimeChars, Vector3Int _direction, Vector3Int _prevDir){
            Tile targetTile = trail_I;
            TileChar tc = (TileChar)slimeChars;
            var direction = new Vector2Int(_direction.x, _direction.y);
            var prevDir = new Vector2Int(_prevDir.x, _prevDir.y);
            var nowhere = new Vector2Int(0, 0);
            var rotation = 0f;

            // Start position
            if(prevDir == nowhere) {
                Debug.Log("Start: "+ " " +prevDir + " " + direction);
                rotation = -GetAngle(direction, Vector2Int.right);
                targetTile = trail_S;
            }
            // End position (about to split)
            else if(direction == nowhere) {
                Debug.Log("End: "+ " " +prevDir + " " + direction);
                rotation = 180-GetAngle(prevDir, Vector2Int.right);
                targetTile = trail_S;
            }
            // Zero cross product
            else if(Mathf.Abs((direction.x * prevDir.y) - (direction.y * prevDir.x)) < 0.1) {
                Debug.Log("Straight: "+ " " +prevDir + " " + direction);
                rotation = GetAngle(direction, Vector2Int.right);
                targetTile = trail_I;
            }
            // Turning on a corner
            else{
                Debug.Log("Turn: "+ " " +prevDir + " " + direction);
                if(prevDir.y == -1){
                    if(direction.x == -1) rotation = -90;
                    else if(direction.x == 1) rotation = -180;
                }
                else if(prevDir.y == 1){
                    if(direction.x == -1) rotation = 0;
                    else if(direction.x == 1) rotation = -270;
                }
                else if(prevDir.x == -1){
                    if(direction.y == -1) rotation = -270;
                    else if(direction.y == 1) rotation = -180;
                }
                else if(prevDir.x == 1){
                    if(direction.y == -1) rotation = 0;
                    else if(direction.y == 1) rotation = -90;
                }

                targetTile = trail_L;
            }

            switch((TileChar)slimeChars){
                case TileChar.Red:
                    targetTile.color = Statics.globR; break;
                case TileChar.Green:
                    targetTile.color = Statics.globG; break;
                case TileChar.Blue:
                    targetTile.color = Statics.globB; break;
                case TileChar.Orange:
                    targetTile.color = Statics.globO; break;
                case TileChar.Cyan:
                    targetTile.color = Statics.globC; break;
                case TileChar.Purple:
                    targetTile.color = Statics.globP; break;
                default:
                    targetTile.color = new Color(255, 255, 255, 255); 
                    break;
            }

            targetTile.transform = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 0, rotation)));

            tmTrail.SetTile(location, targetTile);
        }
    }
}