using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SerializeDifferentKnownArrayObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsontext = """
            {
                "_id": "d98343cd-8d21-4d31-a053-5fd1ca9e699b",
                "Name": "Andy",
                "Items": [
                    {
                        "AProp": 43
                    },
                    {
                        "AProp": 43
                    },
                    {
                        "CProp": "Hirni",
                        "CProp2": 42
                    },
                    {
                        "BProp": "Hallo"
                    },
                    {
                        "CProp": "Horst",
                        "CProp2": 41
                    }
                ]
            }
            """;

            var test = JsonConvert.DeserializeObject<Test>(jsontext, new KnownObjectConverter());

            test?.Items.ForEach(i => Console.WriteLine(i.GetType().Name));

            Console.WriteLine();

            Console.WriteLine(JsonConvert.SerializeObject(test, Newtonsoft.Json.Formatting.Indented));
        }

        public class KnownObjectConverter : JsonConverter<IItem>
        {
            public override bool CanWrite => false;

            public override IItem? ReadJson(JsonReader reader, Type objectType, IItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject obj = JObject.Load(reader);

                if (obj.ContainsKey("AProp"))
                {
                    return obj.ToObject<A>();
                }
                if (obj.ContainsKey("BProp"))
                {
                    return obj.ToObject<B>();
                }
                if (obj.ContainsKey("CProp"))
                {
                    return obj.ToObject<C>();
                }
                throw new JsonSerializationException($"Unknown type");
            }
            
            public override void WriteJson(JsonWriter writer, IItem? value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public class Test
        {
            public Guid _id { get; set; }

            public string Name { get; set; }
            public List<IItem> Items { get; set; } = new List<IItem>();
        }

        public interface IItem {}
        public class A : IItem
        {
            public int AProp { get; set; }
        }

        public class B : IItem
        {
            public string? BProp { get; set; }
        }

        public class C : IItem
        {
            public string? CProp { get; set; }
            public int CProp2 { get; set; }
        }
    }
}
