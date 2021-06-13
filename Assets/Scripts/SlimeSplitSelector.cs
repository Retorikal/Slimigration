using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mechanics.Tiles;
using Mechanics.Player;

public class SlimeSplitSelector : MonoBehaviour{
    public ulong[] slimeChars = {0, 0};
    public Vector3Int[] directions = {new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 0)};
    public Color[] colors = {new Color(75, 75, 75, 255), new Color(75, 75, 75, 255)};
    public Slime s;

    public GameObject wPointer;
    public GameObject aPointer;
    public GameObject sPointer;
    public GameObject dPointer;

    SpriteRenderer wSprite;
    SpriteRenderer aSprite;
    SpriteRenderer sSprite;
    SpriteRenderer dSprite;

    GridBehaviour gb;
    int selcPtr = 0;
    Color defaultColor;

    void Awake(){
        defaultColor = new Color(75, 75, 75, 100);
        wSprite = wPointer.GetComponent<SpriteRenderer>();
        aSprite = aPointer.GetComponent<SpriteRenderer>();
        sSprite = sPointer.GetComponent<SpriteRenderer>();
        dSprite = dPointer.GetComponent<SpriteRenderer>();
    }

    void ChangeWASDColor(Vector3Int dir0, Color col0, Vector3Int dir1, Color col1){
        Color wColor = defaultColor;
        Color aColor = defaultColor;
        Color sColor = defaultColor;
        Color dColor = defaultColor;

        if(dir1 == Vector3Int.up)
            wColor = col1;
        else if(dir1 == Vector3Int.left)
            aColor = col1;
        else if(dir1 == Vector3Int.down)
            sColor = col1;
        else if(dir1 == Vector3Int.right)
            dColor = col1;
        
        if(dir0 == Vector3Int.up)
            wColor = col0;
        else if(dir0 == Vector3Int.left)
            aColor = col0;
        else if(dir0 == Vector3Int.down)
            sColor = col0;
        else if(dir0 == Vector3Int.right)
            dColor = col0;

        wSprite.color = wColor;
        aSprite.color = aColor;
        sSprite.color = sColor;
        dSprite.color = dColor;
    }

    public void OnMouseOver(){
        var location = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        bool vertical = Mathf.Abs(location.y) > Mathf.Abs(location.x);
        bool center = Mathf.Abs(location.y) <= 0.5 &&  Mathf.Abs(location.x) <= 0.5;

        Vector3Int direction = new Vector3Int(
            !vertical   ? (location.x > 0 ? 1 : -1) : 0,
            vertical    ? (location.y > 0 ? 1 : -1) : 0,
            0);

        if(selcPtr == 0)
            ChangeWASDColor(direction, colors[0], direction, colors[0]);
        else
            ChangeWASDColor(directions[0], colors[0], direction, colors[1]);

    }

    public void OnMouseExit(){
        var center = new Vector3Int(0, 0, 0);

        if(selcPtr == 0)
            ChangeWASDColor(center, colors[0], center, colors[0]);
        else
            ChangeWASDColor(directions[0], colors[0], center, colors[1]);
    }

    public void OnMouseDown(){
        var location = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        bool center = Mathf.Abs(location.y) <= 0.5 &&  Mathf.Abs(location.x) <= 0.5;

        if(center){
            gb.ReturnControlSplit(false);

            // Hide
            gameObject.transform.position = new Vector3(-5f, -5f, transform.position.z);
        }
        else{
            bool vertical = Mathf.Abs(location.y) > Mathf.Abs(location.x);
            Vector3Int direction = new Vector3Int(
                !vertical   ? (location.x > 0 ? 1 : -1) : 0,
                vertical    ? (location.y > 0 ? 1 : -1) : 0,
                0);

            var targetTileLoc = s.location + direction;
            var tileChar = gb.GetTileChar(targetTileLoc);

            if(gb.IsFreeTile(targetTileLoc) && s.TargetTileMatch(tileChar)){
                directions[selcPtr] = direction;
                selcPtr++;
            }

            if(selcPtr == 2){
                gb.ReturnControlSplit(true);
                gameObject.transform.position = new Vector3(-5f, -5f, transform.position.z);
            }
        }
    }

    public void SelectSplit(GridBehaviour gbInst, Slime sInst){
        s = sInst;
        gb = gbInst;
        slimeChars[0] = s.componentSlime[0].slimeChars;
        slimeChars[1] = s.componentSlime[1].slimeChars;
        colors[0] = GetColor(slimeChars[0]);
        colors[1] = GetColor(slimeChars[1]);
        selcPtr = 0;

        // Move selector to location
        var moveLocation = s.GetWorldCoord();
        moveLocation.z = transform.position.z;
        transform.localPosition = moveLocation;
    }

    Color GetColor(ulong slimeChars){
        switch((TileChar)slimeChars){
            case TileChar.Red:
                return Statics.globR;
            case TileChar.Green:
                return Statics.globG;
            case TileChar.Blue:
                return Statics.globB;
            case TileChar.Orange:
                return Statics.globO;
            case TileChar.Cyan:
                return Statics.globC;
            case TileChar.Purple:
                return Statics.globP;
            default:
                return new Color(255, 255, 255, 255);
        }
    }

    void Update(){
        
    }
}
