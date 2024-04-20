//Written for Order of War. https://store.steampowered.com/app/34600
using System.IO;

namespace Order_of_War_Extractor
{
    class Program
    {
        static BinaryReader br;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            br.BaseStream.Position = br.BaseStream.Length - 12;
            int metaTableSize = br.ReadInt32();
            int metaTableStart = br.ReadInt32();
            br.BaseStream.Position = br.BaseStream.Length - (12 + metaTableSize);

            System.Collections.Generic.List<MetaTable> files = new();
            while(br.BaseStream.Position < br.BaseStream.Length - 12)
                files.Add(ReadMeta());

            foreach (MetaTable file in files)
            {
                if (file.start == -1)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "\\" + file.name);
                    continue;
                }

                br.BaseStream.Position = file.start;
                BinaryWriter bw = new BinaryWriter(File.OpenWrite(Path.GetDirectoryName(args[0]) + "\\" + file.name));
                bw.Write(br.ReadBytes(file.size));
                bw.Close();
            }
        }

        public static MetaTable ReadMeta()
        {
            int size = br.ReadInt32();
            return new MetaTable()
            {
                start = br.ReadInt32(),
                size = br.ReadInt32(),
                unknown2 = br.ReadInt32(),
                name = new(System.Text.Encoding.GetEncoding("UTF-16").GetChars(br.ReadBytes(size - 16)))
            };
        }

        public struct MetaTable
        {
            public int start;
            public int size;
            public int unknown2;
            public string name;
        }
    }
}