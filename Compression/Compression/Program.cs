using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compression
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //check for user arguments from command prompt, only 2 accepted
            if (args.Count() == 2)
            {
                //check to see if the file the user has pointed to exist
                if (File.Exists(args[1]))
                {
                    //checks if file is empty, cannot compress or decompress empty file
                    if (new FileInfo(args[1]).Length == 0)
                    {
                        //Error to display for empty file
                        Console.WriteLine("Cannot compress or decompress as this file is empty");
                    }
                    else
                    {
                        //check for first user input, -c for compression
                        if (args[0] == "-c")
                        {
                            //create the name of output file by removing the extension from the inptut file and appending "-compressed.bin" to the end
                            string outputFile = Path.GetFileNameWithoutExtension(args[1]) + "-compressed.bin";
                            Compress(args[1], outputFile);
                        }
                        //-d input is for decompression
                        else if (args[0] == "-d")
                        {
                            //create the name of output file by removing the extension from the inptut file and appending "-decompressed.txt" to the end
                            string outputFile = Path.GetFileNameWithoutExtension(args[1]) + "-decompressed.txt";
                            Decompress(args[1], outputFile);
                        }
                        else
                        {
                            //if the user input as the first argument an input other then "-c" or "-d"
                            Console.WriteLine("None valid first argument, \"-c\" for compression or \"-d\" for decompression");
                        }
                    }
                }
                else
                {
                    //Error if the file user points to does not exist
                    Console.WriteLine("File does not exist");
                    
                }
            }
            else if (args.Count() <= 1)
            {
                //Error if user provides a number of arguments less then 2
                Console.WriteLine("Missing arguments");
            }
            else if (args.Count() > 2)
            {
                //Error if user provides a number of arguments greater then 2
                Console.WriteLine("Too many arguments");
            }
        }

        //Method used for compressing files
        static void Compress(string input, string output)
        {
            //start of method

            //Initilize variables
            int totalChars = 0; //Counts number of characters in file, used for decompression purposes
            BinaryTree<CharacterFrequency> charTree = new BinaryTree<CharacterFrequency>();
            StreamReader read = new StreamReader(input);

            //Create a dictionary of CharacterFrequency objects with the int key being the ascii code of each character
            IDictionary<int, CharacterFrequency> charDictionary = new Dictionary<int, CharacterFrequency>();

            //Read entire file character by character...
            while (!read.EndOfStream)
            {
                int i = read.Read();

                //checks if Dictionary contains the char i...
                if (!charDictionary.ContainsKey(i))
                {
                    //if not add new record to dictionary
                    charDictionary.Add(i, new CharacterFrequency((char)i,1));
                }
                else
                {
                    //if so increment the frequency of this record
                    charDictionary[i].increment();
                }
                totalChars++;
            }
            read.Close();

            //Create ordered linked list of TreeNode<CharacterFrequency> objects
            OrderedLinkedList<TreeNode<CharacterFrequency>> orderedNodeList = new OrderedLinkedList<TreeNode<CharacterFrequency>>();

            //Go throughs each KeyValuePair in the CharacterFrequency dicitionary..
            foreach (KeyValuePair<int,CharacterFrequency> c in charDictionary)
            {
                //Make a new TreeNode of the CharacterFrequency value from c...
                TreeNode<CharacterFrequency> node = new TreeNode<CharacterFrequency>(c.Value);
                //add into the ordered linked list of TreeNode<CharacterFrequency>
                orderedNodeList.Add(node);
            }

            //Run a method in the binary tree to convert the ordered node list into a single node with branching children container all other nodes
            //and return and set said node as the root node of the tree
            charTree.Insert(charTree.TransferListToTree(orderedNodeList).GetFirst(), Relative.Root);

            //Run a method from the tree to return a list of BinaryPath objects
            LinkedList<BinaryPath<CharacterFrequency>> binaryTable = charTree.BinaryTable(charTree.Root);

            //Removes all nodes from the binarytable that have a null character as they cannot show up in a file
            var binary = binaryTable.First;
            while (binary != null)
            {
                var nextNode = binary.Next;
                if (binary.Value.Element.Character == '\0')
                {
                    binaryTable.Remove(binary);
                }
                binary = nextNode;
            }

            //Byte writing section

            int bitPosition = 7;//creates an int that determines what bit in the byte to set
            byte y = 0;//creates the byte to set and write with
            BinaryWriter binaryWriter = null;
            bool error = true;
            while(error)
            {
                try
                {
                    binaryWriter = new BinaryWriter(File.Create(CheckFile(output)));
                    error = false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("You do not have the permissions needed to write to this folder");
                }
            }

            StreamReader streamReader = new StreamReader(input);

            //string of the entire table, to used to append to the beginning of the output file. Will be read by out in decompression process
            string tableText = String.Empty;

            //Take the total characters from the input file and append to the beginning of the output file
            tableText += totalChars + "~~~";

            //Take table and write to output file
            foreach (BinaryPath<CharacterFrequency> bp in binaryTable)
            {
                tableText += bp.Element.Character + bp.Path + ";";
            }
            binaryWriter.Write(tableText);

            //Compression process
            while (!streamReader.EndOfStream)
            {
                //Read each character from file..
                int i = streamReader.Read();
                //Find path of character from binary table...
                string path = binaryTable.FirstOrDefault(obj => obj.Element.Character == (char)i).Path;

                //Iterate through each character in path...
                foreach (char c in path)
                {
                    //If bitposition = 0  we have set all other bits
                    if (bitPosition == 0)
                    {
                        //if character is '1' set bit at bitposition in byte to 1 using bitwise OR(|)
                        if (c == '1')
                        {
                            y = (byte)(y | (byte)Math.Pow(2, bitPosition));
                        }
                        //Writes byte to output file 
                        binaryWriter.Write(y);

                        //Resets byte and bitposition
                        y = 0;
                        bitPosition = 7;
                    }
                    //If bitposition != 0
                    else
                    {
                        //if character is '1' set bit at bitposition in byte to 1 using bitwise OR(|)
                        if (c == '1')
                        {
                            y = (byte)(y | (byte)Math.Pow(2, bitPosition));
                        }
                        //progresses bitposition down 1 to next bit in byte
                        bitPosition--;
                    }
                }
            }
            //Close stream reader
            read.Close();
            //write final byte to output file and close BinaryWriter
            binaryWriter.Write(y);
            binaryWriter.Close();

            //Output to command prompt to let user know file is finished being compressied, end program
            Console.WriteLine($"File has been compressed to the file {output}");
        }
        //Compression Method End

        public static string CheckFile(string path)
        {
            //Checking if the file to output to already exists
            if (!File.Exists(path))
            {
                //if not create it using binary writer
                return path;
            }
            else
            {
                //if it does ask the user would they like to overwrite the file
                if (MessageBox.Show($"File {Path.GetFileName(path)} already exist at this location, would you like to overwrite it?", "File already exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //if so, tell binary writer to overwrite the file when writing.
                    return path;
                }
                else
                {
                    //if not, make a loop of asking user what folder they would like to place the output file into
                    bool cancel = true;
                    while (cancel)
                    {
                        cancel = true;
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        fbd.Description = "Please select the folder where you would like the file to placed in.";
                        fbd.RootFolder = Environment.SpecialFolder.Desktop;
                        fbd.SelectedPath = Environment.CurrentDirectory;

                        //if user selects a folder
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            path = Path.GetFileName(path);
                            path = fbd.SelectedPath + "\\" + path;
                            path = CheckFile(path);
                            
                            cancel = false;
                            return path;
                        }
                        //If cancel or user does not select a folder
                        else
                        {
                            if (MessageBox.Show("No folder was selected, program will close.\nIs this okay?", "Must choose folder", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                            {
                                Console.WriteLine("No folder was selected, program has stopped");
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            return path;
        }

        //Method for converting table string to binarypath array
        public static Tuple<BinaryPath<char>[], int, BinaryReader> GetTableFromFile(string input)
        {
            //Open binary reader to read compressed file
            BinaryReader read = new BinaryReader(File.OpenRead(input));
            string tableFileString = "";
            //try catch to catch error from non compressed files
            try
            {
                //read table string from compressed file
                tableFileString = read.ReadString();
            }
            catch (EndOfStreamException ex)
            {
                //if file is not compressed, display error to user and close program
                Console.WriteLine("Cannot decompress a non-compressed file");
                Environment.Exit(0);
            }
            //get the int appended to the beginning of the file of the total number of characters that were contained in the file before compression
            int totalChar = int.Parse(Regex.Split(tableFileString, @"(?(?<=\d)~{3}|\0)")[0]);

            //create array from table string by splitting string by semi-colons
            string[] tableStringArray = Regex.Split(Regex.Split(tableFileString, @"(?(?<=\d)~{3}|\0)")[1], @"(?(?<=;)\0|;)").Where(x => x != String.Empty).ToArray();

            BinaryPath<char>[] binaryTable = new BinaryPath<char>[tableStringArray.Count()];

            //convert table string array into binarypath array
            for (int i = 0; i < binaryTable.Count(); i++)
            {
                binaryTable[i] = new BinaryPath<char>(tableStringArray[i][0], tableStringArray[i].Substring(1));
            }
            //return all variables within tuple
            return Tuple.Create(binaryTable, totalChar, read);

        }

        static void Decompress(string input, string output)
        {
            //get tuple from method
            var tuple = GetTableFromFile(input);

            //get binary reader from tuple to maintain stream position
            BinaryReader read = tuple.Item3;
            StreamWriter write = null;
            bool error = true;
            while (error)
            {
                try
                {
                    write = new StreamWriter(CheckFile(output));
                    error = false;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show("You do not have the permissions needed to write to this folder");
                }
            }
            

            //get total number of characters to write to output file from method
            int totalChar = tuple.Item2;

            //get binary path array from tuple
            BinaryPath<char>[] binaryCharTable = tuple.Item1;
            BinaryTree<TreeNode<char>> tree = new BinaryTree<TreeNode<char>>();

            //create root note in binary tree
            tree.Insert(new TreeNode<char>(), Relative.Root);

            //iterate through the binary paths for each character...
            foreach (BinaryPath<char> bp in binaryCharTable)
            {
                tree.moveTo(Relative.Root);

                int i = 1;
                //navigate through tree using characters from path of binarypath object bp
                foreach (char c in bp.Path)
                {
                    //if character is a 1...
                    if (c == '1')
                    {
                        //and if the next left node does not exist but we have reached the end of the path...
                        if (tree.Current.Left == null && i == bp.Path.Length)
                        {
                            //insert a new node to the left with the value being the character of the bp object
                            tree.Insert(new TreeNode<char>(bp.Element), Relative.LeftChild);
                        }
                        //or if the next left node does not exist but we are not at the end of the path...
                        else if (tree.Current.Left == null)
                        {
                            //insert a new null node to the left...
                            tree.Insert(new TreeNode<char>(), Relative.LeftChild);
                            //then move to this new node and continue the loop
                            tree.moveTo(Relative.LeftChild);
                            i++;
                        }
                        //or if the next left node does exist...
                        else
                        {
                            //then move to the next left node...
                            tree.moveTo(Relative.LeftChild);
                            //and continue the loop
                            i++;
                        }
                    }
                    //else if the character in the path is a 0...
                    else
                    {
                        //and if the next right node does not exist but we have reached the end of the path...
                        if (tree.Current.Right == null && i == bp.Path.Length)
                        {
                            //insert a new node to the right with the value being the character of the bp object
                            tree.Insert(new TreeNode<char>(bp.Element), Relative.RightChild);
                        }
                        //or if the next right node does not exist but we are not at the end of the path...
                        else if (tree.Current.Right == null)
                        {
                            //insert a new null node to the right...
                            tree.Insert(new TreeNode<char>(), Relative.RightChild);
                            //then move to this new node and continue the loop
                            tree.moveTo(Relative.RightChild);
                            i++;
                        }
                        //or if the right node does exist...
                        else
                        {
                            //then move to the next right node...
                            tree.moveTo(Relative.RightChild);
                            //and continue the loop
                            i++;
                        }
                    }
                }
            }
            //once the tree is complete move back to the root 
            tree.moveTo(Relative.Root);

            //Decompression process
            //create byte for reading purposes
            byte b = 0;
            //number of characters read from the file, to be compaired to the number of characters needing to be written for the output file
            int charWritten = 0;

            //Continue to read the next byte in the file until we have reached it's end
            while (read.BaseStream.Position != read.BaseStream.Length)
            {
                //get the next byte
                b = read.ReadByte();

                //loop to iterate through each bit in the byte starting at the 8th position and going backwards
                for (int i = 128; i > 0; i = i / 2)
                {
                    //a check to see if we have written all the characters we need to the output file, if not then continue the navigation
                    if (charWritten != totalChar)
                    {
                        //using bitwise AND to check if the bit at the current position is a 0...
                        if ((b & i) == 0)
                        {
                            //if it is a 0, move to the right child of the current node in the tree
                            tree.moveTo(Relative.RightChild);

                            //check if the node we moved to is a leaf
                            if (tree.Current.isLeaf)
                            {
                                //if it is a leaf, write the character from that leaf to the output file, and move back to the tree's root node
                                write.Write(tree.Current.Element.Element);
                                tree.moveTo(Relative.Root);
                                charWritten++;
                            }
                        }
                        //or a 1
                        else
                        {
                            //if it is a 1, move to the left child of the current node in the tree
                            tree.moveTo(Relative.LeftChild);

                            //check if the node we moved to is a leaf
                            if (tree.Current.isLeaf)
                            {
                                //if it is a leaf, write the character from that leaf to the output file, and move back to the tree's root node
                                write.Write(tree.Current.Element.Element);
                                tree.moveTo(Relative.Root);
                                charWritten++;
                            }
                        }
                    }
                }
            }
            //close the stream writer and output to screen the file has been decompressed and to what file, end program
            write.Close();
            Console.WriteLine($"File has been decompressed to the file {output}");
        }
    }
}
