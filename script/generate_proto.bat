@rem Generate the C# code for .proto files

setlocal
cd /d %~dp0

set PROTOC=%UserProfile%\.nuget\packages\Google.Protobuf.Tools\3.6.1\tools\windows_x64\protoc.exe
set PLUGIN=%UserProfile%\.nuget\packages\Grpc.Tools\1.14.1\tools\windows_x64\grpc_csharp_plugin.exe

%PROTOC% -I../src/ExpertSystem.Common/Proto --csharp_out ../src/ExpertSystem.Common/Generated  ../src/ExpertSystem.Common/Proto/SocketExchange.proto ../src/ExpertSystem.Common/Proto/SocketOperations.proto --grpc_out ../src/ExpertSystem.Common/Generated --plugin=protoc-gen-grpc=%PLUGIN%

endlocal
