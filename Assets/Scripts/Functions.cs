using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;

public class Functions : MonoBehaviour {


    public void PersonAnalyze(string playerName)
    {
        // Player's movements variables
        Vector2[] movements;
        Vector2[] borderLine;
        char tab = Convert.ToChar("\t");
        
        // Movement space variables (ellipse parameters)
        Vector2 min, max;
        Vector2 center, bases;
        Vector2 polar, offset;

        // Initialize borderline variable with a defined resolution
        borderLine = new Vector2[36];

        
        min = max = center = bases = Vector2.zero;

        // Writes a abstract about player's movement [OBS: MOVE CODE TO THE END?]
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

        // Reads player's historic game
        string[] matchesText = File.ReadAllLines(Application.dataPath + "/Choices/" + playerName + "/" + playerName + " - Historic.txt", Encoding.UTF8);
        Debug.Log("Historic Size: " + matchesText.Length);

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

        // Reads all player's movements from file just written
        string[] movementsText = File.ReadAllLines(Application.dataPath + "/" + playerName + " - Movements.txt", Encoding.UTF8);

        // Initialize movement vector with file size (less header)
        movements = new Vector2[movementsText.Length - 1];

        // Reset movement space variables
        min = max = center = bases = Vector2.zero;
        
        // For each player's movement (less header)
        for (int i = 1; i < movementsText.Length; i++)
        {
            try
            {
                // Break string into fields (0 - Time, 1 - X Pos, 2 - Y Pos, etc...)
                // See dissertation's subseccion 5.2.2 for movement space study
                string[] columns = movementsText[i].Split(tab);

                // Transform movement into vector (converted to degrees)
                movements[i - 1] = (new Vector2(float.Parse(columns[1]), float.Parse(columns[2]))) * Mathf.Rad2Deg;

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
        // movementsText = new string[0]; // OBS: Verify if it is necessary

        // Parametrize ellipse (Function 5.7 in dissertation)
        center = (max + min) / 2f;
        bases = (max - min) / 2f;

        // Centralize all borderline at ellipse center
        for (int i = 0; i < borderLine.Length; i++)
            borderLine[i] = center;

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
            int i_ang = Mathf.RoundToInt(ang / 10f);

            // Adjust index between 0 (included) and 36 (excluded) degrees 
            if (i_ang >= 36)
                i_ang -= 36;
            if (i_ang < 0)
                i_ang += 36;

            // Defines offset polar coordinate r is greater than current borderline
            polar = new Vector2(offset.magnitude, ang);

            // Verifies if 
            if (polar.x > borderLine[i_ang].x)
                borderLine[i_ang] = polar;
        }

        // movements = new Vector2[0]; // OBS: Verify if is necessary

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
}