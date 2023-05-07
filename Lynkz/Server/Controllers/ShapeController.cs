using Lynkz.Shared;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text.Json.Serialization;

using TokenItem = System.ValueTuple<Lynkz.Shared.TokenType, string>;

namespace Lynkz.Server.Controllers
{
	[ApiController]
	[Route("[controller]/{request}")]
	public class ShapeController : ControllerBase
	{
		private readonly ILogger<ShapeController> _logger;

		public ShapeController(ILogger<ShapeController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public Shape Get(string request)
		{
			var s = Parse(request);
			return s;
		}

		Shape Parse(string prompt)
		{
			prompt = prompt.ToLower();
			prompt = prompt.Replace("side length", "side-length");
            prompt = prompt.Replace(" triangle", "-triangle");
            var words = prompt.Split();
			var tokens = new Queue<TokenItem>();
			foreach(var word in words)
			{
				switch (word)
				{
					case "draw":
					case "a":
					case "an":
                    case "with":
                    case "of":
					case "and":
					case "side":
                        break;
                    case "isosceles-triangle":
                    case "square":
                    case "scalene-triangle":
                    case "parallelogram":
                    case "equilateral-triangle":
                    case "pentagon":
                    case "rectangle":
                    case "hexagon":
                    case "heptagon":
                    case "octagon":
                    case "circle":
                    case "oval":
                        tokens.Enqueue((TokenType.SHAPE, word));
						break;
					case "radius":
					case "side-length":
					case "width":
					case "height":
						tokens.Enqueue((TokenType.DIMENSION, word));
						break;
					default:
					{
						int val = 0;
						if (int.TryParse(word, out val))
							tokens.Enqueue((TokenType.NUMBER, word));
						else
							tokens.Enqueue((TokenType.UNKNOWN, word));

						break;
					}
				}
			}

            var s = new Shape() { Request = prompt };
            var dims = new List<string>();
            var nums = new List<int>();
            TokenItem ti;
			TokenType last = TokenType.NONE;
			while (s.Msg == null && tokens.TryDequeue(out ti))
			{
				switch (last, ti.Item1)
				{
                    case (_, TokenType.UNKNOWN):
						s.Msg = $"I don't know what a '{ti.Item2}' is.";
                        break;
                    case (TokenType.NONE, TokenType.SHAPE):
                        s.Type = ti.Item2;
                        last = ti.Item1;
                        break;
                    case (TokenType.SHAPE or TokenType.NUMBER, TokenType.DIMENSION):
                        dims.Add(ti.Item2);
                        last = ti.Item1;
                        break;
                    case (TokenType.DIMENSION, TokenType.NUMBER):
						nums.Add(int.Parse(ti.Item2));
                        last = ti.Item1;
                        break;
                }
            }

			s.Dims = dims.ToArray();
            s.Nums = nums.ToArray();
			return s;
		}
	}
}