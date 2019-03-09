using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MakeFakeEditions();
        }
        private void MakeFakeEditions()
        {
            char[] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] lowerCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower().ToCharArray();

            Random r = new Random();

            String[] countryNames = new String[55];

            for (int i = 0; i < countryNames.Length; i++)
            {
                String countryName = "" + upperCase[r.Next(0, 26)];
                int letters = r.Next(3, 6);
                for (int j = 0; j < letters; j++)
                {
                    countryName += lowerCase[r.Next(0, 26)];
                }
                countryNames[i] = countryName;
            }
            for (int i = 71; i <= 80; i++)
            {
                List<String> tempList = new List<String>(countryNames);
                string[] countriesInFinal = new string[27];
                for (int j = 0; j < countriesInFinal.Length; j++)
                {
                    string country = tempList[r.Next(tempList.Count)];
                    countriesInFinal[j] = country;
                    tempList.Remove(country);
                }
                int[] points = new int[] { 12, 10, 8, 7, 6, 5, 4, 3, 2, 1 };
                int[] indexes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27 };
                string topLine = "";
                string[] results = new string[27];
                for (int j = 0; j < countriesInFinal.Length; j++)
                {
                    results[j] = countriesInFinal[j];
                }
                for (int j = 0; j < countryNames.Length; j++)
                {
                    topLine += "," + countryNames[j];

                    List<int> indexList = new List<int>(indexes);
                    int[] intArr = new int[10];
                    for (int k = 0; k < intArr.Length; k++)
                    {
                        int country = indexList[r.Next(indexList.Count)];
                        intArr[k] = country;
                        indexList.Remove(country);
                    }
                    for (int k = 0; k < results.Length; k++)
                    {
                        string num = "";
                        int index = -1;
                        for (int l = 0; l < intArr.Length; l++)
                        {
                            if (k + 1 == intArr[l]) index = l;
                        }
                        if (index != -1) num = "" + points[index];
                        results[k] += "," + num;

                    }

                }
                File.WriteAllText("ISC" + i + ".csv", topLine + "\n");
                File.AppendAllLines("ISC" + i + ".csv", results);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            List<SimilarityTables> sims = new List<SimilarityTables>();
            for (int i = (int)mostRecentEdition.Value - 9; i <= mostRecentEdition.Value; i++)
            {
                string path = "Sims" + i + ".csv";
                if (!File.Exists(path))
                {
                    
                    try
                    {
                        if (!File.Exists("ISC" + i + ".csv"))
                        {
                            throw new Exception();
                        }
                        sims.Add(MakeSimFile(i));
                        
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Could not find valid file for ISC " + i);
                        return;
                    }
                }
                else
                {
                    sims.Add(LoadFile(path));
                }
            }
            
            double[,] combinedSims = Combine(sims, sims[sims.Count-1].voterNames);

            PrintInformation(combinedSims, sims[sims.Count - 1].voterNames, (mostRecentEdition.Value - 9) + "-" + mostRecentEdition.Value);

            new CreatePots(combinedSims, sims[sims.Count - 1].voterNames);
        }
        private SimilarityTables LoadFile(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            string[] countryNames = lines[0].Split(',');
            countryNames = countryNames.Skip(1).ToArray();

            double[,] similarities = new double[countryNames.Length, countryNames.Length];
            for (int i = 0; i < countryNames.Length; i++ )
            {
                string[] doubles = lines[i+1].Split(',');
                for(int j = 0; j < countryNames.Length; j++)
                {
                    similarities[i, j] = Double.Parse(doubles[j + 1]);
                }
            }

            return new SimilarityTables(similarities, countryNames);
        }
        private SimilarityTables MakeSimFile(int editionNum)
        {
            string[] lines = File.ReadAllLines("ISC" + editionNum + ".csv");

            // step one, reorder lines so results are in alphabetical order

            Array.Sort(lines);


            // step two, break all the votes into Vote Objects

            int nonFinalistLocation = 27;
            string[] voterList = lines[0].Split(',');
            string[,] votes = new string[lines.Length - 1, voterList.Length - 1];

            string[] finalists = new string[lines.Length - 1];

            for (int i = 1; i < lines.Length; i++)
            {
                string[] temp = lines[i].Split(',');
                finalists[i - 1] = temp[0];
                for (int j = 1; j < temp.Length; j++)
                    votes[i - 1, j - 1] = temp[j];
            }
            Votes[] fullVotes = new Votes[voterList.Length - 1];
            for (int i = 1; i < voterList.Length; i++)
            {
                int[] intVotes = new int[lines.Length - 1];

                String voter = voterList[i];

                int location = nonFinalistLocation;

                bool flag = true;
                for (int j = 0; j < finalists.Length; j++)
                {
                    if (voter.Equals(finalists[j]))
                    {
                        flag = false;
                        location = j;
                        break;
                    }
                }

                for (int j = 0; j < intVotes.Length; j++)
                {
                    string vote = votes[j, i - 1];
                    int value = 0;
                    if (!vote.Equals(""))
                    {
                        value = int.Parse(vote);
                    }
                    intVotes[j] = value;
                }

                fullVotes[location] = new Votes(voter, intVotes);

                if (flag)
                {
                    nonFinalistLocation++;
                }

            }
            string[] voterNames = new string[fullVotes.Length];
            for(int i = 0; i < voterNames.Length; i++)
            {
                voterNames[i] = fullVotes[i].GetVoter();
            }
            double[,] similarities = new CalculateSimilarity(adjustTheVotes(fullVotes), voterNames).GetSimilarity();
            PrintInformation(similarities, voterNames, "" + editionNum);

            return new SimilarityTables(similarities, voterNames);
        }
        private double[,] adjustTheVotes(Votes[] theVotes)
        {
            double[,] votes = new double[theVotes[0].Length, theVotes.Length];
            for (int i = 0; i < theVotes[0].Length; i++)
            {

                for (int j = 0; j < theVotes.Length; j++)
                {
                    if (j < theVotes[0].Length)
                    {
                        if (i == j)
                        {
                            votes[i,j] = 12.0;
                        }
                        else if (theVotes[i].Get(j) == 12.0 || theVotes[i].Get(j) == 10.0)
                        {
                            votes[i,j] = theVotes[i].Get(j) - 2;
                        }
                        else if (theVotes[i].Get(j) == 1.0)
                        {
                            votes[i,j] = .5;
                        }
                        else if (theVotes[i].Get(j) != 0)
                        {
                            votes[i,j] = theVotes[i].Get(j) - 1;
                        }
                    }
                    else
                    {
                        votes[i, j] = theVotes[j].Get(i);
                    }
                }
            }
            return votes;

        }
        public double[,] Combine(List<SimilarityTables> data, String[] activeCountries)
        {
            double[,] similarities = new double[activeCountries.Length, activeCountries.Length];

            int[,] counter = new int[activeCountries.Length, activeCountries.Length];


            for (int i = 0; i < data.Count; i++)
            {
                combineIt(data[i].similarities, data[i].voterNames, similarities, counter, activeCountries);
            }

            averageIt(similarities, counter);

            return similarities;
        }

        private void combineIt(double[,] ds, String[] strings, double[,] similarities, int[,] counter, String[] activeCountries)
        {
            int checkOneCounter = 0;
            int checkTwoCounter = 0;
            for (int i = 0; i < similarities.GetLength(0); i++)
            {
                for (int j = i + 1; j < similarities.GetLength(0); j++)
                {
                    if (i == j)
                    {
                        similarities[i, j] = 0;
                        continue;
                    }
                    bool foundActiveOne = false;
                    bool foundActiveTwo = false;

                    String countryOne = activeCountries[i];
                    String countryTwo = activeCountries[j];

                    int firstLoc = 0;
                    int secondLoc = 0;

                    for (int k = 0; k < strings.Length; k++)
                    {
                        if (countryOne.Equals(strings[k]))
                        {
                            firstLoc = k;
                            foundActiveOne = true;
                            checkOneCounter++;

                        }
                        else if (countryTwo.Equals(strings[k]))
                        {
                            secondLoc = k;
                            foundActiveTwo = true;
                            checkTwoCounter++;

                        }
                        if (foundActiveOne && foundActiveTwo) break;
                    }
                    if (!foundActiveOne) continue;



                    if (foundActiveOne && foundActiveTwo)
                    {
                        bool valid = true;
                        if (ds[firstLoc, secondLoc] == 0) valid = testValid(ds, firstLoc, secondLoc);
                        if (valid)
                        {
                            similarities[i, j] += ds[firstLoc, secondLoc];
                            counter[i, j]++;
                        }

                    }

                }
            }


        }

    private bool testValid(double[,] ds, int i, int j)
    {
        double sum = 0;
        for (int k = 0; k < ds.GetLength(0); k++)
        {
            sum += ds[i, k];
        }
        if (sum == 0) return false;
        sum = 0;
        for (int k = 0; k < ds.GetLength(0); k++)
        {
            sum += ds[k, j];
        }
        if (sum == 0) return false;
        return true;
    }

    public void averageIt(double[,] similarities, int[,] counter)
    {
        for (int i = 0; i < similarities.GetLength(0); i++)
        {
            for (int j = 0; j < similarities.GetLength(0); j++)
            {
                if (counter[i,j] != 0) similarities[i,j] = similarities[i,j] / counter[i,j];
            }
        }
    }

    
        
        private void PrintInformation(double[,] similarities, String[] voterNames, string editionNum)
        {	
            StringBuilder votingList = new StringBuilder();
	
            StringBuilder[] similarityLines = new StringBuilder[similarities.GetLength(0)];
	
            for(int i  = 0; i < voterNames.Length; i++)
            {
	            if(i == 0)votingList.Append(voterNames[i]);
                else votingList.Append(", " + voterNames[i]);
            }
            for(int i = 0; i < similarities.GetLength(0); i ++)
            {
	            similarityLines[i] = new StringBuilder();
	            similarityLines[i].Append(voterNames[i]);
                for (int j = 0; j < similarities.GetLength(1); j++)
	            {
	            similarityLines[i].Append(", " + similarities[i,j]);
	            }
            }
            String writer = "";
            writer += "ISC " + editionNum + ", " + votingList.ToString() + "\n";
            for (int i = 0; i < similarityLines.Length; i++)
            {
                writer += similarityLines[i].ToString() + "\n";
            }

            File.WriteAllText("Sims" + editionNum + ".csv", writer);
        }
    
    }

}
class Votes
{
    private string voter;
    private int[] votes;    
    public Votes(String voter, int[] votes)
    {
        this.voter = voter;
        this.votes = votes;
    }
    public int Length
    {
        get
        {
            return votes.Length;
        }
    }
    public string GetVoter()
    {
        return voter;
    }
    public int[] GetVotes()
    {
        return votes;
    }
    public int Get(int index)
    {
        return votes[index];
    }

}
class CalculateSimilarity
{
    private double[,] votes, similarity;
    private String[] voterNames;
    public CalculateSimilarity(double[,] votes, String[] voterNames)
    {
        this.votes = votes;
        this.voterNames = voterNames;
        similarity = new double[voterNames.Length, voterNames.Length];

        CreateSimilarity();

    }    
    private void CreateSimilarity()
    {
        for (int i = 0; i < similarity.GetLength(0); i++)
        {
            for (int j = 0; j < similarity.GetLength(0); j++)
            {
                if (i == j) similarity[i, j] = 0;
                else if (i > j)
                {
                    similarity[i, j] = similarity[j, i];
                }
                else
                {
                    similarity[i, j] = Calculate(i, j);
                }
            }
        }

    }
    private double Calculate(int i, int j)
    {
        double returnVal = 0;
        for (int k = 0; k < votes.GetLength(0); k++)
        {
            try
            {
                returnVal += votes[k, i] * votes[k, j];
            }
            catch (Exception E)
            {
                returnVal += 0;
            }

        }

        return returnVal / 4.48;
    }
    public double[,] GetSimilarity()
    {
        return similarity;
    }
}
public class SimilarityTables
{
    public double[,] similarities;
    public string[] voterNames;
    public SimilarityTables(double[,] similarities, string[] voterNames)
    {
        this.similarities = similarities;
        this.voterNames = voterNames;
    }
}