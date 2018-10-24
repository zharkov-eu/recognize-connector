using System;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

namespace ExpertSystem.Server.Services
{
    public struct SocketExchangeOptions
    {
        public string Version;
        public bool Debug;
    }

    /// <inheritdoc />
    /// <summary>Вызов удалённых процедур для разъёмов</summary>
    public class SocketExchangeImpl : SocketExchange.SocketExchangeBase
    {
        private readonly CsvRepository _repository;
        private readonly SocketExchangeOptions _options;

        public SocketExchangeImpl(CsvRepository repository, SocketExchangeOptions options)
        {
            _repository = repository;
            _options = options;
        }

        public override Task<HelloMessage> SayHello(HelloMessage request, ServerCallContext context)
        {
            DebugWrite($"RpcCall {{SayHello}}: '{{{request}}}' from {context.Peer}");
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
            foreach (var socket in _repository.GetSockets())
                await responseStream.WriteAsync(socket);
        }

        /// <summary>Вызов процедуры вставки, при конфликте обновления</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<CustomSocket> UpsertSocket(CustomSocket request, ServerCallContext context)
        {
            var socketName = request.SocketName;
            var tmp = _repository.Select(socketName);
            if (tmp != null)
                return Task.FromResult(_repository.Update(tmp.Item1, request));
            else
                return Task.FromResult(_repository.Insert(request));
        }

        /// <summary>Вызов процедуры удаления</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<Empty> DeleteSocket(CustomSocketIdentity request, ServerCallContext context)
        {
            var hashCode = _repository.Select(request.SocketName).Item1;
            _repository.Delete(hashCode);
            return null;
        }

        private void DebugWrite(string message)
        {
            if (_options.Debug)
                Console.WriteLine(message);
        }
    }
}