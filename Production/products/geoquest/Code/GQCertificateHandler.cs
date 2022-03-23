using System;
using System.Security.Cryptography.X509Certificates;
using QM.Util;
using UnityEngine;
using UnityEngine.Networking;

// Generic Version: uses the standard 
// certificate handler of the OS, hence we return null. The implementation itself in method ValidateCertificate() will never be used

public class GQCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return false;
    }

    public static CertificateHandler GetCertificateHandler()
    {
        return null;
    }
}
