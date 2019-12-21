using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Lomztein.AdvDiscordCommands.Framework;
using Lomztein.AdvDiscordCommands.Framework.Categories;
using Lomztein.AdvDiscordCommands.Framework.Interfaces;
using Discord;
using Lomztein.AdvDiscordCommands.Extensions;
using Lomztein.MathematicalExpressionParser;
using Lomztein.MathematicalExpressionParser.Token;
using Lomztein.MathematicalExpressionParser.Evaluation;
using Lomztein.AdvDiscordCommands.Framework.Execution.TokenParsers;

namespace Lomztein.AdvDiscordCommands.ExampleCommands {
    public class MathCommandSet : CommandSet {

        private static readonly Category BasicCategory = new Category ("Basic", "The most basic arithmatic commands, you might be able to do 4th grade homework with this.");
        private static readonly Category ScienceCategory = new Category ("Science", "More complicated arithmatic commands, used commonly in higher mathematics.");
        private static readonly Category TrigonometyCategory = new Category ("Trigonometry", "Everyones favorite highschool subjects.");
        private static readonly Category ConstantsCategory = new Category ("Constants", "Commands that return a single constant, such as PI.");
        private static readonly Category FunctionsCategory = new Category ("Functions", "Commands that modify the given number into something new and fancy.");

        public MathCommandSet() {
            Name = "math";
            Description = "Floating point mathematics.";
            Category = StandardCategories.Advanced;

            _commandsInSet = new List<ICommand> {
                new Add (), new Subtract (), new Multiply (), new Divide (), new Pow (), new Log (), new Mod (), new Sin (), new Cos (), new Tan (), new ASin (), new ACos (), new ATan (), new Deg2Rad (), new Rad2Deg (), new PI (),
                new Round (), new Ceiling (), new Floor (), new Squareroot (), new Min (), new Max (), new Abs (), new Sign (), new Equal (), new Random (), new Graph (), new Evaluate ()
            };

            _defaultCommand = new Evaluate ();
        }

        public class Add : Command {
            public Add() {
                Name = "add";
                Description = "Add numbers.";
                Category = BasicCategory;
            }

            [Overload (typeof (double), "Get the sum of an array of numbers.")]
            [Example ("4", "\"2 + 2 = 4\"", "2", "2")]
            public Task<Result> Execute(CommandMetadata e, double num1, double num2) {
                return TaskResult (num1 + num2, $"{num1} + {num2} = {num1 + num2}");
            }

            [Overload (typeof (double), "Add two numbers together.")]
            [Example ("10", "\"Sum of given numbers: 10\"", "1", "2", "3", "4")]
            public Task<Result> Execute(CommandMetadata e, double number, params double[] numbers) {
                return TaskResult (number + numbers.Sum (), $"Sum of given numbes: {number + numbers.Sum ()}");
            }

            [Overload (typeof (string), "Get the combination of two strings.")]
            [Example ("FirstSecond", "FirstSecond", "First", "Second")]
            public Task<Result> Execute(CommandMetadata e, string text, params string[] texts) {
                string result = text + string.Join("", texts);
                return TaskResult (result, result);
            }
        }

        public class Subtract : Command {

            public Subtract() {
                Name = "subtract";
                Description = "Subtract numbers.";
                Category = BasicCategory;
            }

            [Overload (typeof (double), "Subtract num2 from num1.")]
            public Task<Result> Execute(CommandMetadata e, double num1, double num2) {
                return TaskResult (num1 - num2, $"{num1} - {num2} = {num1 - num2}");
            }
        }

        public class Multiply : Command {

            public Multiply() {
                Name = "multiply";
                Description = "Multiply numbers.";
                Category = BasicCategory;
            }

            [Overload (typeof (double), "Mutliply two numbers.")]
            public Task<Result> Execute(CommandMetadata e, double num1, double num2) {
                return TaskResult (num1 * num2, $"{num1} * {num2} = {num1 * num2}");
            }
        }

        public class Divide : Command {

            public Divide() {
                Name = "divide";
                Description = "Divide numbers.";
                Category = BasicCategory;
            }

            [Overload (typeof (double), "Divide num1 with num2.")]
            public Task<Result> Execute(CommandMetadata e, double num1, double num2) {
                return TaskResult (num1 / num2, $"{num1} / {num2} = {num1 / num2}");
            }
        }

        public class Pow : Command {

            public Pow() {
                Name = "pow";
                Description = "Get the power.";
                Category = ScienceCategory;
            }

            [Overload (typeof (double), "Get the num1 to the power of num2.")]
            public Task<Result> Execute(CommandMetadata e, double num1, double num2) {
                return TaskResult (Math.Pow (num1, num2), $"{num1}^{num2} = {Math.Pow (num1, num2)}");
            }
        }

        public class Log : Command {

            public Log() {
                Name = "log";
                Description = "Returns logs.";
                Category = ScienceCategory;
            }

            [Overload (typeof (double), "Get the natural logarithm of the given number.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Log (num), $"Log({num}) = {Math.Log (num)}");
            }

            [Overload (typeof (double), "Get the logarithm of the given number in a specific base.")]
            public Task<Result> Execute(CommandMetadata e, double num, double logBase) {
                return TaskResult (Math.Log (num, logBase), $"Log{logBase}({num}) = {Math.Log (num, logBase)}");
            }
        }

        public class Mod : Command {

            public Mod() {
                Name = "mod";
                Description = "Returns modulus.";
                Category = ScienceCategory;
            }

            [Overload (typeof (double), "Get the remainder of num1 / num2.")]
            public Task<Result> Execute(CommandMetadata e, double num1, double num2) {
                return TaskResult (num1 % num2, $"{num1} % {num2} = {num1 % num2}");
            }
        }

        public class Sin : Command {

            public Sin() {
                Name = "sin";
                Description = "Anger the gods.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Get the sin of the given angle in radians.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Sin (num), $"SIN ({num}) = {Math.Sin (num)}");
            }
        }

        public class Cos : Command {

            public Cos() {
                Name = "cos";
                Description = "Returns cosine.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Get the cos of the given angle in radians.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Cos (num), $"COS ({num}) = {Math.Cos (num)}");
            }
        }

        public class Tan : Command {

            public Tan() {
                Name = "tan";
                Description = "Get ready for summer.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Get the tan of the given angle in radians.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Tan (num), $"TAN ({num}) = {Math.Tan (num)}");
            }
        }

        public class ASin : Command {

            public ASin() {
                Name = "asin";
                Description = "Make the gods.. happy?";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Get the inverse sin of the given value in radians.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Asin (num), $"ASIN ({num}) = {Math.Asin (num)}");
            }
        }

        public class ACos : Command {

            public ACos() {
                Name = "acos";
                Description = "Returns inverse cosine.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Get the inverse cos of the given value in radians.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Acos (num), $"ACOS ({num}) = {Math.Acos (num)}");
            }
        }

        public class ATan : Command {

            public ATan() {
                Name = "atan";
                Description = "Get ready for winter.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Get the atan of the given value in radians.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Atan (num), $"TAN ({num}) = {Math.Atan (num)}");
            }
        }

        public class Deg2Rad : Command {
            public Deg2Rad() {
                Name = "deg2rad";
                Description = "Convert degrees to radians.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Convert the given degrees to radians.")]
            public Task<Result> Execute(CommandMetadata e, double degs) {
                return TaskResult (degs / (180d / Math.PI), $"DEG2RAD ({degs}) = {degs / (180d / Math.PI)}");
            }
        }

        public class Rad2Deg : Command {
            public Rad2Deg() {
                Name = "rad2deg";
                Description = "Convert radians to degrees.";
                Category = TrigonometyCategory;
            }

            [Overload (typeof (double), "Convert the given radians to degrees.")]
            public Task<Result> Execute(CommandMetadata e, double rads) {
                return TaskResult (rads * (180d / Math.PI), $"RAD2DEG ({rads}) = {rads * (180d / Math.PI)}");
            }
        }

        public class PI : Command {
            public PI() {
                Name = "pi";
                Description = "I do like pie!";
                Category = ConstantsCategory;
            }

            [Overload (typeof (double), "Returns delicious pi.")]
            public Task<Result> Execute(CommandMetadata e) {
                return TaskResult (Math.PI, "PI = " + Math.PI);
            }
        }

        public class Round : Command {
            public Round() {
                Name = "round";
                Description = "Round the given number.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Rounds given input to the nearest whole number.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Round (num), $"ROUND ({num}) = {Math.Round (num)}");
            }
        }

        public class Floor : Command {
            public Floor() {
                Name = "floor";
                Description = "SuplexFlexDunk.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Floors given input to the nearest whole number below itself.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Floor (num), $"FLOOR ({num}) = {Math.Floor (num)}");
            }
        }

        public class Ceiling : Command {
            public Ceiling() {
                Name = "ceiling";
                Description = "Shoryuken that sucker.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Ceils given input to the nearest whole number above itself.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Ceiling (num), $"ROUND ({num}) = {Math.Ceiling (num)}");
            }
        }

        public class Squareroot : Command {
            public Squareroot() {
                Name = "sqrt";
                Description = "Get square root.";
                Category = ScienceCategory;
            }

            [Overload (typeof (double), "Returns the square root of the given number.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Sqrt (num), $"SQRT ({num}) = {Math.Sqrt (num)}");
            }
        }

        public class Min : Command {
            public Min() {
                Name = "min";
                Description = "Gets lowest number.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Returns the lowest number of the given array.")]
            public Task<Result> Execute(CommandMetadata e, params double[] nums) {
                return TaskResult (nums.Min (), $"Min of given numbers: {nums.Min ()}");
            }
        }

        public class Max : Command {
            public Max() {
                Name = "max";
                Description = "Gets highest number.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Returns the highest number of the given array.")]
            public Task<Result> Execute(CommandMetadata e, params double[] nums) {
                return TaskResult (nums.Max (), $"Max of given numbers: {nums.Max ()}");
            }
        }

        public class Abs : Command {
            public Abs() {
                Name = "abs";
                Description = "Gets absolute number.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Returns the absolute number of the given array.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Abs (num), $"ABS ({num}) = {Math.Abs (num)}");
            }
        }

        public class Sign : Command {
            public Sign() {
                Name = "sign";
                Description = "Gets sign of number.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Returns the sign of the given number.")]
            public Task<Result> Execute(CommandMetadata e, double num) {
                return TaskResult (Math.Sign (num), $"SIGN ({num}) = {Math.Sign (num)}");
            }
        }

        public class Equal : Command {
            public Equal() {
                Name = "equals";
                Description = "Checks equality.";
                Category = BasicCategory;
            }

            [Overload (typeof (bool), "Returns true if given objects are the same.")]
            public Task<Result> Execute(CommandMetadata e, object obj1, object obj2) {
                return TaskResult (obj1.Equals (obj2), $"{obj1} EQUALS {obj2} = {obj1.Equals (obj2)}");
            }

        }

        public class Random : Command {
            public Random() {
                Name = "random";
                Description = "Get random numbers.";
                Category = FunctionsCategory;
            }

            [Overload (typeof (double), "Returns random number between 0 and 1.")]
            public Task<Result> Execute(CommandMetadata e) {
                System.Random random = new System.Random ();
                return TaskResult (random.NextDouble (), "");
            }

            [Overload (typeof (bool), "Returns random number between 0 and given number.")]
            public Task<Result> Execute(CommandMetadata e, double max) {
                System.Random random = new System.Random ();
                return TaskResult (random.NextDouble () * max, "");
            }

            [Overload (typeof (bool), "Returns random number between the given numbers.")]
            public Task<Result> Execute(CommandMetadata e, double min, double max) {
                System.Random random = new System.Random ();
                return TaskResult (random.NextDouble () * (max + min) - min, "");
            }
        }

        public class Evaluate : Command {

            public Evaluate () {
                Name = "evaluate";
                Description = "Evaluate expression.";
                Category = ScienceCategory;
                Aliases = new[] { "eval" };
                Shortcut = "evaluate";
                ShortcutAliases = new[] { "eval" };
            }

            [Overload(typeof(double), "Evaluate the given mathematical expression and return the result.")]
            [Example("10", "6 + 2 * 2 = 10", "6 + 2 * 2")]
            //[Example ("64", "pow (8, 2) = 64", "pow (8, 2)")]
            //[Example ("14.14", "sqrt (pow (10, 2) + pow (10, 2)) = 14.14", "sqrt (pow (10, 2) + pow (10, 2))")]
            public Task<Result> Execute(CommandMetadata metadata, string expression)
            {
                ExpressionParser parser = new ExpressionParser();
                parser.Parsers = parser.Parsers.Concat(new MathematicalExpressionParser.Token.ITokenParser[] { new GetVariableParserWrapper(metadata) }).ToArray();
                double result = 0;
                try
                {
                    result = parser.Parse(expression);
                }
                catch (Exception exc)
                {
                    return TaskResult(result, $"Failed to evaluate '{expression}': it may be formatted in a wrong or otherwise unsupported way.");
                }
                return TaskResult(result, $"{expression} = {result}");
            }

            private class GetVariableParserWrapper : MathematicalExpressionParser.Token.ITokenParser {

                private GetVariableParser Parser { get; set; } = new GetVariableParser ();
                public CommandMetadata Metadata { get; private set; }

                public GetVariableParserWrapper (CommandMetadata metadata) {
                    Metadata = metadata;
                }

                public (IToken token, string result) Parse(string from) {
                    if (from[0] == Parser.Start) {
                        int start = 0;
                        int end = from.IndexOf (Parser.End);
                        string substring = from.Substring (start + 1, end - 1);
                        var result = Parser.TryParseInsidesAsync ("", substring, "", Metadata);
                        if (result.Exception != null) throw result.Exception;
                        if (result.Result.Success) {
                            return (new Value ((double)result.Result.Result), Parser.Start + substring + Parser.End);
                        }
                    }
                    return (null, null);
                }
            }
        }

        public class Graph : Command {

            public const int X_RES = 512, Y_RES = 512;

            public Graph() {
                Name = "graph";
                Description = "Draw a graph of a function.";
                Category = FunctionsCategory;
                Shortcut = "graph";
            }

            [Overload (typeof (void), "Draw a graph of the given function within the given range.")]
            public async Task<Result> Execute(CommandMetadata data, double xrange, double yrange, string yequals) {
                double xstart = -xrange / 2d;
                double xend = xrange / 2d;

                double ystart = yrange / 2d;
                double yend = -yrange / 2d;

                // These are actually the inverse scale, and should be the other way around. Though at this point it doesn't really matter.
                double xscale = (xend - xstart) / X_RES;
                double yscale = (yend - ystart) / Y_RES;

                using (Bitmap bitmap = new Bitmap (X_RES, Y_RES)) {

                    int yprev = (await CalcY(data, yequals, xstart, yscale)).Item1;

                    for (int y = 0; y < Y_RES; y++) {
                        for (int x = 0; x < X_RES; x++) {

                            bitmap.SetPixel (x, y, System.Drawing.Color.White);

                            int offsetX = (int)(((double)x / X_RES) * X_RES - (X_RES / 2d));
                            int offsetY = (int)(((double)y / Y_RES) * Y_RES - (Y_RES / 2d));

                            if (
                                (offsetX % (int)Math.Round (X_RES / xrange)) == 0 ||
                                (offsetY % (int)Math.Round (Y_RES / yrange)) == 0
                                )
                                bitmap.SetPixel (x, y, System.Drawing.Color.LightGray);

                            if (offsetX == 0 || offsetY == 0)
                                bitmap.SetPixel (x, y, System.Drawing.Color.Gray);
                        }
                    }

                    try {
                        using (Graphics graphics = Graphics.FromImage (bitmap)) {
                            using (Font font = new Font ("Areal", 16)) {
                                graphics.DrawString ($"({xstart},{ystart})", font, Brushes.Gray, 0, 0);

                                SizeF lowerSize = graphics.MeasureString ($"({xend},{yend})", font);
                                graphics.DrawString ($"({xend},{yend})", font, Brushes.Gray, 512 - lowerSize.Width, 512 - lowerSize.Height);
                            }
                        }
                    } catch { };

                    for (double x = xstart; x < xend; x += xscale) {

                        int xpix = (int)Math.Round (x / xscale) + X_RES / 2;
                        Tuple<int, bool> res = await CalcY (data, yequals, x, yscale);
                        int ycur = res.Item1;

                        if (res.Item2) { // Is the result defined?
                            int dist = ycur - yprev;
                            int sign = Math.Sign (dist);
                            dist = Math.Min (Math.Abs (dist), X_RES);
                            if (dist == 0)
                                dist = 1;

                            for (int yy = 0; yy < dist; yy++) {
                                double fraction = yy / (double)dist;
                                int ypix = (int)Lerp (yprev, ycur, fraction);

                                if (!(
                                    xpix < 0 || xpix >= X_RES ||
                                    ypix < 0 || ypix >= Y_RES
                                    ))
                                    bitmap.SetPixel (xpix, ypix, System.Drawing.Color.Black);
                            }

                            yprev = ycur;
                        }
                    }

                    using (MemoryStream stream = new MemoryStream ()) {
                        bitmap.Save (stream, System.Drawing.Imaging.ImageFormat.Png);
                        stream.Position = 0;
                        await data.Message.Channel.SendFileAsync (stream, "graph.png");
                    }

                    return new Result (null, "");
                }
            }

            private double Lerp (double start, double end, double t) => start * (1 - t) + end * t;

            private async Task<System.Tuple<int, bool>> CalcY(CommandMetadata data, string cmd, double x, double yscale) {
                CommandVariables.Set (data.Message.Id, "x", x, true);

                ExecutionData execution = data.Root.CreateExecution (cmd, data);
                var result = await data.Executor.Execute (execution);

                try {
                    double y = (double)Convert.ChangeType (result?.Value, typeof (double));
                    int ycur = (int)Math.Round (y / yscale) + Y_RES / 2;
                    return new System.Tuple<int, bool> (ycur, !double.IsNaN (y));
                } catch (Exception) {
                    throw new InvalidDataException ("The given command doesn't return numbers.");
                }

            }
        }
    }
}