#!/usr/bin/env python3
# -*- coding: utf-8 -*

import sys
import csv
import json
import urllib.parse, urllib.request

include_mpn = True
include_manufacturer = True
limit = 100


def load(amount):
    """
    Загружает информацию о требуемых разъёмах
    :param amount: Число разъёмов для загрузки
    :return: [{}, {}, ...]
    """
    if amount > 1000:
        print("Максимальное число значение не может превысить 1000")
        amount = 1000

    responses = []
    query = 'connector'
    for counter in range(0, amount, limit):
        url = "http://octopart.com/api/v3/parts/search"
        url += "?apikey=" + "68d2a6b0"
        args = [
            ('q', query),           # Текст запроса
            ('include[]', 'specs'), # Включать параметры
            ('start', counter),     # Начало списка
            ('limit', limit),       # Размер списка
            ('hide[]', 'offers'),   # Исключить данные о продаже
            ('hide[]', 'facet'),
            ('hide[]', 'filter'),
            ('hide[]', 'msec'),
            ('hide[]', 'stats'),
            # Включать данные с number_of_contacts
            ('filter[queries][]', 'specs.number_of_contacts.value:[0 TO *]'),
            # Включать данные с size_length
            ('filter[queries][]', 'specs.size_length.value:[0 TO *]'),
            # Включать данные с size_width
            ('filter[queries][]', 'specs.size_width.value:[0 TO *]'),
        ]
        url += '&' + urllib.parse.urlencode(args)
        data = urllib.request.urlopen(url).read()
        search_response = json.loads(data)
        responses.append(search_response)
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
    amount = 1000
    responses = load(amount)
    print("Доступно всего {} разъёмов".format(responses[0]['hits']))
    print("Загружены данные о {} разъёмах".format(amount))

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

    # Сохраняем данные в файл
    filename = 'result.csv'
    save(filename, result)


if __name__ == '__main__':
    main(sys.argv)
