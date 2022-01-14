using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOverlay : MonoBehaviour
{

    //maak materiaal (voor lijnen)
    private Material lineMaterial;

    //welke lijnen laat je ziet (main zijn de grote sub zijn de tussenlijnen)
    public bool showMain = true;
    public bool showSub = false;

    //wat wordt de grote van het rooster
    public int gridSizeX;
    public int gridSizeY;

    //waar beginnen de lijnen
    public float startX;
    public float startY;
    public float startZ;

    //afstand tussen de lijnen (smallStep voor showSub en largeStep voor showMain)
    public float smallStep;
    public float largeStep;

    //kleur van de lijnen
    public Color mainColor = new Color(0f, 1f, 0f, 1f);
    public Color subColor = new Color(0f, .5f, 0f, 1f);


    //maak het lijnmateriaal
    void CreateLineMaterial(){

        if (!lineMaterial){

            var shader  = Shader.Find("Hidden/Internal-Colored");

            lineMaterial = new Material(shader);

            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            //turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            //turn off depth writing
            lineMaterial.SetInt("_ZWrite", 0);

            //turn off backface culling
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);

        }

    }

    //dit moest omdat je cullmode uitzet
    private void OnDisable(){

        DestroyImmediate(lineMaterial);

    }

    //hier zet je de lijnen neer
    private void OnPostRender(){

        //maak de lijnen
        CreateLineMaterial();

        lineMaterial.SetPass(0);

        //maak eindelijk de lijnen
        GL.Begin(GL.LINES);

            //teken sublijnen
            if (showSub){

                GL.Color(subColor);

                //teken horizontale lijnen
                for (float y = 0; y <= gridSizeY; y += smallStep){

                    //begin- en eindpunt van de lijnen
                    GL.Vertex3(startX, startY + y, startZ);
                    GL.Vertex3(startX + gridSizeX, startY + y, startZ);

                }

                for (float x = 0; x <= gridSizeX; x += smallStep){

                    //begin- en eindpunt van de lijnen
                    GL.Vertex3(startX + x, startY, startZ);
                    GL.Vertex3(startX + x, startY + gridSizeY, startZ);

                }

            }

            //teken grote lijnen
            if(showMain){

                GL.Color(mainColor);

                //teken horizontale lijnen
                for (float y = 0; y <= gridSizeY; y += largeStep){

                    //begin- en eindpunt van de lijnen
                    GL.Vertex3(startX, startY + y, startZ);
                    GL.Vertex3(startX + gridSizeX, startY + y, startZ);

                }

                //teken verticale lijnen
                for (float x = 0; x <= gridSizeX; x += largeStep){

                    //begin- en eindpunt van de lijnen
                    GL.Vertex3(startX + x, startY, startZ);
                    GL.Vertex3(startX + x, startY + gridSizeY, startZ);

                }

            }

        GL.End();

    }

}
