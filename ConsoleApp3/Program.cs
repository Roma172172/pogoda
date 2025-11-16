using System;

public class Student
{
    public string firstName;
    public string lastName;
    public int age;
    public string groupName; 

    public void Introduce()
    {
        Console.WriteLine($"Привет, меня зовут {firstName} {lastName}, мне {age} лет. Я в группе {groupName}.");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Student student1 = new Student();
        student1.firstName = "Иван";
        student1.lastName = "Петров";
        student1.age = 19;
        student1.groupName = "Группа1"; 

        Student student2 = new Student();
        student2.firstName = "Анна";
        student2.lastName = "Сидорова";
        student2.age = 20;
        student2.groupName = "Группа2"; 

        Console.WriteLine("--- Знакомство со студентами ---");
        student1.Introduce();
        student2.Introduce();

        Console.ReadLine();
    }
}