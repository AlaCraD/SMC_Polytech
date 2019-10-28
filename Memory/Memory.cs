using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.MemoryMappedFiles;

public class MyMemory
{
    string Name;
	public MyMemory(string name)
	{
        Name = name;
	}
    public void Write(string mess)
    {
        //Console.WriteLine("Введите сообщение");
        //Ввод выражения для записи в общую память
        char[] message = mess.ToCharArray();
        //Размер введенного сообщения
        int size = message.Length;

        //Создание участка разделяемой памяти
        //Первый параметр - название участка, 
        //второй - длина участка памяти в байтах: тип char  занимает 2 байта 
        //плюс четыре байта для одного объекта типа Integer
        MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen(Name, size * 2 + 4);
        //Создаем объект для записи в разделяемый участок памяти
        using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size * 2 + 4))
        {
            //запись в разделяемую память
            //запись размера с нулевого байта в разделяемой памяти
            writer.Write(0, size);
            //запись сообщения с четвертого байта в разделяемой памяти
            writer.WriteArray<char>(4, message, 0, message.Length);
        }

        Console.WriteLine("Сообщение записано в разделяемую память");
        Console.WriteLine("Для выхода из программы нажмите любую клавишу");
        //Console.ReadLine();
    }

    public string Read()
    {
        //Массив для сообщения из общей памяти
        char[] message;
        //Размер введенного сообщения
        int size;

        //Получение существующего участка разделяемой памяти
        //Параметр - название участка
        MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting(Name);
        //Сначала считываем размер сообщения, чтобы создать массив данного размера
        //Integer занимает 4 байта, начинается с первого байта, поэтому передаем цифры 0 и 4
        using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 4, MemoryMappedFileAccess.Read))
        {
            size = reader.ReadInt32(0);
        }

        //Считываем сообщение, используя полученный выше размер
        //Сообщение - это строка или массив объектов char, каждый из которых занимает два байта
        //Поэтому вторым параметром передаем число символов умножив на из размер в байтах плюс
        //А первый параметр - смещение - 4 байта, которое занимает размер сообщения
        using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(4, size * 2, MemoryMappedFileAccess.Read))
        {
            //Массив символов сообщения
            message = new char[size];
            reader.ReadArray<char>(0, message, 0, size);
        }
        Console.WriteLine("Получено сообщение :");
        Console.WriteLine(message);
        return message.ToString();
    }

}
