using NUnit.Framework;

namespace BoggleWordChecker
{
	internal class Program
	{
		public class Boggle
		{
			private class TrieNode
			{
				public Dictionary<char, TrieNode> charMap = new();
			}

			private class SuffixTrie
			{
				private TrieNode root = new();

				private List<int[]> GetNeighbors(int rows, int cols, int r, int c)
				{
					int[] deltaR = new int[] { 0, -1, -1, -1, 0, 1, 1, 1 };
					int[] deltaC = new int[] { -1, -1, 0, 1, 1, 1, 0, -1 };
					List<int[]> neighbors = new();
					for (int i = 0; i < deltaR.Length; ++i)
					{
						int newR = r + deltaR[i];
						int newC = c + deltaC[i];
						if (newR >= 0 && newR < rows && newC >= 0 && newC < cols)
						{
							neighbors.Add(new int[] { newR, newC });
						}
					}
					return neighbors;
				}

				private bool DFS(char[][] board, TrieNode node, bool[,] visited, int rows, int cols, int r, int c)
				{
					if (r < 0 || r >= rows || c < 0 || c >= cols || visited[r, c])
					{
						return false;
					}
					char letter = board[r][c];
					if (!node.charMap.ContainsKey(letter))
					{
						return false;
					}
					node = node.charMap[letter];
					visited[r, c] = true;
					if (node.charMap.ContainsKey('*'))
					{
						return true;
					}
					List<int[]> neighbors = GetNeighbors(rows, cols, r, c);
					foreach (int[] neighbor in neighbors)
					{
						if (DFS(board, node, visited, rows, cols, neighbor[0], neighbor[1]))
						{
							return true;
						}
					}
					visited[r, c] = false;
					return false;
				}

				public void Add(string word)
				{
					TrieNode node = root;
					foreach(char c in word)
					{
						if (!node.charMap.ContainsKey(c))
						{
							node.charMap.Add(c, new TrieNode());
						}
						node = node.charMap[c];
					}
					node.charMap.Add('*', null);
				}

				public bool CheckWord(char[][] board)
				{
					int rows = board.Length;
					int cols = board[0].Length;
					bool[,] visited = new bool[rows, cols];
					TrieNode node = root;
					for (int r = 0; r < rows; ++r)
					{
						for (int c = 0; c < cols; ++c)
						{
							if (DFS(board, node, visited, rows, cols, r, c))
							{
								return true;
							}
						}
					}
					return false;
				}
			}

			private readonly char[][] board;
			private readonly SuffixTrie suffixTrie;

			public Boggle(char[][] board, string word)
			{
				suffixTrie = new SuffixTrie();
				suffixTrie.Add(word);
				this.board = board;
			}

			public bool Check()
			{
				return suffixTrie.CheckWord(board);
			}
		}

		private static char[][] DeepCopy(char[][] arr)
		{
			return arr.Select(a =>
			{
				var target = new char[a.Length];
				Array.Copy(a, target, a.Length);
				return target;
			}).ToArray();
		}

		static void Main(string[] args)
		{
			char[][] board =
		   {
			   new []{'E','A','R','A'},
			   new []{'N','L','E','C'},
			   new []{'I','A','I','S'},
			   new []{'B','Y','O','R'}
		   };

			var toCheck = new[] { "C", "EAR", "EARS", "BAILER", "RSCAREIOYBAILNEA", "CEREAL", "ROBES" };
			var expecteds = new[] { true, true, false, true, true, false, false };

			for (int i = 0; i < toCheck.Length; i++)
			{
				Assert.AreEqual(expecteds[i], new Boggle(DeepCopy(board), toCheck[i]).Check());
			}
		}
	}
}