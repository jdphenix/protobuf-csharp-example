using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Google.Protobuf;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Phenix.ProtobufConcat.Messages;

/// <summary>
/// I created to project to demonstrate serializing and deserializing
/// protobuf messages to a file. This could easily be adapted to a
/// stream of any type.
/// </summary>
namespace Phenix.Protobufconcat
{
    class Program
    {
        const int count = 1000;

        static void Main(string[] args)
        {

            var randomFirstNameGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName());
            var randomLastNameGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsLastName());
            var randomDateGenerator = RandomizerFactory.GetRandomizer(new FieldOptionsDateTime());

            for (var o = 10; o <= 10000; o *= 10)
            {
                var list = new List<Person>(count);
                var batchCount = count * o;
                Console.Write($"{batchCount} ");
                for (var i = 0; i < batchCount; i++)
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
                Console.WriteLine();
            }

        }


        private static void ReadStreams(List<Person> list, string path, string id)
        {
            // read back from the streams and verify
            using (var fs = File.OpenRead(Path.Combine(path, "wkp", $"{id}.{count}.dat")))
            {
                var timer = Stopwatch.StartNew();
                Console.Write(",read-proto:");
                foreach (var p in list)
                {
                    var personSerialized = Person.Parser.ParseDelimitedFrom(fs);
                    if (!p.Equals(personSerialized))
                    {
                        Console.Error.WriteLine($"Unexpected message not equal, {p}, {personSerialized}");
                    }
                }
                Console.Write($"{timer.ElapsedMilliseconds}");
            }

            using (var fs = File.OpenRead(Path.Combine(path, "wkp", $"{id}.{count}.compressed.dat")))
            using (var cs = new DeflateStream(fs, CompressionMode.Decompress))
            {
                var timer = Stopwatch.StartNew();
                Console.Write(",read-compressed-proto:");
                foreach (var p in list)
                {
                    var personSerialized = Person.Parser.ParseDelimitedFrom(cs);
                    if (!p.Equals(personSerialized))
                    {
                        Console.Error.WriteLine($"Unexpected message not equal, {p}, {personSerialized}");
                    }
                }
                Console.Write($"{timer.ElapsedMilliseconds}");
            }

            using (var fs = File.OpenRead(Path.Combine(path, "wkp", $"{id}.{count}.json")))
            using (var streamReader = new StreamReader(fs))
            {
                var timer = Stopwatch.StartNew();
                Console.Write(",read-json:");
                var listFromFile = JsonSerializer.Deserialize<List<Person>>(streamReader.ReadToEnd());
                for (var i = 0; i < list.Count; i++) if (!list[i].Equals(listFromFile[i])) throw new Exception();
                Console.Write($"{timer.ElapsedMilliseconds}");
            }

            using (var fs = File.OpenRead(Path.Combine(path, "wkp", $"{id}.{count}.compressed.json")))
            using (var cs = new DeflateStream(fs, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(cs))
            {
                var timer = Stopwatch.StartNew();
                Console.Write(",read-compressed-json:");
                var listFromFile = JsonSerializer.Deserialize<List<Person>>(streamReader.ReadToEnd());
                for (var i = 0; i < list.Count; i++) if (!list[i].Equals(listFromFile[i])) throw new Exception();
                Console.Write($"{timer.ElapsedMilliseconds}");
            }
        }

        private static void WriteStreams(List<Person> list, string path, string id)
        {
            var timer = Stopwatch.StartNew();
            // Just a normal stream.
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.{count}.dat")))
            {
                Console.Write(",write-proto:");
                foreach (var p in list)
                {
                    p.WriteDelimitedTo(fs);
                }
                Console.Write($"{timer.ElapsedMilliseconds}");
            }

            // (deflate) compressed stream
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.{count}.compressed.dat")))
            using (var cs = new DeflateStream(fs, CompressionLevel.Optimal))
            {
                timer = Stopwatch.StartNew();
                Console.Write(",write-compressed-proto:");
                foreach (var p in list)
                {
                    p.WriteDelimitedTo(cs);
                }
                Console.Write($"{timer.ElapsedMilliseconds}");
            }

            // Just a normal stream.
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.{count}.json")))
            using (var writer = new StreamWriter(fs))
            {
                timer = Stopwatch.StartNew();
                Console.Write(",write-json:");
                writer.Write(JsonSerializer.Serialize(list));
                Console.Write($"{timer.ElapsedMilliseconds}");
            }

            // (deflate) compressed stream
            using (var fs = File.Create(Path.Combine(path, "wkp", $"{id}.{count}.compressed.json")))
            using (var cs = new DeflateStream(fs, CompressionLevel.Optimal))
            using (var writer = new StreamWriter(cs))
            {
                timer = Stopwatch.StartNew();
                Console.Write(",write-compressed-json:");
                writer.Write(JsonSerializer.Serialize(list));
                Console.Write($"{timer.ElapsedMilliseconds}");
            }
        }

    }
}
