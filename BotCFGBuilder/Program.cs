using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace BotCFGBuilder
{
    class Program
    {
        static ArrayList names = new ArrayList();
        static int MAXNAMELENGTH = 15;
        static Char[] splitspace = { ' ' };

        static void Main(string[] args)
        {
            int i;
            StreamReader input;
            StreamWriter output, output2;

            if(args.Length < 1 && !File.Exists("botnames.txt"))
            {
                Console.Beep();
                Console.WriteLine("No input file to generate names. Aborting...");
                Console.ReadKey();
                return;
            }

            // Build names...
            try
            {
                input = File.OpenText((args.Length > 0) ? args[0] : "botnames.txt");
            }
            catch (Exception e)
            {
                Console.Beep();
                Console.WriteLine("Input file error: " + e.Message.ToString() + "\nAborting...");
                Console.ReadKey();
                return;
            }

            {
                String nametemp, truncate;
                
                for(i = 0;; i++)
                {
                    nametemp = input.ReadLine();

                    if (nametemp == null)
                        break;

                    if (nametemp.Length == 0)
                    {
                        Console.WriteLine("Line " + (i + 1) + ": Skipping line break.");
                        continue;
                    }

                    // Sane names only
                    nametemp = nametemp.Replace("\"", "");

                    while (nametemp.Length > MAXNAMELENGTH)
                    {
                        Console.WriteLine("Line " + (i + 1) + ": " + nametemp.ToString() + " is too long (max 15). Truncating common seperators...");

                        truncate = nametemp.Replace(" ", "").Replace("_", "").Replace("+", "");
                        if (truncate.Length > MAXNAMELENGTH)
                        {
                            Console.WriteLine("Line " + (i + 1) + ": " + truncate + " is too long (max 15). Truncating segment...");

                            nametemp = nametemp.Split(splitspace, 2)[1];
                        }
                        else
                            nametemp = truncate;
                    }

                    if (nametemp.Length == 0)
                    {
                        Console.Beep();
                        Console.WriteLine("Line " + (i + 1) + ": Name became too small. Please review name...");
                        Console.ReadKey();
                        return;
                    }
                
                    names.Add(nametemp);
                }
            }
            input.Close();
            // Names built

            // Build config
            try
            {
                output = new StreamWriter("bots.cfg", false);
                output2 = new StreamWriter("autoexec.cfg", false);
            }
            catch (Exception e)
            {
                Console.Beep();
                Console.WriteLine("Output file error: " + e.Message.ToString() + "\nAborting...");
                Console.ReadKey();
                return;
            }

            {
                output2.Write("alias addallbots \"");

                int group = 0;
                for (i = 0; i < names.Count; i += 32, group++)
                {
                    output2.Write("addgroup" + group + ";wait 1;");
                }
                output2.Write("\"" + Environment.NewLine);

                Random PRNG;

                int ii;
                for (i = 0, group = 0; i < names.Count; group++)
                {
                    output2.Write("alias addgroup" + group + " \"");

                    for (ii = 0; ii < 32 && i < names.Count; ii++, i++)
                    {
                        PRNG = new Random(names[i].ToString().GetHashCode());

                        output.WriteLine("{");
                        output.WriteLine("  name            \"" + names[i] + "\"");
                        output.WriteLine("  aiming          " + PRNG.Next(1, 101).ToString());
                        output.WriteLine("  perfection      " + PRNG.Next(1, 101).ToString());
                        output.WriteLine("  reaction        " + PRNG.Next(1, 101).ToString());
                        output.WriteLine("  isp             " + PRNG.Next(1, 101).ToString());
                        output.WriteLine("  color           " + BuildColour(PRNG.Next(0, 256), PRNG.Next(0, 256), PRNG.Next(0, 256)));
                        output.WriteLine("  skin            base");
                        output.WriteLine("  //weaponpref    012385687");
                        output.WriteLine("}\n");

                        if (names[i].ToString().Contains(' '))
                            output2.Write("addbot \\\"" + names[i] + "\\\";wait 1;");
                        else
                            output2.Write("addbot " + names[i] + ";wait 1;");
                    }

                    output2.Write("\"" + Environment.NewLine);
                }
                
                output.Flush();
                output2.Flush();

                Console.WriteLine("bots.cfg built (" + names.Count + " names, " + (i * 11) + " lines).");
                Console.WriteLine("autoexec.cfg built.");
                output.Close();
            }

            for(i = 1000; i < 2000; i+=100)
            {
                Console.Beep(i, 100);
            }

            Console.ReadKey();
        }

        static String BuildColour (int R, int G, int B)
        {
            byte RR = (byte)R;
            byte GG = (byte)G;
            byte BB = (byte)B;
            return "\"" + RR.ToString("X") + " " + GG.ToString("X") + " " + BB.ToString("X") + "\"";
        }
    }
}
