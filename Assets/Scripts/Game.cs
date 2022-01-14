using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class Game : MonoBehaviour
{

    private static int SCREEN_WIDTH = 64;       //1024 px
    private static int SCREEN_HEIGHT = 48;      //768 px

    public HUD hud;

    public float speed = 0.1f;

    private float timer = 0;

    public bool simulationEnabled = false;

    public int cellColour = 0;

    private int generationCounter = 0;

    Cell[,] grid = new Cell[SCREEN_WIDTH,SCREEN_HEIGHT];

    // Start is called before the first frame update
    void Start(){

        EventManager.StartListening("SavePattern", SavePattern);
        EventManager.StartListening("LoadPattern", LoadPattern);

        PlaceCells(2);

        CountNeighbors();

    }

    // Update is called once per frame
    void Update(){

        if (simulationEnabled){

            if (timer >= speed){

                generationCounter += 1;

                CountNeighbors();

                PopulationControl();

                timer = 0f;

            }else{

                timer += Time.deltaTime;

            }

        }

        UserInput();

    }

    private void LoadPattern(){

        string path = "patterns";

        if (!Directory.Exists(path)){

            return;

        }

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));

        string patternName = hud.loadDialog.patternName.options[hud.loadDialog.patternName.value].text;

        path = path + '/' + patternName + ".xml";

        StreamReader reader = new StreamReader(path);
        Pattern pattern = (Pattern)serializer.Deserialize(reader.BaseStream);

        reader.Close();

        bool isAlive;

        int x = 0, y = 0;

        Debug.Log(pattern.patternString);

        foreach (char c in pattern.patternString){

            if (c.ToString() == "1"){

                isAlive = true;
                grid[x,y].SetAlive(isAlive, "green");

            } else if (c.ToString() == "2"){
                
                isAlive = true;
                grid[x,y].SetAlive(isAlive, "lightblue");

            } else if (c.ToString() == "3"){
                
                isAlive = true;
                grid[x,y].SetAlive(isAlive, "darkblue");

            } else if (c.ToString() == "4"){
                
                isAlive = true;
                grid[x,y].SetAlive(isAlive, "red");

            } else if (c.ToString() == "5"){
                
                isAlive = true;
                grid[x,y].SetAlive(isAlive, "water");

            }else {

                isAlive = false;
                grid[x,y].SetAlive(isAlive, "dead");

            }

            x ++;
            
            if(x == SCREEN_WIDTH){

                x = 0;
                y ++;

            }

        }

    }

    private void SavePattern(){

        string path = "patterns";

        if (!Directory.Exists(path)){

            Directory.CreateDirectory(path);

        }

        Pattern pattern = new Pattern();

        string patternString = null;

        for(int y = 0; y < SCREEN_HEIGHT; y ++){

            for(int x = 0; x < SCREEN_WIDTH; x ++){

                if (grid[x,y].isAlive == true && grid[x,y].deKleur == "green"){

                    patternString += "1";

                } else if (grid[x,y].isAlive == true && grid[x,y].deKleur == "lightblue"){

                    patternString += "2";

                } else if (grid[x,y].isAlive == true && grid[x,y].deKleur == "darkblue"){

                    patternString += "3";

                } else if (grid[x,y].isAlive == true && grid[x,y].deKleur == "red"){

                    patternString += "4";

                } else if (grid[x,y].isAlive == true && grid[x,y].deKleur == "water"){

                    patternString += "5";

                } else{

                    patternString += "0";

                }

            }

        }

        pattern.patternString = patternString;

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));
        StreamWriter writer = new StreamWriter(path + "/" + hud.saveDialog.patternName.text + ".xml");

        serializer.Serialize(writer.BaseStream, pattern);

        writer.Close();

        Debug.Log(pattern.patternString);

    }

    void UserInput(){

        if (!hud.isActive){

            if (Input.GetKeyUp(KeyCode.Alpha1)){

                //choose green cells
                cellColour = 0;

            }

            if (Input.GetKeyUp(KeyCode.Alpha2)){

                //choose lightblue cells
                cellColour = 1;

            }

            if (Input.GetKeyUp(KeyCode.Alpha3)){

                //choose darkblue cells
                cellColour = 2;

            }

            if (Input.GetKeyUp(KeyCode.Alpha4)){

                //choose red cells
                cellColour = 3;

            }

            if (Input.GetKeyUp(KeyCode.Alpha5)){

                //choose water cells
                cellColour = 4;

            }

            if(Input.GetMouseButtonDown(0)){

                Debug.Log(cellColour);

                Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                int x = Mathf.RoundToInt(mousePoint.x);
                int y = Mathf.RoundToInt(mousePoint.y);

                if(x >= 0 && y >= 0 && x< SCREEN_WIDTH && y < SCREEN_HEIGHT){

                    //we are in bounds

                    if(cellColour == 0){

                        grid[x,y].SetAlive(!grid[x,y].isAlive, "green");

                    } else if(cellColour == 1){

                        grid[x,y].SetAlive(!grid[x,y].isAlive, "lightblue");

                    } else if(cellColour == 2){

                        grid[x,y].SetAlive(!grid[x,y].isAlive, "darkblue");

                    } else if(cellColour == 3){

                        grid[x,y].SetAlive(!grid[x,y].isAlive, "red");

                    } else if(cellColour == 4){

                        grid[x,y].SetAlive(!grid[x,y].isAlive, "water");

                    }

                }

            }

            if (Input.GetKeyUp(KeyCode.P)){

                //pause simulation
                simulationEnabled = false;

            }

            if(Input.GetKeyUp(KeyCode.B)){

                //build simulation / resume
                simulationEnabled = true;

            }

            if (Input.GetKeyUp(KeyCode.S)){

                //save configuration
                hud.showSaveDialog();

            }

            if(Input.GetKeyUp(KeyCode.L)){

                //Load pattern
                hud.showLoadDialog();

            }

        }

    }

    void PlaceCells (int type){

        int totalGreenCells = 0;
        int totalLightblueCells = 0;
        int totalDarkblueCells = 0;
        int totalBlueCells = 0;
        int totalRedCells = 0;
        int totalCells = 0;

        int greenCellPercent = 0;
        int lightblueCellPercent = 0;
        int darkblueCellPercent = 0;
        int blueCellPercent = 0;
        int redCellPercent = 0;
        int aliveCellPercent = 0;

        if (type == 1){

            for (int y = 0; y < SCREEN_HEIGHT; y++){

                for (int x = 0; x < SCREEN_WIDTH; x++){

                    Cell cell = Instantiate(Resources.Load("Prefabs/cell", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                    grid[x,y].SetAlive(false, "dood");

                }

            }

            for (int y = 21; y < 24; y++){

                for (int x = 31; x < 38; x++){

                    if (x != 34){

                        if(y == 22 || y == 23){

                            grid[x,y].SetAlive(true, "zwart");

                        } else if (y == 22 && ((x != 32) && (x != 36))){
                            
                            grid[x,y].SetAlive(true, "rood");                     

                        }

                    }

                }

            }

        } else if (type == 2){

            for (int y = 0; y < SCREEN_HEIGHT; y++){

                for (int x = 0; x < SCREEN_WIDTH; x++){

                    if (0.4*(x-32)*(x-32) + y < 30){

                        Cell cellWater = Instantiate(Resources.Load("Prefabs/cellWater", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                        grid[x,y] = cellWater;
                        grid[x,y].SetAlive(true, "water");

                    } else {

                        //50% kans dat een cel leeft en 50% kans dat een cel dood is
                        int randomNumber = UnityEngine.Random.Range (0, 3);

                        //als een cel leeft
                        if(randomNumber == 0){

                            int randomNumber2 = UnityEngine.Random.Range (0, 10);

                            //50% kans dat een cel groen wordt
                            if(randomNumber2 <= 4){

                                Cell cellGreen = Instantiate(Resources.Load("Prefabs/cellDarkgreen", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                                grid[x,y] = cellGreen;
                                grid[x,y].SetAlive(true, "green");

                                totalGreenCells += 1;

                            //30% kans dat een cel blauw wordt
                            } else if(randomNumber2 == 5 || randomNumber2 == 6 || randomNumber2 == 7){

                                int randomNumber3 = UnityEngine.Random.Range (0, 2);

                                //50% kans dat een cel lichtblauw wordt
                                if(randomNumber3 == 0){

                                    Cell cellLightblue = Instantiate(Resources.Load("Prefabs/cellLightblue", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                                    grid[x,y] = cellLightblue;
                                    grid[x,y].SetAlive(true, "lightblue");

                                    totalLightblueCells += 1;

                                //50% kans dat een cel donkerblauw wordt
                                } else{

                                    Cell cellDarkblue = Instantiate(Resources.Load("Prefabs/cellDarkblue", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                                    grid[x,y] = cellDarkblue;
                                    grid[x,y].SetAlive(true, "darkblue");

                                    totalDarkblueCells += 1;

                                }

                            //20% kans dat een cel rood wordt
                            } else {

                                Cell cellRed = Instantiate(Resources.Load("Prefabs/cellRed", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                                grid[x,y] = cellRed;
                                grid[x,y].SetAlive(true, "red");

                                totalRedCells += 1;

                            }

                        //als een cel dood is
                        } else {

                            Cell cell = Instantiate(Resources.Load("Prefabs/cell", typeof(Cell)), new Vector2 (x,y), Quaternion.identity) as Cell;

                            grid[x,y] = cell;
                            grid[x,y].SetAlive(false, "dead");

                        }

                    }


                }

            }

        }


        totalBlueCells = totalLightblueCells + totalDarkblueCells;
        totalCells = totalGreenCells + totalBlueCells + totalRedCells;

        greenCellPercent = (totalGreenCells*100)/totalCells;
        lightblueCellPercent = (totalLightblueCells*100)/totalCells;
        darkblueCellPercent = (totalDarkblueCells*100)/totalCells;
        blueCellPercent = (totalBlueCells*100)/totalCells;
        redCellPercent = (totalRedCells*100)/totalCells;
        aliveCellPercent = (100*totalCells)/(1024*768/(16*16));

        Debug.Log("Green cells: " + totalGreenCells + " (" + greenCellPercent + "%)  Blue cells: " + totalBlueCells + " (" + blueCellPercent + "%)  Lightblue cells: " + totalLightblueCells + " (" + lightblueCellPercent + "%)  Darkblue cells: " + totalDarkblueCells + " (" + darkblueCellPercent + "%)  Red cells: " + totalRedCells + " (" + redCellPercent + "%)  Total cells: " + totalCells + " (" + aliveCellPercent + "%) Generation: " + generationCounter);

    }

    void CountNeighbors(){

        for (int y = 0; y < SCREEN_HEIGHT; y++){

            for(int x = 0; x < SCREEN_WIDTH; x++){

                int numGreenNeighbors = 0;
                int numLightblueNeighbors = 0;
                int numDarkblueNeighbors = 0;
                int numRedNeighbors = 0;
                int numWaterNeighbors = 0;

                //North
                if(y+1 < SCREEN_HEIGHT){

                    if(grid[x,y+1].isAlive && grid[x,y+1].deKleur == "green"){

                        numGreenNeighbors ++;

                    }else if(grid[x,y+1].isAlive && grid[x,y+1].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    }else if(grid[x,y+1].isAlive && grid[x,y+1].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    }else if(grid[x,y+1].isAlive && grid[x,y+1].deKleur == "red"){

                        numRedNeighbors ++;

                    }else if(grid[x,y+1].isAlive && grid[x,y+1].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }
                
                //East
                if(x+1 < SCREEN_WIDTH){

                    if(grid[x+1,y].isAlive && grid[x+1,y].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x+1,y].isAlive && grid[x+1,y].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    } else if(grid[x+1,y].isAlive && grid[x+1,y].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    } else if(grid[x+1,y].isAlive && grid[x+1,y].deKleur == "red"){

                        numRedNeighbors ++;

                    } else if(grid[x+1,y].isAlive && grid[x+1,y].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                //South
                if(y > 0){

                    if(grid[x,y-1].isAlive && grid[x,y-1].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x,y-1].isAlive && grid[x,y-1].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    }  else if(grid[x,y-1].isAlive && grid[x,y-1].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    }  else if(grid[x,y-1].isAlive && grid[x,y-1].deKleur == "red"){

                        numRedNeighbors ++;

                    }  else if(grid[x,y-1].isAlive && grid[x,y-1].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                //West
                if(x > 0){

                    if(grid[x-1,y].isAlive && grid[x-1,y].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x-1,y].isAlive && grid[x-1,y].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    } else if(grid[x-1,y].isAlive && grid[x-1,y].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    } else if(grid[x-1,y].isAlive && grid[x-1,y].deKleur == "red"){

                        numRedNeighbors ++;

                    } else if(grid[x-1,y].isAlive && grid[x-1,y].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                //North-East
                if(x+1 < SCREEN_WIDTH && y+1 < SCREEN_HEIGHT){

                    if(grid[x+1,y+1].isAlive && grid[x+1,y+1].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x+1,y+1].isAlive && grid[x+1,y+1].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    } else if(grid[x+1,y+1].isAlive && grid[x+1,y+1].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    } else if(grid[x+1,y+1].isAlive && grid[x+1,y+1].deKleur == "red"){

                        numRedNeighbors ++;

                    } else if(grid[x+1,y+1].isAlive && grid[x+1,y+1].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                //North-West
                if(x > 0 && y+1 < SCREEN_HEIGHT){

                    if(grid[x-1,y+1].isAlive && grid[x-1,y+1].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x-1,y+1].isAlive && grid[x-1,y+1].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    } else if(grid[x-1,y+1].isAlive && grid[x-1,y+1].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    } else if(grid[x-1,y+1].isAlive && grid[x-1,y+1].deKleur == "red"){

                        numRedNeighbors ++;

                    } else if(grid[x-1,y+1].isAlive && grid[x-1,y+1].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                //South-East
                if(x+1 < SCREEN_WIDTH && y > 0){

                    if(grid[x+1,y-1].isAlive && grid[x+1,y-1].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x+1,y-1].isAlive && grid[x+1,y-1].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    } else if(grid[x+1,y-1].isAlive && grid[x+1,y-1].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    } else if(grid[x+1,y-1].isAlive && grid[x+1,y-1].deKleur == "red"){

                        numRedNeighbors ++;

                    } else if(grid[x+1,y-1].isAlive && grid[x+1,y-1].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                //South-West
                if(x > 0 && y > 0){

                    if(grid[x-1,y-1].isAlive && grid[x-1,y-1].deKleur == "green"){

                        numGreenNeighbors ++;

                    } else if(grid[x-1,y-1].isAlive && grid[x-1,y-1].deKleur == "lightblue"){

                        numLightblueNeighbors ++;

                    } else if(grid[x-1,y-1].isAlive && grid[x-1,y-1].deKleur == "darkblue"){

                        numDarkblueNeighbors ++;

                    } else if(grid[x-1,y-1].isAlive && grid[x-1,y-1].deKleur == "red"){

                        numRedNeighbors ++;

                    } else if(grid[x-1,y-1].isAlive && grid[x-1,y-1].deKleur == "water"){

                        numWaterNeighbors ++;

                    }

                }

                int numBlueNeighbors = numLightblueNeighbors + numDarkblueNeighbors;
                int numNeighbors = numGreenNeighbors + numBlueNeighbors + numRedNeighbors;

                grid[x,y].numNeighbors = numNeighbors;
                grid[x,y].numGreenNeighbors = numGreenNeighbors;
                grid[x,y].numBlueNeighbors = numBlueNeighbors;
                grid[x,y].numLightblueNeighbors = numLightblueNeighbors;
                grid[x,y].numDarkblueNeighbors = numDarkblueNeighbors;
                grid[x,y].numRedNeighbors = numRedNeighbors;
                grid[x,y].numWaterNeighbors = numWaterNeighbors;

            }

        }

    }

    void PopulationControl (){

        int totalGreenCells = 0;
        int totalLightblueCells = 0;
        int totalDarkblueCells = 0;
        int totalBlueCells = 0;
        int totalRedCells = 0;
        int totalCells = 0;

        int greenCellPercent = 0;
        int lightblueCellPercent = 0;
        int darkblueCellPercent = 0;
        int blueCellPercent = 0;
        int redCellPercent = 0;
        int aliveCellPercent = 0;

        for (int y = 0; y < SCREEN_HEIGHT; y++){

            for(int x = 0; x < SCREEN_WIDTH; x++){

                if(grid[x,y].cellSprite != "WaterCell"){

                    //Als een cel levend en groen is
                    if(grid[x,y].isAlive && grid[x,y].cellSprite == "GreenCell"){

                        //Als er water in de buurt is
                        if(grid[x,y].numWaterNeighbors > 0){

                            //als er blauwe buren zijn
                            if(grid[x,y].numBlueNeighbors > 0){

                                //maak een variabele waar je de kans om te overleven opslaat als getal tussen 0 en 100
                                double survivalChance;

                                if (grid[x,y].numBlueNeighbors >= grid[x,y].numGreenNeighbors){

                                    survivalChance = (0.65 / ((double)grid[x,y].numBlueNeighbors/(double)grid[x,y].numGreenNeighbors)) * 100;

                                }else {

                                    survivalChance = (1 - (0.45/((double)grid[x,y].numGreenNeighbors/(double)grid[x,y].numBlueNeighbors))) * 100;

                                }

                                int randomNumberChance = UnityEngine.Random.Range (0, 101);

                                //als het overlevingsgetal kleiner is dan een random nummer tussen 0 en 100
                                if(survivalChance < randomNumberChance){

                                    //als het aantal blauwe buren gelijk of groter is aan 2
                                    if(grid[x,y].numBlueNeighbors >= 2){

                                        //Als er meer of evenveel donkerblauwe cellen zijn als lichtblauwe cellen
                                        if(grid[x,y].numDarkblueNeighbors - grid[x,y].numLightblueNeighbors >= 0){

                                            //Als het aantal prooidieren/aantal hoofdorganismen minimaal 1,2 is
                                            if(grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors >= 1.2){

                                                //Maak de cel levend en donkerblauw
                                                grid[x,y].SetAlive(true, "darkblue");

                                                //dit is om het aantal donkerblauwe cellen bij te houden
                                                totalDarkblueCells ++;

                                            }

                                        } else {

                                            //Als het aantal prooidieren/aantal hoofdorganismen minimaal 0,7 is
                                            if((grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors) >= 0.7){

                                                //Maak de cel levend en lichtblauw
                                                grid[x,y].SetAlive(true, "lightblue");

                                                //dit is om het aantal lichtblauwe cellen bij te houden
                                                totalLightblueCells ++;

                                            }

                                        }

                                    } else {

                                        //De cel sterft af
                                        grid[x,y].SetAlive(false, "dead");

                                    }

                                } else {

                                    //dit is om het aantal groene cellen bij te houden
                                    totalGreenCells ++;

                                }

                            } else {

                                //dit is om het aantal groene cellen bij te houden
                                totalGreenCells ++;

                            }

                        } else {

                            //Als er geen groene buren zijn of het aantal groene buren hoger is dan 5
                            if(grid[x,y].numGreenNeighbors == 0 || grid[x,y].numGreenNeighbors > 4){
                                
                                //De cel sterft af
                                grid[x,y].SetAlive(false, "dead");

                            //Als het aantal blauwe buren hoger is dan 0
                            } else if(grid[x,y].numBlueNeighbors > 0){

                                //maak een variabele waar je de kans om te overleven opslaat als getal tussen 0 en 100
                                double survivalChance;

                                if (grid[x,y].numBlueNeighbors >= grid[x,y].numGreenNeighbors){

                                    survivalChance = (0.4 / ((double)grid[x,y].numBlueNeighbors/(double)grid[x,y].numGreenNeighbors)) * 100;

                                }else {

                                    survivalChance = (1 - (0.7/((double)grid[x,y].numGreenNeighbors/(double)grid[x,y].numBlueNeighbors))) * 100;

                                }

                                int randomNumberChance = UnityEngine.Random.Range (0, 101);

                                //als het overlevingsgetal kleiner is dan een random nummer tussen 0 en 100
                                if(survivalChance < randomNumberChance){

                                    //als het aantal blauwe buren gelijk of groter is aan 2
                                    if(grid[x,y].numBlueNeighbors >= 2){

                                        //Als er meer of evenveel donkerblauwe cellen zijn als lichtblauwe cellen
                                        if(grid[x,y].numDarkblueNeighbors - grid[x,y].numLightblueNeighbors >= 0){

                                            //Als het aantal prooidieren/aantal hoofdorganismen minimaal 1,2 is
                                            if(grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors >= 1.2){

                                                //Maak de cel levend en donkerblauw
                                                grid[x,y].SetAlive(true, "darkblue");

                                                //dit is om het aantal donkerblauwe cellen bij te houden
                                                totalDarkblueCells ++;

                                            }

                                        } else {

                                            //Als het aantal prooidieren/aantal hoofdorganismen minimaal 0,7 is
                                            if((grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors) >= 0.7){

                                                //Maak de cel levend en lichtblauw
                                                grid[x,y].SetAlive(true, "lightblue");

                                                //dit is om het aantal lichtblauwe cellen bij te houden
                                                totalLightblueCells ++;

                                            }

                                        }

                                    } else {

                                        //De cel sterft af
                                        grid[x,y].SetAlive(false, "dead");

                                    }

                                } else {

                                    //dit is om het aantal groene cellen bij te houden
                                    totalGreenCells ++;

                                }

                            } else {

                                //dit is om het aantal groene cellen bij te houden
                                totalGreenCells ++;

                            }

                        }

                    //Als een cel levend en lichtblauw is
                    }else if (grid[x,y].isAlive && grid[x,y].cellSprite == "LightblueCell"){

                        //Als er geen groene cellen zijn
                        if(grid[x,y].numGreenNeighbors == 0){

                            //De cel sterft af
                            grid[x,y].SetAlive(false, "dead");

                        //Als er wel blauwe buren zijn en de prooidieren(buren)/hoofdorganismen(buren) kleiner is dan 0,25
                        } else if(grid[x,y].numBlueNeighbors != 0 && grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors < 0.25){

                            //De kans is 40% dat de cel overleefd zonder voedsel
                            double survivalChance = 40;
                            int randomNumberChance = UnityEngine.Random.Range (0, 101);

                            if(survivalChance !> randomNumberChance){

                                //De cel sterft af
                                grid[x,y].SetAlive(false, "dead");

                            } else {

                                //Als er rode buren zijn
                                if(grid[x,y].numRedNeighbors > 0) {

                                    //Sla de kans op dat de cel overleeft
                                    double survivalChance1;

                                    if (grid[x,y].numRedNeighbors >= grid[x,y].numBlueNeighbors){

                                        survivalChance1 = 0;

                                    }else {

                                        survivalChance1 = (1 - (1/((double)grid[x,y].numBlueNeighbors/(double)grid[x,y].numRedNeighbors))) * 100;

                                    }

                                    int randomNumberChance1 = UnityEngine.Random.Range (0, 101);

                                    //als het overlevingsgetal kleiner is dan een random nummer tussen 0 en 100
                                    if(survivalChance1 !> randomNumberChance1){

                                        //De cel sterft af
                                        grid[x,y].SetAlive(false, "dead");

                                    } else {

                                        //dit is om het aantal lichtblauwe cellen bij te houden
                                        totalLightblueCells ++;

                                    }

                                }

                            }

                        //als de hoeveelheid rode cellen groter is dan 0
                        }else if(grid[x,y].numRedNeighbors > 0) {

                            //Sla de kans op dat de cel overleeft
                            double survivalChance1;

                            if (grid[x,y].numRedNeighbors >= grid[x,y].numBlueNeighbors){

                                survivalChance1 = 0;

                            }else {

                                survivalChance1 = (1 - (1/((double)grid[x,y].numBlueNeighbors/(double)grid[x,y].numRedNeighbors))) * 100;

                            }

                            int randomNumberChance1 = UnityEngine.Random.Range (0, 101);

                            //als het overlevingsgetal kleiner is dan een random nummer tussen 0 en 100
                            if(survivalChance1 !> randomNumberChance1){

                                //De cel sterft af
                                grid[x,y].SetAlive(false, "dead");

                            } else {

                                //dit is om het aantal lichtblauwe cellen bij te houden
                                totalLightblueCells ++;

                            }

                        }else {

                            //dit is om het aantal lichtblauwe cellen bij te houden
                            totalLightblueCells ++;

                        }

                    //Als een cel levend en donkerblauw is
                    }else if (grid[x,y].isAlive && grid[x,y].cellSprite == "DarkblueCell"){

                        //Als er geen groene buren zijn
                        if(grid[x,y].numGreenNeighbors == 0){

                            //De cel sterft af
                            grid[x,y].SetAlive(false, "dead");

                        } else if(grid[x,y].numBlueNeighbors != 0 && grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors < 0.5){

                            //De kans dat een blauwe cel zonder eten overleefd is 40%
                            double survivalChance = 40;
                            int randomNumberChance = UnityEngine.Random.Range (0, 101);

                            if(survivalChance !> randomNumberChance){

                                //De cel sterft af
                                grid[x,y].SetAlive(false, "dead");

                            } else {

                                //Als er rode cellen in de buurt zijn
                                if(grid[x,y].numRedNeighbors > 0) {

                                    //Sla de kans op dat de cel overleeft
                                    double survivalChance1;

                                    if (grid[x,y].numRedNeighbors >= grid[x,y].numBlueNeighbors){

                                        survivalChance1 = (0.2/((double)grid[x,y].numRedNeighbors/(double)grid[x,y].numBlueNeighbors)) * 100;

                                    }else {

                                        survivalChance1 = (1 - (0.8/((double)grid[x,y].numBlueNeighbors/(double)grid[x,y].numRedNeighbors))) * 100;

                                    }

                                    int randomNumberChance1 = UnityEngine.Random.Range (0, 101);

                                    //als het overlevingsgetal kleiner is dan een random nummer tussen 0 en 100
                                    if(survivalChance1 !> randomNumberChance1){

                                        //De cel sterft af
                                        grid[x,y].SetAlive(false, "dead");

                                    } else {

                                        //dit is om het aantal donkerbauwe cellen bij te houden
                                        totalDarkblueCells ++;

                                    }

                                }

                            }

                        //als er rode cellen in de buurt zijn
                        }else if(grid[x,y].numRedNeighbors > 0) {

                            //Sla de kans op dat de cel overleeft
                            double survivalChance1;

                            if (grid[x,y].numRedNeighbors >= grid[x,y].numBlueNeighbors){

                                survivalChance1 = (0.2/((double)grid[x,y].numRedNeighbors/(double)grid[x,y].numBlueNeighbors)) * 100;

                            }else {

                                survivalChance1 = (1 - (0.8/((double)grid[x,y].numBlueNeighbors/(double)grid[x,y].numRedNeighbors))) * 100;

                            }

                            int randomNumberChance1 = UnityEngine.Random.Range (0, 101);

                            //als het overlevingsgetal kleiner is dan een random nummer tussen 0 en 100
                            if(survivalChance1 !> randomNumberChance1){

                                //De cel sterft af
                                grid[x,y].SetAlive(false, "dead");

                            } else {

                                //dit is om het aantal donkerblauwe cellen bij te houden
                                totalDarkblueCells ++;

                            }

                        }else {

                            //dit is om het aantal donkerlauwe cellen bij te houden
                            totalDarkblueCells ++;

                        }

                    //Als een cel levend en rood is
                    }else if (grid[x,y].isAlive && grid[x,y].cellSprite == "RedCell"){

                        //Als het aantal blauwe cellen 0 is
                        if(grid[x,y].numBlueNeighbors == 0){

                            //De cel sterft af
                            grid[x,y].SetAlive(false, "dead");

                        //Als er rode buren zijn en hoofdorganismen(buren)/roofdieren(buren) kleiner dan 0,4 is
                        } else if(grid[x,y].numRedNeighbors != 0 && (grid[x,y].numBlueNeighbors/grid[x,y].numRedNeighbors < 0.4)){

                            //De kans dat een rode cel zonder eten overleefd is 40%
                            double survivalChance = 40;
                            int randomNumberChance = UnityEngine.Random.Range (0, 101);

                            if(survivalChance !> randomNumberChance){

                                //De cel sterft af
                                grid[x,y].SetAlive(false, "dead");

                            } else {

                                //dit is om het aantal rode cellen bij te houden
                                totalRedCells ++;

                            }

                        } else{

                            //dit is om het aantal rode cellen bij te houden
                            totalRedCells ++;

                        }

                    //Als een cel dood is
                    }else {

                        //Als er 2 of meer rode cellen omheen zitten en het aantal hoofdorganismen(buren)/roofdieren(buren) groter of gelijk is aan 1
                        if((grid[x,y].numRedNeighbors >= 2) && grid[x,y].numBlueNeighbors/grid[x,y].numRedNeighbors >= 1){

                            //Maak de cel levend en rood
                            grid[x,y].SetAlive(true, "red");

                            //dit is om het aantal rode cellen bij te houden
                            totalRedCells ++;

                        //Als het aantal blauwe cellen eromheen 2 of meer is
                        }else if(grid[x,y].numBlueNeighbors >= 2){

                            //Als er meer of evenveel donkerblauwe cellen zijn als lichtblauwe cellen
                            if(grid[x,y].numDarkblueNeighbors - grid[x,y].numLightblueNeighbors >= 0){

                                //Als het aantal prooidieren(buren)/hoofdorganismen(buren) minimaal 1,2 is
                                if(grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors >= 1.2){

                                    //Maak de cel levend en donkerblauw
                                    grid[x,y].SetAlive(true, "darkblue");

                                    //dit is om het aantal donkerblauwe cellen bij te houden
                                    totalDarkblueCells ++;

                                }

                            } else {

                                //Als het aantal prooidieren(buren)/hoofdorganismen(buren) minimaal 0,75 is
                                if((grid[x,y].numGreenNeighbors/grid[x,y].numBlueNeighbors) >= 0.75){

                                    //Maak de cel levend en lichtblauw
                                    grid[x,y].SetAlive(true, "lightblue");

                                    //dit is om het aantal lichtblauwe cellen bij te houden
                                    totalLightblueCells ++;

                                }

                            }

                        //Als er 2 of meer groene cellen omheen zitten
                        }else if(grid[x,y].numGreenNeighbors >= 2){

                            if(grid[x,y].numWaterNeighbors > 1){

                                //Maak de cel levend en groen
                                grid[x,y].SetAlive(true, "green");

                                //dit is om het aantal groene cellen bij te houden
                                totalGreenCells ++;

                            }else {

                                //De kans dat een groene cel kan voortplanten is 50%
                                double survivalChance = 50;
                                int randomNumberChance = UnityEngine.Random.Range (0, 101);

                                if(survivalChance !> randomNumberChance){

                                    //Maak de cel levend en groen
                                    grid[x,y].SetAlive(true, "green");

                                    //dit is om het aantal groene cellen bij te houden
                                    totalGreenCells ++;

                                }

                            }

                        }

                    }

                }

            }

        }

        totalBlueCells = totalLightblueCells + totalDarkblueCells;
        totalCells = totalGreenCells + totalBlueCells + totalRedCells;

        greenCellPercent = (totalGreenCells*100)/totalCells;
        lightblueCellPercent = (totalLightblueCells*100)/totalCells;
        darkblueCellPercent = (totalDarkblueCells*100)/totalCells;
        blueCellPercent = (totalBlueCells*100)/totalCells;
        redCellPercent = (totalRedCells*100)/totalCells;
        aliveCellPercent = (100*totalCells)/(1024*768/(16*16));

        Debug.Log("Green cells: " + totalGreenCells + " (" + greenCellPercent + "%)  Blue cells: " + totalBlueCells + " (" + blueCellPercent + "%)  Lightblue cells: " + totalLightblueCells + " (" + lightblueCellPercent + "%)  Darkblue cells: " + totalDarkblueCells + " (" + darkblueCellPercent + "%)  Red cells: " + totalRedCells + " (" + redCellPercent + "%)  Total cells: " + totalCells + " (" + aliveCellPercent + "%) Generation: " + generationCounter);

    }

}