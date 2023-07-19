namespace Nameless.IO {
    /// <summary>
    /// Provides a rolling <see cref="Stream"/> for a file.
    /// </summary>
    public sealed class RollingFileStream : FileStream {
        #region Private Constants

        private const string MAXIMUM_FILE_LENGTH_SHOULD_BE_GREATER_THAN_ZERO_MESSAGE = "Maximum file length should be greater than zero.";
        private const string MAXIMUM_FILE_COUNT_SHOULD_BE_GREATER_THAN_ZERO_MESSAGE = "Maximum file count should be greater than zero.";
        private const string BUFFER_SIZE_EXCEEDS_MAXIMUM_FILE_LENGTH_MESSAGE = "Buffer size exceeds maximum file length.";

        private const int DEFAULT_BUFFER_SIZE = 32 * 1024; // 32Kb

        #endregion

        #region Private Read-Only Fields

        private readonly string? _fileDirectory;
        private readonly string _fileNameBase;
        private readonly string _fileExtension;

        #endregion

        #region Private Fields

        private int _nextFileIndex;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the maximum file length.
        /// </summary>
        public long MaximumFileLength { get; }

        /// <summary>
        /// Gets the maximum file count.
        /// </summary>
        public int MaximumFileCount { get; }

        /// <summary>
        /// Gets or sets the possibility to split data.
        /// </summary>
        public bool CanSplitData { get; set; }

        #endregion

        #region Public Override Properties

        /// <inheritdoc />
        public override bool CanRead {
            get { return false; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="RollingFileStream"/>.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="maximumFileLength">Maximum size of the file, in bytes.</param>
        /// <param name="maximumFileCount">Maximum number of files in directory.</param>
        /// <param name="mode">See <see cref="FileMode"/>.</param>
        public RollingFileStream(string path, long maximumFileLength, int maximumFileCount, FileMode mode)
            : this(path, maximumFileLength, maximumFileCount, mode, FileShare.Read, DEFAULT_BUFFER_SIZE, false) { }

        /// <summary>
        /// Initializes a new instance of <see cref="RollingFileStream"/>.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="maximumFileLength">Maximum size of the file, in bytes.</param>
        /// <param name="maximumFileCount">Maximum number of files in directory.</param>
        /// <param name="mode">See <see cref="FileMode"/>.</param>
        /// <param name="share"></param>
        public RollingFileStream(string path, long maximumFileLength, int maximumFileCount, FileMode mode, FileShare share)
            : this(path, maximumFileLength, maximumFileCount, mode, share, DEFAULT_BUFFER_SIZE, false) { }

        /// <summary>
        /// Initializes a new instance of <see cref="RollingFileStream"/>.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="maximumFileLength">Maximum size of the file, in bytes.</param>
        /// <param name="maximumFileCount">Maximum number of files in directory.</param>
        /// <param name="mode">See <see cref="FileMode"/>.</param>
        /// <param name="share"></param>
        /// <param name="bufferSize"></param>
        public RollingFileStream(string path, long maximumFileLength, int maximumFileCount, FileMode mode, FileShare share, int bufferSize)
            : this(path, maximumFileLength, maximumFileCount, mode, share, bufferSize, false) { }

        /// <summary>
        /// Initializes a new instance of <see cref="RollingFileStream"/>.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="maximumFileLength">Maximum size of the file, in bytes.</param>
        /// <param name="maximumFileCount">Maximum number of files in directory.</param>
        /// <param name="mode">See <see cref="FileMode"/>.</param>
        /// <param name="share"></param>
        /// <param name="bufferSize"></param>
        /// <param name="useAsync"></param>
        public RollingFileStream(string path, long maximumFileLength, int maximumFileCount, FileMode mode, FileShare share, int bufferSize, bool useAsync)
            : base(path, FilterFileMode(mode), FileAccess.Write, share, bufferSize, useAsync) {
            if (maximumFileLength <= 0) {
                throw new ArgumentOutOfRangeException(nameof(maximumFileLength), maximumFileLength, MAXIMUM_FILE_LENGTH_SHOULD_BE_GREATER_THAN_ZERO_MESSAGE);
            }

            if (maximumFileCount <= 0) {
                throw new ArgumentOutOfRangeException(nameof(maximumFileCount), maximumFileCount, MAXIMUM_FILE_COUNT_SHOULD_BE_GREATER_THAN_ZERO_MESSAGE);
            }

            MaximumFileLength = maximumFileLength;
            MaximumFileCount = maximumFileCount;

            CanSplitData = true;

            var fullPath = Path.GetFullPath(path);
            _fileDirectory = Path.GetDirectoryName(fullPath);
            _fileNameBase = Path.GetFileNameWithoutExtension(fullPath);
            _fileExtension = Path.GetExtension(fullPath);

            switch (mode) {
                case FileMode.Create:
                case FileMode.CreateNew:
                case FileMode.Truncate:
                    // Delete old files
                    for (var fileCount = 0; fileCount < MaximumFileCount; ++fileCount) {
                        var file = GetRollingFileName(fileCount);
                        if (File.Exists(file)) {
                            File.Delete(file);
                        }
                    }
                    break;

                default:
                    // Position file pointer to the last backup file
                    for (var fileCount = 0; fileCount < MaximumFileCount; ++fileCount) {
                        if (File.Exists(GetRollingFileName(fileCount))) {
                            _nextFileIndex = fileCount + 1;
                        }
                    }

                    if (_nextFileIndex == MaximumFileCount) {
                        _nextFileIndex = 0;
                    }

                    Seek(0, SeekOrigin.End);
                    break;
            }
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc />
        public override void Write(byte[] array, int offset, int count) {
            var actualCount = Math.Min(count, array.GetLength(0));

            if (Position + actualCount <= MaximumFileLength) {
                base.Write(array, offset, count);
                return;
            }

            if (CanSplitData) {
                var partialCount = (int)(Math.Max(MaximumFileLength, Position) - Position);

                base.Write(array, offset, partialCount);

                offset += partialCount;
                count = actualCount - partialCount;
            } else {
                if (count > MaximumFileLength) {
                    throw new ArgumentOutOfRangeException(nameof(count), count, BUFFER_SIZE_EXCEEDS_MAXIMUM_FILE_LENGTH_MESSAGE);
                }
            }

            RollingStream();
            Write(array, offset, count);
        }

        #endregion

        #region Private Static Methods

        private static FileMode FilterFileMode(FileMode mode) => mode == FileMode.Append ? FileMode.OpenOrCreate : mode;

        #endregion

        #region Private Methods

        private string GetRollingFileName(int index) {
            var format = string.Concat("D", MaximumFileCount.ToString().Length);
            var fileName = string.Concat(_fileNameBase
                , index.ToString(format)
                , _fileExtension.Length > 0 ? _fileExtension : string.Empty);

            return Path.Combine(_fileDirectory ?? string.Empty, fileName);
        }

        private void RollingStream() {
            Flush();
            File.Copy(Name, GetRollingFileName(_nextFileIndex), true);
            SetLength(0);

            ++_nextFileIndex;
            if (_nextFileIndex >= MaximumFileCount) {
                _nextFileIndex = 0;
            }
        }

        #endregion
    }
}