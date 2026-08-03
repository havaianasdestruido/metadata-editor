using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MetaLab.Engine;

public static class EngineProbe
{
    public static int ProbeMetadataSys()
    {
        var hr = NativeBindings.WLXPSGetItemPropertyHandler(IntPtr.Zero);
        return hr;
    }
}
