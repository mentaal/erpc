/*
 * The Clear BSD License
 * Copyright (c) 2014, Freescale Semiconductor, Inc.
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

#ifndef _ERPC_WIDE_CODEC_H_
#define _ERPC_WIDE_CODEC_H_

#include "erpc_basic_codec.h"

/*!
 * @addtogroup infra_codec
 * @{
 * @file
 */

////////////////////////////////////////////////////////////////////////////////
// Classes
////////////////////////////////////////////////////////////////////////////////

namespace erpc {

/*!
 * @brief Wide binary serialization format.
 *
 * @ingroup infra_codec
 */
class WideCodec : public BasicCodec
{
public:
    static const uint32_t kWideCodecVersion; /*!< Codec version. */

    //! @name Encoding
    //@{
    /*!
     * @brief Prototype for write header of message.
     *
     * @param[in] type Type of message.
     * @param[in] service Which interface is requested.
     * @param[in] request Which function need be called.
     * @param[in] sequence Send sequence number to be sure that
     *                    received message is reply for current request. or write function.
     */
    virtual void startWriteMessage(message_type_t type, uint32_t service, uint32_t request, uint32_t sequence) override;

    //! @name Decoding
    //@{
    /*!
     * @brief Prototype for read header of message.
     *
     * @param[out] type Type of message.
     * @param[out] service Which interface was used.
     * @param[out] request Which function was called.
     * @param[out] sequence Returned sequence number to be sure that
     *                     received message is reply for current request.
     */
    virtual void startReadMessage(message_type_t *type, uint32_t *service, uint32_t *request, uint32_t *sequence) override;

};

/*!
 * @brief Basic codec factory implements functions from codec factory.
 *
 * @ingroup infra_codec
 */
class WideFactory : public CodecFactory
{
public:
    /*!
     * @brief Return created codec.
     *
     * @return Pointer to created codec.
     */
    virtual WideCodec *create(void) override { return new (std::nothrow) WideCodec; }

    /*!
     * @brief Dispose codec.
     *
     * @param[in] codec Codec to dispose.
     */
    virtual void dispose(Codec *codec) override { delete codec; }
};

} // namespace erpc

/*! @} */

#endif // _ERPC_WIDE_CODEC_H_
