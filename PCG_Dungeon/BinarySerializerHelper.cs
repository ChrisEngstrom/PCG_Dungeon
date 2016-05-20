using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PCG_Dungeon {
    public class BinarySerializerHelper<T> {
        public Type _type;

        public BinarySerializerHelper()
        {
            _type = typeof(T);
        }


        public void Save( string path, object obj ) {
            FileStream fs = new FileStream( path, FileMode.Create );

            // Construct a BinaryFormatter and use it to serialize the data to
            //  the stream
            BinaryFormatter formatter = new BinaryFormatter();
            try {
                formatter.Serialize( fs, obj );
            } catch ( SerializationException exception ) {
                Console.WriteLine( "Failed to serialize. Reason: " + exception.Message );
                throw;
            } finally {
                fs.Close();
            }
        }

        public T Read( string path ) {
            T result;
            FileStream fs = new FileStream( path, FileMode.Open );

            try {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the object from the file and assign the
                //  reference to the local variable
                result = (T)formatter.Deserialize( fs );
            } catch ( SerializationException exception ) {
                Console.WriteLine( "Failed to deserialize. Reason: " + exception.Message );
                throw;
            } finally {
                fs.Close();
            }

            return result;
        }
    }
}
