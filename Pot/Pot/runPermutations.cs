using System;
public class runPermutations
{
    int[][] similarity;
    bool[] used;
    String[] countryNames;
    public runPermutations(int[][] data, String[] names)
    {
        similarity = data;
        used = new bool[55];

        countryNames = names;
    }
    public int[][] getPots(int i)
    {
        for (int j = 0; j < used.Length; j++)
        {
            used[j] = false;
        }
        int[][] retVal = new int[5][];
        int counter = 0;
        retVal[counter] = makePot(i);
        counter++;
        i++;
        for (; counter < retVal.Length - 1; counter++)
        {
            while (true)
            {
                if (i == used.Length) i = 0;

                if (!used[i])
                {
                    retVal[counter] = makePot(i);
                    break;
                }
                else i++;
            }
        }
        int otherCounter = 0;
        retVal[4] = new int[11];
        for (int j = 0; j < used.Length; j++)
        {
            if (!used[j])
            {
                retVal[counter][otherCounter] = j;
                used[j] = true;
                otherCounter++;
            }

        }

        return retVal;
    }
    private int[] makePot(int i)
    {
        int[] retVal = new int[11];
        for (int j = 0; j < retVal.Length; j++)
        {

            retVal[j] = addMember(i);

            for (int k = similarity[i].Length - 1; k >= 0; k--)
            {

                if (!used[similarity[i][k]])
                {
                    i = similarity[i][k];
                    break;
                }
            }
        }
        return retVal;
    }
    private int addMember(int i)
    {
        used[i] = true;
        return i;

    }

}

