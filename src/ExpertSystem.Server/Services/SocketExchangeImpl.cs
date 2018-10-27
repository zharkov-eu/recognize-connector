using System;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Services;

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
        private readonly SocketExchangeOptions _options;

        // Контроллеры
        private readonly SocketService _socketService;
        private readonly GroupService _groupService;

        public SocketExchangeImpl(SocketService socketService, GroupService groupService,
            SocketExchangeOptions options)
        {
            _socketService = socketService;
            _groupService = groupService;
            _options = options;
        }

        public override Task<HelloMessage> SayHello(HelloMessage request, ServerCallContext context)
        {
            DebugWrite($"RpcCall 'SayHello': '{request}' from {context.Peer}");

            return Task.FromResult(new HelloMessage {Version = _options.Version});
        }

        // ===========================================================================================================
        // Процедуры над разъёмами
        // ===========================================================================================================

        /// <summary>Вызов процедуры получения списка разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="responseStream">Поток ответа</param>
        /// <param name="context">Контескт</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override async Task GetSockets(Empty request, IServerStreamWriter<CustomSocket> responseStream,
            ServerCallContext context)
        {
            foreach (var socket in _socketService.GetSockets())
                await responseStream.WriteAsync(socket);
        }

        /// <summary>Вызов процедуры вставки, при конфликте обновления</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<CustomSocket> UpsertSocket(CustomSocket request, ServerCallContext context)
        {
            var existingSocket = _socketService.GetSocket(request.SocketName);
            if (existingSocket != null)
                return Task.FromResult(_socketService.UpdateSocket(existingSocket.Item1, request));
            return Task.FromResult(_socketService.InsertSocket(request));
        }

        /// <summary>Вызов процедуры удаления</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<Empty> DeleteSocket(CustomSocketIdentity request, ServerCallContext context)
        {
            _socketService.DeleteSocket(request.SocketName);
            return Task.FromResult<Empty>(null);
        }

        // ===========================================================================================================
        // Процедуры над группами
        // ===========================================================================================================

        /// <summary>Вызов процедуры получения всех групп разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="responseStream">Поток ответа</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override async Task GetSocketGroups(Empty request, IServerStreamWriter<SocketGroup> responseStream,
            ServerCallContext context)
        {
            foreach (var socketGroup in _groupService.GetSocketGroups())
                await responseStream.WriteAsync(socketGroup);
        }

        /// <summary>Вызов процедуры создания группы разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<SocketGroup> AddSocketGroup(SocketGroupIdentity request, ServerCallContext context)
        {
            return Task.FromResult(_groupService.CreateSocketGroup(
                new SocketGroup {GroupName = request.GroupName}));
        }

        /// <summary>Вызов процедуры удаления группы разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<Empty> DeleteSocketGroup(SocketGroupIdentity request, ServerCallContext context)
        {
            _groupService.DeleteSocketGroup(request.GroupName);
            return Task.FromResult<Empty>(null);
        }

        /// <summary>Добавления разъёма в группу разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<SocketGroup> AddToSocketGroup(CustomSocketIdentityJoinGroup request,
            ServerCallContext context)
        {
            var groupName = request.Group.GroupName;
            var socketName = request.Socket.SocketName;
            // Проверка существования такого разъёма и такой группы
            var socket = _socketService.GetSocket(socketName);
            var socketGroup = _groupService.GetSocketGroup(groupName);
            if (socket != null && socketGroup != null)
            {
                return Task.FromResult(_groupService.AddSocketToGroup(groupName, socketName));
            }

            return null;
        }

        /// <summary>Удаление разъёма из группы разъёмов</summary>
        /// <param name="request">Запрос</param>
        /// <param name="context">Контекст</param>
        /// <returns>Завершённая задача после того, как были написаны заголовки ответов</returns>
        public override Task<SocketGroup> RemoveSocketFromGroup(CustomSocketIdentityJoinGroup request,
            ServerCallContext context)
        {
            var groupName = request.Group.GroupName;
            var socketName = request.Socket.SocketName;
            // Проверка существования такого разъёма и такой группы
            var socket = _socketService.GetSocket(socketName);
            var socketGroup = _groupService.GetSocketGroup(groupName);
            if (socket != null && socketGroup != null && socket.Item2 != null)
            {
                _groupService.RemoveSocketFromGroup(groupName, socket.Item2.SocketName);
                return null;
            }
            else return null;
        }

        private void DebugWrite(string message)
        {
            if (_options.Debug)
                Console.WriteLine(message);
        }
    }
}