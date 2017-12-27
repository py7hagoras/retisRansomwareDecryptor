using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace RANSOMWARE
{
  // Token: 0x02000004 RID: 4
  public class Program
  {
    // Token: 0x06000012 RID: 18
    [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
    public static extern bool ZeroMemory(IntPtr Destination, int Length);

    // Token: 0x06000013 RID: 19 RVA: 0x00002A64 File Offset: 0x00000C64
    private static void intro()
    {
      Console.WriteLine("Decrypted by Chinedu");
    
    }

    // Token: 0x06000014 RID: 20 RVA: 0x00002B0C File Offset: 0x00000D0C
    private static void Main()
    {
      Chiffrement chiffrement = new Chiffrement("m4aP}2a_Jd`H~=k9aML58-ZJwy/j:e5Q", "R<0]W&JCfaD^('FX");
      List<Environment.SpecialFolder> validDirectories = new List<Environment.SpecialFolder>
      {
        Environment.SpecialFolder.Desktop,
        Environment.SpecialFolder.Personal,
        Environment.SpecialFolder.MyPictures
      };
      List<string> validExtensions = new List<string>
      {
        ".txt",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".jpg",
        ".jpeg",
        ".png",
        ".one",
        ".pdf"
      };
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      bool flag = commandLineArgs.Length >= 2;
      if (flag)
      {
        bool flag2 = commandLineArgs[1] == "/decrypt";
        if (flag2)
        {
          Program.intro();
          Console.WriteLine("\nDecryption requested");
          try
          {
            chiffrement.DECHIFFRE(validDirectories);
            chiffrement.DECHIFFRE_DRIVES();
            
          }
          catch (CryptographicException)
          {
            Console.WriteLine("#########################################################");

          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.Message);
          }
        }
        else
        {
          Console.WriteLine("\nUnknown argument");
        }
      }
      else
      {
        try
        {
          Program.intro();
          Console.WriteLine("\nDecryption requested");
          try
          {
            chiffrement.DECHIFFRE(validDirectories);
            chiffrement.DECHIFFRE_DRIVES();
          }
          catch (Exception ex2)
          {
            Console.WriteLine(ex2.Message);
          }
        }
        catch (Exception ex2)
        {
          Console.WriteLine(ex2.Message);
        }
      }
    }
  }
  
  public class Chiffrement
  {
    // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
    public Chiffrement()
    {
      //this.secretKey = Chiffrement.GenerateKey();
    }

    // Token: 0x06000002 RID: 2 RVA: 0x00002070 File Offset: 0x00000270
    public Chiffrement(string key)
    {
      this.secretKey = key;
    }

    // Token: 0x06000003 RID: 3 RVA: 0x0000208C File Offset: 0x0000028C
    public Chiffrement(string key, string vector)
    {
      this.secretKey = key;
      this.vecteurAES = vector;
    }

    // Token: 0x06000004 RID: 4 RVA: 0x000020B0 File Offset: 0x000002B0
    private bool IsFileLocked(FileInfo file)
    {
      FileStream fileStream = null;
      try
      {
        fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
      }
      catch (IOException)
      {
        return true;
      }
      finally
      {
        bool flag = fileStream != null;
        if (flag)
        {
          fileStream.Close();
        }
      }
      return false;
    }

    // Token: 0x06000008 RID: 8 RVA: 0x00002528 File Offset: 0x00000728
    public void DECHIFFRE(List<Environment.SpecialFolder> validDirectories)
    {
      foreach (Environment.SpecialFolder folder in validDirectories)
      {
        string folderPath = Environment.GetFolderPath(folder);
        Console.WriteLine("\n[ " + folderPath + " ]\n");
        try
        {
          List<string> list = new List<string>(Directory.EnumerateFiles(folderPath, "*.crypted", SearchOption.AllDirectories));
          foreach (string text in list)
          {
            this.DecryptFile(text.ToString());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" [DECRYPTED] ");
            Console.ResetColor();
            Console.WriteLine(text.ToString());
          }
        }
        catch (UnauthorizedAccessException)
        {
        }
      }
    }

    // Token: 0x06000009 RID: 9 RVA: 0x00002630 File Offset: 0x00000830
    public void DECHIFFRE_DRIVES()
    {
      DriveInfo[] drives = DriveInfo.GetDrives();
      List<string> list = new List<string>();
      foreach (DriveInfo driveInfo in drives)
      {
        bool isReady = driveInfo.IsReady;
        if (isReady)
        {
          list.Add(driveInfo.Name.ToString());
        }
      }
      list.Remove("C:\\");
      foreach (string text in list)
      {
        Console.WriteLine("\n[ " + text + " ]\n");
        try
        {
          List<string> list2 = new List<string>(Directory.EnumerateFiles(text, "*.crypted", SearchOption.AllDirectories));
          foreach (string text2 in list2)
          {
            this.DecryptFile(text2.ToString());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(" [DECRYPTED] ");
            Console.ResetColor();
            Console.WriteLine(text2.ToString());
          }
        }
        catch (UnauthorizedAccessException)
        {
        }
      }
    }
    
    // Token: 0x0600000B RID: 11 RVA: 0x00002840 File Offset: 0x00000A40
    private void DecryptFile(string sInputFilename)
    {
      FileStream fileStream = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
      byte[] array = new byte[fileStream.Length];
      fileStream.Read(array, 0, array.Length);
      fileStream.Close();
      FileStream fileStream2 = new FileStream(sInputFilename, FileMode.Create, FileAccess.Write);
      ICryptoTransform transform = new AesCryptoServiceProvider
      {
        Key = Encoding.ASCII.GetBytes(this.secretKey),
        IV = Encoding.ASCII.GetBytes(this.vecteurAES)
      }.CreateDecryptor();
      CryptoStream cryptoStream = new CryptoStream(fileStream2, transform, CryptoStreamMode.Write);
      cryptoStream.Write(array, 0, array.Length);
      cryptoStream.Close();
      fileStream2.Close();
      try
      {
        File.Move(sInputFilename, Path.ChangeExtension(sInputFilename, ""));
      }
      catch (IOException)
      {
      }
    }

    // Token: 0x04000001 RID: 1
    private string secretKey;

    // Token: 0x04000002 RID: 2
    private string vecteurAES = "!X5-Tr`)7&xgz|]~";
  }
  
}