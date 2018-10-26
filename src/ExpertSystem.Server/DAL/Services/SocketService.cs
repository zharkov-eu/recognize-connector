using System;
using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

namespace ExpertSystem.Server.DAL.Services
{
    /// <summary>Класс для доступа к сущностям разъёмов</summary>
    public class SocketService
    {
        private readonly CsvRepository<CustomSocket> _socketRepository;

        public SocketService(CsvRepository<CustomSocket> socketRepository)
        {
            _socketRepository = socketRepository;
        }

        /// <summary>Получить список разъёмов</summary>
        /// <returns>Список доступных разъёмов</returns>
        public List<CustomSocket> GetSockets()
        {
            return _socketRepository.GetAllRecords();
        }

        /// <summary>Получить разъём</summary>
        /// <param name="socketName">Имя разъёма</param>
        /// <returns>Хэш код и разъём</returns>
        public Tuple<int, CustomSocket> GetSocket(string socketName)
        {
            return _socketRepository.Select(socketName);
        }

        /// <summary>Вставить новый разъём</summary>
        /// <param name="socket">Разъём для вставки</param>
        /// <returns>Вставленный разъём</returns>
        public CustomSocket InsertSocket(CustomSocket socket)
        {
            return _socketRepository.Insert(socket);
        }

        /// <summary>Обновить существующий разъём</summary>
        /// <param name="hashCode">Хэш код разъёма</param>
        /// <param name="socket">Обновлённый разъём</param>
        /// <returns>Обновлённый разъём</returns>
        public CustomSocket UpdateSocket(int hashCode, CustomSocket socket)
        {
            return _socketRepository.Update(hashCode, socket);
        }

        /// <summary>Удалить существующий разъём</summary>
        /// <param name="socketName">Имя разъма</param>
        public void DeleteSocket(string socketName)
        {
            _socketRepository.Delete(socketName);
        }
    }
}