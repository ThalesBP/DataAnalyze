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
}