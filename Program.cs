using System;

namespace UnitMath
{
	class Program
	{
		public static void Main(string[] args)
		{
			//Console.WriteLine(UnitFactory.Units["N"]);
			
			var u = new UnitFactory();
            Console.WriteLine((u["Pa"] / u["N"]).ToString(UnitDisplayFormat.FirstChildren));
		    Console.WriteLine((u["Pa"] / u["N"]).ToString(UnitDisplayFormat.Simplified));
		    Console.WriteLine((u["Pa"] / u["N"]).ToString(UnitDisplayFormat.Flattened));
		    Console.WriteLine((u["Pa"] / u["N"]).ToString(UnitDisplayFormat.RootTree));
		    Console.WriteLine((u["Pa"] / u["N"]).ToString(UnitDisplayFormat.FlattenedAndSimplified));

            //Console.WriteLine(UnitFactory.Units["T"].Flatten());
            Console.ReadKey(true);
		}
	}
}