
using System;

// This is the first program that needs to be run! 
// The information stored in Config.txt is pulled into the program and stored in Var.cs (The global variables file). 
// ******notes on adding global variables*******

public class Configuration
{
    public void config()
    {
        string text =  System.IO.File.ReadAllText(Environment.CurrentDirectory + @"\Config.txt");
        string[] stringcharacteristics = {"IPaddress","SamplesPerScan","ScanDirection","ScanFrequency","FilterType","FilterWidth","ScanDataType","ScanStartAngle","ScanFieldAngle","MaxRange","HMIDisplayType","HMIDisplayText1","HMIDisplayText2"};

        string[] stringvariables = new string[stringcharacteristics.GetLength(0)];
        
        //Finding Number of characters in each stringcharacteristic, so that we can then offset by this amount and get to the data
        int[] stringlength = new int[stringcharacteristics.GetLength(0)];   
        for (int a = 0; a < stringcharacteristics.GetLength(0); a++)
        {
            string name = stringcharacteristics[a]; 
            stringlength[a] = name.Length + 1;
        }

        //Getting the data out and storing it in stringvariables
        Console.WriteLine("Reading Config.txt file");
        try
        {
        for (int a = 0; a < stringcharacteristics.GetLength(0); a++)
        {
            int num = text.IndexOf(stringcharacteristics[a]);
            //Console.WriteLine("int num = " + num);
            string substring = text.Substring(num+stringlength[a], 40);
            //Console.WriteLine("substring = " + substring);
            stringvariables[a] = substring.Substring(0,(substring.IndexOf(';')));
            Console.WriteLine(stringcharacteristics[a] + " = " + stringvariables[a]);
        }
        }
        catch
        {
            Console.WriteLine("Config file error, do not change the settings name, only the value!!!");
            return;
        }
        
        //Each variable need manually entered in
        try
        {

        



        Var.IPaddress = stringvariables[0];
        Var.SamplesPerScan = Convert.ToInt32(stringvariables[1]);        
        Var.ScanDirection = stringvariables[2];
        Var.ScanFrequency = Convert.ToInt32(stringvariables[3]);
        Var.FilterType = stringvariables[4];
        Var.FilterWidth = Convert.ToInt32(stringvariables[5]);
        Var.ScanDataType = stringvariables[6];
        Var.ScanStartAngle = (Convert.ToInt32(stringvariables[7]) * 10000);
        Var.ScanFieldAngle = Convert.ToInt32(stringvariables[8]);
        Var.maxrange = Convert.ToInt32(stringvariables[9]);
        if(Var.FilterType == "none")
        {
            Var.maxnumpointsscan = Math.Round((Var.SamplesPerScan/360) * Convert.ToInt32(stringvariables[8]));
        }
        else
        {
            Var.maxnumpointsscan = Math.Round((Var.SamplesPerScan/360) * Convert.ToInt32(stringvariables[8])/Var.FilterWidth);
        }
        
        Var.HMIDisplayType = stringvariables[10];
        Var.HMIDisplayText1 = stringvariables[11];
        Var.HMIDisplayText2 = stringvariables[12];




        Console.WriteLine("Success!\r\n");
        }
        catch
        {
            Console.WriteLine("Error in the Config File, make sure you are using valid values, please reveiw and try agian");
            return;
        }
    }

}