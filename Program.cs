using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace threads
{
    class MainClass
    {
        static Thread t0;
        static Thread t1;
        static Thread t2;
        static Thread t_sort;
        static double time0, time1, time2, myTime;
        static bool isFirst = true;
        static bool startFirst = true;

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
                Stopwatch watch = Stopwatch.StartNew();
                MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("Shared", 16);
                if (!startFirst)
                {
                    double firstRunned = 0.0;
                    while (firstRunned == 0.0)
                    {
                        using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(isFirst ? 8 : 0, 8, MemoryMappedFileAccess.Read))
                        {
                            firstRunned = reader.ReadDouble(0);
                        }
                    }
                }
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

                myTime = watch.Elapsed.TotalMilliseconds;
                using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(isFirst ? 0 : 8, 8))
                {
                    writer.Write(0, myTime);
                }

                double oppTime = 0.0;
                while (oppTime == 0.0)
                {
                    using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(isFirst ? 8 : 0, 8, MemoryMappedFileAccess.Read))
                    {
                        oppTime = reader.ReadDouble(0);
                    }
                }

                if (myTime <= oppTime)
                {
                    startFirst = true;
                }
                else
                {
                    startFirst = false;
                    using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, 16))
                    {
                        writer.Write(0, 0.0);
                        writer.Write(8, 0.0);
                    }
                }
            }
        }

        static private void T()
        {
            Stopwatch watch = Stopwatch.StartNew();
            using (StreamWriter s = new StreamWriter("/home/paxom/monodevelop-projects/file.txt", true))
            {
                Random rand = new Random();
                int r = rand.Next(1000, 10001);
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
        }
    }
}
