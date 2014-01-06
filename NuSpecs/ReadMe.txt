
######################################################################################

       S I M P L E    L O G G I N G   -   N L O G 
       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^


    This is and NLog package for the SimpleLogging framework.

    This can be used in any .NET application that is .NET 4.0+
    I :heart: using this in Azure worker roles - makes diagnostics so much easier!

    Recommendations: Use this with a GUI NLog viewer - SENTINEL
                                           (http://sentinel.codeplex.com/)


    DEFINING STUFF VIA THE CONSTRUCTOR
    ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

    1. Define everything in the nlog.config file

    // Just using the service without using any specified targets, etc.
    // Eg. all your nLog config stuff is defined in the nlog.config file
    //     so this will just use that.
    var loggingService = new NLogLoggingService("ImageUploadingWorker");
    loggingService.Debug("She says she likes my watch, but she wants Steve's AP.");
    var words = new [] { "hours", "QVC" };
    loggingService.Info("And she stays up {0} watching {1}", words[0], words[1] );
    
    2. Don't use an nlog.config file (because u want to change stuff during runtime)
       OR
       use one (maybe an SMTP Target for errors) but don't define an NLogViewTarget
       which we will do programatically.

    // Send all messages that are Info (or greater/more important) to my 
    // Sentinel app which is running on a computer @ IP addy: 59.12.123.4:9999.
    var loggingService = new NLogLoggingService("ImageUploadingWorker", "udp://59.12.123.4:9999", "info")
    try { .. }
    catch (Exception exception)
    {
        loggingService.Warning("I tell her suspenders and some PVC");
        loggingService.Error(exception, "And then I'll film it on my JVC");
        ...
    }


    NOTE: You can also provide a full instance of an NLog target. Any NLog target.
    


    SET or CHANGE THE NLOGVIEWERTARGET DURING RUNTIME
    ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
    Why would you do this?
    Because you want to change the your app's config settings, apply them
    and -not- have to teardown/restart your app.
    Eg. On Azure, you might want to change the logging levels from Error to Info
        or even change the address from the work pc to your home pc.
    
    var loggingService = new NLogLoggingService("ImageUploaderWorker");
    ....
    var address = CloudConfigurationManager.GetSetting("NLogAddress"); // eg. udp://59.12.123.4:9999
    loggingService.ConfigureNLogViewerTarget(address);
    ....
    loggingService.ConfigureNLogViewerTarget(address, "Info");
    ...
    loggingService.ConfigureNLogViewerTarget(address, "Error");
    ...
    loggingService.ConfigureNLogViewerTarget(address, "Off");



######################################################################################