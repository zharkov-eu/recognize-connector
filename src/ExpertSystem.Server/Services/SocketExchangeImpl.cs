using System;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Controllers;

namespace ExpertSystem.Server.Services
{
    public struct SocketExchangeOptions
    {
        public string Version;
        public bool Debug;
    }

    /// <summary>Вызов удалённых процедур для разъёмов</summary>
    public class SocketExchangeImpl : SocketExchange.SocketExchangeBase
    {
        private readonly SocketExchangeOptions _options;

        // Контроллеры
        private readonly CustomSocketController _socketController;
        private readonly SocketGroupController _socketGroupController;

        public SocketExchangeImpl(CustomSocketController socketController, SocketGroupController socketGroupController,
            SocketExchangeOptions options)
        {
            _socketController = socketController;
            _socketGroupController = socketGroupController;
            _options = options;
        }

        public override Task<HelloMessage> SayHello(HelloMessage request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'SayHello': '{request}' from {context.Peer}");

            return Task.FromResult(new HelloMessage {Version = _options.Version});
        }

        /// <summary>Вызов процедуры получения списка разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="responseStream">Поток ответа</param>
        /// <param name="context">Контескт</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override async Task GetSockets(Empty request, IServerStreamWriter<CustomSocket> responseStream,
            ServerCallContext context)
        {
            foreach (var socket in _socketController.GetSockets())
                await responseStream.WriteAsync(socket);
        }

        /// <summary>Вызов процедуры вставки, при конфликте обновления</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<CustomSocket> UpsertSocket(CustomSocket request, ServerCallContext context)
        {
            var existingSocket = _socketController.GetSocket(request.SocketName);
            if (existingSocket != null)
                return Task.FromResult(_socketController.UpdateSocket(existingSocket.Item1, request));
            return Task.FromResult(_socketController.InsertSocket(request));
        }

        /// <summary>Вызов процедуры удаления</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<Empty> DeleteSocket(CustomSocketIdentity request, ServerCallContext context)
        {
            var hashCode = _socketController.GetSocket(request.SocketName).Item1;
            _socketController.DeleteSocket(hashCode);
            return null;
        }

        private void DebugWrite(string message)
        {
            if (_options.Debug)
                Console.WriteLine(message);
        }
    }
}