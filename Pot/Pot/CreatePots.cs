using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pot
{
    public class CreatePots
    {
        private double[,] data;
        private String[] countryNames;
        public CreatePots(double[,] data, String[] countryNames)
        {
            this.data = data;
            this.countryNames = countryNames;	    

	    int[][] similarity = new int[55][];
        double t = 0.0;


	    for(int i = 0; i < similarity.GetLength(0); i++)
	    {
		    similarity[i] = findBestMatches(i);
	    }

            

	    runPermutations run = new runPermutations(similarity, countryNames);
	    int[][] bestPot = new int[5][];
	    int[][][] arrayOfBestPots = new int[similarity.Length][][];
	    testPots test = new testPots(data);

	    int[][] temp = new int[5][];

	    for(int z = 0; z<similarity.Length;z++)
	    {
		double testVal = 0.0;
		temp = run.getPots(z);

		double tempNum = test.testPot(temp);
		if(testVal < tempNum)
		{

		    testVal = tempNum;
		    for(int k = 0; k< temp.Length; k++)
		    {
			for (int j = 0; j < temp[k].Length; j++)
			{
			    bestPot[k][j] = temp[k][j];
			}
		    }
		}




		testVal = test.testPot(bestPot);
		bool madeChanges = true;
		while(madeChanges)
		{
		    findWorst worst = new findWorst(bestPot, data);
		    madeChanges = false;
		    int[] least = new int[5];
		    least[0] = worst.getWorst(1);
		    least[1] = worst.getWorst(2);
		    least[2] = worst.getWorst(3);
		    least[3] = worst.getWorst(4);
		    least[4] = worst.getWorst(5);

		    temp = new int[5][];


		    for(int k = 0; k< temp.Length; k++)
		    {
			for (int j = 0; j < temp[k].Length; j++)
			{
			    temp[k][j] = bestPot[k][j];

			}
		    }
		    for(int i = 0; i < bestPot.Length; i++)
		    {
			for(int j = 0; j < bestPot.Length; j++)
			{
			    temp[i][least[i]] = bestPot[j][least[j]];
			    temp[j][least[j]] = bestPot[i][least[i]]; 

			    t = test.testPot(temp);

			    if(t > testVal)
			    {

				madeChanges = true;
				testVal = t;
				for(int k = 0; k< temp.Length; k++)
				{
				    for (int l = 0; l < temp[k].Length; l++)
				    {
					bestPot[k][l] = temp[k][l];
				    }
				}
			    }
			    else
			    {
				for(int k = 0; k< temp.Length; k++)
				{
				    for (int l = 0; l < temp[k].Length; l++)
				    {
					temp[k][l] = bestPot[k][l];

				    }
				}
			    }
			}
		    }



		}
		madeChanges = true;
		while(madeChanges)
		{
		    madeChanges = false;

		    temp = new int[5][];


		    for(int k = 0; k< temp.Length; k++)
		    {
			for (int j = 0; j < temp[k].Length; j++)
			{
			    temp[k][j] = bestPot[k][j];

			}
		    }
		    for(int i = 0; i < bestPot.Length; i++)
		    {
			for(int j = 0; j < bestPot[i].Length;j++)
			{
			    for(int k = 0; k < bestPot.Length; k++)
			    {
				for(int l = 0; l < bestPot[k].Length;l++)
				{
				    if(i == k)continue;
				    temp[i][j] = bestPot[k][l];
				    temp[k][l] = bestPot[i][j]; 

				    t = test.testPot(temp);
				    testVal = test.testPot(bestPot);

				    if(t > testVal)
				    {

					madeChanges = true;
					testVal = t;
					for(int m = 0; m< temp.Length; m++)
					{
					    for (int n = 0; n < temp[m].Length; n++)
					    {
						bestPot[m][n] = temp[m][n];
					    }
					}
				    }
				    else
				    {
					for(int m = 0; m< temp.Length; m++)
					{
					    for (int n = 0; n < temp[m].Length; n++)
					    {
						temp[m][n] = bestPot[m][n];

					    }
					}
				    }
				}
			    }
			}
		    }
		}
		for(int m = 0; m< temp.Length; m++)
		{
		    for (int n = 0; n < temp[m].Length; n++)
		    {
			arrayOfBestPots[z][m][n] = bestPot[m][n];

		    }
		}
	    }
	    t = 0.0;
	    for(int i = 0; i < similarity.Length; i++)
	    {
		double tempDouble = test.testPot(arrayOfBestPots[i]);
		Console.WriteLine(tempDouble + " - " + t);
		if(tempDouble > t)
		{
		    t = tempDouble;
		    for(int m = 0; m< temp.Length; m++)
		    {
			for (int n = 0; n < temp[m].Length; n++)
			{
			    bestPot[m][n] = arrayOfBestPots[i][m][n];

			}
		    }
		}
	    }
	    for(int i = 0; i <bestPot.Length; i++)
	    {
		Console.Write("\n [B][U]Pot " + (i+1) + "[/U][/B]\n ");
		for(int j = 0; j < bestPot[i].Length;j++)
		{
		    Console.Write("\n :" + countryNames[bestPot[i][j]].ToLower().Replace(" ", "") + ": " + countryNames[bestPot[i][j]]);
		}
	    }
        }
        public int[] findBestMatches(int i)
        {
            double[] topCountries = new double[54];
            int[] countryLocs = new int[54];

            for (int j = 0; j < data.GetLength(1); j++)
            {
                bool inserted = false;
                for (int k = 0; k < topCountries.Length; k++)
                {

                    if (data[i, j] > topCountries[k])
                    {
                        if (k != 0)
                        {
                            topCountries[k - 1] = topCountries[k];
                            countryLocs[k - 1] = countryLocs[k];
                        }
                    }
                    else
                    {
                        if (k != 0)
                        {
                            countryLocs[k - 1] = j;
                            topCountries[k - 1] = data[i, j];
                        }
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                {
                    countryLocs[topCountries.Length - 1] = j;
                    topCountries[topCountries.Length - 1] = data[i, j];
                }
                //System.out.println(toString(countryLocs));
                //System.out.println(toString(topCountries));

            }
            //String[] retVal = new String[10];
            //for(int j = 0; j<topCountries.length;j++)
            //{
            // retVal[j] = countryNames[countryLocs[j]];
            //}
            return countryLocs;
        }
    }
}
