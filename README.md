# Wu10Man

Anyone else annoyed by the forced automatic updates on Windows 10?  Me too!  Sadly, some of the basic things like setting a group policy or disabling a service aren't quite enough to get it to stop.  So, I wrote this application to try to get a better way to interrupt what Windows 10 Update is trying to do.

This is broken down into 3 parts: group policy, disabling services, and blocking urls.

## Group Policy

If you have Windows 10 Home, you don't have access to the Group Policy Editor, but you can still set it via the registry.  This application will make those updates for you, because who can remember where those settings are.  This setting should be sufficient to block most automatic updates, but with some security updates, Microsoft will push those down regardless of these settings.

### Group Policy Options:
 - **Enable Automatic Updates:** Allows updates to function as normal.
 - **Disable Automatic Updates:** Disables automatic updates.
 - **Notify of Download and Installation:** Provides notifications for download and install.  Should function similar to older version of Windows that had this option.
 - **Automatic Download, Notify of Installation:** Will automatically download updates, but provide notification before installation.
 
## Disabling Services:

When in doubt, you can disable the Windows 10 services that run the updates.  There are two that seem to control everything: Windows Update Service and Windows Module Installer.  On your own, you can disable them and things will be ok, but Windows has a couple tasks that will turn those back on.  Some are set at an interval, some are set at startup.

You could try to disable those tasks, but I went another router.  When disabling a service through this app, it also sets the service credential to an invalid set changing it from Local System to Local Service.  This stops the services from being restarted.  Re-enabling the service will also change the credentials back to Local System.

## Blocking URLs

There are also a number of URLs that have been identified as being use by Windows Update.  That list is included in the app config file so you can alter it if need be. You can set which URLs to block individually or as a group.  This updates the hosts file at C:\Windows\System32\drivers\etc.

### Admin Access

This application does need administrative access as it is trying to write to the registry, alter services, and change the hosts file.  As a result, some anti-virus software may also pick this up as potentially hostile.

### References

There was a fair amount of research that went into this, but a couple sites stood out as references as I was writing this.

[Windows Update Group Policy Settings](https://support.microsoft.com/en-us/help/328010/how-to-configure-automatic-updates-by-using-group-policy-or-registry-s)
[Windows Update Server List](https://www.tenforums.com/windows-updates-activation/38771-windows-updates-white-list-proxy-server.html)
[Windows Service Authorization](https://stackoverflow.com/questions/17031552/how-do-you-take-file-ownership-with-powershell/17047190#17047190)
[Change Windows Service Password](https://stackoverflow.com/questions/3876787/change-windows-service-password/3877268#3877268)

