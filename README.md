# Wu10Man

This started out as a project to shut down the Windows Update mechanic.  It grew from there as Microsoft tweaked how they ran and protected the update service.  It probably would have died there, but it seems people are liking this application.  Version 3.1.0 has over 10k downloads and 4.0.0 over 17k!  So I've felt a little obiligated to keep this updated.

With 4.0, I realized another major annoyance of Windows 10 is a lot of bloatware.  Random apps that either aren't needed or that are added bloatware.

If you've found this a useful app, perhaps you'd like to <a class="bmc-button" target="_blank" href="https://www.buymeacoffee.com/weredev"><span style="margin-left:5px;font-size:28px !important;">buy me a coffee?</span></a>

## Special Thanks
 - Someone<sub>4</sub> - for the super buzzed week, or normally buzzed two weeks!
 - Redfella - kept me caffeinated for a whole month!!
 - piratour20 - for the late evening coffee!
 - Someone<sub>3</sub> - provided a couple good jolts of caffeine!
 - pc305.com - got me a wake-up cup o' joe!
 - John - coffeed me through a long week!
 - Someone<sub>2</sub> - coffeed me through a long week at work!
 - Wes - who coffeed me through hump day!
 - Someone<sub>1</sub> - who bought me a keg of coffee!
 - Govardhan - who bought me a coffee!
 - [Cereal-Killa](https://github.com/Cereal-Killa) - for providing a solution to [Issue #28](https://github.com/WereDev/Wu10Man/issues/28) and for making a "Select All" button.
 - [sungerbob](https://github.com/sungerbob) - for helping with research on [GPO in Windows 10, version 2004](https://github.com/WereDev/Wu10Man/issues/19)
 - [JohnnyTech](https://github.com/JonnyTech) - for contributing to code with some [typo and text fixes](https://github.com/WereDev/Wu10Man/pull/17).

## Disabling Services:

When in doubt, you can disable the Windows 10 services that run the updates.  There are two that seem to control everything: Windows Update Service and Windows Module Installer.  On your own, you can disable them and things will be ok, but Windows has a couple tasks that will turn those back on.  Some are set at an interval, some are set at startup.  There is also a third: Windows Update Medic Service.  This is a new service that Microsoft uses to turn on all the Windows Update stuff.  It looks like this came out with an Oct 2018 update, and as over version 2.0.0, support for disabling the Medic service is also supported.

You could try to disable those tasks, but I went another route.  When disabling a service through this app, it also renames the underlying service file so that it's not possible to run the service.  Previous versions of this changed the running credentials, but I could get whatever security access to the new Medic servie to change that, so I went the file route instead.

If you were running an older version of Wu10Man, don't worry, the new versions will still restore user settings as needed.

## Pausing Updates

A few months ago, Windows added a pause feature to some of the business license of Windows 10.  Recently they added the ability to pause updates as well, but the UI only allows you to go out a few weeks.  I added a screen that allows for a longer pause scenario.  This isn't adding new functionality to Windows, only using the functionality that is built in.  This feature will only work correctly on those versions of Windows that already support Pause/Defer.

## BETA - Declutter
Windows 10 comes with a lot of clutter.  There's the random stuff that they install that aren't particularly necessary, and then there's the 3rd party apps that Windows tries to include.  There's plenty of apps and scripts to remove a bunch of these, but I wanted to add it here as well so that I had a single solution.

This is still sort of in BETA, so definitely use with added caution.


## Legacy
With any application, somethings things just are no longer necessary.  With that, there comes some depricated parts of this application as well.  These parts aren't really supported any more, as much as this application has support.

### Blocking URLs
There are also a number of URLs that have been identified as being use by Windows Update.  That list is included in the app config file so you can alter it if need be. You can set which URLs to block individually or as a group.  This updates the hosts file at C:\Windows\System32\drivers\etc.

Windows Defender likes to block this which is rather annoying.  Also, it's not the best way to actually block Windows updates so I've decided that this goes into the Legacy box unless it turns out to be something that people really really like.


### Group Policy
If you have Windows 10 Home, you don't have access to the Group Policy Editor, but you can still set it via the registry.  This application will make those updates for you, because who can remember where those settings are.  This setting should be sufficient to block most automatic updates, but with some security updates, Microsoft will push those down regardless of these settings.

#### Group Policy Options:
 - **Enable Automatic Updates:** Allows updates to function as normal.
 - **Disable Automatic Updates:** Disables automatic updates.
 - **Notify of Download and Installation:** Provides notifications for download and install.  Should function similar to older version of Windows that had this option.
 - **Automatic Download, Notify of Installation:** Will automatically download updates, but provide notification before installation.

## Additional Info

### System Updates
**Warning!** This program makes changes to your Registry and alters system files. Make sure you have set a System Restore Point before using this software.

### Admin Access

This application does need administrative access as it is trying to write to the registry, alter services, and change the hosts file.  As a result, some anti-virus software may also pick this up as potentially hostile.

### References

There was a fair amount of research that went into this, but a couple sites stood out as references as I was writing this.

- [Windows Update Group Policy Settings](https://support.microsoft.com/en-us/help/328010/how-to-configure-automatic-updates-by-using-group-policy-or-registry-s)
- [Windows Update Server List](https://www.tenforums.com/windows-updates-activation/38771-windows-updates-white-list-proxy-server.html)
- [Windows Service Authorization](https://stackoverflow.com/questions/17031552/how-do-you-take-file-ownership-with-powershell/17047190#17047190)
- [Change Windows Service Password](https://stackoverflow.com/questions/3876787/change-windows-service-password/3877268#3877268)
- [Windows 10, version 1709 basic level Windows diagnostic events and fields](https://docs.microsoft.com/en-us/windows/privacy/basic-level-windows-diagnostic-events-and-fields-1709)
- [Understand the different apps included in Windows 10](https://docs.microsoft.com/en-us/windows/application-management/apps-in-windows-10)
- [Remove-Win10-Apps](https://github.com/Digressive/Remove-Win10-Apps)
- [Windows10Debloater](https://github.com/Sycnex/Windows10Debloater)

## Downloads
[Wu10Man Download](https://github.com/WereDev/Wu10Man/releases)
