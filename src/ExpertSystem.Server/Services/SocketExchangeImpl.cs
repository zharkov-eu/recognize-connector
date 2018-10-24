using System;
using System.Threading.Tasks;
using Grpc.Core;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Controllers;

namespace ExpertSystem.Server.Services
{
    /// <inheritdoc />
    /// <summary>Вызов удалённых процедур для разъёмов</summary>
    public class SocketExchangeImpl : SocketExchange.SocketExchangeBase
    {
        // Текущая версия сервера
        private const string Version = "1.0.0";

        // Контроллеры
        private readonly SocketController _socketController;
        private readonly CategoryController _categoryController;

        public SocketExchangeImpl(SocketController socketController, CategoryController categoryController)
        {
            _socketController = socketController;
            _categoryController = categoryController;
        }

        public override Task<HelloMessage> SayHello(HelloMessage request, ServerCallContext context)
        {
            return Task.FromResult(new HelloMessage {Version = Version});
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
        public override Task<Empty> DeleteSocket(CustomSocket request, ServerCallContext context)
        {
            var hashCode = _socketController.GetSocket(request.SocketName).Item1;
            _socketController.DeleteSocket(hashCode);
            return null;
        }

        public override Task GetGroups(Empty request, IServerStreamWriter<Category> responseStream, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<Category> AddGroup(Category request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<Empty> DeleteGroup(Category request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<Category> AddSocketToGroup(Category request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<Category> RemoveSocketFromGroup(AddSocket request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}