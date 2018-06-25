#!/usr/bin/env python

# The Clear BSD License
# Copyright (c) 2015 Freescale Semiconductor, Inc.
# Copyright 2016-2017 NXP
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without modification,
# are permitted (subject to the limitations in the disclaimer below) provided
# that the following conditions are met:
#
# o Redistributions of source code must retain the above copyright notice, this list
#   of conditions and the following disclaimer.
#
# o Redistributions in binary form must reproduce the above copyright notice, this
#   list of conditions and the following disclaimer in the documentation and/or
#   other materials provided with the distribution.
#
# o Neither the name of the copyright holder nor the names of its
#   contributors may be used to endorse or promote products derived from this
#   software without specific prior written permission.
#
# NO EXPRESS OR IMPLIED LICENSES TO ANY PARTY'S PATENT RIGHTS ARE GRANTED BY THIS LICENSE.
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
# DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
# ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
# (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
# LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
# ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
# (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
# SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

from .codec import (MessageType, MessageInfo, CodecError)
from .basic_codec import BasicCodec

class WideCodec(BasicCodec):
    ## Version of this codec.
    WIDE_CODEC_VERSION = 1

    def start_write_message(self, msgInfo):
        header = (self.WIDE_CODEC_VERSION << 24) \
                        | ((msgInfo.service & 0xff) << 16) \
                        | (msgInfo.sequence & 0xFFFF)
        self.write_uint32(header)

        header =  ((msgInfo.request & 0xFFFFFF) << 8) \
                        | (msgInfo.type.value & 0xFF)
        self.write_uint32(header)

    ##
    # @return 4-tuple of msgType, service, request, sequence.
    def start_read_message(self):
        header = self.read_uint32()
        version = header >> 24
        if version != self.WIDE_CODEC_VERSION:
            raise CodecError("unsupported codec version %d" % version)

        service = (header >> 16) & 0xFF
        sequence = header & 0xFFFF

        header = self.read_uint32()
        request = (header >> 8) & 0xFFFFFF
        msgType = MessageType(header & 0xFF)
        return MessageInfo(type=msgType, service=service, request=request, sequence=sequence)
