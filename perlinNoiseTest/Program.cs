using System;
using System.Collections;
using System.Collections.Generic;

namespace perlinNoiseTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Random rand = new Random();
			int x = 0;	//Width of map
			int y = 0;	//Height of map
			double scale = 0;	//Scale of Perlin Noise Maps
			double noise; //Stores noise values
			int temp = 2; // A temporar
			double xMid = x * 0.025; //Aligns the x axis of map in [2] Map
			double yMid = y * 0.025; //Aligns the y axis of map in [2] Map
			double[,] map = new double[x, y]; //height map
			double[,] tempMap = new double[x, y]; //tempreture map
			double[,] rainfallMap = new double[x, y]; //rainfall map
			double[,] water = new double[x, y]; //stores lowest value of terrain in area for [3] Rivers 0.1.0
			int[,] enviromentMap = new int[x, y]; //stores map of biomes
			byte[,] lowestDirection = new byte[x, y]; //stores direction of lowest terrain
			double lowestTerrain = 0; //stores lowest terrain value for [3] Rivers temporarily
			bool[,] river = new bool[x, y]; //whether a coordinate is a river or not
			int[,] nearestWater = new int[x, y]; //how far the nearest body of water is from any given point
			int seed = 0; //seed for all random numbers
			double xChange = 2550; //x offset for map
			double yChange = 2550; //y offset for map
			bool valid = true; //check for loops if user has used the ui correctly
			ConsoleKeyInfo keypress = new ConsoleKeyInfo(); //records keypresses for navigation and user input
			byte runApp = 0; //what the user has selected
			int[] toWater = new int[3]; //temporary storage for the value closest to water
			while (keypress.Key != ConsoleKey.Escape)
			{
				scale = 0;
				x = 0;
				seed = 0;
				valid = true;
				Console.Clear();
				//Main Menu
				while (runApp == 0)
				{
					Console.WriteLine("Press [1] for Preview \n" +
						"Press [2] for Map \n" +
						"Press [3] for Rivers 0.1.0\n" +
						"Press [4] for Rivers 0.2.0\n" +
						"Press [5] for Diamond Square Generation with Rivers 0.2.1\n" +
						"Press [6] for Diamond Square Generation with Rivers 0.2.2\n" +
						"Press [7] for Diamond Square Generation with Rivers 0.3.0");
					keypress = Console.ReadKey();
					switch (keypress.Key)
					{
						case ConsoleKey.D1:
							runApp = 1;
							break;
						case ConsoleKey.D2:
							runApp = 2;
							break;
						case ConsoleKey.D3:
							runApp = 3;
							break;
						case ConsoleKey.D4:
							runApp = 4;
							break;
						case ConsoleKey.D5:
							runApp = 5;
							break;
						case ConsoleKey.D6:
							runApp = 6;
							break;
						case ConsoleKey.D7:
							runApp = 7;
							break;
						default:
							break;
					}
				}
				Console.Clear();
				//Set parameters for perlin noise generation
				if (runApp < 5)
				{
					while (valid)
					{
						Console.WriteLine("Enter World Size");
						try
						{
							x = Convert.ToInt32(Console.ReadLine());
							if (x > 0 && x < 256)
							{
								y = x * 2;
								valid = false;
							}
							else
							{
								Console.WriteLine("Enter a integer between 1 and 255");

							}
						}
						catch (Exception)
						{
							if (x == 0)
							{
								x = 40;
								y = x * 2;
								valid = false;
							}
							else
							{
								Console.WriteLine("Enter a integer between 1 and 255");

							}
						}
					}
					xMid = x * 0.025;
					yMid = y * 0.025;
					map = new double[x, y];
					tempMap = new double[x, y];
					rainfallMap = new double[x, y];
					water = new double[x, y];
					enviromentMap = new int[x, y];
					lowestDirection = new byte[x, y];
					lowestTerrain = 0;
					river = new bool[x, y];
					nearestWater = new int[x, y];
					valid = true;
					Console.Clear();
					while (valid)
					{
						Console.WriteLine("Enter Scale");
						try
						{
							scale = Convert.ToInt32(Console.ReadLine());
							if (scale > 4 && scale < 65)
							{
								valid = false;
							}
							else
							{
								Console.WriteLine("Enter a integer between 5 and 64");

							}

						}
						catch (Exception)
						{
							if (scale == 0)
							{
								scale = 20;
								valid = false;
							}
							else
							{
								Console.WriteLine("Enter a integer between 5 and 64");

							}
						}
					}
					valid = true;
					Console.Clear();
				}
				//Set seed
				while (valid)
				{
					Console.WriteLine("Enter Seed");
					try
					{
						seed = Convert.ToInt32(Console.ReadLine());
						valid = false;
					}
					catch (Exception)
					{
						if (seed == 0)
						{
							seed = rand.Next(0, 16777216);
							valid = false;
						}
						else
						{
							Console.WriteLine("Enter a integer as the seed with less than 10 digits");
						}
					}
				}
				rand = new Random(seed);
				//Run selected
				switch (runApp)
				{
					case 1:
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								map[xi, yi] = Perlin.perlin(xi / scale, yi / scale, seed + 0.5) * 9 - 1;
							}
						}
						Draw.HeightMap(map);

						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								tempMap[xi, yi] = (Perlin.perlin(xi / scale, yi / scale, seed + 1.5)) * 8;
							}
						}
						Draw.TempMap(tempMap);

						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								rainfallMap[xi, yi] = Perlin.perlin(xi / scale, yi / scale, seed + 2.5) * 5;
							}
						}
						Draw.RainfallMap(rainfallMap);
						keypress = Console.ReadKey();
						break;
					case 2:
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								enviromentMap[xi, yi] = enviromentType(map[xi, yi], tempMap[xi, yi], rainfallMap[xi, yi]);
								switch (enviromentMap[xi, yi])
								{
									case 1:
										writeColours('+', ConsoleColor.White, ConsoleColor.DarkBlue);
										break;
									case 2:
										writeColours('#', ConsoleColor.White, ConsoleColor.DarkBlue);
										break;
									case 3:
										writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.White);
										break;
									case 4:
										writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.Yellow);
										break;
									case 5:
										writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.Red);
										break;
									case 6:
										writeColours('+', ConsoleColor.White, ConsoleColor.Blue);
										break;
									case 7:
										writeColours('#', ConsoleColor.White, ConsoleColor.Blue);
										break;
									case 8:
										writeColours('~', ConsoleColor.Blue, ConsoleColor.White);
										break;
									case 9:
										writeColours('~', ConsoleColor.Blue, ConsoleColor.Yellow);
										break;
									case 10:
										writeColours('~', ConsoleColor.Blue, ConsoleColor.Red);
										break;
									case 11:
										writeColours('+', ConsoleColor.White, ConsoleColor.Cyan);
										break;
									case 12:
										writeColours('#', ConsoleColor.White, ConsoleColor.Cyan);
										break;
									case 13:
										writeColours('~', ConsoleColor.Cyan, ConsoleColor.White);
										break;
									case 14:
										writeColours('~', ConsoleColor.Cyan, ConsoleColor.Yellow);
										break;
									case 15:
										writeColours('~', ConsoleColor.Cyan, ConsoleColor.Red);
										break;
									case 16:
										writeColours(' ', ConsoleColor.White, ConsoleColor.White);
										break;
									case 17:
										writeColours('*', ConsoleColor.White, ConsoleColor.DarkGreen);
										break;
									case 18:
										writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.White);
										break;
									case 19:
										writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Gray);
										break;
									case 20:
										writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Yellow);
										break;
									case 21:
										writeColours('~', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
										break;
									case 22:
										writeColours('n', ConsoleColor.White, ConsoleColor.Gray);
										break;
									case 23:
										writeColours('n', ConsoleColor.White, ConsoleColor.Cyan);
										break;
									case 24:
										writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Cyan);
										break;
									case 25:
										writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Green);
										break;
									case 26:
										writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.DarkRed);
										break;
									case 27:
										writeColours('n', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
										break;
									case 28:
										writeColours('m', ConsoleColor.White, ConsoleColor.Gray);
										break;
									case 29:
										writeColours('m', ConsoleColor.White, ConsoleColor.Cyan);
										break;
									case 30:
										writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.White);
										break;
									case 31:
										writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.Cyan);
										break;
									case 32:
										writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.Gray);
										break;
									case 33:
										writeColours('I', ConsoleColor.DarkYellow, ConsoleColor.DarkRed);
										break;
									case 34:
										writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkGray);
										break;
									case 35:
										writeColours('V', ConsoleColor.Gray, ConsoleColor.Cyan);
										break;
									case 36:
										writeColours('V', ConsoleColor.Gray, ConsoleColor.DarkGray);
										break;
									case 37:
										writeColours('A', ConsoleColor.Gray, ConsoleColor.Cyan);
										break;
									case 38:
										writeColours('A', ConsoleColor.Gray, ConsoleColor.DarkGray);
										break;
									case 39:
										writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkYellow);
										break;
									case 40:
										writeColours('V', ConsoleColor.Gray, ConsoleColor.White);
										break;
									case 41:
										writeColours('A', ConsoleColor.Gray, ConsoleColor.White);
										break;
									case 42:
										writeColours('*', ConsoleColor.White, ConsoleColor.DarkGreen);
										break;
									case 43:
										writeColours('*', ConsoleColor.Green, ConsoleColor.DarkGreen);
										break;
									case 44:
										writeColours('/', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
										break;
									case 45:
										writeColours('n', ConsoleColor.Green, ConsoleColor.DarkGreen);
										break;
									case 46:
										writeColours('n', ConsoleColor.Yellow, ConsoleColor.Green);
										break;
									case 47:
										writeColours('m', ConsoleColor.Green, ConsoleColor.DarkGreen);
										break;
									case 48:
										writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkGray);
										break;
									case 49:
										writeColours('/', ConsoleColor.Green, ConsoleColor.Yellow);
										break;
									case 50:
										writeColours('S', ConsoleColor.DarkCyan, ConsoleColor.DarkGreen);
										break;
									case 51:
										writeColours('n', ConsoleColor.Green, ConsoleColor.Red);
										break;
									case 52:
										writeColours('n', ConsoleColor.Green, ConsoleColor.DarkRed);
										break;
									case 53:
										writeColours('V', ConsoleColor.Cyan, ConsoleColor.White);
										break;
									case 54:
										writeColours('~', ConsoleColor.Green, ConsoleColor.Cyan);
										break;
									default:
										break;
								}
							}
							Console.WriteLine();
						}
						keypress = Console.ReadKey();
						Console.Clear();
						while (keypress.Key != ConsoleKey.Escape)
						{
							//Draws Full Sized Biome Map
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									map[xi, yi] = Perlin.perlin(xi / scale + (xChange / scale), yi / scale + (yChange / scale), seed + 0.5) * 9 - 1;
									tempMap[xi, yi] = Perlin.perlin(xi / scale + (xChange / scale), yi / scale + (yChange / scale), seed + 1.5) * 8;
									rainfallMap[xi, yi] = Perlin.perlin(xi / scale + (xChange / scale), yi / scale + (yChange / scale), seed + 2.5) * 5;
									enviromentMap[xi, yi] = enviromentType(map[xi, yi], tempMap[xi, yi], rainfallMap[xi, yi]);
									switch (enviromentMap[xi, yi])
									{
										case 1:
											writeColours('+', ConsoleColor.White, ConsoleColor.DarkBlue);
											break;
										case 2:
											writeColours('#', ConsoleColor.White, ConsoleColor.DarkBlue);
											break;
										case 3:
											writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.White);
											break;
										case 4:
											writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.Yellow);
											break;
										case 5:
											writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.Red);
											break;
										case 6:
											writeColours('+', ConsoleColor.White, ConsoleColor.Blue);
											break;
										case 7:
											writeColours('#', ConsoleColor.White, ConsoleColor.Blue);
											break;
										case 8:
											writeColours('~', ConsoleColor.Blue, ConsoleColor.White);
											break;
										case 9:
											writeColours('~', ConsoleColor.Blue, ConsoleColor.Yellow);
											break;
										case 10:
											writeColours('~', ConsoleColor.Blue, ConsoleColor.Red);
											break;
										case 11:
											writeColours('+', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 12:
											writeColours('#', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 13:
											writeColours('~', ConsoleColor.Cyan, ConsoleColor.White);
											break;
										case 14:
											writeColours('~', ConsoleColor.Cyan, ConsoleColor.Yellow);
											break;
										case 15:
											writeColours('~', ConsoleColor.Cyan, ConsoleColor.Red);
											break;
										case 16:
											writeColours(' ', ConsoleColor.White, ConsoleColor.White);
											break;
										case 17:
											writeColours('*', ConsoleColor.White, ConsoleColor.DarkGreen);
											break;
										case 18:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.White);
											break;
										case 19:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Gray);
											break;
										case 20:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Yellow);
											break;
										case 21:
											writeColours('~', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
											break;
										case 22:
											writeColours('n', ConsoleColor.White, ConsoleColor.Gray);
											break;
										case 23:
											writeColours('n', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 24:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Cyan);
											break;
										case 25:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Green);
											break;
										case 26:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.DarkRed);
											break;
										case 27:
											writeColours('n', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
											break;
										case 28:
											writeColours('m', ConsoleColor.White, ConsoleColor.Gray);
											break;
										case 29:
											writeColours('m', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 30:
											writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.White);
											break;
										case 31:
											writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.Cyan);
											break;
										case 32:
											writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.Gray);
											break;
										case 33:
											writeColours('I', ConsoleColor.DarkYellow, ConsoleColor.DarkRed);
											break;
										case 34:
											writeColours('n', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 35:
											writeColours('V', ConsoleColor.Gray, ConsoleColor.Cyan);
											break;
										case 36:
											writeColours('V', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 37:
											writeColours('A', ConsoleColor.Gray, ConsoleColor.Cyan);
											break;
										case 38:
											writeColours('A', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 39:
											writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkYellow);
											break;
										case 40:
											writeColours('V', ConsoleColor.Gray, ConsoleColor.White);
											break;
										case 41:
											writeColours('A', ConsoleColor.Gray, ConsoleColor.White);
											break;
										case 42:
											writeColours('*', ConsoleColor.White, ConsoleColor.DarkGreen);
											break;
										case 43:
											writeColours('*', ConsoleColor.Green, ConsoleColor.DarkGreen);
											break;
										case 44:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Green);
											break;
										case 45:
											writeColours('n', ConsoleColor.Green, ConsoleColor.DarkGreen);
											break;
										case 46:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Green);
											break;
										case 47:
											writeColours('m', ConsoleColor.Green, ConsoleColor.DarkGreen);
											break;
										case 48:
											writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 49:
											writeColours('/', ConsoleColor.Green, ConsoleColor.Yellow);
											break;
										case 50:
											writeColours('S', ConsoleColor.DarkCyan, ConsoleColor.DarkGreen);
											break;
										case 51:
											writeColours('n', ConsoleColor.Green, ConsoleColor.Red);
											break;
										case 52:
											writeColours('n', ConsoleColor.Green, ConsoleColor.DarkRed);
											break;
										case 53:
											writeColours('V', ConsoleColor.Cyan, ConsoleColor.White);
											break;
										case 54:
											writeColours('~', ConsoleColor.Green, ConsoleColor.Cyan);
											break;
										default:
											break;
									}
								}
								Console.BackgroundColor = ConsoleColor.Black;
								Console.WriteLine();
							}
							Console.WriteLine();
							//Draws Zoomed in Biome Map
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									map[xi, yi] = Perlin.perlin(xi / (scale * 10) + xMid + (xChange / (scale)), yi / (scale * 10) + yMid + (yChange / scale), seed + 0.5) * 9 - 1;
									tempMap[xi, yi] = Perlin.perlin(xi / (scale * 10) + xMid + (xChange / scale), yi / (scale * 10) + yMid + (yChange / scale), seed + 1.5) * 8;
									rainfallMap[xi, yi] = Perlin.perlin(xi / (scale * 10) + xMid + (xChange / scale), yi / (scale * 10) + yMid + (yChange / scale), seed + 2.5) * 5;
									enviromentMap[xi, yi] = enviromentType(map[xi, yi], tempMap[xi, yi], rainfallMap[xi, yi]);
									switch (enviromentMap[xi, yi])
									{
										case 1:
											writeColours('+', ConsoleColor.White, ConsoleColor.DarkBlue);
											break;
										case 2:
											writeColours('#', ConsoleColor.White, ConsoleColor.DarkBlue);
											break;
										case 3:
											writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.White);
											break;
										case 4:
											writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.Yellow);
											break;
										case 5:
											writeColours('~', ConsoleColor.DarkBlue, ConsoleColor.Red);
											break;
										case 6:
											writeColours('+', ConsoleColor.White, ConsoleColor.Blue);
											break;
										case 7:
											writeColours('#', ConsoleColor.White, ConsoleColor.Blue);
											break;
										case 8:
											writeColours('~', ConsoleColor.Blue, ConsoleColor.White);
											break;
										case 9:
											writeColours('~', ConsoleColor.Blue, ConsoleColor.Yellow);
											break;
										case 10:
											writeColours('~', ConsoleColor.Blue, ConsoleColor.Red);
											break;
										case 11:
											writeColours('+', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 12:
											writeColours('#', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 13:
											writeColours('~', ConsoleColor.Cyan, ConsoleColor.White);
											break;
										case 14:
											writeColours('~', ConsoleColor.Cyan, ConsoleColor.Yellow);
											break;
										case 15:
											writeColours('~', ConsoleColor.Cyan, ConsoleColor.Red);
											break;
										case 16:
											writeColours(' ', ConsoleColor.White, ConsoleColor.White);
											break;
										case 17:
											writeColours('*', ConsoleColor.White, ConsoleColor.DarkGreen);
											break;
										case 18:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.White);
											break;
										case 19:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Gray);
											break;
										case 20:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Yellow);
											break;
										case 21:
											writeColours('~', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
											break;
										case 22:
											writeColours('n', ConsoleColor.White, ConsoleColor.Gray);
											break;
										case 23:
											writeColours('n', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 24:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Cyan);
											break;
										case 25:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Green);
											break;
										case 26:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.DarkRed);
											break;
										case 27:
											writeColours('n', ConsoleColor.Yellow, ConsoleColor.DarkYellow);
											break;
										case 28:
											writeColours('m', ConsoleColor.White, ConsoleColor.Gray);
											break;
										case 29:
											writeColours('m', ConsoleColor.White, ConsoleColor.Cyan);
											break;
										case 30:
											writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.White);
											break;
										case 31:
											writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.Cyan);
											break;
										case 32:
											writeColours('m', ConsoleColor.DarkYellow, ConsoleColor.Gray);
											break;
										case 33:
											writeColours('I', ConsoleColor.DarkYellow, ConsoleColor.DarkRed);
											break;
										case 34:
											writeColours('n', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 35:
											writeColours('V', ConsoleColor.Gray, ConsoleColor.Cyan);
											break;
										case 36:
											writeColours('V', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 37:
											writeColours('A', ConsoleColor.Gray, ConsoleColor.Cyan);
											break;
										case 38:
											writeColours('A', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 39:
											writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkYellow);
											break;
										case 40:
											writeColours('V', ConsoleColor.Gray, ConsoleColor.White);
											break;
										case 41:
											writeColours('A', ConsoleColor.Gray, ConsoleColor.White);
											break;
										case 42:
											writeColours('*', ConsoleColor.White, ConsoleColor.DarkGreen);
											break;
										case 43:
											writeColours('*', ConsoleColor.Green, ConsoleColor.DarkGreen);
											break;
										case 44:
											writeColours('*', ConsoleColor.DarkYellow, ConsoleColor.Green);
											break;
										case 45:
											writeColours('n', ConsoleColor.Green, ConsoleColor.DarkGreen);
											break;
										case 46:
											writeColours('n', ConsoleColor.DarkYellow, ConsoleColor.Green);
											break;
										case 47:
											writeColours('m', ConsoleColor.Green, ConsoleColor.DarkGreen);
											break;
										case 48:
											writeColours('m', ConsoleColor.Gray, ConsoleColor.DarkGray);
											break;
										case 49:
											writeColours('/', ConsoleColor.Green, ConsoleColor.Yellow);
											break;
										case 50:
											writeColours('S', ConsoleColor.DarkCyan, ConsoleColor.DarkGreen);
											break;
										case 51:
											writeColours('n', ConsoleColor.Green, ConsoleColor.Red);
											break;
										case 52:
											writeColours('n', ConsoleColor.Green, ConsoleColor.DarkRed);
											break;
										case 53:
											writeColours('V', ConsoleColor.Cyan, ConsoleColor.White);
											break;
										case 54:
											writeColours('~', ConsoleColor.Green, ConsoleColor.Cyan);
											break;
										default:
											break;
									}
								}
								Console.BackgroundColor = ConsoleColor.Black;
								Console.WriteLine();
							}
							keypress = Console.ReadKey();
							if (keypress.Key == ConsoleKey.UpArrow)
							{
								xChange -= 2;
							}
							else if (keypress.Key == ConsoleKey.DownArrow)
							{
								xChange += 2;
							}
							else if (keypress.Key == ConsoleKey.LeftArrow)
							{
								yChange -= 2;
							}
							else if (keypress.Key == ConsoleKey.RightArrow)
							{
								yChange += 2;
							}
							Console.Clear();
						}
						break;
					case 3:
						//Set and Draw Height Map
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								map[xi, yi] = Perlin.perlin(xi / scale, yi / scale, seed + 0.5) * 9 - 1;
							}
						}
						Draw.HeightMap(map);
						//Finds Lowest Direction for each tile
						for (int xi = 1; xi < (x - 1); xi++)
						{
							for (int yi = 1; yi < (y - 1); yi++)
							{
								lowestTerrain = map[xi, yi];
								lowestDirection[xi, yi] = 0;
								if (map[xi, yi] < map[xi - 1, yi - 1])
								{
									lowestTerrain = map[xi - 1, yi - 1];
									lowestDirection[xi, yi] = 1;
								}
								if (map[xi, yi] < map[xi, yi - 1])
								{
									if (map[xi, yi - 1] < lowestTerrain)
									{
										lowestTerrain = map[xi, yi - 1];
										lowestDirection[xi, yi] = 2;
									}
								}
								if (map[xi, yi] < map[xi + 1, yi - 1])
								{
									if (map[xi + 1, yi - 1] < lowestTerrain)
									{
										lowestTerrain = map[xi + 1, yi - 1];
										lowestDirection[xi, yi] = 3;

									}
								}
								if (map[xi, yi] < map[xi - 1, yi])
								{
									if (map[xi - 1, yi] < lowestTerrain)
									{
										lowestTerrain = map[xi - 1, yi];
										lowestDirection[xi, yi] = 4;
									}
								}
								if (map[xi, yi] < map[xi + 1, yi])
								{
									if (map[xi + 1, yi] < lowestTerrain)
									{
										lowestTerrain = map[xi + 1, yi];
										lowestDirection[xi, yi] = 5;
									}
								}
								if (map[xi, yi] < map[xi - 1, yi + 1])
								{

									if (map[xi - 1, yi + 1] < lowestTerrain)
									{
										lowestTerrain = map[xi - 1, yi + 1];
										lowestDirection[xi, yi] = 6;
									}
								}
								if (map[xi, yi] < map[xi, yi + 1])
								{
									if (map[xi, yi + 1] < lowestTerrain)
									{
										lowestTerrain = map[xi, yi + 1];
										lowestDirection[xi, yi] = 7;
									}
								}
								if (map[xi, yi] < map[xi + 1, yi + 1])
								{
									if (map[xi + 1, yi + 1] < lowestTerrain)
									{
										lowestTerrain = map[xi + 1, yi + 1];
										lowestDirection[xi, yi] = 8;
									}
								}
								water[xi, yi] = lowestTerrain;
							}
						}
						//Shows how water will flow
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								switch (water[xi, yi])
								{
									//None
									case double n when (n < 0.5):
										Console.BackgroundColor = ConsoleColor.DarkRed;
										break;
									//Low
									case double n when (n < 1 & n >= 0.5):
										Console.BackgroundColor = ConsoleColor.Red;
										break;
									//Medium
									case double n when (n < 1.5 & n >= 1):
										Console.BackgroundColor = ConsoleColor.Yellow;
										break;
									//High
									case double n when (n < 2.5 & n >= 1.5):
										Console.BackgroundColor = ConsoleColor.Green;
										break;
									//Very High
									case double n when (n >= 2.5):
										Console.BackgroundColor = ConsoleColor.DarkCyan;
										break;
									default:
										break;
								}
								Console.Write(" ");
							}
							Console.BackgroundColor = ConsoleColor.Black;
							Console.WriteLine();
						}
						//Create Rivers using Perlin Noise
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								if (Perlin.perlin(yi + 0.5, seed + 0.5, xi + 0.5) < 0.15)
								{
									river[xi, yi] = true;
								}
								else
								{
									river[xi, yi] = false;
								}
							}
						}
						//Go through 50 iteration of water moving down
						for (int i = 0; i < 50; i++)
						{
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									//Setting the lowest terrain to river that is adjacent to a river
									if (map[xi, yi] < 2.5)
									{
										river[xi, yi] = false;
									}
									if (river[xi, yi])
									{
										switch (lowestDirection[xi, yi])
										{
											case 0:
												river[xi, yi] = false;
												break;
											case 1:
												river[xi - 1, yi - 1] = true;

												break;
											case 2:
												river[xi, yi - 1] = true;
												break;
											case 3:
												river[xi + 1, yi - 1] = true;
												break;
											case 4:
												river[xi - 1, yi] = true;
												break;
											case 5:
												river[xi + 1, yi] = true;
												break;
											case 6:
												river[xi - 1, yi + 1] = true;
												break;
											case 7:
												river[xi, yi + 1] = true;
												break;
											case 8:
												river[xi + 1, yi + 1] = true;
												break;
											default:
												break;
										}
										Console.BackgroundColor = ConsoleColor.Blue;
									}
									else
									{
										//Graphics
										switch (map[xi, yi])
										{
											case double n when (n < 1):
												Console.BackgroundColor = ConsoleColor.DarkBlue;
												break;
											case double n when (n < 2 & n >= 1):
												Console.BackgroundColor = ConsoleColor.Blue;
												break;
											case double n when (n < 3 & n >= 2):
												Console.BackgroundColor = ConsoleColor.Cyan;
												break;
											case double n when (n < 4 & n >= 3):
												Console.BackgroundColor = ConsoleColor.Green;
												break;
											case double n when (n < 5 & n >= 4):
												Console.BackgroundColor = ConsoleColor.DarkGreen;
												break;
											case double n when (n < 6 & n >= 5):
												Console.BackgroundColor = ConsoleColor.Gray;
												break;
											case double n when (n < 6.5 & n >= 6):
												Console.BackgroundColor = ConsoleColor.DarkGray;
												break;
											case double n when (n >= 6.5):
												Console.BackgroundColor = ConsoleColor.White;
												break;
											default:
												break;
										}
									}
									Console.Write(" ");
								}
								Console.BackgroundColor = ConsoleColor.Black;
								Console.WriteLine();
							}
						}
						keypress = Console.ReadKey();
						break;
					case 4:
						//Draw Map
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								noise = Perlin.perlin(xi / scale, yi / scale, seed + 0.5);
								map[xi, yi] = noise * 9 - 1;
								switch (map[xi, yi])
								{
									case double n when (n < 1):
										Console.BackgroundColor = ConsoleColor.DarkBlue;
										break;
									case double n when (n < 2 & n >= 1):
										Console.BackgroundColor = ConsoleColor.Blue;
										break;
									case double n when (n < 3 & n >= 2):
										Console.BackgroundColor = ConsoleColor.Cyan;
										break;
									case double n when (n < 4 & n >= 3):
										Console.BackgroundColor = ConsoleColor.Green;
										break;
									case double n when (n < 5 & n >= 4):
										Console.BackgroundColor = ConsoleColor.DarkGreen;
										break;
									case double n when (n < 6 & n >= 5):
										Console.BackgroundColor = ConsoleColor.Gray;
										break;
									case double n when (n < 6.5 & n >= 6):
										Console.BackgroundColor = ConsoleColor.DarkGray;
										break;
									case double n when (n >= 6.5):
										Console.BackgroundColor = ConsoleColor.White;
										break;
									default:
										break;
								}
								Console.Write(" ");
							}
							Console.BackgroundColor = ConsoleColor.Black;
							Console.WriteLine();
						}
						//Create Water Sources
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								if (Perlin.perlin(yi + 9.5, seed + 8.5, xi + 0.5) > 0.9 && map[xi, yi] > 3)
								{
									river[xi, yi] = true;
								}
								else
								{
									river[xi, yi] = false;
								}
							}
						}
						//Locate Nearest Water Without going uphill
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								nearestWater[xi, yi] = 1028;
							}
						}
						for (int i = 0; i < 100; i++)
						{
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									if (map[xi, yi] < 3)
									{
										nearestWater[xi, yi] = 0;
									}
									else
									{
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												if ((a != 0 && b != 0) && ((b + yi < y && b + yi >= 0) && (a + xi < x && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] - 14 - Convert.ToInt32(map[xi, yi]))
													{
														if (Math.Abs(a) == Math.Abs(b))
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 14 + (Convert.ToInt32(map[xi + a, yi + b]) * 5);
														}
														else
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 10 + (Convert.ToInt32(map[xi + a, yi + b]) * 5);
														}
													}

												}


											}
										}
									}
									//switch (nearestWater[xi, yi])
									//			{
									//				case int n when (n < 1):
									//					Console.BackgroundColor = ConsoleColor.Blue;
									//					break;
									//				case int n when (n < 50 & n >= 1):
									//					Console.BackgroundColor = ConsoleColor.Cyan;
									//					break;
									//				case int n when (n < 100 & n >= 50):
									//					Console.BackgroundColor = ConsoleColor.Green;
									//					break;
									//				case int n when (n < 150 & n >= 100):
									//					Console.BackgroundColor = ConsoleColor.Yellow;
									//					break;
									//				case int n when (n < 200 & n >= 159):
									//					Console.BackgroundColor = ConsoleColor.Red;
									//					break;
									//				case int n when (n >= 200):
									//					Console.BackgroundColor = ConsoleColor.DarkRed;
									//					break;
									//				default:
									//					break;
									//			}
									//Console.Write(" ");
								}
								//Console.BackgroundColor = ConsoleColor.Black;
								//Console.WriteLine();
							}
							//Console.Read();
							//Console.Clear();
						}
						Console.ForegroundColor = ConsoleColor.Blue;
						for (int i = 0; i < 100; i++)
						{
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									if (map[xi, yi] > 3 && river[xi, yi])
									{
										if (map[xi, yi] < 3)
										{
											nearestWater[xi, yi] = 0;
										}
										else
										{
											for (int a = -1; a < 2; a++)
											{
												for (int b = -1; b < 2; b++)
												{
													if ((a != 0 && b != 0) && ((b + yi < y && b + yi >= 0) && (a + xi < x && a + xi >= 0)))
													{
														if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] - 14 - Convert.ToInt32(map[xi, yi]))
														{
															if (Math.Abs(a) == Math.Abs(b) & river[xi, yi])
															{
																nearestWater[xi, yi] = nearestWater[xi + a, yi + b] - 10;
															}
															else if (Math.Abs(a) == Math.Abs(b))
															{
																nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 14 + (Convert.ToInt32(map[xi + a, yi + b]) * 5);
															}
															else
															{
																nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 10 + (Convert.ToInt32(map[xi + a, yi + b]) * 5);
															}
														}

													}


												}
											}
										}
										toWater[2] = 999999999;
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												if ((a != 0 && b != 0) && ((b + yi < y && b + yi >= 0) && (a + xi < x && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] && nearestWater[xi + a, yi + b] < toWater[2])
													{
														toWater[0] = xi + a;
														toWater[1] = yi + b;
														if (river[xi + a, yi + b])
														{
															toWater[2] = nearestWater[xi + a, yi + b] - 10;
														}
														else
														{
															toWater[2] = nearestWater[xi + a, yi + b];
														}
													}
												}
											}
										}
										if (toWater[2] != 999999999)
										{
											river[toWater[0], toWater[1]] = true;
										}

									}
									switch (nearestWater[xi, yi])
									{
										case int n when (n < 1):
											Console.BackgroundColor = ConsoleColor.Blue;
											break;
										case int n when (n < 50 & n >= 1):
											Console.BackgroundColor = ConsoleColor.Cyan;
											break;
										case int n when (n < 100 & n >= 50):
											Console.BackgroundColor = ConsoleColor.Green;
											break;
										case int n when (n < 150 & n >= 100):
											Console.BackgroundColor = ConsoleColor.Yellow;
											break;
										case int n when (n < 200 & n >= 159):
											Console.BackgroundColor = ConsoleColor.Red;
											break;
										case int n when (n >= 200):
											Console.BackgroundColor = ConsoleColor.DarkRed;
											break;
										default:
											break;
									}
									Console.Write(" ");
								}
								Console.BackgroundColor = ConsoleColor.Black;
								Console.WriteLine();
							}
							Console.Read();
							Console.Clear();
						}
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								switch (map[xi, yi])
								{
									case double n when (n < 1):
										Console.BackgroundColor = ConsoleColor.DarkBlue;
										break;
									case double n when (n < 2 & n >= 1):
										Console.BackgroundColor = ConsoleColor.Blue;
										break;
									case double n when (n < 3 & n >= 2):
										Console.BackgroundColor = ConsoleColor.Cyan;
										break;
									case double n when (n < 4 & n >= 3):
										Console.BackgroundColor = ConsoleColor.Green;
										break;
									case double n when (n < 5 & n >= 4):
										Console.BackgroundColor = ConsoleColor.DarkGreen;
										break;
									case double n when (n < 6 & n >= 5):
										Console.BackgroundColor = ConsoleColor.Gray;
										break;
									case double n when (n < 6.5 & n >= 6):
										Console.BackgroundColor = ConsoleColor.DarkGray;
										break;
									case double n when (n >= 6.5):
										Console.BackgroundColor = ConsoleColor.White;
										break;
									default:
										break;
								}
								if (river[xi, yi])
								{
									Console.Write("~");
								}
								else
								{
									Console.Write(" ");
								}
							}
							Console.BackgroundColor = ConsoleColor.Black;
							Console.WriteLine();
						}
						Console.ForegroundColor = ConsoleColor.White;
						break;
					case 5:
						runApp = 0;
						//Menu for World Size for Diamond Square
						while (runApp == 0)
						{
							Console.WriteLine("Press [1] for Extra Small \n" +
								"Press [2] for Small \n" +
								"Press [3] for Medium\n" +
								"Press [4] for Large\n" +
								"Press [5] for Extra Large");
							keypress = Console.ReadKey();
							switch (keypress.Key)
							{
								case ConsoleKey.D1:
									runApp = 4;
									break;
								case ConsoleKey.D2:
									runApp = 5;
									break;
								case ConsoleKey.D3:
									runApp = 6;
									break;
								case ConsoleKey.D4:
									runApp = 7;
									break;
								case ConsoleKey.D5:
									runApp = 8;
									break;
								default:
									runApp = 6;
									break;
							}
							Console.Clear();
							//Setting the map size to be compatible with Diamond Square
							temp = 2;
							for (int i = 0; i < runApp; i++)
							{
								temp *= 2;
							}
							temp++;
							//Set map to Diamond Square result and outputing result
							map = new double[temp, temp];
							map = DiamondSquare(runApp, 8, -5, 10, 1.75, seed);
							Draw.HeightMap(map);
							Console.ReadKey();
						}
						//Setting values to be compatible with new map size
						x = temp;
						y = temp;
						water = new double[x, y];
						lowestDirection = new byte[x, y];
						lowestTerrain = 0;
						river = new bool[x, y];
						nearestWater = new int[x, y];
						//Create Water Sources
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								if (Perlin.perlin(yi + 9.5, seed + 8.5, xi + 0.5) > 0.9 && map[xi, yi] > 3)
								{
									river[xi, yi] = true;
								}
								else
								{
									river[xi, yi] = false;
								}
							}
						}
						//Locate Nearest Water Without going uphill
						for (int xi = 0; xi < x; xi++)
						{
							for (int yi = 0; yi < y; yi++)
							{
								nearestWater[xi, yi] = int.MaxValue;
							}
						}
						//Search via Dijskra
						for (int i = 0; i < 100; i++)
						{
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									if (map[xi, yi] < 3)
									{
										nearestWater[xi, yi] = 0;
									}
									else
									{
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												if ((a != 0 && b != 0) && ((b + yi < y && b + yi >= 0) && (a + xi < x && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] - 14 - Convert.ToInt32(map[xi, yi]))
													{
														if (Math.Abs(a) == Math.Abs(b))
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 14 + Convert.ToInt32(map[xi + a, yi + b]);
														}
														else
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 10 + Convert.ToInt32(map[xi + a, yi + b]);
														}
													}

												}


											}
										}
									}
								}
							}
						}
						Console.ForegroundColor = ConsoleColor.Blue;
						//Pathfind via Dijskra
						for (int i = 0; i < 100; i++)
						{
							for (int xi = 0; xi < x; xi++)
							{
								for (int yi = 0; yi < y; yi++)
								{
									if (map[xi, yi] > 3 && river[xi, yi])
									{
										toWater[2] = int.MaxValue;
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												if ((a != 0 && b != 0) && ((b + yi < y && b + yi >= 0) && (a + xi < x && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] && nearestWater[xi + a, yi + b] < toWater[2])
													{
														toWater[0] = xi + a;
														toWater[1] = yi + b;
														toWater[2] = nearestWater[xi + a, yi + b];
													}

												}
											}
										}
										if (toWater[2] != int.MaxValue)
										{
											river[toWater[0], toWater[1]] = true;
										}
									}
								}
							}
						}
						Draw.Rivers(map, river);
						break;
					case 6:
						runApp = 0;
						//Menu to set World Size for Diamond Square
						while (runApp == 0)
						{
							Console.WriteLine("Press [1] for Extra Small \n" +
								"Press [2] for Small \n" +
								"Press [3] for Medium\n" +
								"Press [4] for Large\n" +
								"Press [5] for Extra Large");
							keypress = Console.ReadKey();
							switch (keypress.Key)
							{
								case ConsoleKey.D1:
									runApp = 4;
									break;
								case ConsoleKey.D2:
									runApp = 5;
									break;
								case ConsoleKey.D3:
									runApp = 6;
									break;
								case ConsoleKey.D4:
									runApp = 7;
									break;
								case ConsoleKey.D5:
									runApp = 8;
									break;
								default:
									runApp = 6;
									break;
							}
							Console.Clear();
							//Sets map size to be compatible with Diamond Square
							temp = 2;
							for (int i = 0; i < runApp; i++)
							{
								temp *= 2;
							}
							temp++;
							//Creates Diamond Square map
							map = new double[temp, temp];
							map = DiamondSquare(runApp, 8, -5, 10, 2, seed);
							Draw.HeightMap(map);
							Console.ReadKey();
						}
						//Sets varibles to fit Diamond Square
						x = temp;
						y = temp;
						water = new double[x, y];
						river = new bool[x, y];
						//Gereates rivers
						river = Rivers.SeaToRivers(seed, x, y, map);
						Draw.Rivers(map, river);
						Console.ReadKey();
						break;
					case 7:
						runApp = 0;
						//Menu to set World Size for Diamond Square
						while (runApp == 0)
						{
							Console.WriteLine("Press [1] for Extra Small \n" +
								"Press [2] for Small \n" +
								"Press [3] for Medium\n" +
								"Press [4] for Large\n" +
								"Press [5] for Extra Large");
							keypress = Console.ReadKey();
							switch (keypress.Key)
							{
								case ConsoleKey.D1:
									runApp = 4;
									break;
								case ConsoleKey.D2:
									runApp = 5;
									break;
								case ConsoleKey.D3:
									runApp = 6;
									break;
								case ConsoleKey.D4:
									runApp = 7;
									break;
								case ConsoleKey.D5:
									runApp = 8;
									break;
								default:
									runApp = 6;
									break;
							}
							Console.Clear();
							//Sets map size to be compatible with Diamond Square
							temp = 2;
							for (int i = 0; i < runApp; i++)
							{
								temp *= 2;
							}
							temp++;
							//Creates Diamond Square map
							map = new double[temp, temp];
							map = DiamondSquare(runApp, 8, -5, 10, 2, seed);
							Draw.HeightMap(map);
							Console.ReadKey();
						}
						//Sets varibles to fit Diamond Square
						x = temp;
						y = temp;
						water = new double[x, y];
						river = new bool[x, y];
						river = Rivers.RiverToSea(seed, x, y, map);
						Draw.Rivers(map, river);
						Console.ReadKey();
						break;
					default:
						break;
				}
				runApp = 0;
				keypress = Console.ReadKey();
			}

		}
		public static int enviromentType(double height, double temp, double rainfall)	//Determins the biome by the tempreture, rainfall and height map
		{
			int enviro = 0;

			switch (temp)
			{
				//Permafrost
				case double n when (n < 1):
					switch (height)
					{
						case double m when (m < 1):
							enviro = 1;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 6;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 11;
							break;
						case double m when (m < 4 & m >= 3):
							enviro = 16;
							break;
						case double m when (m < 5 & m >= 4):
							enviro = 22;
							break;
						case double m when (m < 6 & m >= 5):
							enviro = 28;
							break;
						case double m when (m < 6.5 & m >= 6):
							if (rainfall >= 3)
							{
								enviro = 53;
							}
							else
							{
								enviro = 35;
							}
							break;
						case double m when (m >= 6.5):
							enviro = 37;
							break;
						default:
							break;
					}
					break;
				//Freezing
				case double n when (n < 2 & n >= 1):
					switch (height)
					{
						case double m when (m < 1):
							enviro = 2;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 7;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 12;
							break;
						case double m when (m < 4 & m >= 3):
							if (rainfall < 3 & rainfall >= 2)
							{
								enviro = 42;
							}
							else
							{
								enviro = 17;
							}
							break;
						case double m when (m < 5 & m >= 4):
							enviro = 23;
							break;
						case double m when (m < 6 & m >= 5):
							enviro = 29;
							break;
						case double m when (m < 6.5 & m >= 6):
							if (rainfall >= 4)
							{
								enviro = 53;
							}
							else if (rainfall >= 1)
							{
								enviro = 40;
							}
							else
							{
								enviro = 35;
							}
							break;
						case double m when (m >= 6.5):
							if (rainfall <= 1)
							{
								enviro = 38;
							}
							else
							{
								enviro = 41;
							}
							break;
						default:
							break;
					}
					break;
				//Chilly
				case double n when (n < 3 & n >= 2):
					switch (height)
					{
						case double m when (m < 1):
							enviro = 3;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 8;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 13;
							break;
						case double m when (m < 4 & m >= 3):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 18;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 43;
									break;
								case double i when (i >= 3 & i < 4):
									enviro = 49;
									break;
								case double i when (i >= 4):
									enviro = 54;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 5 & m >= 4):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 24;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 45;
									break;
								case double i when (i >= 3):
									enviro = 52;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 6 & m >= 5):
							if (rainfall <= 2)
							{
								enviro = 30;
							}
							else
							{
								enviro = 47;
							}
							break;
						case double m when (m < 6.5 & m >= 6):
							if (rainfall >= 3)
							{
								enviro = 53;
							}
							else
							{
								enviro = 36;
							}
							break;
						case double m when (m >= 6.5):
							if (rainfall <= 3)
							{
								enviro = 38;
							}
							else
							{
								enviro = 41;
							}
							break;
						default:
							break;
					}
					break;
				//Temperate
				case double n when (n < 4 & n >= 3):
					switch (height)
					{
						case double m when (m < 1):
							enviro = 4;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 9;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 14;
							break;
						case double m when (m < 4 & m >= 3):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 19;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 43;
									break;
								case double i when (i >= 3 & i < 4):
									enviro = 49;
									break;
								case double i when (i >= 4):
									enviro = 54;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 5 & m >= 4):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 25;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 45;
									break;
								case double i when (i >= 3):
									enviro = 52;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 6 & m >= 5):
							if (rainfall <= 2)
							{
								enviro = 31;
							}
							else
							{
								enviro = 47;
							}
							break;
						case double m when (m < 6.5 & m >= 6):
							if (rainfall >= 2)
							{
								enviro = 53;
							}
							else
							{
								enviro = 36;
							}
							break;
						case double m when (m >= 6.5):
							if (rainfall <= 4)
							{
								enviro = 38;
							}
							else
							{
								enviro = 41;
							}
							break;
						default:
							break;
					}
					break;
				//Warm
				case double n when (n < 5 & n >= 4):
					switch (height)
					{
						case double m when (m < 1):
							enviro = 5;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 10;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 15;
							break;
						case double m when (m < 4 & m >= 3):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 20;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 44;
									break;
								case double i when (i >= 3 & i < 4):
									enviro = 49;
									break;
								case double i when (i >= 4):
									enviro = 54;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 5 & m >= 4):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 26;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 46;
									break;
								case double i when (i >= 3 & i < 4):
									enviro = 45;
									break;
								case double i when (i >= 4):
									enviro = 52;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 6 & m >= 5):
							if (rainfall <= 2)
							{
								enviro = 32;
							}
							else
							{
								enviro = 47;
							}
							break;
						case double m when (m < 6.5 & m >= 6):
							enviro = 36;
							break;
						case double m when (m >= 6.5):
							if (rainfall <= 5)
							{
								enviro = 38;
							}
							else
							{
								enviro = 41;
							}
							break;
						default:
							break;
					}
					break;
				//Hot
				case double n when (n < 6 & n >= 5):

					switch (height)
					{
						case double m when (m < 1):
							enviro = 5;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 10;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 15;
							break;
						case double m when (m < 4 & m >= 3):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 21;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 44;
									break;
								case double i when (i >= 3):
									enviro = 50;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 5 & m >= 4):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 27;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 46;
									break;
								case double i when (i >= 3 & i < 4):
									enviro = 45;
									break;
								case double i when (i >= 4):
									enviro = 51;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 6 & m >= 5):
							if (rainfall <= 1)
							{
								enviro = 33;
							}
							else if (rainfall >= 3)
							{
								enviro = 48;
							}
							else
							{
								enviro = 39;
							}
							break;
						case double m when (m < 6.5 & m >= 6):
							enviro = 36;
							break;
						case double m when (m >= 6.5):
							enviro = 38;
							break;
						default:
							break;
					}
					break;
				//Scorching
				case double n when (n >= 6):

					switch (height)
					{
						case double m when (m < 1):
							enviro = 5;
							break;
						case double m when (m < 2 & m >= 1):
							enviro = 10;
							break;
						case double m when (m < 3 & m >= 2):
							enviro = 15;
							break;
						case double m when (m < 4 & m >= 3):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 21;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 44;
									break;
								case double i when (i >= 3):
									enviro = 50;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 5 & m >= 4):
							switch (rainfall)
							{
								case double i when (i < 2):
									enviro = 27;
									break;
								case double i when (i >= 2 & i < 3):
									enviro = 46;
									break;
								case double i when (i >= 3):
									enviro = 51;
									break;
								default:
									break;
							}
							break;
						case double m when (m < 6 & m >= 5):
							if (rainfall <= 2)
							{
								enviro = 34;
							}
							else
							{
								enviro = 48;
							}
							break;
						case double m when (m < 6.5 & m >= 6):
							enviro = 36;
							break;
						case double m when (m >= 6.5):
							enviro = 38;
							break;
						default:
							break;
					}
					break;
				default:
					break;
			}
			return enviro;
		} 
		public static void writeColours(char letter, ConsoleColor bgdColour, ConsoleColor fgdColour)	//Writes characters with a forground and background colour
		{
			Console.BackgroundColor = bgdColour;
			Console.ForegroundColor = fgdColour;
			Console.Write(letter);
		}
		public class Perlin //Creates perlin noise
		{

			public static double OctavePerlin(double x, double y, double z, int octaves, double persistence)
			{
				double total = 0;
				double frequency = 1;
				double amplitude = 1;
				for (int i = 0; i < octaves; i++)
				{
					total += perlin(x * frequency, y * frequency, z * frequency) * amplitude;

					amplitude *= persistence;
					frequency *= 2;
				}

				return total;
			}

			private static readonly int[] permutation = { 151,160,137,91,90,15,					// Hash lookup table as defined by Ken Perlin.  This is a randomly
		131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,	// arranged array of all numbers from 0-255 inclusive.
		190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
		88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
		77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
		102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
		135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
		5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
		223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
		129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
		251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
		49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
		138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
	};

			private static readonly int[] p;                                                    // Doubled permutation to avoid overflow

			static Perlin()
			{
				p = new int[512];
				for (int x = 0; x < 512; x++)
				{
					p[x] = permutation[x % 256];
				}
			}

			public static double perlin(double x, double y, double z)
			{
				//if (repeat > 0)
				//{                                   // If we have any repeat on, change the coordinates to their "local" repetitions
				//	x = x % repeat;
				//	y = y % repeat;
				//	z = z % repeat;
				//}

				int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in
				int yi = (int)y & 255;                              // The left bound is ( |_x_|,|_y_|,|_z_| ) and the right bound is that
				int zi = (int)z & 255;                              // plus 1.  Next we calculate the location (from 0.0 to 1.0) in that cube.
				double xf = x - (int)x;                             // We also fade the location to smooth the result.
				double yf = y - (int)y;
				double zf = z - (int)z;
				double u = fade(xf);
				double v = fade(yf);
				double w = fade(zf);

				int a = p[xi] + yi;                             // This here is Perlin's hash function.  We take our x value (remember,
				int aa = p[a] + zi;                             // between 0 and 255) and get a random value (from our p[] array above) between
				int ab = p[a + 1] + zi;                             // 0 and 255.  We then add y to it and plug that into p[], and add z to that.
				int b = p[xi + 1] + yi;                             // Then, we get another random value by adding 1 to that and putting it into p[]
				int ba = p[b] + zi;                             // and add z to it.  We do the whole thing over again starting with x+1.  Later
				int bb = p[b + 1] + zi;                             // we plug aa, ab, ba, and bb back into p[] along with their +1's to get another set.
																	// in the end we have 8 values between 0 and 255 - one for each vertex on the unit cube.
																	// These are all interpolated together using u, v, and w below.

				double x1, x2, y1, y2;
				x1 = lerp(grad(p[aa], xf, yf, zf),          // This is where the "magic" happens.  We calculate a new set of p[] values and use that to get
							grad(p[ba], xf - 1, yf, zf),            // our final gradient values.  Then, we interpolate between those gradients with the u value to get
							u);                                     // 4 x-values.  Next, we interpolate between the 4 x-values with v to get 2 y-values.  Finally,
				x2 = lerp(grad(p[ab], xf, yf - 1, zf),          // we interpolate between the y-values to get a z-value.
							grad(p[bb], xf - 1, yf - 1, zf),
							u);                                     // When calculating the p[] values, remember that above, p[a+1] expands to p[xi]+yi+1 -- so you are
				y1 = lerp(x1, x2, v);                               // essentially adding 1 to yi.  Likewise, p[ab+1] expands to p[p[xi]+yi+1]+zi+1] -- so you are adding
																	// to zi.  The other 3 parameters are your possible return values (see grad()), which are actually
				x1 = lerp(grad(p[aa + 1], xf, yf, zf - 1),      // the vectors from the edges of the unit cube to the point in the unit cube itself.
							grad(p[ba + 1], xf - 1, yf, zf - 1),
							u);
				x2 = lerp(grad(p[ab + 1], xf, yf - 1, zf - 1),
							  grad(p[bb + 1], xf - 1, yf - 1, zf - 1),
							  u);
				y2 = lerp(x1, x2, v);

				return (lerp(y1, y2, w) + 1) / 2;                       // For convenience we bound it to 0 - 1 (theoretical min/max before is -1 - 1)
			}

			public static double grad(int hash, double x, double y, double z)
			{
				int h = hash & 15;                                  // Take the hashed value and take the first 4 bits of it (15 == 0b1111)
				double u = h < 8 /* 0b1000 */ ? x : y;              // If the most signifigant bit (MSB) of the hash is 0 then set u = x.  Otherwise y.

				double v;                                           // In Ken Perlin's original implementation this was another conditional operator (?:).  I
																	// expanded it for readability.

				if (h < 4 /* 0b0100 */)                             // If the first and second signifigant bits are 0 set v = y
					v = y;
				else if (h == 12 /* 0b1100 */ || h == 14 /* 0b1110*/)// If the first and second signifigant bits are 1 set v = x
					v = x;
				else                                                // If the first and second signifigant bits are not equal (0/1, 1/0) set v = z
					v = z;

				return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v); // Use the last 2 bits to decide if u and v are positive or negative.  Then return their addition.
			}

			public static double fade(double t)
			{
				// Fade function as defined by Ken Perlin.  This eases coordinate values
				// so that they will "ease" towards integral values.  This ends up smoothing
				// the final output.
				return t * t * t * t * (t * (t * 6 - 15) + 10);         // 6t^5 - 15t^4 + 10t^3
			}

			public static double lerp(double a, double b, double x)
			{
				return a + x * (b - a);
			}
		}
		public static double[,] DiamondSquare(int size, double roughness, int low, int high, double smoothing, int seed) //Generates Diamond Square Noise
		{
			double ROUGHNESS = roughness;
			int x = 2;
			int count = 0;
			int mountains = 0;
			int sea = 0;
			bool valid = false;

			if (size < 2)
			{
				return null;
			}
			for (int i = 0; i < size; i++)
			{
				x *= 2;
			}
			x++;
			//sets increment size
			int chunksize = x - 1;
			int half = chunksize / 2;
			double[,] map = new double[x, x];
			var dsRand = new Random(seed);
			while (!valid)
			{
				roughness = ROUGHNESS;
				sea = 0;
				mountains = 0;
				chunksize = x - 1;
				map = new double[x, x];
				//Set 4 corners
				map[0, 0] = dsRand.Next(low, high);
				map[0, x - 1] = dsRand.Next(low, high);
				map[x - 1, 0] = dsRand.Next(low, high);
				map[x - 1, x - 1] = dsRand.Next(low, high);
				while (chunksize > 1)
				{
					half = chunksize / 2;
					//Square Step
					for (int xi = 0; xi < x - chunksize; xi += chunksize)
					{
						for (int yi = 0; yi < x - chunksize; yi += chunksize)
						{
							map[xi + half, yi + half] = ((map[xi, yi] + map[xi, yi + chunksize] + map[xi + chunksize, yi] + map[xi + chunksize, yi + chunksize]) / 4) + ((dsRand.NextDouble() - 0.5) * roughness);
						}
					}
					//Diamond Step
					for (int xi = 0; xi < x; xi += half)
					{
						for (int yi = (xi + half) % chunksize; yi < x; yi += chunksize)
						{
							count = 4;
							if (xi - half >= 0)
							{
								map[xi, yi] += map[xi - half, yi];
							}
							else
							{
								count--;
							}
							if (xi + half < x)
							{
								map[xi, yi] += map[xi + half, yi];
							}
							else
							{
								count--;
							}
							if (yi - half >= 0)
							{
								map[xi, yi] += map[xi, yi - half];
							}
							else
							{
								count--;
							}
							if (yi + half < x)
							{
								map[xi, yi] += map[xi, yi + half];
							}
							else
							{
								count--;
							}
							map[xi, yi] = map[xi, yi] / count + ((dsRand.NextDouble() - 0.5) * roughness);

						}
					}
					chunksize /= 2;
					roughness /= smoothing;
				}
				for (int a = 0; a < x; a++)
				{
					for (int b = 0; b < x; b++)
					{

						switch (map[a, b])
						{
							case double n when (n < 3):
								sea++;
								break;
							case double n when (n >= 5.5):
								mountains++;
								break;
							default:
								break;
						}
					}

				}
				if (sea < (x * x / 3) & sea > (x * x / 10) & mountains > (x * x / 10) & mountains < (x * x / 5))
				{
					valid = true;
				}
			}
			return map;
		}
		public class Rivers
		{
			public static bool[,] SeaToRivers(int seed, int width, int height, double[,] heightMap) //Generating rivers by pathing rivers to the sea
			{
				int count = 0; //number of rivers generated so far
				byte lowerAreas = 0; //number of adjacent tiles lower than the current tile
				bool[,] river = new bool[width, height]; //if a tile is a river or not
				bool[,] riverBuffer = new bool[width, height]; //buffer for river for reducing runtime
				int[,] nearestWater = new int[width, height]; //stores distance to the nearest sea or river
				int[] toWater = new int[3]; //temporary storage for the lowest water so far found around a point
				List<Coordinates> waterSources = new List<Coordinates> { }; // list of river starting locations
				Random random = new Random(seed);
				//Generate river sources
				for (int i = 0; i < (Math.Sqrt(width * height) / 10) + 2; i++)
				{
					waterSources.Add(new Coordinates { xCoord = random.Next(0, width), yCoord = random.Next(0, height) });
				}
				foreach (var item in waterSources)
				{
					//Locate Nearest Water Without going uphill
					//Dijskra Search
					for (int i = 0; i < Math.Sqrt(width * height); i++)
					{
						//Clear the values for nearest water
						for (int xi = 0; xi < width; xi++)
						{
							for (int yi = 0; yi < height; yi++)
							{
								nearestWater[xi, yi] = int.MaxValue;
							}
						}
						//Set distance values from nearest water
						for (int xi = 0; xi < width; xi++)
						{
							for (int yi = 0; yi < height; yi++)
							{
								//count number of surrounding tiles that are lower
								lowerAreas = 0;
								for (int a = -1; a < 2; a++)
								{
									for (int b = -1; b < 2; b++)
									{

										if (!(a == 0 && b == 0) && ((b + yi < height && b + yi >= 0) && (a + xi < width && a + xi >= 0)) && (heightMap[xi + a, yi + b] <= heightMap[xi, yi]))
										{
											lowerAreas++;
										}
									}
								}
								//make impassable when taller than surrounding area
								if (lowerAreas >= 7)
								{
									for (int a = -1; a < 2; a++)
									{
										for (int b = -1; b < 2; b++)
										{
											if (Math.Abs(a) != Math.Abs(b) && ((b + yi < height && b + yi >= 0) && (a + xi < width && a + xi >= 0)))
											{
												nearestWater[xi + a, yi + b] = int.MaxValue - 1;
											}
										}
									}
								}
							}
						}
						//Create impassable areas for water
						for (int xi = 0; xi < width; xi++)
						{
							for (int yi = 0; yi < height; yi++)
							{
								if (heightMap[xi, yi] < 3 || river[xi, yi])
								{
									nearestWater[xi, yi] = 0;
								}
								else
								{
									if (nearestWater[xi, yi] != int.MaxValue - 1)
									{
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												if (!(a == 0 && b == 0) && ((b + yi < height && b + yi >= 0) && (a + xi < width && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] - 14 - Convert.ToInt32(heightMap[xi, yi]))
													{
														if (Math.Abs(a) == Math.Abs(b))
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 14 + Convert.ToInt32(heightMap[xi + a, yi + b]);
														}
														else
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 10 + Convert.ToInt32(heightMap[xi + a, yi + b]);
														}
													}
												}
											}
										}
									}

								}
							}
						}
						//Set new distance from nearest water
						for (int yi = 0; yi < height; yi++)
						{
							for (int xi = 0; xi < width; xi++)
							{
								if (heightMap[xi, yi] < 3 || river[xi, yi])
								{
									nearestWater[xi, yi] = 0;
								}
								else
								{
									if (nearestWater[xi, yi] != int.MaxValue - 1)
									{
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												//finds shortest distance to sea
												if (!(a == 0 && b == 0) && ((b + yi < height && b + yi >= 0) && (a + xi < width && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] - 14 - Convert.ToInt32(heightMap[xi, yi]))
													{
														if (Math.Abs(a) == Math.Abs(b))
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 14 + Convert.ToInt32(heightMap[xi + a, yi + b]);
														}
														else
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 10 + Convert.ToInt32(heightMap[xi + a, yi + b]);
														}
													}
												}
											}
										}
									}
								}
							}
						}
						for (int xi = 0; xi < width; xi++)
						{
							for (int yi = 0; yi < height; yi++)
							{
								if (heightMap[xi, yi] < 3 || river[xi, yi])
								{
									nearestWater[xi, yi] = 0;
								}
								else
								{
									if (nearestWater[xi, yi] != int.MaxValue - 1)
									{
										for (int a = -1; a < 2; a++)
										{
											for (int b = -1; b < 2; b++)
											{
												//finds shortest distance to sea
												if (!(a == 0 && b == 0) && ((b + yi < height && b + yi >= 0) && (a + xi < width && a + xi >= 0)))
												{
													if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] - 14 - Convert.ToInt32(heightMap[xi, yi]))
													{
														if (Math.Abs(a) == Math.Abs(b))
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 14 + Convert.ToInt32(heightMap[xi + a, yi + b]);
														}
														else
														{
															nearestWater[xi, yi] = nearestWater[xi + a, yi + b] + 10 + Convert.ToInt32(heightMap[xi + a, yi + b]);
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
					//insert new river from river sources
					river[item.xCoord, item.yCoord] = true;
					//Dijskra Find
					for (int i = 0; i < 256; i++)
					{
						riverBuffer = river;
						for (int xi = 0; xi < width; xi++)
						{
							for (int yi = 0; yi < height; yi++)
							{
								if (heightMap[xi, yi] > 3 && river[xi, yi])
								{
									toWater[2] = int.MaxValue;
									//Find lowest terrain nearby
									for (int a = -1; a < 2; a++)
									{
										for (int b = -1; b < 2; b++)
										{
											if ((a != 0 && b != 0) && ((b + yi < height && b + yi >= 0) && (a + xi < width && a + xi >= 0)))
											{
												if (nearestWater[xi + a, yi + b] < nearestWater[xi, yi] && nearestWater[xi + a, yi + b] < toWater[2])
												{
													toWater[0] = xi + a;
													toWater[1] = yi + b;
													toWater[2] = nearestWater[xi + a, yi + b];
												}

											}
										}
									}
									//set where river has flowed to 
									if (toWater[2] != int.MaxValue)
									{
										river[toWater[0], toWater[1]] = true;
									}
								}
							}
						}
					}
					count++;
					Console.WriteLine("Rivers Drawn: " + count);
				}
				Draw.NearestWater(nearestWater);
				Draw.Rivers(heightMap, river, waterSources);
				return river;
			}

			public static bool[,] RiverToSea(int seed, int width, int height, double[,] heightMap)
			{
				int count = 0;
				bool[,] river = new bool[width, height];
				bool[,] searched = new bool[width, height];
				bool[,] searchedBuffer = new bool[width, height];
				int[] water = new int[2];
				bool connected = false;
				bool change = true;
				int[,] distance = new int[width, height];
				int[,,] origin = new int[width, height, 2];
				bool[,] impassable = new bool[width, height];
				int[] temp = new int[2];
				List<Coordinates> waterSources = new List<Coordinates> { };
				int checkX = -1;
				int checkY = -1;
				Coordinates item = new Coordinates { };
				Random random = new Random(seed);
                for (int i = 0; i < (Math.Sqrt(width * height) / 10) + 2; i++)
				{
					checkX = random.Next(0, width);
					checkY = random.Next(0, height);
					waterSources.Add(new Coordinates { xCoord = checkX, yCoord = checkY });
					item = waterSources[i];
					temp[0] = item.xCoord;
					temp[1] = item.yCoord;
                    //Resets pathfinding values
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
							searched[x , y] = false;
							searchedBuffer[x, y] = false;
							origin[x, y, 0] = -1;
							origin[x, y, 1] = -1;
							distance[x, y] = int.MaxValue;
							impassable[x,y] = false;
                        }
                    }
                    //make impassable when taller than surrounding area
                    origin[temp[0], temp[1], 0]	= temp[0];
					origin[temp[0], temp[1], 1] = temp[1];
					water[0] = -1;
					water[1] = -1;
					searched[temp[0], temp[1]] = true;
					distance[temp[0], temp[1]] = 0;
					while (!connected && change)
                    {
						change = false;
                        //Finding nearest water
                        for (int x = width - 1; x >= 0; x--)
                        {
                            for (int y = height-1; y >= 0; y--)
                            {
                                if (searched[x, y])
                                {
                                    for (int a = -1; a < 2; a++)
                                    {
                                        for (int b = -1; b < 2; b++)
                                        {
                                            if (a + x < width && a + x >= 0 && b + y < height && b + y >= 0 && !(a == 0 && b == 0) && heightMap[x + a, y + b] > heightMap[x, y] + 0.05 && !searched[x + a, y + b])
                                            {
                                                impassable[x + a, y + b] = true;
                                            }
                                            else if (a + x < width && a + x >= 0 && b + y < height && b + y >= 0 && !(a == 0 && b == 0) && !impassable[x + a, y + b])
                                            {
                                                searched[x, y] = true;
                                                if (Math.Abs(a) == Math.Abs(b) && distance[x + a, y + b] > distance[x, y] + 14)
                                                {
                                                    distance[x + a, y + b] = distance[x, y] + 14;
													origin[x + a, y + b, 0] = x;
													origin[x + a, y + b, 1] = y;
													if (!searched[x + a, y + b])
													{
														searched[x + a, y + b] = true;
														change = true;
													}
												}
                                                else if (distance[x + a, y + b] > distance[x, y] + 10)
                                                {
                                                    distance[x + a, y + b] = distance[x, y] + 10;
													origin[x + a, y + b, 0] = x;
													origin[x + a, y + b, 1] = y;
                                                    if (!searched[x + a, y + b])
                                                    {
                                                        searched[x + a, y + b] = true;
														change = true;
                                                    }
													
                                                }
                                                if ((heightMap[x + a, y + b] < 2.9 || river[x + a, y + b]) && (water[0] == -1 && water[1] == -1))
                                                {
                                                    water[0] = x;
                                                    water[1] = y;
                                                    connected = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (int x = 0; x < height; x++)
                        {
                            for (int y = 0; y < width; y++)
                            {
                                if (searched[x, y])
                                {
                                    for (int a = -1; a < 2; a++)
                                    {
                                        for (int b = -1; b < 2; b++)
                                        {
                                            if (a + x < width && a + x >= 0 && b + y < height && b + y >= 0 && !(a == 0 && b == 0) && heightMap[x + a, y + b] > heightMap[x, y] + 0.05 && !searched[x + a, y + b])
                                            {
                                                impassable[x + a, y + b] = true;
                                            }
                                            else if (a + x < width && a + x >= 0 && b + y < height && b + y >= 0 && !(a == 0 && b == 0) && !impassable[x + a, y + b])
                                            {
                                                searched[x, y] = true;
                                                if (Math.Abs(a) == Math.Abs(b) && distance[x + a, y + b] > distance[x, y] + 14)
                                                {
                                                    distance[x + a, y + b] = distance[x, y] + 14;
													origin[x + a, y + b, 0] = x;
													origin[x + a, y + b, 1] = y;
													if (!searched[x + a, y + b])
													{
														searched[x + a, y + b] = true;
														change = true;
													}
												}
                                                else if (distance[x + a, y + b] > distance[x, y] + 10)
                                                {
                                                    distance[x + a, y + b] = distance[x, y] + 10;
													origin[x + a, y + b, 0] = x;
													origin[x + a, y + b, 1] = y;
													if (!searched[x + a, y + b])
													{
														searched[x + a, y + b] = true;
														change = true;
													}
												}
                                                
                                            }
                                        }
                                    }
                                    if ((heightMap[x, y] < 2.9 || river[x, y] == true) && (water[0] == -1 && water[1] == -1))
                                    {
                                        water[0] = x;
                                        water[1] = y;
                                        connected = true;
                                    }

                                }

                                }
                        }
                    }
					while (water[0] != item.xCoord && water[1] != item.yCoord && connected)
					{
						temp[0] = origin[water[0], water[1], 0];
						temp[1] = origin[water[0], water[1], 1];
						water[0] = temp[0];
						water[1] = temp[1];
						river[water[0], water[1]] = true;
					}
                    if (connected)
                    {
						count++;
                    }
                    else
                    {
						i--;
                    }
					connected = false;
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("Rivers Drawn: " + count);
				}
				Draw.Rivers(heightMap, river, waterSources);
				return river;
			}
		}
	public class Draw //Draw maps
		{
			public static void Rivers(double[,] heightMap, bool[,] river)
			{
				Console.ForegroundColor = ConsoleColor.Blue;
				for (int x = 0; x < heightMap.GetLength(0); x++)
				{
					for (int y = 0; y < heightMap.GetLength(0); y++)
					{
						switch (heightMap[x, y])
						{
							case double n when (n < 1):
								Console.BackgroundColor = ConsoleColor.DarkBlue;
								break;
							case double n when (n < 2 & n >= 1):
								Console.BackgroundColor = ConsoleColor.Blue;
								break;
							case double n when (n < 3 & n >= 2):
								Console.BackgroundColor = ConsoleColor.Cyan;
								break;
							case double n when (n < 4 & n >= 3):
								Console.BackgroundColor = ConsoleColor.Green;
								break;
							case double n when (n < 5 & n >= 4):
								Console.BackgroundColor = ConsoleColor.DarkGreen;
								break;
							case double n when (n < 6 & n >= 5):
								Console.BackgroundColor = ConsoleColor.Gray;
								break;
							case double n when (n < 6.5 & n >= 6):
								Console.BackgroundColor = ConsoleColor.DarkGray;
								break;
							case double n when (n >= 6.5):
								Console.BackgroundColor = ConsoleColor.White;
								break;
							default:
								break;
						}
						if (river[x, y])
						{
							Console.Write("~");
						}
						else
						{
							Console.Write(" ");
						}
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
				Console.ForegroundColor = ConsoleColor.White;
			}
			public static void Rivers(double[,] heightMap, bool[,] river, List<Coordinates> riverSources)
			{
				Console.ForegroundColor = ConsoleColor.Blue;
				byte[,] riverType = new byte[heightMap.GetLength(0), heightMap.GetLength(0)];
                foreach (var item in riverSources)
                {
					riverType[item.xCoord, item.yCoord] = 2;
                }
				for (int x = 0; x < heightMap.GetLength(0); x++)
				{
					for (int y = 0; y < heightMap.GetLength(0); y++)
					{
                        if (river[x,y] && riverType[x,y] != 2)
                        {
							riverType[x,y] = 1;
                        }
						switch (heightMap[x, y])
						{
							case double n when (n < 1):
								Console.BackgroundColor = ConsoleColor.DarkBlue;
								break;
							case double n when (n < 2 & n >= 1):
								Console.BackgroundColor = ConsoleColor.Blue;
								break;
							case double n when (n < 3 & n >= 2):
								Console.BackgroundColor = ConsoleColor.Cyan;
								break;
							case double n when (n < 4 & n >= 3):
								Console.BackgroundColor = ConsoleColor.Green;
								break;
							case double n when (n < 5 & n >= 4):
								Console.BackgroundColor = ConsoleColor.DarkGreen;
								break;
							case double n when (n < 6 & n >= 5):
								Console.BackgroundColor = ConsoleColor.Gray;
								break;
							case double n when (n < 6.5 & n >= 6):
								Console.BackgroundColor = ConsoleColor.DarkGray;
								break;
							case double n when (n >= 6.5):
								Console.BackgroundColor = ConsoleColor.White;
								break;
							default:
								break;
						}
                        switch (riverType[x,y])
                        {
							case 1:
							Console.Write("~");
								break;
							case 2:
								Console.Write("S");
								break;
                            default:
							Console.Write(" ");
                                break;
                        }
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
				Console.ForegroundColor = ConsoleColor.White;
				Console.ReadKey();
			}
			public static void NearestWater(int[,] nearestWater)
            {
                for (int x = 0; x < nearestWater.GetLength(0); x++)
                {
					for (int y = 0; y < nearestWater.GetLength(0); y++)
					{
						switch (nearestWater[x, y])
						{
							case int n when (n < 1):
								Console.BackgroundColor = ConsoleColor.Blue;
								break;
							case int n when (n < 50 & n >= 1):
								Console.BackgroundColor = ConsoleColor.Cyan;
								break;
							case int n when (n < 100 & n >= 50):
								Console.BackgroundColor = ConsoleColor.Green;
								break;
							case int n when (n < 150 & n >= 100):
								Console.BackgroundColor = ConsoleColor.Yellow;
								break;
							case int n when (n < 200 & n >= 159):
								Console.BackgroundColor = ConsoleColor.Red;
								break;
							case int n when (n >= 200 & n < int.MaxValue - 1):
								Console.BackgroundColor = ConsoleColor.DarkRed;
								break;
							case int.MaxValue - 1:
								Console.BackgroundColor = ConsoleColor.Black;
								break;
							default:
								break;
						}
						Console.Write(" ");
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
			}
			public static void HeightMap(double[,] heightMap)
			{
				for (int x = 0; x < heightMap.GetLength(0); x++)
				{
					for (int y = 0; y < heightMap.GetLength(0); y++)
					{
						switch (heightMap[x, y])
						{
							case double n when (n < 1):
								Console.BackgroundColor = ConsoleColor.DarkBlue;
								break;
							case double n when (n < 2 & n >= 1):
								Console.BackgroundColor = ConsoleColor.Blue;
								break;
							case double n when (n < 3 & n >= 2):
								Console.BackgroundColor = ConsoleColor.Cyan;
								break;
							case double n when (n < 4 & n >= 3):
								Console.BackgroundColor = ConsoleColor.Green;
								break;
							case double n when (n < 5 & n >= 4):
								Console.BackgroundColor = ConsoleColor.DarkGreen;
								break;
							case double n when (n < 6 & n >= 5):
								Console.BackgroundColor = ConsoleColor.Gray;
								break;
							case double n when (n < 6.5 & n >= 6):
								Console.BackgroundColor = ConsoleColor.DarkGray;
								break;
							case double n when (n >= 6.5):
								Console.BackgroundColor = ConsoleColor.White;
								break;
							default:
								break;
						}
						Console.Write(" ");
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
			}
			public static void DEBUG_PATHFINDING(double[,] heightMap, bool[,] impassable, bool[,] searched, Coordinates source)
			{
				for (int x = 0; x < heightMap.GetLength(0); x++)
				{
					for (int y = 0; y < heightMap.GetLength(0); y++)
					{
						if (impassable[x, y])
						{
							Console.BackgroundColor = ConsoleColor.Red;
						}
						else if (source.xCoord == x && source.yCoord == y)
						{
							Console.BackgroundColor = ConsoleColor.Blue;
						}
						else
						{
							switch (heightMap[x, y])
							{
								case double n when (n < 1):
									Console.BackgroundColor = ConsoleColor.DarkBlue;
									break;
								case double n when (n < 2 & n >= 1):
									Console.BackgroundColor = ConsoleColor.Blue;
									break;
								case double n when (n < 3 & n >= 2):
									Console.BackgroundColor = ConsoleColor.Cyan;
									break;
								case double n when (n < 4 & n >= 3):
									Console.BackgroundColor = ConsoleColor.Green;
									break;
								case double n when (n < 5 & n >= 4):
									Console.BackgroundColor = ConsoleColor.DarkGreen;
									break;
								case double n when (n < 6 & n >= 5):
									Console.BackgroundColor = ConsoleColor.Gray;
									break;
								case double n when (n < 6.5 & n >= 6):
									Console.BackgroundColor = ConsoleColor.DarkGray;
									break;
								case double n when (n >= 6.5):
									Console.BackgroundColor = ConsoleColor.White;
									break;
								default:
									break;
							}
						}
						Console.ForegroundColor = ConsoleColor.Black;
						if (searched[x, y])
						{
							Console.Write("X");
						}
						else
						{
							Console.Write(" ");
						}
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
			}
			public static void TempMap(double[,] tempMap)
			{
				for (int x = 0; x < tempMap.GetLength(0); x++)
				{
					for (int y = 0; y < tempMap.GetLength(0); y++)
					{
						switch (tempMap[x, y])
						{
							case double n when (n < 1):
								Console.BackgroundColor = ConsoleColor.DarkBlue;
								break;
							case double n when (n < 2 & n >= 1):
								Console.BackgroundColor = ConsoleColor.Blue;
								break;
							case double n when (n < 3 & n >= 2):
								Console.BackgroundColor = ConsoleColor.DarkGreen;
								break;
							case double n when (n < 4 & n >= 3):
								Console.BackgroundColor = ConsoleColor.Green;
								break;
							case double n when (n < 5 & n >= 4):
								Console.BackgroundColor = ConsoleColor.Yellow;
								break;
							case double n when (n < 6 & n >= 5):
								Console.BackgroundColor = ConsoleColor.Red;
								break;
							case double n when (n >= 6):
								Console.BackgroundColor = ConsoleColor.White;
								break;
							default:
								break;
						}
						Console.Write(" ");
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
			}
			public static void RainfallMap(double[,] rainfallMap)
			{
				for (int x = 0; x < rainfallMap.GetLength(0); x++)
				{
					for (int y = 0; y < rainfallMap.GetLength(0); y++)
					{
						switch (rainfallMap[x, y])
						{
							//None
							case double n when (n < 1):
								Console.BackgroundColor = ConsoleColor.DarkRed;
								break;
							//Low
							case double n when (n < 2 & n >= 1):
								Console.BackgroundColor = ConsoleColor.Red;
								break;
							//Medium
							case double n when (n < 3 & n >= 2):
								Console.BackgroundColor = ConsoleColor.Yellow;
								break;
							//High
							case double n when (n < 4 & n >= 3):
								Console.BackgroundColor = ConsoleColor.Green;
								break;
							//Very High
							case double n when (n >= 4):
								Console.BackgroundColor = ConsoleColor.DarkCyan;
								break;
							default:
								break;
						}
						Console.Write(" ");
					}
					Console.BackgroundColor = ConsoleColor.Black;
					Console.WriteLine();
				}
			}
		}
		public class Coordinates
		{
			public int xCoord { get; set; }
			public int yCoord { get; set; }
		}
		
	}
}
