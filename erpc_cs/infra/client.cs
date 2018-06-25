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

namespace erpc
{
    using System;

    public class Client
    {
        public class RequestError
            : Exception
        {
        }

        public class ClientManager
        {
            Transport m_transport = null;
            //Arbitrator _arbitrator = null;
            MessageBufferFactory m_messageFactory = null;
            CodecFactory m_codecFactory = null;
            //this._codecClass = codecClass;
            UInt32 _sequence = 0;
            public ClientManager()
            {
                this.m_messageFactory = new MessageBufferFactory();
                this.m_codecFactory = new WideCodecFactory();
            }

            public void setTransport(Transport transport)
            {
                this.m_transport = transport;
            }


            public Codec createBufferAndCodec()
            {
                Codec codec = m_codecFactory.create();
                if (codec == null)
                {
                    return null;
                }

                MessageBuffer message = m_messageFactory.create();
                if (message.getBuff() == null)
                {
                    return null;
                }

                codec.setBuffer(message);

                return codec;
            }
            public RequestContext createRequest(bool isOneway)
            {
                // Create codec to read and write the request.
                Codec codec = createBufferAndCodec();

                return new RequestContext(++this._sequence, codec, isOneway);
            }

            public virtual erpc_status performRequest(RequestContext request)
            {
                // Send invocation request to server.
                erpc_status err = m_transport.send(request.getCodec().getBuffer());
                if (err != erpc_status.kErpcStatus_Success)
                {
                    return err;
                }

                // If the request is oneway, then there is nothing more to do.
                if (!request.isOneway())
                {
                    // Receive reply.
                    err = m_transport.receive(request.getCodec().getBuffer());
                    if (err != erpc_status.kErpcStatus_Success)
                    {
                        return err;
                    }

                    // Check the reply.
                    err = verifyReply(request);
                    if (err != erpc_status.kErpcStatus_Success)
                    {
                        return err;
                    }
                }

                return erpc_status.kErpcStatus_Success;
            }

            public virtual erpc_status verifyReply(RequestContext request)
            {
                // Some transport layers change the request's message buffer pointer (for things like zero
                // copy support), so inCodec must be reset to work with correct buffer.
                request.getCodec().reset();

                // Extract the reply header.
                MessageType msgType = MessageType.kInvocationMessage;
                UInt32 service = 0;
                UInt32 requestNumber = 0;
                UInt32 sequence = 0;
                request.getCodec().startReadMessage(ref msgType, ref service, ref requestNumber, ref sequence);

                if (request.getCodec().getStatus() != erpc_status.kErpcStatus_Success)
                {
                    return request.getCodec().getStatus();
                }

                // Verify that this is a reply to the request we just sent.
                if (msgType != MessageType.kReplyMessage || sequence != request.getSequence())
                {
                    return erpc_status.kErpcStatus_ExpectedReply;
                }

                return erpc_status.kErpcStatus_Success;
            }

            public virtual void callErrorHandler(erpc_status err, UInt32 functionID)
            {
            }
            public void releaseRequest(RequestContext request)
            {
            }

            public class RequestContext
            {
                UInt32 m_sequence; //!< Sequence number. To be sure that reply belong to current request.
                Codec m_codec;      //!< Inout codec. Codec for receiving and sending data.
                bool m_oneway;       //!< When true, request context will be oneway type (only send data).

                public RequestContext(UInt32 sequence, Codec codec, bool isOneway)
                {
                    this.m_sequence = sequence;
                    this.m_codec = codec;
                    this.m_oneway = isOneway;
                }

                public UInt32 getSequence() { return m_sequence; }

                public Codec getCodec() { return m_codec; }

                public bool isOneway() { return m_oneway; }

                public void setIsOneway(bool oneway) { m_oneway = oneway; }
            }
        }
    }
}
