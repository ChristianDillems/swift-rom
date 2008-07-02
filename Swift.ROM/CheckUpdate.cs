using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Swift.ROM
{
	class CheckUpdate
	{
		/// <summary>
		/// 升级状态
		///		0 不需要升级
		///		1 需要升级
		///		2 连接服务器出错
		/// </summary>
		public int state = 0;
		public string nowVersion = null;

		public void checkUpdate()
		{
			Debug.WriteLine("开始检测升级版本。。。");

			try
			{
				// Create a request for the URL. 		
				WebRequest request = WebRequest.Create("http://swift-rom.sourceforge.net/version.txt");
				// If required by the server, set the credentials.
				//request.Credentials = CredentialCache.DefaultCredentials;
				// Get the response.
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				// Display the status.
				//Console.WriteLine(response.StatusDescription);
				// Get the stream containing content returned by the server.
				Stream dataStream = response.GetResponseStream();
				// Open the stream using a StreamReader for easy access.
				StreamReader reader = new StreamReader(dataStream);
				if(reader.ReadToEnd()!=nowVersion)
					state = 1;
				// Read the content.
				//string responseFromServer = reader.ReadToEnd();
				// Display the content.
//				Console.WriteLine(responseFromServer);
				// Cleanup the streams and the response.
				reader.Close();
				dataStream.Close();
				response.Close();

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				state = 2;
			}

			Debug.WriteLine("结束检测升级版本。。。");
		}
	}
}
