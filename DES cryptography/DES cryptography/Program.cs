using System;
using System.Text;
using System.Security.Cryptography;

class Program
{
    //To make the project simpler :
    //I've chosen to work on the chars and not on the bits, so I will only use 0 and 1 for the plaintext and the key
    //I won't create a Roundkey, and will only use the key.
    //I will use 8 instead of 64 chars for the blocs.
    


    static void Main()
    {
        string plaintext = "0101010001010101010101010101100001110000101101";

        Console.WriteLine("The plain text is " + plaintext);

        string Key = "010101"; 
        

        string encryptedText = DES(plaintext, Key);

        Console.WriteLine("The encrypted text is " +  encryptedText);


    }

   
    static string DES(string text, string key)
    {       
        int[] InitialPermutationTable =
            { 2, 6, 3, 1, 4, 8, 5, 7 };//Permutation tables in DES, with only 8 because we use characters
        int[] LocalPermutationTable =
            { 2, 6, 3, 1, 4, 5 };
       int[] FinalPermutationTable =
            { 4, 1, 3, 5, 7, 2, 8, 6 };
    
        text = Multipleof8(text);
        string encryptedText = "";

        int NumberofBlocks = text.Length / 8;

        for (int i = 0; i < NumberofBlocks; i++) //Initial permutation
        {
            string block = text.Substring(i * 8, 8); //extract a block

            block = Permute(block, InitialPermutationTable);//initial permutation

            string RightHalf = block.Substring(4);//divide the block into two blocks
            string LeftHalf = block.Substring(0, 4);
       
            for (int j=0; j<16; j++)
            {
                string ExpandedRight = Expand(RightHalf);//right half of the block is expanded

                string XorRight = XOR(ExpandedRight, key); //Xor with the key

                string substitution = ""; 

                for (int a = XorRight.Length - 1; a >= 0; a--) //Subsitution of Xored (we reverse the string to simplify)
                {
                    substitution += XorRight[a];
                }
                //We go back to 4 chars at the end of the process
                string LocalPermutation = Permute(substitution, LocalPermutationTable).Substring(0,4);
                
                string XorLeft = XOR(LocalPermutation, LeftHalf);

                LeftHalf = RightHalf.Substring(0,4);
                RightHalf = XorLeft;              
            }
            string encryptedBlock = "";
            encryptedBlock += LeftHalf;
            encryptedBlock += RightHalf;

            block = Permute(encryptedBlock, FinalPermutationTable);
            encryptedText += encryptedBlock;                        
        }
        return encryptedText;
    }

    static string Multipleof8(string text)
    //Verify if the plaintext is a multiple of 8, makes it if not by adding empty space, with Euclidean division
    {
        int remain = text.Length % 8;

        if (remain != 0)
        {
            int MissingLength = 8 - remain;
            string missingtext = " ";
            for (int i = 1; i < MissingLength; i++)
            {
                missingtext += " ";
            }
            text += missingtext;
        }
       
        return text;
    }

    static string Permute(string text, int[] permutationTable)
    //permute chars inside a block of 8 chars according to the chosen permutation table
    {
        char[] chars = new char[permutationTable.Length];//8
        for (int i = 0; i < permutationTable.Length; i++)
        {
            chars[i] = text[permutationTable[i]-1];
        }
        return new string(chars);
    }

    static string Expand(string text)
    {
        //it works like Permute, we use a table to expand, like in real DES
        int[] expandTable = { 1, 2, 3, 4, 1, 2 }; 
       
        text = text.PadRight(6, ' '); //we expand from 4 char to 6 (instead of 32bits to 48bits in the real algo)

        string expanded = "";
        for (int i = 0; i < expandTable.Length; i++)
        {
            expanded += text[expandTable[i] - 1];
        }

        return expanded;
    }

    static string XOR(string text, string text2) //classic XOR, if bits are equal > 0, if not >1
    {
        string result = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == text2[i])
            {
                result += '0';
            }
            else
            {
                result += '1';
            }
        }
        return result;
    }
    
}
