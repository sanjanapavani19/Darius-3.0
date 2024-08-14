using System;

namespace StudentDetails
{
    // Define the Student class
    public class Student
    {
        // Public variables
        public int Roll_no;
        public string Name;

        // Method to get details from the user
        public void Get_Details()
        {
            Console.Write("Enter the Roll no: ");
            Roll_no = int.Parse(Console.ReadLine());
            Console.Write("Enter the Name: ");
            Name = Console.ReadLine();
        }

        // Method to print details
        public void Print_Details()
        {
            Console.WriteLine($"Roll No: {Roll_no} Name: {Name}");
        }
    }

    // Main Program class
    class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of the Student class
            Student student = new Student();

            // Call the methods to get and print student details
            student.Get_Details();
            student.Print_Details();

            // Keep the console open
            Console.ReadLine();
        }
    }
}
