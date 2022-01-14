using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    public bool isAlive = false;

    public string deKleur;
    public int numGreenNeighbors = 0;
    public int numBlueNeighbors = 0;
    public int numLightblueNeighbors = 0;
    public int numDarkblueNeighbors = 0;
    public int numRedNeighbors = 0;
    public int numWaterNeighbors = 0;

    public int numNeighbors = 0;

    public string cellSprite;

    public SpriteRenderer spriet;

    public Sprite greenCell;

    public Sprite lightblueCell;

    public Sprite darkblueCell;

    public Sprite redCell;

    public Sprite waterCell;

    public void SetAlive (bool alive, string kleur){

        isAlive = alive;

        deKleur = kleur;

        cellSprite = GetComponent<SpriteRenderer>().name;

        greenCell = Resources.Load<Sprite>("Graphics/16 x 16 darkgreen");
        lightblueCell = Resources.Load<Sprite>("Graphics/16 x 16 lightblue");
        darkblueCell = Resources.Load<Sprite>("Graphics/16 x 16 darkblue");
        redCell = Resources.Load<Sprite>("Graphics/16 x 16 red");
        waterCell = Resources.Load<Sprite>("Graphics/16 x 16 water");


        spriet = gameObject.GetComponent<SpriteRenderer>();

        if(alive && kleur == "green"){

            GetComponent<SpriteRenderer>().enabled = true;

            spriet.sprite = greenCell;
            cellSprite = "GreenCell";

        } else if (alive && kleur == "lightblue"){
            
            GetComponent<SpriteRenderer>().enabled = true;

            spriet.sprite = lightblueCell;
            cellSprite = "LightblueCell";
            
        } else if (alive && kleur == "darkblue"){
            
            GetComponent<SpriteRenderer>().enabled = true;

            spriet.sprite = darkblueCell;
            cellSprite = "DarkblueCell";
            
        } else if (alive && kleur == "red"){
            
            GetComponent<SpriteRenderer>().enabled = true;

            spriet.sprite = redCell;
            cellSprite = "RedCell";
            
        } else if (alive && kleur == "water"){
            
            GetComponent<SpriteRenderer>().enabled = true;

            spriet.sprite = waterCell;
            cellSprite = "WaterCell";
            
        }else{

            GetComponent<SpriteRenderer>().enabled = false;

        }

    }

}