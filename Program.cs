using System;

namespace UnitMath
{
	class Program
	{
		public static void Main(string[] args)
		{
			//Console.WriteLine(UnitFactory.Units["N"]);
			
			var facotry = new UnitFactory();
            Console.WriteLine(facotry["N"].ToString2());
            
			//Console.WriteLine(UnitFactory.Units["T"].Flatten());
			Console.ReadKey(true);
		}
	}
}