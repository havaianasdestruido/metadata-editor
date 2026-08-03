using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;

namespace MetaLab.Engine;

public static class ImageMetadataEditor
{
    public static Dictionary<string, string> Load(string path)
    {
        var result = new Dictionary<string, string>();
        var uri = new Uri(path);
        var decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        var metadata = decoder.Frames[0].Metadata as BitmapMetadata;
        if (metadata == null) return result;
        foreach (var key in metadata)
        {
            try { var val = metadata.GetQuery(key) as string; if (val != null) result[key] = val; } catch { }
        }
        return result;
    }

    public static void Save(string path, Dictionary<string, string> updates)
    {
        var uri = new Uri(path);
        var decoder = BitmapDecoder.Create(uri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        var frame = decoder.Frames[0];
        var meta = frame.Metadata as BitmapMetadata;
        if (meta == null) return;
        foreach (var kv in updates)
        {
            try { meta.SetQuery(kv.Key, kv.Value); } catch { }
        }
        var encoder = new JpegBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(frame, null, meta, null));
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        encoder.Save(stream);
    }
}
