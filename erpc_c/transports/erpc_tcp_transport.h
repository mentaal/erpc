/*
 * The Clear BSD License
 * Copyright (c) 2015, Freescale Semiconductor, Inc.
 * Copyright 2016 NXP
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
#ifndef _EMBEDDED_RPC__TCP_TRANSPORT_H_
#define _EMBEDDED_RPC__TCP_TRANSPORT_H_

#include "erpc_framed_transport.h"
#include "erpc_threading.h"

/*!
 * @addtogroup tcp_transport
 * @{
 * @file
 */

////////////////////////////////////////////////////////////////////////////////
// Classes
////////////////////////////////////////////////////////////////////////////////

namespace erpc {
/*!
 * @brief Client side of TCP/IP transport.
 *
 * @ingroup tcp_transport
 */
class TCPTransport : public FramedTransport
{
public:
    /*!
     * @brief Constructor.
     *
     * This function initializes object attributes.
     *
     * @param[in] isServer True when this transport is used for server side application.
     */
    TCPTransport(bool isServer);

    /*!
     * @brief Constructor.
     *
     * This function initializes object attributes.
     *
     * @param[in] host Specify the host name or IP address of the computer.
     * @param[in] port Specify the listening port number.
     * @param[in] isServer True when this transport is used for server side application.
     */
    TCPTransport(const char *host, uint16_t port, bool isServer);

    /*!
     * @brief TCPTransport destructor
     */
    virtual ~TCPTransport(void);

    /*!
     * @brief This function set host and port of this transport layer.
     *
     * @param[in] host Specify the host name or IP address of the computer.
     * @param[in] port Specify the listening port number.
     */
    void configure(const char *host, uint16_t port);

    /*!
     * @brief This function will create host on server side, or connect client to the server.
     *
     * @retval #kErpcStatus_Success When creating host was successful or client connected successfully.
     * @retval #kErpcStatus_UnknownName Host name resolution failed.
     * @retval #kErpcStatus_ConnectionFailure Connecting to the specified host failed.
     */
    virtual erpc_status_t open(void);

    /*!
     * @brief This function disconnects client or stop server host.
     *
     * @retval #kErpcStatus_Success Always return this.
     */
    virtual erpc_status_t close(void);

protected:
    bool m_isServer;       /*!< If true then server is using transport, else client. */
    const char *m_host;    /*!< Specify the host name or IP address of the computer. */
    uint16_t m_port;       /*!< Specify the listening port number. */
    int m_socket;          /*!< Socket number. */
#if ERPC_THREADS
    Thread m_serverThread; /*!< Pointer to server thread. */
    bool m_runServer;      /*!< Thread is executed while this is true. */
#else
    int m_serverSocket;      /*!< Server socket number*/
#endif // #if ERPC_THREADS

    /*!
     * @brief This function connect client to the server.
     *
     * @retval kErpcStatus_Success When client connected successfully.
     * @retval kErpcStatus_Fail When client doesn't connected successfully.
     */
    virtual erpc_status_t connectClient(void);

    /*!
     * @brief This function read data.
     *
     * @param[inout] data Preallocated buffer for receiving data.
     * @param[in] size Size of data to read.
     *
     * @retval #kErpcStatus_Success When data was read successfully.
     * @retval #kErpcStatus_ReceiveFailed When reading data ends with error.
     * @retval #kErpcStatus_ConnectionClosed Peer closed the connection.
     */
    virtual erpc_status_t underlyingReceive(uint8_t *data, uint32_t size);

    /*!
     * @brief This function writes data.
     *
     * @param[in] data Buffer to send.
     * @param[in] size Size of data to send.
     *
     * @retval #kErpcStatus_Success When data was written successfully.
     * @retval #kErpcStatus_SendFailed When writing data ends with error.
     * @retval #kErpcStatus_ConnectionClosed Peer closed the connection.
     */
    virtual erpc_status_t underlyingSend(const uint8_t *data, uint32_t size);

#if ERPC_THREADS
    /*!
     * @brief Server thread function.
     */
    void serverThread(void);

    /*!
     * @brief Thread entry point.
     *
     * Control is passed to the serverThread() method of the TCPTransport instance pointed to
     * by the @c arg parameter.
     *
     * @param arg Thread argument. The pointer to the TCPTransport instance is passed through
     *  this argument.
     */
    static void serverThreadStub(void *arg);
#else
    /*!
     * @brief This function calls socket accept() to wait for next connection
     *
     * @retval kErpcStatus_Success When client connected successfully.
     * @retval kErpcStatus_Fail When accept() fails
     */
    virtual erpc_status_t accept(void);

    /*!
     * @brief Override of receive which first accepts a new connection
     *
     * Same as base class except that accept is called first, then base class
     * receive is called.
     *
     * The @a message is only filled with the message data, not the frame header.
     *
     * This function is blocking.
     *
     * @param[in] message Message buffer, to which will be stored incoming message.
     *
     * @retval kErpcStatus_Success When receiving was successful.
     * @retval kErpcStatus_CrcCheckFailed When receiving failed.
     * @retval other Subclass may return other errors from the underlyingReceive() method.
     */
    virtual erpc_status_t receive(MessageBuffer *message) override;
#endif //#if ERPC_THREADS
};

} // namespace erpc

/*! @} */

#endif // _EMBEDDED_RPC__TCP_TRANSPORT_H_
