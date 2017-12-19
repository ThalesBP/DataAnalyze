using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

public class Analyze : MonoBehaviour {

    public enum Modes {Person, Package, RadialChoices, MergeChoices, ChoicePath};
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

                borderLine1 = new Vector2[360];

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
                    int i_ang = Mathf.RoundToInt(ang);
                    if (i_ang >= 360)
                        i_ang -= 360;
                    if (i_ang < 0)
                        i_ang += 360;

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
                        (bases.x * Mathf.Cos(i * Mathf.Deg2Rad) + center.x) + "\t" +
                        (bases.y * Mathf.Sin(i * Mathf.Deg2Rad) + center.y) + "\t" +
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
                                        "Name \t" + choiceLines[1] + Environment.NewLine
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
                            Debug.Log(player + " - " + testes[2] + " - " + testes[1] + " - " + testes[0] + " - Choices.txt was not found");
                        }
                    }
                }

                bool running = true;
                string currentLine = "";
                int timeStepAux = 0;

                foreach (Choice player in choices)
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePaths.txt", 
                        ""
                        , Encoding.UTF8);
                    
                while (running)
                {
                    running = false;
                    foreach (Choice player in choices)
                    {
                        currentLine = timeStepAux.ToString() + "\t";

                        for (int nChoice = 0; nChoice < player.movementPos.Count; nChoice++)
//                        foreach (Vector2[] choicePath in player.movementPos)
                        {
                            if (timeStepAux < player.movementPos[nChoice].Length)
                            {
                                running = true;
                                currentLine = currentLine + (player.movementPos[nChoice][timeStepAux].x) + "\t";
                            }
                            else
                                currentLine = currentLine + "\t";
                        }

                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePaths.txt", 
                            currentLine +
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
                        player.Add(reac, mot, chos);
                        if ((reac < 0) || (mot < 0))
                            Debug.Log(player.playerName + " - Reaction " + reac + " - Motion " + mot);
                    }
                }

                foreach (Choice player in choices)
                    File.WriteAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePathsNormalized.txt", 
                        ""
                        , Encoding.UTF8);

                running = true;
                currentLine = "";
                timeStepAux = 0;

                while (running)
                {
                    running = false;
                    foreach (Choice player in choices)
                    {
                        currentLine = timeStepAux.ToString() + "\t";

                        for (int nChoice = 0; nChoice < player.movementPos.Count; nChoice++)
                            //                        foreach (Vector2[] choicePath in player.movementPos)
                        {
                            if (timeStepAux < player.movementPos[nChoice].Length)
                            {
                                running = true;
                                try {
                                    currentLine = currentLine + ((player.movementTime[nChoice][timeStepAux] - player.movementTime[nChoice][player.reaction[nChoice]]) / (player.movementTime[nChoice][player.motion[nChoice]] - player.movementTime[nChoice][player.reaction[nChoice]])) + "\t";
                                    currentLine = currentLine + ((player.movementPos[nChoice][timeStepAux].x - player.movementPos[nChoice][0].x) / (player.movementPos[nChoice][player.movementPos[nChoice].Length - 1].x - player.movementPos[nChoice][0].x)) + "\t";
                                }
                                catch
                                {
                                    Debug.Log("Player Movement Time Count = " + player.movementTime.Count + " > NChoice = " + nChoice);
                                    Debug.Log("time[nChoice] Lengh = " + player.movementTime[nChoice].Length + " > TimeStepAux = " + timeStepAux + " and Reaction = " + player.reaction[nChoice] + " and Motion =" + player.motion[nChoice]);
                                    Debug.Log("nChoice = " + nChoice + " < Reaction.Count = " + player.reaction.Count + " and Motion.Count = " + player.motion.Count);
                                }
                            }
                            else
                                currentLine = currentLine + "\t\t";
                        }

                        File.AppendAllText(Application.dataPath + "/Choices/" + player.playerName + " - ChoicePathsNormalized.txt", 
                            currentLine +
                            Environment.NewLine
                            , Encoding.UTF8);
                    }
                    timeStepAux++;
                }
                break;
            #endregion
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
