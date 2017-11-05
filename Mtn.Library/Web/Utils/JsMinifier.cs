using System;
using System.IO;
using System.Text;

/*
 * adapted from http://www.codeproject.com/Articles/33552/www.codeproject.com/KB/aspnet/AspNetOptimizer.aspx
 * I'd like thank's to jeff chin xyz, and Moiz Dhanji for his article.
 */
/* Originally written in 'C', this code has been converted to the C# language.
 * The author's copyright message is reproduced below.
 * All modifications from the original to C# are placed in the public domain.
 */

/* jsmin.c
   2007-05-22

Copyright (c) 2002 Douglas Crockford  (www.crockford.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

The Software shall be used for Good, not Evil.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Mtn.Library.Web.Utils
{
    /// <summary>
    /// minifier for javascript
    /// </summary>
	public class JsMinifier
	{
		const int Eof = -1;

		static StringReader _sr;
		static StringWriter _sw;
		static StringBuilder _sb;
		static int _theA;
		static int _theB;
		static int _theLookahead = Eof;
		
        /// <summary>
		/// 
		/// </summary>
		/// <param name="regularJsCode"></param>
		/// <returns></returns>
		public static string GetMinifiedCode(string regularJsCode)
		{
			string result = null;
			using (_sr = new StringReader(regularJsCode))
			{
				_sb = new StringBuilder();
				using (_sw = new StringWriter(_sb))
				{
					Jsmin();
				}
			}
			_sw.Flush();
			result = _sb.ToString();
			return result;
		}

		/* jsmin -- Copy the input to the output, deleting the characters which are
				insignificant to JavaScript. Comments will be removed. Tabs will be
				replaced with spaces. Carriage returns will be replaced with linefeeds.
				Most spaces and linefeeds will be removed.
		*/
		private static void Jsmin()
		{
			_theA = '\n';
			Action(3);
			while (_theA != Eof)
			{
				switch (_theA)
				{
					case ' ':
				    {
				        Action(IsAlphanum(_theB) ? 1 : 2);
				        break;
				    }
				    case '\n':
						{
							switch (_theB)
							{
								case '{':
								case '[':
								case '(':
								case '+':
								case '-':
									{
										Action(1);
										break;
									}
								case ' ':
									{
										Action(3);
										break;
									}
								default:
							    {
							        Action(IsAlphanum(_theB) ? 1 : 2);
							        break;
							    }
							}
							break;
						}
					default:
						{
							switch (_theB)
							{
								case ' ':
									{
										if (IsAlphanum(_theA))
										{
											Action(1);
											break;
										}
										Action(3);
										break;
									}
								case '\n':
									{
										switch (_theA)
										{
											case '}':
											case ']':
											case ')':
											case '+':
											case '-':
											case '"':
											case '\'':
												{
													Action(1);
													break;
												}
											default:
										    {
										        Action(IsAlphanum(_theA) ? 1 : 3);
										        break;
										    }
										}
										break;
									}
								default:
									{
										Action(1);
										break;
									}
							}
							break;
						}
				}
			}
		}
		/* action -- do something! What you do is determined by the argument:
				1   Output A. Copy B to A. Get the next B.
				2   Copy B to A. Get the next B. (Delete A).
				3   Get the next B. (Delete B).
		   action treats a string as a single character. Wow!
		   action recognizes a regular expression if it is preceded by ( or , or =.
		*/
		private static void Action(int d)
		{
			if (d <= 1)
			{
				Put(_theA);
			}
			if (d <= 2)
			{
				_theA = _theB;
				if (_theA == '\'' || _theA == '"')
				{
					for (; ; )
					{
						Put(_theA);
						_theA = Get();
						if (_theA == _theB)
						{
							break;
						}
						if (_theA <= '\n')
						{
							throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", _theA));
						}
					    if (_theA != '\\') continue;
					    Put(_theA);
					    _theA = Get();
					}
				}
			}
		    if (d > 3) return;
		    _theB = Next();
		    if (_theB != '/' ||
		        (_theA != '(' && _theA != ',' && _theA != '=' && _theA != '[' && _theA != '!' && _theA != ':' && _theA != '&' &&
		         _theA != '|' && _theA != '?' && _theA != '{' && _theA != '}' && _theA != ';' && _theA != '\n')) return;

		    Put(_theA);
		    Put(_theB);
		    for (; ; )
		    {
		        _theA = Get();
		        if (_theA != '/')
		        {
		            if (_theA == '\\')
		            {
		                Put(_theA);
		                _theA = Get();
		            }
		            else if (_theA <= '\n')
		            {
		                throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n",
		                    _theA));
		            }
		            Put(_theA);
		        }
		        else
		        {
		            break;
		        }
		    }
		    _theB = Next();
		}
		/* next -- get the next character, excluding comments. peek() is used to see
				if a '/' is followed by a '/' or '*'.
		*/
		private static int Next()
		{
			var c = Get();
		    if (c != '/') return c;
		    switch (Peek())
		    {
		        case '/':
		        {
		            for (; ; )
		            {
		                c = Get();
		                if (c <= '\n')
		                {
		                    return c;
		                }
		            }
		        }
		        case '*':
		        {
		            Get();
		            for (; ; )
		            {
		                switch (Get())
		                {
		                    case '*':
		                    {
		                        if (Peek() == '/')
		                        {
		                            Get();
		                            return ' ';
		                        }
		                        break;
		                    }
		                    case Eof:
		                    {
		                        throw new Exception("Error: JSMIN Unterminated comment.\n");
		                    }
		                }
		            }
		        }
		        default:
		        {
		            return c;
		        }
		    }
		}
		/* peek -- get the next character without getting it.
		*/
		private static int Peek()
		{
			_theLookahead = Get();
			return _theLookahead;
		}
		/* get -- return the next character from stdin. Watch out for lookahead. If
				the character is a control character, translate it to a space or
				linefeed.
		*/
		private static int Get()
		{
			var c = _theLookahead;
			_theLookahead = Eof;
			if (c == Eof)
			{
				c = _sr.Read();
			}
			if (c >= ' ' || c == '\n' || c == Eof)
			{
				return c;
			}
			return c == '\r' ? '\n' : ' ';
		}
		private static void Put(int c)
		{
			_sw.Write((char)c);
		}
		/* isAlphanum -- return true if the character is a letter, digit, underscore,
				dollar sign, or non-ASCII character.
		*/
		private static bool IsAlphanum(int c)
		{
			return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
				(c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
				c > 126);
		}
	}
}