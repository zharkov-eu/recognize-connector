using System.Collections.Generic;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

namespace ExpertSystem.Server.DAL.Services
{
    /// <summary>Класс для доступа к сущностям категорий</summary>
    public class GroupService
    {
        // Репозиторий категорий
        private readonly CsvRepository<SocketGroup> _groupRepository;

        public GroupService(CsvRepository<SocketGroup> groupRepository)
        {
            _groupRepository = groupRepository;
        }

        /// <summary>Создать новую группу разъёмов</summary>
        /// <param name="socketGroup">Группа разъёмов</param>
        /// <returns>Созданная группа разъёмов</returns>
        public SocketGroup CreateSocketGroup(SocketGroup socketGroup)
        {
            return _groupRepository.Insert(socketGroup);
        }

        /// <summary>Удалить группу разъёмов</summary>
        /// <param name="groupName">Имя группы разъёмов</param>
        public void DeleteSocketGroup(string groupName)
        {
            _groupRepository.Delete(groupName);
        }

        /// <summary>Получить все группы разъёмов</summary>
        /// <returns>Список всех доступных групп разъёмов</returns>
        public List<SocketGroup> GetSocketGroups()
        {
            return _groupRepository.GetAllRecords();
        }

        /// <summary>Получить существующую группу разъёмов</summary>
        /// <param name="groupName">Имя группы разъёмов</param>
        /// <returns>Обновлённая группа разъёмов</returns>
        public SocketGroup GetSocketGroup(string groupName)
        {
            return _groupRepository.Select(groupName)?.Item2;
        }

        /// <summary>Добавить разъём в группу</summary>
        /// <param name="groupName">Имя группы разъёмов</param>
        /// <param name="socketName">Имя разъёма</param>
        /// <returns>Обновлённая группа</returns>
        public SocketGroup AddSocketToGroup(string groupName, string socketName)
        {
            // Получить группу разъёмов
            var group = GetSocketGroup(groupName);
            // Проверить существование группы разъёмов
            if (group != null)
            {
                // Удалить группу разъёмов
                _groupRepository.Delete(groupName);
                // Добавить разъём
                group.SocketNames.Add(socketName);
                // Вставить новую группу разъёмов
                return _groupRepository.Insert(group);
            }
            else return null;
        }

        /// <summary>Удалить разъём из группы</summary>
        /// <param name="groupName">Имя группы</param>
        /// <param name="socketName">Имя разъёма</param>
        /// <returns>Обновлённая группа</returns>
        public SocketGroup RemoveSocketFromGroup(string groupName, string socketName)
        {
            // Получить группу разъёмов
            var group = _groupRepository.Select(groupName).Item2;
            // Проверить существование группы разъёмов
            if (group != null)
            {
                // Удалить группу разъёмов
                _groupRepository.Delete(groupName);
                // Удалить разъём
                group.SocketNames.Remove(socketName);
                // Вставить новую группу разъёмов
                return _groupRepository.Insert(group);
            }
            else return null;
        }
    }
}