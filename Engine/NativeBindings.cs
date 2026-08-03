using System;
using System.Runtime.InteropServices;

namespace MetaLab.Engine;

public static class NativeBindings
{
    // MetadataSys
    [DllImport("MetadataSys.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern int WLXPSGetItemPropertyHandler(IntPtr pHandler);

    // WLMFReadWrite (decorated names)
    [DllImport("WLMFReadWrite.dll", EntryPoint = "_MFReader_Open@4", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
    public static extern int MFReader_Open(string filePath, out IntPtr handle);

    [DllImport("WLMFReadWrite.dll", EntryPoint = "_MFReader_GetProperties@8", CallingConvention = CallingConvention.StdCall)]
    public static extern int MFReader_GetProperties(IntPtr handle, out IntPtr props);

    [DllImport("WLMFReadWrite.dll", EntryPoint = "_MFReader_SetProperties@8", CallingConvention = CallingConvention.StdCall)]
    public static extern int MFReader_SetProperties(IntPtr handle, IntPtr props);

    [DllImport("WLMFReadWrite.dll", EntryPoint = "_MFReader_Close@4", CallingConvention = CallingConvention.StdCall)]
    public static extern int MFReader_Close(IntPtr handle);
}
