using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.Parsers;

namespace ExpertSystem.Server.DAL.Repositories
{
    /// <inheritdoc />
    /// <summary>Класс для работы репозиторием на CSV файле</summary>
    public class CsvRepository : IDisposable
    {
        // Название файла CSV
        private readonly string _csvFileName;

        // Название файла WAL-лога
        private readonly string _walFileName;

        // Поток ввода в WAL-лог файл
        private readonly FileStream _walStream;

        // Разъёмы
        private Dictionary<int, CustomSocket> _sockets = new Dictionary<int, CustomSocket>();

        // Список доступных разъёмов по имени
        private Dictionary<string, int> _socketsByName = new Dictionary<string, int>();

        /// <summary>Конструктор репозитория</summary>
        /// <param name="csvFileName">Название файла CSV</param>
        /// <param name="walFileName">Название файла WAL-лога</param>
        public CsvRepository(string csvFileName, string walFileName)
        {
            // Проверка существования CSV файла
            if (!File.Exists(csvFileName))
                throw new FileNotFoundException($"Файл {csvFileName} не найден");

            _csvFileName = csvFileName;
            _walFileName = walFileName;

            _walStream = !File.Exists(walFileName)
                ? File.Create(walFileName)
                : File.Open(_walFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        /// <summary>Синхронизация</summary>
        public CsvRepository Sync()
        {
            // Читаем доступные разъёмы CSV файла
            using (var reader = new StreamReader(File.OpenRead(_csvFileName)))
            {
                foreach (var socket in SocketParser.ParseSockets(reader))
                {
                    var hashCode = socket.GetHashCode();
                    if (!_sockets.ContainsKey(hashCode))
                    {
                        _sockets.Add(hashCode, socket);
                        _socketsByName.Add(socket.SocketName, hashCode);
                    }
                }
            }

            // Читаем требуемые изменения WAL файла и применяем их
            using (var reader = new StreamReader(_walStream))
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
        /// <param name="socket">Данные</param>
        /// <returns>Вставленный в репозиторий разъём</returns>
        public CustomSocket Insert(CustomSocket socket)
        {
            _socketsByName.Add(socket.SocketName, socket.GetHashCode());
            _sockets.Add(socket.GetHashCode(), socket);
            _walStream.Write(
                Encoding.UTF8.GetBytes(new CsvEntity(CsvDbAction.Insert, socket.GetHashCode(), socket).ToString()));
            return socket;
        }

        /// <summary>Выполнить действие изменния</summary>
        /// <param name="hashCode">Хэш код разъёма</param>
        /// <param name="socket">Данные</param>
        /// <returns>Обновлённый разъём</returns>
        public CustomSocket Update(int hashCode, CustomSocket socket)
        {
            _socketsByName.Add(socket.SocketName, hashCode);
            _sockets.Remove(hashCode);
            _sockets.Add(socket.GetHashCode(), socket);
            _walStream.Write(Encoding.UTF8.GetBytes(new CsvEntity(CsvDbAction.Update, hashCode, socket).ToString()));
            return socket;
        }

        /// <summary>Выполнить действие удаления</summary>
        /// <param name="hashCode">Хэш код разъёма</param>
        public void Delete(int hashCode)
        {
            if (_sockets.ContainsKey(hashCode))
            {
                var socket = _sockets[hashCode];
                _socketsByName.Remove(socket.SocketName);
                _sockets.Remove(hashCode);
                _walStream.Write(Encoding.UTF8.GetBytes(new CsvEntity(CsvDbAction.Delete, hashCode).ToString()));
            }
        }

        public void Dispose()
        {
            _walStream.Close();
        }
    }

    /// <summary>Утилиты для работы с файлами</summary>
    public static partial class FileUtils
    {
        /// <summary>Очистить файл</summary>
        /// <param name="fs"></param>
        public static void ClearFile(FileStream fs)
        {
            fs.SetLength(0);
        }

        /// <summary>Очистить файл</summary>
        /// <param name="path">Путь до файла</param>
        public static void ClearFile(string path)
        {
            if (!File.Exists(path)) return;
            using (var fs = File.Open(path, FileMode.Open))
            {
                fs.SetLength(0);
            }
        }
    }
}