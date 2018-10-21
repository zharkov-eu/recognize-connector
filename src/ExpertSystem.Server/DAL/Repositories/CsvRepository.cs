using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.Parsers;

namespace ExpertSystem.Server.DAL.Repositories
{
    /// <summary>Класс для работы репозиторием на CSV файле</summary>
    public class CsvRepository
    {
        // Название файла CSV
        private readonly string _csvFileName;

        // Название файла WAL-лога
        private readonly string _walFileName;

        // Поток ввода в WAL-лог файл
        private StreamWriter _walStream;

        // Разъёмы
        private Dictionary<int, CustomSocket> _sockets = new Dictionary<int, CustomSocket>();

        // Список доступных разъёмов по имени
        private Dictionary<string, int> _socketsByName = new Dictionary<string, int>();

        /// <summary>Конструктор репозитория</summary>
        /// <param name="csvFileName">Название файла CSV</param>
        /// <param name="walFileName">Название файла WAL-лога</param>
        public CsvRepository(string csvFileName, string walFileName)
        {
            if (!File.Exists(csvFileName))
                throw new FileNotFoundException($"Файл {csvFileName} не найден");
            _csvFileName = csvFileName;
            if (!File.Exists(walFileName))
                File.Create(walFileName);
            _walFileName = walFileName;
        }

        /// <summary>Синхронизация</summary>
        public CsvRepository Sync()
        {
            // Читаем доступные разъём CSV файла
            using (var reader = new StreamReader(File.OpenRead(_csvFileName)))
            {
                var sockets = SocketParser.ParseSockets(reader);
                foreach (var socket in sockets)
                {
                    var hashCode = socket.GetHashCode();
                    _sockets.Add(hashCode, socket);
                    _socketsByName.Add(socket.SocketName, hashCode);
                }
            }

            // Читаем требуемые изменения WAL файла и применяем их
            using (var reader = new StreamReader(File.OpenRead(_walFileName)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var entry = CsvEntity.ParseFromString(line);
                    switch (entry.Action)
                    {
                        case CsvDbAction.Insert:
                            _sockets.Add(entry.HashCode, entry.Socket);
                            _socketsByName.Add(entry.Socket.SocketName, entry.HashCode);
                            break;
                        case CsvDbAction.Update:
                            _sockets.Remove(entry.HashCode);
                            _sockets.Add(entry.Socket.GetHashCode(), entry.Socket);
                            break;
                        case CsvDbAction.Delete:
                            _sockets.Remove(entry.HashCode);
                            _socketsByName.Remove(entry.Socket.SocketName);
                            break;
                    }
                }
            }

            // Очищаем WAL файл
            FileUtils.ClearFile(_walFileName);

            // Очищаем CSV файл и заполняем его новыми значениями
            FileUtils.ClearFile(_csvFileName);
            using (var writer = new StreamWriter(File.OpenWrite(_csvFileName)))
            {
                writer.WriteLine(string.Join(StorageCustomSocket.Delimiter, SocketParser.CsvHead));
                foreach (var socket in _sockets.Values)
                    writer.WriteLine(StorageCustomSocket.Serialize(socket));
            }

            // Открываем WAL файл на запись
            _walStream = new StreamWriter(File.OpenWrite(_walFileName));
            return this;
        }

        /// <summary>Получить список разъёмов</summary>
        /// <returns>Список разъёмов</returns>
        public List<CustomSocket> GetSockets()
        {
            return _sockets.Values.ToList();
        }

        /// <summary>Выполнить действие выбора</summary>
        /// <param name="socketName">Имя разъёма</param>
        /// <returns>Кортеж из хэш кода и разъёма с переданым именем</returns>
        public Tuple<int, CustomSocket> Select(string socketName)
        {
            if (_socketsByName.TryGetValue(socketName, out var hashCode))
                return new Tuple<int, CustomSocket>(hashCode, _sockets[hashCode]);
            return null;
        }

        /// <summary>Выполнить действие вставки</summary>
        /// <param name="customSocket">Данные</param>
        /// <returns>Вставленный в репозиторий разъём</returns>
        public CustomSocket Insert(CustomSocket customSocket)
        {
            _sockets.Add(customSocket.GetHashCode(), customSocket);
            _walStream.WriteLine(new CsvEntity(CsvDbAction.Insert, customSocket.GetHashCode(), customSocket).ToString());
            _socketsByName.Add(customSocket.SocketName, customSocket.GetHashCode());
            return customSocket;
        }

        /// <summary>Выполнить действие изменния</summary>
        /// <param name="hashCode"></param>
        /// <param name="customSocket">Данные</param>
        /// <returns>Обновлённый разъём</returns>
        public CustomSocket Update(int hashCode, CustomSocket customSocket)
        {
            _sockets.Remove(hashCode);
            _sockets.Add(customSocket.GetHashCode(), customSocket);
            _walStream.WriteLine(new CsvEntity(CsvDbAction.Update, hashCode, customSocket).ToString());
            _socketsByName.Add(customSocket.SocketName, hashCode);
            return customSocket;
        }

        /// <summary>Выполнить действие удаления</summary>
        /// <param name="hashCode"></param>
        public void Delete(int hashCode)
        {
            var socket = _sockets[hashCode];
            _sockets.Remove(hashCode);
            _socketsByName.Remove(socket.SocketName);
            _walStream.WriteLine(new CsvEntity(CsvDbAction.Delete, hashCode).ToString());
        }
    }

    /// <summary>Утилиты для работы с файлами</summary>
    public static class FileUtils
    {
        /// <summary>Очистить файл</summary>
        /// <param name="path">Путь до файла</param>
        public static void ClearFile(string path)
        {
            var fileStream = File.Open(path, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
        }
    }
}