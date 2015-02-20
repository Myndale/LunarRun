using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MakeMap
{
	public interface IBinaryEncoder
	{
		int Progress { get; }
		int TotalLength { get; set; }
		long DecodeText(BinaryReader myReader, string fileName2);
		byte[] DecodeText(BinaryReader myReader, out int size);
		Stream DecodeText(BinaryReader myReader);
		long DecodeText(string fileName, string fileName2);
		long EncodeByteArray(string fileName, BinaryWriter myWrite, out int fileSize);
		long EncodeByteArray(byte[] text, BinaryWriter myWrite, bool firstChunk);
		long EncodeByteArray(byte[] text, string fileName);
	}

	/// <summary>
	/// Internal class. a leaf is a tree with the bool <code>leaf</code> turned on
	/// </summary>
	internal class Tree
	{
		#region Fields

		public int freq { get; private set; }
		public bool leaf { get; internal set; }
		public Tree left { get; internal set; }
		public byte letter { get; private set; }
		public Tree right { get; internal set; }

		#endregion Fields

		#region Constructors

		public Tree(bool leaf, byte letter, int freq)
		{
			left = null;
			right = null;
			this.freq = freq;
			this.leaf = leaf;
			this.letter = letter;
		}

		#endregion Constructors

		#region Methods

		public bool IsFull()
		{
			return (left != null && right != null);
		}

		#endregion Methods
	}

	public class HuffManEncoder : IBinaryEncoder
	{
		#region Fields

		public List<List<byte>> codes;

		List<int> frequency = new List<int>();
		List<byte> literals = new List<byte>();
		int[] mapper = new int[256];
		private FileIOPermission permit;
		private int progress = 0;
		Tree root;
		private int totalLength;

		#endregion Fields

		#region Constructors

		public HuffManEncoder()
		{
			permit = new FileIOPermission(PermissionState.Unrestricted);
			permit.AllLocalFiles = FileIOPermissionAccess.AllAccess;
		}

		#endregion Constructors

		#region Properties

		public int Progress
		{
			get
			{
				return progress;
			}
		}

		public int TotalLength
		{
			get { return totalLength; }
			set
			{
				if (value != 0)
					totalLength = value;
			}
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Build encoding tree from the frequency table and the mapping
		/// </summary>
		/// <param name="totalLength">Total Length of the text</param>
		private void buildEncoding(int totalLength)
		{
			//add all literals and their frequencies as leafs. Creates a forest
			List<Tree> nodes = new List<Tree>(0); //Store all tree nodes here so we can connect them easily
			for (int i = 0; i < literals.Count; i++)
				nodes.Add(new Tree(true, literals[i], frequency[i]));

			//connect the nodes with the least frequency
			int first = literals.Count - 1, second = literals.Count - 2;

			//Loop until the current root does not span the whole text
			while (root == null || root.freq != totalLength)
			{
				Tree newTreeNode = new Tree(false, 0, nodes[first].freq + nodes[second].freq);
				newTreeNode.right = nodes[first]; //set parents
				newTreeNode.left = nodes[second];
				//newTreeNode.leaf = false;
				int pos = 0;//check where to push the node
				while (nodes[pos].freq > newTreeNode.freq)
					pos++;
				nodes.Insert(pos, newTreeNode); //put it there.
				first--; //set next leaves.
				second--;
				root = newTreeNode;
				if (first == 0 && totalLength == 0)
					totalLength = root.freq;
			}
		}

		/// <summary>
		/// Write decoding tree to a BinaryWriter
		/// </summary>
		/// <param name="myWrite"></param>
		private void writeTree(BinaryWriter myWrite)
		{
			myWrite.Write(literals.Count);

			for (int i = 0; i < literals.Count; i++)
			{
				myWrite.Write(literals[i]);
				myWrite.Write(frequency[i]);
			}
		}

		/// <summary>
		/// Count Frequencies and mapping from ASCII code of the letter
		/// to it's position in the frequency counter
		/// </summary>
		/// <param name="text">Text (binary text) to decode</param>
		private void buildLiteralTable(byte[] text)
		{
			for (int i = 0; i < 256; i++) //reset the mapping
				mapper[i] = -1;
			foreach (var let in text)
			{
				if (mapper[let] != -1)
				{
					frequency[mapper[let]]++;
				}
				else
				{
					literals.Add(let);
					mapper[let] = literals.Count - 1;
					frequency.Add(1);
				}
			}
			//bubble sort the frequency list
			for (int i = literals.Count - 1; i > 1; i--)
			{
				int mini = i;
				for (int j = 0; j < i - 1; j++)
					if (frequency[j] < frequency[mini])
					{
						mini = j;
					}
				if (mini != i)
				{
					mapper[literals[i]] = mini;
					mapper[literals[mini]] = i;
					int temp = frequency[i];
					frequency[i] = frequency[mini];
					frequency[mini] = temp;
					byte temp2 = literals[i];
					literals[i] = literals[mini];
					literals[mini] = temp2;
				}
			}
			codes = new List<List<byte>>(literals.Count);
		}
		//in Order walk of the tree and write the codes on the leafs
		//used to decode -> walk the tree (according to stream), when you reach a leaf, take the letter
		//and return to the root.
		private void inOrder(Tree node, List<byte> until)
		{
			if (node == null)
				return;
			until.Add(0);
			inOrder(node.left, until);
			until.RemoveAt(until.Count - 1);
			if (node.leaf)
			{
				int index = mapper[node.letter];
				while (codes.Count - 1 < index) codes.Add(new List<byte>());
				codes[index] = new List<byte>(until); //save a list of the codes
			}
			until.Add(1);
			inOrder(node.right, until);
			until.RemoveAt(until.Count - 1);
		}

		//internal method
		private byte[] readAsIs(BinaryReader myReader)
		{
			int realSize = myReader.ReadInt32();
			byte[] text = new byte[realSize];
			return myReader.ReadBytes(realSize);
		}

		//read the encoding tree from a binary reader
		private void readTree(BinaryReader myReader)
		{
			int literalsCount = myReader.ReadInt32();

			literals = new List<byte>(literalsCount);
			frequency = new List<int>(literalsCount);
			//build tables from file
			for (int i = 0; i < literalsCount; i++)
			{
				literals.Add(myReader.ReadByte());
				frequency.Add(myReader.ReadInt32());
			}
			//build encoding
			buildEncoding(totalLength);
		}

		/// <summary>
		/// Decode from binary reader to a file name.
		/// </summary>
		/// <param name="myReader">Source</param>
		/// <param name="fileName2">Target</param>
		/// <returns>UnPacked size</returns>
		public long DecodeText(BinaryReader myReader, string fileName2)
		{
			int size;
			byte[] text2 = DecodeText(myReader, out size);
			if (text2 != null)
				File.WriteAllBytes(fileName2, text2);
			return size;
		}

		/// <summary>
		/// Decode from binary reader, and return a byte array.
		/// </summary>
		/// <param name="myReader">Source</param>
		/// <param name="size">Out parameter, Unpacked size</param>
		/// <returns>Decoded byte array.</returns>
		public byte[] DecodeText(BinaryReader myReader, out int size)
		{
			byte style = myReader.ReadByte();
			byte[] text2;
			if (style == 1)
			{ //check for different versions of encoding
				text2 = readAsIs(myReader); //if it's style 1
				size = text2.Length;  //just read the file - it's not encoded.
				return text2;
			}
			//read tree
			readTree(myReader);

			ulong buffer = 0; //build a buffer - where going to shift bits out of it
			int lengthOfText, posInText = 0;
			Tree node = root;
			lengthOfText = myReader.ReadInt32(); //read int - length of text to decode
			text2 = new byte[lengthOfText];
			do
			{
				try
				{
					buffer = myReader.ReadUInt64();
				}
				catch
				{
					break;
				}
				int rle;
				if (buffer == ulong.MaxValue)
				{ //if it's Run length encoded
					rle = myReader.ReadInt32(); //read the repeater count
					buffer = myReader.ReadUInt64(); //and the input
				}
				else
					rle = 0; //no repeating
				for (int rleRunner = 0; rleRunner <= rle; rleRunner++) //repeat this process
					for (ulong i = 1; i != 0; i = i << 1)
					{ //move through 1 to 2^63

						node = (buffer & i) == i ? node.right : node.left; //if the bit is 1 go right on the decoding tree else go left

						if (node.leaf)
						{ //if it's a leaf, take the letter, and go the root.
							text2[posInText++] = node.letter;
							if (posInText == text2.Length) break;
							node = root;
						}
					}
			}
			while (posInText < text2.Length);

			size = text2.Length;
			return text2;
		}

		/// <summary>
		/// Decode To a stream
		/// </summary>
		/// <param name="myReader">Source</param>
		/// <returns>Target Stream</returns>
		public Stream DecodeText(BinaryReader myReader)
		{
			int size;
			byte[] text2 = DecodeText(myReader, out size);
			return new MemoryStream(text2, false);
		}

		/// <summary>
		/// Decode from file to file
		/// </summary>
		/// <param name="fileName">Source</param>
		/// <param name="fileName2">Target</param>
		/// <returns>UnPacked Size</returns>
		public long DecodeText(string fileName, string fileName2)
		{
			using (BinaryReader myReader = new BinaryReader(
				File.Open(fileName, FileMode.Open, FileAccess.Read)))
			{
				return DecodeText(myReader, fileName2);
			}
		}

		/// <summary>
		/// Encode byte array from file name into a binary writer.
		/// </summary>
		/// <param name="fileName">Source file name</param>
		/// <param name="myWrite">Target writer</param>
		/// <param name="fileSize">Source file size</param>
		/// <returns>Packed size</returns>
		public long EncodeByteArray(string fileName, BinaryWriter myWrite, out int fileSize)
		{
			//try
			//{
			Stream reader = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
			totalLength = (int)reader.Length;
			long actualSize = 0;
			bool firstChunk = true;
			//BinaryReader myReader = new BinaryReader(
			do
			{
				int toRead = (int)(reader.Length - reader.Position);
				int length = (toRead > 180000000) ? 180000000 : toRead; //take chunks of this size
				byte[] text = new byte[length];
				fileSize = reader.Read(text, 0, length); //read 
				if (fileSize < 400 && firstChunk)
				{ //smaller then 400, don't bother encoding.
					byte style = 1;
					myWrite.Write(style);
					myWrite.Write((int)fileSize);
					if (fileSize > 0)
						myWrite.Write(text);
					return fileSize;
				}
				else
				{
					actualSize += EncodeByteArray(text, myWrite, firstChunk); //encode and add
					firstChunk = false;
				}
			}
			while (reader.Position < reader.Length);
			reader.Close();
			return actualSize;
		}

		/// <summary>
		/// Encode from byte array to a writer.
		/// </summary>
		/// <param name="text">Source</param>
		/// <param name="myWrite">Target</param>
		/// <param name="firstChunk">Is it the first chunk of a pack</param>
		/// <returns>Size</returns>
		public long EncodeByteArray(byte[] text, BinaryWriter myWrite, bool firstChunk)
		{
			if (firstChunk)
			{ //If it's first chunk 
				buildLiteralTable(text); //prepare for encoding
				buildEncoding(text.Length);  //it's not adaptive - tree is taken
				inOrder(root, new List<byte>()); //from first chunk.
			}
			ulong buffer = 0, prevBuffer = 0;
			int pos = 0, rle = 0;
			long startPos = myWrite.BaseStream.Position;
			bool initial = true;
			byte style = 0;
			myWrite.Write(style);
			writeTree(myWrite); //write tree

			long anotherPos = myWrite.BaseStream.Position;
			if (firstChunk)
				myWrite.Write(totalLength);

			var DataPos = myWrite.BaseStream.Position;
			for (int i = 0; i < text.Length; i++)
			{
				var k = codes[mapper[text[i]]]; //get code for current char
				if ((i + 1) % 500000 == 0)
				{ //Update progress indicator
					progress = (int)(((float)i / (float)text.Length) * 100);
				}

				for (int l = 0; l < k.Count; l++)
				{
					buffer = buffer | ((ulong)k[l] << pos);//shift bits into the buffer
					if (++pos == 64)
					{
						//When the buffer is full, flush it.
						if (prevBuffer != buffer || initial)
						{
							if (rle < 2 && prevBuffer != ulong.MaxValue)
							{
								if (!initial)
									for (int d = 0; d <= rle; d++)
										myWrite.Write(prevBuffer);
							}
							else
							{
								myWrite.Write(ulong.MaxValue);
								myWrite.Write(rle);
								myWrite.Write(prevBuffer);
							}
							rle = 0;
						}
						else
						{
							rle++;
						}
						prevBuffer = buffer;
						initial = false;
						buffer = 0;
						pos = 0;
					}
				}
			}//all the lines

			///if not repeating and the buffer is not <code>ulong.MaxValue</code>
			if (rle < 2 && prevBuffer != ulong.MaxValue)
			{
				if (!initial) //just write it.
					for (int d = 0; d <= rle; d++)
						myWrite.Write(prevBuffer);
			}
			else
			{
				myWrite.Write(ulong.MaxValue); //write the repeating marker
				myWrite.Write(rle); //time
				myWrite.Write(prevBuffer); //what to repeat.
			}
			if (pos != 0)
				myWrite.Write(buffer);

			long size = myWrite.BaseStream.Position - DataPos;
			return size;
		}

		/// <summary>
		/// Encode from byte array to a file.
		/// </summary>
		/// <param name="text">Source</param>
		/// <param name="fileName">Target</param>
		/// <returns>Packed Size</returns>
		public long EncodeByteArray(byte[] text, string fileName)
		{
			using (BinaryWriter myWrite = new BinaryWriter(
			File.Open(fileName, FileMode.Create, FileAccess.Write)))
			{
				totalLength = (int)text.Length;
				return (EncodeByteArray(text, myWrite, true));
			}


		}
		#endregion Methods
	}
}
