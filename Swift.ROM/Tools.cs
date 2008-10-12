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

		#region GetFileSHA1 取回文件SHA1编码
		/// <summary>
		/// 取回文件SHA1编码
		/// </summary>
		/// <param name="fileName">文件路径</param>
		/// <returns>SHA1编码</returns>
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

        public static string[][] GetCRC32(string zipFile)
        {
            string ret = null;
            zipFile = Tools.cra(zipFile);
			char[] param = new char[] { ' '};

			if (zipFile == "") return null;

            //查看传递来的是压缩文件还是独立文件
            switch (Path.GetExtension(zipFile).ToUpper())
            {
				case ".RAR":
					{
						//判断解压程序是否存在
						if (!File.Exists(Application.StartupPath + @"\UnRAR.exe"))
						{
							MessageBox.Show("Not Found --> UnRAR.exe");
							return null;
						}

						Process proc = new Process();
						proc.StartInfo.FileName = Application.StartupPath + @"\UnRAR.exe";
						proc.StartInfo.Arguments = "l \"" + zipFile + "\"";
						proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						proc.StartInfo.UseShellExecute = false;
						proc.StartInfo.RedirectStandardOutput = true;
						proc.StartInfo.CreateNoWindow = true;
						proc.Start();
					//	proc.WaitForExit();

						string[] rs = proc.StandardOutput.ReadToEnd().Split('\n');//param, StringSplitOptions.RemoveEmptyEntries);
						if (proc.ExitCode != 0)
							return null;
						proc.Dispose();

						bool flag = false;
						foreach (string s in rs)
						{
					//		Debug.Write(s);
							if (s.Trim() == "-------------------------------------------------------------------------------")
							{
								if (flag) break; ;
								flag = true;
								continue;
							}
							if (flag)
							{
								string[] ls = s.Split(param, StringSplitOptions.RemoveEmptyEntries);
								if (ls[ls.Length - 4] == ".D.....") continue;	//如果是目录则略过
								//取文件名称
								string filename = null;
								for (int i = 0; i < ls.Length - 9; i++)
									filename += " " + ls[i];
								ret +=zipFile+"?"+ filename.Substring(1) + "\n";
								ret += ls[ls.Length - 9] + "\n";    //取文件大小
								ret += ls[ls.Length - 3] + "\n";    //取crc32
							}
						}
					}
					break;
				case ".ZIP":
				case ".7Z":
					{
						//判断解压程序是否存在
						if (!File.Exists(Application.StartupPath + @"\7za.exe"))
						{
							MessageBox.Show("Not Found --> 7za.exe");
							return null;
						}

						Process proc = new Process();
						proc.StartInfo.FileName = Application.StartupPath + @"\7za.exe";
						proc.StartInfo.Arguments = "l -slt \"" + zipFile + "\"";
						proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
						proc.StartInfo.UseShellExecute = false;
						proc.StartInfo.RedirectStandardOutput = true;
						proc.StartInfo.CreateNoWindow = true;
						proc.Start();
					//	proc.WaitForExit();

						string[] rs = proc.StandardOutput.ReadToEnd().Split('\n');//param, StringSplitOptions.RemoveEmptyEntries);
						if (proc.ExitCode != 0)
							return null;
						proc.Dispose();

						bool flag = false;
						string tfile = null;
						string tsize = null;
						foreach (string s in rs)
						{
				//			Debug.Write(s);
							if (s.Trim() == "----------")
							{
								flag = true;
								continue;
							}
							if (flag)
							{
								if (s.StartsWith("Path = ")) { tfile = zipFile + "?" + s.Substring(7).Trim(); continue; }
								if (s.StartsWith("Size = ")) { tsize = s.Substring(7).Trim(); continue; }
								if (s == "Attributes = D....") { tfile = null; continue; }
								if (s.StartsWith("CRC = ") && tfile != null)
								{
									ret += tfile + "\n" + tsize + "\n" + s.Substring(6).Trim() + "\n";
								}
							}
						}
					}
					break;
                default:
					FileStream fs = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    ret = zipFile+"\n"+fs.Length.ToString()+"\n"+GetFileCRC32(zipFile).ToString("X8")+"\n";
					fs.Close();
                    break;
            }

			if (ret == null) return null;
			
			//创建返回数组
            //  rett[][0] 文件名称
            //  rett[][1] 文件大小
            //  rett[][2] CRC32
            string[] rett = ret.Split('\n');
            string[][] rets = new string[rett.Length / 3][];
            for (int i = 0; i < rett.Length - 1; )
            {
                rets[i / 3] = new string[3];
				rets[i / 3][0] = Tools.car(rett[i++]);
                rets[i / 3][1] = rett[i++];
                rets[i / 3][2] = rett[i++];
            }

            return rets;
        }

        /// <summary>   
        /// 取文件CRC32值 
        /// </summary>   
        /// <param name="sInputFilename">文件名称</param>   
        /// <returns></returns>
        public static ulong GetFileCRC32(string sInputFilename)   
        {   
            ulong[] crc32Table = new ulong[256];  

            const ulong ulPolynomial = 0xEDB88320;   
            ulong dwCrc;   
            int i, j;   
            for (i = 0; i < 256; i++)   
            {   
                dwCrc = (ulong)i;   
                for (j = 8; j > 0; j--)   
                {   
                    if ((dwCrc & 1) == 1)   
                        dwCrc = (dwCrc >> 1) ^ ulPolynomial;   
                    else  
                        dwCrc >>= 1;   
                }   
                crc32Table[i] = dwCrc;   
            }   


            FileStream inFile = new System.IO.FileStream(sInputFilename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] buffer = new byte[inFile.Length];
            inFile.Read(buffer, 0, buffer.Length);   
            inFile.Close();

            ulong ulCRC = 0xffffffff;
            ulong len;
            len = (ulong)buffer.Length;
            for (ulong buffptr = 0; buffptr < len; buffptr++)
            {
                ulong tabPtr = ulCRC & 0xFF;
                tabPtr = tabPtr ^ buffer[buffptr];
                ulCRC = ulCRC >> 8;
                ulCRC = ulCRC ^ crc32Table[tabPtr];
            }
            return ulCRC ^ 0xffffffff;   
        }

        /// <summary>
        /// 指定一个文件
        /// </summary>
        /// <param name="fileName"></param>
		public static void Start(string fileName)
		{
			Process proc = new Process();
			proc.StartInfo.FileName = fileName;
			proc.Start();
		}

		#region unZIP 解压压缩文件
		/// <summary>
		/// 解压压缩文件
		/// </summary>
		/// <param name="rarFile">压缩文件名称</param>
		/// <param name="extPath">展开路径</param>
		public static void unZIP(string zipFile, string extPath)
		{
			Debug.WriteLine("开始解压..." + zipFile);
			Process proc = new Process();

            zipFile = Tools.cra(zipFile);

			switch (Path.GetExtension(zipFile).ToUpper())
			{ 
				case ".RAR":
					//判断解压程序是否存在
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
					//判断解压程序是否存在
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
			Debug.WriteLine("等待解压结束...");
			proc.WaitForExit();
			Debug.WriteLine("解压结束.ExitCode="+proc.ExitCode);
		}
		#endregion

		public static Image GetImage(string fileName)
		{
			byte[] bytes = File.ReadAllBytes(fileName);
			MemoryStream ms = new MemoryStream(bytes);

			return Image.FromStream(ms);
		}

		/// <summary>
		/// 替换&lt;rom>为真实路径
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
        public static string cra(string s)
        {
            return s.Replace("<rom>", Application.StartupPath);
        }

		/// <summary>
		/// 把真实路径替换为&lt;rom>
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
        public static string car(string s)
        {
            return s.Replace(Application.StartupPath, "<rom>");
        }

        public static Bitmap GetIcon(string nds)
        {
            string file = nds;

            //如果为压缩文件，则需要进行解压
            string[] ss = nds.Split('?');
            if (ss.Length > 1)
            {
                Tools.unZIP(ss[0], Application.StartupPath + @"\TEMP\IconROM\");
                file=Application.StartupPath + @"\TEMP\IconROM\" + ss[1];
            }
            
            byte[] tile=new byte[512];
            byte[] palette=new byte[32];

            FileStream fs = new FileStream(file, FileMode.Open);
            
            fs.Seek(0x68, 0);
            byte[] offset = new byte[4];
            fs.Read(offset, 0, 4);
            int os = offset[0] | offset[1] << 8 | offset[2] << 16 | offset[3] <<24;
            fs.Seek(os+0x20,0);
            fs.Read(tile,0, 512);
            fs.Read(palette,0 , 32);

            Color[] rgb32Palette = new Color[16];
            rgb32Palette[0] = Color.Transparent;
            for (int i = 1; i < 16; i++)
            {
                int rgb16 = palette[i*2] | palette[i*2+1] << 8;
                rgb32Palette[i] = Color.FromArgb((rgb16 << 3) & 0xf8, (rgb16 >> 2) & 0xf8, (rgb16 >> 7) & 0xf8);
            }

           Bitmap bmp=new Bitmap(32,32);
           bmp.MakeTransparent();

            int nowPixel = 0;
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 4; col++)
                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < 8; x += 2)
                        {
                            bmp.SetPixel(col * 8 + x + 1, row * 8 + y, rgb32Palette[(tile[nowPixel] & 0xf0) >> 4]);
                            bmp.SetPixel(col * 8 + x + 0, row * 8 + y, rgb32Palette[(tile[nowPixel] & 0x0f) >> 0]);
                            nowPixel++;
                        }

            fs.Close();

            try
            {
                Directory.Delete(Application.StartupPath + "/TEMP/IconROM", true);
            }
            catch { }

            return bmp;
        }

	}
}
