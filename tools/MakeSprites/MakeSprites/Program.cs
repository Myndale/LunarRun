using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeSprites
{
	class Program
	{
		static Bitmap Source;
		static int Width, Height, Span;
		static int[] Pixels;
		static int[] Mask;
		static int[] Image1;
		static int[] Image2;

		static void Main(string[] args)
		{
			try
			{
				CheckArgs(args);
				LoadBitmap(args[0]);
				ExtractImage();
				ConvertImage();
				ExportImage();
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
				throw new Exception("Usage: MakeSprites sprite_file.png");
		}

		static void LoadBitmap(string filename)
		{
			if (!File.Exists(filename))
				throw new Exception("Input file doesn't exist.");

			try { Source = new Bitmap(filename); }
			catch { throw new Exception("Couldn't load image, please specify a valid image file."); }

			Width = Source.Width;
			Height = Source.Height;
		}

		static void ExtractImage()
		{
			var rect = Rectangle.FromLTRB(0, 0, Width, Height);
			var bitmapData = Source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
			Pixels = new int[Width * Height];
			Marshal.Copy(bitmapData.Scan0, Pixels, 0, Pixels.Length);
			Source.UnlockBits(bitmapData);
		}

		static void ConvertImage()
		{
			Span = (Width + 7) / 8;
			Mask = new int[Span * Height];
			Image1 = new int[Span * Height];
			Image2 = new int[Span * Height];
			for (int y=0; y<Height; y++)
				for (int x=0; x<Width; x++)
				{
					int ofs = y * Span + x / 8;
					int bit = 0x80 >> (x & 7);
					var pixel = Pixels[y * Width + x];
					if ((pixel & 0xff000000) != 0)
					{
						Mask[ofs] |= bit;
						if ((pixel & 0xffffff) == 0)
						{
							// black
							Image1[ofs] |= bit;
							Image2[ofs] |= bit;
						}
						else if ((pixel & 0xffffff) != 0xffffff)
						{
							// gray
							Image1[ofs] |= bit;
						}
					}
				}
		}

		static void ExportImage()
		{
			const int bytes_per_line = 32;

			// mask
			Console.WriteLine("const byte mask[] PROGMEM = {");
			Console.WriteLine(String.Format("  {0}, {1},", Span*8, Height));
			for (int i = 0; i < Mask.Count(); i++)
			{
				if (i % bytes_per_line == 0)
					Console.Write("  ");
				Console.Write(String.Format("0x{0:X2}", Mask[i]));
				if (i != Mask.Length - 1)
					Console.Write(",");
				if (i % bytes_per_line == bytes_per_line - 1)
					Console.WriteLine();
			}
			if (Mask.Count() % bytes_per_line != 0)
				Console.WriteLine();
			Console.WriteLine("};");
			Console.WriteLine();
			Console.WriteLine();

			// image 1
			Console.WriteLine("const byte image1[] PROGMEM = {");
			Console.WriteLine(String.Format("  {0}, {1},", Span * 8, Height));
			for (int i = 0; i < Image1.Count(); i++)
			{
				if (i % bytes_per_line == 0)
					Console.Write("  ");
				Console.Write(String.Format("0x{0:X2}", Image1[i]));
				if (i != Image1.Length - 1)
					Console.Write(",");
				if (i % bytes_per_line == bytes_per_line - 1)
					Console.WriteLine();
			}
			if (Image1.Count() % bytes_per_line != 0)
				Console.WriteLine();
			Console.WriteLine("};");
			Console.WriteLine();
			Console.WriteLine();

			// image 2
			Console.WriteLine("const byte image2[] PROGMEM = {");
			Console.WriteLine(String.Format("  {0}, {1},", Span * 8, Height));
			for (int i = 0; i < Image2.Count(); i++)
			{
				if (i % bytes_per_line == 0)
					Console.Write("  ");
				Console.Write(String.Format("0x{0:X2}", Image2[i]));
				if (i != Image2.Length - 1)
					Console.Write(",");
				if (i % bytes_per_line == bytes_per_line - 1)
					Console.WriteLine();
			}
			if (Image2.Count() % bytes_per_line != 0)
				Console.WriteLine();
			Console.WriteLine("};");
			Console.WriteLine();
			Console.WriteLine();
		}
	}
}
