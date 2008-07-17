using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Swift.ROM
{
	class Tools
	{

		#region GetFileSHA1 ȡ���ļ�SHA1����
		/// <summary>
		/// ȡ���ļ�SHA1����
		/// </summary>
		/// <param name="fileName">�ļ�·��</param>
		/// <returns>SHA1����</returns>
		public static string GetFileSHA1(string fileName)
		{
			FileStream fs;
			string sha1 = "";

			try
			{
				fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch
			{
				return null;
			}

			SHA1 sha1m = new SHA1Managed();
			byte[] result = sha1m.ComputeHash(fs);

			fs.Close();

			foreach (byte b in result)
				sha1 += b.ToString("X2");

			return sha1;
		}
		#endregion

		public static void Start(string fileName)
		{
			Process proc = new Process();
			proc.StartInfo.FileName = fileName;
			proc.Start();
		}

		#region unZIP ��ѹѹ���ļ�
		/// <summary>
		/// ��ѹѹ���ļ�
		/// </summary>
		/// <param name="rarFile">ѹ���ļ�����</param>
		/// <param name="extPath">չ��·��</param>
		public static void unZIP(string zipFile, string extPath)
		{
			Debug.WriteLine("��ʼ��ѹ..." + zipFile);
			Process proc = new Process();

			switch (Path.GetExtension(zipFile).ToUpper())
			{ 
				case ".RAR":
					//�жϽ�ѹ�����Ƿ����
					if (!File.Exists(Application.StartupPath + @"\UnRAR.exe"))
					{
						MessageBox.Show("Not Found --> UnRAR.exe");
						return;
					}
					
					proc.StartInfo.FileName = Application.StartupPath + @"\UnRAR.exe";
					proc.StartInfo.Arguments = "e -y -p- \"" + zipFile + "\" \"" + extPath + "\"";
					
					break;
				case ".ZIP":
				case ".7Z":
					//�жϽ�ѹ�����Ƿ����
					if (!File.Exists(Application.StartupPath + @"\7za.exe"))
					{
						MessageBox.Show("Not Found --> 7za.exe");
						return;
					}

					proc.StartInfo.FileName = Application.StartupPath + @"\7za.exe";
					proc.StartInfo.Arguments = "e -y \"" + zipFile + "\" -o\"" + extPath + "\"";
					
					break;
				default:
					MessageBox.Show("Not Support!");
					return;
			}

			proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			Debug.WriteLine(proc.StartInfo.Arguments);
			proc.Start();
			Debug.WriteLine("�ȴ���ѹ����...");
			proc.WaitForExit();
			Debug.WriteLine("��ѹ����.ExitCode="+proc.ExitCode);
		}
		#endregion

		public static Image GetImage(string fileName)
		{
			byte[] bytes = File.ReadAllBytes(fileName);
			MemoryStream ms = new MemoryStream(bytes);

			return Image.FromStream(ms);
		}

	}
}
