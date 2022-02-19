using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdgChallenge.Model.File
{
    public abstract class NativeFile
    {
        public string FullPath { get; set; }
        public string Path { get => System.IO.Path.GetDirectoryName(FullPath); }
        public string Name { get => System.IO.Path.GetFileName(FullPath); }
        public string Extension { get => System.IO.Path.GetExtension(FullPath); }

        public bool IsTemporary { get; set; }

        public NativeFile(string path) => FullPath = System.IO.Path.GetFullPath(path);
        public NativeFile() : this(System.IO.Path.GetTempFileName()) { IsTemporary = true; }

        public abstract void Read(Stream stream);
        public abstract void Write(Stream stream);
    }
}
