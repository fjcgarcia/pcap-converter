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
using System.Linq;
using System.Text;
using HDF5DotNet;
using System.Diagnostics;
using System.IO;

namespace HD5toPcapConverter.FileReaders
{
    public class HDFDataSetMember
    {
        public string Name { get; private set; }
        public H5T.H5TClass ClassID { get; private set; }
        public int Offset { get; private set; }
        //public long Storage { get; private set; }
        public HDFDataType MemberType { get; private set; }

        public HDFDataSetMember(H5DataTypeId typeid, int n)
        {
            ClassID = H5T.getMemberClass(typeid, n);
            Offset = H5T.getMemberOffset(typeid, n);
            Name = H5T.getMemberName(typeid, n);
            H5DataTypeId mtype = H5T.getMemberType(typeid, n);
            MemberType = new HDFDataType(mtype);
        }

        public override string ToString()
        {
            return Name + " - " + ClassID.ToString() + " offset " + Offset.ToString() + " - member type " + MemberType.ToString();
        }
    }

    public class HDFDataType
    {
        public H5DataTypeId Typeid { get; private set; }
        public H5T.H5TClass ClassID { get; private set; }
        public int Size { get; private set; }
        public HDFDataSetMember[] Members { get; private set; }

        public HDFDataType(H5DataTypeId typeid)
        {
            Typeid = typeid;
            ClassID = H5T.getClass(typeid);
            Size = H5T.getSize(typeid);

            if (ClassID == H5T.H5TClass.COMPOUND)
            {
                int nMembers = H5T.getNMembers(typeid);
                Members = new HDFDataSetMember[nMembers];
                for (int n = 0; n < nMembers; n++)
                {
                    Members[n] = new HDFDataSetMember(typeid, n);
                }
            }
        }

        public override string ToString()
        {
            String s = "";
            s += ClassID.ToString() + ":" + Size.ToString();
            if (Members != null)
            {
                //s += Environment.NewLine + " - " + Members.Length + " members...";
                for (int i = 0; i < Members.Length; i++)
                {
                    s += Environment.NewLine + "    " + Members[i].ToString();
                }
            }
            return s;
        }
    }

    public class HDFFile : HDFFileOrGroup
    {
        private HDFFile(H5FileId fileId)
        {
            m_id = fileId;
            m_locId = fileId;
            m_objectWithAttributes = fileId;
        }

        public HDFFile(string fname, H5F.OpenMode mode)
            : this(H5F.open(fname, mode))
        {
        }

        public HDFFile(string fname, H5F.CreateMode mode)
            : this(H5F.create(fname, mode))
        {
        }
    }

    public class HDFGroup : HDFFileOrGroup
    {
        internal HDFGroup(H5GroupId groupId)
        {
            m_id = groupId;
            m_locId = groupId;
            m_objectWithAttributes = groupId;
        }
    }

    public class HDFFileOrGroup
    {
        protected H5FileOrGroupId m_id;
        protected H5LocId m_locId;
        protected H5ObjectWithAttributes m_objectWithAttributes;

        public H5FileOrGroupId Id
        {
            get { return m_id; }
        }

        public H5LocId LocId
        {
            get { return m_locId; }
        }

        public H5ObjectWithAttributes ObjectWithAttributes
        {
            get { return m_objectWithAttributes; }
        }

        internal HDFFileOrGroup()
        {
        }

        public HDFGroup OpenGroup(string name)
        {
            return new HDFGroup(H5G.open(m_locId, name));
        }

        public HDFGroup CreateGroup(string name)
        {
            return new HDFGroup(H5G.create(m_locId, name));
        }

        static Dictionary<Type, H5DataTypeId> hdfTypes = new Dictionary<Type, H5DataTypeId>();
        static HDFFileOrGroup()
        {
            hdfTypes.Add(typeof(float), new H5DataTypeId(H5T.H5Type.NATIVE_FLOAT));
            hdfTypes.Add(typeof(double), new H5DataTypeId(H5T.H5Type.NATIVE_DOUBLE));
            hdfTypes.Add(typeof(int), new H5DataTypeId(H5T.H5Type.NATIVE_INT));
            hdfTypes.Add(typeof(byte), new H5DataTypeId(H5T.H5Type.STD_U8LE));
        }

        public void SaveScalar(string setName, bool value)
        {
            SaveScalar(setName, value, "");
        }

        public void SaveScalar(string setName, bool value, string attr)
        {
            SaveScalar(setName, value ? (int)1 : (int)0, attr);
        }

        public void SaveScalar<T>(string setName, T value)
        {
            SaveScalar<T>(setName, value, "");
        }

        public void SaveScalar<T>(string setName, T value, string attr)
        {
            long[] dims = new long[1];
            dims[0] = 1;
            H5DataSpaceId spaceId = H5S.create_simple(dims.Length, dims);

            H5DataTypeId type = hdfTypes[typeof(T)];
            H5DataSetId dataSetId = H5D.create(m_id, setName, type, spaceId);

            H5D.writeScalar(dataSetId, type, ref value);

            AddAttribute(dataSetId, attr);

            H5S.close(spaceId);
            H5D.close(dataSetId);
        }

        public T ReadScalar<T>(string name) where T : new()
        {
            T value = new T();
            H5DataSetId setid = null;

            try
            {
                setid = H5D.open(Id, name);
                HDFDataSet dataSet1 = new HDFDataSet(setid);

                string errMsg = "";
                bool okToRead = true;

                if (dataSet1.NDims != 1)
                {
                    errMsg += "DataSet for " + name + " has " + dataSet1.NDims + " dimensions - it should be one.";
                    okToRead = false;
                }
                if (dataSet1.Dims[0] != 1)
                {
                    errMsg += "DataSet for " + name + " has a length of " + dataSet1.Dims[0] + " - it should be one.";
                    okToRead = false;
                }
                HDFDataType t = new HDFDataType(hdfTypes[typeof(T)]);
                if (dataSet1.DataType.ClassID != t.ClassID)
                {
                    errMsg += "DataSet for " + name + " appears to be of ClassId " + dataSet1.DataType.ClassID;
                    okToRead = false;
                }
                if (dataSet1.DataType.Size != t.Size)
                {
                    errMsg += "DataSet for " + name + " appears to be of Size " + dataSet1.DataType.Size;
                    okToRead = false;
                }

                //value = (T)dataSet1.Read();
                if (okToRead)
                {
                    H5D.readScalar<T>(dataSet1.Id, dataSet1.DataType.Typeid, ref value);
                }
                else
                {
                    throw new InvalidDataException("can't read \"" + name + "\" in the file. (" + errMsg + ").");
                }

                Debug.WriteLine(name + " = " + value);
            }
            catch (Exception e1)
            {
                throw new InvalidDataException("can't read \"" + name + "\" in the file. (" + e1.Message.Replace("\n", " ").Trim() + ").");
            }
            finally
            {
                if (setid != null)
                {
                    H5D.close(setid);
                }
            }
            return value;
        }

        public void SaveComplexDataSet<T>(string setName, T[,] rawData, string attr)
        {
            long[] dims = new long[2];
            dims[0] = rawData.GetLength(0);
            dims[1] = 1;

            H5DataSpaceId spaceId = H5S.create_simple(dims.Length, dims);

            int sz = 0;
            if (typeof(T) == typeof(float))
            {
                sz = 4;
            }
            if (typeof(T) == typeof(double))
            {
                sz = 8;
            }
            if (sz == 0)
            {
                throw new InvalidDataException("can't write \"" + setName + "\", Can only write complex float or doubles");
            }

            H5DataTypeId complexTypeId = H5T.create(H5T.CreateClass.COMPOUND, 2 * sz);
            H5T.insert(complexTypeId, "real", 0, H5T.H5Type.NATIVE_FLOAT);
            H5T.insert(complexTypeId, "imag", sz, H5T.H5Type.NATIVE_FLOAT);

            H5DataSetId dataSetId = H5D.create(m_id, setName, complexTypeId, spaceId);

            H5Array<T> array = new H5Array<T>(rawData);

            H5D.write(dataSetId, complexTypeId, array);

            AddAttribute(dataSetId, attr);

            H5S.close(spaceId);
            H5D.close(dataSetId);
        }

        public T[,] ReadComplexDataSet<T>(string name)
        {
            H5DataSetId setid = null;
            T[,] value = null;
            try
            {
                setid = H5D.open(Id, name);
                HDFDataSet dataSet1 = new HDFDataSet(setid);

                string errMsg = "";
                bool okToRead = true;

                if (dataSet1.NDims != 2)
                {
                    errMsg += "DataSet for " + name + " has " + dataSet1.NDims + " dimensions - it should be two.";
                    okToRead = false;
                }
                if (dataSet1.Dims[0] <= 0)
                {
                    errMsg += "DataSet for " + name + " has a length of " + dataSet1.Dims[0] + " - it should be greater than one.";
                    okToRead = false;
                }
                if (dataSet1.Dims[1] != 1)
                {
                    errMsg += "DataSet for " + name + " has a second length of " + dataSet1.Dims[1] + " - it should be one.";
                    okToRead = false;
                }

                bool validType = false;
                if (typeof(T) == typeof(float))
                {
                    validType = true;
                    if ((!(dataSet1.DataType.Members[0].ClassID == H5T.H5TClass.FLOAT) &&
                        (dataSet1.DataType.Members[0].MemberType.Size == 4) &&
                        (dataSet1.DataType.Members[1].ClassID == H5T.H5TClass.FLOAT) &&
                        (dataSet1.DataType.Members[1].MemberType.Size == 4)))
                    {
                        errMsg += "DataSet for " + name + " doesn't look like Complex Float Data.";
                        okToRead = false;
                    }
                }
                if (typeof(T) == typeof(double))
                {
                    validType = true;
                    if ((!(dataSet1.DataType.Members[0].ClassID == H5T.H5TClass.FLOAT) &&
                        (dataSet1.DataType.Members[0].MemberType.Size == 8) &&
                        (dataSet1.DataType.Members[1].ClassID == H5T.H5TClass.FLOAT) &&
                        (dataSet1.DataType.Members[1].MemberType.Size == 8)))
                    {
                        errMsg += "DataSet for " + name + " doesn't look like Complex Double Data.";
                        okToRead = false;
                    }
                }
                if (!validType)
                {
                    errMsg += "Can only read complex float or doubles";
                    okToRead = false;
                }

                if (okToRead)
                {
                    long len = dataSet1.Dims[0];
                    value = new T[len, 2];
                    H5Array<T> hsarray = new H5Array<T>(value);
                    H5D.read(dataSet1.Id, dataSet1.DataType.Typeid, hsarray);

                    Debug.WriteLine(name + " = array of complex " + typeof(T) + " " + value.GetLength(0) + " long");
                }
                else
                {
                    throw new InvalidDataException("can't read \"" + name + "\" in the file. (" + errMsg + ").");
                }

            }
            catch (Exception e1)
            {
                throw new InvalidDataException("can't read \"" + name + "\" in the file. (" + e1.Message.Replace("\n", " ").Trim() + ").");
            }
            finally
            {
                if (setid != null)
                {
                    H5D.close(setid);
                }
            }
            return value;
        }

        public void SaveArrayDataSet<T>(string setName, T[] rawData, string attr)
        {
            long[] dims = new long[1];
            dims[0] = rawData.GetLength(0);

            H5DataSpaceId spaceId = H5S.create_simple(dims.Length, dims);

            H5DataTypeId type = hdfTypes[typeof(T)];

            H5DataSetId dataSetId = H5D.create(m_id, setName, type, spaceId);

            H5Array<T> array = new H5Array<T>(rawData);

            H5D.write(dataSetId, type, array);
            AddAttribute(dataSetId, attr);

            H5S.close(spaceId);
            H5D.close(dataSetId);
        }

        public T[] ReadArray<T>(string name)
        {
            T[] value = new T[1];
            H5DataSetId setid = null;
            try
            {
                setid = H5D.open(Id, name);
                HDFDataSet dataSet = new HDFDataSet(setid);

                string errMsg = "";
                bool okToRead = true;

                if (dataSet.NDims != 1)
                {
                    errMsg += "DataSet for " + name + " has " + dataSet.NDims + " dimensions - it should be one.";
                    okToRead = false;
                }
                if (dataSet.Dims[0] <= 0)
                {
                    errMsg += "DataSet for " + name + " has a length of " + dataSet.Dims[0] + " - it should be greater than one.";
                    okToRead = false;
                }
                HDFDataType t = new HDFDataType(hdfTypes[typeof(T)]);
                if (dataSet.DataType.ClassID != t.ClassID)
                {
                    errMsg += "DataSet for " + name + " appears to be of ClassId " + dataSet.DataType.ClassID;
                    okToRead = false;
                }
                if (dataSet.DataType.Size != t.Size)
                {
                    errMsg += "DataSet for " + name + " appears to be of Size " + dataSet.DataType.Size;
                    okToRead = false;
                }

                //value = (T)dataSet1.Read();
                if (okToRead)
                {
                    long len = dataSet.Dims[0];

                    value = new T[len];

                    H5Array<T> h5arr = new H5Array<T>(value);
                    H5D.read<T>(dataSet.Id, dataSet.DataType.Typeid, h5arr);
                    Debug.WriteLine(name + " = array of " + typeof(T) + " " + value.Length + " long");
                }
                else
                {
                    throw new InvalidDataException("can't read \"" + name + "\" in the file. (" + errMsg + ").");
                }

            }
            catch (Exception e1)
            {
                throw new InvalidDataException("can't read \"" + name + "\" in the file. (" + e1.Message.Replace("\n", " ").Trim() + ").");
            }
            finally
            {
                if (setid != null)
                {
                    H5D.close(setid);
                }
            }
            return value;
        }

        private static void AddAttribute(H5ObjectWithAttributes target, string attr)
        {
            if (!String.IsNullOrWhiteSpace(attr))
            {
                H5DataTypeId strtype = H5T.copy(H5T.H5Type.C_S1);         /* Make a copy of H5T_C_S1 */
                int size = attr.Length;
                H5T.setSize(strtype, size); /* Modify the string to be of length 'size' */
                H5AttributeId attrId = H5A.create(target, "x", strtype, H5S.create_simple(1, new long[] { 1 }));
                H5A.write<byte>(attrId, strtype, new H5Array<byte>(System.Text.Encoding.ASCII.GetBytes(attr)));
            }
        }

        public void Close()
        {
            if (m_id is H5FileId)
            {
                H5F.close((H5FileId)m_id);
            }
            else if (m_id is H5GroupId)
            {
                H5G.close((H5GroupId)m_id);
            }
        }
    }

    public class HDFDataSet
    {
        H5DataSpaceId m_space;
        H5DataSetId m_dataSet;
        public H5DataSetId Id
        {
            get
            {
                return m_dataSet;
            }
        }

        public int NDims { get; private set; }
        public long[] Dims { get; private set; }
        public int NPoints { get; private set; }
        public long Storage { get; private set; }

        public HDFDataType DataType { get; private set; }

        public HDFDataSet(H5DataSetId dataSet)
        {
            m_dataSet = dataSet;
            m_space = H5D.getSpace(dataSet);

            NDims = H5S.getSimpleExtentNDims(m_space);
            Dims = H5S.getSimpleExtentDims(m_space);
            NPoints = H5S.getSimpleExtentNPoints(m_space);
            Storage = H5D.getStorageSize(dataSet);
            DataType = new HDFDataType(H5D.getType(dataSet));
        }

        object o = null;
        public object Read()
        {
            if (o == null)
            {
                switch (DataType.ClassID)
                {
                    case H5T.H5TClass.FLOAT:
                        {
                            switch (DataType.Size)
                            {
                                case 8:
                                    {
                                        double d = 0;
                                        H5D.readScalar<double>(m_dataSet, DataType.Typeid, ref d);
                                        o = d;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case H5T.H5TClass.COMPOUND:
                        {
                            if ((this.NDims == 2) && (this.Dims[1] == 1))
                            {
                                if ((this.DataType.Members.Length == 2) &&
                                    (this.DataType.Members[0].ClassID == H5T.H5TClass.FLOAT) &&
                                    (this.DataType.Members[0].MemberType.Size == 8) &&
                                    (this.DataType.Members[1].ClassID == H5T.H5TClass.FLOAT) &&
                                    (this.DataType.Members[1].MemberType.Size == 8))
                                {
                                    long len = this.Dims[0];
                                    double[,] rawData = new double[len, 2];
                                    H5Array<double> hsarray = new H5Array<double>(rawData);
                                    H5D.read(m_dataSet, this.DataType.Typeid, hsarray);
                                    o = rawData;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return o;
        }

        public override string ToString()
        {
            String s = "NDims = " + NDims + " [";
            for (int i = 0; i < Dims.Length; i++)
            {
                s += Dims[i].ToString();
                if (i != Dims.Length - 1)
                {
                    s += ",";
                }
            }
            s += "] (" + NPoints.ToString() + " points) storage " + Storage + ")" + DataType.ToString();
            return s;
        }
    }
}
