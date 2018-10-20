#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
SRCDIR="$(readlink -m "${DIR}/../src")"

PROTOC=${HOME}/.nuget/packages/grpc.tools/1.14.1/tools/linux_x64/protoc
PLUGIN=${HOME}/.nuget/packages/grpc.tools/1.14.1/tools/linux_x64/grpc_csharp_plugin

$PROTOC -I${SRCDIR}/ExpertSystem.Common/Proto --csharp_out ${SRCDIR}/ExpertSystem.Common/Generated  ${SRCDIR}/ExpertSystem.Common/Proto/CustomSocket.proto --grpc_out ${SRCDIR}/ExpertSystem.Common/Generated --plugin=protoc-gen-grpc=$PLUGIN