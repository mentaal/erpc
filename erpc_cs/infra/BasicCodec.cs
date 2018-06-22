/*
 * The Clear BSD License
 * Copyright (c) 2015-2016, Freescale Semiconductor, Inc.
 * Copyright 2016-2017 NXP
 * All rights reserved.
 *
 *
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted (subject to the limitations in the disclaimer below) provided
 * that the following conditions are met:
 *
 * o Redistributions of source code must retain the above copyright notice, this list
 *   of conditions and the following disclaimer.
 *
 * o Redistributions in binary form must reproduce the above copyright notice, this
 *   list of conditions and the following disclaimer in the documentation and/or
 *   other materials provided with the distribution.
 *
 * o Neither the name of the copyright holder nor the names of its
 *   contributors may be used to endorse or promote products derived from this
 *   software without specific prior written permission.
 *
 * NO EXPRESS OR IMPLIED LICENSES TO ANY PARTY'S PATENT RIGHTS ARE GRANTED BY THIS LICENSE.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace erpc {
    using System;
    using static MessageBuffer;

    public class BasicCodec : Codec
    {
        public UInt32 kBasicCodecVersion = 1;

        public BasicCodec() : base() {
        }

        public override erpc_status startWriteMessage(MessageType type, UInt32 service, UInt32 request, UInt32 sequence) {
            UInt32 header = this.kBasicCodecVersion << 24 | (service & 255) << 16 | (request & 255) << 8 | (UInt32)type & 255;
            this.write(header);
            this.write(sequence);
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status endWriteMessage() {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status write(bool value)
        {
            // Make sure the bool is a single byte.
            byte v = (byte)(value==true?1:0);
            byte[] tv = new byte[1];
            tv[0] = v;
            return _m_cursor.write(tv);
        }

        public override erpc_status write(byte value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(Int16 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(Int32 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(Int64 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(UInt16 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(UInt32 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(UInt64 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(float value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status write(double value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            return _m_cursor.write(ba);
        }

        public override erpc_status writeString(UInt32 length, byte[] value)
        {
            // Just treat the string as binary.
            return writeBinary(length, value);
        }
        public override erpc_status writeBinary(UInt32 length, byte[] value)
        {
            // Write the blob length as a u32.
            erpc_status err = write(length);
            if (err != erpc_status.kErpcStatus_Success)
            {
                return err;
            }

            return _m_cursor.write(value);
        }

        public override erpc_status startWriteList(UInt32 length)
        {
            // Write the list length as a u32.
            return write(length);
        }

        public override erpc_status endWriteList()
        {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status startWriteStruct()
        {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status endWriteStruct()
        {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status writeNullFlag(bool isNull)
        {
            return write((byte)(isNull ? 0 : 1));
        }

        public override erpc_status startReadMessage(ref MessageType type, ref UInt32 service, ref UInt32 request, ref UInt32 sequence)
        {
            UInt32 header=0;
            erpc_status err = read(ref header);
            if (err != erpc_status.kErpcStatus_Success)
            {
                return err;
            }

            if (((header >> 24) & 0xff) != kBasicCodecVersion)
            {
                return erpc_status.kErpcStatus_InvalidMessageVersion;
            }

            service = ((header >> 16) & 0xff);
            request = ((header >> 8) & 0xff);
            type = (MessageType)(header & 0xff);

            return read(ref sequence);
        }

        public override erpc_status endReadMessage()
        {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status read(ref bool value)
        {
            byte[] ba = new byte[1];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToBoolean(ba, 0);
            return err;
        }

        public override erpc_status read(ref byte value)
        {
            byte[] ba = new byte[1];
            erpc_status err = _m_cursor.read(ba);
            value = ba[0];
            return err;
        }

        public override erpc_status read(ref Int16 value)
        {
            byte[] ba = new byte[2];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToInt16(ba, 0);
            return err;
        }

        public override erpc_status read(ref Int32 value)
        {
            byte[] ba = new byte[4];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToInt32(ba, 0);
            return err;
        }

        public override erpc_status read(ref Int64 value)
        {
            byte[] ba = new byte[8];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToInt64(ba, 0);
            return err;
        }

        public override erpc_status read(ref UInt16 value)
        {
            byte[] ba = new byte[2];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToUInt16(ba, 0);
            return err;
        }

        public override erpc_status read(ref UInt32 value)
        {
            byte[] ba = new byte[4];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToUInt32(ba, 0);
            return err;
        }

        public override erpc_status read(ref UInt64 value)
        {
            byte[] ba = new byte[8];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToUInt64(ba, 0);
            return err;
        }

        public override erpc_status read(ref float value)
        {
            byte[] ba = new byte[8];
            erpc_status err = _m_cursor.read(ba);
            value = (float)BitConverter.ToDouble(ba, 0);
            return err;
        }

        public override erpc_status read(ref double value)
        {
            byte[] ba = new byte[8];
            erpc_status err = _m_cursor.read(ba);
            value = BitConverter.ToDouble(ba, 0);
            return err;
        }

        public override erpc_status readString(ref byte[] value)
        {
            return readBinary(ref value);
        }

        public override erpc_status readBinary(ref byte[] value)
        {
            // Read length first as u32.
            int len = 0;
            erpc_status err = read(ref len);
            if (err != erpc_status.kErpcStatus_Success)
            {
                return err;
            }
            byte[] v= new byte[len];
            err = _m_cursor.read(v);
            value = v;
            return err;
        }

        public override erpc_status startReadList(ref UInt32 length)
        {
            // Read list length as u32.
            return read(ref length);
        }

        public override erpc_status endReadList()
        {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status startReadStruct()
        {
            return erpc_status.kErpcStatus_Success;
        }

        public override erpc_status readNullFlag(ref bool isNull)
        {
            byte flag=0;
            erpc_status status = this.read(ref flag);
            if (status != erpc_status.kErpcStatus_Success)
            {
                return status;
            }
            isNull = (flag == 0);
            return erpc_status.kErpcStatus_Success;
        }
    }

    public class BasicCodecFactory : CodecFactory
    {
        public BasicCodecFactory() { }
        public override Codec create() { return new erpc.BasicCodec(); }
    }
}
