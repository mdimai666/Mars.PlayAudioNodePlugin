namespace Mars.PlayAudioNodePlugin.Shared.Dto;

public record OutputDeviceResponse
{
    public required string DeviceId { get; init; }
    public required string DeviceName { get; init; }
    public required string FriendlyName { get; init; }
    public required bool IsDefault { get; init; }
}
