
public class findWorst
{
    int[][] bestPot;
    double[,] data;
    public findWorst(int[][] best, double[,] d)
    {
        bestPot = best;
        data = d;
    }

    public int getWorst(int i)
    {
        int[] currentPot = bestPot[i - 1];
        int retVal = 0;
        double holder = 0;
        for (int j = 0; j < currentPot.Length; j++)
        {
            double temp = calculateSimilarity(currentPot, currentPot[j]);
            if (j == 0) holder = temp;
            else if (temp < holder)
            {
                retVal = j;
                holder = temp;
            }
        }
        return retVal;
    }

    private double calculateSimilarity(int[] currentPot, int j)
    {
        double retVal = 0.0;
        for (int i = 0; i < currentPot.Length; i++)
        {
            if (j != currentPot[i]) retVal += data[j,currentPot[i]];

        }
        return retVal;
    }

}
