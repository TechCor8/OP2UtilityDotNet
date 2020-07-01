using OP2UtilityDotNet.Bitmap;
using OP2UtilityDotNet.Streams;
using System;
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class OP2BmpLoader : IDisposable
	{
		public OP2BmpLoader(string bmpFilename, string artFilename)
		{
			FileStream fs = new FileStream(bmpFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
			bmpReader = new BinaryReader(fs);
			
			artFile = ArtFile.Read(artFilename);
		}

		public void ExtractImage(int index, string filenameOut)
		{
			artFile.VerifyImageIndexInBounds(index);

			ImageMeta imageMeta = artFile.imageMetas[index];

			Color[] palette = new Color[artFile.palettes[imageMeta.paletteIndex].colors.Length];
			System.Array.Copy(artFile.palettes[imageMeta.paletteIndex].colors, palette, palette.Length);

			uint pixelOffset = (uint)(imageMeta.pixelDataOffset + 14 + ImageHeader.SizeInBytes + palette.Length * Color.SizeInBytes);

			SliceStream pixels = GetPixels(pixelOffset, imageMeta.scanLineByteWidth * imageMeta.height);

			byte[] pixelContainer = new byte[imageMeta.scanLineByteWidth * imageMeta.height];
			pixels.Read(pixelContainer, 0, pixelContainer.Length);

			// Outpost 2 stores pixels in normal raster scan order (top-down). This requires a negative height for BMP file format.
			if (imageMeta.height > uint.MaxValue) {
				throw new System.Exception("Image height is too large to fit in standard bitmap file format.");
			}

			BitmapFile.WriteIndexed(filenameOut, imageMeta.GetBitCount(), (int)imageMeta.width, -(int)imageMeta.height, palette, pixelContainer);
		}

		// Bmp loader for Outpost 2 specific BMP file
		// Contains many images in pixels section with a default palette. 
		// Actual palette data and range of pixels to form each image is contained in the .prt file.
		private BinaryReader bmpReader;
		private ArtFile artFile;

		private SliceStream GetPixels(uint startingIndex, uint length)
		{
			return new SliceStream(bmpReader.BaseStream, startingIndex, length);
		}

		public void Dispose()
		{
			if (bmpReader != null)
			{
				bmpReader.Dispose();
				bmpReader = null;
			}
		}
	}
}
