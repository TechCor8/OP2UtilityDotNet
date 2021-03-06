using OP2UtilityDotNet.Bitmap;

namespace OP2UtilityDotNet.Sprite
{
	/// <summary>
	/// Outpost 2 extension for BitmapFile.
	/// 
	/// This class provides extension methods for getting colors that are used for rendering.
	/// These methods are used to represent the special handling used by Outpost 2 for some palette indexes and types of sprites.
	/// </summary>
	public class OP2BitmapFile : BitmapFile
	{
		private ImageMeta imageMeta;

		/// <summary>
		/// Constructor from OP2 meta and bitmap data.
		/// </summary>
		public OP2BitmapFile(ImageMeta meta, Color[] palette, byte[] indexedPixels) : base(meta.GetBitCount(), meta.width, (int)-meta.height, palette, indexedPixels)
		{
			imageMeta = meta;
		}

		public Color GetEnginePixel(int x, int y)
		{
			int paletteIndex = GetPixelPaletteIndex(x,y);

			switch (paletteIndex)
			{
				case 0:
					// Transparent index
					return new Color(0,0,0,0);

				case 1:
					if (imageMeta.type.bShadow != 0)
					{
						return new Color(0,0,0,127); // TODO: Figure out real shadow color
					}
					break;
			}

			// Default to palette color
			return new Color(palette[paletteIndex].blue, palette[paletteIndex].green, palette[paletteIndex].red, 255);
		}

		public Color GetPlayerPixel(int x, int y, Color[] playerPalette)
		{
			int paletteIndex = GetPixelPaletteIndex(x,y);

			if (paletteIndex == 0)
			{
				// Transparent index
				return new Color(0,0,0,0);
			}

			// Player pixels only apply to the first 24 colors
			if (paletteIndex < 24)
			{
				// Player pixels only apply to non-shadow game graphics
				if (imageMeta.type.bShadow == 0 && imageMeta.type.bGameGraphic != 0)
				{
					return new Color(playerPalette[paletteIndex].blue, playerPalette[paletteIndex].green, playerPalette[paletteIndex].red, 255);
				}
			}

			// Default to palette color
			return new Color(palette[paletteIndex].blue, palette[paletteIndex].green, palette[paletteIndex].red, 255);
		}
	}
}