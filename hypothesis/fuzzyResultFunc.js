/**
 * @author Эксперт: Матвей Полетин
 * @author Математик: Руслан Кадыров
 * @author Кодировщик: Евгений Жарков
 */

/**
 * Получить результирующую функцию ()
 * @param {number} noc - кластер NumberOfContacts
 * @param {number} l - кластер SizeLength
 * @param {number} w - кластер SizeWidth
 */
const getValueFunction = (noc, l, w) => (socket) => {
    return socket.NumberOfContacts * Math.log2(noc) + (
        (socket.SizeLength * 100 * Math.exp(l)) * (socket.SizeWidth * 100 * Math.pow(2, w))
    );
}

const getSocketValues = (socket) => {
    console.log("Макс. сила тока при разымкании цепи")

    const results = [];
    for (let i = 1; i <= 5; i++)
        for (let k = 1; k <= 4; k++)
            for (let j = 1; j <= 3; j++) {
                const getValue = getValueFunction(i,k,j)
                const value = getValue(socket);
                results.push(value);
                console.log(`${i}:${k}:${j} = ${value} мА`);
            }

    console.log("Сила тока при размыкании цепи (минимальная): = " + Math.min(...results) + " мА");
    console.log("Сила тока при размыкании цепи (максимальная): = " + Math.max(...results) + " мА");
    console.log("Сила тока при размыкании цепи (средняя): = " + results.reduce((acc, v) => acc += v) / results.length + " мА");
}

const socket = { NumberOfContacts: 50, SizeLength: 0.03, SizeWidth: 0.0075 }

getSocketValues(socket);
