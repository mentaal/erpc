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
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Arbitrator {

        struct PendingClientInfo
        {
            //RequestContext m_request;
            //Semaphore m_sem;
            //bool m_isValid;
            //PendingClientInfo m_next;

            //public PendingClientInfo() { }
        };

        //PendingClientInfo m_clientList;     //!< Active client receive requests.
        //PendingClientInfo m_clientFreeList; //!< Unused client receive info structs.
//        Mutex m_clientListMutex;             //!< Mutex guarding the client active and free lists.

        public class ClientInfo {

            public object @event = null;

            public object msg = null;
        }

        public class TransportArbitrator
            : Transport {
            Transport m_sharedTransport = null;
            Codec m_codec = null;

            public TransportArbitrator(Transport shared) {
                this.m_sharedTransport = shared;
            }

            void setCodec(Codec codec) { this.m_codec = codec; }

            public override erpc_status send(MessageBuffer message) {
                Debug.Assert(this.m_sharedTransport != null);
                return this.m_sharedTransport.send(message);
            }

            public override erpc_status receive(MessageBuffer message) {
                Debug.Assert(this.m_sharedTransport != null);
                Debug.Assert(this.m_codec != null);
                // Repeatedly receive until we get an invocation request.
                while (true)
                {
                    // Receive a message.
                    erpc_status err = this.m_sharedTransport.receive(message);
                    if (err != erpc_status.kErpcStatus_Success)
                    {
                        return err;
                    }

                    this.m_codec.setBuffer(message);

                    // Parse the message header.
                    MessageType msgType = MessageType.kInvocationMessage;
                    UInt32 service = 0;
                    UInt32 requestNumber = 0;
                    UInt32 sequence = 0;
                    err = this.m_codec.startReadMessage(ref msgType, ref service, ref requestNumber, ref sequence);
                    if (err != erpc_status.kErpcStatus_Success)
                    {
                        continue;
                    }

                    // If this message is an invocation, return it to the calling server.
                    if (msgType == MessageType.kInvocationMessage || msgType == MessageType.kOnewayMessage)
                    {
                        return erpc_status.kErpcStatus_Success;
                    }

                    // Just ignore messages we don't know what to do with.
                    if (msgType != MessageType.kReplyMessage)
                    {
                        continue;
                    }

                    // Check if there is a client waiting for this message.
                    //PendingClientInfo client = this.m_clientList;
                    //for (; client; client = client->m_next)
                    //{
                    //    if (client->m_isValid && sequence == client->m_request->getSequence())
                    //    {
                    //        // Swap the received message buffer with the client's message buffer.
                    //        client->m_request->getCodec()->getBuffer()->swap(message);

                    //        // Wake up the client receive thread.
                    //        client->m_sem.put();
                    //        break;
                    //    }
                    //}
                }
            }

            //#
            // @brief Add a client request to the client list.
            //
            // This call is made by the client thread prior to sending the invocation to the server. It
            // ensures that the transport arbitrator has the client's response message buffer ready in
            // case it sees the response before the client even has a chance to call client_receive().
            //
            // @param self
            // @param requestContext
            // @return A token value to be passed to client_receive().
            //public virtual client_token prepareClientReceive(RequestContext request)
            //{
            //    PendingClientInfo info = addPendingClient();
            //    if (!info)
            //    {
            //        return 0;
            //    }
            //    info->m_request = &request;
            //    info->m_isValid = true;
            //    return reinterpret_cast<client_token_t>(info);
            //}

            //#
            // @brief Receive method for the client.
            //
            // Blocks until the a reply message is received with the expected sequence number that is
            // associated with @a token. The client must have called prepare_client_receive() previously.
            //
            // @param self
            // @param token The token previously returned by prepare_client_receive().
            // @return bytearray containing the received message.
            //public virtual erpc_status clientReceive(client_token token)
            //{
            //    //assert(token != 0 && "invalid client token");

            //    //// Convert token to pointer to info struct for this client receive request.
            //    //PendingClientInfo* info = reinterpret_cast<PendingClientInfo*>(token);

            //    //// Wait on the semaphore until we're signaled.
            //    //info.m_sem.get(Semaphore::kWaitForever);

            //    //removePendingClient(info);

            //    return erpc_status.kErpcStatus_Success;
            //}
        }
    }
}
