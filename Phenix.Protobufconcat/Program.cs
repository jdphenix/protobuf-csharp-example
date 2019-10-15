using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Google.Protobuf;
using Phenix.ProtobufConcat.Messages;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// I created to project to demonstrate serializing and deserializing
/// protobuf messages to a file. This could easily be adapted to a
/// stream of any type.
/// </summary>
namespace Phenix.Protobufconcat
{
    class Program
    {
        static void Main(string[] args)
        {
            var randomFirstNameGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
            var randomLastNameGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsLastName());
            var randomDateGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime());

            const int count = 1000000;
            var list = new List<Person>(count);
            for (var i = 0; i < count; i++)
            {
                var offset = (DateTimeOffset)randomDateGenerator.Generate().Value;

                list.Add(new Person
                {
                    Birthdate = offset.ToUnixTimeSeconds(),
                    LastName = randomLastNameGenerator.Generate(true),
                    FirstName = randomFirstNameGenerator.Generate(true)
                });
            }

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var id = Guid.NewGuid().ToString();

            WriteStreams(list, desktopPath, id);
            ReadStreams(list, desktopPath, id);
        }

        private static void ReadStreams(List<Person> list, string path, string id)
        {
            // read back from the streams and verify
            using (var fs = File.OpenRead(Path.Combine(path, "wkp", $"{id}.dat")))
            {
                foreach (var p in list)
                {
                    var personSerialized = Person.Parser.ParseDelimitedFrom(fs);
                    if (!p.Equals(personSerialized))
                    {
                        Console.Error.WriteLine($"Unexpected message not equal, {p}, {personSerialized}");
                    }
                }
            }

            using (var fs = File.OpenRead(Path.Combine(path, "wkp", $"{id}.compressed.dat")))
            using (var cs = new DeflateStream(fs, CompressionMode.Decompress))
            {
                foreach (var p in list)
                {
                    var personSerialized = Person.Parser.ParseDelimitedFrom(cs);
                    if (!p.Equals(personSerialized))
                    {
                        Console.Error.WriteLine($"Unexpected message not equal, {p}, {personSerialized}");
                    }
                }
            }
        }

        private static void WriteStreams(List<Person> list, string path, string id)
        {
            // Just a normal stream.
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.dat")))
            {
                foreach (var p in list)
                {
                    p.WriteDelimitedTo(fs);
                }
            }

            // (deflate) compressed stream
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.compressed.dat")))
            using (var cs = new DeflateStream(fs, CompressionLevel.Optimal))
            {
                foreach (var p in list)
                {
                    p.WriteDelimitedTo(cs);
                }
            }

            // Just a normal stream.
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.json")))
            using (var writer = new StreamWriter(fs))
            {
                writer.Write(JsonSerializer.Serialize(list));
            }

            // (deflate) compressed stream
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.compressed.json")))
            using (var cs = new DeflateStream(fs, CompressionLevel.Optimal))
            using (var writer = new StreamWriter(cs))
            {
                writer.Write(JsonSerializer.Serialize(list));
            }
        }
    }
}
