using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WriteScheduler.Interfaces;
using WriteScheduler.Models;

namespace WriteScheduler.Schedulers
{
    class WriteScheduler : IWriteScheduler,IDevice
    {
        
        private int _pendingWrites=0;
        private int _totalWrites=0;
        private int _totalBytesWritten=0;
        private List<CustomWriter> lstCustomWriter = new List<CustomWriter>();
       
        public int PendingWrites
        {
            get
            {
                return _pendingWrites;
            }
        }

        public int TotalWrites
        {
            get
            {
                return _totalWrites;
            }
        }

        public int TotalBytesWritten
        {
            get
            {
                return _totalBytesWritten;
            }

        }

        public void RunWriteScheduler()
        {
            //Initialize files and their content
            //Interface reference is type casted to CustomWriter class 
            CustomWriter cr1 = (CustomWriter)Write("cr1", Encoding.ASCII.GetBytes("1234567898"));
            CustomWriter cr2 = (CustomWriter)Write("cr2", Encoding.ASCII.GetBytes("this a first model for custom writer"));
            CustomWriter cr3 = (CustomWriter)Write("cr3", Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz"));
            CustomWriter cr4 = (CustomWriter)Write("cr4", Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            CustomWriter cr5 = (CustomWriter)Write("cr5", Encoding.ASCII.GetBytes("abcdefgh123456890ABCDEFGHI"));

            lstCustomWriter.Add(cr1);
            lstCustomWriter.Add(cr2);
            lstCustomWriter.Add(cr3);
            lstCustomWriter.Add(cr4);
            lstCustomWriter.Add(cr5);


            //Initializing properties
            _pendingWrites = lstCustomWriter.Count;
            _totalWrites = 0;
            _totalBytesWritten = 0;

            //we are assuming that there is a file already written
            cr1.Write(cr1._fileName, cr1._fileData);
            UpdateProperties(cr1);

            //we will process the file which has minimum byte count in comparision with totalBytesWritten/totalWrites
            //we are optimizing through writing smallest files first
            while (lstCustomWriter.Count != 0)
            {
                //just to ensure we have a variable in tempCr
                var tempCr = lstCustomWriter.First();
                foreach (var cr in lstCustomWriter)
                {

                    if (cr._fileData.Count() < (this.TotalBytesWritten / this.TotalWrites))
                    {
                        tempCr = cr;
                    }
                    else
                    {
                        if(tempCr._fileData.Count() > cr._fileData.Count())
                        {
                            tempCr = cr;
                        }
                    }
                }
                tempCr.Write(tempCr._fileName, tempCr._fileData);
                UpdateProperties(tempCr);
            }



        }

        public void UpdateProperties(CustomWriter cr)
        {
            //Update properties in every write
            lstCustomWriter.Remove(cr);
            _pendingWrites--;
            _totalWrites++;
            _totalBytesWritten += cr._fileData.Count();
            Console.WriteLine("Total Bytes Writtern: {0}",_totalBytesWritten);
            Console.WriteLine("Total Writes: {0}",_totalWrites);
            Console.WriteLine("Pending Writes: {0}\n",_pendingWrites);
        }

        public IDevice Write(string name, byte[] data)
        {
            return new CustomWriter(name,data);
        }

        void IDevice.Write(string name, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
