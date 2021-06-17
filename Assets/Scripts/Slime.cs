using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using DentedPixel;
using Mechanics.Tiles;

namespace Mechanics.Player{

    public class Slime : MonoBehaviour{
        public bool r;
        public bool g;
        public bool b;
        public bool slippery;
        public bool corrosive;
        public bool moved;
        public GameObject canMoveArrow;
        public ulong slimeChars;
        public Slime[] componentSlime = {null, null};
        public Vector3Int location;
        public Vector3Int facing = Vector3Int.down;

        public bool firstSpawn = true;

        public Vector3Int direction;
        public Vector3Int prevDir;

        SpriteRenderer sr;
        SpriteRenderer bgSr;
        Animator anim;
        SlimeManager sm;
        int xTweenID, yTweenID;
        bool toBeDestroyed = false;

        void Awake(){
            ulong i = 1;
            ulong o = 0;
            xTweenID = -1;
            yTweenID = -1;

            anim = gameObject.GetComponent<Animator>();
            sr = gameObject.GetComponent<SpriteRenderer>();
            sm = gameObject.GetComponentInParent<SlimeManager>();
            bgSr = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            slimeChars +=
                (slippery    ? i<<3 : o) +
                (corrosive   ? i<<4 : o);
        }

        void Start(){
            location.x = (int)transform.position.x;
            location.y = (int)transform.position.y;
            SetColor(r, g, b);
        }

        public void SetColor(bool _r, bool _g, bool _b){
            ulong i = 1;
            ulong o = 0;
            r = _r;
            g = _g;
            b = _b;

            slimeChars =   
                (r  ? i<<0 : o) +
                (g  ? i<<1 : o) +
                (b  ? i<<2 : o) +
                (slippery    ? i<<3 : o) +
                (corrosive   ? i<<4 : o);

            switch((TileChar)slimeChars){
                case TileChar.Red:
                    sr.color = Statics.globR; break;
                case TileChar.Green:
                    sr.color = Statics.globG; break;
                case TileChar.Blue:
                    sr.color = Statics.globB; break;
                case TileChar.Orange:
                    sr.color = Statics.globO; break;
                case TileChar.Cyan:
                    sr.color = Statics.globC; break;
                case TileChar.Purple:
                    sr.color = Statics.globP; break;
                default:
                    sr.color = new Color(255, 255, 255, 255); 
                    break;
            }
        }

        public bool MoveIfValid(Vector3Int target, TileChar tc){
            if(IsAdjacent(target) && TargetTileMatch(tc) && !moved){
                // Determine moving direction
                DisableMove();
                facing = (target - location);
                facing.z = 0;
                location = target;
                var worldTarget = GetWorldCoord();

                prevDir = direction;
                direction = facing;

                switch(facing.y){
                    case 1:
                        anim.SetTrigger("W");
                        break;
                    case -1:
                        anim.SetTrigger("S"); 
                        break;
                }

                switch(facing.x){
                    case 1:
                        anim.SetTrigger("A"); 
                        sr.flipX = false;
                        bgSr.flipX = false;
                        break;
                    case -1:
                        anim.SetTrigger("A");
                        sr.flipX = true;
                        bgSr.flipX = true;
                        break;
                }

                if(xTweenID == -1 || LeanTween.isTweening(xTweenID)) LeanTween.cancel(xTweenID);
                if(yTweenID == -1 || LeanTween.isTweening(yTweenID)) LeanTween.cancel(yTweenID);

                xTweenID = LeanTween.moveX(gameObject, worldTarget.x, 0.75f).id;
                yTweenID = LeanTween.moveY(gameObject, worldTarget.y, 0.75f).id;

                return true;
            }
            return false;
        }

        public Vector3 GetWorldCoord(){
            return sm.gameTM.CellToWorld(location) + new Vector3(0.5f, 0.5f, -3f);
        }

        // Enter the game on given location and play animation.
        public void Spawn(Vector3Int targetLoc){
            Debug.Log("Spawn!" + sr.color);

            location = targetLoc;
            transform.position = GetWorldCoord();

            sr.enabled = true;
            bgSr.enabled = true;
            gameObject.SetActive(true);
            anim.SetTrigger("Spawn");
            // Play spawn animation
        }

        // Remove from game. If permanent, bool destroy = true.
        public void Despawn(bool destroy){
            direction = new Vector3Int(0, 0, 0);
            prevDir = new Vector3Int(0, 0, 0);
            Debug.Log("DeSpawn!" + sr.color);

            toBeDestroyed = destroy;
            anim.ResetTrigger("Spawn");
            anim.SetTrigger("Kill");
            // Play despawn animation

            // Move to function activated by AC
        }

        public void PostAnimDespawn(){
            if(toBeDestroyed) 
                Destroy(gameObject);
            else{ 
                sr.enabled = false;
                bgSr.enabled = false;
            }
        }

        // Enable this slime to move, play corresponding animation
        public void EnableMove(){
            Debug.Log("Enabled");
            moved = false;
            
            if (canMoveArrow != null)
            {
                canMoveArrow.SetActive(true);
            }

            bgSr.color = new Color(255, 255, 255, 255);
            // change sprite
        }

        public void DisableMove(){
            moved = true;
            if (canMoveArrow != null)
            {
                canMoveArrow.SetActive(false);
            }

            bgSr.color = new Color(85, 85, 85, 255);
            Debug.Log("Disabled " + bgSr.color);            
        }

        // Check if this slime can go to a tile with given TileChar
        public bool TargetTileMatch(TileChar tc){
            bool colorMatch = ((ulong)(tc & TileChar.AllColor) ^ slimeChars) == 0;
            bool blankTile = (tc & TileChar.AllColor) == TileChar.AllColor;
            Debug.Log("Tile match: " + (ulong)tc + " " + slimeChars + " " + colorMatch + " " + blankTile);
            return colorMatch || blankTile;
        }

        // Check if given target coordinate is adjacent to this slime
        public bool IsAdjacent(Vector3Int target){
            var x = location.x - target.x;
            var y = location.y - target.y;
            return (x < 0 ? -x:x) + (y < 0 ? -y:y) == 1;
        }

        public bool IsBaseSlime(){
            return componentSlime[0] == null || componentSlime[1] == null;
        }
    }
}