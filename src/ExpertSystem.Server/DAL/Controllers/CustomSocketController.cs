using System;
using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

namespace ExpertSystem.Server.DAL.Controllers
{
    /// <summary>Класс для доступа к сущностям разъёмов</summary>
    public class CustomSocketController
    {
        private CsvRepository<CustomSocket> _socketRepository;

        public CustomSocketController(CsvRepository<CustomSocket> socketRepository)
        {
            _socketRepository = socketRepository;
        }

        /// <summary>Получить список разъёмов</summary>
        /// <returns>Список доступных разъёмов</returns>
        public List<CustomSocket> GetSockets()
        {
            throw new NotImplementedException();
        }

        /// <summary>Получить разъём</summary>
        /// <param name="socketName">Имя разъёма</param>
        /// <returns>Хэш код и разъём</returns>
        public Tuple<int, CustomSocket> GetSocket(string socketName)
        {
            throw new NotImplementedException();
        }

        /// <summary>Вставить новый разъём</summary>
        /// <param name="socket">Разъём для вставки</param>
        /// <returns>Вставленный разъём</returns>
        public CustomSocket InsertSocket(CustomSocket socket)
        {
            throw new NotImplementedException();
        }

        /// <summary>Обновить существующий разъём</summary>
        /// <param name="hashCode">Хэш код разъёма</param>
        /// <param name="socket">Обновлённый разъём</param>
        /// <returns></returns>
        public CustomSocket UpdateSocket(int hashCode, CustomSocket socket)
        {
            throw new NotImplementedException();
        }

        /// <summary>Удалить существующий разъём</summary>
        /// <param name="hashCode">Хэш код разъма</param>
        public void DeleteSocket(int hashCode)
        {
            throw new NotImplementedException();
        }
    }
}