using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MetaLab.Engine;

public static class VideoMetadataEditor
{
    public static Dictionary<string, string> Load(string path)
    {
        var dict = new Dictionary<string, string>();
        if (NativeBindings.MFReader_Open(path, out var handle) != 0) return dict;
        if (NativeBindings.MFReader_GetProperties(handle, out var propsPtr) == 0)
        {
            // Simplified: assume propsPtr points to a null‑terminated UTF‑8 string "key=value\0" list.
            var raw = Marshal.PtrToStringUTF8(propsPtr);
            if (!string.IsNullOrEmpty(raw))
            {
                foreach (var line in raw.Split('\0'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2) dict[parts[0]] = parts[1];
                }
            }
        }
        NativeBindings.MFReader_Close(handle);
        return dict;
    }

    public static void Save(string path, Dictionary<string, string> updates)
    {
        if (NativeBindings.MFReader_Open(path, out var handle) != 0) return;
        // Build "key=value\0" list
        var sb = new StringBuilder();
        foreach (var kv in updates) sb.Append(kv.Key).Append('=')
                                        .Append(kv.Value).Append('\0');
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        var unmanaged = Marshal.AllocHGlobal(bytes.Length + 1);
        Marshal.Copy(bytes, 0, unmanaged, bytes.Length);
        Marshal.WriteByte(unmanaged, bytes.Length, 0);
        NativeBindings.MFReader_SetProperties(handle, unmanaged);
        NativeBindings.MFReader_Close(handle);
        Marshal.FreeHGlobal(unmanaged);
    }
}
