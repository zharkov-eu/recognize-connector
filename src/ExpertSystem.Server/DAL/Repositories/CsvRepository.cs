using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExpertSystem.Common.Generated;
using ExpertSystem.Common.Models;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Server.DAL.Entities;

namespace ExpertSystem.Server.DAL.Repositories
{
    /// <summary>Класс для работы репозиторием на CSV файле</summary>
    /// <typeparam name="TR">Тип записей</typeparam>
    /// <typeparam name="TP">Тип парсера</typeparam>
    /// <typeparam name="TE">Тип расширения</typeparam>
    public class CsvRepository<TR, TP, TE> : IDisposable
        where TP : CustomParser<TR, RecordExtension<TR>>
        where TE : RecordExtension<TR>
    {
        // Название файла CSV
        private readonly string _csvFileName;

        // Название файла WAL-лога
        private readonly string _walFileName;

        // Поток ввода в WAL-лог файл
        private readonly FileStream _walStream;

        // Разъёмы
        private Dictionary<int, TR> _records = new Dictionary<int, TR>();

        // Список доступных разъёмов по имени
        private Dictionary<string, int> _recordsByName = new Dictionary<string, int>();

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

        /// <summary>Синхронизация репозитория</summary>
        public CsvRepository<TR, TP, TE> Sync()
        {
            // Очистака предыдущих значений разъёмов
            _records.Clear();
            _recordsByName.Clear();

            // Читаем доступные разъёмы CSV файла
            using (var reader = new StreamReader(File.OpenRead(_csvFileName)))
            {
                foreach (var record in TP.ParseRecords(reader))
                {
                    var hashCode = record.GetHashCode();
                    if (_records.ContainsKey(hashCode)) continue;
                    _records.Add(hashCode, record);
                    _recordsByName.Add(record.SocketName, hashCode);
                }
            }

            // Читаем требуемые изменения WAL файла и применяем их
            using (var reader = new StreamReader(_walStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var entry = WalEntry<TR, TE>.ParseFromString(line);
                    switch (entry.Action)
                    {
                        case CsvDbAction.Insert:
                            _records.Add(entry.HashCode, entry.Socket);
                            _recordsByName.Add(entry.Socket.SocketName, entry.HashCode);
                            break;
                        case CsvDbAction.Update:
                            _records.Remove(entry.HashCode);
                            _records.Add(entry.Socket.GetHashCode(), entry.Socket);
                            break;
                        case CsvDbAction.Delete:
                            _records.Remove(entry.HashCode);
                            _recordsByName.Remove(entry.Socket.SocketName);
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
                writer.WriteLine(string.Join(CustomSocketExtension.Delimiter, TP.CsvHead));
                foreach (var record in _records.Values)
                    writer.WriteLine(TE.Serialize(record));
            }

            return this;
        }

        /// <summary>Получить список разъёмов</summary>
        /// <returns>Список разъёмов</returns>
        public List<TR> GetSockets()
        {
            return _records.Values.ToList();
        }

        /// <summary>Выполнить действие выбора</summary>
        /// <param name="recordName">Имя разъёма</param>
        /// <returns>Кортеж из хэш кода и разъёма с переданым именем</returns>
        public Tuple<int, TR> Select(string recordName)
        {
            if (_recordsByName.TryGetValue(recordName, out var hashCode))
                return new Tuple<int, TR>(hashCode, _records[hashCode]);
            return null;
        }

        /// <summary>Выполнить действие вставки</summary>
        /// <param name="record">Данные</param>
        /// <returns>Вставленный в репозиторий разъём</returns>
        public TR Insert(TR record)
        {
            _recordsByName.Add(record.SocketName, record.GetHashCode());
            _records.Add(record.GetHashCode(), record);
            _walStream.Write(
                Encoding.UTF8.GetBytes(new WalEntry<TR, TE>(CsvDbAction.Insert, record.GetHashCode(), record).ToString()));
            return record;
        }

        /// <summary>Выполнить действие изменния</summary>
        /// <param name="hashCode">Хэш код разъёма</param>
        /// <param name="record">Данные</param>
        /// <returns>Обновлённый разъём</returns>
        public TR Update(int hashCode, TR record)
        {
            _recordsByName.Remove(_records[hashCode].SocketName);
            _recordsByName.Add(record.SocketName, hashCode);
            _records.Add(record.GetHashCode(), record);
            _walStream.Write(Encoding.UTF8.GetBytes(new WalEntry<TR, TE>(CsvDbAction.Update, hashCode, record).ToString()));
            return record;
        }

        /// <summary>Выполнить действие удаления</summary>
        /// <param name="hashCode">Хэш код разъёма</param>
        public void Delete(int hashCode)
        {
            if (_records.ContainsKey(hashCode))
            {
                var record = _records[hashCode];
                _recordsByName.Remove(record.SocketName);
                _records.Remove(hashCode);
                _walStream.Write(Encoding.UTF8.GetBytes(new WalEntry<TR, TE>(CsvDbAction.Delete, hashCode).ToString()));
            }
        }

        public void Dispose()
        {
            _walStream.Close();
        }
    }

    /// <summary>Утилиты для работы с файлами</summary>
    public static class FileUtils
    {
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