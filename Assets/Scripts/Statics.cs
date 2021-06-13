using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statics : MonoBehaviour{
    public static Color globR;
    public static Color globG;
    public static Color globB;
    public static Color globO;
    public static Color globC;
    public static Color globP;

    public Color R;
    public Color G;
    public Color B;
    public Color O;
    public Color C;
    public Color P;

    void Awake(){
        globR = R;
        globG = G;
        globB = B;
        globO = O;
        globC = C;
        globP = P;
    }
}
