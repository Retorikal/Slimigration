using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Mechanics.Player;

namespace Mechanics.Tiles{
    public class GridBehaviour : MonoBehaviour{
        public GameObject slimeManObject;
        public GameObject blockTilesObject;
        public GameObject trailTilesObject;
        public Tile[] tileList;
        public Tile[] goalTiles;

        public bool acceptingInput = true;
        bool primeAcceptInput = true;

        Dictionary<Tile, TileChar> tileCharDict;
        Tilemap tm;
        Tilemap tmTrail;
        Tilemap tmBlock;
        SlimeManager sm;
        SlimeSplitSelector ssSelect;

        void Awake(){
            tileCharDict = new Dictionary<Tile, TileChar>();
            tileCharDict.Add(tileList[0], TileChar.Red);
            tileCharDict.Add(tileList[1], TileChar.Green);
            tileCharDict.Add(tileList[2], TileChar.Blue);
            tileCharDict.Add(tileList[3], TileChar.Red | TileChar.Green); // O
            tileCharDict.Add(tileList[4], TileChar.Red | TileChar.Blue); // P
            tileCharDict.Add(tileList[5], TileChar.Green | TileChar.Blue); // C
            foreach(var t in goalTiles){
                tileCharDict.Add(t, TileChar.Goal | TileChar.AllColor); // C
            }

            ssSelect = GameObject.FindGameObjectWithTag("SplitSelector").GetComponent<SlimeSplitSelector>();
            sm = slimeManObject.GetComponent<SlimeManager>();
            tm = gameObject.GetComponentInChildren<Tilemap>(true);
            tmBlock = blockTilesObject.GetComponentInChildren<Tilemap>(true);
            tmTrail = trailTilesObject.GetComponentInChildren<Tilemap>(true);
        }

        void Update(){
            if(primeAcceptInput && !acceptingInput){
                acceptingInput = true;
                return;
            }

            if(Input.GetMouseButtonDown(0) && acceptingInput){
                var targetPos = tm.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                var slimeMoved = false;
                var tc = GetTileChar(targetPos);

                // Validate clicks that are not on blocked area
                if(IsFreeTile(targetPos)){
                    slimeMoved = sm.MoveSlimes(targetPos, tc, tm);
                }

                if(!slimeMoved){
                    var s = sm.GetSlimeOnCoord(targetPos);
                    if(s == null) 
                        return;

                    if((ulong)(GetTileChar(targetPos) & TileChar.Goal) > 0){
                        var allSubmitted = sm.SubmitSlime(s);
                        if(allSubmitted) Win();
                        return;
                    }

                    if(!s.IsBaseSlime()){
                        YieldControlSplit(s);
                        return;
                    }
                }

            }
        }

        public void Win(){
            Debug.Log("Win!");
        }

        public TileChar GetTileChar(Vector3Int location){
            var tile = tm.GetTile<Tile>(location);
            var tc = TileChar.AllColor;

            if (tileCharDict.ContainsKey(tile))
                tc = tileCharDict[tile];

            return tc;
        }

        public bool IsFreeTile(Vector3Int targetPos){
            var tile = tm.GetTile<Tile>(targetPos);
            var tileBlock = tmBlock.GetTile<Tile>(targetPos);
            var tileTrail = tmTrail.GetTile<Tile>(targetPos);
            return tile != null && tileBlock == null && tileTrail == null;
        }

        public void ReturnControlSplit(bool success){

            if(success)
                sm.SplitSlimes(ssSelect.s, ssSelect.directions[0], ssSelect.directions[1]);
            
            primeAcceptInput = true;
        }

        public void YieldControlSplit(Slime s){
            primeAcceptInput = false;
            acceptingInput = false;
            ssSelect.SelectSplit(this, s);
        }
    }

    public enum YieldReason{
        Split,
        Pause
    }

    public enum TileChar : ulong{
        None         = 0,
        Red         = 1 << 0,
        Green       = 1 << 1, 
        Blue        = 1 << 2,
        Orange      = Red | Green,
        Cyan        = Green | Blue,
        Purple      = Red | Blue,
        Corrosive   = 1 << 3,
        Slippery    = 1 << 4,
        Goal        = 1 << 5,
        AllColor   = Red | Green | Blue
    }
}