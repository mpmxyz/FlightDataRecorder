using FrooxEngine;

namespace FlightDataRecorder.Generators;
internal readonly struct ImpulseReceivers(Slot readValue, Slot writeValue) {
    internal readonly Slot ReadValue = readValue;
    internal readonly Slot WriteValue = writeValue;
}
