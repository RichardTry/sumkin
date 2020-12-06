using System;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace threads
{
    class MainClass
    {
        static Thread t0;
        static Thread t1;
        static Thread t2;
        static Thread t_sort;
        static double time0, time1, time2;

        public static void Main(string[] args)
        {
            t0 = new Thread(T);
            t1 = new Thread(T);
            t2 = new Thread(T);

            t0.Name = "t0";
            t1.Name = "t1";
            t2.Name = "t2";

            time0 = 0.0;
            time1 = 0.0;
            time2 = 0.0;

            t_sort = new Thread(Sorter);
            t_sort.Start();
        }

        static private void Sorter()
        {
            while (true)
            {
                if (time0 <= time1 && time0 <= time2)
                {
                    t0.Start();
                    t0.Join();
                    if (time1 >= time2)
                    {
                        t1.Start();
                        t1.Join();
                        t2.Start();
                        t2.Join();
                    }
                    else
                    {
                        t2.Start();
                        t2.Join();
                        t1.Start();
                        t1.Join();
                    }
                }
                else if (time1 <= time0 && time1 <= time2)
                {
                    t1.Start();
                    t1.Join();
                    if (time0 >= time2)
                    {
                        t0.Start();
                        t0.Join();
                        t2.Start();
                        t2.Join();
                    }
                    else
                    {
                        t2.Start();
                        t2.Join();
                        t0.Start();
                        t0.Join();
                    }
                }
                else
                {
                    t2.Start();
                    t2.Join();
                    if (time0 >= time1)
                    {
                        t0.Start();
                        t0.Join();
                        t1.Start();
                        t1.Join();
                    }
                    else
                    {
                        t1.Start();
                        t1.Join();
                        t0.Start();
                        t0.Join();
                    }
                }
            }

        }

        static private void T()
        {
            Console.WriteLine(Thread.CurrentThread.Name + " started");
            Stopwatch watch = Stopwatch.StartNew();
            using (StreamWriter s = new StreamWriter("/home/paxom/monodevelop-projects/file.txt", true))
            {
                Random rand = new Random();
                int r = rand.Next(1, 6);
                for (int i = 0; i < r; ++i)
                {
                    s.WriteLine(Thread.CurrentThread.Name + " " + (r - i).ToString());
                }
                s.Flush();
                s.Close();
                s.Dispose();
            }

            switch(Thread.CurrentThread.Name)
            {
                case "t0":
                    time0 = watch.Elapsed.TotalMilliseconds;
                    break;
                case "t1":
                    time1 = watch.Elapsed.TotalMilliseconds;
                    break;
                case "t2":
                    time2 = watch.Elapsed.TotalMilliseconds;
                    break;
            }
            Console.WriteLine(Thread.CurrentThread.Name + " ended");
        }
    }
}