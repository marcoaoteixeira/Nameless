using System.Data;

namespace Nameless.Data.Test.Fixtures {
    public class Animal {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaxAge { get; set; }

        public static Animal Map (IDataRecord record) {
            return new Animal {
                ID = record.GetInt32OrDefault (nameof (ID)).GetValueOrDefault (),
                Name = record.GetStringOrDefault (nameof (Name)),
                MaxAge = record.GetInt32OrDefault (nameof (MaxAge)).GetValueOrDefault ()
            };
        }
    }
}
