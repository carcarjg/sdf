#region --- Copyright Information --- 
/*
 *******************************************************************
|                                                                   |
|           OpenNETCF Smart Device Framework 2.2                    |
|                                                                   |
|                                                                   |
|       Copyright (c) 2000-2008 OpenNETCF Consulting LLC            |
|       ALL RIGHTS RESERVED                                         |
|                                                                   |
|   The entire contents of this file is protected by U.S. and       |
|   International Copyright Laws. Unauthorized reproduction,        |
|   reverse-engineering, and distribution of all or any portion of  |
|   the code contained in this file is strictly prohibited and may  |
|   result in severe civil and criminal penalties and will be       |
|   prosecuted to the maximum extent possible under the law.        |
|                                                                   |
|   RESTRICTIONS                                                    |
|                                                                   |
|   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           |
|   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          |
|   SECRETS OF OPENNETCF CONSULTING LLC THE REGISTERED DEVELOPER IS |
|   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    |
|   CONTROLS AS PART OF A COMPILED EXECUTABLE PROGRAM ONLY.         |
|                                                                   |
|   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      |
|   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        |
|   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       |
|   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  |
|   AND PERMISSION FROM OPENNETCF CONSULTING LLC                    |
|                                                                   |
|   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       |
|   ADDITIONAL RESTRICTIONS.                                        |
|                                                                   |
 ******************************************************************* 
*/
#endregion



using System;
using System.Text;
using System.IO;
using OpenNETCF.Win32;

namespace OpenNETCF.Media.WaveAudio
{

	internal class RiffChunk: _RiffChunk
	{
		public RiffChunk(Stream st): base(new FourCC('R', 'I', 'F', 'F'), st)
		{
            m_posStart = 0;
            m_nSize = 0;
		}

        public override void LoadFromStream(Stream st)
        {
            byte[] data = new byte[4];
            m_posStart = st.Position - 4;
            st.Read(data, 0, 4);
            m_nSize = BitConverter.ToInt32(data, 0);

            st.Read(data, 0, 4);
            if (Encoding.ASCII.GetString(data, 0, 4) != "WAVE")
                throw new InvalidOperationException("Must have WAVE format");

            while (st.Position - m_posStart != m_nSize + 8)
            {
                MMChunk ck = MMChunk.FromStream(st);
                if (ck == null)
                    break;
                m_chunkList.Add(ck);
            }
        }

        public override MMChunk CreateSubChunk(FourCC fourCC, Stream st)
        {
            MMChunk ck = base.CreateSubChunk(fourCC, st);
            long size = 0;
            foreach (MMChunk cck in m_chunkList)
            {
                if (cck != ck)
                    size += cck.Size;
            }
            ck.Start = m_posStart + 4 + size;
            return ck;
        }

        public override Stream BeginWrite()
        {
            base.BeginWrite();
            Stream stm = base.BeginWrite();
            Write(Encoding.ASCII.GetBytes("WAVE"));
            return stm;
        }

        public override long Size
        {
            get
            {
                long size = 4; // + ckSize
                foreach (MMChunk ck in m_chunkList)
                {
                    size += ck.Size + 8;
                }
                return size;
            }
            set
            {
            }
        }

        public override void EndWrite()
        {
            _stm.Position = m_posStart;
            Write(BitConverter.GetBytes((int)m_fourCC));
            _stm.Write(BitConverter.GetBytes((int)Size), 0, 4);
            _stm.Position = m_posCurrent;
        }
	}

	internal class FmtChunk: _RiffChunk
	{
		private WaveFormat2 m_fmt;
		public WaveFormat2 WaveFormat { get { return m_fmt; } set { m_fmt = value; } }
		public FmtChunk(Stream st, WaveFormat2 fmt): base(new FourCC('f', 'm', 't', ' '), st)
		{
			m_fmt = fmt;
		}

		public override long Size
		{
            get
            {
                return m_fmt.GetBytes().Length;
            }
		}
		public override void LoadFromStream(Stream st)
		{
            byte[] data = new byte[4];
            m_posStart = st.Position - 4;
            st.Read(data, 0, 4);
            m_nSize = BitConverter.ToInt32(data, 0);
            m_fmt = WaveFormat2.FromStream(st);
			st.Position = Start + 8 + m_nSize;
		}

        public override void CalculateSize()
        {
            
        }
	}

	internal class DataChunk: _RiffChunk
	{
		public DataChunk(Stream st): base(new FourCC('d', 'a', 't', 'a'), st)
		{
		}

        //public override int GetSize()
        //{
        //    return base.GetSize () + m_nSize;
        //}

        //public override void LoadFromStream(Stream st)
        //{
        //    m_posStart = st.Position - 4;
        //    byte[] data = new byte[4];
        //    st.Read(data, 0, 4);
        //    m_nSize = BitConverter.ToInt32(data, 0);
        //    st.Position += m_nSize;
        //}

		override public void EndWrite()
		{
			//m_nSize = (int)(_stm.Position - m_posStart) - 8;
			_stm.Position = m_posStart;
			Write(BitConverter.GetBytes((int)m_fourCC));
			_stm.Write(BitConverter.GetBytes(m_nSize), 0, 4);
			_stm.Position = m_posCurrent;
		}

        public override void CalculateSize()
        {
            
        }

        public long DataStart { get { return Start + 8; } }
        //public long DataSize { get { return m_nSize; } }
	}
}
 

 