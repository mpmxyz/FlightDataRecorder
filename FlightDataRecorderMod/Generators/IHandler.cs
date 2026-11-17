using FrooxEngine;

namespace FlightDataRecorder.Generators;

internal interface IHandler {
    Slot Build(Slot parent);
}