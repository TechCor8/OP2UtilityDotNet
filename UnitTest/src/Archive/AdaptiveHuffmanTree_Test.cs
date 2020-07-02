using Microsoft.VisualStudio.TestTools.UnitTesting;
using OP2UtilityDotNet.Archive;

namespace UnitTest.src.Archive
{
	[TestClass]
	public class AdaptiveHuffmanTree_Test
	{
		[TestMethod]
		public void SimpleTree()
		{
			// Construct minimum non-degenerate binary tree with 2 data nodes
			AdaptiveHuffmanTree tree = new AdaptiveHuffmanTree(2);

			// With 2 data nodes, the root cannot be a data node
			var rootIndex = tree.GetRootNodeIndex();
			Assert.IsFalse(tree.IsLeaf(rootIndex));

			// With 2 data nodes, the children of the root must be data nodes
			var leftChild = tree.GetChildNode(rootIndex, false);
			var rightChild = tree.GetChildNode(rootIndex, true);
			Assert.IsTrue(tree.IsLeaf(leftChild));
			Assert.IsTrue(tree.IsLeaf(rightChild));

			// No tree updates, so initial data should still be in order
			Assert.AreEqual(0, tree.GetNodeData(leftChild));
			Assert.AreEqual(1, tree.GetNodeData(rightChild));
		}
		
		[TestMethod]
		public void EncodeDecode()
		{
			AdaptiveHuffmanTree tree = new AdaptiveHuffmanTree(314);

			var codeCount = tree.TerminalNodeCount();
			for (ushort i = 0; i < codeCount; ++i) {
				uint codeLength;
				var bitString = tree.GetEncodedBitString(i, out codeLength);
				var node = tree.GetRootNodeIndex();
				for (; codeLength > 0; --codeLength) {
					Assert.IsFalse(tree.IsLeaf(node));
					node = tree.GetChildNode(node, (bitString & 1) != 0);
					bitString >>= 1;
				}
				Assert.IsTrue(tree.IsLeaf(node));
				Assert.AreEqual(i, tree.GetNodeData(node));
			}
		}
	}
}
