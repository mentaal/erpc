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

    public enum _null_flag
    {
        kNotNull = 0,
        kIsNull
    };

    public enum MessageType {
        kInvocationMessage = 0,
        kOnewayMessage = 1,
        kReplyMessage = 2,
        kNotificationMessage = 3
    }

    public class Codec
    {
        MessageBuffer _m_buffer = null;
        protected Cursor _m_cursor = null;
        protected erpc_status m_status;

        public Codec()
        {
            this._m_cursor = new Cursor();
            this.m_status = erpc_status.kErpcStatus_Success;
        }

        public MessageBuffer getBuffer() { return this._m_buffer; }

        public virtual void setBuffer(MessageBuffer buf)
        {
            this._m_buffer = buf;
            this._m_cursor.set(this._m_buffer);
        }

        /*! @brief Reset the codec to initial state. */
        public void reset() {
            //this._m_cursor.set(this._m_buffer);
            this._m_cursor.reset();

        }

        public erpc_status getStatus() { return m_status; }

        public void updateStatus(erpc_status status)
        {
            if (m_status != erpc_status.kErpcStatus_Success)
            {
                m_status = status;
            }
        }

        public Cursor getCurcor() { return this._m_cursor; }

        public virtual void startWriteMessage(MessageType type, UInt32 service, UInt32 request, UInt32 sequence) {}

        public virtual void startWriteUnion(Int32 discriminator) { }

        public virtual void endWriteMessage() {}

        public virtual void write(bool value) {}

        public virtual void write(byte value) {}

        public virtual void write(Int16 value) {}

        public virtual void write(Int32 value) {}

        public virtual void write(Int64 value) {}

        public virtual void write(UInt16 value) {}

        public virtual void write(UInt32 value) {}

        public virtual void write(UInt64 value) {}

        public virtual void write(float value) {}

        public virtual void write(double value) {}

        public virtual void writeString(UInt32 length, byte[] value) {}

        public virtual void writeBinary(UInt32 length, byte[] value) {}

        public virtual void startWriteList(UInt32 length) {}

        public virtual void startWriteStruct() {}

        public virtual void writeNullFlag(bool isNull) {}

        public virtual void startReadMessage(ref MessageType type, ref UInt32 service, ref UInt32 request, ref UInt32 sequence) {}

        public virtual void read(ref bool value) {}

        public virtual void read(ref byte value) {}

        public virtual void read(ref Int16 value) {}

        public virtual void read(ref Int32 value) {}

        public virtual void read(ref Int64 value) {}

        public virtual void read(ref UInt16 value) {}

        public virtual void read(ref UInt32 value) {}

        public virtual void read(ref UInt64 value) {}

        public virtual void read(ref float value) {}

        public virtual void read(ref double value) {}

        public virtual void readString(ref byte[] value) {}

        public virtual void readBinary(ref byte[] value) {}

        public virtual void startReadList(ref UInt32 length) {}

        public virtual void endReadList() {}

        public virtual void startReadStruct() {}

        public virtual void startReadUnion(ref Int32 discriminator) {}

        public virtual void readNullFlag(ref bool isNull) {}
    };

    public class CodecFactory
    {
        public CodecFactory() { }
        public virtual Codec create() { return new erpc.Codec(); }
    };

}
