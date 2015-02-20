using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMap
{
	struct MapLevel
	{
		public int column, row, offset;
	}

	class Program
	{
		static string level_name;
		static Bitmap Source;
		static int Width, Height, Columns, Rows;
		static int[] Pixels;
		static List<int>[][] Spans;
		static List<Point>[][] Pads;

		static int whites = 0;
		static int blacks = 0;
		static int recurses = 0;
		static int raws = 0;

		static void Main(string[] args)
		{
			var test = Wheeler("^BANANA|");
			try
			{
				CheckArgs(args);
				LoadBitmap(args[0]);
				ExtractImage();
				//CompressImage();
				//CompressImage2();
				CompressImage3();
				//ExportTerrain();
				//CompressImage4();
				ExportLevels(args[1]);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
#if DEBUG
				Console.ReadKey();
#endif
			}
		}

		static void CheckArgs(string[] args)
		{
			if (args.Length != 2)
				throw new Exception("Usage: MakeMap terrain_image.png");
		}
		
		static void LoadBitmap(string filename)
		{
			if (!File.Exists(filename))
				throw new Exception("Input file doesn't exist.");

			level_name = Path.GetFileNameWithoutExtension(filename).ToLower();
			try {Source = new Bitmap(filename);}
			catch {throw new Exception("Couldn't load image, please specify a valid image file.");}

			Width = Source.Width;
			Height = Source.Height;
			if (Width % 84 != 0)
				throw new Exception("Width must be a multiple of 84.");
			if (Height % 48 != 0)
				throw new Exception("Height must be a multiple of 48.");

			Columns = Width / 84;
			Rows = Height / 48;
		}

		static void ExtractImage()
		{
			var rect = Rectangle.FromLTRB(0, 0, Width, Height);
			var bitmapData = Source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
			Pixels = new int[Width * Height];
			Marshal.Copy(bitmapData.Scan0, Pixels, 0, Pixels.Length);
			Source.UnlockBits(bitmapData);
			for (int i = 0; i < Pixels.Length; i++)
				Pixels[i] &= 0xffffff;
		}

		static void CompressImage()
		{
			Spans = new List<int>[Rows][];
			Pads = new List<Point>[Rows][];
			for (int row = 0; row < Rows; row++)
			{
				Spans[row] = new List<int>[Columns];
				Pads[row] = new List<Point>[Columns];
				for (int column = 0; column < Columns; column++)
				{
					var spans = Spans[row][column] = new List<int>();
					var pads = Pads[row][column] = new List<Point>();
					var last = 0;
					var span_length = 0;
					bool hasWhite = false;
					bool hasBlack = false;
					for (int x = 0; x < 84; x++)
					{
						for (int y = 0; y < 48; y++)
						{
							var pixel = Pixels[(row*48+y) * Width + (column*84+x)];

							// check for a pad
							if ((pixel != 0x000000) && (pixel != 0xffffff))
							{
								// pad
								pads.Add(new Point(x, y));

								// set pad pixels to background
								for (int i = 0; i < 5; i++)
									Pixels[(row * 48 + y) * Width + (column * 84 + x + i)] = 0xffffff;
							}

							var current =  (pixel== 0) ? 1 : 0;
							hasWhite |= (current == 0);
							hasBlack |= (current == 1);
							if (current != last)
							{
								spans.Add(span_length);
								last = 1 - last;
								span_length = 0;
							}
							span_length++;

							if (span_length == 63)
							{
								spans.Add(span_length);
								last = 1 - last;
								span_length = 0;
							}
						}
					}
					spans.Add(span_length);

					while (spans.Count() % 4 != 0)
						spans.Add(0);

					// don't store anything if the section is a constant color, it'll just waste memory
					if (!hasWhite || !hasBlack)
						spans.Clear();
				}
			}
		}

		const int BIN_BITS = 6;
		const int NUM_BINS = 64 - 2;

		static void CompressImage2()
		{
			int numBits = 0;
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{					
					// convert image to an array of pixels
					List<int> pixels = new List<int>();
					for (int y = 0; y < 48; y++)
					{
						for (int x = 0; x < 84; x++)
						{
							var pixel = Pixels[(row * 48 + y) * Width + (column * 84 + x)];
							var current = (pixel == 0) ? 1 : 0;
							pixels.Add(current);
						}
					}

					var bins = new List<List<int>>();
					var encoded = new List<int>();

					int pos = 0;
					while (pos < pixels.Count())
					{
						List<int> newBin;						
						int longestIndex = FindLongestBin(bins, pixels, pos);
						if (longestIndex == -1)
						{
							encoded.Add(pixels[pos]);
							newBin = new List<int> { pixels[pos++] };
							numBits += BIN_BITS;
						}
						else
						{
							var longestBin = bins[longestIndex];
							newBin = longestBin.ToList();
							pos += longestBin.Count();
							if (pos < pixels.Count())
								encoded.Add(longestIndex * 2 + pixels[pos] + 2);
							else
								encoded.Add(longestIndex * 2 + 2);
							if (pos < pixels.Count())
								newBin.Add(pixels[pos++]);
							numBits += BIN_BITS + 1;
						}

						// add this value and add the bin to our queue
						bins.Add(newBin);

						// if we have too many bins then get rid of the head
						if (bins.Count() > NUM_BINS)
							bins.RemoveAt(0);
					}

					// decompress and test, just to make sure compression is working properly
					var decoded = Decompress(encoded); // might wind up a bit larger but that doesn't matter, we can fix that at run-time
					for (int i=0; i<pixels.Count(); i++)
					{
						Debug.Assert(decoded[i] == pixels[i]);
					}
				}
			}
		}

		static void CompressImage3()
		{
			List<byte> allBytes = new List<byte>();

			int total_size = 0;
			int max_level_size = 0;
			int num_levels = 0;
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					var screen = new byte[504];
					for (int x = 0, ofs=0; x < 84; x++)
					{
						for (int y = 0; y < 48; y++, ofs++)
						{
							var pixel = Pixels[(row * 48 + y) * Width + (column * 84 + x)];
							if (pixel != 0xffffff)
								screen[ofs/8] = (byte)(screen[ofs/8] | (1 << (ofs%8)));
						}
					}
					if (allWhite(screen))
						continue;
					if (allBlack(screen))
						continue;
					num_levels++;
					for (int i = 0; i < screen.Count(); i++)
						allBytes.Add(screen[i]);
					int size = CompressScreen2(screen);
					total_size += size;
					max_level_size = Math.Max(max_level_size, size);
				}
			}

			var all = allBytes.ToArray();
			File.WriteAllBytes(@"c:\temp\test1.dat", all);
			total_size = CompressScreen2(all);
			all = Wheeler(all);
			File.WriteAllBytes(@"c:\temp\test2.dat", all);
			var total_size2 = CompressScreen2(all);

			Console.WriteLine(String.Format("Num levels: {0}", num_levels));
			Console.WriteLine(String.Format("Max size:   {0}", max_level_size / 8));
			Console.WriteLine(String.Format("Total size: {0}", total_size / 8));
			Console.WriteLine(String.Format("Whites:     {0}", whites));
			Console.WriteLine(String.Format("Blacks:     {0}", blacks));
			Console.WriteLine(String.Format("Raws:       {0}", raws));
			Console.WriteLine(String.Format("Recurses:   {0}", recurses));
		}

		static byte[] Wheeler(byte[] screen)
		{
			// create all rotations
			List<int> rotations = new List<int>();
			for (int i = 0; i < screen.Length; i++)
				rotations.Add(i);

			// sort them
			Comparison<int> comparer = new Comparison<int>((diff1, diff2) =>
			{
				for (int i = 0; i < screen.Length; i++)
				{
					var ch1 = screen[(i + diff1) % screen.Length];
					var ch2 = screen[(i + diff2) % screen.Length];
					if (ch1 < ch2)
						return -1;
					else if (ch1 > ch2)
						return 1;
				}
				return 0;
			});
			rotations.Sort(comparer);

			// construct result and return
			byte[] result = new byte[screen.Length];
			for (int i = 0; i < rotations.Count(); i++)
			{
				var r = rotations[i];
				result[i] = screen[(r + screen.Length - 1) % screen.Length];
			}
			return result;
		}

		static string Wheeler(string screen)
		{
			// create all rotations
			List<int> rotations = new List<int>();
			for (int i = 0; i < screen.Length; i++)
				rotations.Add(i);

			// sort them
			Comparison<int> comparer = new Comparison<int>((diff1, diff2) =>
			{
				for (int i = 0; i < screen.Length; i++)
				{
					var ch1 = screen[(i+diff1)%screen.Length];
					var ch2 = screen[(i+diff2)%screen.Length];
					if (ch1 < ch2)
						return -1;
					else if (ch1 > ch2)
						return 1;
				}
				return 0;
			});
			rotations.Sort(comparer);

			// construct result and return
			string result = "";
			for (int i = 0; i < rotations.Count(); i++)
			{
				var r = rotations[i];
				result += screen[(r + screen.Length-1) % screen.Length];
			}
			return result;
		}

		static void CompressImage4()
		{
			int total_size = 0;
			int max_level_size = 0;
			int num_levels = 0;

			// find tree for most compressed level
			int smallest_size = 1000;
			HuffmanTree smallestHuffmanTree = new HuffmanTree();
			for (int row = 0; row < Rows; row++)
			{
				for (int column = 0; column < Columns; column++)
				{
					int ofs = 0;
					var screen = new byte[84*48/8];
					for (int x = 0; x < 84; x++)
					{
						for (int y = 0; y < 48; y += 8)
						{
							for (int dy = 0; dy < 8; dy++)
							{
								var pixel = Pixels[(row * 48 + (y + dy)) * Width + (column * 84 + x)];
								if (pixel != 0xffffff)
									screen[ofs] = (byte)(screen[ofs] | (1 << dy));
							}
							ofs++;
						}
					}

					if (allWhite(screen.ToArray()))
						continue;
					if (allBlack(screen.ToArray()))
						continue;

					string input = new String(screen.Select(c => (char)c).ToArray());

					/*
					HuffmanTree huffmanTree = new HuffmanTree();
					huffmanTree.Build(input);
					BitArray encoded = huffmanTree.Encode(input);
					int size = encoded.Length;
					 * */

					HuffManEncoder encoder = new HuffManEncoder();
					int size = (int)encoder.EncodeByteArray(screen, "test.dat");

					total_size += size;
					max_level_size = Math.Max(max_level_size, size);

					if (size < smallest_size)
					{
						smallest_size = size;
						//smallestHuffmanTree = huffmanTree;
					}
				}
			}

			Console.WriteLine(String.Format("Num levels: {0}", num_levels));
			Console.WriteLine(String.Format("Max size:   {0}", max_level_size));
			Console.WriteLine(String.Format("Total size: {0}", total_size));
			Console.WriteLine(String.Format("Whites:     {0}", whites));
			Console.WriteLine(String.Format("Blacks:     {0}", blacks));
			Console.WriteLine(String.Format("Raws:       {0}", raws));
			Console.WriteLine(String.Format("Recurses:   {0}", recurses));
		}


		static int CompressScreen(int[] screen, int pos, int length)
		{
			if (length==1)
				return 1;

			// check if I'm all white
			if (allWhite(screen, pos, length))
			{
				whites++;
				return 2;
			}

			// check if I'm all black
			if (allBlack(screen, pos, length))
			{
				blacks++;
				return 3;
			}

			int half_length = length / 2;
			int recurse = CompressScreen(screen, pos, half_length) + CompressScreen(screen, pos + half_length, half_length);
			if (recurse < length)
			{
				recurses++;
				return 1 + recurse;
			}
			else
			{
				raws++;
				return 3 + length;
			}
		}

		const int SPAN_BITS = 10;
		const int MAX_SPAN = (1<<SPAN_BITS) - 256;
		const int LENGTH_BITS = 5;
		const int MAX_LENGTH = 1 << LENGTH_BITS;

		static int CompressScreen2(byte[] screen)
		{
			int size = 0;
			int pos = 0;
			while (pos < screen.Length)
			{
				int offset;
				int length = getLongestSpan(screen, pos, out offset);

				for (int i = 0; i < length; i++)
					Debug.Assert(screen[offset + i] == screen[pos + i]);

				pos += length;
				if (length == 0)
					pos++;
				size += SPAN_BITS;
				if (length > 0)
					size += LENGTH_BITS;
			}

			return size;
		}

		static int getLongestSpan(byte[] screen, int pos, out int offset)
		{
			offset = -1;
			int start = Math.Max(0, pos - MAX_SPAN);
			int longest_span = 0;
			while (start < pos)
			{
				int this_len = getSpanLength(screen, start, pos);
				if (this_len > longest_span)
				{
					longest_span = this_len;
					offset = start;
				}
				
				start++;
			}
			return longest_span;
		}

		static int getSpanLength(byte[] screen, int start, int pos)
		{
			int len = 0;
			while ((pos < screen.Length) && (len<MAX_LENGTH) && (screen[start] == screen[pos]))
			{
				len++;
				start++;
				pos++;
			}
			return len;
		}

		static bool allWhite(int[] screen, int pos, int length)
		{
			for (int i = 0; i < length; i++)
				if (screen[pos + i] == 1)
					return false;
			return true;
		}

		static bool allBlack(int[] screen, int pos, int length)
		{
			for (int i = 0; i < length; i++)
				if (screen[pos + i] == 0)
					return false;
			return true;
		}

		static bool allWhite(byte[] screen)
		{
			for (int i = 0; i < screen.Length; i++)
				if (screen[i] != 0x00)
					return false;
			return true;
		}

		static bool allBlack(byte[] screen)
		{
			for (int i = 0; i < screen.Length; i++)
				if (screen[i] != 0xff)
					return false;
			return true;
		}

		static List<int> Decompress(List<int> encoded)
		{
			var decoded = new List<int>();
			var bins = new List<List<int>>();
			for (int i = 0; i < encoded.Count(); i++)
			{
				int index = encoded[i];
				List<int> newBin;
				if (index == 0)
					newBin = new List<int> { 0 };
				else if (index == 1)
					newBin = new List<int> { 1 };
				else
				{
					index -= 2;
					int binNum = index >> 1;
					int pixel = index & 1;
					newBin = bins[binNum].ToList();
					newBin.Add(pixel);
				}
				bins.Add(newBin);
				for (int j = 0; j < newBin.Count(); j++)
					decoded.Add(newBin[j]);
				if (bins.Count() > NUM_BINS)
					bins.RemoveAt(0);
			}
			return decoded;
		}

		static int FindLongestBin(List<List<int>> bins, List<int> pixels, int pos)
		{
			int longestIndex = -1;
			for (int binNum=0; binNum<bins.Count(); binNum++)
			{
				if (BinMatch(bins[binNum], pixels, pos))
				{
					if ((longestIndex == -1) || (bins[binNum].Count() > bins[longestIndex].Count()))
						longestIndex = binNum;
				}
			}
			return longestIndex;
		}

		static bool BinMatch(List<int> bin, List<int> pixels, int pos)
		{
			for (int i=0; (i<bin.Count()) && (pos < pixels.Count()); i++, pos++)
				if (pixels[pos] != bin[i])
					return false;
			return true;
		}

		static void ExportTerrain()
		{
			// export each screen
			int total_bytes = 0;
			for (int row=0; row<Rows; row++)
				for (int column = 0; column < Columns; column++)
				{
					var spans = Spans[row][column];
					if (spans.Count == 0)
						continue;
					List<int> bytes = new List<int>();
					for (int i = 0; i < spans.Count(); i += 4)
					{
						int span1 = spans[i];
						int span2 = spans[i + 1];
						int span3 = spans[i + 2];
						int span4 = spans[i + 3];
						span1 |= (span4 & 3) << 6;
						span2 |= ((span4 >> 2) & 3) << 6;
						span3 |= ((span4 >> 4) & 3) << 6;
						bytes.Add(span1);
						bytes.Add(span2);
						bytes.Add(span3);
					}
					total_bytes += bytes.Count();

					Console.WriteLine(String.Format("// {0} bytes", bytes.Count()));
					Console.WriteLine("const byte " + level_name + "_" + column + "_" + row + "[] PROGMEM = {");
					const int bytes_per_line = 32;
					for (int i = 0; i < bytes.Count(); i++)
					{
						if (i % bytes_per_line == 0)
							Console.Write("  ");
						Console.Write(String.Format("0x{0:X2}", bytes[i]));
						if (i != bytes.Count - 1)
							Console.Write(",");
						if (i % bytes_per_line == bytes_per_line - 1)
							Console.WriteLine();
					}
					if (bytes.Count() % bytes_per_line != 0)
						Console.WriteLine();
					Console.WriteLine("};");
					Console.WriteLine();
					Console.WriteLine();
				}

			// export the screen index
			Console.WriteLine();
			Console.WriteLine(String.Format("// total landscape data: {0} bytes", total_bytes));
			Console.WriteLine("const byte * " + level_name + "_sections[] PROGMEM = {");
			for (int row=0; row<Rows; row++)
			{
				Console.Write("  ");
				for (int column=0; column<Columns; column++)
				{
					var spans = Spans[row][column];
					var str = (spans.Count > 0)
						? String.Format("{0}_{1}_{2}", level_name, column, row)
						: "NULL";
					if ((column < Columns-1) || (row < Rows-1))
						str += ",";
					str = str.PadRight(level_name.Length + 8);
					Console.Write(str);					
				}
				if (row != Rows - 1)
					Console.WriteLine();
			}
			Console.WriteLine();
			Console.WriteLine("};");
			Console.WriteLine();

			Console.WriteLine("level " + level_name + " PROGMEM = {" + Columns.ToString() + ", " + Rows.ToString() + ", " + level_name + "_sections, 10.0f, 10.0f};");

			/*
			// export the pads
			int maxPads = 0;
			for (int y=0; y<Rows; y++)
				for (int x=0; x<Columns; x++)
					maxPads = Math.Max(maxPads, Pads[y][x].Count());
			Console.WriteLine(String.Format("#define MAX_PADS {0}", maxPads));
			Console.WriteLine("const byte pads[NUM_ROWS][NUM_COLUMNS][MAX_PADS][2] PROGMEM = {");
			for (int y = 0; y < Rows; y++)
			{
				Console.WriteLine("  {");
				for (int x = 0; x < Columns; x++)
				{					
					Console.Write("    {");

					var pads = Pads[y][x];
					for (int i = 0; i < maxPads; i++)
					{
						if (i < pads.Count())
							Console.Write("{" + String.Format("{0,3},{1,3}", pads[i].X, pads[i].Y) + "}");
						else
							Console.Write("{  0,  0}");
						if (i < maxPads-1)
							Console.Write(", ");
					}
					Console.Write("}");
					if (x != Columns - 1)
						Console.Write(", ");
					else
						Console.Write("  ");
					Console.WriteLine("// row {0}, column {1}", y, x);
				}
				Console.Write("  }");
				if (y != Rows-1)
					Console.Write(",");
				Console.WriteLine();
			}
			Console.WriteLine("};");
			 * */
		}

		static void ExportLevels(string filename)
		{
			var levels = new List<MapLevel>();
			int offset = 0;
			int total_size = 0;

			// find tree for most compressed level
			using (var file = File.Create(filename))
			{
				for (int row = 0; row < Rows; row++)
				{
					for (int column = 0; column < Columns; column++)
					{
						var screen = new byte[84 * 48 / 8];

						for (int y = 0, ofs = 0; y < 48; y += 8)
							for (int x = 0; x < 84; x++, ofs++)
								for (int dy = 0; dy < 8; dy++)
								{
									var pixel = Pixels[(row * 48 + (y + dy)) * Width + (column * 84 + x)];
									if (pixel != 0xffffff)
										screen[ofs] = (byte)(screen[ofs] | (1 << dy));
								}

						if (allWhite(screen.ToArray()))
							continue;
						if (allBlack(screen.ToArray()))
							continue;

						var mapLevel = new MapLevel { column = column, row = row, offset = levels.Count() };
						levels.Add(mapLevel);

						file.Write(screen, 0, screen.Length);
						total_size += screen.Length;
					}
				}
			}

			Console.WriteLine(String.Format("// total levels = {0}", levels.Count()));
			Console.WriteLine("const unsigned level_offsets[] PROGMEM = {");
			Console.Write("\t");
			for (int i=0; i<levels.Count; i++)
			{
				var level = levels[i];
				Console.Write(String.Format("0x{0:x4}", (int)level.row * 256 + level.column) + ", ");
				if (i % 16 == 15)
				{
					Console.WriteLine();
					Console.Write("\t");
				}
			}
			Console.WriteLine();
			Console.WriteLine("};");
		}

	}
}
