#!/bin/bash

case "$(uname -s)" in
    Linux*)     machine=Linux;;
    Darwin*)    machine=Mac;;
esac

realpath() {
    [[ $1 = /* ]] && echo "$1" || echo "$PWD/${1#./}"
}

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
SRCDIR="$( realpath "${DIR}/../src" )"

if [[ $machine = Mac ]]
then 
    PROTOC=${HOME}/.nuget/packages/grpc.tools/1.14.1/tools/macosx_x64/protoc
    PLUGIN=${HOME}/.nuget/packages/grpc.tools/1.14.1/tools/macosx_x64/grpc_csharp_plugin
else
    PROTOC=${HOME}/.nuget/packages/grpc.tools/1.14.1/tools/linux_x64/protoc
    PLUGIN=${HOME}/.nuget/packages/grpc.tools/1.14.1/tools/linux_x64/grpc_csharp_plugin
fi

$PROTOC -I${SRCDIR}/ExpertSystem.Common/Proto --csharp_out ${SRCDIR}/ExpertSystem.Common/Generated ${SRCDIR}/ExpertSystem.Common/Proto/SocketMessages.proto ${SRCDIR}/ExpertSystem.Common/Proto/SocketExchange.proto ${SRCDIR}/ExpertSystem.Common/Proto/SocketOperations.proto --grpc_out ${SRCDIR}/ExpertSystem.Common/Generated --plugin=protoc-gen-grpc=$PLUGIN