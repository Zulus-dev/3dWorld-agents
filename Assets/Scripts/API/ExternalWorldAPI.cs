public static class ExternalWorldAPI
{
    public static void RegisterObject(string id, string type, object snapshot) { }
    public static void PushEvent(WorldEvent e) { }
    public static void PushWorldSnapshot() { }
    // В будущем сюда подключается твой Оркестрант
}