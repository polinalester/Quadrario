namespace ProtocolLibrary.Messaging {
    public enum MessageTypes {
        Request = 0,
        Response = 1,
        Event = 2,

        MoveRequest = 10,

        BooleanResponse = 11,

        RenderRequest = 12,

        ServiceHostRequest = 100,
        ServiceHostResponse = 101,

        ServiceProviderRequest = 110,
        ServiceProviderResponse = 111,

        DataFacadeEvent = 200
    }
}