﻿using ParametersExtractor.ConsoleApp.Models;
using ParametersExtractor.Core;
using Xunit;

namespace ParametersExtractor.Tests
{
    public sealed class ParametersExtractorTests
    {
        private static ParametersExtractor<User> CreateParametersExtractor(User user)
        {
            return new ParametersExtractor<User>(user);
        }

        [Fact]
        public void Test_01()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Age)
                .ExtractAs("MyAge", x => x.Age)
                .Result();

            Assert.Equal(02, parameters.Count);

            Assert.Equal(21, parameters["Age"]);
            Assert.Equal(21, parameters["MyAge"]);
        }

        [Fact]
        public void Test_02()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Role.Name)
                .ExtractAs("MyName", x => x.Role.Name)
                .Result();

            Assert.Equal(02, parameters.Count);

            Assert.Equal("Student", parameters["Name"]);
            Assert.Equal("Student", parameters["MyName"]);
        }

        [Fact]
        public void Test_03()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Role.Name.ToUpper())
                .Extract(x => x.Role.Name.ToLowerInvariant())
                .Extract(x => x.Age * 2)
                .Result();

            Assert.Empty(parameters);
        }

        [Fact]
        public void Test_04()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .ExtractAsValue(x => x.Role.Name, "Дмитрий")
                .Extract(x => x.Age, x => x.Age)
                .ExtractAs("Double Age", x => x.Age * 2)
                .Result();

            Assert.Equal(03, parameters.Count);

            Assert.Equal("Дмитрий", parameters["Name"]);
            Assert.Equal(21, parameters["Age"]);
            Assert.Equal(42, parameters["Double Age"]);
        }

        [Fact]
        public void Test_05()
        {
            var parameters = CreateParametersExtractor(new User(true))
                .Extract(x => x.Active)
                .ExtractAs("MyActive", x => x.Active == false)
                .Result();

            Assert.Equal(02, parameters.Count);

            Assert.Equal(true, parameters["Active"]);
            Assert.Equal(false, parameters["MyActive"]);
        }

        [Fact]
        public void Test_06()
        {
            string Get(User user) => user.Active ? "block" : "none";

            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Active, Get)
                .Result();

            Assert.Single(parameters);

            Assert.Equal("none", parameters["Active"]);
        }

        [Fact]
        public void Test_07()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Active, x => x.Active ? "block" : "none")
                .Result();

            Assert.Single(parameters);

            Assert.Equal("none", parameters["Active"]);
        }

        [Fact]
        public void Test_08()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Active, onTrue: _ => "block", onFalse: _ => "none")
                .ExtractAs("IsActive", _ => true, onTrue: _ => "block", onFalse: _ => "none")
                .Result();

            Assert.Equal(2, parameters.Count);

            Assert.Equal("none", parameters["Active"]);
            Assert.Equal("block", parameters["IsActive"]);
        }

        [Fact]
        public void Test_09()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Active, onTrue: _ => "-", onFalse: _ => "+")
                .Result();

            Assert.Single(parameters);

            Assert.Equal("+", parameters["Active"]);
        }

        [Fact]
        public void Test_10()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .Extract(x => x.Active, onTrue: _ => "+")
                .Result();

            Assert.Single(parameters);

            Assert.Equal(false, parameters["Active"]);
        }

        [Fact]
        public void Test_11()
        {
            var parameters = CreateParametersExtractor(new User(false))
                .ExtractAs("IsActive", x => x.Active, onFalse: _ => "+")
                .Result();

            Assert.Single(parameters);

            Assert.Equal("+", parameters["IsActive"]);
        }

        [Fact]
        public void Test_12()
        {
            var parameters = CreateParametersExtractor(new User(true))
                .ExtractAs("IsActive", x => x.Active, onTrue: _ => "+")
                .ExtractAsValue(x => x.BirthDate, "12/05/1998")
                .ExtractAsEmpty(x => x.Name)
                .Result();

            Assert.Equal(03, parameters.Count);

            Assert.Equal("+", parameters["IsActive"]);
            Assert.Equal("12/05/1998", parameters["BirthDate"]);

            Assert.Null(parameters["Name"]);
        }
    }
}