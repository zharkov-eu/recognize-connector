#!/usr/bin/env python3
# -*- coding: utf-8 -*

import csv
import json
import sys
import urllib.parse, urllib.request
from time import sleep

# Необходимость включения mpn

include_mpn = True
# Необходимость включение производителя
include_manufacturer = True
# Максимальное ограничение limit
max_limit = 100
# Максимальное число значений (start + limit)
max_amount = 1000
# Минимальное значение number_of_contacts
min_noc = 0
# Максимальное значение number_of_contacts
max_noc = 300


def load(amount):
    """
    Загружает информацию о требуемых разъёмах
    :param amount: Число разъёмов для загрузки
    :return: [{}, {}, ...]
    """
    responses = []
    query = 'connector'

    # Определяем шаг в число контактов
    noc_step = int(max_noc / (amount // max_amount))
    req_counter = 0
    for noc_counter in range(0, max_noc, noc_step):
        # TODO: Делать тестовый запрос и опряделять занчение hits
        # Выполняем 1000 запросов по 100
        for counter in range(0, max_amount, max_limit):
            req_counter = req_counter + 1
            url = "http://octopart.com/api/v3/parts/search"
            url += "?apikey=" + "68d2a6b0"
            args = [
                ('q', query),           # Текст запроса
                ('include[]', 'specs'), # Включать параметры
                ('start', counter),     # Начало списка
                ('limit', max_limit),   # Размер списка
                ('hide[]', 'offers'),   # Исключить данные о продаже
                ('hide[]', 'facet'),
                ('hide[]', 'filter'),
                ('hide[]', 'msec'),     # Исключить время выполнения запроса
                ('hide[]', 'stats'),
                # Включать данные с number_of_contacts
                ('filter[queries][]', 'specs.number_of_contacts.value:[{} TO {}]'.format(noc_counter, noc_counter + noc_step)),
                # Включать данные с size_length
                ('filter[queries][]', 'specs.size_length.value:[0 TO *]'),
                # Включать данные с size_width
                ('filter[queries][]', 'specs.size_width.value:[0 TO *]'),
            ]
            url += '&' + urllib.parse.urlencode(args)
            print("{}: {}".format(req_counter, url))
            data = urllib.request.urlopen(url).read()
            search_response = json.loads(data)
            responses.append(search_response)
            # TODO: Добавить подсчёт времени на запрос и задержку на оставшееся время между запросами
            sleep(0.05)
    return responses


def save(filename, data):
    """
    Создаёт list of dicts и сохраняет его в csv
    :param filename: Имя файла
    :param data: Данные [{}, {}, ...]
    :return:
    """

    # Cписок необходимых параметров
    req_keys = [
        "mpn",
        "gender",
        "contact_material",
        "contact_plating",
        "color",
        "housing_color",
        "housing_material",
        "mounting_style",
        "number_of_contacts",
        "number_of_positions",
        "number_of_rows",
        "orientation",
        "pin_pitch",
        "material",
        "size_diameter",
        "size_length",
        "size_height",
        "size_width"
    ]
    if not include_mpn:
        req_keys.remove("mpn")

    # Создаём список только с необходимыми ключами
    new_data = []
    for item in data:
        new_item = {}
        for key in item:
            if key in req_keys:
                new_item[key] = item[key]
        new_data.append(new_item)

    # Сохраняем в файл
    with open(filename, 'w') as output_file:
        dict_writer = csv.DictWriter(output_file, req_keys, dialect=csv.excel)
        dict_writer.writeheader()
        dict_writer.writerows(new_data)

    print("Данные сохранены в файл {}".format(filename))


def main(argv):
    """
    Главная функция
    :param argv: Аргументы командной строки
    :return:
    """
    # Загружаем данные по api
    amount = 20000
    responses = load(amount)
    # print("Доступно всего {} разъёмов".format(responses[0]['hits']))
    print("Загружены данные {} разъёмов".format(amount))

    # Преобразуем данные (вынимаем результат из полей)
    result = []
    for response in responses:
        for item in response['results']:
            item_specs = {}
            if include_mpn is True:
                item_specs['mpn'] = item['item']['mpn']
            if include_manufacturer is True:
                item_specs['manufacturer'] = item['item']['manufacturer']['name']
            for spec in item['item']['specs']:
                if type(item['item']['specs'][spec]['value']) is list and len(item['item']['specs'][spec]['value']) > 0:
                    item_specs[spec] = item['item']['specs'][spec]['value'][0]
            result.append(item_specs)
    print("Распознаны данные {} разъёмов".format(len(result)))

    # Сохраняем данные в файл
    filename = '1.csv'
    save(filename, result)


if __name__ == '__main__':
    main(sys.argv)
