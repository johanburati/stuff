//make csc.bat ed2k.cs
// Johan Burati (johan.burati@gmail.com)
// Code is licensed under GNU GPL license.
using System;
using System.IO;

public class MyClass {
    static uint Protocol_PartSize = 9000;

    private ArrayList m_HashSet;


    public static void Main(string[] args) {
        try {
            if (args.Length == 0) {
                usage();
                return;
            }
            string file = args[0];
            if(File.Exists(file)) {
                FileInfo fi = new FileInfo(file);
                Console.WriteLine("{0}({1}):{2}", file, fi.Length,GetChunksCount((uint)fi.Length));
            } else {
                Console.WriteLine("Error: Could not find '{0}'.", file);
            }
        } catch(Exception exc) {
            Console.WriteLine("ERROR: {0}", exc.Message);
        }
        //RL();
    }
    private static void usage() {
        Console.WriteLine("Usage: ed2k <filename>\n");
    }

    public static uint GetChunksCount(uint size) {
        uint nChunks=(((size % Protocol_PartSize)>0))? (uint)1:(uint)0;
        nChunks+=(size/Protocol_PartSize);
        return nChunks;
    }
    public byte[] DoFileHash(string fullPathFileName) {
        FileStream file;
        try {
            file=new FileStream(fullPathFileName,FileMode.Open,FileAccess.Read,FileShare.Read);
            byte[] result = DoFileHash(file);
            file.Close();
            file=null;
            return result;
        }
        public byte[] DoFileHash(FileStream file) {
            byte[] chunk=new Byte[Protocol_PartSize];
            byte[] lastchunk;
            int readedBytes;
            uint acumul=0;
            uint nChunks=GetChunksCount((uint)file.Length);
            m_HashSet.Clear();
            // bool salir=false;
            //
            // while (!salir)
            // {
            // try
            // {
            // file.Lock(0,file.Length-1);
            // salir=true;
            // }
            // catch(Exception e)
            // {
            // Debug.WriteLine(e.ToString());
            // Thread.Sleep(500);
            // }
            // }
            file.Seek(0,SeekOrigin.Begin);
            while (nChunks>0) {
                if (nChunks==1) {
                    lastchunk=new byte[file.Length-acumul];
                    file.Read(lastchunk,0,(int)(file.Length-acumul));
                    m_HashSet.Add(HashChunk(lastchunk));
                } else {
                    readedBytes=file.Read(chunk,0,(int)Protocol.PartSize);
                    acumul+=(uint)readedBytes;
                    m_HashSet.Add(HashChunk(chunk));
                    GC.Collect();
                }
                nChunks--;
            }
            // file.Unlock(0,file.Length-1);
            chunk=null;
            lastchunk=null;
            GC.Collect();
            if (m_HashSet.Count==1) {
                byte[] resHashSet=(byte [])m_HashSet[0];
                m_HashSet.Clear();
                return resHashSet;
            } else
                return DoHashSetHash(m_HashSet);
        }

        public static byte[] HashChunk(byte[] chunk) {
            byte[] resultHash;
            MD4 md4=new MD4();
            resultHash=md4.GetByteHashFromBytes(chunk);
            return resultHash;
        }
        #region Helper methods

        private static void WL(object text, params object[] args) {
            Console.WriteLine(text.ToString(), args);
        }

        private static void RL() {
            Console.ReadLine();
        }

        private static void Break() {
            System.Diagnostics.Debugger.Break();
        }

        #endregion
    }
