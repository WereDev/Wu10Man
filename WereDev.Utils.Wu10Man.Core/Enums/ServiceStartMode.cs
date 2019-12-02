namespace WereDev.Utils.Wu10Man.Core.Enums
{
    public enum ServiceStartMode
    {
        // Summary:
        //     Indicates that the service is a device driver started by the system loader. This
        //     value is valid only for device drivers.
        Boot = 0,

        // Summary:
        //     Indicates that the service is a device driver started by the IOInitSystem function.
        //     This value is valid only for device drivers.
        System = 1,

        // Summary:
        //     Indicates that the service is to be started (or was started) by the operating
        //     system, at system start-up. If an automatically started service depends on a
        //     manually started service, the manually started service is also started automatically
        //     at system startup.
        Automatic = 2,

        // Summary:
        //     Indicates that the service is started only manually, by a user (using the Service
        //     Control Manager) or by an application.
        Manual = 3,

        // Summary:
        //     Indicates that the service is disabled, so that it cannot be started by a user
        //     or application.
        Disabled = 4,
    }
}
