using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

public class Analyze : MonoBehaviour {

    public enum Modes {Person, Package, RadialChoices, MergeChoices, ChoicePath, TimesToDo, AverageTimes, ReactionTime, BorderLines, None};
    public Modes mode;

    public string playerName;
    private string[] movementsText;
    char tab;

    public Vector2 min, max;
    public Vector2 center, bases;
    // Use this for initialization
	
    private List<string> playerNames;
    private List<Choice> choices;

    private float[] movementTime;
    private Vector2[] movementPos;
    private Vector2[] movementVel;

    void Start () 
    {
        Application.runInBackground = true;

        tab = Convert.ToChar("\t");

        center = (max + min) / 2f;
        bases = (max - min) / 2f;
        switch (mode)
        {
            #region Person
            case Modes.Person:

                Vector2[] movements1;
                Vector2[] borderLine1;

                borderLine1 = new Vector2[36];

                File.WriteAllText(Application.dataPath + "/" + playerName + " - BorderLine.txt", 
                    "Border Registred" + "\t" +
                    Environment.NewLine +
                    "X min" + "\t" +
                    "Y min" + "\t" +
                    "X max" + "\t" +
                    "Y max" + "\t" +
                    Environment.NewLine +
                    min.x + "\t" +
                    min.y + "\t" +
                    max.x + "\t" +
                    max.y + "\t" +
                    Environment.NewLine +
                    "X center" + "\t" +
                    "Y center" + "\t" +
                    "X bases" + "\t" +
                    "Y bases" + "\t" +
                    Environment.NewLine +
                    center.x + "\t" +
                    center.y + "\t" +
                    bases.x + "\t" +
                    bases.y + "\t" +
                    Environment.NewLine +
                    Environment.NewLine
                    , Encoding.UTF8);

                File.WriteAllText(Application.dataPath + "/" + playerName + " - Movements.txt", 
                    "", 
                    Encoding.UTF8);
                    
                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - Historic.txt", Encoding.UTF8);
                Debug.Log("Historic Size: " + movementsText.Length);

                for (int i = 2; i < movementsText.Length; i++)
                {
                    string[] testes = movementsText[i].Split(tab);
                    if (File.Exists(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Movements.txt"))
                    {
                        string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Movements.txt", Encoding.UTF8);
                        for (int j = 2; j < choiceLines.Length; j++)
                        {
                            File.AppendAllText(Application.dataPath + "/" + playerName + " - Movements.txt", 
                                choiceLines[j] + 
                                Environment.NewLine, Encoding.UTF8);
                        }
                    }
                    else
                    {
                        Debug.Log(playerName + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                    }
                }

                movementsText = File.ReadAllLines(Application.dataPath + "/" + playerName + " - Movements.txt", Encoding.UTF8);
                movements1 = new Vector2[movementsText.Length - 1];
                min = max = center = bases = Vector2.zero;
                Vector2 polar, offset;

                for (int i = 1; i < movementsText.Length; i++)
                {
                    try {
                        string[] columns = movementsText[i].Split(tab);
                        movements1[i - 1] = (new Vector2(float.Parse(columns[1]), float.Parse(columns[2]))) * Mathf.Rad2Deg;
                        if (movements1[i - 1].x < min.x)
                            min.x = movements1[i - 1].x;
                        if (movements1[i - 1].y < min.y)
                            min.y = movements1[i - 1].y;
                        if (movements1[i - 1].x > max.x)
                            max.x = movements1[i - 1].x;
                        if (movements1[i - 1].y > max.y)
                            max.y = movements1[i - 1].y;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
                movementsText = new string[0];

                center = (max + min) / 2f;
                bases = (max - min) / 2f;
                for (int i = 0; i < borderLine1.Length; i++)
                    borderLine1[i] = center;
                
                File.AppendAllText(Application.dataPath + "/" + playerName + " - BorderLine.txt", 
                    "Border Calculated" + "\t" +
                    Environment.NewLine +
                    "X min" + "\t" +
                    "Y min" + "\t" +
                    "X max" + "\t" +
                    "Y max" + "\t" +
                    Environment.NewLine +
                    min.x + "\t" +
                    min.y + "\t" +
                    max.x + "\t" +
                    max.y + "\t" +
                    Environment.NewLine +
                    "X center" + "\t" +
                    "Y center" + "\t" +
                    "X bases" + "\t" +
                    "Y bases" + "\t" +
                    Environment.NewLine +
                    center.x + "\t" +
                    center.y + "\t" +
                    bases.x + "\t" +
                    bases.y + "\t" +
                    Environment.NewLine +
                    Environment.NewLine
                    , Encoding.UTF8);

                for (int i = 0; i < movements1.Length; i++)
                {
                    offset = movements1[i] - center;

                    float ang = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
                    while (ang < 0f)
                        ang += 360f;
                    int i_ang = Mathf.RoundToInt(ang / 10f);
                    if (i_ang >= 36)
                        i_ang -= 36;
                    if (i_ang < 0)
                        i_ang += 36;

                    polar = new Vector2(offset.magnitude, ang);
                    if (polar.x > borderLine1[i_ang].x)
                        borderLine1[i_ang] = polar;
                }
                movements1 = new Vector2[0];

                for (int i = 0; i < borderLine1.Length; i++)
                File.AppendAllText(Application.dataPath + "/" + playerName + " - BorderLine.txt", 
                        i + "\t" +
                        borderLine1[i].x + "\t" +
                        borderLine1[i].y + "\t" +
                        (borderLine1[i].x * Mathf.Cos(borderLine1[i].y * Mathf.Deg2Rad) + center.x) + "\t" +
                        (borderLine1[i].x * Mathf.Sin(borderLine1[i].y * Mathf.Deg2Rad) + center.y) + "\t" +
                        (bases.x * Mathf.Cos(10f * i * Mathf.Deg2Rad) + center.x) + "\t" +
                        (bases.y * Mathf.Sin(10f * i * Mathf.Deg2Rad) + center.y) + "\t" +
                        Environment.NewLine
                    , Encoding.UTF8);
                break;
                #endregion
            #region Package
            case Modes.Package:
                movementsText = File.ReadAllLines(Application.dataPath + "/Packages.txt", Encoding.UTF8);

                string[] columns2; // = movementsText[0].Split(tab);

                /// [Movimentos][Players x 2]
                Vector2[][] movements2 = new Vector2[movementsText.Length][];
                Vector2 polar2, offset2;
                int players = 0;

                for (int i = 0; i < movements2.Length; i++)
                {
                    columns2 = movementsText[i].Split(tab);

                    players = Mathf.RoundToInt((columns2.Length - 1) / 4f);

                    movements2[i] = new Vector2[players * 2];

                    for (int j = 0; j < movements2[i].Length; j += 2)
                    {
                        try
                        {
                            movements2[i][j] = (new Vector2(float.Parse(columns2[j * 2 + 1]), float.Parse(columns2[j * 2 + 2]))); // * Mathf.Rad2Deg;
                            movements2[i][j + 1] = (new Vector2(float.Parse(columns2[j * 2 + 3]), float.Parse(columns2[j * 2 + 4]))); // * Mathf.Rad2Deg;
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                }
                movementsText = new string[0];

                File.WriteAllText(Application.dataPath + "/" + playerName + " - PackagesAdjust.txt", 
                    "", Encoding.UTF8);
                
                Vector2[][] borderLine2 = new Vector2[36][];
                for (int i = 0; i < 36; i++)
                    borderLine2[i] = new Vector2[players];

                for (int i = 0; i < movements2.Length; i++)
                {
                    for (int j = 0; j < players; j++)
                    {
                        offset2 = movements2[i][j * 2];

                        float ang = Mathf.Atan2(offset2.y, offset2.x) * Mathf.Rad2Deg;
                        while (ang < 0f)
                            ang += 360f;
                        int i_ang = Mathf.CeilToInt(ang / 10f);
                        if (i_ang >= 36)
                            i_ang -= 36;
                        if (i_ang < 0)
                            i_ang += 36;

                        polar2 = new Vector2(offset2.magnitude, ang);
                        if (polar2.x > borderLine2[i_ang][j].x)
                            borderLine2[i_ang][j] = polar2;
                    }
                }
//                movements = new Vector2[0];

                for (int i = 0; i < borderLine2.Length; i++)
                {
                    string line = i.ToString();
                    for (int j = 0; j < borderLine2[i].Length; j++)
                    {
                        line = string.Concat(line + "\t", borderLine2[i][j].x * Mathf.Cos(borderLine2[i][j].y * Mathf.Deg2Rad));
                        line = string.Concat(line + "\t", borderLine2[i][j].x * Mathf.Sin(borderLine2[i][j].y * Mathf.Deg2Rad));
                        line = string.Concat(line + "\t", movements2[i * 10][j*2 + 1].x);
                        line = string.Concat(line + "\t", movements2[i * 10][j*2 + 1].y);
                    }
                    File.AppendAllText(Application.dataPath + "/PackagesAdjust.txt", 
                        line +
                        Environment.NewLine
                        , Encoding.UTF8);
                    
                }
                break;
                #endregion
            #region Choices
            case Modes.RadialChoices:
                playerNames = new List<string>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNames.Add(line);
                }
                foreach (string player in playerNames)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);
                    choices.Add(new Choice(player));

                    for (int i = 2; i < movementsText.Length; i++)
                    {
                        string[] testes = movementsText[i].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {
                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);
                            for (int j = 2; j < choiceLines.Length; j++)
                            {
                                string[]choice = choiceLines[j].Split(tab);
                                choices[choices.Count - 1].Add(choice);
                            }
                        }
                        else
                        {
                            Debug.Log(player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                        }
                    }
                }

                foreach (Choice player in choices)
                {
                    Vector2[] movements3;
                    Vector2[] borderLine3;

                    movementsText = File.ReadAllLines(Application.dataPath + "/Packages/" + player.playerName + " - Movements.txt", Encoding.UTF8);
                    movements3 = new Vector2[movementsText.Length - 1];
                    borderLine3 = new Vector2[36];

                    min = max = center = bases = Vector2.zero;
                    Vector2 polar3, offset3;

                    for (int i = 1; i < movementsText.Length; i++)
                    {
                        try {
                            string[] columns = movementsText[i].Split(tab);
                            movements3[i - 1] = (new Vector2(float.Parse(columns[1]), float.Parse(columns[2]))) * Mathf.Rad2Deg;
                            if (movements3[i - 1].x < min.x)
                                min.x = movements3[i - 1].x;
                            if (movements3[i - 1].y < min.y)
                                min.y = movements3[i - 1].y;
                            if (movements3[i - 1].x > max.x)
                                max.x = movements3[i - 1].x;
                            if (movements3[i - 1].y > max.y)
                                max.y = movements3[i - 1].y;
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }

                    movementsText = new string[0];

                    center = (max + min) / 2f;
                    bases = (max - min) / 2f;
                    for (int i = 0; i < borderLine3.Length; i++)
                        borderLine3[i] = center;

                    for (int i = 0; i < movements3.Length; i++)
                    {
                        offset = movements3[i] - center;

                        float ang = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
                        while (ang < 0f)
                            ang += 360f;
                        int i_ang = Mathf.RoundToInt(ang / 10f);
                        if (i_ang >= 36)
                            i_ang -= 36;
                        if (i_ang < 0)
                            i_ang += 36;

                        polar = new Vector2(offset.magnitude, ang);
                        if (polar.x > borderLine3[i_ang].x)
                            borderLine3[i_ang] = polar;
                    }
                    movements3 = new Vector2[0];

                    for (int i = 0; i < borderLine3.Length; i++)
                    {
                        player.package[i] = borderLine3[i].x;
                        player.elipse[i] = (new Vector2(bases.x * Mathf.Cos(10f* i * Mathf.Deg2Rad), bases.y * Mathf.Sin(10f * i * Mathf.Deg2Rad))).magnitude;
                    }
                    player.RadialProfile();
                }

                string lineText = "\t";

                for (int i = 0; i < choices.Count; i++)
                {
                    choices[i].RadialProfile();
                    lineText = lineText + choices[i].playerName + "\t\t\t";
                }

                File.WriteAllText(Application.dataPath + "/Choices/RadialAverageTime.txt", 
                    lineText
                    + Environment.NewLine, Encoding.UTF8);

                File.WriteAllText(Application.dataPath + "/Choices/RadialMistakes.txt", 
                    lineText
                    + Environment.NewLine, Encoding.UTF8);

                for (int i = 0; i < 36; i++)
                {
                    lineText = (10f*i).ToString() + "\t";
                    for (int j = 0; j < choices.Count; j++)
                    {
                        lineText = lineText + choices[j].package[i] + "\t";
                        lineText = lineText + choices[j].elipse[i] + "\t";
                        lineText = lineText + (choices[j].package[i] / choices[j].elipse[i]) + "\t";
                        lineText = lineText + choices[j].averageTime[i] + "\t";
                    }
                    File.AppendAllText(Application.dataPath + "/Choices/RadialAverageTime.txt", 
                        lineText +
                        Environment.NewLine
                        , Encoding.UTF8);

                    lineText = (10f*i).ToString() + "\t";
                    for (int j = 0; j < choices.Count; j++)
                    {
                        lineText = lineText + choices[j].package[i] + "\t";
                        lineText = lineText + choices[j].elipse[i] + "\t";
                        lineText = lineText + (choices[j].package[i] / choices[j].elipse[i]) + "\t";
                        lineText = lineText + choices[j].nMistake[i] + "\t";
                    }
                    File.AppendAllText(Application.dataPath + "/Choices/RadialMistakes.txt", 
                        lineText +
                        Environment.NewLine
                        , Encoding.UTF8);
                }
                break;
            #endregion
            #region MergeChoices
            case Modes.MergeChoices:
                playerNames = new List<string>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNames.Add(line);
                }
                foreach (string player in playerNames)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);

                    for (int i = 2; i < movementsText.Length; i++)
                    {
                        string[] testes = movementsText[i].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {
                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);
                            for (int j = 2; j < choiceLines.Length; j++)
                            {
                                //string[]choice = choiceLines[j].Split(tab);
                                if (File.Exists(Application.dataPath + "/Choices/AllChoices.txt"))
                                {
                                    File.AppendAllText(Application.dataPath + "/Choices/AllChoices.txt", 
                                        player + "\t" + choiceLines[j] + Environment.NewLine
                                        , Encoding.UTF8);
                                }
                                else
                                {
                                    File.WriteAllText(Application.dataPath + "/Choices/AllChoices.txt", 
                                        "\t" + choiceLines[0] + Environment.NewLine + 
                                        "Name \t" + choiceLines[1] + Environment.NewLine +
                                        player + "\t" + choiceLines[j] + Environment.NewLine
                                        , Encoding.UTF8);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log(player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                        }
                    }
                }
                break;
            #endregion
            #region ChoicePath
            case Modes.ChoicePath:
                playerNames = new List<string>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNames.Add(line);
                }
                foreach (string player in playerNames)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);
                    choices.Add(new Choice(player));

                    for (int iMode = 2; iMode < movementsText.Length; iMode++)
                    {
                        int index0 = 0;
                        int index1 = 0;

                        string[] testes = movementsText[iMode].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {

                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Movements.txt", Encoding.UTF8);

//                            Debug.Log(player + " has " + choiceLines.Length + " movements");

                            movementTime = new float[choiceLines.Length - 1];
                            movementPos = new Vector2[choiceLines.Length - 1];
                            movementVel = new Vector2[choiceLines.Length - 1];

                            min = max = Vector2.zero;

                            for (int j = 1; j < choiceLines.Length; j++)
                            {
                                string[]choice = choiceLines[j].Split(tab);
                                movementTime[j - 1] = float.Parse(choice[0]);
                                Vector2 pos = new Vector2(float.Parse(choice[1]), float.Parse(choice[2]));

                                try 
                                {
                                    if (pos.x < min.x)
                                        min.x = pos.x;
                                    if (pos.y < min.y)
                                        min.y = pos.y;
                                    if (pos.x > max.x)
                                        max.x = pos.x;
                                    if (pos.y > max.y)
                                        max.y = pos.y;
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(e);
                                }

                                movementPos[j - 1] = pos; // new Vector2(pos.magnitude, Mathf.Atan2(pos.y, pos.x));

                                if (j >= 2)
                                {
                                    if (movementPos[j - 2] == Vector2.zero)
                                        if (j == 2)
                                        {
                                            movementPos[j - 2] = movementPos[j - 1];
                                        }
                                        else
                                        {
                                        movementPos[j - 2] = (movementPos[j - 1] + movementPos[j - 3])/2f;
                                        }
                                }
                            }

                            center = (max + min) / 2f;

        /*                    for (int j = 0; j < movementPos.Length; j++)
                            {
                                Vector2 pos = movementPos[j] - center;

                                movementPos[j] = new Vector2(pos.magnitude, Mathf.Atan2(pos.y, pos.x));
                                if (j == 0)
                                    movementVel[j] = Vector2.zero;
                                else
                                    movementVel[j] = (movementPos[j] - movementPos[j - 1])/(movementTime[j] - movementTime[j - 1]);
                                //    movementVel[j - 1] = new Vector2(float.Parse(choice[3]), float.Parse(choice[3]));
                            }*/

                            choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);

//                            Debug.Log(player + " has " + choiceLines.Length + " choices");

                            float startTime = 0f;
                            float endTime = 0f;

                            for (int iChoice = 2; iChoice < choiceLines.Length; iChoice++)
                            {
                                string[]choice = choiceLines[iChoice].Split(tab);
                                int lastPlayer = choices.Count - 1;
                                choices[lastPlayer].Add(choice);

                                int lastChoice = choices[lastPlayer].nChoice.Count - 1;

                                endTime = choices[lastPlayer].time[lastChoice]; //  + 2.5f / choices[lastPlayer].speed[lastChoice];
                                /*if (!choices[lastPlayer].match[lastChoice])
                                    endTime += 1.5f / choices[lastPlayer].speed[lastChoice];
                                endTime += 0.75f / choices[lastPlayer].speed[lastChoice];
                                endTime += (1.5f + (choices[lastPlayer].nCards[lastChoice] - 1) * 0.25f) / choices[lastPlayer].speed[lastChoice];
*/
                                //endTime -=  1.25f / choices[lastPlayer].speed[lastChoice];
                                startTime = endTime - (1.25f / choices[lastPlayer].speed[lastChoice] + choices[lastPlayer].timeToDo[lastChoice].z);

                                string linePath = "";
                                string lineAngle = "";
                                int index0_aux = index0;

                                while ((movementTime[index1] < endTime) && (index1 != movementTime.Length - 1))
                                {
                                    if (movementTime[index0] < startTime)
                                        index0++;
                                    index1++;

                                }
                                /*
                                for (int i = index0_aux; i < index1; i++)
                                {
                                    linePath = linePath + (movementPos[i] - movementPos[index0]).magnitude + "\t";
                                    lineAngle = lineAngle + Mathf.Atan2((movementPos[i] - movementPos[index0]).y, (movementPos[i] - movementPos[index0]).x) + "\t";
                                }

                                File.AppendAllText(Application.dataPath + "/Choices/" + player + " - TurnPath.txt", 
                                    linePath +
                                    Environment.NewLine, Encoding.UTF8);

                                File.AppendAllText(Application.dataPath + "/Choices/" + player + " - TurnAngle.txt", 
                                    lineAngle +
                                    Environment.NewLine, Encoding.UTF8);*/
                                
                                if (index0 != index1)
                                {
                                    choices[lastPlayer].Add(movementTime, movementPos, index0, index1);
                                }
                                else
                                    Debug.Log("Erro - " + index0 + " - " + choices[lastPlayer].playerName);

                                index0 = index1;
                            }
                        //    Debug.Log(player + " - endTime = " + endTime.ToString() + " | choice = " + (choices[choices.Count - 1].nChoice.Count - 1).ToString());
                        }
                        else
                        {
                            Debug.Log(player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                        }
                    }
                }

                bool running = true;
                string currentLine = "";
                string currentLine2 = "";
                int timeStepAux = 0;

                foreach (Choice player in choices)
                {

                    // Para implementar cabećalho
     /*               for (int nChoice = 0; nChoice < player.movementPos.Count; nChoice++)
                        //                        foreach (Vector2[] choicePath in player.movementPos)
                    {
                        currentLine = currentLine + (player.movementPos[nChoice][timeStepAux].x) + "\t";
                        currentLine2 = currentLine2 + (player.movementPos[nChoice][timeStepAux].y) + "\t";
                    }*/

                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePaths.txt", 
                        ""
                        , Encoding.UTF8);
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoiceAngle.txt", 
                        ""
                        , Encoding.UTF8);
                }

                while (running)
                {
                    running = false;
                    foreach (Choice player in choices)
                    {
                        currentLine = timeStepAux.ToString() + "\t";
                        currentLine2 = timeStepAux.ToString() + "\t";

                        for (int nChoice = 0; nChoice < player.movementPos.Count; nChoice++)
//                        foreach (Vector2[] choicePath in player.movementPos)
                        {
                            if (timeStepAux < player.movementPos[nChoice].Length)
                            {
                                running = true;
                                currentLine = currentLine + (player.movementPos[nChoice][timeStepAux].x * Mathf.Rad2Deg) + "\t";
                                currentLine2 = currentLine2 + (player.movementPos[nChoice][timeStepAux].y) + "\t";
                            }
                            else
                            {
                                currentLine = currentLine + "\t";
                                currentLine2 = currentLine2 + "\t";
                            }
                        }

                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePaths.txt", 
                            currentLine +
                            Environment.NewLine
                            , Encoding.UTF8);
                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoiceAngle.txt", 
                            currentLine2 +
                            Environment.NewLine
                            , Encoding.UTF8);
                    }
                    timeStepAux++;
                }

                foreach (Choice player in choices)
                {
                    foreach (Vector2[] iChoice in player.movementPos)
                    {
                        float magStart = iChoice[0].x;
                        float magEnd = iChoice[iChoice.Length - 1].x;
                        int reac = -1, mot = -1, chos = -1;

                        for (int iPos = 0; iPos < iChoice.Length; iPos++)
                        {
                            if (reac < 0)
                                if (Mathf.Abs(iChoice[iPos].x - magStart) > magEnd * 0.05f)
                                    reac = iPos - 1;
                            
                            if (Mathf.Abs(iChoice[iPos].x - magEnd) > magEnd * 0.05f)
                                mot = -1;

                            if (mot < 0)
                                if (Mathf.Abs(iChoice[iPos].x - magEnd) < magEnd * 0.05f)
                                    mot = iPos;
                        }
                        player.Add(reac, mot);
                        if ((reac < 0) || (mot < 0))
                            Debug.Log(player.playerName + " - Reaction " + reac + " - Motion " + mot);
                    }
                }

                foreach (Choice player in choices)
                {
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePathsNormalized.txt", 
                        ""
                        , Encoding.UTF8);
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoiceAngleNormalized.txt", 
                        ""
                        , Encoding.UTF8);
                    player.StartNorm();
                }
                running = true;
                currentLine = "";
                currentLine2 = "";
                timeStepAux = 0;

                while (running)
                {
                    running = false;
                    foreach (Choice player in choices)
                    {
                        currentLine = timeStepAux.ToString() + "\t";
                        currentLine2 = timeStepAux.ToString() + "\t";

                        if (timeStepAux < player.maxMove)
                            for (int nChoice = 0; nChoice < player.movementPos.Count; nChoice++)
                            //                        foreach (Vector2[] choicePath in player.movementPos)
                            {
                                if (timeStepAux < player.movementPos[nChoice].Length)
                                {   
                                    running = true;
                                    try 
                                    {
                                        float timeAux = ((player.movementTime[nChoice][timeStepAux] - player.movementTime[nChoice][player.reaction[nChoice]]) / (player.movementTime[nChoice][player.motion[nChoice]] - player.movementTime[nChoice][player.reaction[nChoice]]));
                                        float posAux = ((player.movementPos[nChoice][timeStepAux].x - player.movementPos[nChoice][0].x) / (player.movementPos[nChoice][player.movementPos[nChoice].Length - 1].x - player.movementPos[nChoice][0].x));
                                        float angAux = posAux*Mathf.Sin(Mathf.Deg2Rad*Mathf.DeltaAngle(player.movementPos[nChoice][timeStepAux].y, player.movementPos[nChoice][player.motion[nChoice]].y));// / 180f); //player.motion[nChoice] | player.movementPos[nChoice].Length - 1

                                        currentLine = currentLine + timeAux + "\t";
                                        currentLine = currentLine + posAux + "\t";
                                        currentLine2 = currentLine2 + timeAux + "\t";
                                        currentLine2 = currentLine2 + angAux + "\t";


                                        player.AddNorm(timeAux, new Vector2(posAux, angAux), nChoice);
                                    }
                                    catch
                                    {
                                        Debug.Log("Player Movement Time Count = " + player.movementTime.Count + " > NChoice = " + nChoice);
                                        Debug.Log("time[nChoice] Lengh = " + player.movementTime[nChoice].Length + " > TimeStepAux = " + timeStepAux + " and Reaction = " + player.reaction[nChoice] + " and Motion =" + player.motion[nChoice]);
                                        Debug.Log("nChoice = " + nChoice + " < Reaction.Count = " + player.reaction.Count + " and Motion.Count = " + player.motion.Count);
                                    }
                                }
                                else
                                {
                                    currentLine = currentLine + "\t\t";
                                    currentLine2 = currentLine2 + "\t\t";
                                }
                            }

                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePathsNormalized.txt", 
                            currentLine +
                            Environment.NewLine
                            , Encoding.UTF8);
                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoiceAngleNormalized.txt", 
                            currentLine2 +
                            Environment.NewLine
                            , Encoding.UTF8);
                    }
                    timeStepAux++;
                }

                File.WriteAllText(Application.dataPath + "/Choices/AllAverageAngle.txt", 
                    ""
                    , Encoding.UTF8);

                foreach (Choice player in choices)
                {
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - AveragePath.txt", 
                        ""
                        , Encoding.UTF8);
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - AverageAngle.txt", 
                        ""
                        , Encoding.UTF8);
                    player.FitNorm();
                }
                foreach (Choice player in choices)
                {
                    for (int iTime = 0; iTime <= Choice.elemNorm; iTime ++)
                    {
                        currentLine = iTime + "\t" + player.moveTimeNorm[iTime] + "\t";
                        currentLine2 = iTime + "\t" + player.moveTimeNorm[iTime] + "\t";
                        for (int iChoice = 0; iChoice < player.movementTime.Count; iChoice++)
                        {
                            currentLine = currentLine + player.movePosNorm[iChoice][iTime].x + "\t";
                            currentLine2 = currentLine2 + player.movePosNorm[iChoice][iTime].y + "\t";
                        }
                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - AveragePath.txt", 
                            currentLine +
                            Environment.NewLine
                            , Encoding.UTF8);
                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - AverageAngle.txt", 
                            currentLine2 +
                            Environment.NewLine
                            , Encoding.UTF8);

                        File.AppendAllText(Application.dataPath + "/Choices/AllAverageAngle.txt", 
                            currentLine2 +
                            Environment.NewLine
                            , Encoding.UTF8);
                    }
                }
                break;
            #endregion
            #region TimesToDo
            case Modes.TimesToDo:
                playerNames = new List<string>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNames.Add(line);
                }
                    
                int iPlayer = -1;
                foreach (string player in playerNames)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);
                    choices.Add(new Choice(player));
                    iPlayer++;
                    int iTotalChoice = -1;


                    for (int iMode = 2; iMode < movementsText.Length; iMode++)
                    {
                        string[] testes = movementsText[iMode].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {
                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);

                            for (int iChoice = 2; iChoice < choiceLines.Length; iChoice++)
                            {
                                string[]choice = choiceLines[iChoice].Split(tab);

                                choices[iPlayer].Add(choice);
                                iTotalChoice++;
                            }
                        }
                    }

             //       for (int i = 0; i < extr.Length; i++)
             //           Debug.Log("Mode = " + i + " -> Min = " + extr[i].x + " e Max = " + extr[i].y);

                    choices[iPlayer].CountTimeStart(20);

                    for (int iChoice = 0; iChoice < choices[iPlayer].nChoice.Count; iChoice++)
                    {
                        for (int i = 0; i < 3; i++)
                            choices[iPlayer].AddCountTime(choices[iPlayer].timeToDo[iChoice][i], 0/*choices[iPlayer].gameMode[iChoice] /** 6 + choices[iPlayer].nCards[iChoice] - 3*/, i);
                    }

                    string line = "";

                    for (int i = 0; i < 20; i++)
                        line = line + "Time \t" + "Amount \t";


                    for (int j = 0; j < 3; j++)
                    {
             //           for (int gameMode = 0; gameMode < 4; gameMode++)
             //           {
                            for (int i = 0; i < 20; i++)
                            {
                            line = "";
             //                   for (int nCards = 0; nCards < 6; nCards++)
              //                 {
                                    int iMode = 0;//gameMode; // * 6 + nCards;
                                    float step = (choices[iPlayer].maxTime[iMode].z - choices[iPlayer].minTime[iMode].z) / choices[iPlayer].countTime[iMode][j].Length;
                                    line = line + (choices[iPlayer].minTime[iMode].z + ((float)i + 0.5f) * step) + "\t" + (choices[iPlayer].countTime[iMode][j][i] == 0? "": choices[iPlayer].countTime[iMode][j][i].ToString()) + "\t";
                 //               }

                                File.AppendAllText(Application.dataPath + "/Choices/" + choices[iPlayer].playerName + " - Time " + j  + " - TimeProfile.txt", 
                                    line +
                                    Environment.NewLine
                                    , Encoding.UTF8);
                            }
            //                File.AppendAllText(Application.dataPath + "/Choices/" + choices[iPlayer].playerName + " - Time " + j  + " - TimeProfile.txt", 
            //                    Environment.NewLine
            //                    , Encoding.UTF8);
            //            }
                    }
                }
                break;
            #endregion
            #region AverageTimes
            case Modes.AverageTimes:
                List<string[]> playerNamesACER = new List<string[]>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/PlayersACER.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNamesACER.Add(line.Split(tab));
                }

                iPlayer = -1;
                foreach (string[] player in playerNamesACER)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - Historic.txt", Encoding.UTF8);
                    choices.Add(new Choice(player));
                    iPlayer++;
                    int iTotalChoice;

                    Vector3[] averageTimeToDo = new Vector3[24];
                    Vector3[] devTimeToDo = new Vector3[24];
                    float[] winRate = new float[24];
                    int[] countTimeToDo = new int[24];

 /*                   float[] curveTimeToDo = new float[1];
                    float[] logTimeToDo = new float[1];
                    int[] countGama = new int[1];
                    float[] k = new float[1];
                    float[] theta = new float[1];
*/
                    for (int i = 0; i < 24; i++)
                    {
                        averageTimeToDo[i] = Vector3.zero;
                        devTimeToDo[i] = Vector3.zero;
                        winRate[i] = 0f;
                        countTimeToDo[i] = 0;
                    }
 /*                   for (int i = 0; i < 1; i++)
                    {
                        curveTimeToDo[i] = 0f;
                        logTimeToDo[i] = 0f;
                        countGama[i] = 0;
                    }
*/
                    for (int iMode = 2; iMode < movementsText.Length; iMode++)
                    {
                        string[] testes = movementsText[iMode].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {
                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);

                            for (int iChoice = 2; iChoice < choiceLines.Length; iChoice++)
                            {
                                string[]choice = choiceLines[iChoice].Split(tab);

                                choices[iPlayer].Add(choice);
                                iTotalChoice = choices[iPlayer].nChoice.Count - 1;
                                int nMode = (choices[iPlayer].gameMode[iTotalChoice] * 6 + choices[iPlayer].nCards[iTotalChoice] - 3);

                                averageTimeToDo[nMode] += choices[iPlayer].timeToDo[iTotalChoice];
                                countTimeToDo[nMode] ++;

 //                               if(choices[iPlayer].timeToDo[iTotalChoice].z > 0f)
   //                             {
     //                               logTimeToDo[0/*choices[iPlayer].gameMode[iTotalChoice]*/] += Mathf.Log(choices[iPlayer].timeToDo[iTotalChoice].z);
       //                             curveTimeToDo[0/*choices[iPlayer].gameMode[iTotalChoice]*/] += choices[iPlayer].timeToDo[iTotalChoice].z;
         //                           countGama[0/*choices[iPlayer].gameMode[iTotalChoice]*/] ++;
           //                     }

                                if (choices[iPlayer].match[iTotalChoice])
                                    winRate[nMode] ++;
                            }
                        }
                    }

                    for (int nMode = 0; nMode < 24; nMode++)
                    {
                        averageTimeToDo[nMode] = averageTimeToDo[nMode] / countTimeToDo[nMode];
                        winRate[nMode] = winRate[nMode] / countTimeToDo[nMode];
                    }
  /*                  for (int nMode = 0; nMode < 1; nMode++)
                    {
                        float s = Mathf.Log(curveTimeToDo[nMode] / countGama[nMode]) - logTimeToDo[nMode] / countGama[nMode];
                        k[nMode] = (3f - s + Mathf.Sqrt((s - 3f)*(s - 3f) + 24f*s)) / (12f * s);
                        theta[nMode] = curveTimeToDo[nMode] / (k[nMode] * countGama[nMode]);
                    }
*/
                    for (int iChoice = 0; iChoice < choices[iPlayer].timeToDo.Count; iChoice++)
                    {
                        int nMode = choices[iPlayer].gameMode[iChoice] * 6 + choices[iPlayer].nCards[iChoice] - 3;

                        for (int i = 0; i < 3; i++)
                            devTimeToDo[nMode][i] += Mathf.Pow(choices[iPlayer].timeToDo[iChoice][i] - averageTimeToDo[nMode][i], 2f) / countTimeToDo[nMode];
                    }

                    for (int nMode = 0; nMode < 24; nMode++)
                        for (int i = 0; i < 3; i++)
                            devTimeToDo[nMode][i] = Mathf.Sqrt(devTimeToDo[nMode][i]);
                    
                    File.WriteAllText(Application.dataPath + "/Choices/" + choices[iPlayer].playerName + " - AverageTimes.txt", 
                        "Name \t" +
                        "Mode \t" +
                        "N Cards \t" +
                        "Win Rate \t" +
                        "Avg Time to Play \t" +
                        "Dev Time to Play \t" +
   //                     "Lambda Time to Play \t" +
                        "Avg Time to Memorize \t" +
                        "Dev Time to Memorize \t" +
 //                       "Lambda Time to Memorize \t" + 
                        "Avg Time to Choose \t" +
                        "Dev Time to Choose \t" +
   //                     "k Time to Choose \t" +
    //                    "theta Time to Choose" +
                        Environment.NewLine
                        , Encoding.UTF8);


                    string names = "";
                    foreach (String[] plr in playerNamesACER)
                    {
                        if (names == "")
                            names = names + plr[0].Replace(" ", string.Empty);
                        else
                            names = names + "," + plr[0].Replace(" ", string.Empty);
                    }

                    if (!File.Exists(Application.dataPath + "/Choices/AllChoices.arff"))
                        File.WriteAllText(Application.dataPath + "/Choices/AllChoices.arff", 
                            "@RELATION AllChoices" + Environment.NewLine +
                            Environment.NewLine +
                            "@ATTRIBUTE Name {" + names + "}" + Environment.NewLine +
                            "@ATTRIBUTE Mode {0,1,2,3}" + Environment.NewLine +
                            "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                            "@ATTRIBUTE Match REAL" + Environment.NewLine +
                            "@ATTRIBUTE Value REAL" + Environment.NewLine +
                            "@ATTRIBUTE Suit REAL" + Environment.NewLine +
                            "@ATTRIBUTE Color REAL" + Environment.NewLine +
                            "@ATTRIBUTE TimeToPlay REAL" + Environment.NewLine +
                            "@ATTRIBUTE TimeToMemorize REAL" + Environment.NewLine +
                            "@ATTRIBUTE TimeToChoose REAL" + Environment.NewLine +
                            "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                            "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                            "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                            "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                            "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                            "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                            "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                            Environment.NewLine +
                            "@DATA" +
                            Environment.NewLine, Encoding.Default);

                    for (int iChoice = 0; iChoice < choices[iPlayer].timeToDo.Count; iChoice++)
                    {
                        File.AppendAllText(Application.dataPath + "/Choices/AllChoices.arff", 
                            choices[iPlayer].playerName.Replace(" ", string.Empty) + "," +
                            choices[iPlayer].gameMode[iChoice] + "," +
                            choices[iPlayer].nCards[iChoice] + "," +
                            Convert.ToSingle(choices[iPlayer].match[iChoice]) + "," +
                            Convert.ToSingle(choices[iPlayer].cardMatch[iChoice].value) + "," +
                            Convert.ToSingle(choices[iPlayer].cardMatch[iChoice].suit) + "," +
                            Convert.ToSingle(choices[iPlayer].cardMatch[iChoice].color) + "," +
                            choices[iPlayer].timeToDo[iChoice].x + "," +
                            choices[iPlayer].timeToDo[iChoice].y + "," +
                            choices[iPlayer].timeToDo[iChoice].z + "," +
                            choices[iPlayer].ACER[0] + "," +
                            choices[iPlayer].ACER[1] + "," +
                            choices[iPlayer].ACER[2] + "," +
                            choices[iPlayer].ACER[3] + "," +
                            choices[iPlayer].ACER[4] + "," +
                            choices[iPlayer].ACER[5] + "," +
                            choices[iPlayer].ACER[6] +
                            Environment.NewLine, Encoding.Default);

                        if (!File.Exists(Application.dataPath + "/Choices/AllChoices - Mode " + choices[iPlayer].gameMode[iChoice] +  ".arff"))
                            File.WriteAllText(Application.dataPath + "/Choices/AllChoices - Mode " + choices[iPlayer].gameMode[iChoice] +  ".arff", 
                                "@RELATION AllChoices" + Environment.NewLine +
                                Environment.NewLine +
                                "@ATTRIBUTE Name {" + names + "}" + Environment.NewLine +
                                "@ATTRIBUTE Mode {0,1,2,3}" + Environment.NewLine +
                                "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                                "@ATTRIBUTE Match REAL" + Environment.NewLine +
                                "@ATTRIBUTE Value REAL" + Environment.NewLine +
                                "@ATTRIBUTE Suit REAL" + Environment.NewLine +
                                "@ATTRIBUTE Color REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                                "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                                "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                                "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                                "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                                Environment.NewLine +
                                "@DATA" +
                                Environment.NewLine, Encoding.Default);

                        File.AppendAllText(Application.dataPath + "/Choices/AllChoices - Mode " + choices[iPlayer].gameMode[iChoice] +  ".arff", 
                            choices[iPlayer].playerName.Replace(" ", string.Empty) + "," +
                            choices[iPlayer].gameMode[iChoice] + "," +
                            choices[iPlayer].nCards[iChoice] + "," +
                            Convert.ToSingle(choices[iPlayer].match[iChoice]) + "," +
                            Convert.ToSingle(choices[iPlayer].cardMatch[iChoice].value) + "," +
                            Convert.ToSingle(choices[iPlayer].cardMatch[iChoice].suit) + "," +
                            Convert.ToSingle(choices[iPlayer].cardMatch[iChoice].color) + "," +
                            choices[iPlayer].timeToDo[iChoice].x + "," +
                            choices[iPlayer].timeToDo[iChoice].y + "," +
                            choices[iPlayer].timeToDo[iChoice].z + "," +
                            choices[iPlayer].ACER[0] + "," +
                            choices[iPlayer].ACER[1] + "," +
                            choices[iPlayer].ACER[2] + "," +
                            choices[iPlayer].ACER[3] + "," +
                            choices[iPlayer].ACER[4] + "," +
                            choices[iPlayer].ACER[5] + "," +
                            choices[iPlayer].ACER[6] +
                            Environment.NewLine, Encoding.Default);
                    }
                
                    for (int iMode = 0; iMode < 4; iMode++)
                    {
                        if (!File.Exists(Application.dataPath + "/Choices/AverageTimes - Mode " + Mode(iMode) + ".txt"))
                            File.WriteAllText(Application.dataPath + "/Choices/AverageTimes - Mode " + Mode(iMode) + ".txt", 
                                "Name \t" +
                                "Mode \t" +
                                "N Cards \t" +
                                "Win Rate \t" +
                                "Avg Time to Play \t" +
                                "Dev Time to Play \t" +
//                                "Lambda Time to Play \t" +
                                "Min Time to Play \t" +
                                "Max Time to Play \t" +
                                "Avg Time to Memorize \t" +
                                "Dev Time to Memorize \t" +
   //                             "Lambda Time to Memorize \t" +
                                "Min Time to Memorize \t" +
                                "Max Time to Memorize \t" +
                                "Avg Time to Choose \t" +
                                "Dev Time to Choose \t" +
   //                             "k Time to Choose \t" +
   //                             "theta Time to Choose \t" +
                                "Min Time to Choose \t" +
                                "Max Time to Choose" +
                                Environment.NewLine
                                , Encoding.UTF8);

                        if (!File.Exists(Application.dataPath + "/Choices/AverageTimes Mode - " + Mode(iMode) + ".arff"))
                            File.WriteAllText(Application.dataPath + "/Choices/AverageTimes Mode - " + Mode(iMode) + ".arff", 
                                "@RELATION " + Mode(iMode) + Environment.NewLine +
                                Environment.NewLine +
                                "@ATTRIBUTE Name {" + names + "}" + Environment.NewLine +
                                "@ATTRIBUTE Mode REAL" + Environment.NewLine +
                                "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                                "@ATTRIBUTE WinRate REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevTimetoPlay REAL" + Environment.NewLine +
//                                "@ATTRIBUTE LambdaTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE MinTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE MaxTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevTimetoMemorize REAL" + Environment.NewLine +
  //                              "@ATTRIBUTE LambdaTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE MinTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE MaxTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevTimetoChoose REAL" + Environment.NewLine +
    //                            "@ATTRIBUTE kTimetoChoose REAL" + Environment.NewLine +
      //                          "@ATTRIBUTE LambdaTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE MinTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE MaxTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                                "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                                "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                                "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                                "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                                Environment.NewLine +
                                "@DATA" +
                                Environment.NewLine, Encoding.Default);

                        for (int nCards = 0; nCards < 6; nCards++)
                        {
                            int index = (iMode * 6 + nCards);
                            File.AppendAllText(Application.dataPath + "/Choices/" + choices[iPlayer].playerName + " - AverageTimes.txt", 
                                choices[iPlayer].playerName + "\t" +
                                iMode + "\t" +
                                (nCards + 3) + "\t" +
                                winRate[index] + "\t" +
                                averageTimeToDo[index].x + "\t" +
                                devTimeToDo[index].x + "\t" +
 //                               (averageTimeToDo[index].x == 0f? 0f: 1f / averageTimeToDo[index].x) + "\t" +
                                choices[iPlayer].minTime[index].x + "\t" +
                                choices[iPlayer].maxTime[index].x + "\t" +
                                averageTimeToDo[index].y + "\t" +
                                devTimeToDo[index].y + "\t" +
   //                             (averageTimeToDo[index].y == 0f? 0f: 1f / averageTimeToDo[index].y) + "\t" +
                                choices[iPlayer].minTime[index].y + "\t" +
                                choices[iPlayer].maxTime[index].y + "\t" +
                                averageTimeToDo[index].z + "\t" +
                                devTimeToDo[index].z + "\t" +
     //                           k[0] + "\t" +
    //                            theta[0] + "\t" +
                                choices[iPlayer].minTime[index].z + "\t" +
                                choices[iPlayer].maxTime[index].z +
                                Environment.NewLine
                                , Encoding.UTF8);

                            File.AppendAllText(Application.dataPath + "/Choices/AverageTimes - Mode " + Mode(iMode) + ".txt", 
                                choices[iPlayer].playerName + "\t" +
                                iMode + "\t" +
                                (nCards + 3) + "\t" +
                                winRate[index] + "\t" +
                                averageTimeToDo[index].x + "\t" +
                                devTimeToDo[index].x + "\t" +
   //                             (averageTimeToDo[index].x == 0f? 0f: 1f / averageTimeToDo[index].x) + "\t" +
                                choices[iPlayer].minTime[index].x + "\t" +
                                choices[iPlayer].maxTime[index].x + "\t" +
                                averageTimeToDo[index].y + "\t" +
                                devTimeToDo[index].y + "\t" +
   //                             (averageTimeToDo[index].y == 0f? 0f: 1f / averageTimeToDo[index].y) + "\t" +
                                choices[iPlayer].minTime[index].y + "\t" +
                                choices[iPlayer].maxTime[index].y + "\t" +
                                averageTimeToDo[index].z + "\t" +
                                devTimeToDo[index].z + "\t" +
   //                             k[0] + "\t" +
    //                            theta[0] + "\t" +
                                choices[iPlayer].minTime[index].z + "\t" +
                                choices[iPlayer].maxTime[index].z +
                                Environment.NewLine
                                , Encoding.UTF8);
                            
                            File.AppendAllText(Application.dataPath + "/Choices/AverageTimes Mode - " + Mode(iMode) + ".arff", 
                                choices[iPlayer].playerName.Replace(" ", string.Empty) + "," +
                                iMode + "," +
                                (nCards + 3) + "," +
                                winRate[index] + "," +
                                averageTimeToDo[index].x + "," +
                                devTimeToDo[index].x + "," +
    //                            (averageTimeToDo[index].x == 0f? 0f: 1f / averageTimeToDo[index].x) + "," +
                                choices[iPlayer].minTime[index].x + "," +
                                choices[iPlayer].maxTime[index].x + "," +
                                averageTimeToDo[index].y + "," +
                                devTimeToDo[index].y + "," +
     //                           (averageTimeToDo[index].y == 0f? 0f: 1f / averageTimeToDo[index].y) + "," +
                                choices[iPlayer].minTime[index].y + "," +
                                choices[iPlayer].maxTime[index].y + "," +
                                averageTimeToDo[index].z + "," +
                                devTimeToDo[index].z + "," +
     //                           k[0] + "," +
      //                          theta[0] + "," +
                                choices[iPlayer].minTime[index].z + "," +
                                choices[iPlayer].maxTime[index].z + "," +
                                choices[iPlayer].ACER[0] + "," +
                                choices[iPlayer].ACER[1] + "," +
                                choices[iPlayer].ACER[2] + "," +
                                choices[iPlayer].ACER[3] + "," +
                                choices[iPlayer].ACER[4] + "," +
                                choices[iPlayer].ACER[5] + "," +
                                choices[iPlayer].ACER[6] +
                                Environment.NewLine
                                , Encoding.Default);
                        }
                    }


                }
                break;
                #endregion
            #region ReactionTime
            case Modes.ReactionTime:
                playerNamesACER = new List<string[]>();
                //playerNames = new List<string>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/PlayersACER.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNamesACER.Add(line.Split(tab));
                }

                foreach (string[] player in playerNamesACER)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - Historic.txt", Encoding.UTF8);
                    choices.Add(new Choice(player));

                    for (int iMode = 2; iMode < movementsText.Length; iMode++)
                    {
                        int index0 = 0;
                        int index1 = 0;

                        string[] testes = movementsText[iMode].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {

                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Movements.txt", Encoding.UTF8);

                            movementTime = new float[choiceLines.Length - 1];
                            movementPos = new Vector2[choiceLines.Length - 1];
                            movementVel = new Vector2[choiceLines.Length - 1];

                            min = max = Vector2.zero;

                            for (int j = 1; j < choiceLines.Length; j++)
                            {
                                string[]choice = choiceLines[j].Split(tab);
                                movementTime[j - 1] = float.Parse(choice[0]);
                                Vector2 pos = new Vector2(float.Parse(choice[1]), float.Parse(choice[2]));

                                movementPos[j - 1] = pos; // new Vector2(pos.magnitude, Mathf.Atan2(pos.y, pos.x));

                                if (j >= 2)
                                {
                                    if (movementPos[j - 2] == Vector2.zero)
                                    if (j == 2)
                                    {
                                        movementPos[j - 2] = movementPos[j - 1];
                                    }
                                    else
                                    {
                                        movementPos[j - 2] = (movementPos[j - 1] + movementPos[j - 3])/2f;
                                    }
                                }
                            }

                            choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player[0] + "/" + player[0] + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);

                            //                            Debug.Log(player + " has " + choiceLines.Length + " choices");

                            float startTime = 0f;
                            float endTime = 0f;

                            for (int iChoice = 2; iChoice < choiceLines.Length; iChoice++)
                            {
                                string[]choice = choiceLines[iChoice].Split(tab);
                                int lastPlayer = choices.Count - 1;
                                choices[lastPlayer].Add(choice);

                                int lastChoice = choices[lastPlayer].nChoice.Count - 1;

                                endTime = choices[lastPlayer].time[lastChoice]; //  + 2.5f / choices[lastPlayer].speed[lastChoice];
                                /*if (!choices[lastPlayer].match[lastChoice])
                                    endTime += 1.5f / choices[lastPlayer].speed[lastChoice];
                                endTime += 0.75f / choices[lastPlayer].speed[lastChoice];
                                endTime += (1.5f + (choices[lastPlayer].nCards[lastChoice] - 1) * 0.25f) / choices[lastPlayer].speed[lastChoice];
*/
                                //endTime -=  1.25f / choices[lastPlayer].speed[lastChoice];
                                startTime = endTime - (1.25f / choices[lastPlayer].speed[lastChoice] + choices[lastPlayer].timeToDo[lastChoice].z);

                                while ((movementTime[index1] < endTime) && (index1 != movementTime.Length - 1))
                                {
                                    if (movementTime[index0] < startTime)
                                        index0++;
                                    index1++;
                                }

                                if (index0 != index1)
                                {
                                    choices[lastPlayer].Add(movementTime, movementPos, index0, index1);
                                }
                                else
                                    Debug.Log("Erro - " + index0 + " - " + choices[lastPlayer].playerName);

                                index0 = index1;
                            }
                            //    Debug.Log(player + " - endTime = " + endTime.ToString() + " | choice = " + (choices[choices.Count - 1].nChoice.Count - 1).ToString());
                        }
                        else
                        {
                            Debug.Log(player[0] + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                        }
                    }
                }

                running = true;
                currentLine = "";
                currentLine2 = "";
                timeStepAux = 0;

                foreach (Choice player in choices)
                {
                    foreach (Vector2[] iChoice in player.movementPos)
                    {
                        float magStart = iChoice[0].x;
                        float magEnd = iChoice[iChoice.Length - 1].x;
                        int reac = -1, mot = -1, chos = -1;

                        for (int iPos = 0; iPos < iChoice.Length; iPos++)
                        {
                            if (reac < 0)
                            if (Mathf.Abs(iChoice[iPos].x - magStart) > magEnd * 0.05f)
                                reac = iPos - 1;

                            if (Mathf.Abs(iChoice[iPos].x - magEnd) > magEnd * 0.05f)
                                mot = -1;

                            if (mot < 0)
                            if (Mathf.Abs(iChoice[iPos].x - magEnd) < magEnd * 0.05f)
                                mot = iPos;
                        }
                        player.Add(reac, mot);
                        if ((reac < 0) || (mot < 0))
                            Debug.Log(player.playerName + " - Reaction " + reac + " - Motion " + mot);
                    }
                }

                File.WriteAllText(Application.dataPath + "/Choices/AllChoicesWithReaction.txt",
                    "ID\t" + 
                    "Name\t" +
                    "Choice\t" + 
                    "Time Start\t" +
                    "Reaction Time\t" +
                    "Motion Time\t" +
                    "End Turn\t" + 
                    "Game Mode\t" + 
                    "Game Speed\t" +
                    "nCard\t" +
                    "Match\t" + 
                    "Turn Time\t" +
                    "Choice Changed\t" +
                    "Objective Value\t" +
                    "Objective Suit\t" +
                    "Objective Color\t" +
                    "Choice Value\t" +
                    "Choice Suit\t" +
                    "Choice Color\t" +
                    "Match Value\t" +
                    "Match Suit\t" +
                    "Match Color\t" +
                    "Time to Play\t" +
                    "Time to Memorize\t" +
                    "Time to Choose\t" +
                    "Objective Mag\t" +
                    "Objective Ang\t" +
                    "Choice Mag\t" +
                    "Choice Ang\t" +
                    Environment.NewLine, Encoding.UTF8);

                string namesAux = "";
                foreach (String[] plr in playerNamesACER)
                {
                    if (namesAux == "")
                        namesAux = namesAux + plr[0].Replace(" ", string.Empty);
                    else
                        namesAux = namesAux + "," + plr[0].Replace(" ", string.Empty);
                }

                File.WriteAllText(Application.dataPath + "/Choices/AllChoicesWithReaction.arff",
                    "@RELATION AllChoicesWithReaction" + Environment.NewLine +
                    Environment.NewLine +
                    "@ATTRIBUTE ID REAL" + Environment.NewLine + 
                    "@ATTRIBUTE Name {" + namesAux + "}" + Environment.NewLine +
                    "@ATTRIBUTE Choice REAL" + Environment.NewLine +
                    "@ATTRIBUTE TimeStart REAL" + Environment.NewLine +
                    "@ATTRIBUTE RectionTime REAL" + Environment.NewLine +
                    "@ATTRIBUTE MotionTime REAL" + Environment.NewLine +
                    "@ATTRIBUTE EndTurn REAL" + Environment.NewLine +
                    "@ATTRIBUTE Mode {0,1,2,3}" + Environment.NewLine +
                    "@ATTRIBUTE GameSpeed REAL" + Environment.NewLine +
                    "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                    "@ATTRIBUTE Match REAL" + Environment.NewLine +
                    "@ATTRIBUTE TurnTime REAL" + Environment.NewLine +
                    "@ATTRIBUTE ChoiceChanged REAL" + Environment.NewLine +
                    "@ATTRIBUTE ObjectiveValue REAL" + Environment.NewLine +
                    "@ATTRIBUTE ObjectiveSuit REAL" + Environment.NewLine +
                    "@ATTRIBUTE ObjectiveColor REAL" + Environment.NewLine +
                    "@ATTRIBUTE ChoiceValue REAL" + Environment.NewLine +
                    "@ATTRIBUTE ChoiceSuit REAL" + Environment.NewLine +
                    "@ATTRIBUTE ChoiceColor REAL" + Environment.NewLine +
                    "@ATTRIBUTE MatchValue REAL" + Environment.NewLine +
                    "@ATTRIBUTE MatchSuit REAL" + Environment.NewLine +
                    "@ATTRIBUTE MatchColor REAL" + Environment.NewLine +
                    "@ATTRIBUTE TimeToPlay REAL" + Environment.NewLine +
                    "@ATTRIBUTE TimeToMemorize REAL" + Environment.NewLine +
                    "@ATTRIBUTE TimeToChoose REAL" + Environment.NewLine +
                    "@ATTRIBUTE ObjectiveMag REAL" + Environment.NewLine +
                    "@ATTRIBUTE ObjectiveAng REAL" + Environment.NewLine +
                    "@ATTRIBUTE ChoiceMag REAL" + Environment.NewLine +
                    "@ATTRIBUTE ChoiceAng REAL" + Environment.NewLine +
                    "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                    "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                    "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                    "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                    "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                    "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                    "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                    Environment.NewLine +
                    "@DATA" +
                    Environment.NewLine, Encoding.Default);

                File.WriteAllText(Application.dataPath + "/Choices/AverageTimesWithReaction.txt", 
                    "ID \t" +
                    "Name \t" +
                    "Mode \t" +
                    "N Cards \t" +
                    "Win Rate \t"  +
                    "Avg Reaction Time \t" +
                    "Dev Reaction Time \t" +
                    "Avg Motion Time \t"+
                    "Dev Motion Time \t" +
                    "Avg Time to Play \t" +
                    "Dev Time to Play \t" +
                    "Min Time to Play \t" +
                    "Max Time to Play \t" +
                    "Avg Time to Memorize \t" +
                    "Dev Time to Memorize \t" +
                    "Min Time to Memorize \t" +
                    "Max Time to Memorize \t" +
                    "Avg Time to Choose \t" +
                    "Dev Time to Choose \t" +
                    "Min Time to Choose \t" +
                    "Max Time to Choose \t" +
                    "GameFamiliarity \t" +
                    "CardFamiliarity \t" +
                    "MEEM-30 \t" +
                    "ACER-100 \t" + 
                    "Atençao-Orientacao-18 \t" +
                    "Memoria-26 \t" +
                    "Fluencia-14 \t" +
                    "Linguagem-26 \t" +
                    "Visual-espacial-16" +
                    Environment.NewLine
                    , Encoding.UTF8);

                foreach (Choice player in choices)
                {

                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicesWithReaction.txt",
                        "ID\t" +
                        "Name\t" +
                        "Choice\t" + 
                        "Time Start\t" +
                        "Reaction Time\t" +
                        "Motion Time\t" +
                        "End Turn\t" + 
                        "Game Mode\t" + 
                        "Game Speed\t" +
                        "nCard\t" +
                        "Match\t" + 
                        "Turn Time\t" +
                        "Choice Changed\t" +
                        "Objective Value\t" +
                        "Objective Suit\t" +
                        "Objective Color\t" +
                        "Choice Value\t" +
                        "Choice Suit\t" +
                        "Choice Color\t" +
                        "Match Value\t" +
                        "Match Suit\t" +
                        "Match Color\t" +
                        "Time to Play\t" +
                        "Time to Memorize\t" +
                        "Time to Choose\t" +
                        "Objective Mag\t" +
                        "Objective Ang\t" +
                        "Choice Mag\t" +
                        "Choice Ang\t" +
                        "MEEM-30 \t" +
                        "ACER-100 \t" + 
                        "Atençao-Orientacao-18 \t" +
                        "Memoria-26 \t" +
                        "Fluencia-14 \t" +
                        "Linguagem-26 \t" +
                        "Visual-espacial-16" +
                        Environment.NewLine, Encoding.UTF8);

                    Vector3[] averageTimeToDo = new Vector3[24];
                    Vector3[] devTimeToDo = new Vector3[24];
                    float[] winRate = new float[24];
                    int[] countTimeToDo = new int[24];

                    Vector2[] averageRection = new Vector2[24];
                    Vector2[] devReaction = new Vector2[24];

                    for (int i = 0; i < 24; i++)
                    {
                        averageTimeToDo[i] = Vector3.zero;
                        devTimeToDo[i] = Vector3.zero;
                        winRate[i] = 0f;
                        countTimeToDo[i] = 0;

                        averageRection[i] = Vector2.zero;
                        devReaction[i] = Vector2.zero;
                    }

                    for (int iChoice = 0; iChoice < player.nChoice.Count; iChoice++)
                    {
                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicesWithReaction.txt",
                            player.playerID + "\t" + 
                            player.playerName + "\t" +
                            iChoice + "\t" + 
                            player.movementTime[iChoice][0] + "\t" +
                            (player.movementTime[iChoice][player.reaction[iChoice]] - player.movementTime[iChoice][0]) + "\t" +
                            (player.movementTime[iChoice][player.motion[iChoice]] - player.movementTime[iChoice][player.reaction[iChoice]]) + "\t" +
                            player.time[iChoice] + "\t" + 
                            player.gameMode[iChoice] + "\t" + 
                            player.speed[iChoice] + "\t" +
                            player.nCards[iChoice] + "\t" +
                            player.match[iChoice] + "\t" + 
                            player.turnTime[iChoice] + "\t" +
                            player.choiceChanged[iChoice] + "\t" +
                            player.objective[iChoice].value + "\t" +
                            player.objective[iChoice].suit + "\t" +
                            player.objective[iChoice].color + "\t" +
                            player.choice[iChoice].value + "\t" +
                            player.choice[iChoice].suit + "\t" +
                            player.choice[iChoice].color + "\t" +
                            player.cardMatch[iChoice].value + "\t" +
                            player.cardMatch[iChoice].suit + "\t" +
                            player.cardMatch[iChoice].color + "\t" +
                            player.timeToDo[iChoice].x + "\t" +
                            player.timeToDo[iChoice].y + "\t" +
                            player.timeToDo[iChoice].z + "\t" +
                            player.objectivePos[iChoice].x + "\t" +
                            player.objectivePos[iChoice].y + "\t" +
                            player.choicePos[iChoice].x + "\t" +
                            player.choicePos[iChoice].y + "\t" +
                            player.ACER[0] + "\t" +
                            player.ACER[1] + "\t" +
                            player.ACER[2] + "\t" +
                            player.ACER[3] + "\t" +
                            player.ACER[4] + "\t" +
                            player.ACER[5] + "\t" +
                            player.ACER[6] + 
                            Environment.NewLine, Encoding.UTF8);

                        File.AppendAllText(Application.dataPath + "/Choices/AllChoicesWithReaction.txt",
                            player.playerID + "\t" +
                            player.playerName + "\t" +
                            iChoice + "\t" + 
                            player.movementTime[iChoice][0] + "\t" +
                            (player.movementTime[iChoice][player.reaction[iChoice]] - player.movementTime[iChoice][0]) + "\t" +
                            (player.movementTime[iChoice][player.motion[iChoice]] - player.movementTime[iChoice][player.reaction[iChoice]]) + "\t" +
                            player.time[iChoice] + "\t" + 
                            player.gameMode[iChoice] + "\t" + 
                            player.speed[iChoice] + "\t" +
                            player.nCards[iChoice] + "\t" +
                            player.match[iChoice] + "\t" + 
                            player.turnTime[iChoice] + "\t" +
                            player.choiceChanged[iChoice] + "\t" +
                            player.objective[iChoice].value + "\t" +
                            player.objective[iChoice].suit + "\t" +
                            player.objective[iChoice].color + "\t" +
                            player.choice[iChoice].value + "\t" +
                            player.choice[iChoice].suit + "\t" +
                            player.choice[iChoice].color + "\t" +
                            player.cardMatch[iChoice].value + "\t" +
                            player.cardMatch[iChoice].suit + "\t" +
                            player.cardMatch[iChoice].color + "\t" +
                            player.timeToDo[iChoice].x + "\t" +
                            player.timeToDo[iChoice].y + "\t" +
                            player.timeToDo[iChoice].z + "\t" +
                            player.objectivePos[iChoice].x + "\t" +
                            player.objectivePos[iChoice].y + "\t" +
                            player.choicePos[iChoice].x + "\t" +
                            player.choicePos[iChoice].y + "\t" +
                            player.ACER[0] + "\t" +
                            player.ACER[1] + "\t" +
                            player.ACER[2] + "\t" +
                            player.ACER[3] + "\t" +
                            player.ACER[4] + "\t" +
                            player.ACER[5] + "\t" +
                            player.ACER[6] +
                            Environment.NewLine, Encoding.UTF8);

                        File.AppendAllText(Application.dataPath + "/Choices/AllChoicesWithReaction.arff",
                            player.ID + "," +
                            player.playerName.Replace(" ", string.Empty) + "," +
                            iChoice + "," + 
                            player.movementTime[iChoice][0] + "," +
                            (player.movementTime[iChoice][player.reaction[iChoice]] - player.movementTime[iChoice][0]) + "," +
                            (player.movementTime[iChoice][player.motion[iChoice]] - player.movementTime[iChoice][player.reaction[iChoice]]) + "," +
                            player.time[iChoice] + "," + 
                            player.gameMode[iChoice] + "," + 
                            player.speed[iChoice] + "," +
                            player.nCards[iChoice] + "," +
                            Convert.ToSingle(player.match[iChoice]) + "," + 
                            player.turnTime[iChoice] + "," +
                            player.choiceChanged[iChoice] + "," +
                            player.objective[iChoice].value + "," +
                            player.objective[iChoice].suit + "," +
                            player.objective[iChoice].color + "," +
                            player.choice[iChoice].value + "," +
                            player.choice[iChoice].suit + "," +
                            player.choice[iChoice].color + "," +
                            player.cardMatch[iChoice].value + "," +
                            player.cardMatch[iChoice].suit + "," +
                            player.cardMatch[iChoice].color + "," +
                            player.timeToDo[iChoice].x + "," +
                            player.timeToDo[iChoice].y + "," +
                            player.timeToDo[iChoice].z + "," +
                            player.objectivePos[iChoice].x + "," +
                            player.objectivePos[iChoice].y + "," +
                            player.choicePos[iChoice].x + "," +
                            player.choicePos[iChoice].y + "," +
                            player.ACER[0] + "," +
                            player.ACER[1] + "," +
                            player.ACER[2] + "," +
                            player.ACER[3] + "," +
                            player.ACER[4] + "," +
                            player.ACER[5] + "," +
                            player.ACER[6] +
                            Environment.NewLine, Encoding.Default);

                        if (!File.Exists(Application.dataPath + "/Choices/AllChoicesWithReaction - Mode " + Mode(player.gameMode[iChoice]) + ".arff"))
                            File.WriteAllText(Application.dataPath + "/Choices/AllChoicesWithReaction - Mode " + Mode(player.gameMode[iChoice]) + ".arff",
                                "@RELATION AllChoicesWithReaction" + Environment.NewLine +
                                Environment.NewLine +
                                "@ATTRIBUTE ID REAL" + Environment.NewLine + 
                                "@ATTRIBUTE Name {" + namesAux + "}" + Environment.NewLine +
                                "@ATTRIBUTE Choice REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeStart REAL" + Environment.NewLine +
                                "@ATTRIBUTE RectionTime REAL" + Environment.NewLine +
                                "@ATTRIBUTE MotionTime REAL" + Environment.NewLine +
                                "@ATTRIBUTE EndTurn REAL" + Environment.NewLine +
                                "@ATTRIBUTE Mode {0,1,2,3}" + Environment.NewLine +
                                "@ATTRIBUTE GameSpeed REAL" + Environment.NewLine +
                                "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                                "@ATTRIBUTE Match REAL" + Environment.NewLine +
                                "@ATTRIBUTE TurnTime REAL" + Environment.NewLine +
                                "@ATTRIBUTE ChoiceChanged REAL" + Environment.NewLine +
                                "@ATTRIBUTE ObjectiveValue REAL" + Environment.NewLine +
                                "@ATTRIBUTE ObjectiveSuit REAL" + Environment.NewLine +
                                "@ATTRIBUTE ObjectiveColor REAL" + Environment.NewLine +
                                "@ATTRIBUTE ChoiceValue REAL" + Environment.NewLine +
                                "@ATTRIBUTE ChoiceSuit REAL" + Environment.NewLine +
                                "@ATTRIBUTE ChoiceColor REAL" + Environment.NewLine +
                                "@ATTRIBUTE MatchValue REAL" + Environment.NewLine +
                                "@ATTRIBUTE MatchSuit REAL" + Environment.NewLine +
                                "@ATTRIBUTE MatchColor REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE ObjectiveMag REAL" + Environment.NewLine +
                                "@ATTRIBUTE ObjectiveAng REAL" + Environment.NewLine +
                                "@ATTRIBUTE ChoiceMag REAL" + Environment.NewLine +
                                "@ATTRIBUTE ChoiceAng REAL" + Environment.NewLine +
                                "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                                "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                                "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                                "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                                "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                                Environment.NewLine +
                                "@DATA" +
                                Environment.NewLine, Encoding.Default);

                        File.AppendAllText(Application.dataPath + "/Choices/AllChoicesWithReaction - Mode " + Mode(player.gameMode[iChoice]) + ".arff",
                            player.ID + "," +
                            player.playerName.Replace(" ", string.Empty) + "," +
                            iChoice + "," + 
                            player.movementTime[iChoice][0] + "," +
                            (player.movementTime[iChoice][player.reaction[iChoice]] - player.movementTime[iChoice][0]) + "," +
                            (player.movementTime[iChoice][player.motion[iChoice]] - player.movementTime[iChoice][player.reaction[iChoice]]) + "," +
                            player.time[iChoice] + "," + 
                            player.gameMode[iChoice] + "," + 
                            player.speed[iChoice] + "," +
                            player.nCards[iChoice] + "," +
                            Convert.ToSingle(player.match[iChoice]) + "," + 
                            player.turnTime[iChoice] + "," +
                            player.choiceChanged[iChoice] + "," +
                            player.objective[iChoice].value + "," +
                            player.objective[iChoice].suit + "," +
                            player.objective[iChoice].color + "," +
                            player.choice[iChoice].value + "," +
                            player.choice[iChoice].suit + "," +
                            player.choice[iChoice].color + "," +
                            player.cardMatch[iChoice].value + "," +
                            player.cardMatch[iChoice].suit + "," +
                            player.cardMatch[iChoice].color + "," +
                            player.timeToDo[iChoice].x + "," +
                            player.timeToDo[iChoice].y + "," +
                            player.timeToDo[iChoice].z + "," +
                            player.objectivePos[iChoice].x + "," +
                            player.objectivePos[iChoice].y + "," +
                            player.choicePos[iChoice].x + "," +
                            player.choicePos[iChoice].y + "," +
                            player.ACER[0] + "," +
                            player.ACER[1] + "," +
                            player.ACER[2] + "," +
                            player.ACER[3] + "," +
                            player.ACER[4] + "," +
                            player.ACER[5] + "," +
                            player.ACER[6] +
                            Environment.NewLine, Encoding.Default);

                        int nMode = (player.gameMode[iChoice] * 6 + player.nCards[iChoice] - 3);

                        averageTimeToDo[nMode] += player.timeToDo[iChoice];
                        countTimeToDo[nMode] ++;

                        float reac = player.movementTime[iChoice][player.reaction[iChoice]] - player.movementTime[iChoice][0];
                        float motion = player.movementTime[iChoice][player.motion[iChoice]] - player.movementTime[iChoice][player.reaction[iChoice]];

                        averageRection[nMode] += new Vector2(reac, motion);

                        if (player.match[iChoice])
                            winRate[nMode] ++;
                    }

                    for (int i = 0; i < 24; i++)
                    {
                        averageTimeToDo[i] = averageTimeToDo[i] / countTimeToDo[i];
                        winRate[i] = winRate[i] / countTimeToDo[i];

                        averageRection[i] = averageRection[i] / countTimeToDo[i];
                    }

                    for (int iChoice = 0; iChoice < player.timeToDo.Count; iChoice++)
                    {
                        int nMode = player.gameMode[iChoice] * 6 + player.nCards[iChoice] - 3;

                        for (int i = 0; i < 3; i++)
                            devTimeToDo[nMode][i] += Mathf.Pow(player.timeToDo[iChoice][i] - averageTimeToDo[nMode][i], 2f) / countTimeToDo[nMode];

                        float reac = player.movementTime[iChoice][player.reaction[iChoice]] - player.movementTime[iChoice][0];
                        float motion = player.movementTime[iChoice][player.motion[iChoice]] - player.movementTime[iChoice][player.reaction[iChoice]];

                        devReaction[nMode].x += Mathf.Pow(reac - averageRection[nMode].x, 2f) / countTimeToDo[nMode];
                        devReaction[nMode].y += Mathf.Pow(motion - averageRection[nMode].y, 2f) / countTimeToDo[nMode];
                    }

                    for (int nMode = 0; nMode < 24; nMode++)
                    {
                        for (int i = 0; i < 3; i++)
                            devTimeToDo[nMode][i] = Mathf.Sqrt(devTimeToDo[nMode][i]);
                        for (int i = 0; i < 2; i++)
                            devReaction[nMode][i] = Mathf.Sqrt(devReaction[nMode][i]);
                    }

                    player.ReplaceTime();

                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - AverageTimesWithReaction.txt", 
                        "ID \t" +
                        "Name \t" +
                        "Mode \t" +
                        "N Cards \t" +
                        "Win Rate \t"  +
                        "Avg Reaction Time \t" +
                        "Dev Reaction Time \t" +
                        "Avg Motion Time \t"+
                        "Dev Motion Time \t" +
                        "Avg Time to Play \t" +
                        "Dev Time to Play \t" +
                        "Min Time to Play \t" +
                        "Max Time to Play \t" +
                        "Avg Time to Memorize \t" +
                        "Dev Time to Memorize \t" +
                        "Min Time to Memorize \t" +
                        "Max Time to Memorize \t" +
                        "Avg Time to Choose \t" +
                        "Dev Time to Choose \t" +
                        "Min Time to Choose \t" +
                        "Max Time to Choose \t" +
                        "MEEM-30\t" +
                        "ACER-100\t" + 
                        "Atençao-Orientacao-18\t" +
                        "Memoria-26\t" +
                        "Fluencia-14\t" +
                        "Linguagem-26\t" +
                        "Visual-espacial-16\t" +
                        Environment.NewLine
                        , Encoding.UTF8);

                    string names = "";
                    foreach (String[] plr in playerNamesACER)
                    {
                        if (names == "")
                            names = names + plr[0].Replace(" ", string.Empty);
                        else
                            names = names + "," + plr[0].Replace(" ", string.Empty);
                    }

                    if (!File.Exists(Application.dataPath + "/Choices/AllChoices.arff"))
                        File.WriteAllText(Application.dataPath + "/Choices/AllChoices.arff", 
                            "@RELATION AllChoices" + Environment.NewLine +
                            Environment.NewLine +
                            "@ATTRIBUTE Name {" + names + "}" + Environment.NewLine +
                            "@ATTRIBUTE Mode {0,1,2,3}" + Environment.NewLine +
                            "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                            "@ATTRIBUTE Match REAL" + Environment.NewLine +
                            "@ATTRIBUTE Value REAL" + Environment.NewLine +
                            "@ATTRIBUTE Suit REAL" + Environment.NewLine +
                            "@ATTRIBUTE Color REAL" + Environment.NewLine +
                            "@ATTRIBUTE TimeToPlay REAL" + Environment.NewLine +
                            "@ATTRIBUTE TimeToMemorize REAL" + Environment.NewLine +
                            "@ATTRIBUTE TimeToChoose REAL" + Environment.NewLine +
                            "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                            "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                            "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                            "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                            "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                            "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                            "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                            Environment.NewLine +
                            "@DATA" +
                            Environment.NewLine, Encoding.Default);

                    for (int iChoice = 0; iChoice < player.timeToDo.Count; iChoice++)
                    {
                        File.AppendAllText(Application.dataPath + "/Choices/AllChoices.arff", 
                            player.playerName.Replace(" ", string.Empty) + "," +
                            player.gameMode[iChoice] + "," +
                            player.nCards[iChoice] + "," +
                            Convert.ToSingle(player.match[iChoice]) + "," +
                            Convert.ToSingle(player.cardMatch[iChoice].value) + "," +
                            Convert.ToSingle(player.cardMatch[iChoice].suit) + "," +
                            Convert.ToSingle(player.cardMatch[iChoice].color) + "," +
                            player.timeToDo[iChoice].x + "," +
                            player.timeToDo[iChoice].y + "," +
                            player.timeToDo[iChoice].z + "," +
                            player.ACER[0] + "," +
                            player.ACER[1] + "," +
                            player.ACER[2] + "," +
                            player.ACER[3] + "," +
                            player.ACER[4] + "," +
                            player.ACER[5] + "," +
                            player.ACER[6] +
                            Environment.NewLine, Encoding.Default);

                        //OBS: Mudar player

                        if (!File.Exists(Application.dataPath + "/Choices/AllChoices - Mode " + player.gameMode[iChoice] +  ".arff"))
                            File.WriteAllText(Application.dataPath + "/Choices/AllChoices - Mode " + player.gameMode[iChoice] +  ".arff", 
                                "@RELATION AllChoices" + Environment.NewLine +
                                Environment.NewLine +
                                "@ATTRIBUTE Name {" + names + "}" + Environment.NewLine +
                                "@ATTRIBUTE Mode {0,1,2,3}" + Environment.NewLine +
                                "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                                "@ATTRIBUTE Match REAL" + Environment.NewLine +
                                "@ATTRIBUTE Value REAL" + Environment.NewLine +
                                "@ATTRIBUTE Suit REAL" + Environment.NewLine +
                                "@ATTRIBUTE Color REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE TimeToChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                                "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                                "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                                "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                                "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                                Environment.NewLine +
                                "@DATA" +
                                Environment.NewLine, Encoding.Default);

                        File.AppendAllText(Application.dataPath + "/Choices/AllChoices - Mode " + player.gameMode[iChoice] +  ".arff", 
                            player.playerName.Replace(" ", string.Empty) + "," +
                            player.gameMode[iChoice] + "," +
                            player.nCards[iChoice] + "," +
                            Convert.ToSingle(player.match[iChoice]) + "," +
                            Convert.ToSingle(player.cardMatch[iChoice].value) + "," +
                            Convert.ToSingle(player.cardMatch[iChoice].suit) + "," +
                            Convert.ToSingle(player.cardMatch[iChoice].color) + "," +
                            player.timeToDo[iChoice].x + "," +
                            player.timeToDo[iChoice].y + "," +
                            player.timeToDo[iChoice].z + "," +
                            player.ACER[0] + "," +
                            player.ACER[1] + "," +
                            player.ACER[2] + "," +
                            player.ACER[3] + "," +
                            player.ACER[4] + "," +
                            player.ACER[5] + "," +
                            player.ACER[6] +
                            Environment.NewLine, Encoding.Default);


                    }

                    for (int iMode = 0; iMode < 4; iMode++)
                    {
                        if (!File.Exists(Application.dataPath + "/Choices/AverageTimesWithReaction - Mode " + Mode(iMode) + ".txt"))
                            File.WriteAllText(Application.dataPath + "/Choices/AverageTimesWithReaction - Mode " + Mode(iMode) + ".txt", 
                                "ID \t" +
                                "Name \t" +
                                "Mode \t" +
                                "N Cards \t" +
                                "Win Rate \t" +
                                "Avg Reaction Time \t" +
                                "Dev Reaction Time \t" +
                                "Avg Motion Time \t"+
                                "Dev Motion Time \t" +
                                "Avg Time to Play \t" +
                                "Dev Time to Play \t" +
                                "Min Time to Play \t" +
                                "Max Time to Play \t" +
                                "Avg Time to Memorize \t" +
                                "Dev Time to Memorize \t" +
                                "Min Time to Memorize \t" +
                                "Max Time to Memorize \t" +
                                "Avg Time to Choose \t" +
                                "Dev Time to Choose \t" +
                                "Min Time to Choose \t" +
                                "Max Time to Choose \t" +
                                "GameFamiliarity\t" +
                                "CardFamiliarity\t" +
                                "MEEM-30\t" +
                                "ACER-100\t" + 
                                "Atençao-Orientacao-18\t" +
                                "Memoria-26\t" +
                                "Fluencia-14\t" +
                                "Linguagem-26\t" +
                                "Visual-espacial-16" +
                                Environment.NewLine
                                , Encoding.UTF8);

                        if (!File.Exists(Application.dataPath + "/Choices/AverageTimesWithReaction Mode - " + Mode(iMode) + ".arff"))
                            File.WriteAllText(Application.dataPath + "/Choices/AverageTimesWithReaction Mode - " + Mode(iMode) + ".arff", 
                                "@RELATION " + Mode(iMode) + Environment.NewLine +
                                Environment.NewLine +
                                "@ATTRIBUTE Name {" + names + "}" + Environment.NewLine +
                                "@ATTRIBUTE Mode REAL" + Environment.NewLine +
                                "@ATTRIBUTE NCards REAL" + Environment.NewLine +
                                "@ATTRIBUTE WinRate REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgReactionTime REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevReactionTime REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgMotionTime REAL"+ Environment.NewLine +
                                "@ATTRIBUTE DevMotionTime REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE MinTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE MaxTimetoPlay REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE MinTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE MaxTimetoMemorize REAL" + Environment.NewLine +
                                "@ATTRIBUTE AvgTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE DevTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE MinTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE MaxTimetoChoose REAL" + Environment.NewLine +
                                "@ATTRIBUTE GameFamiliarity REAL" + Environment.NewLine +
                                "@ATTRIBUTE CardFamiliarity REAL" + Environment.NewLine +
                                "@ATTRIBUTE meem-30 REAL" + Environment.NewLine +
                                "@ATTRIBUTE acer-100 REAL" + Environment.NewLine +
                                "@ATTRIBUTE atençaoeorientação-18 REAL" + Environment.NewLine +
                                "@ATTRIBUTE memória-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE fluência-14 REAL" + Environment.NewLine +
                                "@ATTRIBUTE linguagem-26 REAL" + Environment.NewLine +
                                "@ATTRIBUTE visual-espacial-16 REAL" + Environment.NewLine +
                                Environment.NewLine +
                                "@DATA" +
                                Environment.NewLine, Encoding.Default);



                        for (int nCards = 0; nCards < 6; nCards++)
                        {
                            int index = (iMode * 6 + nCards);
                            File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - AverageTimesWithReaction.txt", 
                                player.playerID + "\t" +
                                player.playerName + "\t" +
                                iMode + "\t" +
                                (nCards + 3) + "\t" +
                                winRate[index] + "\t" +
                                averageRection[index].x + "\t" +
                                devReaction[index].x + "\t" +
                                averageRection[index].y + "\t" +
                                devReaction[index].y + "\t" +
                                averageTimeToDo[index].x + "\t" +
                                devTimeToDo[index].x + "\t" +
                                player.minTime[index].x + "\t" +
                                player.maxTime[index].x + "\t" +
                                averageTimeToDo[index].y + "\t" +
                                devTimeToDo[index].y + "\t" +
                                player.minTime[index].y + "\t" +
                                player.maxTime[index].y + "\t" +
                                averageTimeToDo[index].z + "\t" +
                                devTimeToDo[index].z + "\t" +
                                player.minTime[index].z + "\t" +
                                player.maxTime[index].z + "\t" +
                                player.ACER[0] + "\t" +
                                player.ACER[1] + "\t" +
                                player.ACER[2] + "\t" +
                                player.ACER[3] + "\t" +
                                player.ACER[4] + "\t" +
                                player.ACER[5] + "\t" +
                                player.ACER[6] +
                                Environment.NewLine
                                , Encoding.UTF8);

                            File.AppendAllText(Application.dataPath + "/Choices/AverageTimesWithReaction.txt", 
                                player.playerID + "\t" +
                                player.playerName + "\t" +
                                iMode + "\t" +
                                (nCards + 3) + "\t" +
                                winRate[index] + "\t" +
                                averageRection[index].x + "\t" +
                                devReaction[index].x + "\t" +
                                averageRection[index].y + "\t" +
                                devReaction[index].y + "\t" +
                                averageTimeToDo[index].x + "\t" +
                                devTimeToDo[index].x + "\t" +
                                player.minTime[index].x + "\t" +
                                player.maxTime[index].x + "\t" +
                                averageTimeToDo[index].y + "\t" +
                                devTimeToDo[index].y + "\t" +
                                player.minTime[index].y + "\t" +
                                player.maxTime[index].y + "\t" +
                                averageTimeToDo[index].z + "\t" +
                                devTimeToDo[index].z + "\t" +
                                player.minTime[index].z + "\t" +
                                player.maxTime[index].z + "\t" +
                                player.ACER[0] + "\t" +
                                player.ACER[1] + "\t" +
                                player.ACER[2] + "\t" +
                                player.ACER[3] + "\t" +
                                player.ACER[4] + "\t" +
                                player.ACER[5] + "\t" +
                                player.ACER[6] +
                                Environment.NewLine
                                , Encoding.UTF8);

                            File.AppendAllText(Application.dataPath + "/Choices/AverageTimesWithReaction - Mode " + Mode(iMode) + ".txt", 
                                player.playerID + "\t" +
                                player.playerName + "\t" +
                                iMode + "\t" +
                                (nCards + 3) + "\t" +
                                winRate[index] + "\t" +
                                averageRection[index].x + "\t" +
                                devReaction[index].x + "\t" +
                                averageRection[index].y + "\t" +
                                devReaction[index].y + "\t" +
                                averageTimeToDo[index].x + "\t" +
                                devTimeToDo[index].x + "\t" +
                                player.minTime[index].x + "\t" +
                                player.maxTime[index].x + "\t" +
                                averageTimeToDo[index].y + "\t" +
                                devTimeToDo[index].y + "\t" +
                                player.minTime[index].y + "\t" +
                                player.maxTime[index].y + "\t" +
                                averageTimeToDo[index].z + "\t" +
                                devTimeToDo[index].z + "\t" +
                                player.minTime[index].z + "\t" +
                                player.maxTime[index].z + "\t" +
                                player.ACER[7] + "\t" +
                                player.ACER[8] + "\t" + 
                                player.ACER[0] + "\t" +
                                player.ACER[1] + "\t" +
                                player.ACER[2] + "\t" +
                                player.ACER[3] + "\t" +
                                player.ACER[4] + "\t" +
                                player.ACER[5] + "\t" +
                                player.ACER[6] +
                                Environment.NewLine
                                , Encoding.UTF8);

                            File.AppendAllText(Application.dataPath + "/Choices/AverageTimesWithReaction Mode - " + Mode(iMode) + ".arff", 
                                player.playerName.Replace(" ", string.Empty) + "," +
                                iMode + "," +
                                (nCards + 3) + "," +
                                winRate[index] + "," +
                                averageRection[index].x + "," +
                                devReaction[index].x + "," +
                                averageRection[index].y + "," +
                                devReaction[index].y + "," +
                                averageTimeToDo[index].x + "," +
                                devTimeToDo[index].x + "," +
                                player.minTime[index].x + "," +
                                player.maxTime[index].x + "," +
                                averageTimeToDo[index].y + "," +
                                devTimeToDo[index].y + "," +
                                player.minTime[index].y + "," +
                                player.maxTime[index].y + "," +
                                averageTimeToDo[index].z + "," +
                                devTimeToDo[index].z + "," +
                                player.minTime[index].z + "," +
                                player.maxTime[index].z + "," +
                                player.ACER[7] + "," +
                                player.ACER[8] + "," +
                                player.ACER[0] + "," +
                                player.ACER[1] + "," +
                                player.ACER[2] + "," +
                                player.ACER[3] + "," +
                                player.ACER[4] + "," +
                                player.ACER[5] + "," +
                                player.ACER[6] +
                                Environment.NewLine
                                , Encoding.Default);
                        }
                    }

                }
                break;
                #endregion
            #region Borderline
            case Modes.BorderLines:
                playerNames = new List<string>();
                choices = new List<Choice>();

                movementsText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
                foreach (string line in movementsText)
                {
                    playerNames.Add(line);
                }
                foreach (string player in playerNames)
                {
                    movementsText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);
                    choices.Add(new Choice(player));

                    for (int iMode = 2; iMode < movementsText.Length; iMode++)
                    {
                        int index0 = 0;
                        int index1 = 0;

                        string[] testes = movementsText[iMode].Split(tab);
                        if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt"))
                        {

                            string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Movements.txt", Encoding.UTF8);

                            //                            Debug.Log(player + " has " + choiceLines.Length + " movements");

                            movementTime = new float[choiceLines.Length - 1];
                            movementPos = new Vector2[choiceLines.Length - 1];
                            movementVel = new Vector2[choiceLines.Length - 1];

                            min = max = Vector2.zero;

                            for (int j = 1; j < choiceLines.Length; j++)
                            {
                                string[]choice = choiceLines[j].Split(tab);
                                movementTime[j - 1] = float.Parse(choice[0]);
                                Vector2 pos = new Vector2(float.Parse(choice[1]), float.Parse(choice[2]));

                                try 
                                {
                                    if (pos.x < min.x)
                                        min.x = pos.x;
                                    if (pos.y < min.y)
                                        min.y = pos.y;
                                    if (pos.x > max.x)
                                        max.x = pos.x;
                                    if (pos.y > max.y)
                                        max.y = pos.y;
                                }
                                catch (Exception e)
                                {
                                    Debug.Log(e);
                                }

                                movementPos[j - 1] = pos; // new Vector2(pos.magnitude, Mathf.Atan2(pos.y, pos.x));

                                if (j >= 2)
                                {
                                    if (movementPos[j - 2] == Vector2.zero)
                                    if (j == 2)
                                    {
                                        movementPos[j - 2] = movementPos[j - 1];
                                    }
                                    else
                                    {
                                        movementPos[j - 2] = (movementPos[j - 1] + movementPos[j - 3])/2f;
                                    }
                                }
                            }

                            center = (max + min) / 2f;
                            bases = (max - min) / 2f;

                            int angIndex = 36;

                            borderLine1 = new Vector2[angIndex];
                            for (int i = 0; i < borderLine1.Length; i++)
                                borderLine1[i] = center;
                            int slices = 10;

                            for (int i = 0; i < movementPos.Length; i++)
                            {
                                offset = movementPos[i] - center;

                                float ang = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
                                while (ang < 0f)
                                    ang += 360f;
                                int i_ang = Mathf.RoundToInt(ang/slices);
                                if (i_ang >= Mathf.RoundToInt(360/slices))
                                    i_ang -= Mathf.RoundToInt(360/slices);
                                if (i_ang < 0)
                                    i_ang += Mathf.RoundToInt(360/slices);

                                polar = new Vector2(offset.magnitude, ang);
                                if (polar.x > borderLine1[i_ang].x)
                                    borderLine1[i_ang] = polar;
                            }

                            for (int i = 0; i < borderLine1.Length; i++)
                            {
                                choices[choices.Count - 1].package[i] = borderLine1[i].x;
                                choices[choices.Count - 1].elipse[i] = (new Vector2(bases.x * Mathf.Cos(10f* i * Mathf.Deg2Rad), bases.y * Mathf.Sin(10f * i * Mathf.Deg2Rad))).magnitude;
                            }

                            choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt", Encoding.UTF8);

                            //                            Debug.Log(player + " has " + choiceLines.Length + " choices");

                            float startTime = 0f;
                            float endTime = 0f;

                            for (int iChoice = 2; iChoice < choiceLines.Length; iChoice++)
                            {
                                string[]choice = choiceLines[iChoice].Split(tab);
                                int lastPlayer = choices.Count - 1;
                                choices[lastPlayer].Add(choice);
                            }
                            //    Debug.Log(player + " - endTime = " + endTime.ToString() + " | choice = " + (choices[choices.Count - 1].nChoice.Count - 1).ToString());
                        }
                        else
                        {
                            Debug.Log(player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                        }
                    }
                }

                    File.WriteAllText(Application.dataPath + "/Choices/" + playerName + " - BorderLine.txt", 
                        "Border Calculated" + "\t" +
                        Environment.NewLine +
                        "X min" + "\t" +
                        "Y min" + "\t" +
                        "X max" + "\t" +
                        "Y max" + "\t" +
                        Environment.NewLine +
                        min.x + "\t" +
                        min.y + "\t" +
                        max.x + "\t" +
                        max.y + "\t" +
                        Environment.NewLine +
                        "X center" + "\t" +
                        "Y center" + "\t" +
                        "X bases" + "\t" +
                        "Y bases" + "\t" +
                        Environment.NewLine +
                        center.x + "\t" +
                        center.y + "\t" +
                        bases.x + "\t" +
                        bases.y + "\t" +
                        Environment.NewLine +
                        Environment.NewLine
                        , Encoding.UTF8);

                foreach (Choice player in choices)
                {
                    for (int i = 0; i < player.package.Length; i++)
                    {
                        File.AppendAllText(Application.dataPath + "/Choices/" + playerName + " - BorderLine.txt", 
                            i + "\t" +
                            player.package[i] + "\t" +
                            player.elipse[i] + "\t" +
                            Environment.NewLine
                            , Encoding.UTF8);
                    }
                }

                lineText = "\t";

                for (int i = 0; i < choices.Count; i++)
                {
                    choices[i].RadialProfile();
                    lineText = lineText + choices[i].playerName + "\t\t\t\t";
                }

                File.WriteAllText(Application.dataPath + "/Choices/RadialAverageTime.txt", 
                    lineText
                    + Environment.NewLine, Encoding.UTF8);

                File.WriteAllText(Application.dataPath + "/Choices/RadialMistakes.txt", 
                    lineText
                    + Environment.NewLine, Encoding.UTF8);

                for (int i = 0; i < 36; i++)
                {
                    lineText = (10f*i).ToString() + "\t";
                    for (int j = 0; j < choices.Count; j++)
                    {
                        lineText = lineText + choices[j].package[i] + "\t";
                        lineText = lineText + choices[j].elipse[i] + "\t";
                        lineText = lineText + (choices[j].package[i] / choices[j].elipse[i]) + "\t";
                        lineText = lineText + choices[j].averageTime[i] + "\t";
                    }
                    File.AppendAllText(Application.dataPath + "/Choices/RadialAverageTime.txt", 
                        lineText +
                        Environment.NewLine
                        , Encoding.UTF8);

                    lineText = (10f*i).ToString() + "\t";
                    for (int j = 0; j < choices.Count; j++)
                    {
                        lineText = lineText + choices[j].package[i] + "\t";
                        lineText = lineText + choices[j].elipse[i] + "\t";
                        lineText = lineText + (choices[j].package[i] / choices[j].elipse[i]) + "\t";
                        lineText = lineText + choices[j].nMistake[i] + "\t";
                    }
                    File.AppendAllText(Application.dataPath + "/Choices/RadialMistakes.txt", 
                        lineText +
                        Environment.NewLine
                        , Encoding.UTF8);
                }
                break;
            #endregion
            case Modes.None:
                break;
        }
	}


    public string Mode(int nMode)
    {
        switch (nMode)
        {
            case 0:
                return "Basic";
            case 1:
                return "Memory";
            case 2:
                return "MultSuit";
            case 3:
                return "CountSuit";
            default:
                return "NoMode";
        }
    }
}
