using OP2UtilityDotNet.Bitmap;
using System.Collections.Generic;
using System.IO;

namespace OP2UtilityDotNet.Sprite
{
	public class ArtFile
	{
		public List<Palette> palettes = new List<Palette>();
		public List<ImageMeta> imageMetas = new List<ImageMeta>();
		public List<Animation> animations = new List<Animation>();
		public uint unknownAnimationCount;

		public static ArtFile Read(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (BinaryReader artReader = new BinaryReader(fs))
			{
				return Read(artReader);
			}
		}

		public static ArtFile Read(Stream stream)
		{
			using (BinaryReader artReader = new BinaryReader(stream, System.Text.Encoding.ASCII, true))
			{
				return Read(artReader);
			}
		}

		public static ArtFile Read(BinaryReader reader)
		{
			ArtFile artFile = new ArtFile();

			ReadPalette(reader, artFile);
			ReadImageMetadata(reader, artFile);
			ReadAnimations(reader, artFile);

			return artFile;
		}

		public void Write(string filename)
		{
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
			using (BinaryWriter artWriter = new BinaryWriter(fs))
			{
				Write(artWriter);
			}
		}

		public void Write(Stream stream)
		{
			using (BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, true))
			{
				Write(writer);
			}
		}

		public void Write(BinaryWriter writer)
		{
			ValidateImageMetadata();

			WritePalettes(writer);

			writer.Write((uint)imageMetas.Count);
			for (int i=0; i < imageMetas.Count; ++i)
				imageMetas[i].Serialize(writer);

			WriteAnimations(writer);
		}

		public void VerifyImageIndexInBounds(int index)
		{
			if (index > imageMetas.Count) {
				throw new System.Exception("An index of " + index + " exceeds range of images");
			}
		}

		// Read Functions
		private static void ReadPalette(BinaryReader reader, ArtFile artFile)
		{
			SectionHeader paletteSectionHeader = new SectionHeader(reader);
			paletteSectionHeader.Validate(TagPalette);

			artFile.palettes = new List<Palette>((int)paletteSectionHeader.length);
			
			for (int i = 0; i < paletteSectionHeader.length; ++i) {
				PaletteHeader paletteHeader = new PaletteHeader(reader);

				paletteHeader.Validate();

				Palette palette = new Palette(reader);

				// Rearrange color into standard format. Outpost 2 uses custom color order.
				for (int j=0; j < palette.colors.Length; ++j)
				{
					byte temp = palette.colors[j].red;
					palette.colors[j].red = palette.colors[j].blue;
					palette.colors[j].blue = temp;
				}

				artFile.palettes.Add(palette);
			}
		}

		private static void ReadImageMetadata(BinaryReader reader, ArtFile artFile)
		{
			artFile.imageMetas.Clear();

			uint imageMetasCount = reader.ReadUInt32();
			for (int i=0; i < imageMetasCount; ++i)
				artFile.imageMetas.Add(new ImageMeta(reader));

			artFile.ValidateImageMetadata();
		}

		private static void ReadAnimations(BinaryReader reader, ArtFile artFile)
		{
			uint animationCount = reader.ReadUInt32();
			artFile.animations = new List<Animation>((int)animationCount);
			
			uint frameCount = reader.ReadUInt32();
			uint layerCount = reader.ReadUInt32();

			artFile.unknownAnimationCount = reader.ReadUInt32();

			for (uint i = 0; i < animationCount; ++i)
			{
				artFile.animations.Add(ReadAnimation(reader));
			}

			VerifyCountsMatchHeader(artFile, (int)frameCount, (int)layerCount, (int)artFile.unknownAnimationCount);
		}

		private static Animation ReadAnimation(BinaryReader reader)
		{
			Animation animation = new Animation();

			animation.unknown = reader.ReadUInt32();
			animation.selectionRect = new Rect(reader);
			animation.pixelDisplacement.x = reader.ReadInt32();
			animation.pixelDisplacement.y = reader.ReadInt32();
			animation.unknown2 = reader.ReadUInt32();

			uint frameCount = reader.ReadUInt32();
			animation.frames = new List<Animation.Frame>((int)frameCount);

			for (uint i = 0; i < frameCount; ++i) {
				animation.frames.Add(ReadFrame(reader));
			}

			uint unknownContainerCount = reader.ReadUInt32();
			for (int i=0; i < unknownContainerCount; ++i)
				animation.unknownContainer.Add(new Animation.UnknownContainer(reader));

			return animation;
		}

		private static Animation.Frame ReadFrame(BinaryReader reader)
		{
			Animation.Frame frame = new Animation.Frame();
			frame.optional1 = 0;
			frame.optional2 = 0;
			frame.optional3 = 0;
			frame.optional4 = 0;

			frame.layerMetadata = new Animation.Frame.LayerMetadata(reader);
			frame.unknownBitfield = new Animation.Frame.LayerMetadata(reader);

			if (frame.layerMetadata.bReadOptionalData != 0) {
				frame.optional1 = reader.ReadByte();
				frame.optional2 = reader.ReadByte();
			}
			if (frame.unknownBitfield.bReadOptionalData != 0) {
				frame.optional3 = reader.ReadByte();
				frame.optional4 = reader.ReadByte();
			}

			frame.layers = new List<Animation.Frame.Layer>(frame.layerMetadata.count);
			for (int i=0; i < frame.layerMetadata.count; ++i)
				frame.layers.Add(new Animation.Frame.Layer(reader));

			return frame;
		}

		private static void VerifyCountsMatchHeader(ArtFile artFile, int frameCount, int layerCount, int unknownCount)
		{
			int actualFrameCount;
			int actualLayerCount;
			uint actualUnknownCount;
			artFile.CountFrames(out actualFrameCount, out actualLayerCount, out actualUnknownCount);

			if (actualFrameCount != frameCount) {
				throw new System.Exception("Frame count does not match");
			}

			if (actualLayerCount != layerCount) {
				throw new System.Exception("Sub-frame count does not match.");
			}

			if (actualUnknownCount != unknownCount) {
				throw new System.Exception("Unknown count does not match.");
			}
		}


		// Write Functions
		private void WritePalettes(BinaryWriter writer)
		{
			//if (palettes.Count > uint.MaxValue) {
			//	throw new System.Exception("Art file contains too many palettes.");
			//}

			new SectionHeader(TagPalette, (uint)palettes.Count).Serialize(writer);

			// Intentially do not pass palette as reference to allow local modification
			// Switch red and blue color to match Outpost 2 custom format.
			foreach (Palette palette in palettes) {
				PaletteHeader.CreatePaletteHeader().Serialize(writer);

				Palette clonedPalette = new Palette(palette);

				for (int i=0; i < clonedPalette.colors.Length; ++i) {
					byte swap = clonedPalette.colors[i].red;
					clonedPalette.colors[i].red = clonedPalette.colors[i].blue;
					clonedPalette.colors[i].blue = swap;
				}

				clonedPalette.Serialize(writer);
			}
		}

		private void WriteAnimations(BinaryWriter writer)
		{
			//if (animations.Count > uint.MaxValue) {
			//	throw new System.Exception("There are too many animations contained in the ArtFile.");
			//}

			writer.Write((uint)animations.Count);

			int frameCount;
			int layerCount;
			uint unknownCount;
			CountFrames(out frameCount, out layerCount, out unknownCount);

			//if (frameCount > uint.MaxValue) {
			//	throw new System.Exception("There are too many frames to write to file.");
			//}

			//if (layerCount > uint.MaxValue) {
			//	throw new System.Exception("There are too many layers to write to file.");
			//}

			//if (unknownCount > uint.MaxValue) {
			//	throw new System.Exception("There are too many unknown container items to write to file.");
			//}

			writer.Write((uint)frameCount);
			writer.Write((uint)layerCount);
			writer.Write((uint)unknownCount);

			foreach (Animation animation in animations)
			{
				WriteAnimation(writer, animation);
			}
		}

		private static void WriteAnimation(BinaryWriter writer, Animation animation)
		{
			writer.Write(animation.unknown);
			animation.selectionRect.Serialize(writer);
			writer.Write(animation.pixelDisplacement.x);
			writer.Write(animation.pixelDisplacement.y);
			writer.Write(animation.unknown2);

			int frameCount = animation.frames.Count;
			//if (frameCount > uint.MaxValue) {
			//	throw new System.Exception("There are too many frames in animation to write");
			//}
			writer.Write((uint)frameCount);

			foreach (Animation.Frame frame in animation.frames) {
				WriteFrame(writer, frame);
			}

			writer.Write((uint)animation.unknownContainer.Count);
			for (int i=0; i < animation.unknownContainer.Count; ++i)
				animation.unknownContainer[i].Serialize(writer);
		}

		private static void WriteFrame(BinaryWriter writer, Animation.Frame frame)
		{
			if (frame.layerMetadata.count != frame.layers.Count) {
				throw new System.Exception("Recorded layer count must match number of written layers.");
			}

			frame.layerMetadata.Serialize(writer);
			frame.unknownBitfield.Serialize(writer);

			if (frame.layerMetadata.bReadOptionalData != 0) {
				writer.Write(frame.optional1);
				writer.Write(frame.optional2);
			}

			if (frame.unknownBitfield.bReadOptionalData != 0) {
				writer.Write(frame.optional3);
				writer.Write(frame.optional4);
			}

			for (int i=0; i < frame.layers.Count; ++i)
				frame.layers[i].Serialize(writer);
		}


		private void ValidateImageMetadata()
		{
			foreach (ImageMeta imageMeta in imageMetas) {
				// Bitwise operation rounds up to the next 4 byte interval
				if (imageMeta.scanLineByteWidth != ( (imageMeta.width + 3) & ~3) ) {
					throw new System.Exception("Image scan line byte width is not valid. It must be the width of the image rounded up to a 4 byte interval.");
				}

				if (imageMeta.paletteIndex >= palettes.Count) {
					throw new System.Exception("Image palette index is out of range of available palettes.");
				}
			}
		}

		private void CountFrames(out int frameCount, out int layerCount, out uint unknownCount)
		{
			frameCount = 0;
			layerCount = 0;
			unknownCount = unknownAnimationCount; //TODO: Figure out what this value is counting. Optional Frame information???

			foreach (Animation animation in animations) {
				frameCount += animation.frames.Count;
		
				foreach (Animation.Frame frame in animation.frames) {
					layerCount += frame.layers.Count;
				}
			}
		}

		private static readonly Tag TagPalette = new Tag("CPAL");
	}
}
