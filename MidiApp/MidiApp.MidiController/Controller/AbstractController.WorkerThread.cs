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
using MidiApp.MidiController.Model;
using MidiApp.MidiController.Service;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MidiApp.MidiController.Controller
{
    //this implementation enqueues all the send request and send them back to synth each _SysExTransmitDelay
    //the next value. result for synth is accurate in value but not in time .
    public abstract partial class AbstractController
    {
        /// <summary>
        /// The sysex message queue. Message have to be queued since
        /// we are not sending them in real time (transmit delay).
        /// see ctor for init
        /// </summary>
        private readonly Queue<AbstractParameter> _parameterEntryQueue = null;

        private static readonly int PARAMETER_QUEUE_DEFAULT_CAPACITY = 200;

        // the worker thread for dequeuing messages
        private Thread _workerThread = null;

        /// <summary>
        /// starts the enqueuing/dequeuing worker thread
        /// </summary>
        private void StartWorkerThread()
        {
            if (_workerThread != null)
            {
                StopWorkerThread();
            }
            _workerThread = new Thread(new ThreadStart(WorkerThreadProc));
            _workerThread.Priority = ThreadPriority.Normal;

            lock (_parameterEntryQueue)
            {
                _parameterEntryQueue.Clear();
            }
            _workerThread.Start();
        }

        /// <summary>
        /// starts the dequeuing worker thread
        /// </summary>
        private void StopWorkerThread()
        {
            lock (_parameterEntryQueue)
            {
                _parameterEntryQueue.Clear();
            }
            if (_workerThread != null)
            {
                _workerThread.Abort();
                _workerThread = null;
            }
        }

        protected virtual void WorkerThreadProc()
        {
            try
            {
                while (true)
                {
                    //wait for transmission delay to elapse
                    Thread.Sleep(this._parameterTransmitDelay);

                    // iterate thru each parameter of the edited tone
                    // if value of parmater has changed since last scan, enqueue the parameter to send
                    foreach (DictionaryEntry entry in Tone.ParameterMap)
                    {
                        AbstractParameter parameter = (AbstractParameter)entry.Value;
                        if (parameter.Changed)
                        {
                            // this value is taken in account
                            parameter.Changed = false;
                            // enqueue a copy of the parameter, to keep an image of it's current state
                            // value can change between now and the time when it'll be really sent.
                            AbstractParameter clone = (AbstractParameter)parameter.Clone();
                            EnQueueParameter(clone);
                        }
                    }

                    // dequeue the oldest parameter and send it
                    AbstractParameter paramToSend;
                    if (DequeueParameter(out paramToSend))
                    {
                        if (VerifySynthOutputDevice())
                        {
                            _synthOutputDevice.Send(paramToSend.Message);
                        }
                        //
                    } // dequeue
                } // while
            }//try
            catch (ThreadAbortException e)
            {
                Logger.WriteLine(this, TraceLevel.Info, e.Message);
                //thread abortion (call to StopWorkerThread())
            }
        }

        /// <summary>
        /// returns the next message to send or null if none available
        /// </summary>
        /// <returns></returns>
        protected bool DequeueParameter(out AbstractParameter parameter)
        {
            lock (_parameterEntryQueue)
            {
                if (_parameterEntryQueue.Count == 0)
                {
                    parameter = null;
                    return false;
                }
                else
                {
                    parameter = _parameterEntryQueue.Dequeue();
                    return true;
                }
            }
        }

        protected void EnQueueParameter(AbstractParameter parameter)
        {
            lock (_parameterEntryQueue)
            {
                _parameterEntryQueue.Enqueue(parameter);
            }
        }
    }
}