using System;
using System.Security.Cryptography.X509Certificates;
using QM.Util;
using UnityEngine;
using UnityEngine.Networking;

// Spiel.Punkte Version: Accepts all certificates from Let's Encrypt, which we use on our servers.
// We use this only on Android Devices older than Version 7.1 (SDK 25). On other devices we use the standard 
// certificate handler of the OS, hence we return null.

public class GQCertificateHandler : CertificateHandler
{
    private const string LetsEncryptIssuerId = "CN=R3, O=Let's Encrypt, C=US";
   
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        X509Certificate2 certificate = new(certificateData);
        if (LetsEncryptIssuerId.Equals(certificate.IssuerName.Name)) return true;

        return false;
    }

    public static CertificateHandler GetCertificateHandler()
    {
        int androidSDK;
        try
        {
            androidSDK = AndroidVersion.SDK_INT;
            if (androidSDK < 24)
            {
                return new GQCertificateHandler();
            }
            else
            {
                return null;
            }
        }
        catch (Exception)
        {
            // not android:
            return null;
        }

    }
}
