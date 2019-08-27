using WriteScheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteScheduler.Models
{
    class CustomWriter : IDevice
    {
        public string _fileName;
        public byte[] _fileData;
        
        public CustomWriter(string name, byte[] data)
        {
            this._fileName = name;
            this._fileData = data;
        }

        public int PendingWrites => throw new NotImplementedException();

        public int TotalWrites => throw new NotImplementedException();

        public int TotalBytesWritten => throw new NotImplementedException();

        public void Write(string name, byte[] data)
        {
            try
            {
                /* Operating system will create a new file, if it is already 
                exist then file will be overwritten */
                using (var fs = new FileStream(name, FileMode.Create, FileAccess.Write))
                {
                    //Writing block of bytes to the file stream
                    fs.Write(data, 0, data.Length);
                    Console.WriteLine("file " + name + " written successfully");
                }
            }
            catch (IOException e)
            {
                // Extract some information from this exception 
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);

            }
        }
    }
}
