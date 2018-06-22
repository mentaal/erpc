namespace erpc {
    using System;
    using System.Threading;

    public class Transport {

        public Transport() {
        }

        public virtual erpc_status send(MessageBuffer message)
        {
            throw new NotImplementedException();
        }

        public virtual erpc_status receive(MessageBuffer message)
        {
            throw new NotImplementedException();
        }
    }

    public class FramedTransport
        : Transport {

        public object HEADER_LEN = 4;
        private Object thisLock = new Object();

        public FramedTransport() {
        }

        public override erpc_status send(MessageBuffer message) {
            //Mutex::Guard lock(m_sendLock);
            UInt16[] h = new UInt16[2];
            byte[] hb = new byte[4];
            lock (thisLock)
            {
                // Send header first.
                h[0] = (UInt16)message.getUsed(); //.m_messageSize
                h[1] = (UInt16)CRC16.calc(message.getBuff(), message.getUsed());//.m_crc
                Buffer.BlockCopy(h, 0, hb, 0, hb.Length);
                erpc_status ret = _base_send(hb, hb.Length);
                if (ret != erpc_status.kErpcStatus_Success)
                {
                    return ret;
                }

                // Send the rest of the message.
                return _base_send(message.getBuff(), (int)message.getUsed());
            }
        }

        public override erpc_status receive(MessageBuffer message) {
            UInt16[] h = new UInt16[2];
            byte[] hb = new byte[4];

            lock (thisLock) {
                // Receive header first.
                erpc_status ret = _base_receive(hb, hb.Length);
                if (ret != erpc_status.kErpcStatus_Success)
                {
                    return ret;
                }
                Buffer.BlockCopy(hb, 0, h, 0, hb.Length);

                // Receive rest of the message now we know its size.
                ret = _base_receive(message.getBuff(), h[0]);
                if (ret != erpc_status.kErpcStatus_Success)
                {
                    return ret;
                }


                // Verify CRC.
                UInt16 computedCrc = CRC16.calc(message.getBuff(), h[0]);
                if (computedCrc != h[1])
                {
                    return erpc_status.kErpcStatus_CrcCheckFailed;
                }

                message.setUsed(h[0]); //msg size
                return erpc_status.kErpcStatus_Success;
            }
        }

        public virtual erpc_status _base_send(byte[] data, int length) {
            throw new NotImplementedException();
        }

        public virtual erpc_status _base_receive(byte[] data, int length) {
            throw new NotImplementedException();
        }
    }

    public class ConnectionClosed
        : Exception {
    }

}
