using FrooxEngine;

namespace FlightDataRecorder.ProtoFluxHelpers;
internal readonly struct ImpulseReceivers(Slot readValue, Slot writeValue) {
    internal readonly Slot ReadValue = readValue;
    internal readonly Slot WriteValue = writeValue;
}
