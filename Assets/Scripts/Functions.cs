using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

public class Functions : MonoBehaviour {

    readonly char tab = Convert.ToChar("\t");
    readonly int borderlineResolution = 36;

    public void PersonAnalyze(string playerName)
    {
        // Player's movements variables
        Vector2[] movements;
        Vector2[] borderLine;
        
        // Movement space variables (ellipse parameters)
        Vector2 min, max;

        // Merges all player's movement
        MergeMovements(playerName);

        // Reads all player's movement into array of Vector2
        movements = ReadMovements(playerName, out min, out max);

        // Find borderline of all movements
        borderLine = DefineBorderline(movements, min, max, borderlineResolution);

        // Creates Borderline File for the player
        CreateBorderlineFile(playerName, borderLine, min, max);
    }

    public void RadialAnalyze()
    {
        List<string> playerNames = new List<string>();
        List<Choice> choices = new List<Choice>();

        // Reads all player's name in text file
        string[] playersNamesText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
        foreach (string line in playersNamesText)
        {
            playerNames.Add(line);
        }
        // Destroy / Clear playerNamesText?
        // Array.Clear(playersNamesText, 0, 0);

        foreach (string player in playerNames)
        {
            // Reads player's historic in text file
            string[] historicText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);

            // Initialize player's choices
            choices.Add(new Choice(player));

            // For each historic does...
            for (int i = 2; i < historicText.Length; i++)
            {
                // Split historic line into columns
                string[] historicColumns = historicText[i].Split(tab);

                // Checks if choices file for this historic line exists
                if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt"))
                {
                    // Case file exists...

                    // Reads all choices
                    string[] choicesText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt", Encoding.UTF8);

                    // For each choice does...
                    for (int j = 2; j < choicesText.Length; j++)
                    {
                        // Breaks choice into columns
                        string[] choice = choicesText[j].Split(tab);

                        // Add choice to player's choices
                        choices[choices.Count - 1].Add(choice);
                    }
                }
                else
                {
                    Debug.Log(player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt was not found");
                }
            }
        }

        // For each player's choices defines borderline, package and writes radial profiles
        foreach (Choice player in choices)
        {
            Vector2[] movements;
            Vector2[] borderLine;

            Vector2 min, max, center, bases;

            movements = ReadMovements(player.playerName, out min, out max);
            
            borderLine = DefineBorderline(movements, min, max, borderlineResolution);

            center = (max + min) / 2f;
            bases = (max - min) / 2f;

            for (int i = 0; i < borderLine.Length; i++)
            {
                player.package[i] = borderLine[i].x;
                player.elipse[i] = (new Vector2(bases.x * Mathf.Cos(10f * i * Mathf.Deg2Rad), bases.y * Mathf.Sin(10f * i * Mathf.Deg2Rad))).magnitude;
            }
            player.RadialProfile();
        }

        WriteRadialProfiles(choices);
    }

    /// <summary>
    /// Merges all choices into a text file
    /// </summary>
    void MergeChoices()
    {
        List<string> playerNames = new List<string>();
        List<Choice> choices = new List<Choice>();

        string[] playerNamesText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
        foreach (string line in playerNamesText)
        {
            playerNames.Add(line);
        }
        foreach (string player in playerNames)
        {
            string[] historicText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);

            for (int i = 2; i < historicText.Length; i++)
            {
                string[] historicColumns = historicText[i].Split(tab);
                if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt"))
                {
                    string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt", Encoding.UTF8);
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
                    Debug.Log(player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt was not found");
                }
            }
        }
    }

    void ChoicePath()
    {
        Vector2 min, max;
        List<string> playerNames = new List<string>();
        List<Choice> choices = new List<Choice>();

        // Reads all players' name and adds to player names list
        string[] playerNamesText = File.ReadAllLines(Application.dataPath + "/Choices/Players.txt", Encoding.UTF8);
        foreach (string line in playerNamesText)
        {
            playerNames.Add(line);
        }

        // For each player
        foreach (string player in playerNames)
        {
            // Reads all player's historic
            string[] historicLinesText = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - Historic.txt", Encoding.UTF8);
            choices.Add(new Choice(player));

            // Each historic line is supposed to be a game mode (OBS: May/must change in future) 
            // Skipping header (starting from 2)
            for (int iMode = 2; iMode < historicLinesText.Length; iMode++)
            {
                // Aux indexers for start and final indexes
                int index0 = 0;
                int index1 = 0;

                // Reads historic columns of current historic line
                string[] historicColumns = historicLinesText[iMode].Split(tab);
                if (File.Exists(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt"))
                {
                    // Reads all player's movement from text file
                    string[] movementsLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Movements.txt", Encoding.UTF8);

                    // Inicialize aux variables with number of movements (skipping header, so length less one)
                    float[] movementTime = new float[movementsLines.Length - 1];
                    Vector2[] movementPos = new Vector2[movementsLines.Length - 1];
                    ////////////////////Vector2[] movementVel = new Vector2[movementsLines.Length - 1];

                    min = max = Vector2.zero;

                    // For each movement line, fill up aux variables
                    for (int j = 1; j < movementsLines.Length; j++)
                    {
                        string[] movementsColumns = movementsLines[j].Split(tab);
                        movementTime[j - 1] = float.Parse(movementsColumns[0]);
                        Vector2 pos = new Vector2(float.Parse(movementsColumns[1]), float.Parse(movementsColumns[2]));

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

                        // Fixes bug acquisition that saves zero position sometimes by average interpolation (Obs: this bug must be fixed in the future)
                        if (j >= 2)
                        {
                            if (movementPos[j - 2] == Vector2.zero)
                                if (j == 2)
                                {
                                    movementPos[j - 2] = movementPos[j - 1];
                                }
                                else
                                {
                                    movementPos[j - 2] = (movementPos[j - 1] + movementPos[j - 3]) / 2f;
                                }
                        }
                    }

                    //center = (max + min) / 2f;

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

                    string[] choiceLines = File.ReadAllLines(Application.dataPath + "/Choices/" + player + "/" + player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt", Encoding.UTF8);
                    
                    // Aux time variables
                    float startTime = 0f;
                    float endTime = 0f;

                    // For each choice line (skipping header, so index starts at 2)
                    for (int iChoice = 2; iChoice < choiceLines.Length; iChoice++)
                    {
                        string[] choiceColumns = choiceLines[iChoice].Split(tab);

                        // Last player in choices is the current one
                        int lastPlayer = choices.Count - 1;

                        // Adds current choice to player's choices
                        choices[lastPlayer].Add(choiceColumns);

                        // Saves last choice's index into a aux variable
                        int lastChoice = choices[lastPlayer].nChoice.Count - 1;

                        // Defines end and start times based on player's choice infos
                        endTime = choices[lastPlayer].time[lastChoice]; //  + 2.5f / choices[lastPlayer].speed[lastChoice];
                                                                        /*if (!choices[lastPlayer].match[lastChoice])
                                                                            endTime += 1.5f / choices[lastPlayer].speed[lastChoice];
                                                                        endTime += 0.75f / choices[lastPlayer].speed[lastChoice];
                                                                        endTime += (1.5f + (choices[lastPlayer].nCards[lastChoice] - 1) * 0.25f) / choices[lastPlayer].speed[lastChoice];
                                        */
                                                                        //endTime -=  1.25f / choices[lastPlayer].speed[lastChoice];
                        startTime = endTime - (1.25f / choices[lastPlayer].speed[lastChoice] + choices[lastPlayer].timeToDo[lastChoice].z);

                        //string linePath = "";
                        //string lineAngle = "";
                        //int index0_aux = index0;

                        // Finds movement's start and end indexes based on times
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

                        // Verifies if there is a interval
                        if (index0 != index1)
                        {
                            // Adds movement's interval
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
                    Debug.Log(player + " - " + historicColumns[2] + " - " + historicColumns[1] + " - " + historicColumns[0] + " - Choices.txt was not found");
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
                                float angAux = posAux * Mathf.Sin(Mathf.Deg2Rad * Mathf.DeltaAngle(player.movementPos[nChoice][timeStepAux].y, player.movementPos[nChoice][player.motion[nChoice]].y));// / 180f); //player.motion[nChoice] | player.movementPos[nChoice].Length - 1

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
            for (int iTime = 0; iTime <= Choice.elemNorm; iTime++)
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
    }

    /// <summary>
    /// Merges all player's movement into a Movements.txt file
    /// </summary>
    /// <param name="playerName">Player's name</param>
    void MergeMovements(string playerName)
    {
        // Reads player's historic game
        string[] matchesText = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - Historic.txt", Encoding.UTF8);
        Debug.Log("Historic Size: " + matchesText.Length);

        // Reset movement file
        File.WriteAllText(Application.dataPath + "/" + playerName + " - Movements.txt",
            "",
            Encoding.UTF8);

        // For each match does...
        for (int i = 2; i < matchesText.Length; i++)
        {
            // Auxiliar variable to deal with ONE match
            string[] matchData = matchesText[i].Split(tab);

            // Verifies if the movement file exists
            if (File.Exists(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - " + matchData[2] + " - " + matchData[1] + " - " + matchData[0] + " - Movements.txt"))
            {
                // Case movement file exists for this match

                // Reads player's movements for this match
                string[] movementLines = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - " + matchData[2] + " - " + matchData[1] + " - " + matchData[0] + " - Movements.txt", Encoding.UTF8);

                // For each movement... (index starts at 2 to jump the header)
                for (int j = 2; j < movementLines.Length; j++)
                {
                    // Merges all movement in a single movement file for this player
                    File.AppendAllText(Application.dataPath + "/" + playerName + " - Movements.txt",
                        movementLines[j] +
                        Environment.NewLine, Encoding.UTF8);
                }
            }
            else
            {
                // Case movement file does NOT exist for this match
                // Reports it
                Debug.Log(playerName + " - " + matchData[2] + " - " + matchData[1] + " - " + matchData[0] + " - Choices.txt was not found");
            }
        }
    }

    /// <summary>
    /// Reads player's Movement.txt file and convert into array of Vector2
    /// </summary>
    /// <param name="playerName">Player's name</param>
    /// <param name="min">Min movements</param>
    /// <param name="max">Max movements</param>
    /// <returns></returns>
    Vector2[] ReadMovements(string playerName, out Vector2 min, out Vector2 max)
    {
        // Reads all player's movements from file just written
        string[] movementsText = File.ReadAllLines(Application.dataPath + "/" + playerName + " - Movements.txt", Encoding.UTF8);

        // Initialize movement vector with file size (less header)
        Vector2[] movements = new Vector2[movementsText.Length - 1];

        // Reset movement space variables
        min = max = Vector2.zero;

        // For each player's movement (less header)
        for (int i = 1; i < movementsText.Length; i++)
        {
            try
            {
                // Break string into fields (0 - Time, 1 - X Pos, 2 - Y Pos, etc...)
                // See dissertation's subseccion 5.2.2 for movement space study
                string[] movementsColumns = movementsText[i].Split(tab);

                // Transform movement into vector (converted to degrees)
                movements[i - 1] = (new Vector2(float.Parse(movementsColumns[1]), float.Parse(movementsColumns[2]))) * Mathf.Rad2Deg;

                // Verifies max and min movements to parametrize ellipse
                // Used in function 5.7 (dissertation)
                if (movements[i - 1].x < min.x)
                    min.x = movements[i - 1].x;
                if (movements[i - 1].y < min.y)
                    min.y = movements[i - 1].y;
                if (movements[i - 1].x > max.x)
                    max.x = movements[i - 1].x;
                if (movements[i - 1].y > max.y)
                    max.y = movements[i - 1].y;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        return movements;
    }

    /// <summary>
    /// Defines Borderline based in all movement and min and max of them
    /// </summary>
    /// <param name="movements">All movements</param>
    /// <param name="min">Min of movements</param>
    /// <param name="max">Max of movements</param>
    /// <param name="resolution">Resolution of final borderline</param>
    /// <returns></returns>
    Vector2[] DefineBorderline(Vector2[] movements, Vector2 min, Vector2 max, int resolution)
    {
        // Parametrize ellipse (Function 5.7 in dissertation)
        Vector2 polar, offset;
        Vector2 center = (max + min) / 2f;
        Vector2 bases = (max - min) / 2f;
        Vector2[] borderLine = new Vector2[resolution];

        // Centralize all borderline at ellipse center
        for (int i = 0; i < borderLine.Length; i++)
            borderLine[i] = center;

        // For each movement vector...
        for (int i = 0; i < movements.Length; i++)
        {
            // Calculates offset movement from ellipse center
            offset = movements[i] - center;

            // Calculates polar coordinate alpha of this offset
            float ang = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

            // Adjust angle between 0 (included) and 360 (excluded) degrees 
            while (ang < 0f)
                ang += 360f;

            // Rounds polar coordinate alpha to borderline resolution (36 elements)
            int i_ang = Mathf.RoundToInt(ang / (360f / (float)resolution));

            // Adjust index between 0 (included) and 36 (excluded) degrees 
            if (i_ang >= resolution)
                i_ang -= resolution;
            if (i_ang < 0)
                i_ang += resolution;

            // Defines offset polar coordinate r is greater than current borderline
            polar = new Vector2(offset.magnitude, ang);

            // Verifies if 
            if (polar.x > borderLine[i_ang].x)
                borderLine[i_ang] = polar;
        }

        return borderLine;
    }

    /// <summary>
    /// Creates player's borderline text file
    /// </summary>
    /// <param name="playerName">Player for having borderline file created</param>
    /// <param name="borderLine">Borderline array</param>
    /// <param name="min">Min movements</param>
    /// <param name="max">Max movements</param>
    void CreateBorderlineFile(string playerName, Vector2[] borderLine, Vector2 min, Vector2 max)
    {
        Vector2 center = (max + min) / 2f;
        Vector2 bases = (max - min) / 2f;

        // Writes borderline header (OBS: Improve writers)
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

        // For each borderline angle (resolution)...
        for (int i = 0; i < borderLine.Length; i++)
        {
            // Writes in borderline file
            File.AppendAllText(Application.dataPath + "/" + playerName + " - BorderLine.txt",
                    i + "\t" +
                    borderLine[i].x + "\t" +
                    borderLine[i].y + "\t" +
                    (borderLine[i].x * Mathf.Cos(borderLine[i].y * Mathf.Deg2Rad) + center.x) + "\t" +
                    (borderLine[i].x * Mathf.Sin(borderLine[i].y * Mathf.Deg2Rad) + center.y) + "\t" +
                    (bases.x * Mathf.Cos(10f * i * Mathf.Deg2Rad) + center.x) + "\t" +
                    (bases.y * Mathf.Sin(10f * i * Mathf.Deg2Rad) + center.y) + "\t" +
                    Environment.NewLine
                , Encoding.UTF8);
        }
    }

    /// <summary>
    /// Writes text files of radial profiles for Average Time and Mistakes
    /// </summary>
    /// <param name="choices">List of choices (from players)</param>
    void WriteRadialProfiles(List<Choice> choices)
    {
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
            lineText = (10f * i).ToString() + "\t";
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

            lineText = (10f * i).ToString() + "\t";
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
    }
}