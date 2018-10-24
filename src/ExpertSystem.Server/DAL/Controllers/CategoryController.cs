using System;
using ExpertSystem.Common.Generated;
using ExpertSystem.Server.DAL.Repositories;

namespace ExpertSystem.Server.DAL.Controllers
{
	/// <summary>Класс для доступа к сущностям категорий</summary>
	public class CategoryController
	{
		// Репозиторий
		private CsvRepository<SocketGroup, CategoryParser> _categoryRepository;

		public CategoryController(CsvRepository<SocketGroup, CategoryParser> categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		/// <summary>Получить существующую категорию</summary>
		/// <param name="categoryName">Имя категории</param>
		/// <returns>Категория</returns>
		public SocketGroup GetCategory(string categoryName)
		{
			throw new NotImplementedException();
		}

		/// <summary>Добавить разъём в группу</summary>
		/// <param name="hashCode">Хэш код разъёма</param>
		/// <returns>Обновлённая группа</returns>
		public SocketGroup AddSocketToGroup(int hashCode)
		{
			throw new NotImplementedException();
		}

		/// <summary>Удалить разъём из группы</summary>
		/// <param name="hashCode">Хэш код разъёма</param>
		/// <returns>Обновлённая группа</returns>
		public SocketGroup RemoveSocketFromGroup(int hashCode)
		{
			throw new NotImplementedException();
		}
	}
}