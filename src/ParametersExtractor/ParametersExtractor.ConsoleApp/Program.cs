using System;
using ParametersExtractor.ConsoleApp.Models;
using ParametersExtractor.Core.Implementations;

namespace ParametersExtractor.ConsoleApp
{
    internal class Program
    {
        private static string GetMessage(User user)
        {
            return user.Active
                ? "Пользователь активен"
                : "Пользователь не активен";
        }

        private static void Main()
        {
            var user = new User(true);

            var paramters = new ParametersExtractor<User>(user)
                .Extract("UserName", x => x.Name)
                .Extract("ActiveMessage", GetMessage)
                .Extract("UserAge", x => x.Age)
                .Extract(x => x.BirthDate, x => x.BirthDate.Year)
                .Extract("UserRole", x => x.Role.Name.ToUpper())
                .Extract(x => x.Role.Level)
                .ExtractBoolean(x => x.Active, onTrue: _ => "+", onFalse: _ => "-")
                .Result();

            foreach (var parameter in paramters)
                Console.WriteLine($"{parameter.Key}: {parameter.Value}");

            Console.ReadKey();
        }
    }
}