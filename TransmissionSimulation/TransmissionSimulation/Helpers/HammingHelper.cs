﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace TransmissionSimulation.Helpers
{
    public static class HammingHelper
    {
        public static BitArray Encrypt(BitArray bitArrayInput)
        {
            int m = bitArrayInput.Length;
            int r = 0;
            int indiceInput = 0;

            // Compute the number of bit of controle
            while (m + r + 1 > Math.Pow(2, r))
                r++;

            BitArray bitArrayOutput = new BitArray(m + r);

            // Fill the bitArrayOutput with the value of the bitArrayInput with spaces for bit of controle (power of 2)
            for (int i = 0; i < bitArrayOutput.Length; i++)
            {
                if (!IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    bitArrayOutput[i] = bitArrayInput[indiceInput];
                    indiceInput++;
                }
            }

            // Fill the bit of controle (power of 2)
            for (int i = 0; i < bitArrayOutput.Length; i++)
            {
                if (IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    Console.WriteLine("Is Power of 2 : {0}", i+1);

                    int parityCount = GetParityCount(i, bitArrayOutput);

                    Console.WriteLine("ParityCount {0}", parityCount);

                    // Write 1 for odd and 0 for even
                    bitArrayOutput[i] = (parityCount & 1) == 1;
                    Console.WriteLine("ParityBool {0}", bitArrayOutput[i]);
                }
            }
            Console.Write(BitArrayToDigitString(bitArrayOutput));
            return bitArrayOutput;
        }

        public static BitArray Decrypt(BitArray bitArrayInput)
        {
            int errorSyndrome = 0;

            // Check the bit of controle (power of 2) to verify integrity
            for (int i = 0; i < bitArrayInput.Length; i++)
            {
                if (IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    int parityCount = GetParityCount(i, bitArrayInput);

                    //If parityCount is odd
                    if ((parityCount & 1) == 1)
                    {
                        errorSyndrome += i + 1;
                    }
                }
            }

            // Correct the error, if there is one ...
            if (errorSyndrome != 0)
            {
                bitArrayInput[errorSyndrome - 1] = !bitArrayInput[errorSyndrome - 1];
            }

            BitArray bitArrayOutput = new BitArray(GetDataSize(bitArrayInput.Length));
            int indiceOutput = 0;

            // Fill the bitArrayOutput without the bit of controle (power of 2), only data
            for (int i = 0; i < bitArrayInput.Length; i++)
            {
                if (!IsPowerOf2(i + 1)) // i+1 because indices start to 0 instead of 1
                {
                    bitArrayOutput[indiceOutput] = bitArrayInput[i];
                    indiceOutput++;
                }
            }

            return bitArrayOutput;
        }

        public static int GetDataSize(int totalSize)
        {
            int r = 0;
            while (totalSize + 1 > Math.Pow(2, r))
                r++;
            return totalSize - r;
        }

        public static bool IsPowerOf2(int i)
        {
            return (i & (i - 1)) == 0;
        }

        private static bool IsBitSet(int j, int i)
        {
            return (j & i) != 0;
        }
        public static string BitArrayToDigitString(BitArray bitArray)
        {
            var builder = new StringBuilder();
            foreach (bool bit in bitArray)
                builder.Append(bit ? "1" : "0");
            return builder.ToString();
        }

        private static int GetParityCount(int i, BitArray bitArray)
        {
            int parityCount = 0;
            // For each number where j is in decomposition of power of 2 (Tanenbaum book p. 222)
            // e.g. 11 = 8 + 2 + 1
            // e.g. 29 = 16 + 8 + 4 + 1
            for (int j = i; j < bitArray.Length; j++)
            {
                if (IsBitSet(j + 1, i + 1))
                {
                    Console.WriteLine(" -> Is Bit Set : {0}", j + 1);
                    if (bitArray[j])
                        parityCount++;
                }
            }

            return parityCount;
        }
    }
}
