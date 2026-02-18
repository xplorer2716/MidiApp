/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2026 Pascal Schmitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.IO;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// File based mutex. will work in any case, instead of the Mutex class (app restart for ex.)
    /// </summary>
    public class FileMutex : IDisposable
    {
        private readonly object _lockObject = new object();
        private bool _disposed = false;

        private readonly string _fileName;
        private readonly string _path;

        private FileMutexState _fileMutexState;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMutex"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        public FileMutex(string path, string filename)
        {
            Debug.Assert(!String.IsNullOrEmpty(path));
            Debug.Assert(!String.IsNullOrEmpty(filename));
            Debug.Assert(Directory.Exists(path));

            _path = path;
            _fileName = filename;

            _fileMutexState = FileMutexState.UnlockedState;
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        private string FullPath
        {
            get { return Path.Combine(_path, _fileName); }
        }

        /// <summary>
        /// Locks this instance.
        /// </summary>
        public void Lock()
        {
            lock (_lockObject)
            {
                if (!_fileMutexState.IsMutexLocked)
                {
                    FileStream fs = CreateFileStream(FullPath);

                    _fileMutexState = new FileMutexState
                    {
                        IsMutexLocked = true,
                        FileStream = fs,
                    };
                }
                else
                {
                    // already locked
                    Debug.Fail("Already locked !");
                }
            }
        }

        /// <summary>
        /// Unlocks this instance.
        /// </summary>
        public void Unlock()
        {
            lock (_lockObject)
            {
                if (_fileMutexState.IsMutexLocked && _fileMutexState.FileStream != null)
                {
                    CloseFileStream(_fileMutexState.FileStream, FullPath);
                    _fileMutexState = FileMutexState.UnlockedState;
                }
            }
        }

        /// <summary>
        /// Determines whether this instance is locked.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLocked()
        {
            if (!File.Exists(FullPath))
            {
                return false;
            }

            FileStream fs = CreateFileStream(FullPath);
            if (fs == null)
            {
                return true;
            }

            CloseFileStream(fs, FullPath);
            return false;
        }

        /// <summary>
        /// Creates the file stream.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private static FileStream CreateFileStream(string fullPath)
        {
            FileStream fs;
            try
            {
                fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch
            {
                fs = null;
            }

            return fs;
        }

        /// <summary>
        /// Closes the file stream.
        /// </summary>
        /// <param name="fs">The flux fichier.</param>
        /// <param name="fileFullPath">The chemin complet.</param>
        private static void CloseFileStream(FileStream fs, string fileFullPath)
        {
            Debug.Assert(fs != null);
            Debug.Assert(File.Exists(fileFullPath));

            fs.Close();
            fs.Dispose();

            try
            {
                File.Delete(fileFullPath);
            }
            catch
            {
                // no op
            }
        }

        #region IDisposable impl

        /// <summary>
        /// See IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_fileMutexState.IsMutexLocked)
                {
                    Unlock();
                }
            }
            _disposed = true;
        }

        ~FileMutex()
        {
            Dispose(false);
        }

        #endregion IDisposable impl

        /// <summary>
        /// Internal file mutex state
        /// </summary>
        private struct FileMutexState
        {
            /// <summary>
            /// Gets or sets a value indicating whether this instance is mutex locked.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is mutex locked; otherwise, <c>false</c>.
            /// </value>
            public bool IsMutexLocked { get; set; }

            /// <summary>
            /// Gets or sets the file stream.
            /// </summary>
            /// <value>
            /// The file stream.
            /// </value>
            public FileStream FileStream { get; set; }

            /// <summary>
            /// Gets an unlocked state
            /// </summary>
            /// <value>
            /// The state of the unlocked.
            /// </value>
            public static FileMutexState UnlockedState { get { return new FileMutexState { IsMutexLocked = false, FileStream = null }; } }
        }
    }
}