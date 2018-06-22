namespace erpc {
    using System;
    using static MessageBuffer;

    public class AdiBasicCodec : Codec
    {
        public UInt32 kAdiBasicCodecVersion = 2;

        public AdiBasicCodec() : base() {
        }

        public override void startWriteMessage(MessageType type, UInt32 service, UInt32 request, UInt32 sequence)
        {
            UInt32 header = this.kAdiBasicCodecVersion << 24 | ((service & 0xFF) << 16) | (sequence & 0xFFFF);
            this.write(header);

            header = ((request & 0xFFFFFF) << 8) | ((UInt32)type & 0xFF);
            this.write(header);
        }

        private void writeData(byte[] value)
        {
            if (m_status == erpc_status.kErpcStatus_Success)
            {
                if (value != null)
                {
                    m_status = _m_cursor.write(value);
                }
                else
                {
                    m_status = erpc_status.kErpcStatus_MemoryError;
                }
            }
        }

        public override void write(bool value)
        {
            // Make sure the bool is a single byte.
            byte v = (byte)(value==true?1:0);
            byte[] tv = new byte[1];
            tv[0] = v;
            writeData(tv);
        }

        public override void write(byte value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(Int16 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(Int32 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(Int64 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(UInt16 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(UInt32 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(UInt64 value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(float value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

        public override void write(double value)
        {
            byte[] ba = BitConverter.GetBytes(value);
            writeData(ba);
        }

//        public override void writePtr(uintptr_t value)
//        {
//            byte ptrSize = sizeof(value);
//            write(ptrSize);
//
//            writeData(value, ptrSize);
//        }

        public override void writeString(UInt32 length, byte[] value)
        {
            // Just treat the string as binary.
            writeBinary(length, value);
        }

        public override void writeBinary(UInt32 length, byte[] value)
        {
            // Write the blob length as a u32.
            write(length);

            writeData(value);
        }

        public override void startWriteList(UInt32 length)
        {
            // Write the list length as a u32.
            write(length);
        }

        public override void startWriteUnion(Int32 discriminator)
        {
            // Write the union discriminator as a u32.
            write(discriminator);
        }

        public override void writeNullFlag(bool isNull)
        {
            write((byte)(isNull ? _null_flag.kIsNull : _null_flag.kNotNull));
        }

//        public override void writeCallback(arrayOfFunPtr callbacks, byte callbacksCount, funPtr callback)
//        {
//            assert(callbacksCount > 1);
//            // callbacks = callbacks table
//            for (byte i = 0; i < callbacksCount; i++)
//            {
//                if (callbacks[i] == callback)
//                {
//                    write(i);
//                    break;
//                }
//                if (i + 1 == callbacksCount)
//                {
//                    updateStatus(kErpcStatus_UnknownCallback);
//                }
//            }
//        }

//        public override void writeCallback(funPtr callback1, funPtr callback2)
//        {
//            // callbacks = callback directly
//            // When declared only one callback function no serialization is needed.
//            if (callback1 != callback2)
//            {
//                updateStatus(kErpcStatus_UnknownCallback);
//            }
//        }

        public override void startReadMessage(ref MessageType type,
                                              ref UInt32 service,
                                              ref UInt32 request,
                                              ref UInt32 sequence)
        {
            UInt32 header=0;
            read(ref header);

            if (((header >> 24) & 0xFF) != kAdiBasicCodecVersion)
            {
                updateStatus(erpc_status.kErpcStatus_InvalidMessageVersion);
            }

            if (m_status == erpc_status.kErpcStatus_Success)
            {
                service = ((header >> 16) & 0xFF);
                sequence = header & 0xFFFF;

                read(ref header);

                request = ((header >> 8) & 0xFFFFFF);
                type = (MessageType)(header & 0xFF);
            }
        }

        private void readData(byte[] value)
        {
            if (m_status == erpc_status.kErpcStatus_Success)
            {
                if (value != null)
                {
                    m_status = _m_cursor.read(value);
                }
                else
                {
                    m_status = erpc_status.kErpcStatus_MemoryError;
                }
            }
        }

        public override void read(ref bool value)
        {
            byte[] ba = new byte[1];
            readData(ba);
            if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToBoolean(ba, 0);
            }
        }

        public override void read(ref byte value)
        {
            byte[] ba = new byte[1];
            readData(ba);
            if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = ba[0];
            }
        }

        public override void read(ref Int16 value)
        {
            byte[] ba = new byte[2];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToInt16(ba, 0);
            }
        }

        public override void read(ref Int32 value)
        {
            byte[] ba = new byte[4];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToInt32(ba, 0);
            }
        }

        public override void read(ref Int64 value)
        {
            byte[] ba = new byte[8];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToInt64(ba, 0);
            }
        }

        public override void read(ref UInt16 value)
        {
            byte[] ba = new byte[2];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToUInt16(ba, 0);
            }
        }

        public override void read(ref UInt32 value)
        {
            byte[] ba = new byte[4];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToUInt32(ba, 0);
            }
        }

        public override void read(ref UInt64 value)
        {
            byte[] ba = new byte[8];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToUInt64(ba, 0);
            }
        }

        public override void read(ref float value)
        {
            byte[] ba = new byte[8];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = (float)BitConverter.ToDouble(ba, 0);
            }
        }

        public override void read(ref double value)
        {
            byte[] ba = new byte[8];
            readData(ba);
			if (m_status == erpc_status.kErpcStatus_Success)
            {
                value = BitConverter.ToDouble(ba, 0);
            }
        }

//        public override void readPtr(ref uintptr_t value)
//        {
//            byte ptrSize;
//            read(ref ptrSize);
//
//            if (ptrSize > sizeof(value))
//            {
//                updateStatus(kErpcStatus_BadAddressScale);
//            }
//
//            readData(value, ptrSize);
//        }

        public override void readString(ref byte[] value)
        {
            readBinary(ref value);
        }

        public override void readBinary(ref byte[] value)
        {
            // Read length first as u32.
            int len = 0;
            read(ref len);

            if (m_status == erpc_status.kErpcStatus_Success)
            {

                byte[] v= new byte[len];
                readData(v);
                value = v;
            }
        }

        public override void startReadList(ref UInt32 length)
        {
            // Read list length as u32.
            read(ref length);
            if (m_status != erpc_status.kErpcStatus_Success)
            {
                length = 0;
            }
        }

        public override void startReadUnion(ref Int32 discriminator)
        {
            // Read union discriminator as u32.
            read(ref discriminator);
        }

        public override void readNullFlag(ref bool isNull)
        {
            byte flag=0;
            read(ref flag);
            if (m_status == erpc_status.kErpcStatus_Success)
            {
                isNull = (flag == (byte)_null_flag.kIsNull);
            }
        }

        //        public override void readCallback(arrayOfFunPtr callbacks, byte callbacksCount, funPtr *callback)
        //        {
        //            assert(callbacksCount > 1);
        //            // callbacks = callbacks table
        //            byte _tmp_local;
        //            read(&_tmp_local);
        //            if (m_status == erpc_status.kErpcStatus_Success)
        //            {
        //                if (_tmp_local < callbacksCount)
        //                {
        //                    *callback = callbacks[_tmp_local];
        //                }
        //                else
        //                {
        //                    m_status = kErpcStatus_UnknownCallback;
        //                }
        //            }
        //        }

        //        public override void readCallback(funPtr callbacks1, funPtr *callback2)
        //        {
        //            // callbacks = callback directly
        //            *callback2 = callbacks1;
        //        }
    }

    public class AdiBasicCodecFactory : CodecFactory
    {
        public AdiBasicCodecFactory() { }
        public override Codec create() { return new erpc.AdiBasicCodec(); }
    }
}
