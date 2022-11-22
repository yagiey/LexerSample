using System;
using System.Collections.Generic;
using System.Linq;

namespace PsudoDfa
{
	using DfaTransitionMap = IDictionary<int, IDictionary<Character, int>>;

	public class Dfa
	{
		private int _state;
		private bool _isError;

		public static Dfa GenerateString(string str, bool ignoreCase)
		{
			if (string.IsNullOrEmpty(str))
			{
				const string ErrMsg = "string must not be null or empty.";
				throw new ArgumentException(ErrMsg, nameof(str));
			}

			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>();
			for (int i = 0; i < str.Length; i++)
			{
				transitionMap.Add(
					i,
					new Dictionary<Character, int> { { new Character(CharacterType.Literal, str[i]), i + 1 } }
				);
			}
			return new Dfa(0, new int[] { str.Length }, transitionMap, ignoreCase);
		}

		public static Dfa GenerateIdentifier()
		{
			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
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
			return new Dfa(0, new int[] { 1 }, transitionMap, false);
		}

		public static Dfa GenerateIntegerNumber()
		{
			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
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
			return new Dfa(0, new int[] { 2, 3 }, transitionMap, false);
		}

		public static Dfa GenerateLineComment()
		{
			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
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
			return new Dfa(0, new int[] { 2 }, transitionMap, false);
		}

		public static Dfa GenerateBlockComment()
		{
			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
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
			return new Dfa(0, new int[] { 4 }, transitionMap, false);
		}

		public static Dfa GenerateJsonNumber()
		{
			/*
			 <decimal-point>   ::= %x2E                                       ; .
			 <char_e>          ::= %x65                                       ; e
			 <char_E>          ::= %x45                                       ; E
			 <minus>           ::= %x2D                                       ; -
			 <plus>            ::= %x2B                                       ; +
			 <zero>            ::= %x30                                       ; 0
			 <digit1-9>        ::= %x49-57                                    ; 1-9
			 <digit>           ::= 0
			 <digit>           ::= <digit1-9>

			 <digits>          ::= <digit><digitsTail>
			 <digitsTail>      ::= ε
			 <digitsTail>      ::= <digit><digitsTail>
			 <digits0>         ::= <digits>?

			 <sign>            ::= <minus>
			 <sign>            ::= <plus>
			 <e>               ::= <char_e>
			 <e>               ::= <char_E>
			 <exp>             ::= <e><sign>?<digits>
			 <frac>            ::= <decimal-point><digits>
			 <int>             ::= <zero>
			 <int>             ::= <digit1-9><digits0>
			@<number>          ::= <minus>?<int><frac>?<exp>?
			*/


			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
			{
				{
					0,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '-'), 1 },
						{ new Character(CharacterType.Literal, '0'), 5 },
						{ Character.Digit1_9, 8 },
					}
				},
				{
					1,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '0'), 5 },
						{ Character.Digit1_9, 8 },
					}
				},
				{
					2,
					new Dictionary<Character, int> {
						{ Character.Digit, 6 },
					}
				},
				{
					3,
					new Dictionary<Character, int> {
						{ Character.Digit, 7 },
					}
				},
				{
					4,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '+'), 3 },
						{ new Character(CharacterType.Literal, '-'), 3 },
						{ Character.Digit, 7 },
					}
				},
				{
					5,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '.'), 2 },
						{ new Character(CharacterType.Literal, 'E'), 4 },
						{ new Character(CharacterType.Literal, 'e'), 4 },
					}
				},
				{
					6,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, 'E'), 4 },
						{ new Character(CharacterType.Literal, 'e'), 4 },
						{ Character.Digit, 6 },
					}
				},
				{
					7,
					new Dictionary<Character, int> {
						{ Character.Digit, 7 },
					}
				},
				{
					8,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '.'), 2 },
						{ new Character(CharacterType.Literal, 'E'), 4 },
						{ new Character(CharacterType.Literal, 'e'), 4 },
						{ Character.Digit, 8 },
					}
				},
			};
			return new Dfa(0, new int[] { 5, 6, 7, 8 }, transitionMap, false);
		}

		public static Dfa GenerateJsonStringLiteral()
		{
			/*
			 <digit>           ::= %x48-57                                    ; 0-9
			 <escape>          ::= %x5C                                       ; \
			 <quotation-mark>  ::= %x22                                       ; "
			 <hex-dig>         ::= %x41-46                                    ; A-F
			 <hex-dig>         ::= %x61-66                                    ; a-f
			 <hex-dig>         ::= <digit>                                    ; 0-9
			 <escaped>         ::= %x22                                       ; "    quotation mark  U+0022
			 <escaped>         ::= %x5C                                       ; \    reverse solidus U+005C
			 <escaped>         ::= %x2F                                       ; /    solidus         U+002F
			 <escaped>         ::= %x62                                       ; b    backspace       U+0008
			 <escaped>         ::= %x66                                       ; f    form feed       U+000C
			 <escaped>         ::= %x6E                                       ; n    line feed       U+000A
			 <escaped>         ::= %x72                                       ; r    carriage return U+000D
			 <escaped>         ::= %x74                                       ; t    tab             U+0009
			 <escaped>         ::= %x75<hex-dig><hex-dig><hex-dig><hex-dig>   ; uXXXX                U+XXXX
			 <unescaped>       ::= %x20-21                                    ;
			 <unescaped>       ::= %x23-5B                                    ;
			 <unescaped>       ::= %x5D-10FFFF                                ;

			 <char>            ::= <unescaped>                                ;
			 <char>            ::= <escape><escaped>                          ;\("|\|/|b|f|n|r|t|u(0|1|2|3|4|5|6|7|8|9|a|b|c|d|e|f|A|B|C|D|E|F)(0|1|2|3|4|5|6|7|8|9|a|b|c|d|e|f|A|B|C|D|E|F)(0|1|2|3|4|5|6|7|8|9|a|b|c|d|e|f|A|B|C|D|E|F)(0|1|2|3|4|5|6|7|8|9|a|b|c|d|e|f|A|B|C|D|E|F))

			 <chars>           ::= <char><charsTail>                          ;
			 <charsTail>       ::= ε                                          ;
			 <charsTail>       ::= <char><charsTail>                          ;
			 <chars0>          ::= <chars>?                                   ;

			@<string>          ::= <quotation-mark><chars0><quotation-mark>   ;
			*/

			DfaTransitionMap transitionMap = new Dictionary<int, IDictionary<Character, int>>
			{
				{
					0,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '"'), 1 },
					}
				},
				{
					1,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '"'), 7 },
						{ new Character(CharacterType.Literal, '\\'), 2 },
						{ Character.JsonUnescapedChar, 1 },
					}
				},
				{
					2,
					new Dictionary<Character, int> {
						{ new Character(CharacterType.Literal, '"'), 1 },
						{ new Character(CharacterType.Literal, '/'), 1 },
						{ new Character(CharacterType.Literal, '\\'), 1 },
						{ new Character(CharacterType.Literal, 'b'), 1 },
						{ new Character(CharacterType.Literal, 'f'), 1 },
						{ new Character(CharacterType.Literal, 'n'), 1 },
						{ new Character(CharacterType.Literal, 'r'), 1 },
						{ new Character(CharacterType.Literal, 't'), 1 },
						{ new Character(CharacterType.Literal, 'u'), 3 },
					}
				},
				{
					3,
					new Dictionary<Character, int> {
						{ Character.HexDigt, 4 },
					}
				},
				{
					4,
					new Dictionary<Character, int> {
						{ Character.HexDigt, 5 },
					}
				},
				{
					5,
					new Dictionary<Character, int> {
						{ Character.HexDigt, 6 },
					}
				},
				{
					6,
					new Dictionary<Character, int> {
						{ Character.HexDigt, 1 },
					}
				},
			};
			return new Dfa(0, new int[] { 7 }, transitionMap, false);
		}

		public Dfa(int startNode, IEnumerable<int> acceptingNodeSet, DfaTransitionMap transitionMap, bool ignoreCase)
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

		public DfaTransitionMap TransitionMap
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

		public static DfaTransitionMap AddTransition(DfaTransitionMap transitionMap, int node, Character input, int dest)
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

		public void MoveNext(int ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			if (result.Item1)
			{
				_state = result.Item2;
				return;
			}

			if (IgnoreCase)
			{
				int? corr = ToCorrespondingCase(ch);
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

		public bool IsNextError(int ch)
		{
			var result = GetNext(ch, _state, TransitionMap, IsError());
			return !result.Item1;
		}

		private static Tuple<bool, int> GetNext(int ch, int current, DfaTransitionMap transitionMap, bool isError)
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

		private static int? ToCorrespondingCase(int ch)
		{
			if (Character.IsUpperCase(ch))
			{
				return Character.ToLower(ch);
			}
			else if (Character.IsLowerCase(ch))
			{
				return Character.ToUpper(ch);
			}
			else
			{
				return null;
			}
		}
	}
}
