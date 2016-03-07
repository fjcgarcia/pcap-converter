/*
Copyright (c) 2016, Keysight Technologies

All rights reserved.
Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation 
and/or other materials provided with the distribution.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HDF5DotNet;

namespace HD5toPcapConverter.FileReaders
{
    public partial class Hdf5FileReader : Component
    {
        List<byte[]> _data = new List<byte[]>();

        Hdf5FileReaderEnum runMode;
        protected string errorMessage = String.Empty;
        protected int Std;

        // Notification interfaces for status events
        public delegate void Hdf5FileReaderStatusEvent(object sender, Hdf5FileReaderEnum mode);
        public event Hdf5FileReaderStatusEvent StatusEvent;

        public delegate void Hdf5FileReaderReaderData(object sender, List<byte[]> data);
        public event Hdf5FileReaderReaderData DataEvent;

        HDFFile _hdstream;
        HDFGroup _hdprotocolGroup;
        string _fileName;
        int _pktCount;
        int _pktAvailable;

        bool moreData;

        public Hdf5FileReader()
        {
            InitializeComponent();
        }

        public Hdf5FileReader(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                _hdstream = new HDFFile(_fileName, H5F.OpenMode.ACC_RDONLY);
                _hdprotocolGroup = _hdstream.OpenGroup("Protocol");
                _pktAvailable = _hdprotocolGroup.ReadScalar<int>("nEntries");
                _pktCount = 0;
                moreData = true;
            }
        }

        public long PacketCount
        {
            get { return _pktCount; }
        }

        public int PacketsAvailable
        {
            get { return _pktAvailable; }
        }

        public int Std802_11
        {
            get { return Std; }
            set { Std = value; }
        }

        public Hdf5FileReaderEnum RunMode
        {
            get { return runMode; }
            set { runMode = value; }
        }

        #region Thread Control

        public enum Hdf5FileReaderEnum
        {
            Start,
            Pause,
            Stop,
            Running,
            EndData
        }

        public bool Start()
        {
            if (!reader.IsBusy)
            {
                reader.RunWorkerAsync();
                RunMode = Hdf5FileReaderEnum.Start;
                return true;
            }
            return false;
        }

        public bool Pause()
        {
            RunMode = Hdf5FileReaderEnum.Pause;
            reader.CancelAsync();
            return true;
        }

        public void Stop()
        {
            RunMode = Hdf5FileReaderEnum.Stop;
            reader.CancelAsync();
            _hdstream.Close();
        }

        void reader_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    RunMode = Hdf5FileReaderEnum.Stop;
                    DoStatusEvent();
                    throw new Exception("HDF5 File Reader Background Worker Error: ", e.Error);
                }
                else if (e.Cancelled)
                {
                    // int n = 0; // do nothing
                }
                else
                {
                    if (RunMode == Hdf5FileReaderEnum.Running)
                    {
                        if (moreData)
                        {
                            reader.RunWorkerAsync();
                        }
                        else
                        {
                            RunMode = Hdf5FileReaderEnum.Pause;
                            DoStatusEvent();
                        }
                    }
                    else if (RunMode == Hdf5FileReaderEnum.Stop)
                    {
                        DoStatusEvent();
                    }
                    else if (RunMode == Hdf5FileReaderEnum.Pause)
                    {
                        DoStatusEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                if (errorMessage != String.Empty)
                {
                    errorMessage = string.Empty;
                }
            }

        }

        void reader_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            RunMode = Hdf5FileReaderEnum.Running;
            while (moreData)
            {
                byte[] buff = GetGoodDecodeData(_hdprotocolGroup, _pktCount);
                _data.Add(buff);
                if (_pktCount >= _pktAvailable)
                    moreData = false;
                _pktCount++;
            }
            if (!moreData)
            {
                RunMode = Hdf5FileReaderEnum.EndData;
                DataEvent(this, _data);
                DoStatusEvent();
                _hdstream.Close();
            }
            e.Result = moreData;
        }

        #endregion

        #region Help Methods, Some may not be used for now

        private void DoStatusEvent()
        {
            if (StatusEvent == null)
                return;
            StatusEvent(this, RunMode);
        }

        private byte[] GetGoodDecodeData(HDFGroup hdfGroup, int packetid)
        {
            HDFGroup thisPacket = null;
            byte[] decodedData = null;
            int PSDULength = 0;
            try
            {
                thisPacket = hdfGroup.OpenGroup("Packet" + packetid.ToString("D4"));
                byte[] buff = thisPacket.ReadArray<byte>("decoded data");

                // get length of actual payload without stuffing bytes due to modulation
                HDFGroup rawDataGroup = thisPacket.OpenGroup("raw measurement data");
                //HDFGroup wiGigData = rawDataGroup.OpenGroup("WiGig");
                HDFGroup wiGigData = rawDataGroup.OpenGroup("AD");
                HDFGroup errorSummary = wiGigData.OpenGroup("Error Summary");
                PSDULength = errorSummary.ReadScalar<int>("PSDULength");

                decodedData = new byte[PSDULength];
                System.Buffer.BlockCopy(buff, 0, decodedData, 0, PSDULength);

                Console.WriteLine("PSDULength = " + PSDULength);

                return decodedData;
            }
            catch
            {
                Console.WriteLine("can't find decoded data from measid " + packetid);
                return null;
            }
            finally
            {
                if (thisPacket != null)
                    thisPacket.Close();
            }
        }

        #endregion
    }
}
