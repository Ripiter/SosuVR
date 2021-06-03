using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public class ZipFile : MonoBehaviour
{
    /// <summary>
    /// Write the given bytes data under the given filePath. 
    /// The filePath should be given with its path and filename. (e.g. c:/tmp/test.zip)
    /// </summary>
    public static bool UnZip(string zipFile, string zipUnpackPath, ref string _message)
    {
        try
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(zipFile, zipUnpackPath, System.Text.Encoding.UTF8);
        }
        catch (Exception e)
        {
            _message = "Error while unzipping: " + e.Message;
            return false;
        }
        _message = "Finished unzipping";
        return true;
    }
}
