namespace OP2UtilityDotNet.Archive
{
	using NodeType = System.UInt16;
	using NodeIndex = System.UInt16;
	using NodeData = System.UInt16;
	using System.Collections.Generic;

	// The Huffman Tree stores codes in the terminal nodes of the (binary) tree.
	// The tree is initially balanced with codes 0 to terminalNodeCount-1 in the
	// terminal nodes at the bottom of the tree. The tree branches are traversed
	// using binary values: "0" traverse Left branch, "1" traverse Right branch.
	public class AdaptiveHuffmanTree
	{
		// Creates an (adaptive) Huffman tree with terminalNodeCount at the bottom.
		public AdaptiveHuffmanTree(NodeType terminalNodeCount)
		{
			// Initialize tree properties
			this.terminalNodeCount = terminalNodeCount;
			nodeCount = (NodeType)(terminalNodeCount * 2 - 1);
			rootNodeIndex = (NodeType)(nodeCount - 1);

			// Allocate space for tree
			linkOrData = new List<NodeType>(nodeCount);
			subtreeCount =  new List<NodeType>(nodeCount);
			parentIndex = new List<NodeType>(nodeCount + terminalNodeCount);
		
			// Initialize the tree
			// Initialize terminal nodes
			for (NodeIndex i = 0; i < terminalNodeCount; ++i)
			{
				linkOrData[i] = (NodeType)(i + nodeCount);			// Initilize data values
				subtreeCount[i] = 1;
				parentIndex[i] = (NodeType)((i >> 1) + terminalNodeCount);
				parentIndex[i + nodeCount] = i;						// "Parent of code" (node index)
			}
			// Initialize non terminal nodes
			NodeIndex left = 0;
			for (NodeIndex i = terminalNodeCount; i < nodeCount; ++i)
			{
				linkOrData[i] = left;								// Initialize link values
				subtreeCount[i] = (NodeType)(subtreeCount[left] + subtreeCount[left + 1]);	// Count is sum of two subtrees
				parentIndex[i] = (NodeType)((i >> 1) + terminalNodeCount);
				left += 2;										// Calc index of left child (next loop)
			}
		}

		// Tree info
		public NodeType TerminalNodeCount()
		{
			return terminalNodeCount;
		}

		// Decompression routines

		// Returns the index of the root node.
		// All tree searches start at the root node.
		public NodeIndex GetRootNodeIndex()
		{
			return rootNodeIndex;
		}

		// Return a child node of the given node.
		// This is used to traverse the tree to a terminal node.
		// bRight = 1 --> follow right branch
		public NodeIndex GetChildNode(NodeIndex nodeIndex, bool bRight)
		{
			VerifyNodeIndexInBounds(nodeIndex);

			// Return the child node index
			return (NodeIndex)(linkOrData[nodeIndex] + (bRight ? 1 : 0));
		}
		
		// Returns true if the node is a terminal node.
		// This is used to know when a tree search has completed.
		public bool IsLeaf(NodeIndex nodeIndex)
		{
			VerifyNodeIndexInBounds(nodeIndex);

			// Return whether or not this is a terminal node
			return linkOrData[nodeIndex] >= nodeCount;
		}

		// Returns the data stored in a terminal node
		public NodeData GetNodeData(NodeIndex nodeIndex)
		{
			VerifyNodeIndexInBounds(nodeIndex);

			// Return data stored in node translated back to normal form
			// Note: This assumes the node is a terminal node
			return (NodeData)(linkOrData[nodeIndex] - nodeCount);
		}

		// Tree restructuring routines
		
		// Perform tree update/restructure
		// This updates the count for the given code and restructures the tree if needed.
		// This is used after a tree search to update the tree (gives more frequently used
		// codes a shorter bit encoding).
		public void UpdateCodeCount(NodeData code)
		{
			VerifyNodeDataInBounds(code);

			// Get the index of the node containing this code
			NodeIndex curNodeIndex = parentIndex[code + nodeCount];
			subtreeCount[curNodeIndex]++; // Update the node count

			// Propagate the count increase up to the root of the tree
			while (curNodeIndex != rootNodeIndex)
			{
				// Find the block leader of the block
				// Note: the block leader is the "rightmost" node with count equal to the
				//  count of the current node, BEFORE the count of the current node is
				//  updated. (The current node has already had it's count updated.)
				NodeIndex blockLeaderIndex = curNodeIndex;
				while (subtreeCount[curNodeIndex] > subtreeCount[blockLeaderIndex + 1])
					blockLeaderIndex++;

				// Check if Current Node needs to be swapped with the Block Leader
				//if (curNodeIndex != blockLeaderIndex)
				{
					SwapNodes(curNodeIndex, blockLeaderIndex);
					curNodeIndex = blockLeaderIndex;	// Update index of current node
				}

				// Follow the current node up to it's parent
				curNodeIndex = parentIndex[curNodeIndex];
				// Increment the count of this new node
				subtreeCount[curNodeIndex]++;
			}
		}

		// Compression routines

		// NOTE: experimental, API likely to change
		// Retuns the path from the root node to the node with the given code
		//
		// Used during compression to get the bitstring to emit for a given code.
		// Returns the bitstring for a given code
		// Places the length of the bitstring in the bitCount out parameter
		// NOTE: Bit order subject to change (and likely will change). Currently:
		// The branch to take between the root and a child of the root is placed in the LSB
		// Subsequent branches are stored in higher bits
		public uint GetEncodedBitString(NodeData code, out uint bitCount)
		{
			VerifyNodeDataInBounds(code);

			// Record the path to the root
			bitCount = 0;
			uint bitString = 0;
			NodeIndex curNodeIndex = code;
			while (curNodeIndex != rootNodeIndex)
			{
				uint bBit = (uint)(curNodeIndex & 1);  // Get the direction from parent to current node
				bitString = (bitString << 1) | bBit;  // Pack the bit into the returned string
				bitCount++;
				curNodeIndex = parentIndex[curNodeIndex];
			}

			return bitString;
		}

		// Raise exception if nodeIndex is out of range
		private void VerifyNodeIndexInBounds(NodeIndex nodeIndex)
		{
			if (nodeIndex >= nodeCount)
			{
				throw new System.ArgumentOutOfRangeException("AdaptiveHuffmanTree NodeIndex of " + nodeIndex + " is out of range " + nodeCount);
			}
		}

		// Raise exception if code is out of range
		private void VerifyNodeDataInBounds(NodeData code)
		{
			if (code >= terminalNodeCount)
			{
				throw new System.ArgumentOutOfRangeException("AdaptiveHuffmanTree NodeData of " + code + " is out of range " + terminalNodeCount);
			}
		}

		// Private function to swap two nodes in the Huffman tree.
		// This is used during tree restructing by UpdateCodeCount.
		private void SwapNodes(NodeIndex nodeIndex1, NodeIndex nodeIndex2)
		{
			// Swap Count values
			(subtreeCount[nodeIndex1], subtreeCount[nodeIndex2]) = (subtreeCount[nodeIndex2], subtreeCount[nodeIndex1]);

			// Update the Parent of the children
			// Note: If the current node is a terminal node (data node) then the left
			//  child is a code value and it's "parent" must be updated but there is
			//  no right child to update a parent link for. If the current node is not
			//  a terminal node (not a data node) then both left and right child nodes
			//  need to have their parent link updated

			NodeType temp = linkOrData[nodeIndex1];
			parentIndex[temp] = nodeIndex2;			// Update left child
			if (temp < nodeCount)			// Check for non-data node (has right child)
				parentIndex[temp + 1] = nodeIndex2;	// Update right child

			temp = linkOrData[nodeIndex2];
			parentIndex[temp] = nodeIndex1;			// Update left child
			if (temp < nodeCount)			// Check for non-data node (has right child)
				parentIndex[temp + 1] = nodeIndex1;	// Update right child

			// Swap Data values (link to children or code value)
			(linkOrData[nodeIndex1], linkOrData[nodeIndex2]) = (linkOrData[nodeIndex2], linkOrData[nodeIndex1]);
		}

		// Tree properties
		private NodeType terminalNodeCount;
		private NodeType nodeCount;
		private NodeIndex rootNodeIndex;
		// Next three arrays comprise the adaptive huffman tree
		// Note:  is used for both tree links and node data.
		//   Values above or equal nodeCount represent data
		//   Values below nodeCount represent links
		private List<NodeType> linkOrData;   // index of left child (link) or code+nodeCount (data)
		private List<NodeType> subtreeCount; // number of occurances of code or codes in subtrees
		private List<NodeType> parentIndex; // index of the parent node to (current node or data)
							//  Note: This is also used to translate: code -> node_index
	};




	// Implementation Notes
	// --------------------

	// Note: the linkOrData array contains both link data and code data. If the value is
	//  less than nodeCount then the value represents the index of the node which is
	//  the left child of the current node and value+1 represents the index of the
	//  node which is the right child of the current node.
	//  If the value is greater or equal to nodeCount then the current node is a
	//  terminal node and the code stored in this node is value-nodeCount.

	// Note: Block Leader referse to the "rightmost" node whose count is equal to
	//  the count of the current node (before the count of the current node is
	//  updated). When restructing the tree, the current node must be swapped with
	//  the block leader before the count increase is propagated up the tree. The
	//  node to the right is considered the node with the next highest index.

	// Note: Since the tree is fully formed upon initialization and all terminal nodes
	//  have a count of at least 1, there is never a case in which the block leader
	//  is the parent of the current node (since the parent's count must be at least
	//  1 greater than either of it's two subtress, which both exist). Thus such a check
	//  does not need to be performed when swapping the current node with the block leader.

	// Note: The parentIndex array contains more entries than the other two. Entries below
	//  nodeCount are used to find the parent of a given node. Entries above nodeCount
	//  (up to nodeCount + terminalNodeCount) are used to find the node which contains
	//  a given code. i.e. the "parent" of the code.

	// Note: Parents are always to the "right" of their children. Also every node except
	//  the root has a sibling. Nodes with a higher count value are always found further
	//  to the right in the tree. Thus the tree maintains the Sibling Property.
}
