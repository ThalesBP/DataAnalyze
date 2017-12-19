using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice {

    public string playerName;
    public int[] nMistake;
    public float[] package, elipse, averageTime;
    public List<int> nChoice, nCards, gameMode;
    public List<float> time, speed, turnTime, choiceChanged;
    public List<bool> match;
    public List<Vector2> choicePos, objectivePos;
    /// <summary>
    /// The time to do.
    /// x = play; y = memorize; z = choose
    /// </summary>
    public List<Vector3> timeToDo;
    public List<Card> choice, objective, cardMatch;

    public List<float[]> movementTime;
    public List<Vector2[]> movementPos;
    public List<int> reaction, motion, chosen;

    public int elemNorm = 20;
    public float[] moveTimeNorm;
    public int[][] moveCountNorm;
    public Vector2[][] movePosNorm;

    public int maxMove;
    public float maxStartMag, minEndMag;

    public Choice (string name)
    {
        playerName = name;
        nChoice = new List<int>();
        nCards = new List<int>();
        gameMode = new List<int>();
        time = new List<float>();
        speed = new List<float>();
        turnTime = new List<float>();
        choiceChanged = new List<float>();
        match = new List<bool>();
        choicePos = new List<Vector2>();
        objectivePos = new List<Vector2>();
        timeToDo = new List<Vector3>();
        choice = new List<Card>();
        objective = new List<Card>();
        cardMatch = new List<Card>();
        package = new float[36];
        elipse = new float[36];
        averageTime = new float[36];
        nMistake = new int[36];

        movementTime = new List<float[]>();
        movementPos = new List<Vector2[]>();
        reaction = new List<int>();
        motion = new List<int>();
        chosen = new List<int>();

        maxStartMag = 0f;
        minEndMag = float.PositiveInfinity;

        moveTimeNorm = new float[elemNorm + 1];
        for (int i = 0; i <= elemNorm; i++)
        {
            moveTimeNorm[i] = (float)i / (float)elemNorm;
        }
    }

    public void Add(string[] line)
    {
        nChoice.Add(int.Parse(line[0]));
        time.Add(float.Parse(line[1]));
        switch (line[2])
        {
            case "Basic":
                gameMode.Add(0);
                break;
            case "Memory":
                gameMode.Add(1);
                break;
            case "MultiSuits":
                gameMode.Add(2);
                break;
            case "CountSuits":
                gameMode.Add(3);
                break;
            default:
                gameMode.Add(0);
                Debug.Log("Game mode error - " + line[2] + " - " + playerName);
                break;
        }
        speed.Add(float.Parse(line[3]));
        nCards.Add(int.Parse(line[4]));
        if (int.Parse(line[5]) == 1)
            match.Add(true);
        else
            match.Add(false);
        turnTime.Add(float.Parse(line[6]));

        choiceChanged.Add(float.Parse(line[8]));
        objective.Add(new Card(line[9], line[10], line[11]));
        choice.Add(new Card(line[12], line[13], line[14]));
        cardMatch.Add(new Card(line[15], line[16], line[17]));
        timeToDo.Add(new Vector3(float.Parse(line[19]), float.Parse(line[20]), float.Parse(line[21])));
        objectivePos.Add(new Vector2(float.Parse(line[22]), float.Parse(line[23])));
        choicePos.Add(new Vector2(float.Parse(line[24]), float.Parse(line[25])));
    }

    public void Add(float[] times, Vector2[] movements, int index0, int index1)
    {
        if (index1 - index0 > maxMove)
            maxMove = index1 - index0;

        float[] timesAux = new float[index1 - index0];
        Vector2[] movementsAux = new Vector2[index1 - index0];

        for (int i = 0; i < timesAux.Length; i++)
        {
            timesAux[i] = times[index0 + i];

            Vector2 pos;
            pos = movements[index0 + i] - movements[index0];
            movementsAux[i] = new Vector2(pos.magnitude, Mathf.Atan2(pos.y, pos.x));
        }
        movementTime.Add(timesAux);
        movementPos.Add(movementsAux);
    }

    /// <summary>
    /// Add the specified reac, mot and chos.
    /// </summary>
    /// <param name="reac">Reaction.</param>
    /// <param name="mot">Motion.</param>
    /// <param name="chos">Chosen.</param>
    public void Add(int reac, int mot, int chos)
    {
        if (reac < 0)
            reaction.Add(0);
        else
            reaction.Add(reac);

        if (reac >= mot)
            motion.Add(reac);
        else
            motion.Add(mot);
    
        chosen.Add(chos);
    }

    public void StartNorm()
    {
        movePosNorm = new Vector2[nChoice.Count][];
        moveCountNorm = new int[nChoice.Count][];
        for (int i = 0; i < nChoice.Count; i++)
        {
            movePosNorm[i] = new Vector2[elemNorm + 1];
            moveCountNorm[i] = new int[elemNorm + 1];
            for (int j = 0; j <= elemNorm; j++)
            {
                movePosNorm[i][j] = Vector2.zero;
                moveCountNorm[i][j] = 0;
            }
        }
    }

    public void AddNorm(float timeNorm, Vector2 posNorm, int iChoice)
    {
        int index = Mathf.RoundToInt(timeNorm * elemNorm);

        if ((index >= 0) && (index <= elemNorm))
        {
            movePosNorm[iChoice][index] = (movePosNorm[iChoice][index] * (float)moveCountNorm[iChoice][index] + posNorm) / (float)(moveCountNorm[iChoice][index] + 1);
            moveCountNorm[iChoice][index]++;
        }
    }

    public void FitNorm()
    {
        for (int i = 0; i < nChoice.Count; i++)
        {
            Vector2 elemBefore = Vector2.zero;
            for (int j = 0; j <= elemNorm; j++)
            {
                int indexAux = 0;
                while (moveCountNorm[i][j + indexAux] == 0)
                {
                    if (j + indexAux + 1 == moveCountNorm[i].Length)
                        break;
                    
                    indexAux++;
                }
                for (int k = 0; k < indexAux; k++)
                {
                    movePosNorm[i][j] = elemBefore + (float) (k + 1) * (movePosNorm[i][j + indexAux] - elemBefore) / (float)(indexAux + 1);
                }
                elemBefore = movePosNorm[i][j];
            }
        }
    }

    /// <summary>
    /// Average the specified mode.
    /// </summary>
    /// <param name="mode">Mode.</param>
    public void RadialProfile()
    {
        float[] angs = new float[36];
        int[] total = new int[36];

        for (int i = 0; i < angs.Length; i++)
        {
            angs[i] = 0f;
            total[i] = 0;
        }
        for (int i = 0; i < objectivePos.Count; i++)
        {
            float ang = objectivePos[i].y;
            if (ang < 0f)
                ang += 360f;
            if (ang >= 360f)
                ang -= 360f;

            int i_ang = Mathf.CeilToInt(objectivePos[i].y / 10f);
            if (i_ang >= 36)
                i_ang -= 36;
            if (i_ang < 0)
                i_ang += 36;

            if (match[i])
            {
                angs[i_ang] += timeToDo[i].z;
                total[i_ang]++;
            }
            else
            {
                nMistake[i_ang]++;
            }
        }


        for (int i = 0; i < angs.Length; i++)
        {
            if (total[i] != 0)
                averageTime[i] = angs[i] / total[i];
            else
            {
                switch(i)
                {
                    case 0:
                        averageTime[i] = ((angs[1] / total[1]) + (angs[35]  / total[35]))/2f;
                        break;
                    case 35:
                        averageTime[i] = ((angs[0] / total[0]) + (angs[34] / total[34]))/ 2f;
                        break;
                    default:
                        averageTime[i] = ((angs[i-1] / total[i-1]) + (angs[i+1] / total[i+1])) / 2f;
                        break;
                }
            }
        }
    }

    public Vector3[] Average(int elements)
    {
        int [] elemPath = new int[elements + 1];
        float[] timePath = new float[elements + 1];
        float[] avgPath = new float[elements + 1];
        float[] devPath = new float[elements + 1];

        for (int elem = 0; elem <= elements; elem++)
        {
            elemPath[elem] = 0;
            timePath[elem] = (float)elem / (float)elements;
            avgPath[elem] = 0f;
            devPath[elem] = 0f;
        }

        for (int nChoice = 0; nChoice < movementTime.Count; nChoice++)
        {
            for (int nMovement = 0; nMovement < movementTime[nChoice].Length; nMovement++)
            {
                int index = Mathf.RoundToInt(movementTime[nChoice][nMovement] * (float)elements);

                if ((index >= 0) && (index <= elements))
                {
                    elemPath[index]++;
                    avgPath[index] += movementPos[nChoice][nMovement].x;
                }
            }
        }

        for (int nChoice = 0; nChoice < movementTime.Count; nChoice++)
        {
            for (int nMovement = 0; nMovement < movementTime[nChoice].Length; nMovement++)
            {
                int index = Mathf.RoundToInt(movementTime[nChoice][nMovement] * (float)elements);

                if ((index >= 0) && (index <= elements))
                {
                    avgPath[index] = avgPath[index] / (float)elemPath[index];
                    devPath[index] += Mathf.Pow((avgPath[index] - movementPos[nChoice][nMovement].x), 2f) / (float)elemPath[index];
                }
            }
        }

        Vector3[] aux = new Vector3[elements + 1];

        for (int elem = 0; elem <= elements; elem++)
        {
            devPath[elem] = Mathf.Sqrt(devPath[elem]);
            aux[elem] = new Vector3(
                timePath[elem],
                avgPath[elem],
                devPath[elem]
            );
        }
        return aux;
    }
}

public class Card {
    int value, suit, color;

    public Card (string v, string s, string c)
    {
        value = int.Parse(v);
        suit = int.Parse(s);
        color = int.Parse(c);
    }
}