using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExpertSystem.Common.Parsers;
using ExpertSystem.Common.Serializers;
using ExpertSystem.Server.DAL.Entities;
using ExpertSystem.Server.DAL.Serializers;

namespace ExpertSystem.Server.DAL.Repositories
{
    /// <summary>Структура опций CSV репозитория</summary>
    public struct CsvRepositoryOptions
    {
        // Имя свойства-идентификатора
        public string IdPropertyName;

        // Название файла CSV
        public string CsvFileName;

        // Название файла WAL-лога
        public string WalFileName;
    }

    /// <summary>Класс для работы репозиторием на CSV файле</summary>
    /// <typeparam name="T">Тип записей</typeparam>
    public class CsvRepository<T>
    {
        // Опции работы CSV репозитория
        private readonly CsvRepositoryOptions _options;

        // Парсер
        private readonly CsvRecordParser<T> _parser;

        // Сериализатор записи
        private readonly ICsvRecordSerializer<T> _recordSerializer;

        // Сериализатор WAL-лога 
        private readonly WalEntrySerializer<T> _walSerializer;

        // Тип записи
        private readonly Type _recordType = typeof(T);

        // Записи
        private readonly Dictionary<int, T> _records = new Dictionary<int, T>();

        // Список доступных записей по идентификатору
        private readonly Dictionary<string, int> _recordsByName = new Dictionary<string, int>();

        /// <summary>Конструктор репозитория</summary>
        /// <param name="options">Опции репозитория</param>
        /// <param name="serializer">Расширение данного типа</param>
        /// <param name="parser">Парсер данного типа</param>
        public CsvRepository(ICsvRecordSerializer<T> serializer, CsvRecordParser<T> parser,
            CsvRepositoryOptions options)
        {
            _recordSerializer = serializer;
            _walSerializer = new WalEntrySerializer<T>(serializer);
            _parser = parser;
            _options = options;

            // Проверка существования CSV файла
            if (!File.Exists(_options.CsvFileName))
                throw new FileNotFoundException($"Файл {_options.CsvFileName} не найден");
        }

        /// <summary>Получить поток файла WAL-лога</summary>
        /// <returns>Поток файла WAL-лога</returns>
        protected FileStream GetWalStream()
        {
            return File.Open(_options.WalFileName, FileMode.Open);
        }
        
        /// <summary>Получить StreamWriter для файла WAL-лога</summary>
        /// <returns>StreamWriter для файла WAL-лога</returns>
        protected StreamWriter GetWalWriter()
        {
            return new StreamWriter(_options.WalFileName, true, Encoding.UTF8);
        }

        /// <summary>Получить StreamReader для файла WAL-лога</summary>
        /// <returns>StreamReader для файла WAL-лога</returns>
        protected StreamReader GetWalReader()
        {
            return new StreamReader(_options.WalFileName, Encoding.UTF8);
        }
        
        /// <summary>Синхронизация репозитория</summary>
        public CsvRepository<T> Sync()
        {
            // Очистка предыдущих значений записей
            _records.Clear();
            _recordsByName.Clear();

            // Читаем доступные записи из CSV файла
            using (var csvReader = new StreamReader(File.OpenRead(_options.CsvFileName)))
            {
                foreach (var record in _parser.ParseRecords(csvReader))
                {
                    var hashCode = record.GetHashCode();
                    if (_records.ContainsKey(hashCode)) continue;
                    _records.Add(hashCode, record);
                    _recordsByName.Add(GetRecordId(record), hashCode);
                }
            }

            // Читаем требуемые изменения WAL файла и применяем их
            using (var walReader = GetWalReader())
            {
                string line;
                while ((line = walReader.ReadLine()) != null)
                {
                    var entry = _walSerializer.Deserialize(line);
                    switch (entry.Action)
                    {
                        case CsvDbAction.Insert:
                            _records.Add(entry.HashCode, entry.Record);
                            _recordsByName.Add(GetRecordId(entry.Record), entry.HashCode);
                            break;
                        case CsvDbAction.Update:
                            _records.Remove(entry.HashCode);
                            _records.Add(entry.Record.GetHashCode(), entry.Record);
                            break;
                        case CsvDbAction.Delete:
                            _records.Remove(entry.HashCode);
                            _recordsByName.Remove(GetRecordId(entry.Record));
                            break;
                    }
                }
            }

            // Очищаем WAL файл
            using (var stream = GetWalStream())
                stream.SetLength(0);

            // Очищаем CSV файл и заполняем его новыми значениями
            FileUtils.ClearFile(_options.CsvFileName);
            using (var writer = new StreamWriter(File.OpenWrite(_options.CsvFileName)))
            {
                writer.WriteLine(string.Join(_recordSerializer.Delimiter, _parser.CsvHead));
                foreach (var record in _records.Values)
                    writer.WriteLine(_recordSerializer.Serialize(record));
            }

            return this;
        }

        /// <summary>Получить список записей</summary>
        /// <returns>Список записей</returns>
        public List<T> GetAllRecords()
        {
            return _records.Values.ToList();
        }

        /// <summary>Выполнить действие выбора</summary>
        /// <param name="recordName">Имя записи</param>
        /// <returns>Кортеж из хэш кода и записи с переданным именем</returns>
        public Tuple<int, T> Select(string recordName)
        {
            return _recordsByName.TryGetValue(recordName, out var hashCode)
                ? new Tuple<int, T>(hashCode, _records[hashCode]) 
                : null;
        }

        /// <summary>Выполнить действие вставки</summary>
        /// <param name="record">Данные</param>
        /// <returns>Вставленная в репозиторий запись</returns>
        public T Insert(T record)
        {
            _recordsByName.Add(GetRecordId(record), record.GetHashCode());
            _records.Add(record.GetHashCode(), record);
            using (var writer = GetWalWriter())
                writer.WriteLine(_walSerializer.Serialize(new WalEntry<T>(CsvDbAction.Insert, record.GetHashCode(), record)));
            return record;
        }

        /// <summary>Выполнить действие изменения</summary>
        /// <param name="hashCode">Хэш код записи</param>
        /// <param name="record">Данные</param>
        /// <returns>Обновлённая запись</returns>
        public T Update(int hashCode, T record)
        {
            _recordsByName.Remove(GetRecordId(_records[hashCode]));
            _recordsByName.Add(GetRecordId(record), hashCode);
            _records.Add(record.GetHashCode(), record);
            using (var writer = GetWalWriter())
                writer.WriteLine(_walSerializer.Serialize(new WalEntry<T>(CsvDbAction.Update, hashCode, record)));
            return record;
        }

        /// <summary>Выполнить действие удаления</summary>
        /// <param name="recordName">Имя записи</param>
        public void Delete(string recordName)
        {
            if (_recordsByName.TryGetValue(recordName, out var hashCode))
            {
                var record = _records[hashCode];
                _recordsByName.Remove(GetRecordId(record));
                _records.Remove(hashCode);
                using (var writer = GetWalWriter())
                    writer.WriteLine(_walSerializer.Serialize(new WalEntry<T>(CsvDbAction.Delete, hashCode, record)));
            }
        }

        /// <summary>Получить идентификатор записи</summary>
        /// <param name="record"></param>
        /// <returns>Строка идентификатора записи</returns>
        private string GetRecordId(T record)
        {
            return _recordType.GetProperty(_options.IdPropertyName).GetValue(record).ToString();
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
            using (var fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
                fs.SetLength(0);
        }
    }
}