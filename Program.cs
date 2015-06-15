using System;
using System.Numerics;

namespace Problem82K
{
    class Program
    {
        static void Main(string[] args)
        {

            // The base up to which to calculate results.
            // Set this to 5 to verify that the algorithm correctly identifies 82000.
            int maxBase = 6;

            // Maximum number of digits (in base 3).
            int maxNumbers = 20000;
            int maxDigits = 100000;

            // precalculate a table of powers for all bases up to the target base.
            BigInteger[][] powerOf = new BigInteger[maxBase + 1][];
            for (int b = 3; b <= maxBase; b++)
            {
                powerOf[b] = new BigInteger[maxNumbers];
                powerOf[b][0] = 1;
                for (int i = 1; i < maxNumbers; i++)
                {
                    powerOf[b][i] = powerOf[b][i - 1] * b;
                }
            }

            // bitmap that represents the current number
            byte[] currentNumberMap = new byte[maxDigits];
            byte[] tempNumberMap = new byte[maxDigits];
            int currentMaxBit = 0;



            currentMaxBit = 9000;
            currentNumberMap[currentMaxBit] = 1;



            BigInteger currentNumber = 0;
            BigInteger tempNumber;

            int ticks = Environment.TickCount;
            int numsPerSec = 0;

            while (true)
            {
                // build number from map...
                currentNumber = 0;
                for (int bit = 0; bit <= currentMaxBit; bit++)
                {
                    if (currentNumberMap[bit] != 0)
                    {
                        currentNumber += powerOf[maxBase][bit];
                    }
                }

                bool allGood = true;
                bool doAdd = true;

                if (allGood)
                {

                    for (int b = maxBase - 1; b >= 3; b--)
                    {
                        int logBaseB = logCeil(currentNumber, b);
                        int maxLog = logBaseB;

                        tempNumber = currentNumber;
                        tempNumberMap[logBaseB] = 0; tempNumberMap[logBaseB + 1] = 0;

                        bool good = true;
                        while (logBaseB > 0)
                        {
                            logBaseB--;
                            if (tempNumber >= powerOf[b][logBaseB])
                            {
                                tempNumberMap[logBaseB] = 1;

                                tempNumber -= powerOf[b][logBaseB];
                                if (tempNumber >= powerOf[b][logBaseB])
                                {

                                    getNextNumber(b, maxBase, tempNumberMap, maxLog, logBaseB, powerOf, currentNumberMap, ref currentMaxBit);
                                    doAdd = false;

                                    good = false;
                                    break;
                                }
                            }
                            else
                            {
                                tempNumberMap[logBaseB] = 0;
                            }
                        }
                        if (!good)
                        {
                            allGood = false;
                            break;
                        }
                    }

                }


                if (allGood)
                {
                    Console.WriteLine("Found it >>>>>> " + currentNumber);
                    Console.ReadKey();
                }


                numsPerSec++;
                if (Environment.TickCount > ticks + 1000)
                {
                    ticks = Environment.TickCount;
                    Console.WriteLine("Numbers per second: " + numsPerSec + ", digit length: " + currentNumber.ToString().Length);
                    numsPerSec = 0;
                }

                if (doAdd)
                {
                    addOne(currentNumberMap, ref currentMaxBit, 0);
                }

            }
        }


        private static void addOne(byte[] numberMap, ref int maxBit, int startBit)
        {
            for (int i = startBit; i < numberMap.Length - 1; i++)
            {
                if (i > maxBit)
                {
                    maxBit = i;
                }
                if (numberMap[i] == 0)
                {
                    numberMap[i] = 1;
                    break;
                }
                else
                {
                    numberMap[i] = 0;
                }
                // carrying...
            }
        }


        private static int logCeil(BigInteger number, int logBase)
        {
            int log = 0;
            BigInteger temp = number;
            while (temp > 0)
            {
                log++;
                temp /= logBase;
            }
            return log;
        }




        private static void getNextNumber(int currentBase, int desiredBase, byte[] tempNumberMap, int maxLog, int minLog, BigInteger[][] powerOf, byte[] currentNumberMap, ref int currentMaxBit)
        {
            tempNumberMap[minLog] = 1;
            addOne(tempNumberMap, ref maxLog, minLog);

            BigInteger tempNumber = 0;
            for (int i = minLog; i <= maxLog; i++)
            {
                if (tempNumberMap[i] != 0)
                {
                    tempNumber += powerOf[currentBase][i];
                }
            }



            int logBaseB = logCeil(tempNumber, desiredBase);
            int newMaxLog = logBaseB;

            tempNumberMap[logBaseB] = 0; tempNumberMap[logBaseB + 1] = 0;

            bool doAdd = false;
            while (logBaseB > 0)
            {
                logBaseB--;
                if (tempNumber >= powerOf[desiredBase][logBaseB])
                {
                    tempNumberMap[logBaseB] = 1;

                    tempNumber -= powerOf[desiredBase][logBaseB];
                    if (tempNumber >= powerOf[desiredBase][logBaseB])
                    {
                        doAdd = true;
                        break;
                    }
                }
                else
                {
                    tempNumberMap[logBaseB] = 0;
                }
            }

            if (doAdd)
            {
                addOne(tempNumberMap, ref newMaxLog, logBaseB);
            }

            for (int i = 0; i < newMaxLog; i++)
            {
                currentNumberMap[i] = 0;
            }
            for (int i = logBaseB; i <= newMaxLog; i++)
            {
                currentNumberMap[i] = tempNumberMap[i];
            }
            currentMaxBit = newMaxLog;

        }


    }
}
