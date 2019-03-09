public class testPots
{
    double[,] data;
    public testPots(double[,] d)
    {
        data = d;
    }
    public double testPot(int[][] temp)
    {
        double returnVal = 0.0;
        for (int i = 0; i < temp.Length; i++)
        {
            returnVal += testPot(temp[i]);

        }
        return returnVal;
    }
    private double testPot(int[] temp)
    {
        double returnVal = 0.0;
        for (int i = 0; i < temp.Length; i++)
        {
            for (int j = i + 1; j < temp.Length; j++)
            {
                returnVal += data[temp[i],temp[j]];
            }
        }
        return returnVal;
    }

}
