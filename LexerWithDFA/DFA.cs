using LexerSample;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LexerWithDFA
{
	using DFATransitionMap = IDictionary<int, IDictionary<Character, int>>;

	internal class DFA
	{
		private int _state;
		private bool _isError;

		public static DFA GenerateString(string str, bool ignoreCase)
		{
			if (string.IsNullOrEmpty(str))
			{
				const string ErrMsg = "string must not be null or empty.";
				throw new ArgumentException(ErrMsg, nameof(str));
			}

			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>();
			for (int i = 0; i < str.Length; i++)
			{
				transitionMap.Add(
					i,
					new Dictionary<Character, int> { { new Character(CharacterType.Literal, str[i]), i + 1 } }
				);
			}
			return new DFA(0, new int[] { str.Length }, transitionMap, ignoreCase);
		}

		public static DFA GenerateIdentifier()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
			{
				{
					0,
					new Dictionary<Character, int> {
						{Character.IdentifierHead, 1}
					} 
				},
				{
					1,
					new Dictionary<Character, int> {
						{Character.IdentifierCharacter, 1 }
					}
				},
			};
			return new DFA(0, new int[] { 1 }, transitionMap, false);
		}

		public static DFA GenerateIntegerNumber()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
			{
				{
					0,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '+'), 1},
						{ new Character(CharacterType.Literal, '-'), 1},
						{ new Character(CharacterType.Literal, '0'), 2},
						{ Character.Digit1_9                       , 3},
					}
				},
				{
					1,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '0'), 2},
						{ Character.Digit1_9                       , 3},
					}
				},
				{
					3,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '0'), 3},
						{ Character.Digit1_9,                        3},
					}
				},
			};
			return new DFA(0, new int[] { 2, 3 }, transitionMap, false);
		}

		public static DFA GenerateLineComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
			{
				{
					0,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '/'), 1},
					}
				},
				{
					1,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '/'), 2},
					}
				},
				{
					2,
					new Dictionary<Character, int> {
						{ Character.NeitherLfNorCr, 2},
					}
				},
			};
			return new DFA(0, new int[] { 2 }, transitionMap, false);
		}

		public static DFA GenerateBlockComment()
		{
			DFATransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
			{
				{
					0 ,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '/'), 1},
					}
				},
				{
					1 ,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '*'), 2},
					}
				},
				{
					2 ,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '*'), 3},
						{ new Character(CharacterType.NotEqual, '*'), 2},
					}
				},
				{
					3 ,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '/'), 4},
						{ new Character(CharacterType.NotEqual, '/'), 2},
					}
				},
			};
			return new DFA(0, new int[] { 4 }, transitionMap, false);
		}

		public DFA(int startNode, IEnumerable<int> acceptingNodeSet, DFATransitionMap transitionMap, bool ignoreCase)
		{
			StartNode = startNode;
			AcceptingNodeSet = acceptingNodeSet;
			TransitionMap = transitionMap;
			IgnoreCase = ignoreCase;
		}

		public bool IgnoreCase
		{
			get;
			private set;
		}

		public int StartNode
		{
			get;
			private set;
		}

		public IEnumerable<int> AcceptingNodeSet
		{
			get;
			private set;
		}

		public DFATransitionMap TransitionMap
		{
			get;
			private set;
		}

		public IEnumerable<int> GetAllNodes()
		{
			IEnumerable<int> result = Enumerable.Empty<int>();
			foreach (var pair1 in TransitionMap)
			{
				int node = pair1.Key;
				result = result.Append(node);
				IDictionary<Character, int> dic = pair1.Value;
				foreach (var pair2 in dic)
				{
					int destination = pair2.Value;
					result = result.Append(destination);
				}
			}
			return result.Distinct().OrderBy(_ => _);
		}

		public static DFATransitionMap AddTransition(DFATransitionMap transitionMap, int node, Character input, int dest)
		{
			if (transitionMap.ContainsKey(node))
			{
				var dic = transitionMap[node];
				dic.Add(input, dest);
			}
			else
			{
				IDictionary<Character, int> dic = new Dictionary<Character, int>
				{
					{ input, dest }
				};
				transitionMap.Add(node, dic);
			}
			return transitionMap;
		}

		public override string ToString()
		{
			List<string> lines = new()
			{
				string.Format("start: {0}", StartNode),
				string.Format("accepting: [{0}]", string.Join(",", AcceptingNodeSet))
			};

			foreach (var pair in TransitionMap.OrderBy(_ => _.Key))
			{
				var strMap = string.Join(",", pair.Value.OrderBy(_ => _.Key).Select(pair2 => string.Format("{0}->{1}", pair2.Key , pair2.Value)));
				lines.Add(string.Format("{0}:{1}", pair.Key, strMap));
			}
			return string.Join("\r\n", lines);
		}

		public void Reset()
		{
			_state = 0;
			_isError = false;
		}

		public bool IsMatch(string str)
		{
			int oldState = _state;
			bool oldIsError = _isError;
			Reset();
			foreach (char ch in str)
			{
				if (IsError())
				{
					break;
				}
				MoveNext(ch);
			}
			bool result = IsAcceptable();
			_state = oldState;
			_isError = oldIsError;
			return result;
		}

		public void MoveNext(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			if (result.Item1)
			{
				_state = result.Item2;
				return;
			}

			if (IgnoreCase)
			{
				char? corr = ToCorrespondingCase(ch);
				if (corr.HasValue)
				{
					var result2 = GetNext(corr.Value, _state, TransitionMap, IsError());
					if (result2.Item1)
					{
						_state = result2.Item2;
						return;
					}
				}
			}

			_isError = true;
		}

		public bool IsInitialState()
		{
			return _state == 0;
		}

		public bool IsAcceptable()
		{
			return !IsError() && AcceptingNodeSet.Contains(_state);
		}

		public bool IsAcceptable(IEnumerable<char> source)
		{
			foreach (char ch in source)
			{
				MoveNext(ch);
			}
			return IsAcceptable();
		}

		public bool IsError()
		{
			return _isError;
		}

		public bool IsNextError(char ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			return !result.Item1;
		}

		private static Tuple<bool, int> GetNext(char ch, int current, DFATransitionMap transitionMap, bool isError)
		{
			if (isError)
			{
				return Tuple.Create(false, 0);
			}

			bool result = transitionMap.TryGetValue(current, out IDictionary<Character, int>? dic);
			if (!result || dic == null)
			{
				return Tuple.Create(false, 0);
			}
			else
			{
				result = dic.TryGetValue(new Character(CharacterType.Literal, ch), out int dest);
				if (result)
				{
					return Tuple.Create(true, dest);
				}
				else
				{
					var dic2 = GetTransitionsWithPredicate(dic);
					foreach (var pair in dic2)
					{
						Character key = pair.Key;
						if (key.Match(ch))
						{
							return Tuple.Create(true, pair.Value);
						}
					}
					return Tuple.Create(false, 0);
				}
			}
		}

		private static IDictionary<Character, int> GetTransitionsWithPredicate(IDictionary<Character, int> map)
		{
			return new Dictionary<Character, int>(map.Where(_ => !_.Key.IsLiteralCharacter()));
		}

		private static char? ToCorrespondingCase(char ch)
		{
			if (IsUpperCase(ch))
			{
				return char.ToLower(ch);
			}
			else if (IsLowerCase(ch))
			{
				return char.ToUpper(ch);
			}
			else
			{
				return null;
			}
		}

		private static bool IsUpperCase(char ch)
		{
			return 'A' <= ch && ch <= 'Z';
		}

		private static bool IsLowerCase(char ch)
		{
			return 'a' <= ch && ch <= 'z';
		}
	}
}
