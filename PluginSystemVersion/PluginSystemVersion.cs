/*
  Copyright (C) 2014 Birunthan Mohanathas

  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU General Public License
  as published by the Free Software Foundation; either version 2
  of the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;
using Ubidots;
using System.Diagnostics;
using Newtonsoft.Json.Linq;


// Overview: This example demonstrates the basic concept of Rainmeter C# plugins.

// Sample skin:
/*
    [Rainmeter]
    Update=1000
    BackgroundMode=2
    SolidColor=000000

    [mString]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=String

    [mMajor]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Major

    [mMinor]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Minor

    [mNumber]
    Measure=Plugin
    Plugin=SystemVersion.dll
    Type=Number

    [Text1]
    Meter=STRING
    MeasureName=mString
    MeasureName2=mMajor
    MeasureName3=mMinor
    MeasureName4=mNumber
    X=5
    Y=5
    W=300
    H=70
    FontColor=FFFFFF
    Text="String: %1#CRLF#Major: %2#CRLF#Minor: %3#CRLF#Number: %4#CRLF#"

    [Text2]
    Meter=STRING
    MeasureName=mString
    MeasureName2=mMajor
    MeasureName3=mMinor
    MeasureName4=mNumber
    NumOfDecimals=1
    X=5
    Y=5R
    W=300
    H=70
    FontColor=FFFFFF
    Text="String: %1#CRLF#Major: %2#CRLF#Minor: %3#CRLF#Number: %4#CRLF#"
*/

namespace PluginSystemVersion
{
    internal class Measure
    {
        enum MeasureType
        {
            Time,
            String
        }
        private MeasureType Type = MeasureType.String;

        static ApiClient Api;
        internal string ApiKey;
        internal string Variable;

        internal Measure()
        {
            //Debugger.Launch();
        }

        internal void Reload(Rainmeter.API rm, ref double maxValue)
        {
            ApiKey = rm.ReadString("ApiKey", string.Empty);
            Variable = rm.ReadString("Variable", string.Empty);

            if (Api == null)
                Api = new ApiClient(ApiKey);

            string type = rm.ReadString("Type", "");
            switch (type.ToLowerInvariant())
            {
                case "time":
                    Type = MeasureType.Time;
                    break;

                case "string":
                    Type = MeasureType.String;
                    break;

                default:
                    API.Log(API.LogType.Error, "SystemVersion.dll: Type=" + type + " not valid");
                    break;
            }
        }

        internal double Update()
        {
            return 0.0;
        }

        internal string GetString()
        {
            try
            {
                var temperature = Api.GetVariable(Variable);
                var attribute = (JObject)temperature.GetAttribute("last_value");
                string temp = attribute["value"].Value<float>().ToString();
                long time = attribute["timestamp"].Value<long>();
                var datetime = UnixTimeStampToDateTime(time);

                switch (Type)
                {
                    case MeasureType.String:
                        return temp.ToString();
                    case MeasureType.Time:
                        return datetime.ToString("G");
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }
    }
}
