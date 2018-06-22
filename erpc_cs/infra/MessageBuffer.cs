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

using erpc;
using System;

namespace erpc
{
    public class MessageBuffer
    {
        private byte[] m_buf;
        private int m_len;
        private int m_used;

        public MessageBuffer(byte[] buffer)
        {
            m_buf = buffer;
            m_len = buffer.Length;
            m_used = 0;
            return;
        }

        public MessageBuffer(MessageBuffer mb)
        {
            m_buf = mb.m_buf;
            m_len = mb.m_len;
            m_used = mb.m_used;
            return;
        }

        public void set(byte[] buffer)
        {
            m_buf = buffer;
            m_len = buffer.Length;
            m_used = 0;
        }

        public byte[] getBuff() { return m_buf; }

        public int getLength() { return m_len; }

        public int getUsed() { return m_used; }

        public int getFree() { return (m_len - m_used); }

        public void setUsed(int used) { m_used = used; }
        //public erpc_status read(UInt32 offset, byte[] data, UInt32 length)
        //{
        //    if (offset + length > m_len)
        //    {
        //        return erpc_status.kErpcStatus_BufferOverrun;
        //    }

        //    if (length > 0)
        //    {
        //        memcpy(data, m_buf + offset, length);
        //    }

        //    return erpc_status.kErpcStatus_Success;
        //}

        //public erpc_status write(UInt16 offset, byte[] data, UInt32 length)
        //{
        //    if (offset + length > m_len)
        //    {
        //        return erpc_status.kErpcStatus_BufferOverrun;
        //    }

        //    if (length > 0)
        //    {
        //        memcpy(m_buf, data, length);
        //    }

        //    return erpc_status.kErpcStatus_Success;
        //}
        //public erpc_status copy(MessageBuffer other)
        //{
        //    //assert(m_len >= other.m_len);
        //    m_used = other.m_used;
        //    memcpy(m_buf, other.m_buf, m_used);

        //    return erpc_status.kErpcStatus_Success;
        //}
        public void swap(MessageBuffer other)
        {
            //assert(other);
            MessageBuffer temp = new MessageBuffer(other);
            other.m_len = m_len;
            other.m_used = m_used;
            other.m_buf = m_buf;
            m_len = temp.m_len;
            m_used = temp.m_used;
            m_buf = temp.m_buf;
        }

        //operator uint8_t* (){ return m_buf; }
        //operator const uint8_t*() const { return m_buf; }
        //uint8_t &operator[] (int index) { return m_buf[index]; }
        //const uint8_t &operator[] (int index) const { return m_buf[index]; }

        public class Cursor
        {
            private MessageBuffer m_buffer;
            private int m_pos;
            private int m_remaining;

            public Cursor()
            {
                m_buffer = null;
                m_pos = 0;
                m_remaining = 0;
            }

            public Cursor(MessageBuffer buffer)
            {
                m_buffer = buffer;
                m_pos = buffer.getUsed();
                m_remaining = buffer.getLength();
            }

            public void set(MessageBuffer buffer)
            {
                m_buffer = buffer;
                //assert(buffer.get() && "Data buffer wasn't set to MessageBuffer."); // receive function should return err if it couldn't set data buffer.
                m_pos = buffer.getUsed();
                m_remaining = buffer.getLength();
            }

            public void reset()
            {
                m_pos = 0;
                m_remaining = this.m_buffer.getLength();
            }

            //public uint get() { return m_pos; }

            //public UInt32 getRemaining() { return m_remaining; }

            public erpc_status read(byte[] data)
            {
                int length = data.Length;
                if (m_remaining < length)
                {
                    return erpc_status.kErpcStatus_BufferOverrun;
                }

                Buffer.BlockCopy(m_buffer.getBuff(), m_pos, data, 0, (int)length);
                m_pos += length;
                m_remaining -= length;

                return erpc_status.kErpcStatus_Success;
            }

            public erpc_status write(byte[] data)
            {
                int length = data.Length;
                if (length > m_remaining)
                {
                    return erpc_status.kErpcStatus_BufferOverrun;
                }

                Buffer.BlockCopy(data, 0, m_buffer.getBuff(), m_pos, (int)length);
                m_pos += length;
                m_remaining -= length;
                m_buffer.setUsed(m_buffer.getUsed() + length);

                return erpc_status.kErpcStatus_Success;
            }

            //operator uint8_t* (){ return m_pos; }
            //operator const uint8_t*() const { return m_pos; }
            //uint8_t &operator[] (int index) { return m_pos[index]; }
            //const uint8_t &operator[] (int index) const { return m_pos[index]; }

            //public Cursor &operator+=(uint16_t n)
            //{
            //    m_pos += n;
            //    m_remaining -= n;
            //    return *this;
            //}
            //public Cursor &operator-=(uint16_t n)
            //{
            //    m_pos -= n;
            //    m_remaining += n;
            //    return *this;
            //}

            //public Cursor &operator ++()
            //{
            //    ++m_pos;
            //    --m_remaining;
            //    return *this;
            //}
            //public Cursor &operator --()
            //{
            //    --m_pos;
            //    ++m_remaining;
            //    return *this;
            //}

        };
    };

    public class MessageBufferFactory
    {
        public MessageBufferFactory() { }

        public virtual MessageBuffer create() { return new MessageBuffer(new byte[1024]);  }

        public virtual bool createServerBuffer() { return true; }

        public virtual erpc_status prepareServerBufferForSend(MessageBuffer message)
        {
            message.setUsed(0);
            return erpc_status.kErpcStatus_Success;
        }
    };

} // namespace erpc
