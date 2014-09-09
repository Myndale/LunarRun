using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMap
{
	class Program
	{
		static Bitmap Source;
		static int Width, Height, Columns, Rows;
		static int[] Pixels;
		static List<int>[][] Spans;

		static void Main(string[] args)
		{
			try
			{
				CheckArgs(args);
				LoadBitmap(args[0]);
				ExtractImage();
				CompressImage();
				ExportTerrain();
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
			if (args.Length != 1)
				throw new Exception("Usage: MakeMap terrain.bmp");
		}
		
		static void LoadBitmap(string filename)
		{
			if (!File.Exists(filename))
				throw new Exception("Input file doesn't exist.");

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
			for (int row = 0; row < Rows; row++)
			{
				Spans[row] = new List<int>[Columns];
				for (int column = 0; column < Columns; column++)
				{
					var spans = Spans[row][column] = new List<int>();
					var last = 0;
					var span_length = 0;
					for (int x = 0; x < 84; x++)
					{
						for (int y = 0; y < 48; y++)
						{
							var current = Pixels[(row*48+y) * Width + (column*84+x)] == 0 ? 1 : 0;
							//var current = Pixels[y * Width + x] == 0 ? 1 : 0;
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
				}
			}
		}

		static void ExportTerrain()
		{
			// export each screen
			int total_bytes = 0;
			for (int row=0; row<Rows; row++)
				for (int column = 0; column < Columns; column++)
				{
					var spans = Spans[row][column];
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
					Console.WriteLine("const byte landscape_" + column + "_" + row + "[] PROGMEM = {");
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
			Console.WriteLine(String.Format("#define NUM_COLUMNS {0}", Columns));
			Console.WriteLine(String.Format("#define NUM_ROWS {0}", Rows));
			Console.WriteLine();
			Console.WriteLine(String.Format("// total landscape data: {0} bytes", total_bytes));
			Console.WriteLine("const byte * landscapes[NUM_ROWS][NUM_COLUMNS] PROGMEM = {");
			for (int row=0; row<Rows; row++)
			{
				Console.Write("  {");
				for (int column=0; column<Columns; column++)
				{
					Console.Write(String.Format("landscape_{0}_{1}", column, row));
					if (column < Columns - 1)
						Console.Write(",");
					else
					{
						Console.Write("}");
						if (row < Rows-1)
							Console.Write(",");
						Console.WriteLine();
					}
				}
			}
			Console.WriteLine("};");
		}
	}
}
