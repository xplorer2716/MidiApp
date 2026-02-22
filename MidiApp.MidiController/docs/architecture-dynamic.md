# MidiApp.MidiController — Dynamic Architecture

## Overview

This document describes the main dynamic flows of the `MidiApp.MidiController` framework: call sequences during startup, parameter modification, MIDI reception, persistence and error handling.

---

## 1. Application Startup

```mermaid
sequenceDiagram
    participant App as Application
    participant MainForm as AbstractControllerMainForm
    participant Controller as AbstractController
    participant Tone as AbstractTone
    participant WorkerThread as WorkerThread
    participant InputDev as InputDevice
    participant OutputDev as OutputDevice

    App->>MainForm: new MainForm()
    MainForm->>Controller: get Controller (lazy init)
    Controller->>Controller: AbstractController()
    Controller->>Tone: CreateToneInstance()
    Tone->>Tone: InitializeParameterMap()
    Tone-->>Controller: AbstractTone

    App->>MainForm: OnLoad()
    MainForm->>MainForm: RecursiveRegisterValuedUserControl()
    Note right of MainForm: Walks the controls tree,<br/>maps Tag → Control<br/>into RegisteredControlsMap

    MainForm->>MainForm: LoadSettings()
    MainForm->>Controller: SetAutomationInputDevice(name)
    Controller->>InputDev: new InputDevice(id)
    Controller->>Controller: subscribe ChannelMessage, SysEx...
    MainForm->>Controller: SetSynthInputDevice(name)
    MainForm->>Controller: SetSynthOutputDevice(name)
    Controller->>OutputDev: new OutputDevice(id)
    MainForm->>Controller: SetMIDIChannel(channel)

    MainForm->>MainForm: RegisterForControllerEvents()
    MainForm->>Controller: subscribe AutomationParameterChangeEvent

    MainForm->>Controller: Start()
    Controller->>InputDev: StartRecording()
    Controller->>WorkerThread: StartWorkerThread()
    WorkerThread->>WorkerThread: new Thread(WorkerThreadProc)
    WorkerThread->>WorkerThread: Start()
```

---

## 2. Parameter Change from the UI

This is the application's main flow: the user turns a knob, and the change is sent to the synthesizer.

```mermaid
sequenceDiagram
    participant User as User
    participant Knob as KnobControl (UI)
    participant MainForm as MainForm
    participant Controller as AbstractController
    participant Param as AbstractParameter
    participant Queue as Parameter Queue
    participant Worker as WorkerThread
    participant Output as OutputDevice
    participant Synth as Synthesizer

    User->>Knob: Turns the knob
    Knob->>MainForm: AnyValuedControl_ValueChanged()
    MainForm->>MainForm: GetParameterNameForValuedControlTag(tag)
    Note right of MainForm: Page resolution:<br/>ENV_X_ATTACK → ENV_3_ATTACK
    MainForm->>Controller: SetParameter(name, value)
    Controller->>Param: Value = value
    Param->>Param: UpdateMessageFromValue()
    Param->>Param: Changed = true

    MainForm->>MainForm: VfdDisplayHelper.UpdateState(control)

    Note over Worker: WorkerThread runs in a loop
    Worker->>Worker: Sleep(ParameterTransmitDelay)
    Worker->>Param: if Changed == true
    Param-->>Worker: Changed = false
    Worker->>Param: Clone()
    Worker->>Queue: EnQueueParameter(clone)
    Worker->>Queue: DequeueParameter()
    Queue-->>Worker: AbstractParameter
    Worker->>Output: Send(param.Message)
    Output->>Synth: SysEx MIDI message
```

---

## 3. Incoming MIDI CC# (DAW automation → synth)

```mermaid
sequenceDiagram
    participant DAW as DAW / MIDI Controller
    participant AutoInput as AutomationInputDevice
    participant Controller as AbstractController
    participant AutoTable as StringIntDualDictionary
    participant Param as AbstractParameter
    participant Event as AutomationParameterChangeEvent
    participant MainForm as MainForm
    participant UI as IValuedControl (UI)
    participant Output as OutputDevice
    participant Synth as Synthesizer

    DAW->>AutoInput: CC# message
    AutoInput->>Controller: ChannelMessageReceived(e)

    alt CC# found in AutomationTable
        Controller->>AutoTable: [e.Data1] → List<parameterName>
        AutoTable-->>Controller: parameterNames

        loop For each associated parameter
            Controller->>Param: GetParameter(name)
            Controller->>Controller: Auto-scale calculation (0-127 → MinValue-MaxValue)
            Controller->>Controller: SetParameter(name, scaledValue)
            Param->>Param: Changed = true

            Controller->>Event: NotifyAutomationParameterChangeEvent
            Event->>MainForm: OnAutomationParameterChange()
            MainForm->>UI: current.Value = value
            MainForm->>MainForm: VfdDisplayHelper.UpdateState()
        end

        Note over Controller: The WorkerThread will<br/>send the SysEx to the synth
    else CC# not registered
        Controller->>Output: Forward ChannelMessage to synth
        Output->>Synth: CC# MIDI message
    end
```

---

## 4. SysEx Reception from the Synthesizer (patch dump)

```mermaid
sequenceDiagram
    participant Synth as Synthesizer
    participant SynthInput as SynthInputDevice
    participant Controller as AbstractController
    participant Tone as AbstractTone
    participant Param as AbstractParameter
    participant Event as AutomationParameterChangeEvent
    participant MainForm as MainForm
    participant UI as UI Controls

    Synth->>SynthInput: SysEx message (patch data)
    SynthInput->>Controller: SysExMessageReceived(e)
    Note right of Controller: Implemented in the<br/>subclass (XpanderController)

    Controller->>Tone: Parse and update parameters
    loop For each tone parameter
        Controller->>Param: Value = parsedValue
        Controller->>Event: NotifyAutomationParameterChangeEvent
        Event->>MainForm: OnAutomationParameterChange()
        MainForm->>UI: control.Value = value
    end
    MainForm->>MainForm: VfdDisplayHelper.UpdateState()
```

---

## 5. Loading / Saving a SysEx File

```mermaid
sequenceDiagram
    participant User as User
    participant MainForm as MainForm
    participant Controller as AbstractController
    participant Reader as IToneReader
    participant Writer as IToneWriter
    participant Tone as AbstractTone
    participant Iterator as SysexIterator
    participant Event as AutomationParameterChangeEvent
    participant UI as UI Controls

    rect rgb(230, 245, 255)
        Note over User, UI: Load
        User->>MainForm: Open File Dialog → filename
        MainForm->>Controller: LoadTone(filename, reader)
        Controller->>Reader: ReadTone(filename, tone)
        Reader->>Iterator: new SysexIterator(stream)
        Iterator-->>Reader: byte[] sysex data
        Reader->>Tone: Updates ParameterMap

        loop For each parameter
            Controller->>Event: AutomationParameterChangeEvent
            Event->>MainForm: OnAutomationParameterChange()
            MainForm->>UI: control.Value = value
        end
    end

    rect rgb(255, 245, 230)
        Note over User, UI: Save
        User->>MainForm: Save File Dialog → filename
        MainForm->>Controller: SaveTone(filename, writer)
        Controller->>Writer: WriteTone(filename, tone)
        Writer->>Tone: Reads ParameterMap → SysEx bytes
        Writer->>Writer: Writes .syx file
    end
```

---

## 6. WorkerThread Loop (Producer-Consumer)

```mermaid
sequenceDiagram
    participant UI as UI Thread
    participant Param as AbstractParameter
    participant Queue as Queue~AbstractParameter~
    participant Worker as WorkerThread
    participant Output as OutputDevice

    Note over Worker: Loop while(!stopRequested)

    loop Main loop
        Worker->>Worker: Sleep(ParameterTransmitDelay)

        rect rgb(240, 255, 240)
            Note over Worker, Param: SCAN phase
            loop For each Tone parameter
                Worker->>Param: if Changed == true
                alt Parameter modified
                    Param-->>Worker: Changed = false
                    Worker->>Param: Clone()
                    Worker->>Queue: EnQueueParameter(clone)
                    Note right of Queue: lock(_parameterEntryQueue)
                end
            end
        end

        rect rgb(255, 240, 240)
            Note over Worker, Output: SEND phase
            Worker->>Queue: DequeueParameter()
            alt Parameter available
                Queue-->>Worker: AbstractParameter
                Worker->>Output: Send(param.Message)
            end
        end
    end
```

---

## 7. Error Handling — Top Level Exception

```mermaid
sequenceDiagram
    participant App as Application
    participant Handler as TopLevelExceptionHandler
    participant Logger as Logger
    participant BugReport as BugReportFactory
    participant ErrorForm as TopLevelExceptionForm
    participant MainForm as MainForm
    participant Controller as AbstractController

    alt UI thread exception
        App->>Handler: UIThreadExceptionHandler(e)
        Handler->>Logger: WriteLine(Error, details)
        Handler->>BugReport: CreateDetailsFromException(ex)
        Handler->>ErrorForm: new TopLevelExceptionForm(ex, message)
        ErrorForm->>ErrorForm: ShowDialog()
        Note right of ErrorForm: User can continue<br/>working
        Handler->>Logger: Flush()
    else Non-UI thread exception
        App->>Handler: NonUIThreadExceptionHandler(e)
        Handler->>Logger: WriteLine(Error, details)
        Handler->>ErrorForm: new TopLevelExceptionForm(ex, message)
        ErrorForm->>ErrorForm: ShowDialog()
        Note right of ErrorForm: Application will shut down
        Handler->>Logger: Flush()
        loop For each open Form
            Handler->>MainForm: Close() + Dispose()
            MainForm->>MainForm: OnClosing()
            MainForm->>Controller: DoCleanupBeforeClosing()
            Controller->>Controller: Stop()
            Controller->>Controller: CloseMidiDevices()
        end
        Handler->>App: Environment.Exit(0)
    end
```

---

## 8. Tone Randomization

```mermaid
sequenceDiagram
    participant User as User
    participant MainForm as MainForm
    participant Controller as AbstractController
    participant Tone as AbstractTone
    participant Param as AbstractParameter
    participant Event as AutomationParameterChangeEvent
    participant UI as UI Controls

    User->>MainForm: Click "Random"
    MainForm->>Controller: RandomizeTone(argument)
    Controller->>Controller: Stop()
    Controller->>Controller: EnableSetParameter = false
    Note right of Controller: Blocks modifications<br/>during randomization

    Controller->>Tone: RandomizeToneParameters(excluded, ratio)
    loop For each parameter (not excluded)
        Tone->>Param: Value = random(Min, Max)
    end
    Controller->>Tone: ToneName = "RANDOM"

    loop For each parameter
        Controller->>Param: Changed = true
        Controller->>Event: NotifyAutomationParameterChangeEvent
        Event->>MainForm: OnAutomationParameterChange()
        MainForm->>UI: control.Value = value
    end

    Controller->>Controller: EnableSetParameter = true
    Controller->>Controller: Start()
    Note right of Controller: WorkerThread resumes<br/>and will send all modified<br/>parameters to the synth
```

---

## 9. Start / Stop Lifecycle

```mermaid
stateDiagram-v2
    [*] --> Stopped : Constructor
    Stopped --> Running : Start()
    Running --> Stopped : Stop()
    Running --> Running : SetParameter()

    state Running {
        [*] --> InputRecording
        InputRecording --> WorkerActive : StartWorkerThread()
        WorkerActive --> Scanning : Sleep(delay)
        Scanning --> Sending : Parameters changed
        Sending --> Scanning : Loop
    }

    state Stopped {
        [*] --> DevicesIdle
        DevicesIdle --> DevicesClosed : CloseMidiDevices()
        DevicesClosed --> [*]
    }
```

---

## 10. Temporary CC# Automation Disable Flow

```mermaid
sequenceDiagram
    participant User as User
    participant Knob as KnobControl
    participant MainForm as MainForm
    participant Controller as AbstractController
    participant AutoTable as StringIntDualDictionary
    participant DAW as DAW (incoming CC#)

    User->>Knob: MouseDown on the knob
    Knob->>MainForm: HandleControlMouseDown()
    MainForm->>AutoTable: [control.Tag] → CC#
    MainForm->>Controller: DisabledControlChangeNumber = CC#
    Note right of Controller: The corresponding CC#<br/>is temporarily ignored

    User->>Knob: Turns the knob (values sent)

    DAW->>Controller: CC# received
    Controller->>Controller: if Data1 == DisabledCC# → ignore
    Note right of Controller: Prevents conflict between<br/>automation and UI

    User->>Knob: MouseUp
    Knob->>MainForm: HandleControlMouseUp()
    MainForm->>Controller: DisabledControlChangeNumber = None
    Note right of Controller: CC# re-enabled
```

---

## Thread Summary

| Thread | Responsibility | Synchronization Mechanism |
|---|---|---|
| **UI Thread** | Display, user events, MIDI reception (callbacks) | `SynchronizationContext.Post()` |
| **WorkerThread** | Scan modified parameters + send SysEx | `lock(_parameterEntryQueue)` |
| **MIDI Input Callbacks** | Asynchronous MIDI message reception | `lock(_controlChangeAutomationTable)` for `DisabledControlChangeNumber` |

## Thread-Safety Critical Points

- `AbstractParameter`: all properties are protected by `lock(_lockObject)`
- `StringIntDualDictionary`: all operations are protected by `lock(_lockObject)`
- `_parameterEntryQueue`: access protected by `lock` in `EnQueue` / `Dequeue`
- `_workerThreadStopRequested`: marked `volatile` for cross-thread visibility
- `DisabledControlChangeNumber`: protected by `lock(_controlChangeAutomationTable)`
