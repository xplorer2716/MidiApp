/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2024 Pascal Schmitt

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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MidiApp.MidiController.Service
{
    /// <summary>
    /// Iterates the sysex of a given binary stream
    /// </summary>
    public class SysexIterator : IEnumerator<byte[]>, IEnumerable<byte[]>
    {
        // the stream
        private Stream _stream;

        // the stream's content
        private byte[] _data;

        //the current sysex _buffer
        private byte[] _current;

        private int _startPosition;

        /// <summary>
        /// Do not use this ctor
        /// </summary>
        private SysexIterator()
        {
            Debug.Fail("Do not use this ctor");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SysexIterator"/> class.
        /// Assumes that the stream is already opened
        /// </summary>
        /// <param name="stream">The stream.</param>
        public SysexIterator(Stream stream)
        {
            Debug.Assert(stream != null && stream.CanRead);
            _stream = stream;
            _data = new byte[_stream.Length];
            // one single access is faster than a lot. sysex files are not so big
            int byteRead = _stream.Read(_data, 0, (int)_stream.Length);
            Debug.Assert(byteRead == _stream.Length);
        }

        #region IEnumerator<byte[]> Membres

        /// <summary>
        /// Obtient l'élément de la collection situé à la position actuelle de l'énumérateur.
        /// </summary>
        /// <returns>Élément dans la collection à la position actuelle de l'énumérateur.</returns>
        public byte[] Current
        {
            get
            {
                return _current;
            }
        }

        #endregion IEnumerator<byte[]> Membres

        #region IDisposable Membres

        /// <summary>
        /// Exécute les tâches définies par l'application associées à la libération ou à la redéfinition des ressources non managées.
        /// </summary>
        public void Dispose()
        {
            _data = null;
            _stream = null;
        }

        #endregion IDisposable Membres

        #region IEnumerator Membres

        /// <summary>
        /// Obtient l'élément de la collection situé à la position actuelle de l'énumérateur.
        /// </summary>
        /// <returns>Élément dans la collection à la position actuelle de l'énumérateur.</returns>
        object System.Collections.IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// Avance l'énumérateur à l'élément suivant de la collection.
        /// </summary>
        /// <returns>
        /// true si l'énumérateur a pu avancer jusqu'à l'élément suivant ; false si l'énumérateur a dépassé la fin de la collection.
        /// </returns>
        public virtual bool MoveNext()
        {
            bool status = false;

            while (_startPosition < _data.Length && _data[_startPosition] != (byte)Sanford.Multimedia.Midi.SysExType.Start)
            {
                _startPosition++;
            }
            int currentPosition = _startPosition;
            while (currentPosition < _data.Length && _data[currentPosition] != (byte)Sanford.Multimedia.Midi.SysExType.Continuation)
            {
                currentPosition++;
            }

            if (currentPosition < _data.Length && _data[currentPosition] == (byte)Sanford.Multimedia.Midi.SysExType.Continuation && currentPosition > _startPosition)
            {
                int byteCount = currentPosition - _startPosition + 1;
                _current = new byte[byteCount];
                Array.Copy(_data, _startPosition, _current, 0, byteCount);
                _startPosition = currentPosition;
                status = true;
            }
            else
            {
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Rétablit l'énumérateur à sa position initiale, qui précède le premier élément de la collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">La collection a été modifiée après la création de l'énumérateur. </exception>
        public void Reset()
        {
            _startPosition = 0;
            _current = null;
        }

        #endregion IEnumerator Membres

        #region IEnumerable<byte[]> Membres

        /// <summary>
        /// Retourne un énumérateur qui parcourt la collection.
        /// </summary>
        /// <returns>
        ///   <see cref="T:System.Collections.Generic.IEnumerator`1"/> pouvant être utilisé pour parcourir la collection.
        /// </returns>
        public IEnumerator<byte[]> GetEnumerator()
        {
            return this;
        }

        #endregion IEnumerable<byte[]> Membres

        #region IEnumerable Membres

        /// <summary>
        /// Retourne un énumérateur qui itère au sein d'une collection.
        /// </summary>
        /// <returns>
        /// Objet <see cref="T:System.Collections.IEnumerator"/> pouvant être utilisé pour itérer au sein de la collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        #endregion IEnumerable Membres
    }
}