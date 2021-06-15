using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using MEC;

using Mechanics.Tiles;
using Mechanics.Trail;

namespace Mechanics.Player{
    public class SlimeManager : MonoBehaviour{
        public GameObject gameGridObject;
        public GameObject trailGridObject;
        public GameObject baseSlime;
        public float mergeDelay;
        public Tilemap gameTM;

        List<Slime> slimes;
        TrailManager trailMan;

        void Awake(){
            slimes = new List<Slime>(gameObject.GetComponentsInChildren<Slime>());
            gameTM = gameGridObject.GetComponentInChildren<Tilemap>(true);
            trailMan = trailGridObject.GetComponent<TrailManager>();
        }

        void Start(){
            foreach(var s in slimes){
                s.Spawn(s.location);
            }    
        }

        void Update(){}

        public bool SubmitSlime(Slime s){
            slimes.Remove(s);
            s.Despawn(true);

            return(slimes.Count == 0);
        }

        public Slime GetSlimeOnCoord(Vector3Int coord){
            foreach(var s in slimes){
                if(s.location.x == coord.x && s.location.y == coord.y){
                    return s;
                }
            }    
            return null;
        }

        public void MergeSlimes(Slime s0, Slime s1){
            Debug.Log("Merge begins!");
            var newSlimeGO = Instantiate(baseSlime, gameObject.transform);
            var newSlime = newSlimeGO.GetComponent<Slime>();
            slimes.Add(newSlime);

            slimes.Remove(s0);
            slimes.Remove(s1);
            s0.Despawn(false);
            s1.Despawn(false);
            s0.DisableMove();
            s1.DisableMove();

            newSlime.componentSlime[0] = s0;
            newSlime.componentSlime[1] = s1;
            newSlime.SetColor(s0.r || s1.r, s0.g || s1.g, s0.b || s1.b);
            newSlime.Spawn(s0.location);
        } 

        public void SplitSlimes(Slime source, Vector3Int dir0, Vector3Int dir1){
            var s0 = source.componentSlime[0];
            var s1 = source.componentSlime[1];

            // If base slime, return.
            if(s0 == null || s1 == null) return;

            trailMan.DrawTrail(source.location, source.slimeChars, new Vector3Int(0, 0, 0), source.direction);

            slimes.Add(s0);
            slimes.Add(s1);
            slimes.Remove(source);

            s0.Spawn(source.location + dir0);
            s1.Spawn(source.location + dir1);
            source.Despawn(true);
            
            // Enable all slime to move after all has moved
            bool allMoved = true;

            foreach(var s in slimes)
                allMoved = allMoved && s.moved;

            if(allMoved){
                foreach(var s in slimes){
                    s.EnableMove();
                    allMoved = allMoved && s.moved;
                }
            }

        }

        // Respond to click on given target
        public bool MoveSlimes(Vector3Int target, TileChar tc, Tilemap tm){
            bool someMoved = false;
            bool allMoved = true;
            
            // Return value is whether a slime has been moved
            foreach(var s in slimes){
                var pastLoc = s.location;
                bool moved = s.MoveIfValid(target, tc);
                if(moved){
                    trailMan.DrawTrail(pastLoc, s.slimeChars, s.direction, s.prevDir);
                    someMoved = true;
                }

                allMoved = allMoved && s.moved;
            }

            // Merge routine; look for matches; exit loop, and merge
            int ic = 0;
            while(ic< 50){
                Slime s0 = null;
                Slime s1 = null;
                foreach (var s in slimes){
                    bool foundMatch = false;
                    foreach (var os in slimes){
                        if(os == s) continue;
                        if(os.location == s.location){
                            foundMatch = true;
                            s1 = s;
                            s0 = os;
                            break;
                        }
                    }
                    if (foundMatch) break;
                }    
                if(s0 == null && s1 == null) break;
                MergeSlimes(s0, s1);
                ic++;
            }
            
            // Enable all slime to move after all has moved
            if(allMoved){
                foreach(var s in slimes){
                    s.EnableMove();
                    allMoved = allMoved && s.moved;
                }
            }

            return someMoved;
        }

    }
}