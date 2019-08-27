using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WriteScheduler.Interfaces;
using System.IO;

namespace WriteScheduler.Models
{
    class RoundRobin : IDevice
    {
        private string _fileName;
        private byte[] _fileContent;
        private double _timeQuantum = 0;
        private bool _IsProcessComplete;
        
        public RoundRobin(string fileName, byte[] data)
        {
            this._fileName = fileName;
            this._fileContent = data;//Encoding.ASCII.GetBytes(fileContent);
            _IsInitialized = true;
            
        }
        private bool _IsInitialized;

        public int PendingWrites => throw new NotImplementedException();

        public int TotalWrites => throw new NotImplementedException();

        public int TotalBytesWritten => throw new NotImplementedException();

        public bool getIsInitialized() { return _IsInitialized; }

        public bool IsProcessComplete()
        {
            return _IsProcessComplete;
        }
        public void SetProcessComplete(bool IsComplete)
        {
            this._IsInitialized = true;
            this._IsProcessComplete = IsComplete;
        }

        public void setTimeQuantum(double time)
        {
            this._timeQuantum = time;
        }

        public double getTimeQuantum()
        {
            return this._timeQuantum;
        }

        public string getFileName()
        {
            return _fileName;
        }
        public byte[] getFileContent()
        {
            return _fileContent;
        }

        public void Write(string name, byte[] data)
        {
            if (_IsProcessComplete != true)
            {
                try
                {
                    /* Operating system will create a new file, if it is already 
                    exist then file will be overwritten */
                    using (var fs = new FileStream(name, FileMode.Create, FileAccess.Write))
                    {
                        //Writing block of bytes to the file stream
                        fs.Write(data, 0, data.Length);
                        _IsProcessComplete = true;
                        Console.WriteLine("file " + name + " written successfully\n");
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
}
