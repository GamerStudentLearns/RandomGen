using System;

public static class RoomEvents
{
    public static Action<Room> OnRoomEntered;
    public static Action<Room> OnRoomCleared;

    public static void RoomEntered(Room room) => OnRoomEntered?.Invoke(room);
    public static void RoomCleared(Room room) => OnRoomCleared?.Invoke(room);
}
