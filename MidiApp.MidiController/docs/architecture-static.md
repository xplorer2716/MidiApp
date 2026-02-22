# MidiApp.MidiController — Static Architecture

## Overview

`MidiApp.MidiController` is a reusable **MVC** (Model-View-Controller) framework designed for building real-time MIDI synthesizer editors. It provides base abstractions for managing MIDI devices, tone parameters, persistence (SysEx file read/write) and the user interface.

The project is structured in **4 layers**: Controller, Model, View and Service.

---

## Main Class Diagram

```mermaid
classDiagram
    direction TB

    class AbstractController {
        <<abstract, partial>>
        #AbstractTone Tone
        -StringIntDualDictionary _controlChangeAutomationTable
        -bool _isRunning
        -bool _isSetParameterEnabled
        +ControlChangeAutomationTable : StringIntDualDictionary
        +CurrentProgramNumber : int*
        +ToneName : string
        +IsRunning : bool
        +DisabledControlChangeNumber : int
        +Start()
        +Stop()
        +SetParameter(name, value) : bool
        +GetParameter(name) : AbstractParameter
        +LoadTone(filename, IToneReader)
        +SaveTone(filename, IToneWriter)
        +SetMIDIChannel(channel)
        +RandomizeTone(argument)
        +ExtractSinglePatchesFromAllDataDumpFileToDirectory()*
        +PlayNote(isNoteOn, PianoKeyEventArgs)
        +CloseMidiDevices()
        #CreateToneInstance() : AbstractTone*
        #NotifyAutomationParameterChangeEvent(arg)
    }

    class AbstractController_Events {
        <<partial>>
        +AutomationParameterChangeEvent : EventHandler~ParameterChangeEventArgs~
    }

    class ParameterChangeEventArgs {
        +ParameterName : string
        +Value : int
    }

    class AbstractController_MIDIDevices {
        <<partial>>
        #OutputDevice _synthOutputDevice
        #InputDevice _synthInputDevice
        #InputDevice _automationInputDevice
        -int _parameterTransmitDelay
        +ParameterTransmitDelay : int
        +SetAutomationInputDevice(name) : bool
        +SetSynthInputDevice(name) : bool
        +SetSynthOutputDevice(name) : bool
        #AutomationInputDeviceChannelMessageReceived()
        #SynthInputDeviceSysExMessageReceived()
    }

    class AbstractController_WorkerThread {
        <<partial>>
        -Queue~AbstractParameter~ _parameterEntryQueue
        -Thread _workerThread
        -volatile bool _workerThreadStopRequested
        #IsWorkerThreadStopRequested : bool
        -StartWorkerThread()
        -StopWorkerThread()
        #WorkerThreadProc()
        #EnQueueParameter(param)
        #DequeueParameter(out param) : bool
    }

    class AbstractController_Rules {
        <<partial>>
        -VerifyParameterName(name) : bool
        #VerifySynthOutputDevice() : bool
        #VerifySynthInputDevice() : bool
        -VerifyAutomationInputDevice() : bool
    }

    AbstractController <|-- AbstractController_Events : partial
    AbstractController <|-- AbstractController_MIDIDevices : partial
    AbstractController <|-- AbstractController_WorkerThread : partial
    AbstractController <|-- AbstractController_Rules : partial
    AbstractController_Events *-- ParameterChangeEventArgs
    AbstractController --> AbstractTone : owns
    AbstractController --> StringIntDualDictionary : uses
```

---

## Model Layer

```mermaid
classDiagram
    direction TB

    class AbstractTone {
        <<abstract>>
        -int _midiChannel
        +MIDIChannel : int
        +ToneName : string*
        +ParameterMap : OrderedDictionary*
        #InitializeParameterMap()*
        +RandomizeToneParameters(excluded, ratio)
        +MorphTones(toneA, toneB, result, factor)
        +GetEligibleParametersForToneMorhping() : HashSet~string~
        #DumpParameterMap() : string
    }

    class AbstractParameter {
        <<abstract>>
        #object _lockObject
        +Name : string
        +Label : string
        +MinValue : int
        +MaxValue : int
        +Step : int
        +Value : int
        +Changed : bool
        +Message : SysExMessage*
        #UpdateMessageFromValue()*
        +Clone() : object
    }

    class IToneReader {
        <<interface>>
        +ReadTone(filename, tone)
        +ReadTones(filename) : ICollection~Tuple~
    }

    class IToneWriter {
        <<interface>>
        +WriteTone(filename, tone)
    }

    class ToneException {
        +ToneException()
        +ToneException(message)
        +ToneException(message, inner)
    }

    AbstractTone o-- AbstractParameter : ParameterMap contains *
    AbstractParameter ..|> ICloneable
    AbstractParameter --> SysExMessage : generates
    ToneException --|> NonFatalException
    IToneReader ..> AbstractTone : reads
    IToneWriter ..> AbstractTone : writes
```

---

## View Layer

```mermaid
classDiagram
    direction TB

    class AbstractControllerMainForm {
        <<partial>>
        #string ToneFileName
        #AbstractController Controller*
        #Dictionary~string_Control~ RegisteredControlsMap
        #RegisterForControllerEvents()
        #UnRegisterForControllerEvents()
        #RecursiveRegisterValuedUserControl(control)
    }

    class AbstractControllerMainForm_Events {
        <<partial>>
        #HandleControlValueChanged(paramName, value)
        #HandleControlMouseDown(sender, e)
        #HandleControlMouseUp(sender, e)
        #DoCleanupBeforeClosing()
        #OnAutomationParameterChange(sender, arg)
        #OnClosing(e)
    }

    class TopLevelExceptionHandler {
        <<static>>
        +NonUIThreadExceptionHandler(sender, e)$
        +UIThreadExceptionHandler(sender, e)$
    }

    class TopLevelExceptionForm {
        -Exception _exception
        -string _additionalMessage
    }

    class PianoControlForm {
        WinForms Piano UI
    }

    AbstractControllerMainForm --|> Form
    AbstractControllerMainForm <|-- AbstractControllerMainForm_Events : partial
    AbstractControllerMainForm --> AbstractController : references
    TopLevelExceptionHandler ..> TopLevelExceptionForm : creates
    TopLevelExceptionHandler ..> Logger : uses
```

---

## Service Layer

```mermaid
classDiagram
    direction TB

    class Logger {
        <<static>>
        -TextWriterTraceListener _textWriterListener$
        -FileStream _filestream$
        +AutoFlush : bool$
        +Switch : TraceSwitch$
        +Write(message)$
        +WriteLine(message)$
        +Flush()$
    }

    class BugReportFactory {
        <<static>>
        +CreateBugReport(exception, message, ref errorId) : string$
        +CreateDetailsFromException(exception) : string$
        +CreateEnvironmentInfo() : string$
        +CreateMidiDevicesInfo() : string$
        +CreateScreensInfo(form) : string$
    }

    class AssemblyService {
        <<static>>
        +GetProductName() : string$
        +GetProductNameAndVersionAsString() : string$
        +GetVersionAsString() : string$
    }

    class FileUtils {
        <<static>>
        +SYSEX_FILE_EXTENSION : string$
        +SYSEX_FILE_EXTENSION_WITH_DOT : string$
        +SYSEX_FILE_FILTER : string$
        +MakeUniqueFilenameFromString(name, ext, dir) : string$
    }

    class NonFatalException {
        +NonFatalException()
        +NonFatalException(message)
        +NonFatalException(message, inner)
    }

    class FileMutex {
        -string _fileName
        -string _path
        -FileMutexState _fileMutexState
        +Lock()
        +Unlock()
        +Dispose()
    }

    class SysexIterator {
        -Stream _stream
        -byte[] _data
        -byte[] _current
        +Current : byte[]
        +MoveNext() : bool
        +Reset()
        +Dispose()
    }

    class ExecutionModeService {
        <<static>>
        +IsDesignModeActive : bool$
    }

    NonFatalException --|> Exception
    FileMutex ..|> IDisposable
    SysexIterator ..|> IEnumerator~byte[]~
    SysexIterator ..|> IEnumerable~byte[]~
```

---

## Controller Layer — Support

```mermaid
classDiagram
    direction TB

    class StringIntDualDictionary {
        -Dictionary~int_List~ _intStringListDictionary
        -Dictionary~string_int~ _stringIntListDictionary
        -object _lockObject
        +this[string] : int
        +this[int] : List~string~
        +Add(item)
        +Remove(item) : bool
        +Clear()
        +Count : int
    }

    class StringIntDualEntry {
        +StringValue : string
        +IntValue : int
    }

    class ControlChangeType {
        <<enum>>
        None = -1
        BankSelect
        ModulationWheel
        Volume
        ...~78 MIDI CC values~
    }

    class RandomizeToneArgument {
        +ExcludedParameters : HashSet~string~
        +HumanizeRatio : float?
    }

    class MidiDeviceExceptionExtension {
        <<static>>
        +GetErrorMessageForErrorCode(exception, code) : string$
    }

    class StringExtensions {
        <<static>>
        +ToHtml(text) : string$
    }

    StringIntDualDictionary ..|> ICollection~StringIntDualEntry~
    StringIntDualDictionary ..|> IEnumerator~StringIntDualEntry~
    StringIntDualDictionary o-- StringIntDualEntry
```

---

## File Organization by Layer

```mermaid
graph LR
    subgraph Controller
        AC[AbstractController.cs]
        AC_E[AbstractController.Events.cs]
        AC_M[AbstractController.MIDIDevices.cs]
        AC_W[AbstractController.WorkerThread.cs]
        AC_R[AbstractController.Rules.cs]
        SID[StringIntDualDictionary.cs]
        CCT[ControlChangeType.cs]
        RTA[Arguments/RandomizeToneArgument.cs]
    end

    subgraph Model
        AT[AbstractTone.cs]
        AP[AbstractParameter.cs]
        ITR[IToneReader.cs]
        ITW[IToneWriter.cs]
        TE[ToneException.cs]
    end

    subgraph View
        ACMF[AbstractControllerMainForm.cs]
        ACMF_E[AbstractControllerMainForm.Events.cs]
        TLEH[TopLevelExceptionHandler.cs]
        TLEF[TopLevelExceptionForm.cs]
        PCF[PianoControlForm.cs]
    end

    subgraph Service
        LOG[Logger.cs]
        BRF[BugReportFactory.cs]
        AS[AssemblyService.cs]
        FU[FileUtils.cs]
        NFE[NonFatalException.cs]
        FM[FileMutex.cs]
        SI[SysexIterator.cs]
        SE[StringExtension.cs]
    end

    subgraph Extensions
        MDE[MidiDeviceExceptionExtension.cs]
    end
```

---

## External Dependencies

```mermaid
graph TD
    MidiController["MidiApp.MidiController"]
    SanfordCore["Sanford.Multimedia.Midi.Core"]
    SanfordUI["Sanford.Multimedia.Midi.UI.Windows"]
    UIControls["MidiApp.UIControls"]
    WinForms["System.Windows.Forms"]

    MidiController --> SanfordCore
    MidiController --> SanfordUI
    MidiController --> UIControls
    MidiController --> WinForms

    SanfordCore -.->|provides| InputDevice
    SanfordCore -.->|provides| OutputDevice
    SanfordCore -.->|provides| SysExMessage
    SanfordCore -.->|provides| ChannelMessage
    SanfordUI -.->|provides| PianoKeyEventArgs
    UIControls -.->|provides| IValuedControl
    UIControls -.->|provides| KnobControl
```

---

## Extension Points (abstract classes / interfaces)

| Element | Type | Role | Implemented by (Xplorer project) |
|---|---|---|---|
| `AbstractController` | `abstract class` | MIDI business logic | `XpanderController` |
| `AbstractTone` | `abstract class` | Tone data model | `XpanderTone` |
| `AbstractParameter` | `abstract class` | Individual SysEx parameter | `XpanderParameter` |
| `IToneReader` | `interface` | SysEx file reading | `XpanderToneReader` |
| `IToneWriter` | `interface` | SysEx file writing | `XpanderToneWriter` |
| `AbstractControllerMainForm` | `abstract class` | Editor main window | `MainForm` |
| `CreateToneInstance()` | `abstract method` | Tone factory method | Overridden in `XpanderController` |

---

## Design Patterns Used

| Pattern | Where | Description |
|---|---|---|
| **Template Method** | `AbstractController`, `AbstractTone` | Subclasses implement specific steps |
| **Observer** | `AutomationParameterChangeEvent` | Controller notifies the view of parameter changes |
| **Strategy** | `IToneReader` / `IToneWriter` | Reader/writer injection for persistence |
| **Producer-Consumer** | `WorkerThread` + `Queue<AbstractParameter>` | Thread-safe queue for SysEx transmission |
| **Partial Class** | `AbstractController` (5 files) | Separation of concerns by file |
| **Dual Dictionary** | `StringIntDualDictionary` | Bidirectional lookup CC# ↔ parameter |
| **Clone/Prototype** | `AbstractParameter.Clone()` | Parameter copy for asynchronous sending |
