using System;
using System.IO;
using System.Net;

/*********************************************************
Command.cs stores the Http commands that you can send to the R2000

How to call on these programs is discussed at the top of Program.cs

A simple command protocol using HTTP requests (and responses) is provided in order to parametrize and control the
sensor. The HTTP can be accessed using a standard web browser or by establishing temporary TCP/IP connections to
the HTTP port.

The HTTP command protocol provides a simple unified way to control sensor operation for application software. HTTP
commands are used to configure sensor measurement as well as to read and change sensor parameters.

Each HTTP command is constructed as Uniform Resource Identifier

    Structure: 
    http://<sensor IP address>/cmd/<cmd_name>?<argument1=value>&<argument2=value>

    Example: http://169.254.194.202/cmd/request_handle_tcp
    or       http://169.254.194.202/cmd/set_parameter?samples_per_scan=1200

The console feedback can be disabled by commenting out the Console.WriteLine(s) that are in each class


 *********************************************************/
public class Command
{
    //Starting the data stream 
    public void startstream()
    {
        WebRequest Request = WebRequest.Create("http://" + Var.IPaddress + "/cmd/start_scanoutput?handle=" + Var.Handle);
        Console.WriteLine("Starting data stream \r\nSending: http://" + Var.IPaddress + "/cmd/start_scanoutput?handle=" + Var.Handle);
        WebResponse Response = Request.GetResponse();
        using (Stream dataStream = Response.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);
            Var.responseFromR2000 = reader.ReadToEnd();
            //Console.WriteLine("Response: \r\n" + Var.responseFromR2000);
        }
        errorcheck();
    }

    //Stopping data stream 
    public void stopstream()
    {
        WebRequest Request = WebRequest.Create("http://" + Var.IPaddress + "/cmd/stop_scanoutput?handle=" + Var.Handle);
        Console.WriteLine("Stopping data stream \r\nSending: http://" + Var.IPaddress + "/cmd/stop_scanoutput?handle=" + Var.Handle);
        WebResponse Response = Request.GetResponse();
        using (Stream dataStream = Response.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);
            Var.responseFromR2000 = reader.ReadToEnd();
            //Console.WriteLine("Response: \r\n" + Var.responseFromR2000);
        }
        errorcheck();
    }

    //Feeding the watchdog, this needs done ever 60 seconds to keep the connection from closing
    public void watchdog()
    {
        try
        {
        //Feeding the Watchdog
        WebRequest FeedWatchdogRequest = WebRequest.Create("http://" + Var.IPaddress + "/cmd/feed_watchdog?handle=" + Var.Handle);
        WebResponse FeedWatchdogresponse = FeedWatchdogRequest.GetResponse();

        //Console.WriteLine("Feeding Watchdog \r\nSending: http://" + Var.IPaddress + "/cmd/feed_watchdog?handle=" + Var.Handle);
        using (Stream dataStream = FeedWatchdogresponse.GetResponseStream())
            {
            StreamReader reader = new StreamReader(dataStream);
            Var.responseFromR2000 = reader.ReadToEnd();
            //Console.WriteLine("Response: \r\n"+Var.responseFromR2000);
            }
        errorcheck();
        }   
        catch
        {
            Console.WriteLine("Watchdog failure!!!\r\n");
        }
    }

    //Terminating the handle if you didn't need to keep it open, handle will close automatically after 60 seconds if watchdog is not fed.
    public void handlerelease()
    {
        WebRequest Request = WebRequest.Create("http://" + Var.IPaddress + "/cmd/release_handle?handle=" + Var.Handle);
        Console.WriteLine("Releasing handle \r\nSending: http://" + Var.IPaddress + "/cmd/release_handle?handle=" + Var.Handle);
        WebResponse Response = Request.GetResponse();
        using (Stream dataStream = Response.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);
            Var.responseFromR2000 = reader.ReadToEnd();
            //Console.WriteLine("Response: \r\n" + Var.responseFromR2000);
        }
        errorcheck();
    }

    //All parameters from Config.txt are sent to the R2000
    public void setparameters()
    {
        // Parameter setup using the set_parameter function.
        string[] setparameterlist = {"samples_per_scan","scan_direction","scan_frequency","filter_type","filter_width","hmi_display_mode","hmi_static_text_1","hmi_static_text_2"};
        object[] varparameterlist = {Var.SamplesPerScan,Var.ScanDirection,Var.ScanFrequency,Var.FilterType,Var.FilterWidth,Var.HMIDisplayType,Var.HMIDisplayText1,Var.HMIDisplayText2};
        for (int a = 0; a < setparameterlist.GetLength(0); a++ )
        {
            WebRequest Request = WebRequest.Create("http://" + Var.IPaddress + "/cmd/set_parameter?" + setparameterlist[a] + "=" + varparameterlist[a]);
            Console.WriteLine("Setting " + setparameterlist[a] + " = " + varparameterlist[a] + "\r\nSending: http://" + Var.IPaddress + "/cmd/set_parameter?" + setparameterlist[a] + "=" + varparameterlist[a]);
            WebResponse response = Request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                Var.responseFromR2000 = reader.ReadToEnd();
                //Console.WriteLine("Response: \r\n"+Var.responseFromR2000);
            }
            errorcheck();  
        }

        // Parameter setup using the set_scanoutput_config function. 
        string [] setscanouputlist = {"packet_type","start_angle","max_num_points_scan"};
        object[] varscanoutputlist = {Var.ScanDataType,Var.ScanStartAngle,Var.maxnumpointsscan};
        for (int a = 0; a < setscanouputlist.GetLength(0); a++ )
        {
            WebRequest Request = WebRequest.Create("http://" + Var.IPaddress + "/cmd/set_scanoutput_config?handle=" + Var.Handle + "&" + setscanouputlist[a] + "=" + varscanoutputlist[a]);
            Console.WriteLine("Setting " + setscanouputlist[a] + " = " + varscanoutputlist[a] + "\r\nSending: http://" + Var.IPaddress + "/cmd/set_scanoutput_config?handle=" + Var.Handle + "&" + setscanouputlist[a] + "=" + varscanoutputlist[a]);
            WebResponse response = Request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                Var.responseFromR2000 = reader.ReadToEnd();
                //Console.WriteLine("Response: \r\n"+Var.responseFromR2000);
            }
            errorcheck();      
        }
    }

    //Request for all current settings are sent
    public void getparameters()
    {
        WebRequest FactoryResetRequest = WebRequest.Create("http://" + Var.IPaddress+ "/cmd/get_parameter?list=vendor;serial;revision_fw;revision_hw;max_connections;feature_flags;radial_range_min;radial_range_max;radial_resolution;angular_fov;angular_resolution;ip_mode;ip_address;subnet_mask;gateway;scan_frequency;scan_direction;samples_per_scan;scan_frequency_measured;status_flags;load_indication;device_family;mac_address;hmi_display_mode;hmi_language;hmi_button_lock;hmi_parameter_lock;ip_mode_current;ip_address_current;subnet_mask_current;gateway_current;system_time_raw;user_tag;user_notes");
        Console.WriteLine("\r\nParameters and Settings \r\nSending: http://" + Var.IPaddress+ "/cmd/get_parameter?list=vendor;serial;revision_fw;revision_hw;max_connections;feature_flags;radial_range_min;radial_range_max;radial_resolution;angular_fov;angular_resolution;ip_mode;ip_address;subnet_mask;gateway;scan_frequency;scan_direction;samples_per_scan;scan_frequency_measured;status_flags;load_indication;device_family;mac_address;hmi_display_mode;hmi_language;hmi_button_lock;hmi_parameter_lock;ip_mode_current;ip_address_current;subnet_mask_current;gateway_current;system_time_raw;user_tag;user_notes");
        WebResponse FactoryResetResponse = FactoryResetRequest.GetResponse();
        using (Stream dataStream = FactoryResetResponse.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);
            Var.responseFromR2000 = reader.ReadToEnd();
            Console.WriteLine("Response: \r\n" + Var.responseFromR2000);
        }
        errorcheck();
    }

    //Factory reset of the device
    public void factoryreset()
    {
        WebRequest FactoryResetRequest = WebRequest.Create("http://" + Var.IPaddress + "/cmd/factory_reset");
        Console.WriteLine("Factory Resetting R2000 \r\nSending: http://" + Var.IPaddress + "/cmd/factory_reset");
        WebResponse FactoryResetResponse = FactoryResetRequest.GetResponse();
        using (Stream dataStream = FactoryResetResponse.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);
            Var.responseFromR2000 = reader.ReadToEnd();
            //Console.WriteLine("Response: \r\n" + Var.responseFromR2000);
        }
        errorcheck();
    }

    public void errorcheck()
    {
        int num = Var.responseFromR2000.IndexOf("error_code");
        string error = Var.responseFromR2000.Substring(num + 12, 1);
        //Console.WriteLine("error code = " + error);

        if (error == "0")
        {
            //Console.WriteLine("no error to report");
        }
        else
        {
            Console.WriteLine("Response: \r\n" + Var.responseFromR2000);
            Console.WriteLine("Command not successful, exiting system");
            Environment.Exit(0);
        }

            // int num = text.IndexOf(stringcharacteristics[a]);
            // //Console.WriteLine("int num = " + num);
            // string substring = text.Substring(num+stringlength[a], 40);
            // //Console.WriteLine("substring = " + substring);
            // stringvariables[a] = substring.Substring(0,(substring.IndexOf(';')));
            // Console.WriteLine(stringcharacteristics[a] + " = " + stringvariables[a]);

    }








}


