﻿using System.Security.Cryptography;

namespace Nameless.Infrastructure {
    /// <summary>
    /// Describes the type of a sequential GUID value.
    /// </summary>
    public enum SequentialType {

        /// <summary>
        /// The GUID should be sequential when formatted using the
        /// <see cref="Guid.ToString()" /> method.
        /// </summary>
        AsString,

        /// <summary>
        /// The GUID should be sequential when formatted using the
        /// <see cref="Guid.ToByteArray()" /> method.
        /// </summary>
        AsBinary,

        /// <summary>
        /// The sequential portion of the GUID should be located at the end
        /// of the Data4 block.
        /// Works best for Microsoft SQL Server Database.
        /// </summary>
        AtEnd
    }

    /// <summary>
    /// Contains methods for creating sequential GUID values.
    /// </summary>
    /// <remarks>
    /// Source: https://github.com/jhtodd/SequentialGuid
    /// </remarks>
    public static class SequentialGuid {
        #region Private Static Read-Only Fields

        /// <summary>
        /// Provides cryptographically strong random data for GUID creation.
        /// </summary>
        private static readonly RandomNumberGenerator Generator = RandomNumberGenerator.Create();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Returns a new GUID value which is sequentially ordered when formatted as
        /// a string, a byte array, or ordered by the least significant six bytes of the
        /// Data4 block, as specified by <paramref name="type" />.
        /// </summary>
        /// <param name="type">
        /// Specifies the type of sequential GUID (i.e. whether sequential as a string,
        /// as a byte array, or according to the Data4 block.  This can affect
        /// performance under various database types; see below.
        /// </param>
        /// <param name="useRandomNumberGenerator">
        /// Whether if should use RandomNumberGenerator or Guid.NewGuid().ToByteArray()
        /// to populate the random array. if <c>false</c>, will generate the GUID faster.
        /// </param>
        /// <returns>
        /// A <see cref="Guid" /> structure whose value is created by replacing
        /// certain randomly-generated bytes with a sequential timestamp.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a new GUID value which combines a random component
        /// with the current timestamp, also known as a COMB.  The general concept
        /// is outlined in Jimmy Nilsson's article "The Cost of GUIDs as Primary Keys",
        /// and involves replacing either the least significant or most significant
        /// six bytes of the GUID with the current timestamp.  This reduces the
        /// random component of the GUID from 16 bytes to 10 bytes, but this is
        /// still sufficient to prevent a collision under most real-world circumstances.
        /// </para>
        /// <para>
        /// The purpose of sequential GUIDs is not to promote the use of GUIDs as
        /// sortable entities.  In fact, GUIDs generated very close together may
        /// have the same timestamp and are not guaranteed to be sequentially ordered
        /// at all.  The intent is to increase performance when doing repeated
        /// inserts into database tables that have a clustered index on a GUID
        /// column, so that later entries do not have to be inserted into the middle
        /// of the table, but can simply be appended to the end.
        /// </para>
        /// <para>
        /// According to experiments, Microsoft SQL Server sorts GUID values using
        /// the least significant six bytes of the Data4 block; therefore, GUIDs being
        /// generated for use with SQL Server should pass a <paramref name="type" />
        /// value of <c>SequentialAtEnd</c>.  GUIDs generated for most other database
        /// types should be passed a <paramref name="type" /> value of
        /// <c>SequentialAsString</c> or <c>SequentialAsByteArray</c>.
        /// </para>
        /// <para>
        /// Various standards already define a time-based UUID; however, the
        /// format specified by these standards splits the timestamp into
        /// several components, limiting its usefulness as a sequential ID.
        /// Additionally, the format used for such UUIDs is not compatible
        /// with the GUID ordering on Microsoft SQL Server.
        /// </para>
        /// </remarks>
        public static Guid NewGuid(SequentialType type = SequentialType.AtEnd, bool useRandomNumberGenerator = true) {
            // We start with 16 bytes of cryptographically strong random data.
            var randomBytes = new byte[16];

            // An alternate method: use a normally-created GUID to get our initial
            // random data:
            // byte[] randomBytes = Guid.NewGuid().ToByteArray();
            // This is faster than using RandomNumberGenerator, but I don't
            // recommend it because the .NET Framework makes no guarantee of the
            // randomness of GUID data, and future versions (or different
            // implementations like Mono) might use a different method.
            if (useRandomNumberGenerator) {
                Generator.GetBytes(randomBytes);
            } else {
                randomBytes = Guid.NewGuid().ToByteArray();
            }

            // Now we have the random basis for our GUID.  Next, we need to
            // create the six-byte block which will be our timestamp.

            // We start with the number of milliseconds that have elapsed since
            // DateTime.MinValue.  This will form the timestamp.  There's no use
            // being more specific than milliseconds, since DateTime.Now has
            // limited resolution.

            // Using millisecond resolution for our 48-bit timestamp gives us
            // about 5900 years before the timestamp overflows and cycles.
            // Hopefully this should be sufficient for most purposes. :)
            var timeStamp = DateTime.UtcNow.Ticks / 10000L;

            // Then get the bytes
            var timeStampBytes = BitConverter.GetBytes(timeStamp);

            // Since we're converting from an Int64, we have to reverse on
            // little-endian systems.
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(timeStampBytes);
            }

            var buffer = new byte[16];

            switch (type) {
                case SequentialType.AsString:
                case SequentialType.AsBinary:

                    // For string and byte-array version, we copy the timestamp first, followed
                    // by the random data.
                    Buffer.BlockCopy(
                        src: timeStampBytes,
                        srcOffset: 2,
                        dst: buffer,
                        dstOffset: 0,
                        count: 6
                    );
                    Buffer.BlockCopy(
                        src: randomBytes,
                        srcOffset: 0,
                        dst: buffer,
                        dstOffset: 6,
                        count: 10
                    );

                    // If formatting as a string, we have to compensate for the fact
                    // that .NET regards the Data1 and Data2 block as an Int32 and an Int16,
                    // respectively.  That means that it switches the order on little-endian
                    // systems.  So again, we have to reverse.
                    if (type == SequentialType.AsString && BitConverter.IsLittleEndian) {
                        Array.Reverse(
                            array: buffer,
                            index: 0,
                            length: 4
                        );
                        Array.Reverse(
                            array: buffer,
                            index: 4,
                            length: 2
                        );
                    }

                    break;

                case SequentialType.AtEnd:

                    // For sequential-at-the-end versions, we copy the random data first,
                    // followed by the timestamp.
                    Buffer.BlockCopy(
                        src: randomBytes,
                        srcOffset: 0,
                        dst: buffer,
                        dstOffset: 0,
                        count: 10
                    );
                    Buffer.BlockCopy(
                        src: timeStampBytes,
                        srcOffset: 2,
                        dst: buffer,
                        dstOffset: 10,
                        count: 6
                    );
                    break;
            }

            return new Guid(buffer);
        }

        #endregion
    }
}