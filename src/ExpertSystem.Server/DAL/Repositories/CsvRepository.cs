using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.Parsers;

namespace ExpertSystem.Server.DAL.Repositories
{
    /// <summary>
    /// Класс для работы с БД в виде CSV файла
    /// </summary>
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

        /// <summary>
        /// Конструктор БД
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public CsvRepository Initialize()
        {
            using (var reader = new StreamReader(File.OpenRead(_csvFileName)))
            {
                var sockets = SocketParser.ParseSockets(reader);
                foreach (var socket in sockets)
                    _sockets.Add(socket.GetHashCode(), socket);
            }

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
                            break;
                        case CsvDbAction.Update:
                            _sockets.Remove(entry.HashCode);
                            _sockets.Add(entry.Socket.GetHashCode(), entry.Socket);
                            break;
                        case CsvDbAction.Delete:
                            _sockets.Remove(entry.HashCode);
                            break;
                    }
                }
            }

            ClearFile(_walFileName);
            ClearFile(_csvFileName);

            using (var writer = new StreamWriter(File.OpenWrite(_csvFileName)))
            {
                writer.WriteLine(string.Join(StorageCustomSocket.Delimiter, SocketParser.CsvHead));
                foreach (var socket in _sockets.Values)
                    writer.WriteLine(StorageCustomSocket.Serialize(socket));
            }

            _walStream = new StreamWriter(File.OpenWrite(_walFileName));
            return this;
        }

        /// <summary>
        /// Получить список разъёмов
        /// </summary>
        /// <returns>Список разъёмов</returns>
        public List<CustomSocket> GetSockets()
        {
            return _sockets.Values.ToList();
        }

        /// <summary>
        /// Выполнить действие вставки
        /// </summary>
        /// <param name="customSocket">Данные</param>
        public void Insert(CustomSocket customSocket)
        {
            _sockets.Add(customSocket.GetHashCode(), customSocket);
            _walStream.WriteLine(new CsvEntity(CsvDbAction.Insert, customSocket.GetHashCode(), customSocket).ToString());
        }

        /// <summary>
        /// Выполнить действие изменния
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="customSocket">Данные</param>
        public void Update(int hashCode, CustomSocket customSocket)
        {
            _sockets.Remove(hashCode);
            _sockets.Add(customSocket.GetHashCode(), customSocket);
            _walStream.WriteLine(new CsvEntity(CsvDbAction.Update, hashCode, customSocket).ToString());
        }

        /// <summary>
        /// Выполнить действие удаления
        /// </summary>
        /// <param name="hashCode"></param>
        public void Delete(int hashCode)
        {
            _sockets.Remove(hashCode);
            _walStream.WriteLine(new CsvEntity(CsvDbAction.Delete, hashCode).ToString());
        }

        /// <summary>
        /// Очистить файл
        /// </summary>
        /// <param name="path">Путь до файла</param>
        private static void ClearFile(string path)
        {
            var fileStream = File.Open(path, FileMode.Open);
            fileStream.SetLength(0);
            fileStream.Close();
        }
    }
}